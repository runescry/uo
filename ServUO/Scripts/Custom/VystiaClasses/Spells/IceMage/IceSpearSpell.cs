using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Ice Spear - Piercing ice projectile that hits multiple targets
    /// Deals heavy cold damage and can pierce through up to 3 enemies in a line
    /// Circle: 3rd (9 mana)
    /// Reagents: Frostbloom, Winterleaf, Glacier Crystal (Vystia reagents)
    /// </summary>
    public class IceSpearSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ice Spear", "Vas Frio Sagitta",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public IceSpearSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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
                SpellHelper.Turn(Caster, target);

                // Visual effect - Energy Bolt animation (0x379F) but ice colored (0x481) as requested
                if (target is Mobile targetMobile)
                    Effects.SendMovingParticles(Caster, targetMobile, 0x379F, 7, 0, false, true, 0x481, 0, 9502, 1, 0, EffectLayer.Head, 0x100);
                Caster.PlaySound(0x1FB);

                // Find targets in line from caster to primary target
                List<IDamageable> targets = GetTargetsInLine(Caster, target, 14, 3);

                int hitCount = 0;
                foreach (IDamageable dest in targets)
                {
                    if (hitCount >= 3)
                        break;

                    // Calculate damage (25-40 base)
                    double damage = Utility.RandomMinMax(25, 40);

                    // Apply damage (100% cold damage)
                    SpellHelper.Damage(this, dest, damage, 0, 0, 100, 0, 0);

                    // Visual effect on each hit
                    dest.FixedParticles(0x36CB, 1, 9, 9999, 0x481, 0, EffectLayer.Head);
                    dest.PlaySound(0x1FB);

                    hitCount++;
                }

                if (hitCount > 1)
                    Caster.SendMessage(0x3B2, String.Format("Your ice spear pierces through {0} enemies!", hitCount));
            }

            FinishSequence();
        }

        private List<IDamageable> GetTargetsInLine(Mobile from, IDamageable primary, int range, int maxTargets)
        {
            List<IDamageable> list = new List<IDamageable>();

            if (primary == null || from.Map == null)
                return list;

            // Add primary target first
            list.Add(primary);

            // Get direction from caster to primary target
            Point3D fromLoc = from.Location;
            Point3D toLoc = primary is Mobile ? ((Mobile)primary).Location : ((Item)primary).Location;

            // Calculate direction
            int dx = toLoc.X - fromLoc.X;
            int dy = toLoc.Y - fromLoc.Y;

            // Normalize direction
            double length = Math.Sqrt(dx * dx + dy * dy);
            if (length == 0)
                return list;

            double ndx = dx / length;
            double ndy = dy / length;

            // Find additional targets along the line
            IPooledEnumerable eable = from.Map.GetMobilesInRange(fromLoc, range);

            foreach (Mobile m in eable)
            {
                if (list.Count >= maxTargets)
                    break;

                if (m == from || m == primary || !m.Alive || !from.CanBeHarmful(m, false))
                    continue;

                // Check if mobile is roughly on the line path
                int mdx = m.X - fromLoc.X;
                int mdy = m.Y - fromLoc.Y;

                // Project mobile position onto the line
                double projection = mdx * ndx + mdy * ndy;

                if (projection < 0 || projection > length)
                    continue; // Mobile is not between caster and target

                // Calculate distance from line
                double perpX = mdx - projection * ndx;
                double perpY = mdy - projection * ndy;
                double distFromLine = Math.Sqrt(perpX * perpX + perpY * perpY);

                // If within 1 tile of the line, add to targets
                if (distFromLine <= 1.5)
                    list.Add(m);
            }

            eable.Free();

            return list;
        }

        private class InternalTarget : Target
        {
            private readonly IceSpearSpell m_Owner;

            public InternalTarget(IceSpearSpell owner)
                : base(14, false, TargetFlags.Harmful)
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
