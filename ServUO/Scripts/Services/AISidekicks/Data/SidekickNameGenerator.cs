using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Syllable-based fantasy name generator for sidekicks.
    /// Generates first and last names appropriate for each archetype.
    /// </summary>
    public static class SidekickNameGenerator
    {
        private static readonly Random m_Random = new Random();

        #region Warrior Name Parts
        private static readonly string[] WarriorPrefixes = { "Thor", "Rag", "Brun", "Grim", "Iron", "Stone", "War", "Blood", "Storm", "Blade", "Steel", "Doom", "Battle", "Wulf", "Bear", "Axe" };
        private static readonly string[] WarriorMiddles = { "nar", "rik", "dar", "gor", "mar", "val", "dor", "gar", "bor", "kar", "mund", "hard", "wald", "helm", "brand" };
        private static readonly string[] WarriorSuffixes = { "son", "axe", "hammer", "shield", "blade", "fist", "heart", "born", "slayer", "breaker", "bane", "guard", "forge", "arm", "helm" };
        private static readonly string[] WarriorLastPrefixes = { "Iron", "Stone", "Storm", "Thunder", "Black", "Red", "Blood", "War", "Battle", "Steel", "Grim", "Dark", "Doom", "Frost", "Flame" };
        private static readonly string[] WarriorLastSuffixes = { "fist", "hammer", "axe", "shield", "helm", "bane", "heart", "born", "forge", "guard", "breaker", "slayer", "blade", "arm", "maw" };
        #endregion

        #region Mage Name Parts
        private static readonly string[] MagePrefixes = { "Mal", "Thal", "Eld", "Mord", "Arch", "Zan", "Vel", "Kael", "Aur", "Syl", "Myr", "Vor", "Xan", "Cel", "Lum", "Nyx" };
        private static readonly string[] MageMiddles = { "an", "or", "in", "us", "ar", "en", "il", "os", "ir", "al", "oth", "wyn", "dor", "rin", "thos" };
        private static readonly string[] MageSuffixes = { "dris", "ius", "eon", "nar", "wyn", "thor", "vyn", "ion", "ael", "ros", "max", "zar", "dan", "rak", "ven" };
        private static readonly string[] MageLastPrefixes = { "Star", "Moon", "Sun", "Spell", "Rune", "Mist", "Shadow", "Frost", "Flame", "Storm", "Void", "Aether", "Arcane", "Crystal", "Ether" };
        private static readonly string[] MageLastSuffixes = { "weaver", "binder", "singer", "walker", "seeker", "keeper", "caller", "shaper", "wielder", "master", "born", "touched", "blessed", "sworn", "bound" };
        #endregion

        #region Archer Name Parts
        private static readonly string[] ArcherPrefixes = { "Swift", "Keen", "True", "Fleet", "Sharp", "Wind", "Hawk", "Fal", "Rav", "Syl", "Glen", "Wood", "Leaf", "Elm", "Oak", "Ash" };
        private static readonly string[] ArcherMiddles = { "an", "en", "ar", "or", "in", "el", "al", "ir", "ow", "ey", "yn", "ael", "iel", "wen" };
        private static readonly string[] ArcherSuffixes = { "shot", "eye", "arrow", "bow", "mark", "wind", "flight", "wing", "feather", "strike", "aim", "sight", "hunter", "stalker", "tracker" };
        private static readonly string[] ArcherLastPrefixes = { "Swift", "True", "Keen", "Sharp", "Far", "Long", "Sure", "Quick", "Fleet", "Wind", "Storm", "Shadow", "Silent", "Dead", "Hawk" };
        private static readonly string[] ArcherLastSuffixes = { "arrow", "shot", "eye", "bow", "aim", "mark", "flight", "strike", "hunter", "stalker", "wind", "feather", "wing", "hand", "sight" };
        #endregion

        #region Healer Name Parts
        private static readonly string[] HealerPrefixes = { "Ser", "Lum", "Cel", "Aur", "Sol", "Bel", "Grace", "Mercy", "Hope", "Faith", "Light", "Dawn", "Bright", "Clear", "Pure", "Sana" };
        private static readonly string[] HealerMiddles = { "a", "e", "i", "o", "ia", "ea", "ana", "ina", "ara", "ela", "iel", "ael", "wen", "lyn" };
        private static readonly string[] HealerSuffixes = { "na", "ra", "la", "lia", "nia", "ria", "dia", "via", "mia", "tia", "sia", "cia", "zia", "fia", "gia" };
        private static readonly string[] HealerLastPrefixes = { "Light", "Bright", "Sun", "Dawn", "Hope", "Grace", "Mercy", "Faith", "Gentle", "Kind", "Warm", "Soft", "Pure", "Holy", "Sacred" };
        private static readonly string[] HealerLastSuffixes = { "touch", "hand", "heart", "soul", "spirit", "blessing", "grace", "hope", "light", "giver", "bringer", "keeper", "tender", "healer", "mender" };
        #endregion

        #region Paladin Name Parts
        private static readonly string[] PaladinPrefixes = { "Gal", "Per", "Lan", "Tris", "Uth", "Bald", "Ced", "Gar", "Val", "Lor", "Jus", "Right", "True", "Noble", "Just", "Vir" };
        private static readonly string[] PaladinMiddles = { "a", "e", "i", "o", "an", "en", "ar", "or", "al", "el", "ric", "win", "mund", "ard", "old" };
        private static readonly string[] PaladinSuffixes = { "had", "val", "lot", "tan", "ric", "win", "mund", "ard", "old", "red", "ald", "ius", "eon", "ion", "ian" };
        private static readonly string[] PaladinLastPrefixes = { "Light", "Dawn", "Sun", "Holy", "Sacred", "Noble", "True", "Just", "Right", "Valor", "Honor", "Glory", "Divine", "Bright", "Golden" };
        private static readonly string[] PaladinLastSuffixes = { "shield", "blade", "sword", "guard", "helm", "heart", "soul", "sworn", "bound", "blessed", "born", "keeper", "defender", "champion", "knight" };
        #endregion

        #region Ranger Name Parts
        private static readonly string[] RangerPrefixes = { "Strid", "Path", "Trail", "Wood", "Wild", "Fern", "Moss", "Briar", "Thorn", "Root", "Bark", "Leaf", "Glen", "Dale", "Moor", "Heath" };
        private static readonly string[] RangerMiddles = { "er", "an", "en", "or", "ar", "in", "ow", "ey", "el", "al", "wyn", "iel", "ael", "wen" };
        private static readonly string[] RangerSuffixes = { "walker", "finder", "seeker", "tracker", "runner", "stalker", "wander", "strider", "hunter", "ranger", "scout", "guide", "path", "trail", "way" };
        private static readonly string[] RangerLastPrefixes = { "Wild", "Wood", "Forest", "Trail", "Path", "Wind", "Storm", "Shadow", "Silent", "Swift", "Far", "Long", "Deep", "Dark", "Green" };
        private static readonly string[] RangerLastSuffixes = { "walker", "stalker", "tracker", "runner", "hunter", "finder", "seeker", "watcher", "keeper", "guide", "scout", "strider", "wanderer", "ranger", "path" };
        #endregion

        #region Thief Name Parts
        private static readonly string[] ThiefPrefixes = { "Shade", "Shadow", "Silk", "Sly", "Quick", "Nim", "Dusk", "Twi", "Gloom", "Murk", "Haze", "Fog", "Mist", "Veil", "Mask", "Cloak" };
        private static readonly string[] ThiefMiddles = { "a", "e", "i", "o", "er", "ar", "in", "an", "ow", "ey", "yx", "ix", "ax", "ex" };
        private static readonly string[] ThiefSuffixes = { "finger", "hand", "step", "blade", "strike", "touch", "whisper", "shadow", "shade", "cloak", "mask", "veil", "thief", "sneak", "ghost" };
        private static readonly string[] ThiefLastPrefixes = { "Shadow", "Silent", "Swift", "Quick", "Sly", "Dark", "Night", "Dusk", "Black", "Grey", "Smoke", "Mist", "Ghost", "Phantom", "Shade" };
        private static readonly string[] ThiefLastSuffixes = { "finger", "hand", "blade", "step", "cloak", "mask", "shadow", "whisper", "strike", "touch", "thief", "sneak", "prowler", "stalker", "walker" };
        #endregion

        #region Necromancer Name Parts
        private static readonly string[] NecromancerPrefixes = { "Mor", "Nec", "Dread", "Grave", "Bone", "Soul", "Death", "Doom", "Blight", "Wither", "Decay", "Rot", "Corpse", "Ghast", "Lich", "Vex" };
        private static readonly string[] NecromancerMiddles = { "o", "a", "i", "u", "ar", "or", "ur", "oth", "ath", "eth", "ith", "uth", "rak", "zar", "gul" };
        private static readonly string[] NecromancerSuffixes = { "ius", "ax", "oth", "ul", "an", "en", "on", "us", "is", "os", "zar", "rak", "gul", "mor", "vex" };
        private static readonly string[] NecromancerLastPrefixes = { "Death", "Grave", "Bone", "Soul", "Doom", "Dread", "Blight", "Wither", "Decay", "Corpse", "Ghost", "Shade", "Dark", "Black", "Void" };
        private static readonly string[] NecromancerLastSuffixes = { "binder", "caller", "speaker", "walker", "reaper", "lord", "master", "bane", "touch", "grip", "whisper", "shroud", "veil", "curse", "doom" };
        #endregion

        #region Battlemage Name Parts
        private static readonly string[] BattlemagePrefixes = { "Spell", "War", "Battle", "Storm", "Thunder", "Flame", "Frost", "Arcane", "Myst", "Rune", "Glyph", "Sigil", "Hex", "Jinx", "Charm", "Ward" };
        private static readonly string[] BattlemageMiddles = { "or", "ar", "an", "en", "in", "on", "ir", "ur", "ric", "ald", "ius", "eon", "ion", "wyn", "dor" };
        private static readonly string[] BattlemageSuffixes = { "blade", "sword", "axe", "mace", "hammer", "spell", "cast", "strike", "storm", "bolt", "fire", "frost", "shock", "force", "power" };
        private static readonly string[] BattlemageLastPrefixes = { "Spell", "Storm", "Thunder", "Flame", "Frost", "War", "Battle", "Arcane", "Rune", "Steel", "Iron", "Blade", "Sword", "Fire", "Ice" };
        private static readonly string[] BattlemageLastSuffixes = { "blade", "sword", "caster", "striker", "weaver", "binder", "caller", "forger", "wielder", "master", "born", "touched", "sworn", "bound", "breaker" };
        #endregion

        #region Cleric Name Parts
        private static readonly string[] ClericPrefixes = { "Bene", "Sanct", "Pius", "Devot", "Faith", "Pious", "Holy", "Sacred", "Divine", "Blessed", "Aur", "Lum", "Sol", "Cel", "Ser", "Vir" };
        private static readonly string[] ClericMiddles = { "e", "i", "a", "o", "us", "is", "os", "as", "ius", "eus", "ael", "iel", "wen", "lyn" };
        private static readonly string[] ClericSuffixes = { "dict", "tus", "nus", "rus", "lus", "mus", "dus", "gus", "cus", "bus", "ius", "eus", "ael", "iel", "ion" };
        private static readonly string[] ClericLastPrefixes = { "Holy", "Sacred", "Divine", "Blessed", "Light", "Sun", "Dawn", "Faith", "Hope", "Grace", "Mercy", "Truth", "Virtue", "Spirit", "Soul" };
        private static readonly string[] ClericLastSuffixes = { "keeper", "bearer", "bringer", "speaker", "walker", "servant", "voice", "hand", "heart", "soul", "spirit", "blessing", "prayer", "faith", "light" };
        #endregion

        #region Druid Name Parts
        private static readonly string[] DruidPrefixes = { "Oak", "Ash", "Elm", "Birch", "Willow", "Thorn", "Fern", "Moss", "Root", "Bark", "Leaf", "Branch", "Grove", "Glen", "Meadow", "Bloom" };
        private static readonly string[] DruidMiddles = { "en", "an", "in", "on", "ar", "or", "er", "wyn", "ael", "iel", "wen", "lyn", "dale", "vale", "wood" };
        private static readonly string[] DruidSuffixes = { "heart", "soul", "spirit", "song", "voice", "whisper", "call", "keeper", "warden", "guardian", "tender", "walker", "speaker", "friend", "kin" };
        private static readonly string[] DruidLastPrefixes = { "Oak", "Moss", "Fern", "Thorn", "Leaf", "Root", "Bark", "Grove", "Forest", "Wood", "Wild", "Green", "Deep", "Ancient", "Elder" };
        private static readonly string[] DruidLastSuffixes = { "heart", "soul", "spirit", "keeper", "warden", "guardian", "tender", "walker", "speaker", "friend", "whisper", "song", "call", "bloom", "root" };
        #endregion

        #region Tamer Name Parts
        private static readonly string[] TamerPrefixes = { "Beast", "Fang", "Claw", "Wild", "Tame", "Pack", "Herd", "Flock", "Den", "Lair", "Hunt", "Track", "Stalk", "Prowl", "Roam", "Range" };
        private static readonly string[] TamerMiddles = { "er", "ar", "or", "an", "en", "in", "on", "ow", "ey", "al", "el", "wyn", "ael", "ien" };
        private static readonly string[] TamerSuffixes = { "friend", "kin", "bond", "call", "speak", "touch", "heart", "soul", "spirit", "master", "keeper", "warden", "lord", "walker", "runner" };
        private static readonly string[] TamerLastPrefixes = { "Beast", "Wild", "Fang", "Claw", "Pack", "Hunt", "Track", "Stalk", "Prowl", "Roam", "Free", "Swift", "Fierce", "Loyal", "True" };
        private static readonly string[] TamerLastSuffixes = { "friend", "bond", "heart", "soul", "keeper", "master", "caller", "speaker", "walker", "runner", "hunter", "tracker", "warden", "lord", "kin" };
        #endregion

        /// <summary>
        /// Generate a full name (first + last) for the given archetype
        /// </summary>
        public static string GenerateFullName(SidekickArchetypeType archetype, bool isMale)
        {
            string firstName = GenerateFirstName(archetype, isMale);
            string lastName = GenerateLastName(archetype);
            return $"{firstName} {lastName}";
        }

        /// <summary>
        /// Generate a first name for the given archetype
        /// </summary>
        public static string GenerateFirstName(SidekickArchetypeType archetype, bool isMale)
        {
            var parts = GetNameParts(archetype);

            string prefix = parts.Prefixes[m_Random.Next(parts.Prefixes.Length)];
            string middle = "";
            string suffix = "";

            // 60% chance of middle syllable
            if (m_Random.NextDouble() > 0.4)
            {
                middle = parts.Middles[m_Random.Next(parts.Middles.Length)];
            }

            // 40% chance of suffix (makes shorter names more common)
            if (m_Random.NextDouble() > 0.6)
            {
                suffix = parts.Suffixes[m_Random.Next(parts.Suffixes.Length)];
            }

            string name = prefix + middle + suffix;

            // Apply gender modifications for some archetypes
            if (!isMale)
            {
                name = FeminizeName(name, archetype);
            }

            return CapitalizeName(name);
        }

        /// <summary>
        /// Generate a last name (surname) for the given archetype
        /// </summary>
        public static string GenerateLastName(SidekickArchetypeType archetype)
        {
            var parts = GetNameParts(archetype);

            string prefix = parts.LastPrefixes[m_Random.Next(parts.LastPrefixes.Length)];
            string suffix = parts.LastSuffixes[m_Random.Next(parts.LastSuffixes.Length)];

            return CapitalizeName(prefix + suffix);
        }

        /// <summary>
        /// Apply feminine endings to names
        /// </summary>
        private static string FeminizeName(string name, SidekickArchetypeType archetype)
        {
            // Some archetypes have more feminine name patterns
            switch (archetype)
            {
                case SidekickArchetypeType.Healer:
                case SidekickArchetypeType.Druid:
                    // Already feminine-sounding, maybe add 'a' ending
                    if (!name.EndsWith("a") && !name.EndsWith("ia") && !name.EndsWith("lia") && m_Random.NextDouble() > 0.5)
                    {
                        if (name.EndsWith("n") || name.EndsWith("r") || name.EndsWith("l"))
                            return name + "a";
                        if (name.EndsWith("s"))
                            return name.Substring(0, name.Length - 1) + "ia";
                    }
                    break;

                case SidekickArchetypeType.Mage:
                case SidekickArchetypeType.Necromancer:
                    // Change -us/-os endings to -a/-ia
                    if (name.EndsWith("us"))
                        return name.Substring(0, name.Length - 2) + "ia";
                    if (name.EndsWith("os"))
                        return name.Substring(0, name.Length - 2) + "a";
                    if (name.EndsWith("or"))
                        return name.Substring(0, name.Length - 2) + "ora";
                    break;

                case SidekickArchetypeType.Warrior:
                case SidekickArchetypeType.Paladin:
                    // Add feminine endings
                    if (name.EndsWith("n") && m_Random.NextDouble() > 0.5)
                        return name + "a";
                    if (name.EndsWith("r") && m_Random.NextDouble() > 0.5)
                        return name + "ia";
                    break;

                default:
                    // General feminization
                    if (m_Random.NextDouble() > 0.6)
                    {
                        if (name.EndsWith("n") || name.EndsWith("r") || name.EndsWith("l") || name.EndsWith("d"))
                            return name + "a";
                    }
                    break;
            }

            return name;
        }

        /// <summary>
        /// Properly capitalize a name
        /// </summary>
        private static string CapitalizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            // Handle compound names (e.g., "Ironhammer" -> "Ironhammer")
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        /// <summary>
        /// Get the name parts for a given archetype
        /// </summary>
        private static NameParts GetNameParts(SidekickArchetypeType archetype)
        {
            switch (archetype)
            {
                case SidekickArchetypeType.Warrior:
                    return new NameParts(WarriorPrefixes, WarriorMiddles, WarriorSuffixes, WarriorLastPrefixes, WarriorLastSuffixes);
                case SidekickArchetypeType.Mage:
                    return new NameParts(MagePrefixes, MageMiddles, MageSuffixes, MageLastPrefixes, MageLastSuffixes);
                case SidekickArchetypeType.Archer:
                    return new NameParts(ArcherPrefixes, ArcherMiddles, ArcherSuffixes, ArcherLastPrefixes, ArcherLastSuffixes);
                case SidekickArchetypeType.Healer:
                    return new NameParts(HealerPrefixes, HealerMiddles, HealerSuffixes, HealerLastPrefixes, HealerLastSuffixes);
                case SidekickArchetypeType.Paladin:
                    return new NameParts(PaladinPrefixes, PaladinMiddles, PaladinSuffixes, PaladinLastPrefixes, PaladinLastSuffixes);
                case SidekickArchetypeType.Ranger:
                    return new NameParts(RangerPrefixes, RangerMiddles, RangerSuffixes, RangerLastPrefixes, RangerLastSuffixes);
                case SidekickArchetypeType.Thief:
                    return new NameParts(ThiefPrefixes, ThiefMiddles, ThiefSuffixes, ThiefLastPrefixes, ThiefLastSuffixes);
                case SidekickArchetypeType.Necromancer:
                    return new NameParts(NecromancerPrefixes, NecromancerMiddles, NecromancerSuffixes, NecromancerLastPrefixes, NecromancerLastSuffixes);
                case SidekickArchetypeType.Battlemage:
                    return new NameParts(BattlemagePrefixes, BattlemageMiddles, BattlemageSuffixes, BattlemageLastPrefixes, BattlemageLastSuffixes);
                case SidekickArchetypeType.Cleric:
                    return new NameParts(ClericPrefixes, ClericMiddles, ClericSuffixes, ClericLastPrefixes, ClericLastSuffixes);
                case SidekickArchetypeType.Druid:
                    return new NameParts(DruidPrefixes, DruidMiddles, DruidSuffixes, DruidLastPrefixes, DruidLastSuffixes);
                case SidekickArchetypeType.Tamer:
                    return new NameParts(TamerPrefixes, TamerMiddles, TamerSuffixes, TamerLastPrefixes, TamerLastSuffixes);
                default:
                    return new NameParts(WarriorPrefixes, WarriorMiddles, WarriorSuffixes, WarriorLastPrefixes, WarriorLastSuffixes);
            }
        }

        /// <summary>
        /// Container for name generation parts
        /// </summary>
        private struct NameParts
        {
            public string[] Prefixes;
            public string[] Middles;
            public string[] Suffixes;
            public string[] LastPrefixes;
            public string[] LastSuffixes;

            public NameParts(string[] prefixes, string[] middles, string[] suffixes, string[] lastPrefixes, string[] lastSuffixes)
            {
                Prefixes = prefixes;
                Middles = middles;
                Suffixes = suffixes;
                LastPrefixes = lastPrefixes;
                LastSuffixes = lastSuffixes;
            }
        }

        #region Test/Debug Methods

        /// <summary>
        /// Generate sample names for testing
        /// </summary>
        public static void PrintSampleNames(int count = 5)
        {
            Console.WriteLine("=== Sidekick Name Generator Samples ===\n");

            foreach (SidekickArchetypeType archetype in Enum.GetValues(typeof(SidekickArchetypeType)))
            {
                Console.WriteLine($"--- {archetype} ---");
                Console.WriteLine("Male:");
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine($"  {GenerateFullName(archetype, true)}");
                }
                Console.WriteLine("Female:");
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine($"  {GenerateFullName(archetype, false)}");
                }
                Console.WriteLine();
            }
        }

        #endregion
    }
}
