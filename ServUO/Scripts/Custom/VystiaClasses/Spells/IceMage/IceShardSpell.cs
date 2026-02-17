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
    /// Ice Shard - Basic ice projectile attack
    /// Deals cold damage at range
    /// Circle: 1st (4 mana)
    /// Reagents: Frostbloom (Vystia reagent)
    /// </summary>
    public class IceShardSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ice Shard", "Glacius Sagitta",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.First;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public IceShardSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents
            if (!HasReagents(typeof(Frostbloom), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagent (Frostbloom).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Frostbloom), 1);
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
                            Effects.SendMovingParticles(src, dst, 0x36E4, 5, 0, false, true, 0x481, 0, 9502, 1, 0, EffectLayer.Head, 0x100);
                    });
                }

                // Visual effect - small ice projectile (ice blue hue 0x481)
                if (dest is Mobile targetMobile)
                    Effects.SendMovingParticles(Caster, targetMobile, 0x36E4, 5, 0, false, true, 0x481, 0, 9502, 1, 0, EffectLayer.Head, 0x100);
                Caster.PlaySound(0x64F);

                // Calculate damage (5-10 base)
                double damage = Utility.RandomMinMax(5, 10);

                // Apply damage (100% cold damage)
                SpellHelper.Damage(this, dest, damage, 0, 0, 100, 0, 0);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly IceShardSpell m_Owner;

            public InternalTarget(IceShardSpell owner)
                : base(10, false, TargetFlags.Harmful)
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
