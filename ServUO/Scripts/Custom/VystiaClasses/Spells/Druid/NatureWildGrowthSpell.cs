using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Nature
{
    /// <summary>
    /// Wild Growth - Creates thick vegetation that blocks line of sight and slows enemies 30%
    /// Circle: 3 (9 mana)
    /// </summary>
    public class NatureWildGrowthSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Wild Growth", "Wildum Growthum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureWildGrowthSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(DruidBark), 1) || !Caster.Backpack.ConsumeTotal(typeof(TreantSap), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: DruidBark (1), TreantSap (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
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

                Point3D loc = new Point3D(p);
                Map map = Caster.Map;

                // Visual effect - thick vegetation
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3789, 10, 30, 0x21, 0, 5029, 0);
                Effects.PlaySound(loc, map, 0x22F);

                Caster.SendMessage(0x3B2, "Thick vegetation erupts from the ground!");

                // Calculate duration (8-15 seconds)
                double duration = 8.0 + (Caster.Skills.Magery.Value / 20.0);

                // Apply slow to all enemies in 4 tile radius
                System.Collections.Generic.List<Mobile> targets = new System.Collections.Generic.List<Mobile>();
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 4);

                foreach (Mobile m in eable)
                {
                    if (m != Caster && m.Alive && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile m in targets)
                {
                    Caster.DoHarmful(m);

                    // Visual on target
                    m.FixedParticles(0x375A, 10, 15, 5013, 0x21, 0, EffectLayer.Waist);

                    // 30% slow = -30 DEX
                    int slowAmount = (int)(m.Dex * 0.3);
                    m.AddStatMod(new StatMod(StatType.Dex, "WildGrowth_Slow", -slowAmount, TimeSpan.FromSeconds(duration)));

                    m.SendMessage(0x22, "Thick vegetation tangles around you, slowing your movement!");
                }

                if (targets.Count > 0)
                    Caster.SendMessage(0x3B2, $"You slow {targets.Count} enemies with wild growth!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureWildGrowthSpell m_Owner;

            public InternalTarget(NatureWildGrowthSpell owner)
                : base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
