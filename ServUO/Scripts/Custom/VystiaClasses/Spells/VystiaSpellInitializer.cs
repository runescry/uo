using System;
using Server.Spells.VystiaSpells.IceMage;
using Server.Spells.VystiaSpells.Nature;
using Server.Spells.VystiaSpells.Hex;
using Server.Spells.VystiaSpells.Elemental;
using Server.Spells.VystiaSpells.Dark;
using Server.Spells.VystiaSpells.Divination;
using Server.Spells.VystiaSpells.Necromancy;
using Server.Spells.VystiaSpells.Summoning;
using Server.Spells.VystiaSpells.Shamanic;
#if VYSTIA_SONGWEAVING
using Server.Spells.VystiaSpells.Songweaving;
#endif
using Server.Spells.VystiaSpells.Enchanting;
using Server.Spells.VystiaSpells.Illusion;

namespace Server.Spells
{
    /// <summary>
    /// Initializes all Vystia custom spells and registers them with the spell registry
    /// Auto-generated to fix spell ID conflicts
    /// </summary>
    public class VystiaSpellInitializer
    {
        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized)
            {
                Console.WriteLine("[VYSTIA] WARNING: Initialize() called again, skipping duplicate registration!");
                return;
            }

            _initialized = true;
            Console.WriteLine("[VYSTIA] Initializing Vystia spells...");

            RegisterIceMageSpells();
            RegisterDruidSpells();
            RegisterWitchSpells();
            RegisterSorcererSpells();
            RegisterWarlockSpells();
            RegisterOracleSpells();
            RegisterNecromancerSpells();
            RegisterSummonerSpells();
            RegisterShamanSpells();
#if VYSTIA_SONGWEAVING
            RegisterSongweavingSpells();
#endif
            RegisterEnchanterSpells();
            RegisterIllusionistSpells();

            Console.WriteLine("[VYSTIA] Spell registration complete. Total: 367 spells");

            // Initialize Vystia systems
            Server.Custom.VystiaClasses.Religion.VystiaDevotionPowers.Initialize();
            Server.Custom.VystiaClasses.Factions.VystiaFactionSystemInit.Initialize();
            Server.Custom.VystiaClasses.Crafting.VystiaCraftingRecipes.Initialize();
        }

        private static void RegisterIceMageSpells()
        {
            // Ice Magic Spells (IDs 1000-1031)
            Register(1000, typeof(FrostTouchSpell)); // Fixed: Changed from 999 to 1000 to match client
            Register(1001, typeof(IceShardSpell));
            Register(1002, typeof(FrostWardSpell));
            Register(1003, typeof(AvalancheSpell)); // Moved from 1023 (Circle 6) to Circle 1 position
            Register(1004, typeof(FreezingGraspSpell));
            Register(1005, typeof(IceShieldSpell));
            Register(1006, typeof(FrostSlickSpell));
            Register(1007, typeof(GlacialMendSpell));
            Register(1008, typeof(IceBoltSpell));
            Register(1009, typeof(FrostbiteSpell));
            Register(1010, typeof(FrozenGroundSpell));
            Register(1011, typeof(IceSpearSpell)); // Circle 3, spell 4
            Register(1012, typeof(FrostArmorSpell));
            Register(1013, typeof(IceWallSpell)); // Circle 4, spell 2
            Register(1014, typeof(IcicleBarrageSpell)); // Circle 4, spell 3
            Register(1015, typeof(PermafrostSpell)); // Circle 4, spell 4
            Register(1016, typeof(GlacialStrikeSpell)); // Circle 5, spell 1
            Register(1017, typeof(FrozenTombSpell)); // Circle 5, spell 2
            Register(1018, typeof(ShatterSpell)); // Circle 5, spell 3
            Register(1019, typeof(HypothermiaSpell)); // Circle 5, spell 4
            Register(1020, typeof(BlizzardSpell)); // Circle 6, spell 1
            Register(1021, typeof(GlacialFortressSpell)); // Circle 6, spell 2
            Register(1022, typeof(DeepFreezeSpell)); // Circle 6, spell 3
            Register(1023, typeof(ChillAuraSpell)); // Moved from 1003 (Circle 1) to Circle 6 position
            Register(1024, typeof(AbsoluteZeroSpell)); // Circle 7, spell 1
            Register(1025, typeof(GlacierSummonSpell)); // Circle 7, spell 2
            Register(1026, typeof(EternalWinterSpell)); // Circle 7, spell 3
            Register(1027, typeof(FimbulwintersWrathSpell)); // Circle 7, spell 4
            Register(1028, typeof(FrostMeteorSpell)); // Circle 8, spell 1
            Register(1029, typeof(IceAgeSpell)); // Circle 8, spell 2
            Register(1030, typeof(RimeReaperSpell)); // Circle 8, spell 3
            Register(1031, typeof(CocytusPrisonSpell)); // Circle 8, spell 4
        }

        private static void RegisterDruidSpells()
        {
            // Nature Magic Spells (IDs 1032-1063) - Fixed: Starts at 1032 (Ice Magic ends at 1031)
            Register(1032, typeof(NatureNaturesTouchSpell));
            Register(1033, typeof(NatureThornDartSpell));
            Register(1034, typeof(NatureBarkskinSpell));
            Register(1035, typeof(NatureDetectLifeSpell));
            Register(1036, typeof(NatureEntangleSpell));
            Register(1037, typeof(NaturePoisonSporesSpell));
            Register(1038, typeof(NatureRejuvenationSpell));
            Register(1039, typeof(NatureAnimalAspectSpeedSpell));
            Register(1040, typeof(NatureWildGrowthSpell));
            Register(1041, typeof(NatureBearFormLesserShapeshiftSpell));
            Register(1042, typeof(NatureThornVolleySpell));
            Register(1043, typeof(NatureNaturesBlessingSpell));
            Register(1044, typeof(NatureWolfFormAdvancedShapeshiftSpell));
            Register(1045, typeof(NatureStranglingVinesSpell));
            Register(1046, typeof(NatureHealingGroveSpell));
            Register(1047, typeof(NatureToxicBloomSpell));
            Register(1048, typeof(NatureHawkFormAerialShapeshiftSpell));
            Register(1049, typeof(NatureEarthquakeSpell));
            Register(1050, typeof(NatureGreaterRegenerationSpell));
            Register(1051, typeof(NatureSporeCloudSpell));
            Register(1052, typeof(NatureTreantFormGreaterShapeshiftSpell));
            Register(1053, typeof(NatureSwarmSpell));
            Register(1054, typeof(NatureLivingFortressSpell));
            Register(1055, typeof(NatureNaturesWrathSpell));
            Register(1056, typeof(NatureForceofNatureSpell));
            Register(1057, typeof(NatureSummonAncientTreantSpell));
            Register(1058, typeof(NaturePlagueSpell));
            Register(1059, typeof(NaturePrimordialRestorationSpell));
            Register(1060, typeof(NatureWorldTreesEmbraceSpell));
            Register(1061, typeof(NatureHydraFormLegendaryShapeshiftSpell));
            Register(1062, typeof(NatureThornApocalypseSpell));
            Register(1063, typeof(NatureAvataroftheForestSpell));
        }

        private static void RegisterWitchSpells()
        {
            // Hex Magic Spells (IDs 1064-1095) - Fixed: Starts at 1064 (Nature Magic ends at 1063)
            Register(1064, typeof(HexEvilEyeSpell));
            Register(1065, typeof(HexWeakCurseSpell));
            Register(1066, typeof(HexSiphonLifeSpell));
            Register(1067, typeof(HexWitchSightSpell));
            Register(1068, typeof(HexWastingCurseSpell));
            Register(1069, typeof(HexPoisonTouchSpell));
            Register(1070, typeof(HexEnfeebleSpell));
            Register(1071, typeof(HexDarkPactSpell));
            Register(1072, typeof(HexContagiousHexSpell));
            Register(1073, typeof(HexLifeLeechSpell));
            Register(1074, typeof(HexHexofFrailtySpell));
            Register(1075, typeof(HexVoodooDollSpell));
            Register(1076, typeof(HexCripplingCurseSpell));
            Register(1077, typeof(HexPlagueBearerSpell));
            Register(1078, typeof(HexDrainEssenceSpell));
            Register(1079, typeof(HexHexofAgonySpell));
            Register(1080, typeof(HexMassHexSpell));
            Register(1081, typeof(HexSoulSiphonSpell));
            Register(1082, typeof(HexHexofSilenceSpell));
            Register(1083, typeof(HexNecroticTouchSpell));
            Register(1084, typeof(HexCurseofMortalitySpell));
            Register(1085, typeof(HexHexStormSpell));
            Register(1086, typeof(HexVampiricAuraSpell));
            Register(1087, typeof(HexDoomcurseSpell));
            Register(1088, typeof(HexPlagueofSorrowsSpell));
            Register(1089, typeof(HexSoulHarvestSpell));
            Register(1090, typeof(HexCurseoftheHagSpell));
            Register(1091, typeof(HexHexbladeRitualSpell));
            Register(1092, typeof(HexCurseofUndeathSpell));
            Register(1093, typeof(HexVoodooMasterySpell));
            Register(1094, typeof(HexApocalypticHexSpell));
            Register(1095, typeof(HexWitchQueensDominionSpell));
        }

        private static void RegisterSorcererSpells()
        {
            // Elemental Magic Spells (IDs 1096-1127) - Fixed: Starts at 1096 (Hex Magic ends at 1095)
            Register(1096, typeof(ElementalFlameBoltSpell));
            Register(1097, typeof(ElementalMoltenTouchSpell));
            Register(1098, typeof(ElementalHeatShieldSpell));
            Register(1099, typeof(ElementalSmokeScreenSpell));
            Register(1100, typeof(ElementalFireballSpell));
            Register(1101, typeof(ElementalLavaPuddleSpell));
            Register(1102, typeof(ElementalEmberBurstSpell));
            Register(1103, typeof(ElementalFlameWardSpell));
            Register(1104, typeof(ElementalIncinerateSpell));
            Register(1105, typeof(ElementalVolcanicRockSpell));
            Register(1106, typeof(ElementalRingofFireSpell));
            Register(1107, typeof(ElementalIgniteSpell));
            Register(1108, typeof(ElementalFlamePillarSpell));
            Register(1109, typeof(ElementalMagmaArmorSpell));
            Register(1110, typeof(ElementalPyroclasmSpell));
            Register(1111, typeof(ElementalMeteorStrikeSpell));
            Register(1112, typeof(ElementalInfernoSpell));
            Register(1113, typeof(ElementalLavaFlowSpell));
            Register(1114, typeof(ElementalCombustionSpell));
            Register(1115, typeof(ElementalPhoenixShieldSpell));
            Register(1116, typeof(ElementalHellfireNovaSpell));
            Register(1117, typeof(ElementalMoltenTitanFormSpell));
            Register(1118, typeof(ElementalVolcanicEruptionSpell));
            Register(1119, typeof(ElementalFlameTempestSpell));
            Register(1120, typeof(ElementalCataclysmSpell));
            Register(1121, typeof(ElementalSummonFireElementalLordSpell));
            Register(1122, typeof(ElementalPyroclasticFlowSpell));
            Register(1123, typeof(ElementalAvatarofFlameSpell));
            Register(1124, typeof(ElementalApocalypseSpell));
            Register(1125, typeof(ElementalMagmaCoreSpell));
            Register(1126, typeof(ElementalSolarFlareSpell));
            Register(1127, typeof(ElementalPrimordialInfernoSpell));
        }

        private static void RegisterWarlockSpells()
        {
            // Dark Magic Spells (IDs 1128-1159) - Fixed: Starts at 1128 (Elemental Magic ends at 1127)
            Register(1128, typeof(DarkShadowBoltSpell));
            Register(1129, typeof(DarkLifeTapSpell));
            Register(1130, typeof(DarkMinorFearSpell));
            Register(1131, typeof(DarkDemonicSightSpell));
            Register(1132, typeof(DarkChaosBoltSpell));
            Register(1133, typeof(DarkShadowStepSpell));
            Register(1134, typeof(DarkDrainSoulSpell));
            Register(1135, typeof(DarkLesserDemonSpell));
            Register(1136, typeof(DarkShadowNovaSpell));
            Register(1137, typeof(DarkDemonicPactSpell));
            Register(1138, typeof(DarkFearWaveSpell));
            Register(1139, typeof(DarkCorruptionSpell));
            Register(1140, typeof(DarkSummonVoidwalkerSpell));
            Register(1141, typeof(DarkShadowChainsSpell));
            Register(1142, typeof(DarkSoulBurnSpell));
            Register(1143, typeof(DarkChaosRiftSpell));
            Register(1144, typeof(DarkFelArmorSpell));
            Register(1145, typeof(DarkMassFearSpell));
            Register(1146, typeof(DarkDemonicSacrificeSpell));
            Register(1147, typeof(DarkShadowOrbSpell));
            Register(1148, typeof(DarkSummonSuccubusSpell));
            Register(1149, typeof(DarkDarkPortalSpell));
            Register(1150, typeof(DarkChaosStormSpell));
            Register(1151, typeof(DarkShadowformSpell));
            Register(1152, typeof(DarkSummonPitLordSpell));
            Register(1153, typeof(DarkSoulHarvestSpell));
            Register(1154, typeof(DarkApocalypticChaosSpell));
            Register(1155, typeof(DarkDemonicAscensionSpell));
            Register(1156, typeof(DarkSummonDemonPrinceSpell));
            Register(1157, typeof(DarkVoidCollapseSpell));
            Register(1158, typeof(DarkChaosIncarnateSpell));
            Register(1159, typeof(DarkDarkApotheosisSpell));
        }

        private static void RegisterOracleSpells()
        {
            // Divination Magic Spells (IDs 1160-1191) - Fixed: Starts at 1160 (Dark Magic ends at 1159)
            Register(1160, typeof(DivinationCrystalDartSpell));
            Register(1161, typeof(DivinationGlimpseFutureSpell));
            Register(1162, typeof(DivinationMinorWardSpell));
            Register(1163, typeof(DivinationClaritySpell));
            Register(1164, typeof(DivinationPrismaticBoltSpell));
            Register(1165, typeof(DivinationForesightSpell));
            Register(1166, typeof(DivinationCrystalShieldSpell));
            Register(1167, typeof(DivinationHasteSelfSpell));
            Register(1168, typeof(DivinationEnergyBurstSpell));
            Register(1169, typeof(DivinationPrecognitionSpell));
            Register(1170, typeof(DivinationTemporalSlowSpell));
            Register(1171, typeof(DivinationManaCrystalSpell));
            Register(1172, typeof(DivinationPrismaticSpraySpell));
            Register(1173, typeof(DivinationTimeWarpSpell));
            Register(1174, typeof(DivinationBarrierofLightSpell));
            Register(1175, typeof(DivinationOraclesSightSpell));
            Register(1176, typeof(DivinationCrystalBarrageSpell));
            Register(1177, typeof(DivinationFatesThreadSpell));
            Register(1178, typeof(DivinationMassHasteSpell));
            Register(1179, typeof(DivinationTemporalShieldSpell));
            Register(1180, typeof(DivinationPrismaticStormSpell));
            Register(1181, typeof(DivinationProphecyofDoomSpell));
            Register(1182, typeof(DivinationCrystalFortressSpell));
            Register(1183, typeof(DivinationTimeStopSpell));
            Register(1184, typeof(DivinationCosmicRiftSpell));
            Register(1185, typeof(DivinationFateShiftSpell));
            Register(1186, typeof(DivinationMassForesightSpell));
            Register(1187, typeof(DivinationChronoLordSpell));
            Register(1188, typeof(DivinationPrismaticApocalypseSpell));
            Register(1189, typeof(DivinationCrystalMazeSpell));
            Register(1190, typeof(DivinationTimelessStateSpell));
            Register(1191, typeof(DivinationOracleAscendantSpell));
        }

        private static void RegisterNecromancerSpells()
        {
            // Necromancy Magic Spells (IDs 1192-1223) - Fixed: Starts at 1192 (Divination ends at 1191), DemiLich moved to correct position
            Register(1192, typeof(NecromancyDeathBoltSpell));
            Register(1193, typeof(NecromancyAnimateBoneSpell));
            Register(1194, typeof(NecromancyLifeSiphonSpell));
            Register(1195, typeof(NecromancyDeathsightSpell));
            Register(1196, typeof(NecromancyBoneShardSpell));
            Register(1197, typeof(NecromancyDecaySpell));
            Register(1198, typeof(NecromancyRaiseZombieSpell));
            Register(1199, typeof(NecromancySoulShieldSpell));
            Register(1200, typeof(NecromancyDeathCoilSpell));
            Register(1201, typeof(NecromancyBoneArmorSpell));
            Register(1202, typeof(NecromancyMassRaiseSpell));
            Register(1203, typeof(NecromancySoulHarvestSpell));
            Register(1204, typeof(NecromancySkeletalMageSpell));
            Register(1205, typeof(NecromancyCorpseExplosionSpell));
            Register(1206, typeof(NecromancyDeathGripSpell));
            Register(1207, typeof(NecromancyVampiricTouchSpell));
            Register(1208, typeof(NecromancyBoneWallSpell));
            Register(1209, typeof(NecromancyDeathandDecaySpell));
            Register(1210, typeof(NecromancyRaiseBoneGolemSpell));
            Register(1211, typeof(NecromancySoulLinkSpell));
            Register(1212, typeof(NecromancyDeathKnightsSpell));
            Register(1213, typeof(NecromancyPlagueCloudSpell));
            Register(1214, typeof(NecromancyUnholyFrenzySpell));
            Register(1215, typeof(NecromancyLichFormSpell));
            Register(1216, typeof(NecromancyArmyoftheDeadSpell));
            Register(1217, typeof(NecromancyDeathWaveSpell));
            Register(1218, typeof(NecromancyBonePrisonSpell));
            Register(1219, typeof(NecromancyDemiLichTransformationSpell)); // Fixed: Moved from 1218 to correct position
            Register(1220, typeof(NecromancyApocalypseofDeathSpell));
            Register(1221, typeof(NecromancySummonUndeadDragonSpell));
            Register(1222, typeof(NecromancyDeathsDoorSpell));
            Register(1223, typeof(NecromancyArchlichAscensionSpell));
        }

        private static void RegisterSummonerSpells()
        {
            // Summoning Magic Spells (IDs 1224-1255) - Fixed: Starts at 1224 (Necromancy ends at 1223)
            Register(1224, typeof(SummoningSummonRabbitSpell));
            Register(1225, typeof(SummoningArcaneBoltSpell));
            Register(1226, typeof(SummoningEmpowerSummonSpell));
            Register(1227, typeof(SummoningSummonWispSpell));
            Register(1228, typeof(SummoningSummonWolfSpell));
            Register(1229, typeof(SummoningSummonFireSpriteSpell));
            Register(1230, typeof(SummoningMendSummonSpell));
            Register(1231, typeof(SummoningSummonShieldSpell));
            Register(1232, typeof(SummoningSummonBearSpell));
            Register(1233, typeof(SummoningSummonAirElementalSpell));
            Register(1234, typeof(SummoningMassEmpowerSpell));
            Register(1235, typeof(SummoningBindBeastSpell));
            Register(1236, typeof(SummoningSummonDrakeSpell));
            Register(1237, typeof(SummoningSummonEarthElementalSpell));
            Register(1238, typeof(SummoningSummonFrenzySpell));
            Register(1239, typeof(SummoningUnsummonSpell));
            Register(1240, typeof(SummoningSummonHydraSpell));
            Register(1241, typeof(SummoningSummonStormElementalSpell));
            Register(1242, typeof(SummoningGreaterHealSummonSpell));
            Register(1243, typeof(SummoningSymbioticLinkSpell));
            Register(1244, typeof(SummoningSummonPhoenixSpell));
            Register(1245, typeof(SummoningSummonVoidElementalSpell));
            Register(1246, typeof(SummoningArmyofBeastsSpell));
            Register(1247, typeof(SummoningMassHealSummonsSpell));
            Register(1248, typeof(SummoningSummonGreaterDragonSpell));
            Register(1249, typeof(SummoningSummonElementalLordSpell));
            Register(1250, typeof(SummoningSacrificeSummonSpell));
            Register(1251, typeof(SummoningSwarmofCreaturesSpell));
            Register(1252, typeof(SummoningSummonTitanSpell));
            Register(1253, typeof(SummoningPlanarConvergenceSpell));
            Register(1254, typeof(SummoningSummonersApocalypseSpell));
            Register(1255, typeof(SummoningAvatarofSummoningSpell));
        }

        private static void RegisterShamanSpells()
        {
            // Shamanic Magic Spells (IDs 1256-1287) - Fixed: Starts at 1256 (Summoning ends at 1255)
            Register(1256, typeof(ShamanicLightningBoltSpell));
            Register(1257, typeof(ShamanicStrengthTotemSpell));
            Register(1258, typeof(ShamanicGhostWolfFormSpell));
            Register(1259, typeof(ShamanicHealingStreamSpell));
            Register(1260, typeof(ShamanicChainLightningSpell));
            Register(1261, typeof(ShamanicFireTotemSpell));
            Register(1262, typeof(ShamanicSpiritStrikeSpell));
            Register(1263, typeof(ShamanicPurificationSpell));
            Register(1264, typeof(ShamanicLightningStormSpell));
            Register(1265, typeof(ShamanicEarthShieldSpell));
            Register(1266, typeof(ShamanicTotemicRecallSpell));
            Register(1267, typeof(ShamanicSummonSpiritWolfSpell));
            Register(1268, typeof(ShamanicChainHealSpell));
            Register(1269, typeof(ShamanicManaSpringTotemSpell));
            Register(1270, typeof(ShamanicLavaBurstSpell));
            Register(1271, typeof(ShamanicFlameShockSpell));
            Register(1272, typeof(ShamanicThunderstormTotemSpell));
            Register(1273, typeof(ShamanicAncestralSpiritSpell));
            Register(1274, typeof(ShamanicEarthElementalSpell));
            Register(1275, typeof(ShamanicMaelstromSpell));
            Register(1276, typeof(ShamanicMegaChainLightningSpell));
            Register(1277, typeof(ShamanicTotemofWrathSpell));
            Register(1278, typeof(ShamanicSpiritLinkTotemSpell));
            Register(1279, typeof(ShamanicElementalFurySpell));
            Register(1280, typeof(ShamanicSummonGreaterEarthElementalSpell));
            Register(1281, typeof(ShamanicAncestorsBlessingSpell));
            Register(1282, typeof(ShamanicFourTotemsSpell));
            Register(1283, typeof(ShamanicAscendanceSpell));
            Register(1284, typeof(ShamanicApocalypticChainLightningSpell));
            Register(1285, typeof(ShamanicSpiritoftheWildSpell));
            Register(1286, typeof(ShamanicTotemArmySpell));
            Register(1287, typeof(ShamanicShamanLordSpell));
        }

#if VYSTIA_SONGWEAVING
        private static void RegisterSongweavingSpells()
        {
            // Songweaving Spells (IDs 1384-1398)
            Register(1384, typeof(SongweavingDiscordanceSpell));          // Discordant Note
            Register(1385, typeof(SongweavingCourageSpell));              // Song of Courage
            Register(1386, typeof(SongweavingPeacemakingSpell));          // Lullaby
            Register(1387, typeof(SongweavingFortuneSpell));              // Inspire Accuracy
            Register(1388, typeof(SongweavingSharpNoteSpell));            // Sharp Note
            Register(1389, typeof(SongweavingMendingSpell));              // Song of Healing
            Register(1390, typeof(SongweavingRequiemSpell));              // Dirge of Weakness
            Register(1391, typeof(SongweavingLightSpell));                // Song of Illumination
            Register(1392, typeof(SongweavingFortissimoSpell));           // Fortissimo
            Register(1393, typeof(SongweavingProvocationSpell));          // Song of Provocation
            Register(1394, typeof(SongweavingInterludeSpell));            // Mesmerise
            Register(1395, typeof(SongweavingSwiftnessSpell));            // Song of Swiftness
            Register(1396, typeof(SongweavingSymphonyOfDestructionSpell)); // Symphony of Destruction
            Register(1397, typeof(SongweavingSoothingChorusSpell));       // Soothing Chorus
            Register(1398, typeof(SongweavingRallyingAnthemSpell));        // Cacophony
        }
#endif

        private static void RegisterEnchanterSpells()
        {
            // Enchanting Magic Spells (IDs 1320-1351) - Fixed: Starts at 1320 (Bardic ends at 1319)
            Register(1320, typeof(EnchantingMagicWeaponSpell));
            Register(1321, typeof(EnchantingArcaneShieldSpell));
            Register(1322, typeof(EnchantingRuneofPowerSpell));
            Register(1323, typeof(EnchantingDetectMagicSpell));
            Register(1324, typeof(EnchantingFlamingWeaponSpell));
            Register(1325, typeof(EnchantingFortifyArmorSpell));
            Register(1326, typeof(EnchantingDisenchantSpell));
            Register(1327, typeof(EnchantingRuneofProtectionSpell));
            Register(1328, typeof(EnchantingLightningWeaponSpell));
            Register(1329, typeof(EnchantingElementalBarrierSpell));
            Register(1330, typeof(EnchantingSharpenSpell));
            Register(1331, typeof(EnchantingRuneofHealingSpell));
            Register(1332, typeof(EnchantingVampiricWeaponSpell));
            Register(1333, typeof(EnchantingSpellReflectionSpell));
            Register(1334, typeof(EnchantingEnchantArrowsSpell));
            Register(1335, typeof(EnchantingMassDisenchantSpell));
            Register(1336, typeof(EnchantingHolyWeaponSpell));
            Register(1337, typeof(EnchantingAegisofWardingSpell));
            Register(1338, typeof(EnchantingRunicEmpowermentSpell));
            Register(1339, typeof(EnchantingEnchantPartyWeaponsSpell));
            Register(1340, typeof(EnchantingLegendaryWeaponSpell));
            Register(1341, typeof(EnchantingInvulnerabilitySpell));
            Register(1342, typeof(EnchantingMassEnchantWeaponsSpell));
            Register(1343, typeof(EnchantingRuneofResurrectionSpell));
            Register(1344, typeof(EnchantingGodlyWeaponSpell));
            Register(1345, typeof(EnchantingPrismaticBarrierSpell));
            Register(1346, typeof(EnchantingEnchantArmySpell));
            Register(1347, typeof(EnchantingGreaterDisenchantSpell));
            Register(1348, typeof(EnchantingArtifactEmpowermentSpell));
            Register(1349, typeof(EnchantingInvincibleArmorSpell));
            Register(1350, typeof(EnchantingRuneofApocalypseSpell));
            Register(1351, typeof(EnchantingArchmagesBlessingSpell));
        }

        private static void RegisterIllusionistSpells()
        {
            // Illusion Magic Spells (IDs 1352-1383) - Fixed: Starts at 1352 (Enchanting ends at 1351)
            Register(1352, typeof(IllusionMindSpikeSpell));
            Register(1353, typeof(IllusionBlurSpell));
            Register(1354, typeof(IllusionMinorIllusionSpell));
            Register(1355, typeof(IllusionDetectThoughtsSpell));
            Register(1356, typeof(IllusionPhantomBoltSpell));
            Register(1357, typeof(IllusionInvisibilitySpell));
            Register(1358, typeof(IllusionIllusoryDoubleSpell));
            Register(1359, typeof(IllusionConfuseSpell));
            Register(1360, typeof(IllusionPsychicScreamSpell));
            Register(1361, typeof(IllusionGreaterInvisibilitySpell));
            Register(1362, typeof(IllusionMirrorImageSpell));
            Register(1363, typeof(IllusionCharmBeastSpell));
            Register(1364, typeof(IllusionMindBlastSpell));
            Register(1365, typeof(IllusionIllusoryTerrainSpell));
            Register(1366, typeof(IllusionPhantasmalKillerSpell));
            Register(1367, typeof(IllusionMassConfusionSpell));
            Register(1368, typeof(IllusionMindControlSpell));
            Register(1369, typeof(IllusionPerfectInvisibilitySpell));
            Register(1370, typeof(IllusionIllusoryArmySpell));
            Register(1371, typeof(IllusionPsychicStormSpell));
            Register(1372, typeof(IllusionDominateMindSpell));
            Register(1373, typeof(IllusionPhaseShiftSpell));
            Register(1374, typeof(IllusionLegionofMirrorsSpell));
            Register(1375, typeof(IllusionMassCharmSpell));
            Register(1376, typeof(IllusionMindShatterSpell));
            Register(1377, typeof(IllusionTrueInvisibilitySpell));
            Register(1378, typeof(IllusionPhantasmalDragonSpell));
            Register(1379, typeof(IllusionRealityWarpSpell));
            Register(1380, typeof(IllusionApocalypticNightmareSpell));
            Register(1381, typeof(IllusionMasterofPuppetsSpell));
            Register(1382, typeof(IllusionIllusoryApocalypseSpell));
            Register(1383, typeof(IllusionPerfectIllusionSpell));
        }

        private static void Register(int spellID, Type type)
        {
            SpellRegistry.Register(spellID, type);
        }
    }
}
