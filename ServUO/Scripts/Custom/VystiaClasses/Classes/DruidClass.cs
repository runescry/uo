using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    /// <summary>
    /// Druid - Guardian of Verdantpeak
    /// Low STR, medium DEX, high WIS (INT)
    /// Focus: Nature magic, shapeshifting, healing
    /// </summary>
    public class DruidClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Druid;
        public override string ClassName => "Druid";
        public override string ClassDescription => "Guardian of Verdantpeak - Master of nature magic and shapeshifting";

        // Stats (Total: 80) - Hybrid caster/healer
        public override int StartStr => 20;  // Low strength
        public override int StartDex => 20;  // Medium dexterity
        public override int StartInt => 40;  // High intelligence (nature magic)

        public override int StrCap => 100;
        public override int DexCap => 115;
        public override int IntCap => 135;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Druidism,    // Primary class skill - nature magic, shapeshifting
            SkillName.Magery,
            SkillName.EvalInt,
            SkillName.AnimalLore,
            SkillName.AnimalTaming,
            SkillName.Veterinary,
            SkillName.Meditation
        };

        public override double[] StartingSkillValues => new double[]
        {
            50.0,  // Druidism
            45.0,  // Magery
            45.0,  // Eval Int
            40.0,  // Animal Lore
            40.0,  // Animal Taming
            40.0,  // Veterinary
            30.0   // Meditation
        };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null)
                return;

            // Druid robes
            Robe robe = new Robe();
            robe.Name = "Druid's Robes";
            robe.Hue = 2010; // Forest green
            m.AddItem(robe);

            // Leafweave cloak
            Cloak cloak = new Cloak();
            cloak.Name = "Leafweave Cloak";
            cloak.Hue = 2010;
            m.AddItem(cloak);

            // Sandals
            Sandals sandals = new Sandals();
            sandals.Hue = 2010;
            m.AddItem(sandals);

            // Quarterstaff
            QuarterStaff staff = new QuarterStaff();
            staff.Name = "Druid's Staff";
            staff.Hue = 2010;
            m.AddItem(staff);

            // Nature reagents
            m.Backpack.DropItem(new BagOfReagents(50));
            m.Backpack.DropItem(new LivingBark(20));
            m.Backpack.DropItem(new TreantHeart(5));

            // Shapeshifting totem
            ShapeshiftTotem totem = new ShapeshiftTotem();
            m.Backpack.DropItem(totem);

            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            DruidSpellbook book = new DruidSpellbook();
            m.Backpack.DropItem(book);
        }
    }
}
