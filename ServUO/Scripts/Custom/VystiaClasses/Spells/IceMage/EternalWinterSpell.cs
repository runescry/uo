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
    /// Eternal Winter - Zone control ultimate
    /// Creates massive frozen wasteland with continuous damage and slowing
    /// Circle: 7th (40 mana)
    /// Reagents: Arctic Pearl, Frozen Soul, EternalIce (Vystia reagents)
    /// </summary>
    public class EternalWinterSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Eternal Winter", "Vas Kal An Frio Terra",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public EternalWinterSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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

                Effects.PlaySound(loc, Caster.Map, 0x64F);
                Caster.SendMessage(0x3B2, "You create an eternal winter!");

                // Create eternal winter effect
                new EternalWinterEffect(Caster, loc, Caster.Map).Start();
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private EternalWinterSpell m_Owner;

            public InternalTarget(EternalWinterSpell owner) : base(12, true, TargetFlags.None)
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

    /// <summary>
    /// Eternal Winter effect - massive zone DoT
    /// </summary>
    public class EternalWinterEffect
    {
        private Mobile m_Caster;
        private Point3D m_Location;
        private Map m_Map;
        private Timer m_Timer;
        private int m_Ticks;
        private const int MAX_TICKS = 30; // 30 seconds
        private const int RADIUS = 8;

        public EternalWinterEffect(Mobile caster, Point3D loc, Map map)
        {
            m_Caster = caster;
            m_Location = loc;
            m_Map = map;
            m_Ticks = 0;
        }

        public void Start()
        {
            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
        }

        private void OnTick()
        {
            if (m_Ticks >= MAX_TICKS || m_Caster == null || m_Caster.Deleted || m_Map == null)
            {
                Stop();
                return;
            }

            // Constant blizzard effects
            Effects.SendLocationParticles(
                EffectItem.Create(m_Location, m_Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 0x481, 0, 5052, 0);

            if (m_Ticks % 5 == 0)
                Effects.PlaySound(m_Location, m_Map, 0x64F);

            // Get all mobiles in 8-tile radius
            IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Location, RADIUS);
            List<Mobile> targets = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m != m_Caster && m.Alive && m_Caster.CanBeHarmful(m, false))
                {
                    targets.Add(m);
                }
            }
            eable.Free();

            // Damage and slow each target
            foreach (Mobile target in targets)
            {
                m_Caster.DoHarmful(target);

                // Damage (5-10 per tick)
                double damage = Utility.RandomMinMax(5, 10);
                AOS.Damage(target, m_Caster, (int)damage, 0, 0, 100, 0, 0);

                // Apply 50% slow
                if (m_Ticks % 3 == 0)
                {
                    StatMod slowMod = new StatMod(StatType.Dex, "EternalWinter_Slow", -50, TimeSpan.FromSeconds(4.0));
                    target.AddStatMod(slowMod);
                    target.SendMessage(0x3B2, "The eternal winter freezes you!");
                }
            }

            m_Ticks++;
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
}
