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
    /// Blizzard - AoE ice storm that deals damage over time and slows enemies
    /// Circle: 6th (20 mana)
    /// Area: 5 tile radius
    /// Duration: 10 seconds
    /// Reagents: PermafrostEssence, ArcticPearl, FrozenSoul (Vystia reagents)
    /// </summary>
    public class BlizzardSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Blizzard", "Tempestas Glacialis",
            230,
            9022,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public BlizzardSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents
            if (!HasReagents(typeof(PermafrostEssence), 1) || !HasReagents(typeof(ArcticPearl), 1) || !HasReagents(typeof(FrozenSoul), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Permafrost Essence, Arctic Pearl, Frozen Soul).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(PermafrostEssence), 1) &&
                   ConsumeReagent(typeof(ArcticPearl), 1) &&
                   ConsumeReagent(typeof(FrozenSoul), 1);
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
                Effects.PlaySound(loc, map, 0x64F); // Storm sound
                Effects.SendLocationParticles(
                    EffectItem.Create(loc, map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 1150, 0, 5052, 0);

                Caster.SendMessage(0x3B2, "You summon a raging blizzard!");

                // Create blizzard effect that damages over time
                new BlizzardEffect(Caster, loc, map).Start();
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private BlizzardSpell m_Owner;

            public InternalTarget(BlizzardSpell owner) : base(12, true, TargetFlags.None)
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
    /// Blizzard effect - handles DoT and slow in AoE
    /// </summary>
    public class BlizzardEffect
    {
        private Mobile m_Caster;
        private Point3D m_Location;
        private Map m_Map;
        private Timer m_Timer;
        private int m_Ticks;
        private const int MAX_TICKS = 10; // 10 seconds, 1 tick per second
        private const int RADIUS = 5;

        public BlizzardEffect(Mobile caster, Point3D loc, Map map)
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

            // Visual effects
            Effects.SendLocationParticles(
                EffectItem.Create(m_Location, m_Map, EffectItem.DefaultDuration),
                0x3709, 10, 30, 1150, 0, 5052, 0);

            if (m_Ticks % 3 == 0)
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

                // Calculate damage per tick
                double damage = Utility.RandomMinMax(3, 8);
                damage += m_Caster.Skills[SkillName.Cryomancy].Value / 20.0;

                AOS.Damage(target, m_Caster, (int)damage, 0, 0, 100, 0, 0); // Cold damage

                // Apply slow (reduced movement)
                if (m_Ticks % 2 == 0) // Every other tick
                {
                    target.SendMessage(0x3B2, "The blizzard chills you to the bone!");
                    target.FixedParticles(0x374A, 10, 15, 5021, 1150, 0, EffectLayer.Waist);
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
