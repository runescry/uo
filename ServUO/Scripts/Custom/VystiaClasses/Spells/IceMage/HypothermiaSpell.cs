using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Hypothermia - Severe debuff
    /// Reduces max HP, movement speed, and attack speed
    /// Circle: 5th (14 mana)
    /// Reagents: Permafrost Essence, Arctic Pearl (Vystia reagents)
    /// </summary>
    public class HypothermiaSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Hypothermia", "Vas Frio Weaken",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public HypothermiaSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(PermafrostEssence), 1) || !HasReagents(typeof(ArcticPearl), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Permafrost Essence, Arctic Pearl).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(PermafrostEssence), 1) && ConsumeReagent(typeof(ArcticPearl), 1);
        }

        private bool HasReagents(Type type, int amount)
        {
            return (Caster.Backpack != null && Caster.Backpack.GetAmount(type) >= amount);
        }

        private bool ConsumeReagent(Type type, int amount)
        {
            if (Caster.Backpack == null)
                return false;

            return Caster.Backpack.ConsumeTotal(type, amount);
        }

        public override void OnCast()
        {
// Check fizzle and trigger skill gain

            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect - blue debuff aura
                target.FixedParticles(0x374A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x1ED);

                target.SendMessage(0x3B2, "You suffer from severe hypothermia!");

                // Calculate damage based on caster's magery skill (not target HP to avoid huge numbers)
                // Base 30-50 cold damage for a Circle 5 spell
                double damage = 30.0 + (Caster.Skills[GetSchoolSkill()].Value * 0.20);
                if (damage > 0)
                    AOS.Damage(target, Caster, (int)damage, 0, 0, 100, 0, 0);

                // Apply movement and attack speed debuffs via DEX reduction
                // -25% movement speed, -15% attack speed = ~40 DEX reduction
                StatMod dexMod = new StatMod(StatType.Dex, "Hypothermia_Debuff", -40, TimeSpan.FromSeconds(20.0));
                target.AddStatMod(dexMod);

                // Remove debuff after duration
                Timer.DelayCall(TimeSpan.FromSeconds(20.0), () =>
                {
                    if (target != null && !target.Deleted)
                    {
                        target.SendMessage("The hypothermia wears off.");
                        target.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
                    }
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HypothermiaSpell m_Owner;

            public InternalTarget(HypothermiaSpell owner) : base(10, false, TargetFlags.Harmful)
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
