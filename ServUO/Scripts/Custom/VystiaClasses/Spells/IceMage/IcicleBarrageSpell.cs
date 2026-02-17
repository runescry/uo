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
    /// Icicle Barrage - AoE burst damage
    /// Rains icicles in an area dealing cold damage
    /// Circle: 4th (11 mana)
    /// Reagents: Glacier Crystal, Permafrost Essence (Vystia reagents)
    /// </summary>
    public class IcicleBarrageSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Icicle Barrage", "Vas Frio Multi",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public IcicleBarrageSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(GlacierCrystal), 1) || !HasReagents(typeof(PermafrostEssence), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Glacier Crystal, Permafrost Essence).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(GlacierCrystal), 1) && ConsumeReagent(typeof(PermafrostEssence), 1);
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
            if (CheckSequence())
            {
            Caster.Target = new InternalTarget(this);
            }
            else
            {
                FinishSequence();
            }
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

                Point3D loc = target is Mobile ? ((Mobile)target).Location : ((Item)target).Location;
                Map map = Caster.Map;

                // Visual effects - multiple icicles
                for (int i = 0; i < 5; i++)
                {
                    Timer.DelayCall(TimeSpan.FromMilliseconds(i * 200), delegate
                    {
                        Point3D offset = new Point3D(
                            loc.X + Utility.RandomMinMax(-2, 2),
                            loc.Y + Utility.RandomMinMax(-2, 2),
                            loc.Z
                        );

                        Effects.SendLocationParticles(
                            EffectItem.Create(offset, map, EffectItem.DefaultDuration),
                            0x36E4, 10, 20, 0x481, 0, 5052, 0);
                    });
                }

                Effects.PlaySound(loc, map, 0x64F);

                // Get all targets in 3-tile radius
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 3);
                List<IDamageable> targets = new List<IDamageable>();

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                    {
                        targets.Add(m);
                    }
                }
                eable.Free();

                // Damage each target
                foreach (IDamageable dest in targets)
                {
                    Caster.DoHarmful(dest);

                    double damage = Utility.RandomMinMax(15, 25);
                    SpellHelper.Damage(this, dest, damage, 0, 0, 100, 0, 0);

                    dest.FixedParticles(0x36CB, 1, 9, 9999, 0x481, 0, EffectLayer.Head);
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, String.Format("Your icicle barrage hits {0} targets!", targets.Count));
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly IcicleBarrageSpell m_Owner;

            public InternalTarget(IcicleBarrageSpell owner) : base(12, false, TargetFlags.Harmful)
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
