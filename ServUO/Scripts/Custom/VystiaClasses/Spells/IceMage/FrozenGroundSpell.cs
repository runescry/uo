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
    /// Frozen Ground - Creates frozen terrain that damages and slows
    /// Creates a 3-tile radius area that deals DoT and applies slow
    /// Circle: 3rd (9 mana) - Balanced for mid-tier spell
    /// Reagents: Frostbloom, Winterleaf, Glacier Crystal (Vystia reagents)
    /// </summary>
    public class FrozenGroundSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Frozen Ground", "Vas Frio Terra",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Third;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FrozenGroundSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                if (p is Item)
                    p = ((Item)p).GetWorldLocation();

                Point3D loc = new Point3D(p);
                Map map = Caster.Map;

                if (map == null)
                    return;

                // Visual effects
                Effects.PlaySound(loc, map, 0x64F);
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x122A, 10, 30, 0x481, 0, 5052, 0);

                Caster.SendMessage(0x3B2, "You freeze the ground in a wide area!");

                // Create frozen ground effect
                new FrozenGroundEffect(Caster, loc, map).Start();
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private FrozenGroundSpell m_Owner;

            public InternalTarget(FrozenGroundSpell owner) : base(12, true, TargetFlags.None)
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
    /// Frozen Ground effect - deals DoT and slows in 4-tile radius
    /// </summary>
    public class FrozenGroundEffect
    {
        private Mobile m_Caster;
        private Point3D m_Location;
        private Map m_Map;
        private Timer m_Timer;
        private int m_Ticks;
        private const int MAX_TICKS = 15; // 15 seconds (reduced from 20)
        private const int RADIUS = 3; // 3-tile radius (reduced from 4)
        private List<Point3D> m_SnowTiles = new List<Point3D>(); // Store snow tile locations

        public FrozenGroundEffect(Mobile caster, Point3D loc, Map map)
        {
            m_Caster = caster;
            m_Location = loc;
            m_Map = map;
            m_Ticks = 0;
        }

        public void Start()
        {
            // Create visible snow tiles in radius
            for (int x = -RADIUS; x <= RADIUS; x++)
            {
                for (int y = -RADIUS; y <= RADIUS; y++)
                {
                    int dist = (int)Math.Sqrt(x * x + y * y);
                    if (dist <= RADIUS)
                    {
                        Point3D tileLoc = new Point3D(m_Location.X + x, m_Location.Y + y, m_Location.Z);
                        m_SnowTiles.Add(tileLoc);
                        
                        // Create visible snow tile (using item 0x122A with ice hue)
                        Item snowTile = new Static(0x122A);
                        snowTile.Hue = 0x481; // Ice blue (as documented)
                        snowTile.MoveToWorld(tileLoc, m_Map);
                        
                        // Remove tile after spell duration
                        Timer.DelayCall(TimeSpan.FromSeconds(MAX_TICKS), () =>
                        {
                            if (snowTile != null && !snowTile.Deleted)
                                snowTile.Delete();
                        });
                    }
                }
            }
            
            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
        }

        private void OnTick()
        {
            if (m_Ticks >= MAX_TICKS || m_Caster == null || m_Caster.Deleted || m_Map == null)
            {
                Stop();
                return;
            }

            // Visual effects
            if (m_Ticks % 2 == 0)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(m_Location, m_Map, EffectItem.DefaultDuration),
                    0x122A, 10, 20, 0x481, 0, 5052, 0);
            }

            if (m_Ticks % 5 == 0)
                Effects.PlaySound(m_Location, m_Map, 0x64F);

            // Get all mobiles in range
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

                // Calculate damage per tick (2-4, balanced for Circle 3)
                double damage = Utility.RandomMinMax(2, 4);

                AOS.Damage(target, m_Caster, (int)damage, 0, 0, 100, 0, 0); // Cold damage

                // Apply slow effect every 3 ticks (refresh while in area)
                if (m_Ticks % 3 == 0)
                {
                    StatMod existingMod = target.GetStatMod("FrozenGround_Slow");
                    if (existingMod != null)
                        target.RemoveStatMod("FrozenGround_Slow");
                    
                    StatMod slowMod = new StatMod(StatType.Dex, "FrozenGround_Slow", -20, TimeSpan.FromSeconds(4.0));
                    target.AddStatMod(slowMod);
                }

                if (m_Ticks % 3 == 0) // Message every 3rd tick
                {
                    target.SendMessage(0x3B2, "The frozen ground chills you!");
                    target.FixedParticles(0x374A, 10, 15, 5021, 0x481, 0, EffectLayer.Waist);
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
            
            // Remove all slow effects from targets
            IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Location, RADIUS);
            foreach (Mobile m in eable)
            {
                if (m != null && !m.Deleted)
                {
                    StatMod mod = m.GetStatMod("FrozenGround_Slow");
                    if (mod != null)
                        m.RemoveStatMod("FrozenGround_Slow");
                }
            }
            eable.Free();
        }
    }
}
