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
    /// Frost Meteor - Devastating AoE nuke
    /// Calls down massive ice meteor with enormous damage and frozen terrain
    /// Circle: 8th (50 mana)
    /// Reagents: Frozen Soul, EternalIce, HeartOfWinter (Vystia reagents)
    /// </summary>
    public class FrostMeteorSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Frost Meteor", "Kal Vas An Corp Frio",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FrostMeteorSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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

                Point3D loc = new Point3D(p);

                // Meteor falling effect
                Caster.SendMessage(0x3B2, "You call down a frost meteor!");

                // Delay for meteor impact
                Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
                {
                    // Massive impact effects
                    Effects.PlaySound(loc, Caster.Map, 0x664);
                    Effects.SendLocationParticles(
                        EffectItem.Create(loc, Caster.Map, EffectItem.DefaultDuration),
                        0x36D4, 20, 10, 0x481, 0, 5052, 0);

                    // Get all targets in 10-tile radius
                    IPooledEnumerable eable = Caster.Map.GetMobilesInRange(loc, 10);
                    List<Mobile> targets = new List<Mobile>();

                    foreach (Mobile m in eable)
                    {
                        if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        {
                            targets.Add(m);
                        }
                    }
                    eable.Free();

                    // Damage each target
                    foreach (Mobile m in targets)
                    {
                        Caster.DoHarmful(m);

                        // Massive damage (80-120)
                        double damage = Utility.RandomMinMax(80, 120);
                        SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);

                        // Apply 75% slow
                        StatMod slowMod = new StatMod(StatType.Dex, "FrostMeteor_Slow", -75, TimeSpan.FromSeconds(10.0));
                        m.AddStatMod(slowMod);

                        m.FixedParticles(0x376A, 9, 32, 5030, 0x481, 0, EffectLayer.Waist);
                        m.SendMessage(0x22, "The frost meteor devastates you!");
                    }

                    // Create frozen terrain
                    new FrozenGroundEffect(Caster, loc, Caster.Map).Start();

                    if (targets.Count > 0)
                        Caster.SendMessage(0x3B2, String.Format("Frost meteor hits {0} targets!", targets.Count));
                });
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private FrostMeteorSpell m_Owner;

            public InternalTarget(FrostMeteorSpell owner) : base(20, true, TargetFlags.None)
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
