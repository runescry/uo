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
    /// Ice Bolt - Basic ice damage spell
    /// Deals cold damage and has chance to slow target
    /// Circle: 3rd (9 mana)
    /// Reagents: Frostbloom, Winterleaf, Glacier Crystal (Vystia reagents)
    /// </summary>
    public class IceBoltSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ice Bolt", "Kal Ort Frio",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public IceBoltSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents
            if (!HasReagents(typeof(Frostbloom), 1) || !HasReagents(typeof(Winterleaf), 1) || !HasReagents(typeof(GlacierCrystal), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Frostbloom, Winterleaf, Glacier Crystal).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Frostbloom), 1) &&
                   ConsumeReagent(typeof(Winterleaf), 1) &&
                   ConsumeReagent(typeof(GlacierCrystal), 1);
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
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(target))
            {
                IDamageable source = Caster;
                IDamageable dest = target;

                SpellHelper.Turn(Caster, target);

                // Spell reflection check
                if (SpellHelper.CheckReflect((int)Circle, ref source, ref dest))
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
                    {
                        if (source is Mobile src && dest is Mobile dst)
                            Effects.SendMovingParticles(src, dst, 0x36E4, 7, 0, false, true, 0x481, 0, 9502, 1, 0, EffectLayer.Head, 0x100);
                    });
                }

                // Calculate damage using ServUO's standard formula
                double damage = GetNewAosDamage(19, 1, 5, target);

                // Visual effect - energy bolt animation (0x379F) with ice blue hue (0x481)
                if (dest is Mobile targetMobile)
                    Caster.MovingParticles(targetMobile, 0x379F, 7, 0, false, true, 3043, 4043, 0x481);
                Caster.PlaySound(0x20A); // Energy bolt sound

                // Apply damage (100% cold damage)
                SpellHelper.Damage(this, dest, damage, 0, 0, 100, 0, 0);

                // 25% chance to apply slow effect
                if (target is Mobile && Utility.RandomDouble() < 0.25)
                {
                    Mobile m = (Mobile)target;

                    // Apply slow debuff (-15 DEX for 5 seconds)
                    StatMod slowMod = new StatMod(StatType.Dex, "IceBolt_Slow", -15, TimeSpan.FromSeconds(5.0));
                    m.AddStatMod(slowMod);

                    // Visual effect for slow
                    m.FixedParticles(0x374A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                    m.SendMessage(0x481, "You have been slowed by icy magic!");

                    // Remove slow effect after duration
                    Timer.DelayCall(TimeSpan.FromSeconds(5.0), () =>
                    {
                        if (m != null && !m.Deleted)
                        {
                            m.SendMessage("The icy slow effect wears off.");
                        }
                    });
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly IceBoltSpell m_Owner;

            public InternalTarget(IceBoltSpell owner)
                : base(12, false, TargetFlags.Harmful)
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
