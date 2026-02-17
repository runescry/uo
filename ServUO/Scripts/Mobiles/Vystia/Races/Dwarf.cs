using System;
using Server.Items;
using Server.Commands;

namespace Server.Mobiles
{
    /// <summary>
    /// Dwarf NPC - uses custom 75% scaled human body (987/988)
    /// Equipped with plate armor (no helmet) and warhammer
    /// Equipment automatically converts to dwarf-sized via equipconv.def
    /// Requires patched anim.mul/anim.idx client files
    /// </summary>
    [CorpseName("a dwarf corpse")]
    public class Dwarf : BaseCreature
    {
        private static readonly string[] m_MaleNames = new string[]
        {
            "Thorin", "Balin", "Dwalin", "Gimli", "Gloin", "Oin", "Bifur", "Bofur", "Bombur",
            "Durin", "Thrain", "Dain", "Nain", "Fundin", "Farin", "Gror", "Fror", "Thror",
            "Borin", "Fili", "Kili", "Ori", "Nori", "Dori", "Floi", "Frerin", "Loni"
        };

        private static readonly string[] m_FemaleNames = new string[]
        {
            "Dis", "Hilda", "Sigrid", "Brunhild", "Ingrid", "Helga", "Freya", "Astrid",
            "Thyra", "Gudrun", "Ragna", "Torunn", "Brynhild", "Valdis", "Greta", "Eira"
        };

        private static readonly string[] m_Surnames = new string[]
        {
            "Ironforge", "Stonehammer", "Deepdelve", "Goldvein", "Copperpick", "Bronzebeard",
            "Steelaxe", "Ironhand", "Rockbreaker", "Oreseeker", "Gemcutter", "Anvildweller",
            "Coaldigger", "Forgemaster", "Hammerfall", "Mithrilhelm", "Silverpick", "Tunnelborn"
        };

        [Constructable]
        public Dwarf() : this(Utility.RandomBool())
        {
        }

        [Constructable]
        public Dwarf(bool female)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Female = female;

            string firstName = female
                ? m_FemaleNames[Utility.Random(m_FemaleNames.Length)]
                : m_MaleNames[Utility.Random(m_MaleNames.Length)];
            string surname = m_Surnames[Utility.Random(m_Surnames.Length)];
            Name = $"{firstName} {surname}";

            // Custom dwarf body - 75% scaled human sprites
            // Requires patched anim.mul/anim.idx files
            // Body 987/988 already exist in anim.mul and are HUMAN type in mobtypes.txt
            Body = female ? 988 : 987;  // Custom dwarf bodies
            Hue = Utility.RandomSkinHue();

            // Add hair and facial hair for dwarven look
            // Using pigtails (0x2049) - the only hair with 75% scaled dwarf animation
            HairItemID = 0x2049; // Two Pig Tails - has dwarf-sized animation (AnimID 902 -> 920)
            HairHue = Utility.RandomHairHue();

            if (!female)
            {
                // Long beards for dwarves - using the longer beard styles
                // 0x203E = Long beard, 0x203F = Short beard, 0x2040 = Goatee
                // 0x2041 = Mustache, 0x204B = Medium long beard, 0x204C = Vandyke
                // 0x204D = Long beard 2
                FacialHairItemID = Utility.RandomList(0x203E, 0x204B, 0x204D); // Long beards only!
                FacialHairHue = HairHue;
            }

            SetStr(150, 200);
            SetDex(60, 80);
            SetInt(80, 100);

            SetHits(120, 160);
            SetStam(60, 80);
            SetMana(80, 100);

            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 70.0, 85.0);
            SetSkill(SkillName.Tactics, 80.0, 95.0);
            SetSkill(SkillName.Anatomy, 70.0, 85.0);
            SetSkill(SkillName.Wrestling, 75.0, 90.0);
            SetSkill(SkillName.Mining, 80.0, 100.0);
            SetSkill(SkillName.Blacksmith, 80.0, 100.0);

            Fame = 2000;
            Karma = 2000; // Good aligned by default

            VirtualArmor = 35;

            // Dwarven equipment - uses equipconv.def to convert to dwarf-sized animations
            // Male: plate armor (527->909 chest, 528->910 arms, 529->911 legs, 530->912 gloves, 646->913 warhammer)
            // Female: leather armor (542->914 tunic, 543->915 legs, 544->916 sleeves, 545->917 gloves, 546->918 gorget)
            // NO HELMET - dwarves show off their magnificent beards!
            if (female)
            {
                AddItem(new LeatherChest());
                AddItem(new LeatherLegs());
                AddItem(new LeatherArms());
                AddItem(new LeatherGloves());
                AddItem(new LeatherGorget());
                AddItem(new WarHammer());
            }
            else
            {
                AddItem(new PlateChest());
                AddItem(new PlateLegs());
                AddItem(new PlateArms());
                AddItem(new PlateGloves());
                AddItem(new WarHammer());
            }
        }

        public Dwarf(Serial serial)
            : base(serial)
        {
        }

        // Use human sounds since we're using human body
        public override int GetAngerSound() { return Female ? 0x338 : 0x43F; }
        public override int GetIdleSound() { return Female ? 0x33B : 0x442; }
        public override int GetAttackSound() { return Female ? 0x337 : 0x43E; }
        public override int GetHurtSound() { return Female ? 0x33A : 0x441; }
        public override int GetDeathSound() { return Female ? 0x339 : 0x440; }

        public override bool CanRummageCorpses { get { return false; } }
        public override bool AlwaysMurderer { get { return false; } }
        public override bool ClickTitle { get { return false; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, 1);
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
    /// Command to spawn a dwarf at cursor location
    /// </summary>
    public class SpawnDwarfCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnDwarf", AccessLevel.GameMaster, SpawnDwarf_OnCommand);
            CommandSystem.Register("sd", AccessLevel.GameMaster, SpawnDwarf_OnCommand); // Shortcut (random gender)
            CommandSystem.Register("sdm", AccessLevel.GameMaster, SpawnDwarfMale_OnCommand); // Male dwarf
            CommandSystem.Register("sdf", AccessLevel.GameMaster, SpawnDwarfFemale_OnCommand); // Female dwarf
        }

        [Usage("SpawnDwarf")]
        [Description("Spawns a Dwarf at the targeted location.")]
        private static void SpawnDwarf_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Target the location to spawn a Dwarf.");
            from.Target = new DwarfSpawnTarget(null); // Random gender
        }

        [Usage("sdm")]
        [Description("Spawns a Male Dwarf at the targeted location.")]
        private static void SpawnDwarfMale_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Target the location to spawn a Male Dwarf.");
            from.Target = new DwarfSpawnTarget(false); // Male
        }

        [Usage("sdf")]
        [Description("Spawns a Female Dwarf at the targeted location.")]
        private static void SpawnDwarfFemale_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Target the location to spawn a Female Dwarf.");
            from.Target = new DwarfSpawnTarget(true); // Female
        }

        private class DwarfSpawnTarget : Server.Targeting.Target
        {
            private bool? m_Female;

            public DwarfSpawnTarget(bool? female) : base(-1, true, Server.Targeting.TargetFlags.None)
            {
                m_Female = female;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                {
                    from.SendMessage("Invalid target.");
                    return;
                }

                Point3D loc;
                if (p is Item item)
                {
                    loc = item.GetWorldLocation();
                }
                else
                {
                    loc = new Point3D(p);
                }

                // Get proper ground Z level
                int z = from.Map.GetAverageZ(loc.X, loc.Y);
                Point3D spawnLoc = new Point3D(loc.X, loc.Y, z);

                // Spawn the Dwarf
                Dwarf dwarf;
                if (m_Female.HasValue)
                    dwarf = new Dwarf(m_Female.Value);
                else
                    dwarf = new Dwarf();

                dwarf.MoveToWorld(spawnLoc, from.Map);

                string gender = dwarf.Female ? "Female" : "Male";
                from.SendMessage($"Spawned {gender} Dwarf: {dwarf.Name} at {spawnLoc}");
            }
        }
    }
}
