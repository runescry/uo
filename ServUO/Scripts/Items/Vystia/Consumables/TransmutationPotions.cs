// Vystia Transmutation Potions - Resource Enhancement Potions
// Potions that modify secondary resource mechanics

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Items.Vystia
{
    #region Base Classes

    /// <summary>
    /// Base class for timed enhancement potions that modify resource behavior
    /// </summary>
    public abstract class ResourceEnhancementPotion : Item
    {
        public abstract string EffectDescription { get; }
        public abstract int Duration { get; } // seconds
        public abstract int Cooldown { get; } // seconds

        private static Dictionary<Mobile, DateTime> m_Cooldowns = new Dictionary<Mobile, DateTime>();

        public ResourceEnhancementPotion(int itemID) : base(itemID)
        {
            Weight = 0.5;
            Stackable = true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, EffectDescription);
            if (Duration > 0)
                list.Add(1042971, $"Duration: {Duration / 60}min");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // Must be in backpack
                return;
            }

            string potionType = GetType().Name;
            string cooldownKey = $"{from.Serial}_{potionType}";

            if (m_Cooldowns.ContainsKey(from) && DateTime.UtcNow < m_Cooldowns[from])
            {
                TimeSpan remaining = m_Cooldowns[from] - DateTime.UtcNow;
                from.SendMessage($"You must wait {(int)remaining.TotalSeconds} seconds before using another enhancement potion.");
                return;
            }

            if (!(from is PlayerMobile pm))
            {
                from.SendMessage("Only players can use this.");
                return;
            }

            if (ApplyEffect(pm))
            {
                from.PlaySound(0x31); // Drink sound
                from.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);

                m_Cooldowns[from] = DateTime.UtcNow + TimeSpan.FromSeconds(Cooldown);
                Consume();
            }
        }

        public abstract bool ApplyEffect(PlayerMobile pm);

        public ResourceEnhancementPotion(Serial serial) : base(serial) { }

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

    /// <summary>
    /// Base class for potions that store resources for later use
    /// </summary>
    public abstract class ResourceStorageFlask : Item
    {
        public abstract ResourceType StoredResourceType { get; }
        public abstract int Capacity { get; }
        public abstract string ClassName { get; }

        private int m_StoredAmount;

        [CommandProperty(AccessLevel.GameMaster)]
        public int StoredAmount
        {
            get { return m_StoredAmount; }
            set { m_StoredAmount = Math.Max(0, Math.Min(value, Capacity)); InvalidateProperties(); }
        }

        public ResourceStorageFlask(int itemID) : base(itemID)
        {
            Weight = 1.0;
            Stackable = false;
            m_StoredAmount = 0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, $"Stores {StoredResourceType}");
            list.Add(1060658, $"Stored\t{m_StoredAmount}/{Capacity}");
            list.Add(1042971, $"For {ClassName} class");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
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

            var resource = manager.GetResource(StoredResourceType);
            if (resource == null)
            {
                from.SendMessage($"You don't have access to {StoredResourceType}.");
                return;
            }

            // If flask has stored resource, release it
            if (m_StoredAmount > 0)
            {
                int canRestore = resource.Maximum - resource.Current;
                int toRestore = Math.Min(m_StoredAmount, canRestore);

                if (toRestore > 0)
                {
                    resource.Generate(toRestore);
                    m_StoredAmount -= toRestore;
                    from.PlaySound(0x31);
                    from.SendMessage(0x3B2, $"You released {toRestore} {StoredResourceType} from the flask.");
                    InvalidateProperties();
                }
                else
                {
                    from.SendMessage($"Your {StoredResourceType} is already at maximum.");
                }
            }
            else
            {
                // Store current resource
                int toStore = Math.Min(resource.Current, Capacity);
                if (toStore > 0 && resource.Spend(toStore))
                {
                    m_StoredAmount = toStore;
                    from.PlaySound(0x240);
                    from.SendMessage(0x3B2, $"You stored {toStore} {StoredResourceType} in the flask.");
                    InvalidateProperties();
                }
                else
                {
                    from.SendMessage($"You don't have enough {StoredResourceType} to store.");
                }
            }
        }

        public ResourceStorageFlask(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_StoredAmount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_StoredAmount = reader.ReadInt();
        }
    }

    #endregion

    #region Fury Enhancement Potions (Barbarian)

    /// <summary>
    /// Fury Draught - Temporarily increases max Fury by 15
    /// </summary>
    public class FuryDraught : ResourceEnhancementPotion
    {
        public override string EffectDescription => "+15 Max Fury";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 900; // 15 minutes

        [Constructable]
        public FuryDraught() : this(1) { }

        [Constructable]
        public FuryDraught(int amount) : base(0x0F0E)
        {
            Name = "Fury Draught";
            Hue = 1157; // Blood red
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your maximum Fury has increased by 15!");

            // Apply the buff - would need to hook into the resource system
            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Fury Draught effect has worn off.");
            });

            return true;
        }

        public FuryDraught(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Berserker's Blood - Reduces Fury decay rate by 35%
    /// </summary>
    public class BerserkersBlood : ResourceEnhancementPotion
    {
        public override string EffectDescription => "Fury decay -35%";
        public override int Duration => 300; // 5 minutes
        public override int Cooldown => 600; // 10 minutes

        [Constructable]
        public BerserkersBlood() : this(1) { }

        [Constructable]
        public BerserkersBlood(int amount) : base(0x0F0E)
        {
            Name = "Berserker's Blood";
            Hue = 1194; // Dark red
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your Fury decays much slower!");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Berserker's Blood effect has worn off.");
            });

            return true;
        }

        public BerserkersBlood(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Chi Enhancement Potions (Monk)

    /// <summary>
    /// Chi Elixir - Temporarily increases max Chi by 1
    /// </summary>
    public class ChiElixir : ResourceEnhancementPotion
    {
        public override string EffectDescription => "+1 Max Chi";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 900; // 15 minutes

        [Constructable]
        public ChiElixir() : this(1) { }

        [Constructable]
        public ChiElixir(int amount) : base(0x0F0E)
        {
            Name = "Chi Elixir";
            Hue = 1161; // Golden
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your inner Chi expands! Max Chi increased by 1.");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Chi Elixir effect has worn off.");
            });

            return true;
        }

        public ChiElixir(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Focus Enhancement Potions (Ranger)

    /// <summary>
    /// Focused Serum - Reduces Focus decay rate by 35%
    /// </summary>
    public class FocusedSerum : ResourceEnhancementPotion
    {
        public override string EffectDescription => "Focus decay -35%";
        public override int Duration => 300; // 5 minutes
        public override int Cooldown => 600; // 10 minutes

        [Constructable]
        public FocusedSerum() : this(1) { }

        [Constructable]
        public FocusedSerum(int amount) : base(0x0F0E)
        {
            Name = "Focused Serum";
            Hue = 2010; // Forest green
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your Focus holds steady even when moving!");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Focused Serum effect has worn off.");
            });

            return true;
        }

        public FocusedSerum(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Zeal Enhancement Potions (Templar)

    /// <summary>
    /// Zealot's Tonic - Gain +1 Zeal per kill for 5 minutes
    /// </summary>
    public class ZealotsTonic : ResourceEnhancementPotion
    {
        public override string EffectDescription => "+1 Zeal per kill";
        public override int Duration => 300; // 5 minutes
        public override int Cooldown => 600; // 10 minutes

        [Constructable]
        public ZealotsTonic() : this(1) { }

        [Constructable]
        public ZealotsTonic(int amount) : base(0x0F0E)
        {
            Name = "Zealot's Tonic";
            Hue = 1153; // Holy white
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your righteous fervor intensifies! +1 Zeal per kill.");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Zealot's Tonic effect has worn off.");
            });

            return true;
        }

        public ZealotsTonic(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Fortitude Enhancement Potions (Knight)

    /// <summary>
    /// Knight's Fortifier - Temporarily increases max Fortitude by 2
    /// </summary>
    public class KnightsFortifier : ResourceEnhancementPotion
    {
        public override string EffectDescription => "+2 Max Fortitude";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 900; // 15 minutes

        [Constructable]
        public KnightsFortifier() : this(1) { }

        [Constructable]
        public KnightsFortifier(int amount) : base(0x0F0E)
        {
            Name = "Knight's Fortifier";
            Hue = 2305; // Ironclad metallic
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your defensive resolve strengthens! Max Fortitude increased by 2.");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Knight's Fortifier effect has worn off.");
            });

            return true;
        }

        public KnightsFortifier(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Pursuit Enhancement Potions (Bounty Hunter)

    /// <summary>
    /// Hunter's Mark Oil - Increases Mark duration by 35%
    /// </summary>
    public class HuntersMarkOil : ResourceEnhancementPotion
    {
        public override string EffectDescription => "Mark duration +35%";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 900; // 15 minutes

        [Constructable]
        public HuntersMarkOil() : this(1) { }

        [Constructable]
        public HuntersMarkOil(int amount) : base(0x1848) // Oil flask
        {
            Name = "Hunter's Mark Oil";
            Hue = 1719; // Desert gold
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your marks will last 35% longer!");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Hunter's Mark Oil effect has worn off.");
            });

            return true;
        }

        public HuntersMarkOil(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Soul Shard Enhancement Potions (Warlock)

    /// <summary>
    /// Shard Catalyst - Increases Soul Shard generation by 5%
    /// </summary>
    public class ShardCatalyst : ResourceEnhancementPotion
    {
        public override string EffectDescription => "+5% Soul Shard generation";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 900; // 15 minutes

        [Constructable]
        public ShardCatalyst() : this(1) { }

        [Constructable]
        public ShardCatalyst(int amount) : base(0x0F0E)
        {
            Name = "Shard Catalyst";
            Hue = 1109; // ShadowVoid purple
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Soul Shards generate 5% faster!");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Shard Catalyst effect has worn off.");
            });

            return true;
        }

        public ShardCatalyst(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Chill Enhancement Potions (Ice Mage)

    /// <summary>
    /// Chill Enhancer - Increases freeze duration by 5%
    /// </summary>
    public class ChillEnhancer : ResourceEnhancementPotion
    {
        public override string EffectDescription => "+5% freeze duration";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 900; // 15 minutes

        [Constructable]
        public ChillEnhancer() : this(1) { }

        [Constructable]
        public ChillEnhancer(int amount) : base(0x0F0E)
        {
            Name = "Chill Enhancer";
            Hue = 1152; // Ice blue
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your freezing effects last 5% longer!");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Chill Enhancer effect has worn off.");
            });

            return true;
        }

        public ChillEnhancer(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Crescendo Enhancement Potions (Bard)

    /// <summary>
    /// Crescendo Catalyst - Increases Crescendo generation by 35%
    /// </summary>
    public class CrescendoCatalyst : ResourceEnhancementPotion
    {
        public override string EffectDescription => "Crescendo gen +35%";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 900; // 15 minutes

        [Constructable]
        public CrescendoCatalyst() : this(1) { }

        [Constructable]
        public CrescendoCatalyst(int amount) : base(0x0F0E)
        {
            Name = "Crescendo Catalyst";
            Hue = 1161; // Golden
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "Your songs build Crescendo 35% faster!");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Crescendo Catalyst effect has worn off.");
            });

            return true;
        }

        public CrescendoCatalyst(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Virtue Enhancement Potions (Paladin)

    /// <summary>
    /// Virtue Essence - Increases one Virtue by 1
    /// </summary>
    public class VirtueEssence : ResourceEnhancementPotion
    {
        public override string EffectDescription => "+1 to one Virtue";
        public override int Duration => 600; // 10 minutes
        public override int Cooldown => 900; // 15 minutes

        [Constructable]
        public VirtueEssence() : this(1) { }

        [Constructable]
        public VirtueEssence(int amount) : base(0x0F0E)
        {
            Name = "Virtue Essence";
            Hue = 1153; // Holy white
            Amount = amount;
        }

        public override bool ApplyEffect(PlayerMobile pm)
        {
            pm.SendMessage(0x3B2, "You feel more virtuous! +1 to your primary Virtue.");

            Timer.DelayCall(TimeSpan.FromSeconds(Duration), () =>
            {
                if (pm != null && !pm.Deleted)
                    pm.SendMessage(0x22, "Your Virtue Essence effect has worn off.");
            });

            return true;
        }

        public VirtueEssence(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Resource Storage Flasks

    /// <summary>
    /// LifeForce Flask - Stores up to 35 LifeForce for Necromancers
    /// </summary>
    public class LifeForceFlask : ResourceStorageFlask
    {
        public override ResourceType StoredResourceType => ResourceType.LifeForce;
        public override int Capacity => 35;
        public override string ClassName => "Necromancer";

        [Constructable]
        public LifeForceFlask() : base(0x0E24) // Large flask
        {
            Name = "LifeForce Flask";
            Hue = 1175; // Bone/death
        }

        public LifeForceFlask(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Faith Vessel - Stores up to 35 Faith for Clerics
    /// </summary>
    public class FaithVessel : ResourceStorageFlask
    {
        public override ResourceType StoredResourceType => ResourceType.Faith;
        public override int Capacity => 35;
        public override string ClassName => "Cleric";

        [Constructable]
        public FaithVessel() : base(0x0E24)
        {
            Name = "Faith Vessel";
            Hue = 1153; // Holy white
        }

        public FaithVessel(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Steam Concentrate - Portable Steam storage with 35 charges for Artificers
    /// </summary>
    public class SteamConcentrate : ResourceStorageFlask
    {
        public override ResourceType StoredResourceType => ResourceType.Steam;
        public override int Capacity => 35;
        public override string ClassName => "Artificer";

        [Constructable]
        public SteamConcentrate() : base(0x0E24)
        {
            Name = "Steam Concentrate";
            Hue = 2305; // Ironclad metallic
        }

        public SteamConcentrate(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Transmutation Tool

    /// <summary>
    /// Transmutation Kit - Portable alchemy tool for crafting potions
    /// </summary>
    public class TransmutationKit : Item, ITool
    {
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining
        {
            get { return true; }
            set { }
        }

        public CraftSystem CraftSystem => DefTransmutation.CraftSystem;

        public bool BreakOnDepletion => true;

        public bool CheckAccessible(Mobile from, ref int num)
        {
            if (!IsChildOf(from.Backpack) && !IsChildOf(from.BankBox))
            {
                num = 1044263; // The tool must be on your person to use.
                return false;
            }
            return true;
        }

        [Constructable]
        public TransmutationKit() : this(50) { }

        [Constructable]
        public TransmutationKit(int uses) : base(0x1849) // Mortar and pestle
        {
            Name = "Transmutation Kit";
            Hue = 2010; // Verdantpeak green
            Weight = 2.0;
            m_UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            from.SendGump(new CraftGump(from, CraftSystem, this, null));
        }

        public TransmutationKit(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_UsesRemaining = reader.ReadInt();
        }
    }

    #endregion
}
