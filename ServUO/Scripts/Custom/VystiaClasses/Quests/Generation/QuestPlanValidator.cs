using System;
using System.Collections.Generic;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    public sealed class QuestPlanValidationResult
    {
        public bool Success => Errors.Count == 0;
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
    }

    public interface IQuestPlanValidator
    {
        QuestPlanValidationResult Validate(DynamicQuestPlan plan);
    }

    /// <summary>
    /// Hard validation for LLM output plans (anti-hallucination + safety).
    /// Keep this strict and expand it over time.
    /// </summary>
    public sealed class QuestPlanValidator : IQuestPlanValidator
    {
        private static readonly HashSet<string> AllowedConditions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(WaypointCondition.TalkToNPC),
            nameof(WaypointCondition.ReachLocation),
            nameof(WaypointCondition.DefeatBoss),
            nameof(WaypointCondition.CollectItems),
            nameof(WaypointCondition.RecruitSidekick),
            nameof(WaypointCondition.Custom),
        };

        private static readonly HashSet<string> AllowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(WaypointType.Origin),
            nameof(WaypointType.Waypoint),
            nameof(WaypointType.BossCompletion),
            nameof(WaypointType.NPCCompletion),
        };

        // Expandable allowlist: all bosses from NPC template registry
        private static readonly HashSet<string> AllowedBossTypeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "FrostFather",
            "ForgeMaster",
            "CovenMatriarch",
            "VolcanoWyrm",
            "TimewornLich",
            "AncientTreant",
            "CrystalDrakeAlpha",
            "SphinxOfSurya",
            "AncientKraken",
            "GriffinLord"
        };

        public QuestPlanValidationResult Validate(DynamicQuestPlan plan)
        {
            var r = new QuestPlanValidationResult();

            if (plan == null)
            {
                r.Errors.Add("Plan is null.");
                return r;
            }

            if (string.IsNullOrWhiteSpace(plan.Title))
                r.Errors.Add("Missing quest title.");

            if (plan.Waypoints == null || plan.Waypoints.Count < 2)
                r.Errors.Add("Plan must include at least Origin + Completion (and typically >= 1 objective waypoint).");

            if (plan.ExpiresMinutes < 10 || plan.ExpiresMinutes > 24 * 60)
                r.Errors.Add("expiresMinutes out of bounds (10..1440).");

            bool hasOrigin = false;
            bool hasCompletion = false;

            for (int i = 0; i < (plan.Waypoints?.Count ?? 0); i++)
            {
                var wp = plan.Waypoints[i];
                if (wp == null)
                {
                    r.Errors.Add($"Waypoint[{i}] is null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(wp.Type) || !AllowedTypes.Contains(wp.Type))
                    r.Errors.Add($"Waypoint[{i}] has invalid type '{wp.Type}'.");

                if (string.IsNullOrWhiteSpace(wp.Condition) || !AllowedConditions.Contains(wp.Condition))
                    r.Errors.Add($"Waypoint[{i}] has invalid condition '{wp.Condition}'.");

                if (wp.Type != null && wp.Type.Equals(nameof(WaypointType.Origin), StringComparison.OrdinalIgnoreCase))
                    hasOrigin = true;

                if (wp.Type != null &&
                    (wp.Type.Equals(nameof(WaypointType.NPCCompletion), StringComparison.OrdinalIgnoreCase) ||
                     wp.Type.Equals(nameof(WaypointType.BossCompletion), StringComparison.OrdinalIgnoreCase)))
                    hasCompletion = true;

                // POI is required for any spawned NPC/boss objective. (We allow empty poiId only for "spawn at player" in future.)
                if (string.IsNullOrWhiteSpace(wp.PoiId))
                    r.Errors.Add($"Waypoint[{i}] missing poiId.");
                else if (!VystiaPoiRegistry.TryGet(wp.PoiId, out _))
                    r.Errors.Add($"Waypoint[{i}] poiId '{wp.PoiId}' not found in POI registry.");

                if (wp.Condition != null &&
                    wp.Condition.Equals(nameof(WaypointCondition.DefeatBoss), StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(wp.BossTypeName))
                        r.Errors.Add($"Waypoint[{i}] DefeatBoss requires bossTypeName.");
                    else
                    {
                        // Normalize boss type name: remove spaces and trim (e.g., "Frost Father" -> "FrostFather")
                        string normalizedBossType = wp.BossTypeName.Trim().Replace(" ", "").Replace("-", "");
                        if (!AllowedBossTypeNames.Contains(normalizedBossType))
                            r.Errors.Add($"Waypoint[{i}] bossTypeName '{wp.BossTypeName}' (normalized: '{normalizedBossType}') is not allowlisted.");
                    }
                }

                if (wp.RequiredAmount < 1 || wp.RequiredAmount > 1000)
                    r.Errors.Add($"Waypoint[{i}] requiredAmount out of bounds (1..1000).");

                if (wp.Radius < 1 || wp.Radius > 200)
                    r.Errors.Add($"Waypoint[{i}] radius out of bounds (1..200).");
            }

            if (!hasOrigin)
                r.Errors.Add("Plan missing Origin waypoint.");
            if (!hasCompletion)
                r.Errors.Add("Plan missing Completion waypoint (NPCCompletion or BossCompletion).");

            return r;
        }
    }
}


