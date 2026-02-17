using System;
using System.Collections.Generic;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Library of fantasy-inspired names for sidekicks, organized by archetype
    /// Names are inspired by classic fantasy literature styles
    /// </summary>
    public static class SidekickNameLibrary
    {
        private static readonly Random m_Random = new Random();

        #region Warrior Names
        // Strong, bold names befitting warriors and fighters
        private static readonly string[] WarriorMaleNames = new string[]
        {
            // Nordic/Barbarian style
            "Thorin", "Bjorn", "Ragnar", "Ulfric", "Gunnar", "Kael", "Brom", "Druss",
            "Wulfgar", "Bruenor", "Cadderly", "Zaknafein", "Dak", "Grom", "Thokk",
            // Knight/Noble style
            "Roland", "Gareth", "Baldwin", "Cedric", "Edmund", "Aldric", "Beric",
            "Corwin", "Darion", "Eamon", "Fendrel", "Gorin", "Hadrian", "Kellan",
            // Eastern/Exotic style
            "Kazuo", "Takeshi", "Jin", "Feng", "Zhao", "Kenji", "Ryu", "Shen"
        };

        private static readonly string[] WarriorFemaleNames = new string[]
        {
            // Shield maiden style
            "Brienne", "Freya", "Sigrid", "Astrid", "Helga", "Ingrid", "Thyra", "Brunhilde",
            "Eowyn", "Catti", "Shallya", "Valeria", "Kira", "Lyanna", "Ygritte",
            // Knight/Noble style
            "Isolde", "Morgana", "Elspeth", "Rowena", "Gwendolyn", "Alanna", "Brianna",
            "Cordelia", "Deirdre", "Erin", "Fiora", "Gwyneth", "Helena", "Katarina"
        };
        #endregion

        #region Mage Names
        // Mystical, arcane names befitting spellcasters
        private static readonly string[] MageMaleNames = new string[]
        {
            // Classic wizard style
            "Elminster", "Mordenkainen", "Raistlin", "Pug", "Macros", "Kulgan", "Milamber",
            "Tomas", "Nakor", "Sidi", "Magnus", "Alaric", "Theron", "Eldrin",
            // Elvish/Ethereal style
            "Celeborn", "Galadrion", "Finrod", "Maglor", "Cirdan", "Thranduil", "Elrohir",
            "Lindir", "Erestor", "Glorfindel", "Haldir", "Rumil", "Orophin", "Beleg",
            // Dark/Mysterious style
            "Malakai", "Vortigern", "Saruman", "Denethor", "Curunir", "Pallando", "Radagast"
        };

        private static readonly string[] MageFemaleNames = new string[]
        {
            // Enchantress style
            "Arwen", "Galadriel", "Luthien", "Idril", "Nimrodel", "Melian", "Finduilas",
            "Aredhel", "Earwen", "Celebrian", "Morwen", "Nienor", "Elwing", "Miriel",
            // Sorceress style
            "Medivha", "Syndra", "Morgause", "Vivienne", "Yennefer", "Triss", "Keira",
            "Philippa", "Margarita", "Sheala", "Sabrina", "Fringilla", "Assire", "Lydia"
        };
        #endregion

        #region Archer Names
        // Swift, precise names befitting rangers and marksmen
        private static readonly string[] ArcherMaleNames = new string[]
        {
            // Woodland/Elvish style
            "Legolas", "Beleg", "Haldir", "Rumil", "Orophin", "Mablung", "Damrod",
            "Anborn", "Faramir", "Ithilien", "Tanis", "Gilthanas", "Porthios", "Kagonesti",
            // Hunter style
            "Hawkeye", "Fletcher", "Archer", "Bowman", "Hunter", "Tracker", "Swift",
            "Keen", "Sharp", "True", "Falcon", "Raven", "Crow", "Sparrow"
        };

        private static readonly string[] ArcherFemaleNames = new string[]
        {
            // Huntress style
            "Tauriel", "Sylvanas", "Alleria", "Vereesa", "Maiev", "Tyrande", "Shandris",
            "Naisha", "Mirana", "Windrunner", "Moonsong", "Starfall", "Nightwhisper",
            // Woodland style
            "Laurelin", "Elanor", "Niphredil", "Lissuin", "Alfirin", "Simbelmyne",
            "Athelas", "Mallos", "Lebethron", "Culumalda", "Oiolaire", "Yavannamire"
        };
        #endregion

        #region Healer Names
        // Gentle, holy names befitting healers and priests
        private static readonly string[] HealerMaleNames = new string[]
        {
            // Priest/Cleric style
            "Aelfric", "Benedict", "Cuthbert", "Dominic", "Erasmus", "Francis", "Gregory",
            "Hieronymus", "Ignatius", "Jerome", "Kentigern", "Lanfranc", "Methodius",
            // Gentle/Wise style
            "Elrond", "Beorn", "Bombur", "Bofur", "Bifur", "Dori", "Nori", "Ori",
            "Balin", "Dwalin", "Oin", "Gloin", "Fili", "Kili", "Gimli"
        };

        private static readonly string[] HealerFemaleNames = new string[]
        {
            // Priestess style
            "Aerith", "Yuna", "Rosa", "Garnet", "Eiko", "Ovelia", "Alma", "Agrias",
            "Celes", "Terra", "Relm", "Faris", "Lenna", "Krile", "Refia",
            // Healer/Saint style
            "Seraphina", "Celestia", "Lumina", "Aurora", "Solara", "Lunara", "Stellara",
            "Astraea", "Vespera", "Novella", "Silvana", "Aurellia", "Meridia", "Azura"
        };
        #endregion

        #region Paladin Names
        // Noble, holy knight names
        private static readonly string[] PaladinMaleNames = new string[]
        {
            // Knight style
            "Galahad", "Percival", "Gawain", "Lancelot", "Bors", "Kay", "Bedivere",
            "Geraint", "Lamorak", "Tristan", "Palamedes", "Safir", "Segwarides",
            // Holy warrior style
            "Uther", "Tirion", "Turalyon", "Lothar", "Alexandros", "Mograine", "Fordring",
            "Bolvar", "Saidan", "Gavinrad", "Dathrohan", "Abbendis", "Isillien", "Brigitte"
        };

        private static readonly string[] PaladinFemaleNames = new string[]
        {
            // Lady knight style
            "Jehanne", "Adelaide", "Matilda", "Eleanor", "Isabella", "Constance", "Beatrice",
            "Margaret", "Catherine", "Blanche", "Philippa", "Joan", "Jacqueline", "Yolande",
            // Holy warrior style
            "Tyrande", "Jaina", "Sylvanas", "Vereesa", "Alleria", "Maiev", "Naisha",
            "Shandris", "Cordana", "Ysera", "Alexstrasza", "Chromie", "Aegwynn", "Garona"
        };
        #endregion

        #region Ranger Names
        // Wild, nature-connected names
        private static readonly string[] RangerMaleNames = new string[]
        {
            // Wanderer style
            "Strider", "Estel", "Thorongil", "Telcontar", "Elessar", "Envinyatar", "Dunadan",
            "Longshanks", "Wingfoot", "Elfstone", "Isildur", "Arathorn", "Arador", "Argonui",
            // Wilderness style
            "Drizzt", "Entreri", "Jarlaxle", "Regis", "Cattibrie", "Wulfgar", "Bruenor",
            "Pwent", "Athrogate", "Dahlia", "Effron", "Barrabus", "Herzgo", "Alegni"
        };

        private static readonly string[] RangerFemaleNames = new string[]
        {
            // Wilderness style
            "Gilraen", "Ivorwen", "Firiel", "Rian", "Andreth", "Haleth", "Morwen",
            "Aerin", "Nellas", "Finduilas", "Nienna", "Nessa", "Vana", "Vaire",
            // Huntress style
            "Khelben", "Laeral", "Alustriel", "Qilue", "Dove", "Storm", "Syluné",
            "Elué", "Mystra", "Selûne", "Sune", "Tymora", "Mielikki", "Silvanus"
        };
        #endregion

        #region Thief Names
        // Shadowy, cunning names
        private static readonly string[] ThiefMaleNames = new string[]
        {
            // Rogue style
            "Locke", "Setzer", "Shadow", "Gogo", "Sabin", "Edgar", "Cyan",
            "Strago", "Relm", "Mog", "Umaro", "Gau", "Realm", "Interceptor",
            // Assassin style
            "Artemis", "Jarlaxle", "Entreri", "Regis", "Morik", "Wei", "Kimmuriel",
            "Rai-Guy", "Berg'inyon", "Dinin", "Masoj", "Alton", "Gelroos", "Sorn"
        };

        private static readonly string[] ThiefFemaleNames = new string[]
        {
            // Shadow style
            "Shale", "Cutter", "Silk", "Whisper", "Ghost", "Shade", "Nightshade",
            "Venom", "Viper", "Asp", "Cobra", "Mamba", "Adder", "Taipan",
            // Cunning style
            "Dahlia", "Liriel", "Qilue", "Caladnei", "Jhessail", "Illistyl", "Shaerl",
            "Shandril", "Symrustar", "Tanalasta", "Tessaril", "Zaranda", "Mirt", "Durnan"
        };
        #endregion

        #region Necromancer Names
        // Dark, ominous names
        private static readonly string[] NecromancerMaleNames = new string[]
        {
            // Dark lord style
            "Sauron", "Melkor", "Morgoth", "Gothmog", "Ancalagon", "Glaurung", "Scatha",
            "Smaug", "Balrog", "Ungoliant", "Shelob", "Carcharoth", "Draugluin", "Thuringwethil",
            // Lich style
            "Vecna", "Acererak", "Szass", "Tam", "Larloch", "Sammaster", "Manshoon",
            "Fzoul", "Xanathar", "Halaster", "Ioulaum", "Karsus", "Lathander", "Moander"
        };

        private static readonly string[] NecromancerFemaleNames = new string[]
        {
            // Dark sorceress style
            "Morticia", "Bellatrix", "Narcissa", "Maleficent", "Ursula", "Grimhilde", "Ravenna",
            "Lamia", "Circe", "Medusa", "Echidna", "Hecate", "Nyx", "Ereshkigal",
            // Shadow queen style
            "Liliana", "Vraska", "Elesh", "Sheoldred", "Vorinclex", "Jin-Gitaxias", "Urabrask",
            "Atraxa", "Marchesa", "Yawgmoth", "Volrath", "Crovax", "Greven", "Takara"
        };
        #endregion

        #region Battlemage Names
        // Combination warrior-mage names
        private static readonly string[] BattlemageMaleNames = new string[]
        {
            // Spellsword style
            "Tomas", "Pug", "Macros", "Nakor", "Sidi", "Magnus", "Miranda",
            "Kulgan", "Meecham", "Hochopepa", "Shimone", "Elgahar", "Watoom", "Milambride",
            // War mage style
            "Khadgar", "Medivh", "Antonidas", "Rhonin", "Krasus", "Kalecgos", "Malygos",
            "Nozdormu", "Ysera", "Alexstrasza", "Deathwing", "Galakrond", "Wrathion", "Ebonhorn"
        };

        private static readonly string[] BattlemageFemaleNames = new string[]
        {
            // Spellsword style
            "Miranda", "Gamina", "Borric", "Carline", "Anita", "Arutha", "Lyam",
            "Martin", "Amos", "Jimmy", "Locklear", "Gorath", "Owyn", "Patrus",
            // War mage style  
            "Jaina", "Aegwynn", "Magna", "Modera", "Vereesa", "Sylvanas", "Alleria",
            "Tyrande", "Maiev", "Malfurion", "Illidan", "Cenarius", "Elune", "Aviana"
        };
        #endregion

        #region Cleric Names
        // Religious, sacred names
        private static readonly string[] ClericMaleNames = new string[]
        {
            // Priest style
            "Anduin", "Velen", "Akama", "Nobundo", "Rehgar", "Thrall", "Drek'Thar",
            "Ner'zhul", "Gul'dan", "Durotan", "Orgrim", "Blackhand", "Grommash", "Garrosh",
            // Holy man style
            "Aurelius", "Bartholomew", "Cornelius", "Damian", "Ezekiel", "Ferdinand", "Gideon",
            "Horatio", "Isaiah", "Josiah", "Lazarus", "Matthias", "Nathaniel", "Obadiah"
        };

        private static readonly string[] ClericFemaleNames = new string[]
        {
            // Priestess style
            "Moira", "Aggra", "Draka", "Garona", "Geyah", "Greatmother", "Leyara",
            "Magatha", "Sinestra", "Sintharia", "Onyxia", "Nefarian", "Chromatus", "Galakrond",
            // Holy woman style
            "Anastasia", "Bernadette", "Celestina", "Dorothea", "Evangeline", "Felicity", "Genevieve",
            "Hildegard", "Imogen", "Jacinta", "Lucretia", "Magdalena", "Natalia", "Ophelia"
        };
        #endregion

        #region Druid Names
        // Nature-based, Celtic-inspired names
        private static readonly string[] DruidMaleNames = new string[]
        {
            // Nature style
            "Malfurion", "Cenarius", "Keeper", "Remulos", "Zaetar", "Celebras", "Ordanus",
            "Morthis", "Talran", "Loren", "Celestine", "Hamuul", "Runetotem", "Archdruid",
            // Celtic style
            "Brennan", "Cormac", "Declan", "Egan", "Fintan", "Galen", "Kieran",
            "Lorcan", "Murphy", "Niall", "Oscar", "Patrick", "Quinn", "Ronan"
        };

        private static readonly string[] DruidFemaleNames = new string[]
        {
            // Nature style
            "Lunara", "Brightwing", "Ysera", "Alexstrasza", "Aviana", "Elune", "Goldrinn",
            "Malorne", "Omen", "Tortolla", "Aessina", "Agamaggan", "Ashamane", "Ursoc",
            // Celtic style
            "Aisling", "Brigid", "Ciara", "Deirdre", "Eithne", "Fiona", "Grainne",
            "Keeva", "Maeve", "Niamh", "Oonagh", "Roisin", "Siobhan", "Tara"
        };
        #endregion

        #region Tamer Names
        // Beast-master, wilderness names
        private static readonly string[] TamerMaleNames = new string[]
        {
            // Beastmaster style
            "Rexxar", "Rokhan", "Chen", "Stormstout", "Vol'jin", "Zul'jin", "Rastakhan",
            "Bwonsamdi", "Hakkar", "Jin'do", "Zanzil", "Krag'wa", "Gonk", "Pa'ku",
            // Wilderness style
            "Beorn", "Radagast", "Tom", "Bombadil", "Goldberry", "Iarwain", "Ben-Adar",
            "Forn", "Orald", "Elder", "Wandlimb", "Leaflock", "Skinbark", "Finglas"
        };

        private static readonly string[] TamerFemaleNames = new string[]
        {
            // Beastmaster style
            "Rexxara", "Rokha", "Li-Li", "Talanji", "Zekhan", "Aponi", "Brightmane",
            "Dezco", "Sunwalker", "Perith", "Amulet", "Tarah", "Kelsey", "Steelspark",
            // Wilderness style
            "Goldberry", "Luthien", "Tinuviel", "Arwen", "Undomiel", "Evenstar", "Elbereth",
            "Gilthoniel", "Varda", "Elentari", "Tintalle", "Fanuilos", "Sindarin", "Quenya"
        };
        #endregion

        /// <summary>
        /// Get a random name for the given archetype and gender.
        /// Uses the syllable-based generator for unique first + last names.
        /// </summary>
        public static string GetRandomName(SidekickArchetypeType archetype, bool isMale)
        {
            // Use the new syllable-based generator for full names
            string name = SidekickNameGenerator.GenerateFullName(archetype, isMale);
            Console.WriteLine($"[SidekickNameLibrary] Generated name: {name} for {archetype} (male={isMale})");
            return name;
        }

        /// <summary>
        /// Get a random name from the legacy static lists (single names only)
        /// </summary>
        public static string GetRandomLegacyName(SidekickArchetypeType archetype, bool isMale)
        {
            string[] names = GetNamesForArchetype(archetype, isMale);

            if (names == null || names.Length == 0)
                return isMale ? "Companion" : "Companion";

            return names[m_Random.Next(names.Length)];
        }

        /// <summary>
        /// Get the name array for a given archetype and gender
        /// </summary>
        private static string[] GetNamesForArchetype(SidekickArchetypeType archetype, bool isMale)
        {
            switch (archetype)
            {
                case SidekickArchetypeType.Warrior:
                    return isMale ? WarriorMaleNames : WarriorFemaleNames;
                case SidekickArchetypeType.Mage:
                    return isMale ? MageMaleNames : MageFemaleNames;
                case SidekickArchetypeType.Archer:
                    return isMale ? ArcherMaleNames : ArcherFemaleNames;
                case SidekickArchetypeType.Healer:
                    return isMale ? HealerMaleNames : HealerFemaleNames;
                case SidekickArchetypeType.Paladin:
                    return isMale ? PaladinMaleNames : PaladinFemaleNames;
                case SidekickArchetypeType.Ranger:
                    return isMale ? RangerMaleNames : RangerFemaleNames;
                case SidekickArchetypeType.Thief:
                    return isMale ? ThiefMaleNames : ThiefFemaleNames;
                case SidekickArchetypeType.Necromancer:
                    return isMale ? NecromancerMaleNames : NecromancerFemaleNames;
                case SidekickArchetypeType.Battlemage:
                    return isMale ? BattlemageMaleNames : BattlemageFemaleNames;
                case SidekickArchetypeType.Cleric:
                    return isMale ? ClericMaleNames : ClericFemaleNames;
                case SidekickArchetypeType.Druid:
                    return isMale ? DruidMaleNames : DruidFemaleNames;
                case SidekickArchetypeType.Tamer:
                    return isMale ? TamerMaleNames : TamerFemaleNames;
                default:
                    return isMale ? WarriorMaleNames : WarriorFemaleNames;
            }
        }

        /// <summary>
        /// Get all names for a given archetype (both genders)
        /// </summary>
        public static List<string> GetAllNamesForArchetype(SidekickArchetypeType archetype)
        {
            var names = new List<string>();
            names.AddRange(GetNamesForArchetype(archetype, true));
            names.AddRange(GetNamesForArchetype(archetype, false));
            return names;
        }
    }
}

