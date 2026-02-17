using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Nature
{
    /// <summary>
    /// Nature's Blessing - +10% max HP, +5 HP regen/tick, +10 Poison Resist
    /// Circle: 3 (9 mana)
    /// </summary>
    public class NatureNaturesBlessingSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Nature's Blessing", "Nature'sum Blessingum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureNaturesBlessingSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(DruidBark), 1) || !Caster.Backpack.ConsumeTotal(typeof(TreantSap), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DruidBark (1), TreantSap (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckBSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect - nature blessing aura
                target.FixedParticles(0x375A, 10, 30, 5013, 0x21, 0, EffectLayer.Waist);
                target.PlaySound(0x1F2);

                // Calculate duration (3-6 minutes)
                double duration = 3.0 + (Caster.Skills.Magery.Value / 40.0);

                // +10 Poison Resistance
                ResistanceMod poisMod = new ResistanceMod(ResistanceType.Poison, 10);
                target.AddResistanceMod(poisMod);

                // Store original max hits for HP increase (approximate with STR boost)
                // +10% max HP approximated as +10 STR
                target.AddStatMod(new StatMod(StatType.Str, "NaturesBlessing_HP", 10, TimeSpan.FromMinutes(duration)));

                target.SendMessage(0x3B2, "Nature's blessing empowers you! (+10% HP, +5 HP regen, +10 Poison Resist)");
                if (target != Caster)
                    Caster.SendMessage(0x3B2, "You grant nature's blessing!");

                // HP regeneration timer (5 HP every 2 seconds)
                new NaturesBlessingHealTimer(Caster, target, duration).Start();

                // Remove resistance mod after duration
                Timer.DelayCall(TimeSpan.FromMinutes(duration), () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        target.RemoveResistanceMod(poisMod);
                        target.SendMessage("Nature's blessing fades.");
                    }
                });
            }

            FinishSequence();
        }

        private class NaturesBlessingHealTimer : Timer
        {
            private Mobile m_Caster;
            private Mobile m_Target;
            private DateTime m_EndTime;

            public NaturesBlessingHealTimer(Mobile caster, Mobile target, double durationMinutes)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                m_Caster = caster;
                m_Target = target;
                m_EndTime = DateTime.UtcNow + TimeSpan.FromMinutes(durationMinutes);
            }

            protected override void OnTick()
            {
                if (m_Target == null || m_Target.Deleted || !m_Target.Alive || DateTime.UtcNow >= m_EndTime)
                {
                    Stop();
                    return;
                }

                // Heal 5 HP per tick
                int heal = 5;
                m_Target.Heal(heal);
                m_Target.FixedParticles(0x376A, 1, 10, 5016, 0x21, 0, EffectLayer.Waist);
            }
        }

        private class InternalTarget : Target
        {
            private readonly NatureNaturesBlessingSpell m_Owner;

            public InternalTarget(NatureNaturesBlessingSpell owner)
                : base(10, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
