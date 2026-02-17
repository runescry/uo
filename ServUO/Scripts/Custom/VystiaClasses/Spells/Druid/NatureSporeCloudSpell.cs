using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Nature
{
    /// <summary>
    /// Spore Cloud - Creates poison cloud dealing 5-9 damage/tick to all enemies, applies poison, -25% accuracy
    /// Circle: 5 (14 mana)
    /// </summary>
    public class NatureSporeCloudSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Spore Cloud", "Sporeus Cloudum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Nature;

        public NatureSporeCloudSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(ElderwoodSeed), 1) || !Caster.Backpack.ConsumeTotal(typeof(PrimalVine), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: ElderwoodSeed (1), PrimalVine (1)");
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
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                Point3D loc = new Point3D(p);
                Map map = Caster.Map;

                // Initial visual effect - toxic spore cloud
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3728, 10, 30, 0x1A, 0, 5029, 0);
                Caster.PlaySound(0x205);

                Caster.SendMessage(0x3B2, "You release a cloud of toxic spores!");

                // Start AoE poison DoT effect - 8 ticks, 5-9 damage per tick
                new SporeCloudAoE(Caster, loc, map, 8).Start();
            }

            FinishSequence();
        }

        /// <summary>
        /// Spore Cloud AoE - Poisons and damages all enemies in area
        /// </summary>
        private class SporeCloudAoE
        {
            private Mobile m_Caster;
            private Point3D m_Location;
            private Map m_Map;
            private Timer m_Timer;
            private int m_TicksRemaining;
            private const int RADIUS = 5;

            public SporeCloudAoE(Mobile caster, Point3D location, Map map, int ticks)
            {
                m_Caster = caster;
                m_Location = location;
                m_Map = map;
                m_TicksRemaining = ticks;
            }

            public void Start()
            {
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1), OnTick);
            }

            private void OnTick()
            {
                if (m_TicksRemaining <= 0 || m_Caster == null || m_Caster.Deleted || m_Map == null)
                {
                    Stop();
                    return;
                }

                // Visual effect - green toxic cloud
                Effects.SendLocationParticles(
                    EffectItem.Create(m_Location, m_Map, EffectItem.DefaultDuration),
                    0x3728, 10, 15, 0x1A, 0, 5029, 0);

                // Get all enemies in radius
                IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Location, RADIUS);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (m != m_Caster && m.Alive && m_Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }
                eable.Free();

                foreach (Mobile target in targets)
                {
                    m_Caster.DoHarmful(target);

                    // 5-9 poison damage per tick
                    int damage = Utility.RandomMinMax(5, 9);
                    AOS.Damage(target, m_Caster, damage, 0, 0, 0, 100, 0);

                    // Apply poison on first tick and every 3 ticks
                    if (m_TicksRemaining == 8 || m_TicksRemaining % 3 == 0)
                    {
                        target.ApplyPoison(m_Caster, Poison.Regular);
                    }

                    // -25% accuracy via DEX debuff
                    if (!target.GetStatMod("SporeCloud_Accuracy")?.Name.Equals("SporeCloud_Accuracy") == true)
                    {
                        target.AddStatMod(new StatMod(StatType.Dex, "SporeCloud_Accuracy", -25, TimeSpan.FromSeconds(4.0)));
                    }

                    target.FixedParticles(0x374A, 5, 10, 5021, 0x1A, 0, EffectLayer.Waist);
                    target.SendMessage(0x22, $"[Spore Cloud] {damage} poison damage!");
                }

                m_TicksRemaining--;

                if (m_TicksRemaining <= 0)
                {
                    if (m_Caster != null && !m_Caster.Deleted)
                        m_Caster.SendMessage(0x3B2, "The spore cloud dissipates.");
                    Stop();
                }
            }

            private void Stop()
            {
                if (m_Timer != null)
                {
                    m_Timer.Stop();
                    m_Timer = null;
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly NatureSporeCloudSpell m_Owner;

            public InternalTarget(NatureSporeCloudSpell owner)
                : base(12, true, TargetFlags.Harmful)
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
