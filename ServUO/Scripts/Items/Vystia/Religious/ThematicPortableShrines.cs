// Vystia Thematic Portable Shrines
// 6 unique portable shrines, one per religion, with themed graphics and effects

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Items.Vystia
{
    #region Cogsmith Creed - Cogsmith's Portable Anvil

    /// <summary>
    /// Cogsmith's Portable Anvil - Cogsmith Creed religion portable shrine
    /// Provides crafting bonuses when placed
    /// </summary>
    public class CogsmithePortableAnvil : Item
    {
        private bool m_IsPlaced;
        private DateTime m_PlacedTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPlaced
        {
            get { return m_IsPlaced; }
            set { m_IsPlaced = value; InvalidateProperties(); }
        }

        [Constructable]
        public CogsmithePortableAnvil() : base(0xFAF) // Anvil graphic
        {
            Name = "Cogsmith's Portable Anvil";
            Hue = 2305; // Ironclad metallic
            Weight = 15.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != VystiaReligion.CogsmithCreed)
            {
                pm.SendMessage(0x22, "You must follow the Cogsmith Creed to use this shrine.");
                return;
            }

            if (m_IsPlaced)
            {
                // Use as shrine
                pm.SendMessage(0x3B2, "You feel the blessing of the Forge!");
                pm.FixedParticles(0x376A, 9, 32, 5005, 2305, 0, EffectLayer.Waist);
                pm.PlaySound(0x2A);

                // Grant 5% crafting bonus for 10 minutes
                pm.AddStatMod(new StatMod(StatType.Str, "ForgeBlessing", 5, TimeSpan.FromMinutes(10)));
                pm.SendMessage(0x3B2, "You receive +5 STR for crafting for 10 minutes.");

                // Check if can pick up
                if (DateTime.UtcNow - m_PlacedTime > TimeSpan.FromMinutes(1))
                {
                    pm.SendMessage("You can pick up the anvil by double-clicking again.");
                }
            }
            else
            {
                // Place the shrine
                PlaceShrine(pm);
            }
        }

        private void PlaceShrine(PlayerMobile pm)
        {
            foreach (Item item in pm.Map.GetItemsInRange(pm.Location, 10))
            {
                if (item is VystiaShrine || item is CogsmithePortableAnvil || item is PortableShrine)
                {
                    pm.SendMessage(0x22, "There is already a shrine nearby.");
                    return;
                }
            }

            m_IsPlaced = true;
            m_PlacedTime = DateTime.UtcNow;
            Movable = false;
            pm.SendMessage(0x35, "You place the Cogsmith's Portable Anvil.");
            pm.PlaySound(0x2A);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\tCogsmith Creed");
            list.Add(m_IsPlaced ? "Status: Placed" : "Status: Portable");
            list.Add("Grants crafting blessings when placed");
        }

        public CogsmithePortableAnvil(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_IsPlaced);
            writer.Write(m_PlacedTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_IsPlaced = reader.ReadBool();
            m_PlacedTime = reader.ReadDateTime();
        }
    }

    #endregion

    #region Lunara's Covenant - Moonstone Circle

    /// <summary>
    /// Moonstone Circle - Lunara's Covenant / Lunara's Covenant portable shrine
    /// Provides healing bonus when placed
    /// </summary>
    public class MoonstoneCircle : Item
    {
        private bool m_IsPlaced;
        private DateTime m_PlacedTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPlaced
        {
            get { return m_IsPlaced; }
            set { m_IsPlaced = value; InvalidateProperties(); }
        }

        [Constructable]
        public MoonstoneCircle() : base(0x1F13) // Crystal/stone circle graphic
        {
            Name = "Moonstone Circle";
            Hue = 1153; // Moonlight silver
            Weight = 10.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != VystiaReligion.LunarasCovenant)
            {
                pm.SendMessage(0x22, "You must follow Lunara's Covenant to use this shrine.");
                return;
            }

            if (m_IsPlaced)
            {
                pm.SendMessage(0x3B2, "The moonlight soothes your wounds...");
                pm.FixedParticles(0x376A, 9, 32, 5005, 1153, 0, EffectLayer.Waist);
                pm.PlaySound(0x1E3);

                // Heal 25% of max HP
                int heal = pm.HitsMax / 4;
                pm.Heal(heal);
                pm.SendMessage(0x3B2, $"You are healed for {heal} hit points.");
            }
            else
            {
                PlaceShrine(pm);
            }
        }

        private void PlaceShrine(PlayerMobile pm)
        {
            foreach (Item item in pm.Map.GetItemsInRange(pm.Location, 10))
            {
                if (item is VystiaShrine || item is MoonstoneCircle || item is PortableShrine)
                {
                    pm.SendMessage(0x22, "There is already a shrine nearby.");
                    return;
                }
            }

            m_IsPlaced = true;
            m_PlacedTime = DateTime.UtcNow;
            Movable = false;
            pm.SendMessage(0x35, "You place the Moonstone Circle.");
            pm.PlaySound(0x1E3);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\tLunara's Covenant");
            list.Add(m_IsPlaced ? "Status: Placed" : "Status: Portable");
            list.Add("Provides healing when activated");
        }

        public MoonstoneCircle(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_IsPlaced);
            writer.Write(m_PlacedTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_IsPlaced = reader.ReadBool();
            m_PlacedTime = reader.ReadDateTime();
        }
    }

    #endregion

    #region Surya's Sandscript - Sun Dial

    /// <summary>
    /// Sun Dial - Surya's Sandscript / Surya's Sandscript portable shrine
    /// Provides fire damage bonus when placed
    /// </summary>
    public class SunDialShrine : Item
    {
        private bool m_IsPlaced;
        private DateTime m_PlacedTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPlaced
        {
            get { return m_IsPlaced; }
            set { m_IsPlaced = value; InvalidateProperties(); }
        }

        [Constructable]
        public SunDialShrine() : base(0x1089) // Sundial graphic
        {
            Name = "Sun Dial of Surya";
            Hue = 1161; // Golden/solar
            Weight = 12.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != VystiaReligion.SuryasSandscript)
            {
                pm.SendMessage(0x22, "You must follow Surya's Sandscript to use this shrine.");
                return;
            }

            if (m_IsPlaced)
            {
                pm.SendMessage(0x3B2, "The sun's power flows through you!");
                pm.FixedParticles(0x3709, 10, 30, 5052, 1161, 0, EffectLayer.LeftFoot);
                pm.PlaySound(0x208);

                // Grant fire damage bonus for 5 minutes
                pm.AddStatMod(new StatMod(StatType.Int, "SunBlessing", 10, TimeSpan.FromMinutes(5)));
                pm.SendMessage(0x3B2, "+10 INT for 5 minutes (fire magic enhanced).");
            }
            else
            {
                PlaceShrine(pm);
            }
        }

        private void PlaceShrine(PlayerMobile pm)
        {
            foreach (Item item in pm.Map.GetItemsInRange(pm.Location, 10))
            {
                if (item is VystiaShrine || item is SunDialShrine || item is PortableShrine)
                {
                    pm.SendMessage(0x22, "There is already a shrine nearby.");
                    return;
                }
            }

            m_IsPlaced = true;
            m_PlacedTime = DateTime.UtcNow;
            Movable = false;
            pm.SendMessage(0x35, "You place the Sun Dial.");
            pm.PlaySound(0x208);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\tSurya's Sandscript");
            list.Add(m_IsPlaced ? "Status: Placed" : "Status: Portable");
            list.Add("Enhances fire magic");
        }

        public SunDialShrine(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_IsPlaced);
            writer.Write(m_PlacedTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_IsPlaced = reader.ReadBool();
            m_PlacedTime = reader.ReadDateTime();
        }
    }

    #endregion

    #region Oceana's Covenant - Tide Pool Basin

    /// <summary>
    /// Tide Pool Basin - Oceana's Covenant / Oceana's Covenant portable shrine
    /// Provides water/cold resistance when placed
    /// </summary>
    public class TidePoolBasin : Item
    {
        private bool m_IsPlaced;
        private DateTime m_PlacedTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPlaced
        {
            get { return m_IsPlaced; }
            set { m_IsPlaced = value; InvalidateProperties(); }
        }

        [Constructable]
        public TidePoolBasin() : base(0x0E7C) // Basin/water container graphic
        {
            Name = "Tide Pool Basin";
            Hue = 1152; // Ocean blue
            Weight = 8.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != VystiaReligion.OceanasCovenant)
            {
                pm.SendMessage(0x22, "You must follow Oceana's Covenant to use this shrine.");
                return;
            }

            if (m_IsPlaced)
            {
                pm.SendMessage(0x3B2, "The waters of the abyss protect you!");
                pm.FixedParticles(0x376A, 9, 32, 5005, 1152, 0, EffectLayer.Waist);
                pm.PlaySound(0x026);

                // Restore mana
                int manaRestore = pm.ManaMax / 4;
                pm.Mana = Math.Min(pm.ManaMax, pm.Mana + manaRestore);
                pm.SendMessage(0x3B2, $"You restore {manaRestore} mana.");
            }
            else
            {
                PlaceShrine(pm);
            }
        }

        private void PlaceShrine(PlayerMobile pm)
        {
            foreach (Item item in pm.Map.GetItemsInRange(pm.Location, 10))
            {
                if (item is VystiaShrine || item is TidePoolBasin || item is PortableShrine)
                {
                    pm.SendMessage(0x22, "There is already a shrine nearby.");
                    return;
                }
            }

            m_IsPlaced = true;
            m_PlacedTime = DateTime.UtcNow;
            Movable = false;
            pm.SendMessage(0x35, "You place the Tide Pool Basin.");
            pm.PlaySound(0x026);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\tOceana's Covenant");
            list.Add(m_IsPlaced ? "Status: Placed" : "Status: Portable");
            list.Add("Restores mana when activated");
        }

        public TidePoolBasin(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_IsPlaced);
            writer.Write(m_PlacedTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_IsPlaced = reader.ReadBool();
            m_PlacedTime = reader.ReadDateTime();
        }
    }

    #endregion

    #region Celestis Arcanum - Star Chart

    /// <summary>
    /// Star Chart - Celestis Arcanum / Celestis Arcanum portable shrine
    /// Provides arcane power when placed
    /// </summary>
    public class StarChartShrine : Item
    {
        private bool m_IsPlaced;
        private DateTime m_PlacedTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPlaced
        {
            get { return m_IsPlaced; }
            set { m_IsPlaced = value; InvalidateProperties(); }
        }

        [Constructable]
        public StarChartShrine() : base(0x14ED) // Map/chart graphic
        {
            Name = "Star Chart of Celestis";
            Hue = 1154; // Crystal blue
            Weight = 3.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != VystiaReligion.CelestisArcanum)
            {
                pm.SendMessage(0x22, "You must follow the Celestis Arcanum to use this shrine.");
                return;
            }

            if (m_IsPlaced)
            {
                pm.SendMessage(0x3B2, "The stars align in your favor!");
                pm.FixedParticles(0x376A, 9, 32, 5005, 1154, 0, EffectLayer.Waist);
                pm.PlaySound(0x1E9);

                // Grant spell power bonus
                pm.AddStatMod(new StatMod(StatType.Int, "StarBlessing", 15, TimeSpan.FromMinutes(5)));
                pm.SendMessage(0x3B2, "+15 INT for 5 minutes (spell power enhanced).");
            }
            else
            {
                PlaceShrine(pm);
            }
        }

        private void PlaceShrine(PlayerMobile pm)
        {
            foreach (Item item in pm.Map.GetItemsInRange(pm.Location, 10))
            {
                if (item is VystiaShrine || item is StarChartShrine || item is PortableShrine)
                {
                    pm.SendMessage(0x22, "There is already a shrine nearby.");
                    return;
                }
            }

            m_IsPlaced = true;
            m_PlacedTime = DateTime.UtcNow;
            Movable = false;
            pm.SendMessage(0x35, "You unfurl the Star Chart.");
            pm.PlaySound(0x1E9);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\tCelestis Arcanum");
            list.Add(m_IsPlaced ? "Status: Placed" : "Status: Portable");
            list.Add("Enhances spell power");
        }

        public StarChartShrine(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_IsPlaced);
            writer.Write(m_PlacedTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_IsPlaced = reader.ReadBool();
            m_PlacedTime = reader.ReadDateTime();
        }
    }

    #endregion

    #region Frosthelm Faith - Frost Totem

    /// <summary>
    /// Frost Totem - Frosthelm Faith / Frosthelm Faith portable shrine
    /// Provides cold resistance and damage when placed
    /// </summary>
    public class FrostTotemShrine : Item
    {
        private bool m_IsPlaced;
        private DateTime m_PlacedTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPlaced
        {
            get { return m_IsPlaced; }
            set { m_IsPlaced = value; InvalidateProperties(); }
        }

        [Constructable]
        public FrostTotemShrine() : base(0x12CB) // Totem pole graphic
        {
            Name = "Frost Totem";
            Hue = 1152; // Ice blue
            Weight = 20.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != VystiaReligion.FrosthelmFaith)
            {
                pm.SendMessage(0x22, "You must follow the Frosthelm Faith to use this shrine.");
                return;
            }

            if (m_IsPlaced)
            {
                pm.SendMessage(0x3B2, "The cold empowers you!");
                pm.FixedParticles(0x376A, 9, 32, 5005, 1152, 0, EffectLayer.Waist);
                pm.PlaySound(0x1E0);

                // Grant cold resistance and damage bonus
                pm.AddStatMod(new StatMod(StatType.Str, "FrostBlessing", 10, TimeSpan.FromMinutes(5)));
                pm.SendMessage(0x3B2, "+10 STR for 5 minutes (cold damage enhanced).");

                // Also restore some stamina
                int stamRestore = pm.StamMax / 4;
                pm.Stam = Math.Min(pm.StamMax, pm.Stam + stamRestore);
                pm.SendMessage(0x3B2, $"You restore {stamRestore} stamina.");
            }
            else
            {
                PlaceShrine(pm);
            }
        }

        private void PlaceShrine(PlayerMobile pm)
        {
            foreach (Item item in pm.Map.GetItemsInRange(pm.Location, 10))
            {
                if (item is VystiaShrine || item is FrostTotemShrine || item is PortableShrine)
                {
                    pm.SendMessage(0x22, "There is already a shrine nearby.");
                    return;
                }
            }

            m_IsPlaced = true;
            m_PlacedTime = DateTime.UtcNow;
            Movable = false;
            pm.SendMessage(0x35, "You plant the Frost Totem.");
            pm.PlaySound(0x1E0);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\tFrosthelm Faith");
            list.Add(m_IsPlaced ? "Status: Placed" : "Status: Portable");
            list.Add("Enhances cold damage and restores stamina");
        }

        public FrostTotemShrine(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_IsPlaced);
            writer.Write(m_PlacedTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_IsPlaced = reader.ReadBool();
            m_PlacedTime = reader.ReadDateTime();
        }
    }

    #endregion
}

