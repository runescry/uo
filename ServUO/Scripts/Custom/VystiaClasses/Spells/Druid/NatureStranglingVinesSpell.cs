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
    /// Strangling Vines - Target is rooted and takes 6-10 damage/tick (6 ticks), -20% attack speed
    /// Circle: 4 (11 mana)
    /// </summary>
    public class NatureStranglingVinesSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Strangling Vines", "Stranglingum Vinesum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureStranglingVinesSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(TreantSap), 1) || !Caster.Backpack.ConsumeTotal(typeof(ElderwoodSeed), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: TreantSap (1), ElderwoodSeed (1)");
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
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Initial visual effect - vines wrapping
                target.FixedParticles(0x375A, 10, 15, 5013, 0x21, 0, EffectLayer.Waist);
                Caster.PlaySound(0x5D1);

                target.SendMessage(0x22, "Strangling vines wrap around you!");
                Caster.SendMessage(0x3B2, "You bind your target with strangling vines!");

                // Apply root effect (frozen)
                target.Frozen = true;

                // Apply -20% attack speed debuff
                target.AddStatMod(new StatMod(StatType.Dex, "StranglingVines_Slow", -20, TimeSpan.FromSeconds(6.0)));

                // Start DoT effect - 6 ticks of 6-10 damage
                new StranglingVinesDoT(Caster, target, 6).Start();
            }

            FinishSequence();
        }

        /// <summary>
        /// Strangling Vines DoT - Deals 6-10 damage per tick, roots target
        /// </summary>
        private class StranglingVinesDoT
        {
            private Mobile m_Caster;
            private Mobile m_Target;
            private Timer m_Timer;
            private int m_TicksRemaining;

            public StranglingVinesDoT(Mobile caster, Mobile target, int ticks)
            {
                m_Caster = caster;
                m_Target = target;
                m_TicksRemaining = ticks;
            }

            public void Start()
            {
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
            }

            private void OnTick()
            {
                if (m_TicksRemaining <= 0 || m_Target == null || m_Target.Deleted || !m_Target.Alive)
                {
                    Stop();
                    return;
                }

                // Deal 6-10 physical damage per tick
                int damage = Utility.RandomMinMax(6, 10);

                if (m_Caster != null && !m_Caster.Deleted)
                    m_Caster.DoHarmful(m_Target);

                AOS.Damage(m_Target, m_Caster, damage, 100, 0, 0, 0, 0);

                // Visual effect every tick
                m_Target.FixedParticles(0x375A, 5, 10, 5013, 0x21, 0, EffectLayer.Waist);
                m_Target.SendMessage(0x22, $"[Strangling Vines] {damage} damage! ({m_TicksRemaining - 1} ticks remaining)");

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                {
                    // Remove root
                    if (m_Target != null && !m_Target.Deleted)
                    {
                        m_Target.Frozen = false;
                        m_Target.SendMessage(0x3B2, "The strangling vines wither away.");
                        m_Target.FixedParticles(0x3735, 1, 30, 9966, 0x21, 0, EffectLayer.Waist);
                    }
                    Stop();
                }
            }

            private void Stop()
            {
                if (m_Target != null && !m_Target.Deleted)
                    m_Target.Frozen = false;

                if (m_Timer != null)
                {
                    m_Timer.Stop();
                    m_Timer = null;
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly NatureStranglingVinesSpell m_Owner;

            public InternalTarget(NatureStranglingVinesSpell owner)
                : base(10, false, TargetFlags.Harmful)
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
