using System;
using Server.Targeting;
using Server.Engines.Craft;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// Base class for all Vystia regional ores.
    /// These are standalone items that drop from creatures and can be used for crafting.
    /// Double-click and target a forge to smelt into regional ingots.
    /// </summary>
    public abstract class BaseVystiaOre : Item
    {
        private string m_Region;
        private double m_MiningSkillRequired;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Region { get { return m_Region; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double MiningSkillRequired { get { return m_MiningSkillRequired; } }

        /// <summary>
        /// Override in derived classes to return the appropriate ingot type
        /// </summary>
        public abstract BaseVystiaIngot GetIngot();

        public BaseVystiaOre(int hue, string region, double miningSkill)
            : this(1, hue, region, miningSkill)
        {
        }

        public BaseVystiaOre(int amount, int hue, string region, double miningSkill)
            : base(0x19B9) // Large ore graphic
        {
            Stackable = true;
            Amount = amount;
            Hue = hue;
            m_Region = region;
            m_MiningSkillRequired = miningSkill;
        }

        public BaseVystiaOre(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!Movable)
                return;

            if (RootParent is BaseCreature)
            {
                from.SendLocalizedMessage(500447); // That is not accessible
            }
            else if (from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage("Select the forge on which to smelt the ore.");
                from.Target = new VystiaOreSmeltTarget(this);
            }
            else
            {
                from.SendMessage("The ore is too far away.");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_Region);
            writer.Write(m_MiningSkillRequired);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Region = reader.ReadString();
            m_MiningSkillRequired = reader.ReadDouble();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: {0}", m_Region);
            list.Add("Mining Skill: {0:F1}", m_MiningSkillRequired);
        }
    }

    /// <summary>
    /// Target handler for smelting Vystia regional ores at a forge.
    /// </summary>
    public class VystiaOreSmeltTarget : Target
    {
        private readonly BaseVystiaOre m_Ore;

        public VystiaOreSmeltTarget(BaseVystiaOre ore)
            : base(2, false, TargetFlags.None)
        {
            m_Ore = ore;
        }

        private bool IsForge(object obj)
        {
            if (obj.GetType().IsDefined(typeof(ForgeAttribute), false))
                return true;

            int itemID = 0;

            if (obj is Item)
                itemID = ((Item)obj).ItemID;
            else if (obj is StaticTarget)
                itemID = ((StaticTarget)obj).ItemID;

            // Standard forge IDs
            return (itemID == 4017 || (itemID >= 6522 && itemID <= 6569));
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Ore.Deleted)
                return;

            if (!from.InRange(m_Ore.GetWorldLocation(), 2))
            {
                from.SendMessage("The ore is too far away.");
                return;
            }

            // Combine ore piles of same type
            if (targeted is BaseVystiaOre)
            {
                BaseVystiaOre targetOre = (BaseVystiaOre)targeted;

                if (!targetOre.Movable)
                    return;

                if (m_Ore == targetOre)
                {
                    from.SendMessage("Select another pile of ore to combine.");
                    from.Target = new VystiaOreSmeltTarget(targetOre);
                    return;
                }

                if (targetOre.GetType() != m_Ore.GetType())
                {
                    from.SendMessage("You cannot combine different types of ore.");
                    return;
                }

                // Combine the ore
                targetOre.Amount += m_Ore.Amount;
                m_Ore.Delete();
                from.SendMessage("You combine the ore piles.");
                return;
            }

            if (IsForge(targeted))
            {
                double difficulty = m_Ore.MiningSkillRequired;
                double minSkill = difficulty - 25.0;
                double maxSkill = difficulty + 25.0;

                if (difficulty > from.Skills[SkillName.Mining].Value)
                {
                    from.SendMessage("You have no idea how to smelt this strange ore!");
                    return;
                }

                if (m_Ore.Amount < 2)
                {
                    from.SendMessage("There is not enough ore to smelt into an ingot.");
                    return;
                }

                if (from.CheckTargetSkill(SkillName.Mining, targeted, minSkill, maxSkill))
                {
                    int toConsume = m_Ore.Amount;
                    if (toConsume > 30000)
                        toConsume = 30000;

                    // 2 ore = 1 ingot for Vystia ores
                    int ingotAmount = toConsume / 2;

                    if (ingotAmount < 1)
                    {
                        from.SendMessage("There is not enough ore to smelt into an ingot.");
                        return;
                    }

                    BaseVystiaIngot ingot = m_Ore.GetIngot();
                    ingot.Amount = ingotAmount;

                    m_Ore.Consume(ingotAmount * 2);
                    from.AddToBackpack(ingot);

                    from.PlaySound(0x2A); // Anvil sound
                    from.PlaySound(0x240); // Forge sound
                    from.SendMessage("You smelt the {0} ore into {1} ingot{2}.", m_Ore.Region.ToLower(), ingotAmount, ingotAmount > 1 ? "s" : "");
                }
                else
                {
                    // Failed - lose some ore
                    int lost = m_Ore.Amount / 2;
                    if (lost < 1)
                        lost = 1;

                    m_Ore.Amount -= lost;

                    if (m_Ore.Amount <= 0)
                        m_Ore.Delete();

                    from.SendMessage("You burn away the impurities but are left with less useable metal.");
                }
            }
            else
            {
                from.SendMessage("That is not a forge.");
            }
        }
    }

    /// <summary>
    /// Frozen Ore from Frosthold - smelts into Frostforged Ingots
    /// Cold damage +5 when crafted into equipment
    /// </summary>
    public class FrozenOre : BaseVystiaOre
    {
        [Constructable]
        public FrozenOre() : this(1) { }

        [Constructable]
        public FrozenOre(int amount) : base(amount, 1152, "Frosthold", 85.0) { }

        public FrozenOre(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "frozen ore"; } }

        public override BaseVystiaIngot GetIngot()
        {
            return new FrostforgedIngot();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Molten Ore from Emberlands - smelts into Emberforged Ingots
    /// Fire damage +5 when crafted into equipment
    /// </summary>
    public class MoltenOre : BaseVystiaOre
    {
        [Constructable]
        public MoltenOre() : this(1) { }

        [Constructable]
        public MoltenOre(int amount) : base(amount, 1358, "Emberlands", 90.0) { }

        public MoltenOre(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "molten ore"; } }

        public override BaseVystiaIngot GetIngot()
        {
            return new EmberforgedIngot();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Crystal Ore from Crystal Barrens - smelts into Crystalline Ingots
    /// Energy resist +5 when crafted into equipment
    /// </summary>
    public class CrystalOre : BaseVystiaOre
    {
        [Constructable]
        public CrystalOre() : this(1) { }

        [Constructable]
        public CrystalOre(int amount) : base(amount, 1154, "Crystal Barrens", 95.0) { }

        public CrystalOre(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "crystal ore"; } }

        public override BaseVystiaIngot GetIngot()
        {
            return new CrystallineIngot();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Steamwork Ore from Ironclad Empire - smelts into Clockwork Ingots
    /// Durability +25% when crafted into equipment
    /// </summary>
    public class SteamworkOre : BaseVystiaOre
    {
        [Constructable]
        public SteamworkOre() : this(1) { }

        [Constructable]
        public SteamworkOre(int amount) : base(amount, 2401, "Ironclad Empire", 80.0) { }

        public SteamworkOre(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "steamwork ore"; } }

        public override BaseVystiaIngot GetIngot()
        {
            return new ClockworkIngot();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Bog Iron Ore from Shadowfen - smelts into Shadowforged Ingots
    /// Self-repair 1 when crafted into equipment
    /// </summary>
    public class BogIronOre : BaseVystiaOre
    {
        [Constructable]
        public BogIronOre() : this(1) { }

        [Constructable]
        public BogIronOre(int amount) : base(amount, 2212, "Shadowfen", 75.0) { }

        public BogIronOre(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "bog iron ore"; } }

        public override BaseVystiaIngot GetIngot()
        {
            return new ShadowforgedIngot();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Sandstone Ore from Desert regions - smelts into Sunforged Ingots
    /// Weight -30% when crafted into equipment
    /// </summary>
    public class SandstoneOre : BaseVystiaOre
    {
        [Constructable]
        public SandstoneOre() : this(1) { }

        [Constructable]
        public SandstoneOre(int amount) : base(amount, 2305, "Desert", 70.0) { }

        public SandstoneOre(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "sandstone ore"; } }

        public override BaseVystiaIngot GetIngot()
        {
            return new SunforgedIngot();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Obsidian Ore from Obsidian Wastes - smelts into Voidforged Ingots
    /// Mage Armor when crafted into equipment
    /// </summary>
    public class ObsidianOre : BaseVystiaOre
    {
        [Constructable]
        public ObsidianOre() : this(1) { }

        [Constructable]
        public ObsidianOre(int amount) : base(amount, 1109, "Obsidian Wastes", 100.0) { }

        public ObsidianOre(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "obsidian ore"; } }

        public override BaseVystiaIngot GetIngot()
        {
            return new VoidforgedIngot();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Living Ore from Verdantpeak - smelts into Natureforged Ingots
    /// HP Regen +1 when crafted into equipment
    /// </summary>
    public class LivingOre : BaseVystiaOre
    {
        [Constructable]
        public LivingOre() : this(1) { }

        [Constructable]
        public LivingOre(int amount) : base(amount, 2010, "Verdantpeak", 65.0) { }

        public LivingOre(Serial serial) : base(serial) { }

        public override string DefaultName { get { return "living ore"; } }

        public override BaseVystiaIngot GetIngot()
        {
            return new NatureforgedIngot();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
