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
    /// Rime Reaper - Execute ability
    /// Massive ice scythe that shatters low HP targets
    /// Circle: 8th (50 mana)
    /// Reagents: Frozen Soul, EternalIce, HeartOfWinter (Vystia reagents)
    /// </summary>
    public class RimeReaperSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Rime Reaper", "Corp Por Frio Mort",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public RimeReaperSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(FrozenSoul), 1) || !HasReagents(typeof(FrostEssence), 1) || !HasReagents(typeof(HeartOfWinter), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Frozen Soul, Eternal Ice, Heart of Winter).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(FrozenSoul), 1) &&
                   ConsumeReagent(typeof(FrostEssence), 1) &&
                   ConsumeReagent(typeof(HeartOfWinter), 1);
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

                // Visual effects - giant ice scythe slash
                Effects.PlaySound(Caster.Location, Caster.Map, 0x232);
                Effects.SendLocationParticles(
                    EffectItem.Create(targetLoc, Caster.Map, EffectItem.DefaultDuration),
                    0x36BD, 20, 10, 0x481, 0, 5052, 0);

                Caster.SendMessage(0x3B2, "You swing the Rime Reaper!");

                // Get targets in arc (15-tile range)
                List<Mobile> targets = GetArcTargets(Caster.Location, targetLoc, 15);

                foreach (Mobile m in targets)
                {
                    if (!Caster.CanBeHarmful(m, false))
                        continue;

                    Caster.DoHarmful(m);

                    // Base damage (100-150)
                    double damage = Utility.RandomMinMax(100, 150);

                    // Check for execute (below 20% HP)
                    double hpPercent = (double)m.Hits / (double)m.HitsMax;
                    bool isExecute = (hpPercent <= 0.20);

                    if (isExecute)
                    {
                        // Instant kill - shatter into ice
                        m.Kill();
                        m.FixedParticles(0x3735, 1, 30, 9966, 0x481, 0, EffectLayer.Waist);
                        Caster.SendMessage(0x3B2, String.Format("{0} shatters into ice!", m.Name));
                    }
                    else
                    {
                        // Normal damage
                        SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);
                        m.FixedParticles(0x36CB, 1, 9, 9999, 0x481, 0, EffectLayer.Head);
                    }
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, String.Format("Rime Reaper hits {0} targets!", targets.Count));
            }

            FinishSequence();
        }

        private List<Mobile> GetArcTargets(Point3D from, Point3D target, int range)
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

                // Check if in arc (90 degree angle for scythe sweep)
                int mdx = m.X - from.X;
                int mdy = m.Y - from.Y;
                double mdist = Math.Sqrt(mdx * mdx + mdy * mdy);

                if (mdist == 0 || mdist > range)
                    continue;

                // Dot product
                double dot = (mdx * ndx + mdy * ndy) / mdist;

                // cos(45°) = 0.707 (90° arc total)
                if (dot >= 0.707)
                    list.Add(m);
            }

            eable.Free();
            return list;
        }

        private class InternalTarget : Target
        {
            private RimeReaperSpell m_Owner;

            public InternalTarget(RimeReaperSpell owner) : base(15, true, TargetFlags.None)
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
