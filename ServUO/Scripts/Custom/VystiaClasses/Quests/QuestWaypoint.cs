using System;
using System.Xml;
using Server;

namespace Server.Custom.VystiaClasses.Quests
{
    /// <summary>
    /// Types of waypoints in a quest chain
    /// </summary>
    public enum WaypointType
    {
        Origin,         // Starting point - where player accepts quest
        Waypoint,       // Intermediate step (location, NPC, or action)
        BossCompletion, // Defeat a boss to complete
        NPCCompletion   // Talk to final NPC to complete
    }

    /// <summary>
    /// Completion conditions for a waypoint
    /// </summary>
    public enum WaypointCondition
    {
        TalkToNPC,      // Must speak to assigned NPC
        ReachLocation,  // Must enter region/coordinates
        DefeatBoss,     // Must kill specific creature
        CollectItems,   // Must gather items
        CastSpell,      // Must cast specific spell
        Custom,         // Custom objective key

        // NOTE: Append new values to preserve numeric values in saved XML/world data
        RecruitSidekick // Recruit a sidekick (AISidekicks) of a specific archetype (optional)
    }

    /// <summary>
    /// Represents a single waypoint in a quest chain
    /// </summary>
    public class QuestWaypoint
    {
        public int WaypointID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public WaypointType Type { get; set; }
        public WaypointCondition Condition { get; set; }

        // Location data
        public Point3D Location { get; set; }
        public Map Map { get; set; }
        public int Radius { get; set; }
        public string RegionName { get; set; }

        // NPC data
        public Serial AssignedNPCSerial { get; set; }
        public string NPCTypeName { get; set; }
        public string NPCDialogueContext { get; set; }
        
        // Player-facing location hint (GM-authored). This is fed to the LLM so NPCs can give flavorful clues
        // like "near the entrance of Destard" or "somewhere in the heart of Despise".
        public string PlayerLocationHint { get; set; }

        // Objective data
        public string ObjectiveKey { get; set; }
        public int RequiredAmount { get; set; }
        public string TargetTypeName { get; set; }

        // Sequencing
        public int OrderIndex { get; set; }
        public bool IsOptional { get; set; }

        // LLM Integration
        public string LLMPersonality { get; set; }
        public string LLMSpeechPattern { get; set; }

        public QuestWaypoint()
        {
            WaypointID = 0;
            Name = "New Waypoint";
            Description = "Waypoint description";
            Type = WaypointType.Waypoint;
            Condition = WaypointCondition.TalkToNPC;
            Location = Point3D.Zero;
            Map = null;
            Radius = 5;
            AssignedNPCSerial = Serial.MinusOne;
            RequiredAmount = 1;
            OrderIndex = 0;
            IsOptional = false;
        }

        public QuestWaypoint(string name, WaypointType type) : this()
        {
            Name = name;
            Type = type;

            // Set default condition based on type
            switch (type)
            {
                case WaypointType.Origin:
                case WaypointType.NPCCompletion:
                    Condition = WaypointCondition.TalkToNPC;
                    break;
                case WaypointType.BossCompletion:
                    Condition = WaypointCondition.DefeatBoss;
                    break;
                default:
                    Condition = WaypointCondition.TalkToNPC;
                    break;
            }
        }

        public void SaveXml(XmlWriter writer)
        {
            writer.WriteStartElement("Waypoint");

            writer.WriteAttributeString("id", WaypointID.ToString());
            writer.WriteAttributeString("order", OrderIndex.ToString());
            writer.WriteAttributeString("type", ((int)Type).ToString());
            writer.WriteAttributeString("condition", ((int)Condition).ToString());

            writer.WriteElementString("Name", Name ?? "");
            writer.WriteElementString("Description", Description ?? "");
            if (!string.IsNullOrEmpty(PlayerLocationHint))
                writer.WriteElementString("PlayerLocationHint", PlayerLocationHint);

            // Location
            writer.WriteStartElement("Location");
            writer.WriteAttributeString("x", Location.X.ToString());
            writer.WriteAttributeString("y", Location.Y.ToString());
            writer.WriteAttributeString("z", Location.Z.ToString());
            writer.WriteAttributeString("map", Map?.Name ?? "");
            writer.WriteAttributeString("radius", Radius.ToString());
            writer.WriteEndElement();

            // NPC
            writer.WriteStartElement("NPC");
            writer.WriteAttributeString("serial", AssignedNPCSerial.Value.ToString());
            writer.WriteAttributeString("typeName", NPCTypeName ?? "");
            if (!string.IsNullOrEmpty(NPCDialogueContext))
                writer.WriteElementString("DialogueContext", NPCDialogueContext);
            if (!string.IsNullOrEmpty(LLMPersonality))
                writer.WriteElementString("Personality", LLMPersonality);
            if (!string.IsNullOrEmpty(LLMSpeechPattern))
                writer.WriteElementString("SpeechPattern", LLMSpeechPattern);
            writer.WriteEndElement();

            // Objective
            writer.WriteStartElement("Objective");
            writer.WriteAttributeString("key", ObjectiveKey ?? "");
            writer.WriteAttributeString("amount", RequiredAmount.ToString());
            writer.WriteAttributeString("targetType", TargetTypeName ?? "");
            writer.WriteAttributeString("optional", IsOptional.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public static QuestWaypoint LoadXml(XmlElement node)
        {
            var waypoint = new QuestWaypoint();

            waypoint.WaypointID = int.Parse(node.GetAttribute("id") ?? "0");
            waypoint.OrderIndex = int.Parse(node.GetAttribute("order") ?? "0");
            waypoint.Type = (WaypointType)int.Parse(node.GetAttribute("type") ?? "0");
            waypoint.Condition = (WaypointCondition)int.Parse(node.GetAttribute("condition") ?? "0");

            waypoint.Name = node["Name"]?.InnerText ?? "Waypoint";
            waypoint.Description = node["Description"]?.InnerText ?? "";
            waypoint.PlayerLocationHint = node["PlayerLocationHint"]?.InnerText ?? "";

            // Location
            var locNode = node["Location"];
            if (locNode != null)
            {
                int x = int.Parse(locNode.GetAttribute("x") ?? "0");
                int y = int.Parse(locNode.GetAttribute("y") ?? "0");
                int z = int.Parse(locNode.GetAttribute("z") ?? "0");
                waypoint.Location = new Point3D(x, y, z);

                string mapName = locNode.GetAttribute("map");
                if (!string.IsNullOrEmpty(mapName))
                    waypoint.Map = Map.Parse(mapName);

                waypoint.Radius = int.Parse(locNode.GetAttribute("radius") ?? "5");
            }

            // NPC
            var npcNode = node["NPC"];
            if (npcNode != null)
            {
                int serial = int.Parse(npcNode.GetAttribute("serial") ?? "-1");
                waypoint.AssignedNPCSerial = (Serial)serial;
                waypoint.NPCTypeName = npcNode.GetAttribute("typeName");
                waypoint.NPCDialogueContext = npcNode["DialogueContext"]?.InnerText;
                waypoint.LLMPersonality = npcNode["Personality"]?.InnerText;
                waypoint.LLMSpeechPattern = npcNode["SpeechPattern"]?.InnerText;
            }

            // Objective
            var objNode = node["Objective"];
            if (objNode != null)
            {
                waypoint.ObjectiveKey = objNode.GetAttribute("key");
                waypoint.RequiredAmount = int.Parse(objNode.GetAttribute("amount") ?? "1");
                waypoint.TargetTypeName = objNode.GetAttribute("targetType");
                waypoint.IsOptional = bool.Parse(objNode.GetAttribute("optional") ?? "false");
            }

            return waypoint;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)1); // version

            writer.Write(WaypointID);
            writer.Write(OrderIndex);
            writer.Write((int)Type);
            writer.Write((int)Condition);

            writer.Write(Name ?? "");
            writer.Write(Description ?? "");

            writer.Write(Location);
            writer.Write(Map);
            writer.Write(Radius);
            writer.Write(RegionName ?? "");

            writer.Write(AssignedNPCSerial);
            writer.Write(NPCTypeName ?? "");
            writer.Write(NPCDialogueContext ?? "");
            writer.Write(PlayerLocationHint ?? "");

            writer.Write(ObjectiveKey ?? "");
            writer.Write(RequiredAmount);
            writer.Write(TargetTypeName ?? "");
            writer.Write(IsOptional);

            writer.Write(LLMPersonality ?? "");
            writer.Write(LLMSpeechPattern ?? "");
        }

        public static QuestWaypoint Deserialize(GenericReader reader)
        {
            var waypoint = new QuestWaypoint();
            int version = reader.ReadInt();

            waypoint.WaypointID = reader.ReadInt();
            waypoint.OrderIndex = reader.ReadInt();
            waypoint.Type = (WaypointType)reader.ReadInt();
            waypoint.Condition = (WaypointCondition)reader.ReadInt();

            waypoint.Name = reader.ReadString();
            waypoint.Description = reader.ReadString();

            waypoint.Location = reader.ReadPoint3D();
            waypoint.Map = reader.ReadMap();
            waypoint.Radius = reader.ReadInt();
            waypoint.RegionName = reader.ReadString();

            waypoint.AssignedNPCSerial = reader.ReadInt();
            waypoint.NPCTypeName = reader.ReadString();
            waypoint.NPCDialogueContext = reader.ReadString();
            if (version >= 1)
                waypoint.PlayerLocationHint = reader.ReadString();
            else
                waypoint.PlayerLocationHint = "";

            waypoint.ObjectiveKey = reader.ReadString();
            waypoint.RequiredAmount = reader.ReadInt();
            waypoint.TargetTypeName = reader.ReadString();
            waypoint.IsOptional = reader.ReadBool();

            waypoint.LLMPersonality = reader.ReadString();
            waypoint.LLMSpeechPattern = reader.ReadString();

            return waypoint;
        }

        public string GetObjectiveKeyForProgress()
        {
            // Prefer explicit ObjectiveKey if present (allows quest-scoped keys),
            // otherwise fall back to legacy behavior for older saved quests.
            if (!string.IsNullOrWhiteSpace(ObjectiveKey))
                return ObjectiveKey;

            return GetLegacyObjectiveKeyForProgress();
        }

        public string GetLegacyObjectiveKeyForProgress()
        {
            return $"waypoint_{WaypointID}";
        }
    }
}
