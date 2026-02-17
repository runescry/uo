using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Bard - Musical enchanter who buffs allies and debuffs enemies
    /// Region: Multi-Regional
    /// Role: Support/CC
    /// </summary>
    public class BardClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Bard;
        public override string ClassName => "Bard";
        public override string ClassDescription => "Musical enchanter who buffs allies and debuffs enemies";

        // Stats (Total: 80) - Support/CC (DEX/INT hybrid)
        public override int StartStr => 15;  // Low strength
        public override int StartDex => 35;  // High dexterity (performance)
        public override int StartInt => 30;  // Medium-high intelligence

        // Stat Caps
        public override int StrCap => 100;
        public override int DexCap => 125;
        public override int IntCap => 115;

        // Primary Skills (7 skills)
        public override SkillName[] PrimarySkills =>
#if VYSTIA_SONGWEAVING
            new SkillName[]
            {
                SkillName.Songweaving, // Primary class skill - song power
                SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation, SkillName.Discordance, SkillName.Magery, SkillName.MagicResist
            };
#else
            new SkillName[]
            {
                SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation, SkillName.Discordance, SkillName.Magery, SkillName.MagicResist
            };
#endif

        public override double[] StartingSkillValues =>
#if VYSTIA_SONGWEAVING
            new double[]
            {
                50.0, // Songweaving
                60.0, 50.0, 45.0, 40.0, 30.0, 15.0
            };
#else
            new double[]
            {
                60.0, 50.0, 45.0, 40.0, 30.0, 15.0
            };
#endif

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Leather Armor
            m.AddItem(new LeatherChest() { Name = "Bard's Jerkin", Hue = 1153 });
        m.AddItem(new LeatherLegs() { Hue = 1153 });
        m.AddItem(new LeatherArms() { Hue = 1153 });
        m.AddItem(new LeatherGloves() { Hue = 1153 });
        m.AddItem(new LeatherGorget() { Hue = 1153 });
        m.AddItem(new LeatherCap() { Hue = 1153 });

            // Weapon
            m.AddItem(new Dagger() { Name = "Bard's Dagger", Hue = 1153 });

            // Starting Resources
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Special Class Item
#if VYSTIA_SONGWEAVING
            MagicLute item = new MagicLute();
            // m.Backpack.DropItem(item);
            // Give class songbook
            SongweavingSpellbook songbook = new SongweavingSpellbook();
            m.Backpack.DropItem(songbook);
#endif
        }
    }
}
