using System;
using Server;
using Server.Items;
using Server.Items.VystiaClassItems;

namespace Server.Custom.VystiaClasses
{
    // ==================== RANGER ====================
    public class RangerClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Ranger;
        public override string ClassName => "Ranger";
        public override string ClassDescription => "Dune Strider - Master archer and desert tracker";

        public override int StartStr => 25;
        public override int StartDex => 45;
        public override int StartInt => 10;

        public override int StrCap => 110;
        public override int DexCap => 145;
        public override int IntCap => 95;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Marksmanship, // Primary class skill - focus, aspects
            SkillName.Archery, SkillName.Tactics, SkillName.Anatomy,
            SkillName.Tracking, SkillName.Hiding, SkillName.Stealth
        };

        public override double[] StartingSkillValues => new double[]
        { 50.0, 50.0, 50.0, 40.0, 40.0, 35.0, 25.0 };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new LeatherChest() { Name = "Ranger's Jerkin", Hue = 1719 });
            m.AddItem(new LeatherLegs() { Name = "Ranger's Leggings", Hue = 1719 });
            m.AddItem(new LeatherGloves() { Hue = 1719 });
            m.AddItem(new LeatherArms() { Hue = 1719 });
            m.AddItem(new LeatherCap() { Name = "Ranger's Cap", Hue = 1719 });
            m.AddItem(new ThighBoots() { Hue = 1719 });

            Bow bow = new Bow();
            bow.Name = "Ranger's Longbow";
            bow.Hue = 1719;
            m.AddItem(bow);

            m.Backpack.DropItem(new Arrow(100));
            m.Backpack.DropItem(new Dagger() { Name = "Ranger's Knife", Hue = 1719 });
            m.Backpack.DropItem(new Gold(100));
        }
    }

    // ==================== FIGHTER ====================
    public class FighterClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Fighter;
        public override string ClassName => "Fighter";
        public override string ClassDescription => "Ironclad Legionnaire - Elite soldier and weapons master";

        public override int StartStr => 40;
        public override int StartDex => 25;
        public override int StartInt => 15;

        public override int StrCap => 140;
        public override int DexCap => 110;
        public override int IntCap => 100;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.CombatMastery, // Primary class skill - stance effectiveness
            SkillName.Swords, SkillName.Tactics, SkillName.Anatomy,
            SkillName.Parry, SkillName.Healing, SkillName.MagicResist
        };

        public override double[] StartingSkillValues => new double[]
        { 50.0, 50.0, 50.0, 40.0, 40.0, 35.0, 25.0 };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new PlateChest() { Name = "Legionnaire's Cuirass", Hue = 2305 });
            m.AddItem(new PlateLegs() { Name = "Legionnaire's Greaves", Hue = 2305 });
            m.AddItem(new PlateArms() { Name = "Legionnaire's Vambraces", Hue = 2305 });
            m.AddItem(new PlateGloves() { Hue = 2305 });
            m.AddItem(new PlateHelm() { Name = "Legionnaire's Helm", Hue = 2305 });
            m.AddItem(new PlateGorget() { Hue = 2305 });
            m.AddItem(new Boots() { Hue = 2305 });

            Longsword sword = new Longsword();
            sword.Name = "Legionnaire's Blade";
            sword.Hue = 2305;
            m.AddItem(sword);

            HeaterShield shield = new HeaterShield();
            shield.Name = "Legionnaire's Shield";
            shield.Hue = 2305;
            m.AddItem(shield);

            m.Backpack.DropItem(new Bandage(50));
            m.Backpack.DropItem(new Gold(150));
        }
    }

    // ==================== WIZARD ====================
    public class WizardClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Wizard;
        public override string ClassName => "Wizard";
        public override string ClassDescription => "Arcane Scholar - Master of all schools of magic";

        public override int StartStr => 10;
        public override int StartDex => 20;
        public override int StartInt => 50;

        public override int StrCap => 85;
        public override int DexCap => 110;
        public override int IntCap => 155;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.ArcaneStudies, // Primary class skill - spell mastery
            SkillName.Magery, SkillName.EvalInt, SkillName.Meditation,
            SkillName.Inscribe, SkillName.MagicResist, SkillName.Wrestling
        };

        public override double[] StartingSkillValues => new double[]
        { 50.0, 55.0, 50.0, 45.0, 40.0, 35.0, 15.0 };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Wizard's Robes", Hue = 1154 });
            m.AddItem(new WizardsHat() { Name = "Arcane Hat", Hue = 1154 });
            m.AddItem(new Sandals() { Hue = 1154 });
            m.AddItem(new GnarledStaff() { Name = "Wizard's Staff", Hue = 1154 });

            m.Backpack.DropItem(new Spellbook());
            m.Backpack.DropItem(new BagOfReagents(100));
            m.Backpack.DropItem(new BlankScroll(50));
            m.Backpack.DropItem(new Gold(100));
        }
    }

    // ==================== CLERIC ====================
    public class ClericClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Cleric;
        public override string ClassName => "Cleric";
        public override string ClassDescription => "Priest of Elemental Balance - Divine healer and protector";

        public override int StartStr => 20;
        public override int StartDex => 20;
        public override int StartInt => 40;

        public override int StrCap => 105;
        public override int DexCap => 110;
        public override int IntCap => 140;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.DivineGrace, // Primary class skill - faith, healing
            SkillName.Magery, SkillName.EvalInt, SkillName.Meditation,
            SkillName.Macing, SkillName.MagicResist, SkillName.SpiritSpeak
        };

        public override double[] StartingSkillValues => new double[]
        { 50.0, 50.0, 45.0, 40.0, 35.0, 40.0, 30.0 };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Cleric's Vestments", Hue = 1153 });
            m.AddItem(new ChainChest() { Name = "Blessed Chainmail", Hue = 1153 });
            m.AddItem(new ChainLegs() { Hue = 1153 });
            m.AddItem(new Boots() { Hue = 1153 });
            m.AddItem(new WarMace() { Name = "Holy Mace", Hue = 1153 });

            m.Backpack.DropItem(new Spellbook());
            m.Backpack.DropItem(new BagOfReagents(50));
            m.Backpack.DropItem(new Bandage(50));
            m.Backpack.DropItem(new Gold(100));
        }

        public override void GiveStartingAbilities(Mobile m)
        {
            // Clerics get a holy symbol
            HolySymbol symbol = new HolySymbol();
            m.Backpack.DropItem(symbol);
        }
    }

    // ==================== PALADIN ====================
    public class PaladinClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Paladin;
        public override string ClassName => "Paladin";
        public override string ClassDescription => "Knight of the Cogsmith Creed - Holy warrior and protector";

        public override int StartStr => 35;
        public override int StartDex => 20;
        public override int StartInt => 25;

        public override int StrCap => 130;
        public override int DexCap => 105;
        public override int IntCap => 120;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.HolyDevotion, // Primary class skill - virtue power
            SkillName.Swords, SkillName.Tactics, SkillName.Chivalry,
            SkillName.Parry, SkillName.MagicResist, SkillName.Healing
        };

        public override double[] StartingSkillValues => new double[]
        { 50.0, 50.0, 50.0, 45.0, 40.0, 30.0, 25.0 };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new PlateChest() { Name = "Paladin's Cuirass", Hue = 1153 });
            m.AddItem(new PlateLegs() { Name = "Paladin's Greaves", Hue = 1153 });
            m.AddItem(new PlateArms() { Hue = 1153 });
            m.AddItem(new PlateGloves() { Hue = 1153 });
            m.AddItem(new PlateHelm() { Name = "Paladin's Helm", Hue = 1153 });
            m.AddItem(new PlateGorget() { Hue = 1153 });

            Longsword sword = new Longsword();
            sword.Name = "Paladin's Blade";
            sword.Hue = 1153;
            m.AddItem(sword);

            OrderShield shield = new OrderShield();
            shield.Name = "Paladin's Shield";
            shield.Hue = 1153;
            m.AddItem(shield);

            m.Backpack.DropItem(new BookOfChivalry());
            m.Backpack.DropItem(new Bandage(50));
            m.Backpack.DropItem(new Gold(150));
        }
    }

    // ==================== WITCH ====================
    public class WitchClass : PlayerClass
    {
        public override PlayerClassType ClassType => PlayerClassType.Witch;
        public override string ClassName => "Witch";
        public override string ClassDescription => "Bog Witch - Master of hexes, curses, and swamp magic";

        public override int StartStr => 15;
        public override int StartDex => 20;
        public override int StartInt => 45;

        public override int StrCap => 90;
        public override int DexCap => 110;
        public override int IntCap => 145;

        public override SkillName[] PrimarySkills => new SkillName[]
        {
            SkillName.Hexcraft, // Primary class skill - curse potency
            SkillName.Magery, SkillName.EvalInt, SkillName.Poisoning,
            SkillName.Meditation, SkillName.SpiritSpeak, SkillName.Alchemy
        };

        public override double[] StartingSkillValues => new double[]
        { 50.0, 50.0, 45.0, 40.0, 40.0, 35.0, 30.0 };

        public override void EquipStartingGear(Mobile m)
        {
            if (m == null || m.Backpack == null) return;

            m.AddItem(new Robe() { Name = "Witch's Robes", Hue = 2073 });
            m.AddItem(new WizardsHat() { Name = "Witch's Hat", Hue = 2073 });
            m.AddItem(new Sandals() { Hue = 2073 });
            m.AddItem(new GnarledStaff() { Name = "Witch's Staff", Hue = 2073 });

            WitchSpellbook book = new WitchSpellbook();
            m.Backpack.DropItem(book);

            m.Backpack.DropItem(new BagOfReagents(50));
            m.Backpack.DropItem(new SwampLotus(20)); // Shadowfen nature reagent
            m.Backpack.DropItem(new BogIronOre(10)); // Shadowfen ore
            m.Backpack.DropItem(new GreaterPoisonPotion(10));
            m.Backpack.DropItem(new Gold(100));
        }
    }
}

