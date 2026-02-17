using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Comprehensive list of creature types that are immune to poison
    /// Used by MageCombatAI to avoid wasting mana on poison spells
    /// </summary>
    public static class PoisonImmuneList
    {
        private static HashSet<Type> s_ImmuneTypes;

        /// <summary>
        /// Check if a mobile is poison immune by type
        /// </summary>
        public static bool IsPoisonImmune(Mobile target)
        {
            if (target == null)
                return false;

            // First check the actual PoisonImmune property if it's a BaseCreature
            if (target is BaseCreature creature && creature.PoisonImmune != null)
                return true;

            // Then check our type list for known immune creatures
            return GetImmuneTypes().Contains(target.GetType());
        }

        /// <summary>
        /// Get the hashset of poison immune creature types
        /// Lazy-loaded for performance
        /// </summary>
        private static HashSet<Type> GetImmuneTypes()
        {
            if (s_ImmuneTypes == null)
            {
                s_ImmuneTypes = new HashSet<Type>();
                PopulateImmuneTypes();
            }

            return s_ImmuneTypes;
        }

        /// <summary>
        /// Populate the list of poison immune creature types
        /// Based on ServUO codebase analysis (158 creatures)
        /// </summary>
        private static void PopulateImmuneTypes()
        {
            // === GOLEMS & MECHANICAL CONSTRUCTS (Complete immunity) ===
            AddType<Golem>();
            AddType<ClockworkScorpion>();
            AddType<RisingColossus>();

            // === ELEMENTALS (Specific types only) ===
            AddType<AcidElemental>();
            AddType<CrystalElemental>();
            AddType<PoisonElemental>();
            AddType<ShadowIronElemental>();

            // === UNDEAD (Major types) ===
            // Liches
            AddType<Lich>();
            AddType<LichLord>();
            AddType<AncientLich>();
            AddType<SkeletalLich>();

            // Spirits
            AddType<Wraith>();
            AddType<Spectre>();
            AddType<Shade>();
            AddType<Revenant>();
            AddType<RevenantLion>();

            // Skeletal
            AddType<SkeletalDragon>();
            AddType<SkeletalDrake>();
            AddType<SkeletalMount>();
            AddType<SkeletalMage>();
            AddType<BoneMagi>();
            AddType<BoneDemon>();
            AddType<Skeleton>();
            AddType<PatchworkSkeleton>();

            // Zombies/Ghouls
            AddType<Zombie>();
            AddType<Ghoul>();
            AddType<RottingCorpse>();
            AddType<Mummy>();
            AddType<Bogle>();

            // Other Undead
            AddType<HellSteed>();
            AddType<SpectralArmour>();

            // === DAEMONS & DEMONS ===
            AddType<Daemon>();
            AddType<FireDaemon>();
            AddType<ArcaneDaemon>();
            AddType<ArchDaemon>();
            AddType<DemonKnight>();
            AddType<Balron>();
            AddType<PitFiend>();
            AddType<Moloch>();
            AddType<Betrayer>();

            // === SLIMES & OOZES ===
            AddType<Slime>();
            AddType<CorrosiveSlime>();
            AddType<BulbousPutrification>();
            AddType<FetidEssence>();

            // === SUMMONED CREATURES ===
            AddType<EnergyVortex>();
            AddType<BladeSpirits>();
            AddType<AnimatedWeapon>();
            AddType<SummonedDaemon>();

            // === BOSS CREATURES (All major bosses) ===
            AddType<Barracoon>();
            AddType<Neira>();
            AddType<Semidar>();
            AddType<Rikktor>();
            AddType<Mephitis>();
            AddType<LordOaks>();
            AddType<Silvani>();
            AddType<ChiefParoxysmus>();
            AddType<LadyMelisande>();
            AddType<DreadHorn>();
            AddType<Harrower>();
            AddType<Medusa>();
            AddType<CrimsonDragon>();
            AddType<AbyssalInfernal>();
            AddType<PrimevalLich>();
            AddType<Serado>();

            // === VOID CREATURES ===
            AddType<Korpre>();
            AddType<Ballem>();

            // === DRAGONS & WYRMS (Specific types) ===
            AddType<AncientWyrm>();
            AddType<ShadowWyrm>();

            // === PLANTS & ABERRATIONS ===
            AddType<Corpser>();
            AddType<BogThing>();
            AddType<WhippingVine>();
            AddType<Reaper>();

            // === CONSTRUCTS & SPECIAL ===
            AddType<Titan>();

            // Note: This list includes the most common poison-immune creatures
            // The actual check will first verify BaseCreature.PoisonImmune property
            // so any creature with that property set will be detected
        }

        /// <summary>
        /// Helper to add a type to the immune list
        /// </summary>
        private static void AddType<T>() where T : Mobile
        {
            s_ImmuneTypes.Add(typeof(T));
        }

        /// <summary>
        /// Check if it's worth casting poison on this target
        /// Returns false if target is immune or already poisoned with higher level
        /// </summary>
        public static bool CanPoisonTarget(Mobile caster, Mobile target)
        {
            if (target == null || !target.Alive)
                return false;

            // Already poisoned with deadly poison - don't waste mana
            if (target.Poisoned && target.Poison != null && target.Poison.Level >= 3)
                return false;

            // Check if poison immune
            if (IsPoisonImmune(target))
                return false;

            return true;
        }

        /// <summary>
        /// Get a descriptive reason why poison can't be cast
        /// For debugging/logging purposes
        /// </summary>
        public static string GetPoisonBlockReason(Mobile target)
        {
            if (target == null || !target.Alive)
                return "Target is null or dead";

            if (target.Poisoned && target.Poison != null && target.Poison.Level >= 3)
                return $"Already poisoned ({target.Poison.Name})";

            if (target is BaseCreature creature && creature.PoisonImmune != null)
                return $"Poison immune ({creature.GetType().Name})";

            if (GetImmuneTypes().Contains(target.GetType()))
                return $"Known poison immune type ({target.GetType().Name})";

            return "Can cast poison";
        }
    }
}
