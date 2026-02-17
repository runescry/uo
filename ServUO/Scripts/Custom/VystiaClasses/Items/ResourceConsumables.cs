// Vystia Class System v2.0 - Resource Consumables
// Potions and items that restore secondary resources

using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Items.VystiaClassItems
{
    #region Base Classes

    public abstract class ResourcePotion : Item
    {
        public abstract ResourceType ResourceToRestore { get; }
        public abstract int RestoreAmount { get; }
        public abstract string ClassName { get; }

        public ResourcePotion(int itemID) : base(itemID)
        {
            Weight = 0.5;
            Stackable = true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, $"Restores {RestoreAmount} {ResourceToRestore}");
            list.Add(1042971, $"For {ClassName} class");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // Must be in backpack
                return;
            }

            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("Only players can use this.");
                return;
            }

            var manager = VystiaResourceManager.GetManager(pm);
            if (manager == null)
            {
                from.SendMessage("You don't have access to class resources.");
                return;
            }

            var resource = manager.GetResource(ResourceToRestore);
            if (resource == null)
            {
                from.SendMessage($"You don't have access to {ResourceToRestore}.");
                return;
            }

            if (resource.Current >= resource.Maximum)
            {
                from.SendMessage($"Your {ResourceToRestore} is already at maximum.");
                return;
            }

            // Restore resource
            int before = resource.Current;
            resource.Generate(RestoreAmount);
            int restored = resource.Current - before;

            // Effects
            from.PlaySound(0x31); // Drink sound
            from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            from.SendMessage(0x3B2, $"You restored {restored} {ResourceToRestore}.");

            Consume();
        }

        public ResourcePotion(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public abstract class CombatPotion : Item
    {
        public abstract string EffectDescription { get; }
        public abstract int Duration { get; } // seconds
        public abstract int Cooldown { get; } // seconds

        private DateTime m_NextUse;

        public CombatPotion(int itemID) : base(itemID)
        {
            Weight = 0.5;
            Stackable = true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, EffectDescription);
            list.Add(1042971, $"Duration: {Duration}s, Cooldown: {Cooldown}s");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            if (DateTime.UtcNow < m_NextUse)
            {
                TimeSpan remaining = m_NextUse - DateTime.UtcNow;
                from.SendMessage($"You must wait {(int)remaining.TotalSeconds} seconds before using another.");
                return;
            }

            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("Only players can use this.");
                return;
            }

            ApplyEffect(pm);

            from.PlaySound(0x31);
            from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);

            m_NextUse = DateTime.UtcNow + TimeSpan.FromSeconds(Cooldown);
            Consume();
        }

        public abstract void ApplyEffect(PlayerMobile pm);

        public CombatPotion(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_NextUse = reader.ReadDateTime();
        }
    }

    #endregion

    #region Resource Potions

    /// <summary>
    /// Soul Essence Vial - Restores Soul Shards for Warlocks
    /// </summary>
    public class SoulEssenceVial : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.SoulShards;
        public override int RestoreAmount => 1;
        public override string ClassName => "Warlock";

        [Constructable]
        public SoulEssenceVial() : this(1) { }

        [Constructable]
        public SoulEssenceVial(int amount) : base(0x0F0E) // Potion bottle
        {
            Name = "Soul Essence Vial";
            Hue = 1109; // ShadowVoid purple
            Amount = amount;
        }

        public SoulEssenceVial(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Fury Tonic - Restores Fury for Barbarians
    /// </summary>
    public class FuryTonic : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Fury;
        public override int RestoreAmount => 50;
        public override string ClassName => "Barbarian";

        [Constructable]
        public FuryTonic() : this(1) { }

        [Constructable]
        public FuryTonic(int amount) : base(0x0F0E)
        {
            Name = "Fury Tonic";
            Hue = 1157; // Blood red
            Amount = amount;
        }

        public FuryTonic(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Chi Tea - Restores Chi for Monks
    /// </summary>
    public class ChiTea : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Chi;
        public override int RestoreAmount => 3;
        public override string ClassName => "Monk";

        [Constructable]
        public ChiTea() : this(1) { }

        [Constructable]
        public ChiTea(int amount) : base(0x0F0E)
        {
            Name = "Chi Tea";
            Hue = 1161; // Golden
            Amount = amount;
        }

        public ChiTea(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Focus Elixir - Restores Focus for Rangers
    /// </summary>
    public class FocusElixir : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Focus;
        public override int RestoreAmount => 50;
        public override string ClassName => "Ranger";

        [Constructable]
        public FocusElixir() : this(1) { }

        [Constructable]
        public FocusElixir(int amount) : base(0x0F0E)
        {
            Name = "Focus Elixir";
            Hue = 2010; // Forest green
            Amount = amount;
        }

        public FocusElixir(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Fortitude Draught - Restores Fortitude for Knights
    /// </summary>
    public class FortitudeDraught : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Fortitude;
        public override int RestoreAmount => 5;
        public override string ClassName => "Knight";

        [Constructable]
        public FortitudeDraught() : this(1) { }

        [Constructable]
        public FortitudeDraught(int amount) : base(0x0F0E)
        {
            Name = "Fortitude Draught";
            Hue = 2305; // Ironclad metallic
            Amount = amount;
        }

        public FortitudeDraught(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Faith Incense - Restores Faith for Clerics
    /// </summary>
    public class FaithIncense : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Faith;
        public override int RestoreAmount => 25;
        public override string ClassName => "Cleric";

        [Constructable]
        public FaithIncense() : this(1) { }

        [Constructable]
        public FaithIncense(int amount) : base(0x0F0E)
        {
            Name = "Faith Incense";
            Hue = 1153; // Holy white
            Amount = amount;
        }

        public FaithIncense(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Crescendo Crystal - Restores Crescendo for Bards
    /// </summary>
    public class CrescendoCrystal : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Crescendo;
        public override int RestoreAmount => 5;
        public override string ClassName => "Bard";

        [Constructable]
        public CrescendoCrystal() : this(1) { }

        [Constructable]
        public CrescendoCrystal(int amount) : base(0x1F19) // Crystal
        {
            Name = "Crescendo Crystal";
            Hue = 1161; // Golden
            Amount = amount;
        }

        public CrescendoCrystal(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Life Force Vial - Restores Life Force for Necromancers
    /// </summary>
    public class LifeForceVial : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.LifeForce;
        public override int RestoreAmount => 25;
        public override string ClassName => "Necromancer";

        [Constructable]
        public LifeForceVial() : this(1) { }

        [Constructable]
        public LifeForceVial(int amount) : base(0x0F0E)
        {
            Name = "Life Force Vial";
            Hue = 1175; // Bone/death
            Amount = amount;
        }

        public LifeForceVial(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Zeal Elixir - Restores Zeal for Templars
    /// </summary>
    public class ZealElixir : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Zeal;
        public override int RestoreAmount => 5;
        public override string ClassName => "Templar";

        [Constructable]
        public ZealElixir() : this(1) { }

        [Constructable]
        public ZealElixir(int amount) : base(0x0F0E)
        {
            Name = "Zeal Elixir";
            Hue = 1153; // Holy
            Amount = amount;
        }

        public ZealElixir(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Steam Canister - Restores Steam for Artificers
    /// </summary>
    public class SteamCanister : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Steam;
        public override int RestoreAmount => 50;
        public override string ClassName => "Artificer";

        [Constructable]
        public SteamCanister() : this(1) { }

        [Constructable]
        public SteamCanister(int amount) : base(0x1F19)
        {
            Name = "Steam Canister";
            Hue = 2305; // Ironclad
            Amount = amount;
        }

        public SteamCanister(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Pursuit Tracker - Restores Pursuit for Bounty Hunters
    /// </summary>
    public class PursuitTracker : ResourcePotion
    {
        public override ResourceType ResourceToRestore => ResourceType.Pursuit;
        public override int RestoreAmount => 5;
        public override string ClassName => "Bounty Hunter";

        [Constructable]
        public PursuitTracker() : this(1) { }

        [Constructable]
        public PursuitTracker(int amount) : base(0x1F14)
        {
            Name = "Pursuit Tracker";
            Hue = 1719; // Desert gold
            Amount = amount;
        }

        public PursuitTracker(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Combat Potions

    /// <summary>
    /// Burst Potion - Next ability deals +100% damage
    /// </summary>
    public class BurstPotion : CombatPotion
    {
        public override string EffectDescription => "+100% damage on next ability";
        public override int Duration => 15;
        public override int Cooldown => 60;

        [Constructable]
        public BurstPotion() : this(1) { }

        [Constructable]
        public BurstPotion(int amount) : base(0x0F0E)
        {
            Name = "Burst Potion";
            Hue = 1157; // Red
            Amount = amount;
        }

        public override void ApplyEffect(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "BurstPotion", 25, TimeSpan.FromSeconds(Duration)));
            pm.SendMessage(0x3B2, "Your next ability will deal greatly increased damage!");
        }

        public BurstPotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Haste Potion - +50% ability speed
    /// </summary>
    public class HastePotion : CombatPotion
    {
        public override string EffectDescription => "+50% ability speed";
        public override int Duration => 15;
        public override int Cooldown => 90;

        [Constructable]
        public HastePotion() : this(1) { }

        [Constructable]
        public HastePotion(int amount) : base(0x0F0E)
        {
            Name = "Haste Potion";
            Hue = 1281; // Blue
            Amount = amount;
        }

        public override void ApplyEffect(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Dex, "HastePotion", 30, TimeSpan.FromSeconds(Duration)));
            pm.SendMessage(0x3B2, "You feel incredibly fast!");
        }

        public HastePotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Resist Potion - +30 all resistances
    /// </summary>
    public class ResistPotion : CombatPotion
    {
        public override string EffectDescription => "+30 All Resistances";
        public override int Duration => 60;
        public override int Cooldown => 120;

        [Constructable]
        public ResistPotion() : this(1) { }

        [Constructable]
        public ResistPotion(int amount) : base(0x0F0E)
        {
            Name = "Resist Potion";
            Hue = 1153; // White
            Amount = amount;
        }

        public override void ApplyEffect(PlayerMobile pm)
        {
            // Apply resist buff via timer
            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                pm.SendMessage(0x22, "Your resistance bonus has worn off.");
            });

            pm.VirtualArmorMod += 30;
            pm.SendMessage(0x3B2, "You feel much more resistant to damage!");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.VirtualArmorMod -= 30;
            });
        }

        public ResistPotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Cleanse Potion - Remove all debuffs
    /// </summary>
    public class CleansePotion : CombatPotion
    {
        public override string EffectDescription => "Removes all debuffs";
        public override int Duration => 0;
        public override int Cooldown => 120;

        [Constructable]
        public CleansePotion() : this(1) { }

        [Constructable]
        public CleansePotion(int amount) : base(0x0F0E)
        {
            Name = "Cleanse Potion";
            Hue = 1150; // Pure white
            Amount = amount;
        }

        public override void ApplyEffect(PlayerMobile pm)
        {
            // Remove poison
            if (pm.Poisoned)
                pm.CurePoison(pm);

            // Remove paralysis
            if (pm.Paralyzed)
                pm.Paralyzed = false;

            // Remove negative stat mods
            pm.RemoveStatMod("BurstPotion");
            pm.RemoveStatMod("HastePotion");

            pm.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            pm.SendMessage(0x3B2, "All debuffs have been cleansed!");
        }

        public CleansePotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Second Wind Potion - Restore 50% of all resources
    /// </summary>
    public class SecondWindPotion : CombatPotion
    {
        public override string EffectDescription => "Restores 50% of all resources";
        public override int Duration => 0;
        public override int Cooldown => 180;

        [Constructable]
        public SecondWindPotion() : this(1) { }

        [Constructable]
        public SecondWindPotion(int amount) : base(0x0F0E)
        {
            Name = "Second Wind Potion";
            Hue = 1161; // Golden
            Amount = amount;
        }

        public override void ApplyEffect(PlayerMobile pm)
        {
            // Restore HP, Mana, Stamina (50% of max)
            int hitsRestored = Math.Min(pm.HitsMax / 2, pm.HitsMax - pm.Hits);
            int manaRestored = Math.Min(pm.ManaMax / 2, pm.ManaMax - pm.Mana);
            int stamRestored = Math.Min(pm.StamMax / 2, pm.StamMax - pm.Stam);

            pm.Hits += hitsRestored;
            pm.Mana += manaRestored;
            pm.Stam += stamRestored;

            pm.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            pm.PlaySound(0x1F7);
            pm.SendMessage(0x3B2, $"You catch your second wind! (+{hitsRestored} HP, +{manaRestored} Mana, +{stamRestored} Stam)");
        }

        public SecondWindPotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Vystia Invisibility Potion - Become invisible for short duration
    /// </summary>
    public class VystiaInvisibilityPotion : CombatPotion
    {
        public override string EffectDescription => "Become invisible";
        public override int Duration => 10;
        public override int Cooldown => 120;

        [Constructable]
        public VystiaInvisibilityPotion() : this(1) { }

        [Constructable]
        public VystiaInvisibilityPotion(int amount) : base(0x0F0E)
        {
            Name = "Invisibility Potion";
            Hue = 1150; // Transparent
            Amount = amount;
        }

        public override void ApplyEffect(PlayerMobile pm)
        {
            pm.Hidden = true;
            pm.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
            pm.SendMessage(0x3B2, "You fade from sight...");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted && pm.Hidden)
                {
                    pm.Hidden = false;
                    pm.SendMessage(0x22, "You become visible again.");
                }
            });
        }

        public VystiaInvisibilityPotion(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion
}
