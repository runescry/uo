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
    /// Freezing Grasp - Improved cold damage with slow
    /// Deals cold damage and has higher chance to slow target
    /// Circle: 2nd (6 mana)
    /// Reagents: Frostbloom, Winterleaf (Vystia reagents)
    /// </summary>
    public class FreezingGraspSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Freezing Grasp", "Kal Frio Mani",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Second;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FreezingGraspSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents
            if (!HasReagents(typeof(Frostbloom), 1) || !HasReagents(typeof(Winterleaf), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Frostbloom, Winterleaf).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Frostbloom), 1) && ConsumeReagent(typeof(Winterleaf), 1);
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
                            Effects.SendMovingParticles(src, dst, 0x36BD, 7, 0, false, true, 0x481, 0, 9502, 1, 0, EffectLayer.Head, 0x100);
                    });
                }

                // Visual effect - paralyze field animation recolored to ice blue (0x376A with hue 0x481)
                if (dest is Mobile targetMobile)
                {
                    targetMobile.FixedEffect(0x376A, 10, 16, 0x481, 0);
                    Effects.SendLocationParticles(EffectItem.Create(targetMobile.Location, targetMobile.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5048, 0x481, 0, 0);
                }
                Caster.PlaySound(0x204); // Paralyze field sound

                // Calculate damage (8-14 base)
                double damage = Utility.RandomMinMax(8, 14);

                // Apply damage (100% cold damage)
                SpellHelper.Damage(this, dest, damage, 0, 0, 100, 0, 0);

                // Effect animation on target
                if (dest is Mobile mTarget)
                {
                    mTarget.FixedParticles(0x374A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                }

                // Always apply DEX reduction (guaranteed effect)
                if (target is Mobile)
                {
                    Mobile m = (Mobile)target;

                    // Remove any existing slow mod first
                    StatMod existingMod = m.GetStatMod("FreezingGrasp_Slow");
                    if (existingMod != null)
                    {
                        m.RemoveStatMod("FreezingGrasp_Slow");
                        m.CheckStatTimers();
                    }

                    // Apply slow debuff (-20 DEX for 5 seconds)
                    StatMod slowMod = new StatMod(StatType.Dex, "FreezingGrasp_Slow", -20, TimeSpan.FromSeconds(5.0));
                    m.AddStatMod(slowMod);
                    m.CheckStatTimers(); // Force stat recalculation

                    // Visual effect for slow
                    m.FixedParticles(0x374A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                    m.SendMessage(0x3B2, "You have been slowed by the freezing grasp!");

                    // Remove slow effect after duration
                    Timer.DelayCall(TimeSpan.FromSeconds(5.0), () =>
                    {
                        if (m != null && !m.Deleted)
                        {
                            StatMod mod = m.GetStatMod("FreezingGrasp_Slow");
                            if (mod != null)
                            {
                                m.RemoveStatMod("FreezingGrasp_Slow");
                                m.CheckStatTimers(); // Force stat recalculation
                            }
                            m.SendMessage("The freezing grasp effect wears off.");
                        }
                    });
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FreezingGraspSpell m_Owner;

            public InternalTarget(FreezingGraspSpell owner)
                : base(8, false, TargetFlags.Harmful)
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
