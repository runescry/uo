using System;
using System.Collections.Generic;
using Server;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Ice Age - Screen-wide devastation
    /// Everything freezes across massive area
    /// Circle: 8th (50 mana)
    /// Reagents: Frozen Soul, Frost Essence, Heart of Winter, Frozen Ore (5) (Vystia reagents)
    /// </summary>
    public class IceAgeSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ice Age", "Kal Vas An Xen Frio",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Eighth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public IceAgeSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Circle 8 ultimate spell - requires the most powerful ice reagents
            if (!HasReagents(typeof(FrozenSoul), 1) || !HasReagents(typeof(FrostEssence), 2) ||
                !HasReagents(typeof(HeartOfWinter), 1) || !HasReagents(typeof(ArcticPearl), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Frozen Soul, 2 Frost Essence, Heart of Winter, Arctic Pearl).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(FrozenSoul), 1) &&
                   ConsumeReagent(typeof(FrostEssence), 2) &&
                   ConsumeReagent(typeof(HeartOfWinter), 1) &&
                   ConsumeReagent(typeof(ArcticPearl), 1);
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

            if (CheckSequence())
            {
                Caster.FixedParticles(0x3779, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                Caster.PlaySound(0x307);

                Caster.SendMessage(0x22, "You bring forth an ICE AGE!");

                // Create ice age effect
                new IceAgeEffect(Caster).Start();
            }

            FinishSequence();
        }
    }

    /// <summary>
    /// Ice Age effect - screen-wide freeze
    /// </summary>
    public class IceAgeEffect
    {
        private Mobile m_Caster;
        private Timer m_Timer;
        private int m_Ticks;
        private const int MAX_TICKS = 20; // 20 seconds
        private const int RADIUS = 30; // Entire screen

        public IceAgeEffect(Mobile caster)
        {
            m_Caster = caster;
            m_Ticks = 0;
        }

        public void Start()
        {
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
        }

        private void OnTick()
        {
            if (m_Ticks >= MAX_TICKS || m_Caster == null || m_Caster.Deleted || m_Caster.Map == null)
            {
                Stop();
                return;
            }

            // Constant ice and snow effects
            if (m_Ticks % 2 == 0)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(m_Caster.Location, m_Caster.Map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 0x481, 0, 5052, 0);
            }

            if (m_Ticks % 5 == 0)
                Effects.PlaySound(m_Caster.Location, m_Caster.Map, 0x307);

            // Get ALL enemies in massive radius
            IPooledEnumerable eable = m_Caster.Map.GetMobilesInRange(m_Caster.Location, RADIUS);
            List<Mobile> targets = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m != m_Caster && m.Alive && m_Caster.CanBeHarmful(m, false))
                {
                    targets.Add(m);
                }
            }
            eable.Free();

            // Damage and slow all enemies
            foreach (Mobile target in targets)
            {
                m_Caster.DoHarmful(target);

                // Damage (8-15 per tick)
                double damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(target, m_Caster, (int)damage, 0, 0, 100, 0, 0);

                // Apply 90% slow
                if (m_Ticks % 3 == 0)
                {
                    StatMod slowMod = new StatMod(StatType.Dex, "IceAge_Slow", -90, TimeSpan.FromSeconds(4.0));
                    target.AddStatMod(slowMod);
                }

                // Chance to freeze solid
                if (Utility.RandomDouble() < 0.20) // 20% chance per tick
                {
                    target.Frozen = true;
                    target.SendMessage(0x22, "You are frozen solid by the ice age!");

                    Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
                    {
                        if (target != null && !target.Deleted)
                            target.Frozen = false;
                    });
                }

                if (m_Ticks % 4 == 0)
                    target.FixedParticles(0x374A, 10, 15, 5021, 0x481, 0, EffectLayer.Waist);
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
