using System;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.LLM;
using Server.Services.AISidekicks;

// ServUO has built-in types under Server.* that can conflict with ours.
using VystiaWaypointType = Server.Custom.VystiaClasses.Quests.WaypointType;
using VystiaWaypointCondition = Server.Custom.VystiaClasses.Quests.WaypointCondition;

namespace Server.Custom.VystiaClasses.Gumps
{
    /// <summary>
    /// Dropdown-like picker used by the Quest Wizard for clearer choices (big buttons + descriptions).
    /// </summary>
    public class VystiaQuestWizardPickerGump : Gump
    {
        public enum PickerKind
        {
            WaypointType = 0,
            WaypointCondition = 1,
            SidekickArchetype = 2
        }

        private readonly Mobile m_From;
        private readonly PickerKind m_Kind;
        private readonly int m_QuestId;
        private readonly int m_WaypointId;
        private readonly VystiaQuestWizardGump.WizardStep m_ReturnStep;

        // carry wizard state
        private readonly string m_NpcName;
        private readonly string m_NpcTitle;
        private readonly NPCPersonalities.PersonalityType m_Personality;
        private readonly NPCPersonalities.SpeechPattern m_Speech;
        private readonly VystiaQuestWizardGump.WizardStep m_AfterSelectStep;
        private readonly int m_Filter;

        private const int W = 620;
        private const int H = 520;
        private const int Margin = 20;

        public VystiaQuestWizardPickerGump(
            Mobile from,
            PickerKind kind,
            int questId,
            int waypointId,
            VystiaQuestWizardGump.WizardStep returnStep,
            string npcName,
            string npcTitle,
            NPCPersonalities.PersonalityType personality,
            NPCPersonalities.SpeechPattern speech,
            VystiaQuestWizardGump.WizardStep afterSelectStep,
            int filter = 0)
            : base(80, 80)
        {
            m_From = from;
            m_Kind = kind;
            m_QuestId = questId;
            m_WaypointId = waypointId;
            m_ReturnStep = returnStep;
            m_NpcName = npcName ?? "";
            m_NpcTitle = npcTitle ?? "";
            m_Personality = personality;
            m_Speech = speech;
            m_AfterSelectStep = afterSelectStep;
            m_Filter = filter;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            Build();
        }

        private void Build()
        {
            AddPage(0);
            AddBackground(0, 0, W, H, 9270);
            AddAlphaRegion(10, 10, W - 20, H - 20);

            AddHtml(Margin, 18, W - (Margin * 2), 25, Center(Color(GetTitle(), 0xFFFFFF)), false, false);

            int y = 60;
            AddHtml(Margin, y, W - (Margin * 2), 18, Color("Pick one option:", 0xAAAAAA), false, false);
            y += 24;

            switch (m_Kind)
            {
                case PickerKind.WaypointType:
                    y = DrawWaypointTypeOptions(y);
                    break;
                case PickerKind.WaypointCondition:
                    y = DrawWaypointConditionOptions(y);
                    break;
                case PickerKind.SidekickArchetype:
                    y = DrawSidekickArchetypes(y);
                    break;
            }

            AddButton(Margin, H - 45, 4014, 4016, 2, GumpButtonType.Reply, 0);
            AddHtml(Margin + 35, H - 45, 120, 20, Color("Back", 0xAAAAAA), false, false);
        }

        private string GetTitle()
        {
            switch (m_Kind)
            {
                case PickerKind.WaypointType: return "Select Waypoint Type";
                case PickerKind.WaypointCondition: return "Select Completion Condition";
                case PickerKind.SidekickArchetype: return "Select Sidekick Archetype";
                default: return "Select";
            }
        }

        private int DrawWaypointTypeOptions(int y)
        {
            var options = new[]
            {
                new { Value = VystiaWaypointType.Origin, Label = "Origin", Desc = "Quest start (usually the quest giver NPC)." },
                new { Value = VystiaWaypointType.Waypoint, Label = "Waypoint", Desc = "Intermediate step in the chain." },
                new { Value = VystiaWaypointType.NPCCompletion, Label = "NPC End", Desc = "Quest ends by talking to the final NPC." },
                new { Value = VystiaWaypointType.BossCompletion, Label = "Boss End", Desc = "Quest ends by defeating the final boss." }
            };

            int id = 1000;
            foreach (var opt in options)
            {
                AddButton(Margin, y, 4005, 4007, id, GumpButtonType.Reply, 0);
                AddHtml(Margin + 35, y, 200, 20, Color(opt.Label, 0x88FF88), false, false);
                AddHtml(Margin + 220, y, W - (Margin + 220) - 30, 40, Color(opt.Desc, 0xAAAAAA), false, false);
                y += 45;
                id++;
            }
            return y;
        }

        private int DrawWaypointConditionOptions(int y)
        {
            // Filter meanings:
            // 0 = Any (all options)
            // 1 = Origin (Talk/Reach)
            // 2 = Completion (Talk/Defeat)
            var all = new[]
            {
                new { Value = VystiaWaypointCondition.TalkToNPC, Label = "Talk to NPC", Desc = "Player must talk to the linked QuestNPC." },
                new { Value = VystiaWaypointCondition.ReachLocation, Label = "Reach Location", Desc = "Player must walk into a radius at the saved location." },
                new { Value = VystiaWaypointCondition.DefeatBoss, Label = "Defeat Creature", Desc = "Player must kill a creature type (TypeName) a number of times." },
                new { Value = VystiaWaypointCondition.CollectItems, Label = "Collect Items", Desc = "Player must collect an item type (TypeName) a number of times." },
                new { Value = VystiaWaypointCondition.CastSpell, Label = "Cast Spell", Desc = "Player must cast a spell a number of times." },
                new { Value = VystiaWaypointCondition.RecruitSidekick, Label = "Recruit Sidekick", Desc = "Player must recruit a sidekick (optionally a specific archetype)." },
                new { Value = VystiaWaypointCondition.Custom, Label = "Custom Key", Desc = "Advanced: you increment a custom objective key via scripts." }
            };

            var options = all;
            if (m_Filter == 1)
                options = all.Where(o => o.Value == VystiaWaypointCondition.TalkToNPC || o.Value == VystiaWaypointCondition.ReachLocation).ToArray();
            else if (m_Filter == 2)
                options = all.Where(o => o.Value == VystiaWaypointCondition.TalkToNPC || o.Value == VystiaWaypointCondition.DefeatBoss).ToArray();

            int id = 2000;
            foreach (var opt in options)
            {
                AddButton(Margin, y, 4005, 4007, id, GumpButtonType.Reply, 0);
                AddHtml(Margin + 35, y, 200, 20, Color(opt.Label, 0x88FF88), false, false);
                AddHtml(Margin + 220, y, W - (Margin + 220) - 30, 40, Color(opt.Desc, 0xAAAAAA), false, false);
                y += 45;
                id++;
            }
            return y;
        }

        private int DrawSidekickArchetypes(int y)
        {
            AddButton(Margin, y, 4005, 4007, 3000, GumpButtonType.Reply, 0);
            AddHtml(Margin + 35, y, 200, 20, Color("Any archetype", 0x88FF88), false, false);
            AddHtml(Margin + 220, y, W - (Margin + 220) - 30, 40, Color("Leave blank so any recruited sidekick completes the waypoint.", 0xAAAAAA), false, false);
            y += 45;

            var values = Enum.GetValues(typeof(SidekickArchetypeType)).Cast<SidekickArchetypeType>().ToArray();
            int id = 3001;
            foreach (var v in values)
            {
                AddButton(Margin, y, 4005, 4007, id, GumpButtonType.Reply, 0);
                AddHtml(Margin + 35, y, 200, 20, Color(v.ToString(), 0x88FF88), false, false);
                y += 30;
                id++;
                if (y > H - 80)
                    break;
            }
            return y;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_From.Deleted)
                return;

            int buttonID = info.ButtonID;
            if (buttonID == 0)
                return;

            // Back
            if (buttonID == 2)
            {
                m_From.SendGump(new VystiaQuestWizardGump(m_From, m_ReturnStep, m_QuestId, m_WaypointId, false, m_NpcName, m_NpcTitle, m_Personality, m_Speech, m_AfterSelectStep));
                return;
            }

            var quest = DynamicQuestManager.GetQuest(m_QuestId);
            var wp = quest?.GetWaypoint(m_WaypointId);
            if (quest == null || wp == null)
            {
                m_From.SendGump(new VystiaQuestWizardGump(m_From, m_ReturnStep, m_QuestId, m_WaypointId, false, m_NpcName, m_NpcTitle, m_Personality, m_Speech, m_AfterSelectStep));
                return;
            }

            if (m_Kind == PickerKind.WaypointType && buttonID >= 1000 && buttonID < 1100)
            {
                int idx = buttonID - 1000;
                var values = new[] { VystiaWaypointType.Origin, VystiaWaypointType.Waypoint, VystiaWaypointType.NPCCompletion, VystiaWaypointType.BossCompletion };
                if (idx >= 0 && idx < values.Length)
                {
                    wp.Type = values[idx];
                    DynamicQuestManager.Save();
                }
            }
            else if (m_Kind == PickerKind.WaypointCondition && buttonID >= 2000 && buttonID < 2100)
            {
                int idx = buttonID - 2000;
                var all = new[]
                {
                    VystiaWaypointCondition.TalkToNPC,
                    VystiaWaypointCondition.ReachLocation,
                    VystiaWaypointCondition.DefeatBoss,
                    VystiaWaypointCondition.CollectItems,
                    VystiaWaypointCondition.CastSpell,
                    VystiaWaypointCondition.RecruitSidekick,
                    VystiaWaypointCondition.Custom
                };
                var values = all;
                if (m_Filter == 1)
                    values = all.Where(v => v == VystiaWaypointCondition.TalkToNPC || v == VystiaWaypointCondition.ReachLocation).ToArray();
                else if (m_Filter == 2)
                    values = all.Where(v => v == VystiaWaypointCondition.TalkToNPC || v == VystiaWaypointCondition.DefeatBoss).ToArray();

                if (idx >= 0 && idx < values.Length)
                {
                    wp.Condition = values[idx];
                    DynamicQuestManager.Save();
                }
            }
            else if (m_Kind == PickerKind.SidekickArchetype && buttonID >= 3000 && buttonID < 4000)
            {
                if (buttonID == 3000)
                {
                    wp.TargetTypeName = "";
                    DynamicQuestManager.Save();
                }
                else
                {
                    int idx = buttonID - 3001;
                    var values = Enum.GetValues(typeof(SidekickArchetypeType)).Cast<SidekickArchetypeType>().ToArray();
                    if (idx >= 0 && idx < values.Length)
                    {
                        wp.TargetTypeName = values[idx].ToString();
                        DynamicQuestManager.Save();
                    }
                }
            }

            m_From.SendGump(new VystiaQuestWizardGump(m_From, m_ReturnStep, m_QuestId, m_WaypointId, false, m_NpcName, m_NpcTitle, m_Personality, m_Speech, m_AfterSelectStep));
        }

        private string Color(string text, int color) => $"<BASEFONT COLOR=#{color:X6}>{text}</BASEFONT>";
        private string Center(string text) => $"<CENTER>{text}</CENTER>";
    }
}


