using System;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Chill Aura - Creates a chilling aura around caster
    /// Enemies within 2 tiles are frozen and have DEX drained
    /// Circle: 6th (16 mana)
    /// Reagents: Winterleaf, Glacier Crystal, Permafrost Essence (Vystia reagents)
    /// </summary>
    public class ChillAuraSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Chill Aura", "Frio Aura",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public ChillAuraSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            // Check for Vystia reagents (Circle 6)
            if (!HasReagents(typeof(Winterleaf), 1) || !HasReagents(typeof(GlacierCrystal), 1) || !HasReagents(typeof(PermafrostEssence), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Winterleaf, Glacier Crystal, Permafrost Essence).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(Winterleaf), 1) &&
                   ConsumeReagent(typeof(GlacierCrystal), 1) &&
                   ConsumeReagent(typeof(PermafrostEssence), 1);
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
                Caster.FixedParticles(0x3709, 10, 30, 5052, 0x481, 0, EffectLayer.Waist);
                Caster.PlaySound(0x1F1);

                Caster.SendMessage(0x3B2, "A chilling aura surrounds you!");

                // Create aura effect that affects enemies over time
                new ChillAuraEffect(Caster).Start();
            }

            FinishSequence();
        }
    }

    /// <summary>
    /// Chill Aura effect - applies attack speed debuff to nearby enemies
    /// </summary>
    public class ChillAuraEffect
    {
        private Mobile m_Caster;
        private Timer m_Timer;
        private int m_Ticks;
        private const int MAX_TICKS = 20; // 20 seconds, 1 tick per second
        private const int RADIUS = 2;
        private Dictionary<Mobile, DateTime> m_DrainedMobiles = new Dictionary<Mobile, DateTime>();

        public ChillAuraEffect(Mobile caster)
        {
            m_Caster = caster;
            m_Ticks = 0;
        }

        public void Start()
        {
            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
        }

        private void OnTick()
        {
            if (m_Ticks >= MAX_TICKS || m_Caster == null || m_Caster.Deleted || m_Caster.Map == null)
            {
                Stop();
                if (m_Caster != null && !m_Caster.Deleted)
                    m_Caster.SendMessage("Your chill aura fades away.");
                return;
            }

            // Visual effects every few ticks
            if (m_Ticks % 3 == 0)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(m_Caster.Location, m_Caster.Map, EffectItem.DefaultDuration),
                    0x3709, 10, 15, 0x481, 0, 5052, 0);
            }

            // Get all mobiles in range
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

            // Apply paralyze OR drain DEX completely for targets in range
            foreach (Mobile target in targets)
            {
                // Check if this is first time entering range (not in dictionary)
                if (!m_DrainedMobiles.ContainsKey(target))
                {
                    // First time: Paralyze for 2 seconds
                    target.Paralyze(TimeSpan.FromSeconds(2.0));
                    target.SendMessage(0x3B2, "The chilling aura freezes you in place!");
                    target.FixedParticles(0x374A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                    m_DrainedMobiles[target] = DateTime.UtcNow;
                }
                else
                {
                    // While in range: Drain DEX completely
                    StatMod existingMod = target.GetStatMod("ChillAura_DexDrain");
                    if (existingMod != null)
                        target.RemoveStatMod("ChillAura_DexDrain");
                    
                    int currentDex = target.RawDex;
                    StatMod dexDrain = new StatMod(StatType.Dex, "ChillAura_DexDrain", -currentDex, TimeSpan.FromSeconds(1.0));
                    target.AddStatMod(dexDrain);
                    
                    if (m_Ticks % 2 == 0) // Message every other tick
                    {
                        target.SendMessage(0x3B2, "The chilling aura drains your dexterity!");
                        target.FixedParticles(0x374A, 10, 15, 5038, 0x481, 0, EffectLayer.Waist);
                    }
                }
            }

            // Restore DEX for mobiles that left range
            List<Mobile> toRestore = new List<Mobile>();
            foreach (var kvp in m_DrainedMobiles)
            {
                if (!targets.Contains(kvp.Key))
                {
                    toRestore.Add(kvp.Key);
                }
            }
            foreach (Mobile m in toRestore)
            {
                StatMod mod = m.GetStatMod("ChillAura_DexDrain");
                if (mod != null)
                {
                    m.RemoveStatMod("ChillAura_DexDrain");
                    m.SendMessage("You feel your dexterity returning as you leave the chilling aura.");
                }
                m_DrainedMobiles.Remove(m);
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
