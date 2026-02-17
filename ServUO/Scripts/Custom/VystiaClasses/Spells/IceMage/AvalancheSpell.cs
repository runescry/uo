using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Avalanche - Cone AoE with knockback
    /// Massive ice and snow cascade dealing damage and knocking back enemies
    /// Circle: 1st (4 mana)
    /// Reagents: Frostbloom (Vystia reagent)
    /// </summary>
    public class AvalancheSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Avalanche", "Kal Vas Frio Multi",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.First;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public AvalancheSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

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

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                if (p is Item)
                    p = ((Item)p).GetWorldLocation();

                Point3D targetLoc = new Point3D(p);

                // Visual effects
                Effects.PlaySound(targetLoc, Caster.Map, 0x664);

                // Get targets in 8-tile cone
                List<Mobile> targets = GetConeTargets(Caster.Location, targetLoc, 8);

                foreach (Mobile m in targets)
                {
                    if (!Caster.CanBeHarmful(m, false))
                        continue;

                    Caster.DoHarmful(m);

                    // Damage (30-50)
                    double damage = Utility.RandomMinMax(30, 50);
                    SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);

                    // Visual effect on each target
                    m.FixedParticles(0x36E4, 10, 20, 5052, 0x481, 0, EffectLayer.Waist);
                    m.FixedParticles(0x3709, 10, 30, 5052, 0x481, 0, EffectLayer.Waist);

                    // Knockback effect (push away from caster)
                    int dx = m.X - Caster.X;
                    int dy = m.Y - Caster.Y;

                    // Normalize and multiply by 3 tiles
                    double length = Math.Sqrt(dx * dx + dy * dy);
                    if (length > 0)
                    {
                        int knockX = (int)(dx / length * 3);
                        int knockY = (int)(dy / length * 3);

                        Point3D newLoc = new Point3D(m.X + knockX, m.Y + knockY, m.Z);
                        m.MoveToWorld(newLoc, m.Map);
                    }

                    // Apply slow
                    StatMod slowMod = new StatMod(StatType.Dex, "Avalanche_Slow", -20, TimeSpan.FromSeconds(5.0));
                    m.AddStatMod(slowMod);

                    m.SendMessage(0x3B2, "You are knocked back by the avalanche!");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, String.Format("Your avalanche hits {0} targets!", targets.Count));
            }

            FinishSequence();
        }

        private List<Mobile> GetConeTargets(Point3D from, Point3D target, int range)
        {
            List<Mobile> list = new List<Mobile>();

            if (Caster.Map == null)
                return list;

            // Get direction vector
            int dx = target.X - from.X;
            int dy = target.Y - from.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);

            if (length == 0)
                return list;

            double ndx = dx / length;
            double ndy = dy / length;

            IPooledEnumerable eable = Caster.Map.GetMobilesInRange(from, range);

            foreach (Mobile m in eable)
            {
                if (m == Caster || !m.Alive)
                    continue;

                // Check if in cone (45 degree angle)
                int mdx = m.X - from.X;
                int mdy = m.Y - from.Y;
                double mdist = Math.Sqrt(mdx * mdx + mdy * mdy);

                if (mdist == 0 || mdist > range)
                    continue;

                // Dot product to check angle
                double dot = (mdx * ndx + mdy * ndy) / mdist;

                // cos(45°) = 0.707
                if (dot >= 0.707)
                    list.Add(m);
            }

            eable.Free();
            return list;
        }

        private class InternalTarget : Target
        {
            private AvalancheSpell m_Owner;

            public InternalTarget(AvalancheSpell owner) : base(14, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;
                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
