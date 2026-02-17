using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Services.LLM
{
    /// <summary>
    /// Comprehensive database mapping NPC class types to personality types
    /// </summary>
    public static class NPCPersonalityDatabase
    {
        /// <summary>
        /// Maps NPC class types to their specific personality types
        /// </summary>
        public static readonly Dictionary<Type, NPCPersonalities.PersonalityType> ClassToPersonalityMap = 
            new Dictionary<Type, NPCPersonalities.PersonalityType>
        {
            // Actors and Performers
            { typeof(Actor), NPCPersonalities.PersonalityType.Actor },
            { typeof(Artist), NPCPersonalities.PersonalityType.Artist },
            { typeof(Bard), NPCPersonalities.PersonalityType.Bard },
            // Note: Impresario is in Server.Engines.Quests.Collector namespace

            // Gypsies
            { typeof(Gypsy), NPCPersonalities.PersonalityType.Gypsy },
            { typeof(GypsyAnimalTrainer), NPCPersonalities.PersonalityType.Gypsy },
            { typeof(GypsyBanker), NPCPersonalities.PersonalityType.Gypsy },
            { typeof(GypsyFortuneTeller), NPCPersonalities.PersonalityType.Gypsy },
            { typeof(GypsyMaiden), NPCPersonalities.PersonalityType.Gypsy },

            // Guards
            { typeof(ArcherGuard), NPCPersonalities.PersonalityType.Guard },
            { typeof(WarriorGuard), NPCPersonalities.PersonalityType.Guard },
            { typeof(ChaosGuard), NPCPersonalities.PersonalityType.Guard },
            { typeof(OrderGuard), NPCPersonalities.PersonalityType.Guard },
            { typeof(BaseGuard), NPCPersonalities.PersonalityType.Guard },
            { typeof(BaseShieldGuard), NPCPersonalities.PersonalityType.Guard },
            // Note: MansionGuard is in Server.Engines.Quests.Haven namespace

            // Healers
            { typeof(Healer), NPCPersonalities.PersonalityType.Healer },
            { typeof(BaseHealer), NPCPersonalities.PersonalityType.Healer },
            { typeof(HealerGuildmaster), NPCPersonalities.PersonalityType.Healer },
            { typeof(WanderingHealer), NPCPersonalities.PersonalityType.Healer },
            { typeof(EvilHealer), NPCPersonalities.PersonalityType.Healer },
            { typeof(EvilWanderingHealer), NPCPersonalities.PersonalityType.Healer },
            { typeof(GargishWanderingHealer), NPCPersonalities.PersonalityType.Healer },
            { typeof(ShrineHealer), NPCPersonalities.PersonalityType.Healer },
            { typeof(PricedHealer), NPCPersonalities.PersonalityType.Healer },
            // Note: Aluniol (not AluniolTheHealer) exists but may be in different namespace

            // Mages
            { typeof(Mage), NPCPersonalities.PersonalityType.Mage },
            { typeof(MageGuildmaster), NPCPersonalities.PersonalityType.Mage },
            { typeof(EscortableMage), NPCPersonalities.PersonalityType.Mage },
            { typeof(HolyMage), NPCPersonalities.PersonalityType.Mage },
            { typeof(Necromancer), NPCPersonalities.PersonalityType.Mage },
            { typeof(Mystic), NPCPersonalities.PersonalityType.Mage },

            // Nobles
            { typeof(Noble), NPCPersonalities.PersonalityType.Noble },
            { typeof(GargishNoble), NPCPersonalities.PersonalityType.Noble },

            // Warriors and Combat
            { typeof(WarriorGuildmaster), NPCPersonalities.PersonalityType.Warrior },
            { typeof(Paladin), NPCPersonalities.PersonalityType.Paladin },
            { typeof(Samurai), NPCPersonalities.PersonalityType.Samurai },
            { typeof(Ninja), NPCPersonalities.PersonalityType.Ninja },
            { typeof(Monk), NPCPersonalities.PersonalityType.Monk },
            { typeof(Ranger), NPCPersonalities.PersonalityType.Ranger },
            { typeof(RangerGuildmaster), NPCPersonalities.PersonalityType.Ranger },
            { typeof(Thief), NPCPersonalities.PersonalityType.Thief },
            { typeof(ThiefGuildmaster), NPCPersonalities.PersonalityType.Thief },

            // Vendors and Merchants
            { typeof(Merchant), NPCPersonalities.PersonalityType.Merchant },
            { typeof(MerchantGuildmaster), NPCPersonalities.PersonalityType.Merchant },
            { typeof(BaseVendor), NPCPersonalities.PersonalityType.Merchant },
            { typeof(Provisioner), NPCPersonalities.PersonalityType.Provisioner },
            { typeof(VarietyDealer), NPCPersonalities.PersonalityType.Merchant },
            { typeof(PlayerVendor), NPCPersonalities.PersonalityType.Merchant },
            { typeof(RentedVendor), NPCPersonalities.PersonalityType.Merchant },
            { typeof(CommissionPlayerVendor), NPCPersonalities.PersonalityType.Merchant },

            // Crafters
            { typeof(Blacksmith), NPCPersonalities.PersonalityType.Blacksmith },
            { typeof(BlacksmithGuildmaster), NPCPersonalities.PersonalityType.Blacksmith },
            { typeof(Tailor), NPCPersonalities.PersonalityType.Tailor },
            { typeof(TailorGuildmaster), NPCPersonalities.PersonalityType.Tailor },
            { typeof(Alchemist), NPCPersonalities.PersonalityType.Alchemist },
            { typeof(Carpenter), NPCPersonalities.PersonalityType.Carpenter },
            { typeof(Tinker), NPCPersonalities.PersonalityType.Tinker },
            { typeof(TinkerGuildmaster), NPCPersonalities.PersonalityType.Tinker },
            { typeof(LeatherWorker), NPCPersonalities.PersonalityType.LeatherWorker },
            { typeof(Bowyer), NPCPersonalities.PersonalityType.Bowyer },
            { typeof(Weaponsmith), NPCPersonalities.PersonalityType.Weaponsmith },
            { typeof(Armorer), NPCPersonalities.PersonalityType.Armorer },
            { typeof(Jeweler), NPCPersonalities.PersonalityType.Jeweler },
            { typeof(StoneCrafter), NPCPersonalities.PersonalityType.Carpenter },
            { typeof(GolemCrafter), NPCPersonalities.PersonalityType.Tinker },
            { typeof(IronWorker), NPCPersonalities.PersonalityType.Blacksmith },
            { typeof(Glassblower), NPCPersonalities.PersonalityType.Artist },

            // Service Providers
            { typeof(Banker), NPCPersonalities.PersonalityType.Banker },
            { typeof(GypsyBanker), NPCPersonalities.PersonalityType.Banker },
            { typeof(InnKeeper), NPCPersonalities.PersonalityType.InnKeeper },
            { typeof(Barkeeper), NPCPersonalities.PersonalityType.Barkeeper },
            { typeof(PlayerBarkeeper), NPCPersonalities.PersonalityType.Barkeeper },
            { typeof(Cook), NPCPersonalities.PersonalityType.Cook },
            { typeof(HairStylist), NPCPersonalities.PersonalityType.HairStylist },
            { typeof(CustomHairstylist), NPCPersonalities.PersonalityType.HairStylist },
            { typeof(AnimalTrainer), NPCPersonalities.PersonalityType.AnimalTrainer },
            { typeof(GypsyAnimalTrainer), NPCPersonalities.PersonalityType.AnimalTrainer },
            { typeof(Veterinarian), NPCPersonalities.PersonalityType.Veterinarian },
            { typeof(Herbalist), NPCPersonalities.PersonalityType.Herbalist },
            { typeof(Scribe), NPCPersonalities.PersonalityType.Scribe },
            { typeof(Shipwright), NPCPersonalities.PersonalityType.Shipwright },
            { typeof(Mapmaker), NPCPersonalities.PersonalityType.Mapmaker },
            { typeof(RealEstateBroker), NPCPersonalities.PersonalityType.RealEstateBroker },

            // Workers
            { typeof(Farmer), NPCPersonalities.PersonalityType.Farmer },
            // Note: FarmerNash is in Server.Engines.Quests.Haven namespace
            { typeof(Fisherman), NPCPersonalities.PersonalityType.Fisherman },
            { typeof(FisherGuildmaster), NPCPersonalities.PersonalityType.Fisherman },
            { typeof(Miner), NPCPersonalities.PersonalityType.Miner },
            { typeof(MinerGuildmaster), NPCPersonalities.PersonalityType.Miner },
            { typeof(Beekeeper), NPCPersonalities.PersonalityType.Farmer },
            { typeof(Rancher), NPCPersonalities.PersonalityType.Farmer },
            // Note: Huntsman is in Server.Engines.Quests.Mondain namespace
            { typeof(Gardener), NPCPersonalities.PersonalityType.Farmer },
            { typeof(Miller), NPCPersonalities.PersonalityType.Farmer },

            // Specialized
            { typeof(TownCrier), NPCPersonalities.PersonalityType.TownCrier },
            { typeof(Vagabond), NPCPersonalities.PersonalityType.Vagabond },
            { typeof(Peasant), NPCPersonalities.PersonalityType.Peasant },
            // Note: Henchman is in Server.Engines.Quests.Ninja namespace
            { typeof(BaseEscortable), NPCPersonalities.PersonalityType.Escortable },
            { typeof(BaseHire), NPCPersonalities.PersonalityType.Henchman },
            { typeof(HireBard), NPCPersonalities.PersonalityType.Bard },
            { typeof(HireBardArcher), NPCPersonalities.PersonalityType.Bard },
            { typeof(HireBeggar), NPCPersonalities.PersonalityType.Peasant },
            { typeof(HireFighter), NPCPersonalities.PersonalityType.Warrior },
            { typeof(HireMage), NPCPersonalities.PersonalityType.Mage },
            { typeof(HirePaladin), NPCPersonalities.PersonalityType.Paladin },
            { typeof(HirePeasant), NPCPersonalities.PersonalityType.Peasant },
            { typeof(HireRanger), NPCPersonalities.PersonalityType.Ranger },
            { typeof(HireRangerArcher), NPCPersonalities.PersonalityType.Ranger },
            { typeof(HireSailor), NPCPersonalities.PersonalityType.Shipwright },
            { typeof(HireThief), NPCPersonalities.PersonalityType.Thief },

            // Guildmasters (inherit from their profession)
            { typeof(BaseGuildmaster), NPCPersonalities.PersonalityType.Merchant },
            { typeof(BardGuildmaster), NPCPersonalities.PersonalityType.Bard },

            // Other Specialized NPCs
            { typeof(Sculptor), NPCPersonalities.PersonalityType.Artist },
            { typeof(Architect), NPCPersonalities.PersonalityType.Carpenter },
            { typeof(HarborMaster), NPCPersonalities.PersonalityType.Shipwright },
            { typeof(Messenger), NPCPersonalities.PersonalityType.TownCrier },
            { typeof(SeekerOfAdventure), NPCPersonalities.PersonalityType.Vagabond },
            { typeof(Waiter), NPCPersonalities.PersonalityType.Barkeeper },
            { typeof(Butcher), NPCPersonalities.PersonalityType.Cook },
            { typeof(Cobbler), NPCPersonalities.PersonalityType.LeatherWorker },
            { typeof(Tanner), NPCPersonalities.PersonalityType.LeatherWorker },
            { typeof(Weaver), NPCPersonalities.PersonalityType.Tailor },
            { typeof(Furtrader), NPCPersonalities.PersonalityType.Merchant },
            // Note: Naturalist is in Server.Engines.Quests.Naturalist namespace
        };
    }
}

