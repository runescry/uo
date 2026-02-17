using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Shaman - Spiritual guide who channels elemental spirits
    /// Region: Multi-Regional
    /// Role: Healer/Hybrid
    /// </summary>
    public class ShamanClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Shaman;
        public override string ClassName => "Shaman";
        public override string ClassDescription => "Spiritual guide who channels elemental spirits";

        // Stats (Total: 80) - Healer/Hybrid
        public override int StartStr => 20;  // Low-medium strength
        public override int StartDex => 20;  // Medium dexterity
        public override int StartInt => 40;  // High intelligence (spirit magic)

        // Stat Caps
        public override int StrCap => 110;
        public override int DexCap => 110;
        public override int IntCap => 130;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.SpiritCalling, // Primary class skill - totem power
            SkillName.Magery, SkillName.SpiritSpeak, SkillName.Meditation, SkillName.Healing, SkillName.MagicResist, SkillName.AnimalLore
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0, // SpiritCalling
            45.0, 50.0, 45.0, 40.0, 40.0, 20.0
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Leather Armor
            m.AddItem(new LeatherChest() { Name = "Shaman's Jerkin", Hue = 1153 });
        m.AddItem(new LeatherLegs() { Hue = 1153 });
        m.AddItem(new LeatherArms() { Hue = 1153 });
        m.AddItem(new LeatherGloves() { Hue = 1153 });
        m.AddItem(new LeatherGorget() { Hue = 1153 });
        m.AddItem(new LeatherCap() { Hue = 1153 });

            // Weapon
            m.AddItem(new ShepherdsCrook() { Name = "Shaman's ShepherdsCrook", Hue = 1153 });

            // Starting Resources
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
            SpiritTotem item = new SpiritTotem();
        //         m.Backpack.DropItem(item);
        // Give class spellbook
        ShamanSpellbook spellbook = new ShamanSpellbook();
        m.Backpack.DropItem(spellbook);
        }
    }
}
