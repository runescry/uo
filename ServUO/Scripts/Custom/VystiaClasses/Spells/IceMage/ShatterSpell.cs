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
    /// Shatter - Combo spell with bonus vs slowed/frozen
    /// Deals cold damage with +50% bonus if target is slowed or frozen
    /// Circle: 5th (14 mana)
    /// Reagents: Permafrost Essence, Glacier Crystal (Vystia reagents)
    /// </summary>
    public class ShatterSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Shatter", "Ort Frio Vas",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public ShatterSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(PermafrostEssence), 1) || !HasReagents(typeof(GlacierCrystal), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Permafrost Essence, Glacier Crystal).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(PermafrostEssence), 1) && ConsumeReagent(typeof(GlacierCrystal), 1);
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

        public void Target(IDamageable target)
        {
            if (!Caster.CanSee(target))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                // Visual effect - shattering ice
                Caster.MovingParticles(target, 0x36BD, 7, 0, false, true, 0x481, 0, 0x1FB);
                Caster.PlaySound(0x1FB);

                // Base damage (20-35)
                double damage = Utility.RandomMinMax(20, 35);

                // Check if target is slowed or frozen
                bool isSlowed = false;
                if (target is Mobile)
                {
                    Mobile m = (Mobile)target;
                    isSlowed = m.Frozen || CheckForSlowDebuff(m);
                }

                // Apply +50% damage bonus if slowed/frozen
                if (isSlowed)
                {
                    damage *= 1.5;
                    Caster.SendMessage(0x3B2, "Your shatter spell shatters the frozen enemy!");

                    if (target is Mobile)
                        ((Mobile)target).FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
                }

                SpellHelper.Damage(this, target, damage, 0, 0, 100, 0, 0);
            }

            FinishSequence();
        }

        private bool CheckForSlowDebuff(Mobile m)
        {
            // Check for any ice magic slow effects
            StatMod mod = m.GetStatMod("IceBolt_Slow");
            if (mod != null) return true;

            mod = m.GetStatMod("FrostTouch_Slow");
            if (mod != null) return true;

            mod = m.GetStatMod("FreezingGrasp_Slow");
            if (mod != null) return true;

            mod = m.GetStatMod("FrostSlick_Slow");
            if (mod != null) return true;

            mod = m.GetStatMod("FrozenGround_Slow");
            if (mod != null) return true;

            return false;
        }

        private class InternalTarget : Target
        {
            private readonly ShatterSpell m_Owner;

            public InternalTarget(ShatterSpell owner) : base(12, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                    m_Owner.Target((IDamageable)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
