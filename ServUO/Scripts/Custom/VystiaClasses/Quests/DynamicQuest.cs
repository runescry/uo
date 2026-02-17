using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Custom.VystiaClasses.Quests.Generation;

namespace Server.Custom.VystiaClasses.Quests
{
    /// <summary>
    /// A quest that can be created and configured at runtime by GMs
    /// </summary>
    public class DynamicQuest : VystiaQuest
    {
        private string m_Title;
        private string m_Description;
        private PlayerClassTypeV2 m_RequiredClass;
        private int m_PrerequisiteQuestID;
        private QuestTier m_Tier;
        private Server.Custom.VystiaClasses.Religion.VystiaReligion m_Religion = Server.Custom.VystiaClasses.Religion.VystiaReligion.None;
        private int m_PietyReward = 0;
        private Server.Custom.VystiaClasses.Factions.VystiaFaction m_Faction = Server.Custom.VystiaClasses.Factions.VystiaFaction.None;
        private int m_ReputationReward = 0;
        private int m_ReputationTier = 0;
        private List<QuestObjective> m_ObjectivesList = new List<QuestObjective>();
        private List<QuestReward> m_Rewards = new List<QuestReward>();
        private List<QuestWaypoint> m_Waypoints = new List<QuestWaypoint>();

        // Generated / temporary quests should not be persisted to Data/VystiaQuests.xml.
        // They are cleaned up by the quest instance manager.
        public bool IsEphemeral { get; set; }

        public override string Title => m_Title;
        public override string Description => m_Description;
        public override PlayerClassTypeV2 RequiredClass => m_RequiredClass;
        public override int PrerequisiteQuestID => m_PrerequisiteQuestID;
        public override QuestTier Tier => m_Tier;
        public override Server.Custom.VystiaClasses.Religion.VystiaReligion Religion => m_Religion;
        public override int PietyReward => m_PietyReward;
        public override Server.Custom.VystiaClasses.Factions.VystiaFaction Faction => m_Faction;
        public override int ReputationReward => m_ReputationReward;
        public override int ReputationTier => m_ReputationTier;

        public List<QuestObjective> ObjectivesList => m_ObjectivesList;
        public List<QuestReward> Rewards => m_Rewards;
        public List<QuestWaypoint> Waypoints => m_Waypoints;

        #region Waypoint Management

        public QuestWaypoint GetOrigin() =>
            m_Waypoints.FirstOrDefault(w => w.Type == WaypointType.Origin);

        public QuestWaypoint GetCompletion() =>
            m_Waypoints.FirstOrDefault(w =>
                w.Type == WaypointType.BossCompletion || w.Type == WaypointType.NPCCompletion);

        public IEnumerable<QuestWaypoint> GetIntermediateWaypoints() =>
            m_Waypoints.Where(w => w.Type == WaypointType.Waypoint).OrderBy(w => w.OrderIndex);

        public void AddWaypoint(QuestWaypoint waypoint)
        {
            waypoint.WaypointID = m_Waypoints.Count > 0 ? m_Waypoints.Max(w => w.WaypointID) + 1 : 1;
            waypoint.OrderIndex = m_Waypoints.Count;
            m_Waypoints.Add(waypoint);
        }

        public void RemoveWaypoint(int waypointId)
        {
            m_Waypoints.RemoveAll(w => w.WaypointID == waypointId);
            // Reorder remaining waypoints
            for (int i = 0; i < m_Waypoints.Count; i++)
            {
                m_Waypoints[i].OrderIndex = i;
            }
        }

        public QuestWaypoint GetWaypoint(int waypointId) =>
            m_Waypoints.FirstOrDefault(w => w.WaypointID == waypointId);

        public QuestWaypoint GetWaypointByOrder(int orderIndex) =>
            m_Waypoints.FirstOrDefault(w => w.OrderIndex == orderIndex);

        public QuestWaypoint GetCurrentWaypoint(QuestProgress progress)
        {
            foreach (var waypoint in m_Waypoints.OrderBy(w => w.OrderIndex))
            {
                if (!IsWaypointComplete(waypoint, progress))
                    return waypoint;
            }
            return null; // All complete
        }

        public bool IsWaypointComplete(QuestWaypoint waypoint, QuestProgress progress)
        {
            // Support both current objective keys (quest-scoped) and legacy keys (waypoint_{id})
            string key = waypoint.GetObjectiveKeyForProgress();
            if (progress.GetProgress(key) >= 1)
                return true;

            string legacyKey = waypoint.GetLegacyObjectiveKeyForProgress();
            return !key.Equals(legacyKey, StringComparison.OrdinalIgnoreCase) && progress.GetProgress(legacyKey) >= 1;
        }

        public void MoveWaypointUp(int waypointId)
        {
            var waypoint = GetWaypoint(waypointId);
            if (waypoint == null || waypoint.OrderIndex <= 0)
                return;

            var prevWaypoint = GetWaypointByOrder(waypoint.OrderIndex - 1);
            if (prevWaypoint != null)
            {
                prevWaypoint.OrderIndex++;
                waypoint.OrderIndex--;
            }
        }

        public void MoveWaypointDown(int waypointId)
        {
            var waypoint = GetWaypoint(waypointId);
            if (waypoint == null || waypoint.OrderIndex >= m_Waypoints.Count - 1)
                return;

            var nextWaypoint = GetWaypointByOrder(waypoint.OrderIndex + 1);
            if (nextWaypoint != null)
            {
                nextWaypoint.OrderIndex--;
                waypoint.OrderIndex++;
            }
        }

        public void ClearWaypoints()
        {
            m_Waypoints.Clear();
        }

        #endregion

        public DynamicQuest()
        {
            m_Title = "New Quest";
            m_Description = "Quest description";
            m_RequiredClass = PlayerClassTypeV2.None;
            m_PrerequisiteQuestID = 0;
            m_Tier = QuestTier.Initiation;
        }

        public void SetTitle(string title) => m_Title = title;
        public void SetDescription(string desc) => m_Description = desc;
        public void SetRequiredClass(PlayerClassTypeV2 classType) => m_RequiredClass = classType;
        public void SetPrerequisiteQuestID(int id) => m_PrerequisiteQuestID = id;
        public void SetTier(QuestTier tier) => m_Tier = tier;
        public void SetReligion(Server.Custom.VystiaClasses.Religion.VystiaReligion religion) => m_Religion = religion;
        public void SetPietyReward(int piety) => m_PietyReward = piety;
        public void SetFaction(Server.Custom.VystiaClasses.Factions.VystiaFaction faction) => m_Faction = faction;
        public void SetReputationReward(int reputation) => m_ReputationReward = reputation;
        public void SetReputationTier(int tier) => m_ReputationTier = tier;

        public void AddObjective(string key, int amount, string displayName)
        {
            Objectives[key] = amount;
            m_ObjectivesList.Add(new QuestObjective(key, amount, displayName));
        }

        public void RemoveObjective(string key)
        {
            Objectives.Remove(key);
            m_ObjectivesList.RemoveAll(o => o.Key == key);
        }

        public void ClearObjectives()
        {
            Objectives.Clear();
            m_ObjectivesList.Clear();
        }

        public void AddReward(QuestReward reward)
        {
            m_Rewards.Add(reward);
        }

        public void ClearRewards()
        {
            m_Rewards.Clear();
        }

        public override void GiveRewards(PlayerMobile pm)
        {
            foreach (var reward in m_Rewards)
            {
                reward.Give(pm);
            }

            if (m_Rewards.Count > 0)
                pm.SendMessage("You have received your quest rewards!");
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("Quest");
            writer.WriteAttributeString("id", QuestID.ToString());
            writer.WriteAttributeString("ephemeral", IsEphemeral ? "true" : "false");
            writer.WriteElementString("Title", m_Title);
            writer.WriteElementString("Description", m_Description);
            writer.WriteElementString("RequiredClass", m_RequiredClass.ToString());
            writer.WriteElementString("PrerequisiteQuestID", m_PrerequisiteQuestID.ToString());
            writer.WriteElementString("Tier", ((int)m_Tier).ToString());
            writer.WriteElementString("Religion", ((int)m_Religion).ToString());
            writer.WriteElementString("PietyReward", m_PietyReward.ToString());
            writer.WriteElementString("Faction", ((int)m_Faction).ToString());
            writer.WriteElementString("ReputationReward", m_ReputationReward.ToString());
            writer.WriteElementString("ReputationTier", m_ReputationTier.ToString());

            writer.WriteStartElement("Objectives");
            foreach (var obj in m_ObjectivesList)
            {
                writer.WriteStartElement("Objective");
                writer.WriteAttributeString("key", obj.Key);
                writer.WriteAttributeString("amount", obj.Amount.ToString());
                writer.WriteAttributeString("display", obj.DisplayName);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Rewards");
            foreach (var reward in m_Rewards)
            {
                reward.Save(writer);
            }
            writer.WriteEndElement();

            // Save waypoints
            writer.WriteStartElement("Waypoints");
            foreach (var waypoint in m_Waypoints.OrderBy(w => w.OrderIndex))
            {
                waypoint.SaveXml(writer);
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public static DynamicQuest Load(XmlElement node)
        {
            var quest = new DynamicQuest();

            quest.m_Title = node["Title"]?.InnerText ?? "Untitled";
            quest.m_Description = node["Description"]?.InnerText ?? "";
            quest.m_RequiredClass = ParseRequiredClass(node["RequiredClass"]?.InnerText);
            quest.m_PrerequisiteQuestID = int.Parse(node["PrerequisiteQuestID"]?.InnerText ?? "0");
            quest.m_Tier = (QuestTier)int.Parse(node["Tier"]?.InnerText ?? "0");
            quest.m_Religion = (Server.Custom.VystiaClasses.Religion.VystiaReligion)int.Parse(node["Religion"]?.InnerText ?? "0");
            quest.m_PietyReward = int.Parse(node["PietyReward"]?.InnerText ?? "0");
            quest.m_Faction = (Server.Custom.VystiaClasses.Factions.VystiaFaction)int.Parse(node["Faction"]?.InnerText ?? "0");
            quest.m_ReputationReward = int.Parse(node["ReputationReward"]?.InnerText ?? "0");
            quest.m_ReputationTier = int.Parse(node["ReputationTier"]?.InnerText ?? "0");
            quest.IsEphemeral = node.GetAttribute("ephemeral").Equals("true", StringComparison.OrdinalIgnoreCase);

            var objectivesNode = node["Objectives"];
            if (objectivesNode != null)
            {
                foreach (XmlElement objNode in objectivesNode.GetElementsByTagName("Objective"))
                {
                    string key = objNode.GetAttribute("key");
                    int amount = int.Parse(objNode.GetAttribute("amount"));
                    string display = objNode.GetAttribute("display");
                    quest.AddObjective(key, amount, display);
                }
            }

            var rewardsNode = node["Rewards"];
            if (rewardsNode != null)
            {
                foreach (XmlElement rewardNode in rewardsNode.ChildNodes)
                {
                    var reward = QuestReward.Load(rewardNode);
                    if (reward != null)
                        quest.m_Rewards.Add(reward);
                }
            }

            // Load waypoints
            var waypointsNode = node["Waypoints"];
            if (waypointsNode != null)
            {
                foreach (XmlElement wpNode in waypointsNode.GetElementsByTagName("Waypoint"))
                {
                    var waypoint = QuestWaypoint.LoadXml(wpNode);
                    quest.m_Waypoints.Add(waypoint);
                }
            }

            return quest;
        }

        private static PlayerClassTypeV2 ParseRequiredClass(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return PlayerClassTypeV2.None;

            if (Enum.TryParse(rawValue, true, out PlayerClassTypeV2 v2Value))
                return v2Value;

            if (int.TryParse(rawValue, out int intValue))
            {
                if (Enum.IsDefined(typeof(PlayerClassType), intValue))
                {
                    var v1Value = (PlayerClassType)intValue;
                    if (Enum.TryParse(v1Value.ToString(), true, out PlayerClassTypeV2 v2FromV1))
                        return v2FromV1;
                }

                if (Enum.IsDefined(typeof(PlayerClassTypeV2), intValue))
                    return (PlayerClassTypeV2)intValue;
            }

            return PlayerClassTypeV2.None;
        }
    }

    /// <summary>
    /// Quest objective data
    /// </summary>
    public class QuestObjective
    {
        public string Key { get; set; }
        public int Amount { get; set; }
        public string DisplayName { get; set; }

        public QuestObjective(string key, int amount, string displayName)
        {
            Key = key;
            Amount = amount;
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// Base class for quest rewards
    /// </summary>
    public abstract class QuestReward
    {
        public abstract void Give(PlayerMobile pm);
        public abstract void Save(XmlWriter writer);
        public abstract string GetDescription();

        public static QuestReward Load(XmlElement node)
        {
            string type = node.Name;
            switch (type)
            {
                case "GoldReward":
                    return new GoldReward(int.Parse(node.GetAttribute("amount")));
                case "SkillReward":
                    return new SkillReward(
                        (SkillName)Enum.Parse(typeof(SkillName), node.GetAttribute("skill")),
                        double.Parse(node.GetAttribute("amount")));
                case "ItemReward":
                    return new ItemReward(
                        node.GetAttribute("typeName"),
                        int.Parse(node.GetAttribute("amount")));
                case "TitleReward":
                    return new TitleReward(node.GetAttribute("title"));
                default:
                    return null;
            }
        }
    }

    public class GoldReward : QuestReward
    {
        public int Amount { get; set; }

        public GoldReward(int amount) => Amount = amount;

        public override void Give(PlayerMobile pm)
        {
            pm.Backpack?.DropItem(new Gold(Amount));
            pm.SendMessage($"You receive {Amount} gold.");
        }

        public override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("GoldReward");
            writer.WriteAttributeString("amount", Amount.ToString());
            writer.WriteEndElement();
        }

        public override string GetDescription() => $"{Amount} gold";
    }

    public class SkillReward : QuestReward
    {
        public SkillName Skill { get; set; }
        public double Amount { get; set; }

        public SkillReward(SkillName skill, double amount)
        {
            Skill = skill;
            Amount = amount;
        }

        public override void Give(PlayerMobile pm)
        {
            pm.Skills[Skill].Base += Amount;
            pm.SendMessage($"Your {Skill} skill increases by {Amount}!");
        }

        public override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("SkillReward");
            writer.WriteAttributeString("skill", Skill.ToString());
            writer.WriteAttributeString("amount", Amount.ToString());
            writer.WriteEndElement();
        }

        public override string GetDescription() => $"+{Amount} {Skill}";
    }

    public class ItemReward : QuestReward
    {
        public string TypeName { get; set; }
        public int Amount { get; set; }

        public ItemReward(string typeName, int amount)
        {
            TypeName = typeName;
            Amount = amount;
        }

        public override void Give(PlayerMobile pm)
        {
            Type itemType = ScriptCompiler.FindTypeByName(TypeName);
            if (itemType != null)
            {
                for (int i = 0; i < Amount; i++)
                {
                    Item item = Activator.CreateInstance(itemType) as Item;
                    if (item != null)
                        pm.Backpack?.DropItem(item);
                }
                pm.SendMessage($"You receive {Amount}x {TypeName}.");
            }
        }

        public override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("ItemReward");
            writer.WriteAttributeString("typeName", TypeName);
            writer.WriteAttributeString("amount", Amount.ToString());
            writer.WriteEndElement();
        }

        public override string GetDescription() => $"{Amount}x {TypeName}";
    }

    public class TitleReward : QuestReward
    {
        public string PlayerTitle { get; set; }

        public TitleReward(string title) => PlayerTitle = title;

        public override void Give(PlayerMobile pm)
        {
            pm.Title = PlayerTitle;
            pm.SendMessage($"You have earned the title: {PlayerTitle}");
        }

        public override void Save(XmlWriter writer)
        {
            writer.WriteStartElement("TitleReward");
            writer.WriteAttributeString("title", PlayerTitle);
            writer.WriteEndElement();
        }

        public override string GetDescription() => $"Title: {PlayerTitle}";
    }

    /// <summary>
    /// Manages saving/loading of dynamic quests
    /// </summary>
    public static class DynamicQuestManager
    {
        private static string SavePath => Path.Combine(Core.BaseDirectory, "Data", "VystiaQuests.xml");
        private static List<DynamicQuest> s_DynamicQuests = new List<DynamicQuest>();
        private static bool s_Initialized = false;

        public static List<DynamicQuest> GetAllDynamicQuests()
        {
            EnsureInitialized();
            return s_DynamicQuests;
        }

        public static DynamicQuest GetQuest(int questId)
        {
            EnsureInitialized();
            return s_DynamicQuests.FirstOrDefault(q => q.QuestID == questId);
        }

        public static DynamicQuest GetQuestByTitle(string title)
        {
            EnsureInitialized();
            return s_DynamicQuests.FirstOrDefault(q =>
                q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        private static void EnsureInitialized()
        {
            if (!s_Initialized)
                Initialize();
        }

        public static void RegisterDynamicQuest(DynamicQuest quest)
        {
            EnsureInitialized();
            VystiaQuestSystem.RegisterQuest(quest);
            EnsureWaypointObjectiveKeys(quest);
            s_DynamicQuests.Add(quest);
        }

        public static void RemoveDynamicQuest(DynamicQuest quest)
        {
            if (quest == null)
                return;

            int questID = quest.QuestID;
            
            // Remove from registry
            s_DynamicQuests.Remove(quest);
            VystiaQuestSystem.UnregisterQuest(questID);
            
            // Clean up player tracking to prevent "Vystia Quest #X" from appearing in quest logs
            VystiaQuestSystem.RemoveQuestFromAllPlayers(questID);
            
            // Clean up generated quest instances and delete spawned entities for all players
            int instanceCount = 0;
            int deletedEntityCount = 0;
            
            foreach (Mobile m in Server.World.Mobiles.Values)
            {
                if (m is Server.Mobiles.PlayerMobile pm)
                {
                    var attachment = GeneratedQuestInstanceAttachment.Get(pm);
                    if (attachment != null)
                    {
                        // Find all instances for this quest
                        var instancesToRemove = attachment.Instances.Where(inst => inst.QuestId == questID).ToList();
                        
                        foreach (var inst in instancesToRemove)
                        {
                            // Delete all spawned entities (NPCs, bosses, items)
                            if (inst.SpawnedSerials != null)
                            {
                                foreach (int serialValue in inst.SpawnedSerials)
                                {
                                    try
                                    {
                                        Serial serial = (Serial)serialValue;
                                        
                                        // Try to find as mobile first
                                        var mob = World.FindMobile(serial);
                                        if (mob != null && !mob.Deleted)
                                        {
                                            mob.Delete();
                                            deletedEntityCount++;
                                            continue;
                                        }
                                        
                                        // Try to find as item
                                        var item = World.FindItem(serial);
                                        if (item != null && !item.Deleted)
                                        {
                                            item.Delete();
                                            deletedEntityCount++;
                                        }
                                    }
                                    catch
                                    {
                                        // best-effort cleanup
                                    }
                                }
                            }
                            
                            instanceCount++;
                        }
                        
                        // Remove the instances
                        attachment.RemoveInstancesForQuest(questID);
                    }
                }
            }
            
            if (instanceCount > 0 || deletedEntityCount > 0)
            {
                Console.WriteLine($"[DynamicQuestManager] Deleted quest [{questID}]: Removed {instanceCount} instance(s), deleted {deletedEntityCount} spawned entity/entities.");
            }
        }

        public static void Save()
        {
            string dir = Path.GetDirectoryName(SavePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var writer = XmlWriter.Create(SavePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("VystiaQuests");

                foreach (var quest in s_DynamicQuests)
                {
                    // Skip temporary/generated instance quests (they are cleaned up and should not bloat persistent XML).
                    if (quest != null && quest.IsEphemeral)
                        continue;

                    quest.Save(writer);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Console.WriteLine($"VystiaQuests: Saved {s_DynamicQuests.Count} dynamic quests.");
        }

        public static void Load()
        {
            if (!File.Exists(SavePath))
                return;

            try
            {
                var doc = new XmlDocument();
                doc.Load(SavePath);

                var root = doc.DocumentElement;
                if (root == null)
                    return;

                foreach (XmlElement questNode in root.GetElementsByTagName("Quest"))
                {
                    var quest = DynamicQuest.Load(questNode);
                    // Direct registration during load to avoid EnsureInitialized loop
                    VystiaQuestSystem.RegisterQuest(quest);
                    EnsureWaypointObjectiveKeys(quest);
                    s_DynamicQuests.Add(quest);
                }

                Console.WriteLine($"VystiaQuests: Loaded {s_DynamicQuests.Count} dynamic quests.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VystiaQuests: Error loading quests: {ex.Message}");
            }
        }

        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;
            EventSink.WorldSave += OnWorldSave;
            Load();

            Console.WriteLine("VystiaQuests: Dynamic quest system initialized.");
        }

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            Save();
        }

        private static void EnsureWaypointObjectiveKeys(DynamicQuest quest)
        {
            if (quest == null)
                return;

            // Make waypoint keys quest-scoped to avoid collisions across quests.
            // Keep any explicitly-set ObjectiveKey values as-is.
            foreach (var wp in quest.Waypoints)
            {
                if (wp == null)
                    continue;

                if (string.IsNullOrWhiteSpace(wp.ObjectiveKey))
                {
                    wp.ObjectiveKey = $"waypoint_{quest.QuestID}_{wp.WaypointID}";
                }
            }
        }
    }
}
