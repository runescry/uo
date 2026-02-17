// Vystia Regional Potions - Resistance and Enhancement Consumables
// Implements 8 regional potions with timed effects

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Items.Vystia
{
    #region Base Class

    public abstract class VystiaEffectPotion : Item
    {
        public abstract string PotionName { get; }
        public abstract string EffectDescription { get; }
        public abstract int Duration { get; }  // seconds
        public abstract int Cooldown { get; }  // seconds
        public abstract int PotionHue { get; }

        private static Dictionary<Mobile, Dictionary<Type, DateTime>> s_Cooldowns = new Dictionary<Mobile, Dictionary<Type, DateTime>>();
        private static Dictionary<Mobile, Dictionary<Type, Timer>> s_ActiveEffects = new Dictionary<Mobile, Dictionary<Type, Timer>>();

        // Track resistance mods per mobile per potion type
        protected static Dictionary<Mobile, Dictionary<Type, ResistanceMod>> s_ResistMods = new Dictionary<Mobile, Dictionary<Type, ResistanceMod>>();

        public VystiaEffectPotion() : base(0xF0E)
        {
            Weight = 0.5;
            Stackable = true;
        }

        public override void OnAfterDuped(Item newItem)
        {
            base.OnAfterDuped(newItem);
            if (newItem is VystiaEffectPotion potion)
                potion.Hue = PotionHue;
        }

        public override int LabelNumber => 0;
        public override string DefaultName => PotionName;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, EffectDescription);
            list.Add(1042971, $"Duration: {Duration / 60}m, Cooldown: {Cooldown / 60}m");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // Must be in backpack
                return;
            }

            // Check cooldown
            if (IsOnCooldown(from))
            {
                TimeSpan remaining = GetCooldownRemaining(from);
                from.SendMessage($"You must wait {(int)remaining.TotalSeconds} seconds before using another {PotionName}.");
                return;
            }

            // Check if effect already active
            if (HasActiveEffect(from))
            {
                from.SendMessage($"You already have a {PotionName} effect active.");
                return;
            }

            // Apply effect
            ApplyEffect(from);

            // Visual/sound effects
            from.PlaySound(0x31);
            from.FixedParticles(0x376A, 9, 32, 5005, PotionHue, 0, EffectLayer.Waist);
            from.SendMessage(0x3B2, $"You drink the {PotionName}. {EffectDescription} for {Duration / 60} minutes.");

            // Set cooldown
            SetCooldown(from);

            // Start effect timer
            StartEffectTimer(from);

            Consume();
        }

        public abstract void ApplyEffect(Mobile m);
        public abstract void RemoveEffect(Mobile m);

        private bool IsOnCooldown(Mobile m)
        {
            if (!s_Cooldowns.ContainsKey(m))
                return false;
            if (!s_Cooldowns[m].ContainsKey(GetType()))
                return false;
            return DateTime.UtcNow < s_Cooldowns[m][GetType()];
        }

        private TimeSpan GetCooldownRemaining(Mobile m)
        {
            if (!s_Cooldowns.ContainsKey(m) || !s_Cooldowns[m].ContainsKey(GetType()))
                return TimeSpan.Zero;
            return s_Cooldowns[m][GetType()] - DateTime.UtcNow;
        }

        private void SetCooldown(Mobile m)
        {
            if (!s_Cooldowns.ContainsKey(m))
                s_Cooldowns[m] = new Dictionary<Type, DateTime>();
            s_Cooldowns[m][GetType()] = DateTime.UtcNow + TimeSpan.FromSeconds(Cooldown);
        }

        private bool HasActiveEffect(Mobile m)
        {
            if (!s_ActiveEffects.ContainsKey(m))
                return false;
            return s_ActiveEffects[m].ContainsKey(GetType());
        }

        private void StartEffectTimer(Mobile m)
        {
            if (!s_ActiveEffects.ContainsKey(m))
                s_ActiveEffects[m] = new Dictionary<Type, Timer>();

            Type potionType = GetType();
            Timer timer = Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                RemoveEffect(m);
                if (s_ActiveEffects.ContainsKey(m))
                    s_ActiveEffects[m].Remove(potionType);
                m.SendMessage(0x22, $"The {PotionName} effect has worn off.");
            });

            s_ActiveEffects[m][potionType] = timer;
        }

        // Helper to store and retrieve resistance mods
        protected void StoreResistMod(Mobile m, ResistanceMod mod)
        {
            if (!s_ResistMods.ContainsKey(m))
                s_ResistMods[m] = new Dictionary<Type, ResistanceMod>();
            s_ResistMods[m][GetType()] = mod;
        }

        protected ResistanceMod GetStoredResistMod(Mobile m)
        {
            if (s_ResistMods.ContainsKey(m) && s_ResistMods[m].ContainsKey(GetType()))
                return s_ResistMods[m][GetType()];
            return null;
        }

        protected void ClearStoredResistMod(Mobile m)
        {
            if (s_ResistMods.ContainsKey(m))
                s_ResistMods[m].Remove(GetType());
        }

        public VystiaEffectPotion(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Hue = PotionHue;
        }
    }

    #endregion

    #region Resistance Potions

    public class ColdResistancePotion : VystiaEffectPotion
    {
        public override string PotionName => "Cold Resistance Potion";
        public override string EffectDescription => "+25 Cold Resistance";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 300; // 5 minutes
        public override int PotionHue => 1152; // Ice Blue (Frosthold)

        [Constructable]
        public ColdResistancePotion() : base()
        {
            Hue = PotionHue;
        }

        public override void ApplyEffect(Mobile m)
        {
            var mod = new ResistanceMod(ResistanceType.Cold, 25);
            m.AddResistanceMod(mod);
            StoreResistMod(m, mod);
        }

        public override void RemoveEffect(Mobile m)
        {
            var mod = GetStoredResistMod(m);
            if (mod != null)
            {
                m.RemoveResistanceMod(mod);
                ClearStoredResistMod(m);
            }
        }

        public ColdResistancePotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }

    public class HeatResistancePotion : VystiaEffectPotion
    {
        public override string PotionName => "Heat Resistance Potion";
        public override string EffectDescription => "+25 Fire Resistance";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 300; // 5 minutes
        public override int PotionHue => 1358; // Fire Red (Emberlands)

        [Constructable]
        public HeatResistancePotion() : base()
        {
            Hue = PotionHue;
        }

        public override void ApplyEffect(Mobile m)
        {
            var mod = new ResistanceMod(ResistanceType.Fire, 25);
            m.AddResistanceMod(mod);
            StoreResistMod(m, mod);
        }

        public override void RemoveEffect(Mobile m)
        {
            var mod = GetStoredResistMod(m);
            if (mod != null)
            {
                m.RemoveResistanceMod(mod);
                ClearStoredResistMod(m);
            }
        }

        public HeatResistancePotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }

    public class PoisonResistancePotion : VystiaEffectPotion
    {
        public override string PotionName => "Poison Resistance Potion";
        public override string EffectDescription => "+25 Poison Resistance";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 300; // 5 minutes
        public override int PotionHue => 2212; // Swamp Green (Shadowfen)

        [Constructable]
        public PoisonResistancePotion() : base()
        {
            Hue = PotionHue;
        }

        public override void ApplyEffect(Mobile m)
        {
            var mod = new ResistanceMod(ResistanceType.Poison, 25);
            m.AddResistanceMod(mod);
            StoreResistMod(m, mod);
        }

        public override void RemoveEffect(Mobile m)
        {
            var mod = GetStoredResistMod(m);
            if (mod != null)
            {
                m.RemoveResistanceMod(mod);
                ClearStoredResistMod(m);
            }
        }

        public PoisonResistancePotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }

    public class EnergyResistancePotion : VystiaEffectPotion
    {
        public override string PotionName => "Energy Resistance Potion";
        public override string EffectDescription => "+25 Energy Resistance";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 300; // 5 minutes
        public override int PotionHue => 1154; // Crystal Blue (Crystal Barrens)

        [Constructable]
        public EnergyResistancePotion() : base()
        {
            Hue = PotionHue;
        }

        public override void ApplyEffect(Mobile m)
        {
            var mod = new ResistanceMod(ResistanceType.Energy, 25);
            m.AddResistanceMod(mod);
            StoreResistMod(m, mod);
        }

        public override void RemoveEffect(Mobile m)
        {
            var mod = GetStoredResistMod(m);
            if (mod != null)
            {
                m.RemoveResistanceMod(mod);
                ClearStoredResistMod(m);
            }
        }

        public EnergyResistancePotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }

    #endregion

    #region Enhancement Potions

    public class NaturesBlessingPotion : VystiaEffectPotion
    {
        public override string PotionName => "Nature's Blessing Potion";
        public override string EffectDescription => "+5 HP Regeneration";
        public override int Duration => 900; // 15 minutes
        public override int Cooldown => 600; // 10 minutes
        public override int PotionHue => 2010; // Forest Green (Verdantpeak)

        private static Dictionary<Mobile, Timer> s_RegenTimers = new Dictionary<Mobile, Timer>();

        [Constructable]
        public NaturesBlessingPotion() : base()
        {
            Hue = PotionHue;
        }

        public override void ApplyEffect(Mobile m)
        {
            // Start HP regen timer (heals 5 HP every 2 seconds = +2.5 HP/sec effective)
            if (s_RegenTimers.ContainsKey(m) && s_RegenTimers[m] != null)
                s_RegenTimers[m].Stop();

            int ticks = Duration / 2; // Heal every 2 seconds
            int currentTick = 0;

            Timer timer = Timer.DelayCall(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), () =>
            {
                if (m == null || m.Deleted || !m.Alive || currentTick >= ticks)
                {
                    if (s_RegenTimers.ContainsKey(m))
                    {
                        s_RegenTimers[m]?.Stop();
                        s_RegenTimers.Remove(m);
                    }
                    return;
                }

                if (m.Hits < m.HitsMax)
                {
                    m.Hits = Math.Min(m.Hits + 5, m.HitsMax);
                    m.FixedParticles(0x376A, 1, 14, 0x13B5, PotionHue, 0, EffectLayer.Waist);
                }
                currentTick++;
            });

            s_RegenTimers[m] = timer;
        }

        public override void RemoveEffect(Mobile m)
        {
            if (s_RegenTimers.ContainsKey(m))
            {
                s_RegenTimers[m]?.Stop();
                s_RegenTimers.Remove(m);
            }
        }

        public NaturesBlessingPotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }

    public class CrystalClarityPotion : VystiaEffectPotion
    {
        public override string PotionName => "Crystal Clarity Potion";
        public override string EffectDescription => "+15 INT, Detect Hidden";
        public override int Duration => 1200; // 20 minutes
        public override int Cooldown => 900; // 15 minutes
        public override int PotionHue => 1154; // Crystal Blue (Crystal Barrens)

        private const string ModName = "VystiaINT";

        [Constructable]
        public CrystalClarityPotion() : base()
        {
            Hue = PotionHue;
        }

        public override void ApplyEffect(Mobile m)
        {
            m.AddStatMod(new StatMod(StatType.Int, ModName, 15, TimeSpan.FromSeconds(Duration)));

            // Reveal hidden mobiles in range
            IPooledEnumerable eable = m.GetMobilesInRange(8);
            foreach (Mobile target in eable)
            {
                if (target.Hidden && target != m)
                {
                    target.RevealingAction();
                    m.SendMessage(0x3B2, $"You sense {target.Name} nearby!");
                }
            }
            eable.Free();
        }

        public override void RemoveEffect(Mobile m)
        {
            m.RemoveStatMod(ModName);
        }

        public CrystalClarityPotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }

    public class DesertSwiftnessPotion : VystiaEffectPotion
    {
        public override string PotionName => "Desert Swiftness Potion";
        public override string EffectDescription => "+10 DEX";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 300; // 5 minutes
        public override int PotionHue => 2305; // Sand (Desert)

        private const string ModName = "VystiaDEX";

        [Constructable]
        public DesertSwiftnessPotion() : base()
        {
            Hue = PotionHue;
        }

        public override void ApplyEffect(Mobile m)
        {
            m.AddStatMod(new StatMod(StatType.Dex, ModName, 10, TimeSpan.FromSeconds(Duration)));
        }

        public override void RemoveEffect(Mobile m)
        {
            m.RemoveStatMod(ModName);
        }

        public DesertSwiftnessPotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }

    public class IroncladFortitudePotion : VystiaEffectPotion
    {
        public override string PotionName => "Ironclad Fortitude Potion";
        public override string EffectDescription => "+10 STR";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 300; // 5 minutes
        public override int PotionHue => 2401; // Bronze (Ironclad)

        private const string ModName = "VystiaSTR";

        [Constructable]
        public IroncladFortitudePotion() : base()
        {
            Hue = PotionHue;
        }

        public override void ApplyEffect(Mobile m)
        {
            m.AddStatMod(new StatMod(StatType.Str, ModName, 10, TimeSpan.FromSeconds(Duration)));
        }

        public override void RemoveEffect(Mobile m)
        {
            m.RemoveStatMod(ModName);
        }

        public IroncladFortitudePotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
    }

    #endregion
}
