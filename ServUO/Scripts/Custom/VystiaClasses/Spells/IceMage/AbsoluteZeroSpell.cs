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
    /// Absolute Zero - Ultimate AoE nuke
    /// Massive cold damage in large area with freeze and frozen ground
    /// Circle: 7th (40 mana)
    /// Reagents: Arctic Pearl, Frozen Soul, EternalIce (Vystia reagents)
    /// </summary>
    public class AbsoluteZeroSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Absolute Zero", "Kal Vas An Frio",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public AbsoluteZeroSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(ArcticPearl), 1) || !HasReagents(typeof(FrozenSoul), 1) || !HasReagents(typeof(FrostEssence), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Arctic Pearl, Frozen Soul, Eternal Ice).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(ArcticPearl), 1) &&
                   ConsumeReagent(typeof(FrozenSoul), 1) &&
                   ConsumeReagent(typeof(FrostEssence), 1);
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

                Point3D loc = new Point3D(p);

                // Visual effects - massive blue explosion
                Effects.PlaySound(loc, Caster.Map, 0x307);
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, Caster.Map, EffectItem.DefaultDuration),
                    0x36BD, 20, 10, 0x481, 0, 5052, 0);

                Caster.SendMessage(0x3B2, "You unleash absolute zero!");

                // Get all targets in 6-tile radius
                IPooledEnumerable eable = Caster.Map.GetMobilesInRange(loc, 6);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                    {
                        targets.Add(m);
                    }
                }
                eable.Free();

                // Damage and freeze each target
                foreach (Mobile m in targets)
                {
                    Caster.DoHarmful(m);

                    // Damage (50-80)
                    double damage = Utility.RandomMinMax(50, 80);
                    SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);

                    // Freeze for 3 seconds
                    m.Frozen = true;
                    m.FixedParticles(0x376A, 9, 32, 5030, 0x481, 0, EffectLayer.Waist);
                    m.SendMessage(0x3B2, "You are frozen by absolute zero!");

                    Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
                    {
                        if (m != null && !m.Deleted)
                        {
                            m.Frozen = false;
                        }
                    });
                }

                // Create frozen ground effect
                new FrozenGroundEffect(Caster, loc, Caster.Map).Start();

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, String.Format("Absolute zero hits {0} targets!", targets.Count));
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private AbsoluteZeroSpell m_Owner;

            public InternalTarget(AbsoluteZeroSpell owner) : base(12, true, TargetFlags.None)
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
