using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses.Quests;
using Server.Services.LLM;

namespace Server.Custom.VystiaClasses.Gumps
{
    /// <summary>
    /// GM picker for spawning curated Vystia NPCs (bosses, leaders, key NPCs) at the GM's location.
    /// This is NOT the quest-linked QuestNPC spawner; it's for world content placement.
    /// </summary>
    public class VystiaNPCSpawnPickerGump : Gump
    {
        private enum Category
        {
            Bosses = 0,
            Leaders = 1,
            TalkingCreatures = 2,
            QuestGivers = 3,
            Vendors = 4
        }

        private readonly Mobile m_From;
        private readonly Category m_Category;

        // Return to wizard state
        private readonly VystiaQuestWizardGump.WizardStep m_ReturnStep;
        private readonly int m_ReturnQuestId;
        private readonly int m_ReturnWaypointId;
        private readonly string m_NpcName;
        private readonly string m_NpcTitle;
        private readonly NPCPersonalities.PersonalityType m_Personality;
        private readonly NPCPersonalities.SpeechPattern m_Speech;
        private readonly VystiaQuestWizardGump.WizardStep m_AfterSelectStep;

        private const int W = 930;
        private const int H = 780;
        private const int Margin = 20;

        // Curated lists (TypeName must match script class names)
        private static readonly string[] Bosses = new[]
        {
            "AncientKraken",
            "AncientTreant",
            "CovenMatriarch",
            "CrystalDrakeAlpha",
            "ForgeMaster",
            "FrostFather",
            "GriffinLord",
            "SphinxOfSurya",
            "TimewornLich",
            "VolcanoWyrm"
        };

        private static readonly string[] Leaders = new[]
        {
            "EmperorGarrickSteelarm",
            "ChieftainBjornFrostbeard",
            "ElderSeraphinaLeafwhisper",
            "SultanAziralRashid",
            "ArchmagePyrusAshborn",
            "AdmiralMarisHawkseye",
            "LordMarinerTideseeker",
            "HighGuardianEldurMountainborn",
            "ChiefMaraWildsong",
            "ArchmageLumis",
            "SorceressNocturna",
            "SageOrin",
            "DruidLordFaelar",
            "GuardianSylas",
            "QueenIceshadow",
            "QueenAmaryllis",
            "WarlordEmberonFlamefist",
            "SheikhTarik",
            "WarlordKael",
            "KingFrostbeard"
        };

        private static readonly string[] TalkingCreatures = new[]
        {
            "AbyssusDepthKing",
            "CrystalwingPrismaticOracle",
            "ElderOakbark",
            "EmberflameAshenTyrant",
            "FrostFatherAvatar",
            "FrosthelmEternalWinter",
            "GreatMachinistConstruct",
            "IronbarkWarAncient",
            "LunaraDryadHerald",
            "SphynxOfEmberlands",
            "TheCrystalSphinx",
            "VerdantheartForestGuardian"
        };

        private static readonly string[] QuestGivers = new[]
        {
            "QuartermasterGrimwald",
            "SageTheron",
            "Chronicler"
        };

        private static readonly string[] Vendors = new[]
        {
            "FrostholmHealer",
            "IronhavenBanker",
            "IronhavenGuardCaptain"
        };

        public VystiaNPCSpawnPickerGump(
            Mobile from,
            int category,
            VystiaQuestWizardGump.WizardStep returnStep,
            int returnQuestId,
            int returnWaypointId,
            string npcName,
            string npcTitle,
            NPCPersonalities.PersonalityType personality,
            NPCPersonalities.SpeechPattern speech,
            VystiaQuestWizardGump.WizardStep afterSelectStep)
            : base(60, 60)
        {
            m_From = from;
            m_Category = (Category)category;
            m_ReturnStep = returnStep;
            m_ReturnQuestId = returnQuestId;
            m_ReturnWaypointId = returnWaypointId;
            m_NpcName = npcName ?? "";
            m_NpcTitle = npcTitle ?? "";
            m_Personality = personality;
            m_Speech = speech;
            m_AfterSelectStep = afterSelectStep;

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

            AddHtml(Margin, 18, W - (Margin * 2), 25, Center(Color("Spawn Vystia NPC", 0xFFFFFF)), false, false);
            AddHtml(Margin, 50, W - (Margin * 2), 20, Color("Select a category and spawn an NPC at your location.", 0xAAAAAA), false, false);

            // Category buttons
            int y = 85;
            int bx = Margin;
            int bw = 170;

            DrawCatButton(bx + (bw * 0), y, 10, "Bosses", m_Category == Category.Bosses);
            DrawCatButton(bx + (bw * 1), y, 11, "Leaders", m_Category == Category.Leaders);
            DrawCatButton(bx + (bw * 2), y, 12, "Talking", m_Category == Category.TalkingCreatures);
            DrawCatButton(bx + (bw * 3), y, 13, "Quest Givers", m_Category == Category.QuestGivers);
            DrawCatButton(bx + (bw * 4), y, 14, "Vendors", m_Category == Category.Vendors);

            y += 45;
            AddImage(Margin, y - 5, 0x39);
            y += 10;

            // List
            foreach (var entry in GetEntriesForCategory(m_Category))
            {
                AddButton(Margin, y, 4005, 4007, entry.ButtonId, GumpButtonType.Reply, 0);
                AddHtml(Margin + 35, y, W - (Margin * 2) - 40, 20, Color(entry.Display, 0x88FF88), false, false);
                y += 26;

                if (y > H - 80)
                    break;
            }

            // Back to wizard
            AddButton(Margin, H - 45, 4014, 4016, 2, GumpButtonType.Reply, 0);
            AddHtml(Margin + 35, H - 45, 200, 20, Color("Back to Quest Wizard", 0xAAAAAA), false, false);
        }

        private void DrawCatButton(int x, int y, int id, string label, bool selected)
        {
            AddButton(x, y, selected ? 9723 : 9720, 9723, id, GumpButtonType.Reply, 0);
            AddHtml(x + 30, y + 2, 130, 20, Color(label, selected ? 0x44FF44 : 0xAAAAAA), false, false);
        }

        private struct Entry
        {
            public int ButtonId;
            public string TypeName;
            public string Display;
        }

        private IEnumerable<Entry> GetEntriesForCategory(Category cat)
        {
            string[] list;
            switch (cat)
            {
                case Category.Bosses: list = Bosses; break;
                case Category.Leaders: list = Leaders; break;
                case Category.TalkingCreatures: list = TalkingCreatures; break;
                case Category.QuestGivers: list = QuestGivers; break;
                case Category.Vendors: list = Vendors; break;
                default: list = Bosses; break;
            }

            int id = 1000;
            foreach (var t in list)
            {
                yield return new Entry
                {
                    ButtonId = id++,
                    TypeName = t,
                    Display = t
                };
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_From.Deleted)
                return;

            int id = info.ButtonID;
            if (id == 0)
                return;

            // Back
            if (id == 2)
            {
                ReturnToWizard();
                return;
            }

            // Category switch
            if (id >= 10 && id <= 14)
            {
                int cat = id - 10;
                m_From.SendGump(new VystiaNPCSpawnPickerGump(m_From, cat, m_ReturnStep, m_ReturnQuestId, m_ReturnWaypointId, m_NpcName, m_NpcTitle, m_Personality, m_Speech, m_AfterSelectStep));
                return;
            }

            // Spawn selection
            if (id >= 1000)
            {
                var entries = GetEntriesForCategory(m_Category).ToList();
                int index = id - 1000;
                if (index >= 0 && index < entries.Count)
                {
                    SpawnType(entries[index].TypeName);
                }
                ReturnToWizard();
                return;
            }

            ReturnToWizard();
        }

        private void SpawnType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return;

            Type t = ScriptCompiler.FindTypeByName(typeName);
            if (t == null)
            {
                m_From.SendMessage($"Type not found: {typeName}");
                return;
            }

            object obj = null;
            try
            {
                obj = Activator.CreateInstance(t);
            }
            catch
            {
                m_From.SendMessage($"Failed to create: {typeName} (missing parameterless [Constructable]?)");
                return;
            }

            if (obj is Mobile m)
            {
                m.MoveToWorld(m_From.Location, m_From.Map);
                m_From.SendMessage($"Spawned: {typeName}");
                Effects.SendLocationParticles(m, 0x376A, 9, 32, 5008);
                m.PlaySound(0x1FE);
            }
            else
            {
                m_From.SendMessage($"Type is not a Mobile: {typeName}");
            }
        }

        private void ReturnToWizard()
        {
            m_From.SendGump(new VystiaQuestWizardGump(
                m_From,
                m_ReturnStep,
                m_ReturnQuestId,
                m_ReturnWaypointId,
                false,
                m_NpcName,
                m_NpcTitle,
                m_Personality,
                m_Speech,
                m_AfterSelectStep));
        }

        private string Color(string text, int color) => $"<BASEFONT COLOR=#{color:X6}>{text}</BASEFONT>";
        private string Center(string text) => $"<CENTER>{text}</CENTER>";
    }
}


