using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Server;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    public sealed class NpcTemplate
    {
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("mobileTypeName")]
        public string MobileTypeName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("defaultPersonality")]
        public string DefaultPersonality { get; set; }

        [JsonProperty("defaultSpeechPattern")]
        public string DefaultSpeechPattern { get; set; }

        [JsonProperty("knowledgeTags")]
        public List<string> KnowledgeTags { get; set; } = new List<string>();
    }

    /// <summary>
    /// NPC template registry: loads Vystia NPC templates for quest generation.
    /// Allows LLM to select NPCs by templateId and spawn them as avatars.
    /// </summary>
    public static class VystiaNpcTemplateRegistry
    {
        private static readonly object _sync = new object();
        private static bool _loaded;
        private static Dictionary<string, NpcTemplate> _byId = new Dictionary<string, NpcTemplate>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, NpcTemplate> _byMobileType = new Dictionary<string, NpcTemplate>(StringComparer.OrdinalIgnoreCase);

        public static string RegistryPath =>
            Path.Combine(Core.BaseDirectory, "Data", "Vystia", "npc_templates.json");

        public static void EnsureLoaded()
        {
            if (_loaded)
                return;

            lock (_sync)
            {
                if (_loaded)
                    return;

                _byId = new Dictionary<string, NpcTemplate>(StringComparer.OrdinalIgnoreCase);
                _byMobileType = new Dictionary<string, NpcTemplate>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    if (File.Exists(RegistryPath))
                    {
                        string json = File.ReadAllText(RegistryPath);
                        var templates = JsonConvert.DeserializeObject<List<NpcTemplate>>(json) ?? new List<NpcTemplate>();

                        foreach (var template in templates)
                        {
                            if (template == null || string.IsNullOrWhiteSpace(template.TemplateId))
                                continue;

                            _byId[template.TemplateId.Trim()] = template;

                            if (!string.IsNullOrWhiteSpace(template.MobileTypeName))
                            {
                                _byMobileType[template.MobileTypeName.Trim()] = template;
                            }
                        }

                        Console.WriteLine($"[VystiaNpcTemplateRegistry] Loaded {_byId.Count} NPC templates.");
                    }
                    else
                    {
                        Console.WriteLine($"[VystiaNpcTemplateRegistry] Missing NPC template registry at {RegistryPath}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VystiaNpcTemplateRegistry] Failed to load templates: {ex.Message}");
                }

                _loaded = true;
            }
        }

        public static bool TryGet(string templateId, out NpcTemplate template)
        {
            EnsureLoaded();

            template = null;
            if (string.IsNullOrWhiteSpace(templateId))
                return false;

            return _byId.TryGetValue(templateId.Trim(), out template) && template != null;
        }

        public static bool TryGetByMobileType(string mobileTypeName, out NpcTemplate template)
        {
            EnsureLoaded();

            template = null;
            if (string.IsNullOrWhiteSpace(mobileTypeName))
                return false;

            return _byMobileType.TryGetValue(mobileTypeName.Trim(), out template) && template != null;
        }

        public static IEnumerable<NpcTemplate> GetAll()
        {
            EnsureLoaded();
            return _byId.Values;
        }

        public static IEnumerable<NpcTemplate> GetByCategory(string categoryPrefix)
        {
            EnsureLoaded();
            foreach (var template in _byId.Values)
            {
                if (template?.TemplateId != null && template.TemplateId.StartsWith(categoryPrefix, StringComparison.OrdinalIgnoreCase))
                    yield return template;
            }
        }
    }
}

