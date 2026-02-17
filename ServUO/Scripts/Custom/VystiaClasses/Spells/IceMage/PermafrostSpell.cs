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
    /// Permafrost - Root spell that prevents movement
    /// Roots target in place and deals DoT
    /// Circle: 4th (11 mana)
    /// Reagents: Winterleaf, Glacier Crystal, Permafrost Essence (Vystia reagents)
    /// </summary>
    public class PermafrostSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Permafrost", "An Frio Mort",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public PermafrostSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(Winterleaf), 1) || !HasReagents(typeof(GlacierCrystal), 1) || !HasReagents(typeof(PermafrostEssence), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Winterleaf, Glacier Crystal, Permafrost Essence).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Winterleaf), 1) &&
                   ConsumeReagent(typeof(GlacierCrystal), 1) &&
                   ConsumeReagent(typeof(PermafrostEssence), 1);
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

                // Visual effect - ice at feet
                target.FixedParticles(0x376A, 9, 32, 5030, 0x481, 0, EffectLayer.Waist);
                target.PlaySound(0x1F8);

                target.SendMessage(0x3B2, "Your feet are frozen in place!");
                Caster.SendMessage(0x3B2, "You root your target with permafrost!");

                // Root the target
                target.Frozen = true;

                // Create permafrost effect
                new PermafrostEffect(Caster, target).Start();
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly PermafrostSpell m_Owner;

            public InternalTarget(PermafrostSpell owner) : base(10, false, TargetFlags.Harmful)
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

    /// <summary>
    /// Permafrost DoT effect - roots and damages
    /// </summary>
    public class PermafrostEffect
    {
        private Mobile m_Caster;
        private Mobile m_Target;
        private Timer m_Timer;
        private int m_Ticks;
        private const int MAX_TICKS = 8; // 8 seconds

        public PermafrostEffect(Mobile caster, Mobile target)
        {
            m_Caster = caster;
            m_Target = target;
            m_Ticks = 0;
        }

        public void Start()
        {
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
        }

        private void OnTick()
        {
            if (m_Ticks >= MAX_TICKS || m_Caster == null || m_Caster.Deleted ||
                m_Target == null || m_Target.Deleted || !m_Target.Alive)
            {
                Stop();
                return;
            }

            // Damage per tick (5-10)
            double damage = Utility.RandomMinMax(5, 10);
            AOS.Damage(m_Target, m_Caster, (int)damage, 0, 0, 100, 0, 0);

            // Visual effect
            if (m_Ticks % 2 == 0)
            {
                m_Target.FixedParticles(0x374A, 10, 15, 5021, 0x481, 0, EffectLayer.Waist);
                m_Target.SendMessage(0x3B2, "The permafrost damages you!");
            }

            m_Ticks++;
        }

        private void Stop()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_Target != null && !m_Target.Deleted)
            {
                m_Target.Frozen = false;
                m_Target.SendMessage("The permafrost melts and you can move again.");
            }
        }
    }
}
