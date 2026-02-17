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
    /// Frost Slick - Creates icy ground that slows enemies
    /// Creates a 3x3 tile area of ice that slows enemies passing through
    /// Circle: 2nd (6 mana)
    /// Reagents: Frostbloom, Winterleaf (Vystia reagents)
    /// </summary>
    public class FrostSlickSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Frost Slick", "Frio Terra",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Second;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public FrostSlickSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents
            if (!HasReagents(typeof(Frostbloom), 1) || !HasReagents(typeof(Winterleaf), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Frostbloom, Winterleaf).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Frostbloom), 1) && ConsumeReagent(typeof(Winterleaf), 1);
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
                Effects.PlaySound(loc, map, 0x028);
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x122A, 10, 30, 0x481, 0, 5052, 0);

                Caster.SendMessage(0x3B2, "You create a frost slick on the ground!");

                // Create frost slick effect
                new FrostSlickEffect(Caster, loc, map).Start();
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private FrostSlickSpell m_Owner;

            public InternalTarget(FrostSlickSpell owner) : base(12, true, TargetFlags.None)
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
    /// Frost Slick effect - slows enemies in 3x3 area
    /// </summary>
    public class FrostSlickEffect
    {
        private Mobile m_Caster;
        private Point3D m_Location;
        private Map m_Map;
        private Timer m_Timer;
        private int m_Ticks;
        private const int MAX_TICKS = 15; // 15 seconds
        private const int AREA_SIZE = 3; // 3x3 area (3 tiles in each direction from center)

        private Dictionary<Mobile, StatMod> m_SlowedMobiles = new Dictionary<Mobile, StatMod>();
        private Dictionary<Mobile, int> m_OriginalStamina = new Dictionary<Mobile, int>(); // Track original stamina
        private List<Item> m_Tiles = new List<Item>(); // Store actual tile items for visibility

        public FrostSlickEffect(Mobile caster, Point3D loc, Map map)
        {
            m_Caster = caster;
            m_Location = loc;
            m_Map = map;
            m_Ticks = 0;
        }

        public void Start()
        {
            // Create visible 3x3 tile area with actual Static items
            for (int x = -AREA_SIZE; x <= AREA_SIZE; x++)
            {
                for (int y = -AREA_SIZE; y <= AREA_SIZE; y++)
                {
                    Point3D tileLoc = new Point3D(m_Location.X + x, m_Location.Y + y, m_Map.GetAverageZ(m_Location.X + x, m_Location.Y + y));
                    
                    // Create visible snow tile (Static item)
                    Item snowTile = new Static(0x122A);
                    snowTile.Hue = 0x481; // Ice blue
                    snowTile.Name = "frost slick";
                    snowTile.Movable = false;
                    snowTile.Light = LightType.Circle150; // Emit some light
                    snowTile.MoveToWorld(tileLoc, m_Map);
                    m_Tiles.Add(snowTile);
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

            // Visual effects on all tiles
            if (m_Ticks % 3 == 0)
            {
                foreach (Item tile in m_Tiles)
                {
                    if (tile != null && !tile.Deleted)
                    {
                        Effects.SendLocationParticles(
                            EffectItem.Create(tile.Location, m_Map, EffectItem.DefaultDuration),
                            0x122A, 1, 1, 0x481, 0, 5052, 0);
                    }
                }
            }

            // Get all mobiles in the 3x3 area
            IPooledEnumerable eable = m_Map.GetMobilesInRange(m_Location, AREA_SIZE);
            List<Mobile> currentTargets = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m != m_Caster && m.Alive && m_Caster.CanBeHarmful(m, false))
                {
                    // Check if mobile is actually on the frost slick (within 3x3 area)
                    if (Math.Abs(m.Location.X - m_Location.X) <= AREA_SIZE &&
                        Math.Abs(m.Location.Y - m_Location.Y) <= AREA_SIZE)
                    {
                        currentTargets.Add(m);
                    }
                }
            }
            eable.Free();

            // Apply slow and stamina drain to targets on the slick
            foreach (Mobile target in currentTargets)
            {
                // Apply slow
                if (!m_SlowedMobiles.ContainsKey(target))
                {
                    StatMod slowMod = new StatMod(StatType.Dex, "FrostSlick_Slow", -20, TimeSpan.FromSeconds(5.0));
                    target.AddStatMod(slowMod);
                    m_SlowedMobiles[target] = slowMod;

                    target.SendMessage(0x3B2, "You slip on the icy ground!");
                    target.FixedParticles(0x374A, 10, 15, 5021, 0x481, 0, EffectLayer.Waist);
                }

                // Drain stamina to 0 while on slick
                if (!m_OriginalStamina.ContainsKey(target))
                {
                    m_OriginalStamina[target] = target.Stam;
                    target.Stam = 0;
                    target.SendMessage(0x3B2, "Your stamina is drained by the freezing slick!");
                }
                else
                {
                    // Keep stamina at 0 while on slick
                    target.Stam = 0;
                }
            }

            // Remove effects from mobiles that left the slick
            List<Mobile> toRemoveSlow = new List<Mobile>();
            List<Mobile> toRemoveStam = new List<Mobile>();

            foreach (var kvp in m_SlowedMobiles)
            {
                if (!currentTargets.Contains(kvp.Key))
                    toRemoveSlow.Add(kvp.Key);
            }
            foreach (var kvp in m_OriginalStamina)
            {
                if (!currentTargets.Contains(kvp.Key))
                    toRemoveStam.Add(kvp.Key);
            }

            foreach (Mobile m in toRemoveSlow)
            {
                if (m != null && !m.Deleted)
                {
                    m.RemoveStatMod("FrostSlick_Slow");
                    m.SendMessage("You regain your footing.");
                }
                m_SlowedMobiles.Remove(m);
            }
            foreach (Mobile m in toRemoveStam)
            {
                if (m != null && !m.Deleted)
                {
                    // Restore original stamina (or at least some of it)
                    int originalStam = m_OriginalStamina[m];
                    m.Stam = Math.Min(originalStam, m.StamMax);
                    m.SendMessage("Your stamina returns as you leave the slick.");
                }
                m_OriginalStamina.Remove(m);
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

            // Remove all created tiles
            foreach (Item tile in m_Tiles)
            {
                if (tile != null && !tile.Deleted)
                    tile.Delete();
            }
            m_Tiles.Clear();

            // Remove all slow effects
            foreach (var kvp in m_SlowedMobiles)
            {
                if (kvp.Key != null && !kvp.Key.Deleted)
                {
                    kvp.Key.RemoveStatMod("FrostSlick_Slow");
                    kvp.Key.SendMessage("You regain your footing as the frost slick dissipates.");
                }
            }
            m_SlowedMobiles.Clear();

            // Restore all stamina
            foreach (var kvp in m_OriginalStamina)
            {
                if (kvp.Key != null && !kvp.Key.Deleted)
                {
                    // Restore original stamina (or at least some of it)
                    int originalStam = kvp.Value;
                    kvp.Key.Stam = Math.Min(originalStam, kvp.Key.StamMax);
                    kvp.Key.SendMessage("Your stamina returns as the frost slick dissipates.");
                }
            }
            m_OriginalStamina.Clear();
        }
    }
}
