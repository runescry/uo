using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using Server.Targeting;

namespace Server.Services.LLM
{
    /// <summary>
    /// Gump for spawning and managing LLM NPCs
    /// </summary>
    public class LLMNpcGump : Gump
    {
        private const int GumpWidth = 600;
        private const int GumpHeight = 530;

        public LLMNpcGump() : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Background
            AddBackground(0, 0, GumpWidth, GumpHeight, 9200);
            AddImageTiled(10, 10, GumpWidth - 20, GumpHeight - 20, 2624);
            AddAlphaRegion(10, 10, GumpWidth - 20, GumpHeight - 20);

            // Title
            AddHtml(20, 20, GumpWidth - 40, 40, "<center><basefont color=#FFFFFF size=7><b>LLM NPC Spawner</b></basefont></center>", false, false);

            // Plugin Status Section
            bool pluginEnabled = LLMConversationPlugin.Enabled;
            string statusText = pluginEnabled ? "ENABLED" : "DISABLED";
            string statusColor = pluginEnabled ? "#00FF00" : "#FF0000";
            
            AddHtml(20, 70, 200, 30, "<basefont color=#FFFFFF size=4><b>Conversation Plugin:</b></basefont>", false, false);
            AddHtml(220, 70, 150, 30, $"<basefont color={statusColor} size=4><b>{statusText}</b></basefont>", false, false);
            
            AddButton(GumpWidth - 120, 70, pluginEnabled ? 2117 : 2118, pluginEnabled ? 2118 : 2117, 200, GumpButtonType.Reply, 0);
            AddHtml(GumpWidth - 90, 70, 80, 30, $"<basefont color=#FFFFFF size=3>{(pluginEnabled ? "Disable" : "Enable")}</basefont>", false, false);

            // Global NPC Enable/Disable Section
            AddHtml(20, 110, 200, 30, "<basefont color=#FFFFFF size=4><b>All Vendor NPCs:</b></basefont>", false, false);

            AddButton(220, 110, 2117, 2118, 201, GumpButtonType.Reply, 0);
            AddHtml(250, 110, 100, 30, "<basefont color=#00FF00 size=3>Enable All</basefont>", false, false);

            AddButton(350, 110, 2117, 2118, 202, GumpButtonType.Reply, 0);
            AddHtml(380, 110, 100, 30, "<basefont color=#FF0000 size=3>Disable All</basefont>", false, false);

            // Categories
            AddButton(40, 150, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(75, 150, 200, 30, "<basefont color=#FFFFFF size=5>Town NPCs</basefont>", false, false);

            AddButton(40, 190, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtml(75, 190, 200, 30, "<basefont color=#FFFFFF size=5>Magic & Mystical</basefont>", false, false);

            AddButton(40, 230, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddHtml(75, 230, 200, 30, "<basefont color=#FFFFFF size=5>Adventurers</basefont>", false, false);

            AddButton(40, 270, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddHtml(75, 270, 200, 30, "<basefont color=#FFFFFF size=5>Merchants</basefont>", false, false);

            AddButton(40, 310, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddHtml(75, 310, 200, 30, "<basefont color=#FFFFFF size=5>Underworld</basefont>", false, false);

            AddButton(40, 350, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddHtml(75, 350, 200, 30, "<basefont color=#FFFFFF size=5>Special Characters</basefont>", false, false);

            AddButton(40, 390, 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddHtml(75, 390, 200, 30, "<basefont color=#FFFFFF size=5>Random NPC</basefont>", false, false);

            // Quick Spawn Buttons
            AddHtml(20, 430, GumpWidth - 40, 30, "<center><basefont color=#FFFFFF size=5><b>Quick Spawn Groups</b></basefont></center>", false, false);

            AddButton(40, 465, 4005, 4007, 100, GumpButtonType.Reply, 0);
            AddHtml(75, 465, 150, 30, "<basefont color=#FFFF00 size=4>Spawn Town Set</basefont>", false, false);

            AddButton(250, 465, 4005, 4007, 101, GumpButtonType.Reply, 0);
            AddHtml(285, 465, 150, 30, "<basefont color=#FFFF00 size=4>Spawn Magic Set</basefont>", false, false);

            AddButton(460, 465, 4005, 4007, 102, GumpButtonType.Reply, 0);
            AddHtml(495, 465, 80, 30, "<basefont color=#FFFF00 size=4>Spawn Adventurers</basefont>", false, false);

            // Info
            AddHtml(20, 505, GumpWidth - 40, 30, "<center><basefont color=#00FF00 size=3>Click a category to see available NPCs</basefont></center>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0: // Close
                    break;

                case 1: // Town NPCs
                    from.SendGump(new LLMNpcCategoryGump("Town NPCs", GetTownNPCs()));
                    break;

                case 2: // Magic & Mystical
                    from.SendGump(new LLMNpcCategoryGump("Magic & Mystical", GetMagicNPCs()));
                    break;

                case 3: // Adventurers
                    from.SendGump(new LLMNpcCategoryGump("Adventurers", GetAdventurerNPCs()));
                    break;

                case 4: // Merchants
                    from.SendGump(new LLMNpcCategoryGump("Merchants", GetMerchantNPCs()));
                    break;

                case 5: // Underworld
                    from.SendGump(new LLMNpcCategoryGump("Underworld", GetUnderworldNPCs()));
                    break;

                case 6: // Special
                    from.SendGump(new LLMNpcCategoryGump("Special Characters", GetSpecialNPCs()));
                    break;

                case 7: // Random
                    from.Target = new LLMNpcTarget(null); // null = random
                    from.SendMessage("Target where you want to spawn a random LLM NPC.");
                    break;

                case 100: // Spawn Town Set
                    CommandSystem.Handle(from, $"{CommandSystem.Prefix}SpawnTownNPCs");
                    from.SendGump(new LLMNpcGump());
                    break;

                case 101: // Spawn Magic Set
                    CommandSystem.Handle(from, $"{CommandSystem.Prefix}SpawnMagicNPCs");
                    from.SendGump(new LLMNpcGump());
                    break;

                case 102: // Spawn Adventurer Set
                    CommandSystem.Handle(from, $"{CommandSystem.Prefix}SpawnAdventurerNPCs");
                    from.SendGump(new LLMNpcGump());
                    break;

                case 200: // Toggle Plugin
                    LLMConversationPlugin.Enabled = !LLMConversationPlugin.Enabled;
                    from.SendMessage($"LLM Conversation Plugin {(LLMConversationPlugin.Enabled ? "ENABLED" : "DISABLED")}.");
                    from.SendGump(new LLMNpcGump());
                    break;

                case 201: // Enable All Vendor NPCs
                    EnableDisableAllVendorNPCs(from, true);
                    from.SendGump(new LLMNpcGump());
                    break;

                case 202: // Disable All Vendor NPCs
                    EnableDisableAllVendorNPCs(from, false);
                    from.SendGump(new LLMNpcGump());
                    break;
            }
        }

        /// <summary>
        /// Enable or disable LLM conversations for all vendor NPCs in the world
        /// </summary>
        private static void EnableDisableAllVendorNPCs(Mobile from, bool enable)
        {
            int count = 0;

            // Get all BaseVendor NPCs that implement ILLMConversational
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is BaseVendor vendor && m is ILLMConversational llmNpc)
                {
                    llmNpc.LLMConversationEnabled = enable;
                    count++;
                }
            }

            string action = enable ? "ENABLED" : "DISABLED";
            from.SendMessage(0x22, $"LLM conversations {action} for {count} vendor NPCs.");
            Console.WriteLine($"[LLMNpcGump] {from.Name} {action} LLM for {count} vendor NPCs");
        }

        private static List<string> GetTownNPCs()
        {
            return new List<string> { "Blacksmith", "Tavernkeeper", "Town Guard", "Herbalist", "Librarian", "Fisherman" };
        }

        private static List<string> GetMagicNPCs()
        {
            return new List<string> { "Archmage", "Mystic Fortune Teller", "Necromancer", "Druid" };
        }

        private static List<string> GetAdventurerNPCs()
        {
            return new List<string> { "Veteran Warrior", "Treasure Hunter", "Bard", "Ranger" };
        }

        private static List<string> GetMerchantNPCs()
        {
            return new List<string> { "Silk Trader", "Jeweler" };
        }

        private static List<string> GetUnderworldNPCs()
        {
            return new List<string> { "Thief", "Assassin", "Smuggler" };
        }

        private static List<string> GetSpecialNPCs()
        {
            return new List<string> { "Crazy Hermit", "Ship Captain", "Alchemist", "Monk", "Tinker", "Noble", "Beggar" };
        }
    }

    /// <summary>
    /// Category gump showing specific NPC types
    /// </summary>
    public class LLMNpcCategoryGump : Gump
    {
        private const int GumpWidth = 400;
        private const int GumpHeight = 450;

        private readonly string m_CategoryName;
        private readonly List<string> m_NPCTypes;

        public LLMNpcCategoryGump(string categoryName, List<string> npcTypes) : base(100, 100)
        {
            m_CategoryName = categoryName;
            m_NPCTypes = npcTypes;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Background
            AddBackground(0, 0, GumpWidth, GumpHeight, 9200);
            AddImageTiled(10, 10, GumpWidth - 20, GumpHeight - 20, 2624);
            AddAlphaRegion(10, 10, GumpWidth - 20, GumpHeight - 20);

            // Title
            AddHtml(20, 20, GumpWidth - 40, 40, $"<center><basefont color=#FFFFFF size=6><b>{m_CategoryName}</b></basefont></center>", false, false);

            // NPC List
            int yPos = 80;
            for (int i = 0; i < m_NPCTypes.Count; i++)
            {
                AddButton(40, yPos, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                AddHtml(75, yPos, GumpWidth - 100, 30, $"<basefont color=#FFFFFF size=4>{m_NPCTypes[i]}</basefont>", false, false);
                yPos += 35;
            }

            // Back button
            AddButton(40, GumpHeight - 50, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtml(75, GumpHeight - 50, 100, 30, "<basefont color=#FFFFFF size=4>Back</basefont>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
            {
                // Back to main menu
                from.SendGump(new LLMNpcGump());
                return;
            }

            if (info.ButtonID > 0 && info.ButtonID <= m_NPCTypes.Count)
            {
                string npcType = m_NPCTypes[info.ButtonID - 1];
                from.Target = new LLMNpcTarget(npcType);
                from.SendMessage($"Target where you want to spawn: {npcType}");
            }
        }
    }

    /// <summary>
    /// Targeting system for placing NPCs
    /// </summary>
    public class LLMNpcTarget : Target
    {
        private readonly string m_NPCType;

        public LLMNpcTarget(string npcType) : base(-1, true, TargetFlags.None)
        {
            m_NPCType = npcType;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            IPoint3D p = targeted as IPoint3D;

            if (p == null)
                return;

            Point3D loc = new Point3D(p);
            Map map = from.Map;

            if (map == null || map == Map.Internal)
            {
                from.SendMessage("Invalid location.");
                return;
            }

            // Use new personality system
            NPCPersonalities.PersonalityType personalityType;
            NPCPersonalities.SpeechPattern speechPattern = NPCPersonalities.SpeechPattern.Archaic; // Default to medieval style

            if (m_NPCType == null)
            {
                // Random NPC
                personalityType = NPCPersonalities.GetRandomPersonality();
            }
            else
            {
                // Try to parse the NPC type as a personality, or map it
                if (!Enum.TryParse(m_NPCType, true, out personalityType))
                {
                    // Map old NPC types to new personality system
                    personalityType = MapNPCTypeToPersonality(m_NPCType);
                }
            }

            // Generate a name based on NPC type and personality
            string[] firstNames = { "Aldric", "Beatrix", "Cedric", "Daphne", "Edmund", "Fiona", "Gareth", "Helena", "Ivor", "Jocelyn" };
            string firstName = firstNames[Utility.Random(firstNames.Length)];
            string fullName = $"{firstName} the {m_NPCType ?? personalityType.ToString()}";

            Console.WriteLine($"[LLMNpcGump] Creating NPC: {fullName}");
            LLMNpc npc = new LLMNpc(fullName, personalityType, speechPattern);
            Console.WriteLine($"[LLMNpcGump] NPC created successfully");
            npc.Body = Utility.RandomBool() ? 0x190 : 0x191; // Male or female
            npc.Hue = Utility.RandomSkinHue();

            // The new personality system handles items automatically via the constructor

            npc.MoveToWorld(loc, map);
            from.SendMessage($"Spawned: {fullName}");

            // Reopen gump for convenience
            from.SendGump(new LLMNpcGump());
        }

        /// <summary>
        /// Map old NPC type names to new personality system
        /// </summary>
        private static NPCPersonalities.PersonalityType MapNPCTypeToPersonality(string npcType)
        {
            if (string.IsNullOrEmpty(npcType))
                return NPCPersonalities.PersonalityType.Commoner;

            string lower = npcType.ToLower();

            // Blacksmiths
            if (lower.Contains("blacksmith") || lower.Contains("smith"))
                return NPCPersonalities.PersonalityType.Blacksmith;

            // Merchants/Traders
            if (lower.Contains("trader") || lower.Contains("jeweler") || lower.Contains("silk"))
                return NPCPersonalities.PersonalityType.Merchant;

            // Guards
            if (lower.Contains("guard") || lower.Contains("captain"))
                return NPCPersonalities.PersonalityType.Guard;

            // Mages
            if (lower.Contains("archmage") || lower.Contains("necromancer") ||
                lower.Contains("mage") || lower.Contains("druid") || lower.Contains("mystic"))
                return NPCPersonalities.PersonalityType.Mage;

            // Warriors
            if (lower.Contains("warrior") || lower.Contains("veteran"))
                return NPCPersonalities.PersonalityType.Warrior;

            // Healers
            if (lower.Contains("healer") || lower.Contains("herbalist"))
                return NPCPersonalities.PersonalityType.Healer;

            // Sages
            if (lower.Contains("librarian") || lower.Contains("fortune"))
                return NPCPersonalities.PersonalityType.Sage;

            // Villains
            if (lower.Contains("thief") || lower.Contains("assassin") ||
                lower.Contains("smuggler"))
                return NPCPersonalities.PersonalityType.Villain;

            // Commoners
            if (lower.Contains("tavern") || lower.Contains("fisherman") ||
                lower.Contains("bard") || lower.Contains("ranger") ||
                lower.Contains("treasure hunter"))
                return NPCPersonalities.PersonalityType.Commoner;

            return NPCPersonalities.PersonalityType.Commoner;
        }

        private static Item PersonalitySpawner_CreateItem(string itemName)
        {
            // Reuse the logic from PersonalitySpawner
            try
            {
                switch (itemName)
                {
                    case "PlateChest": return new Server.Items.PlateChest();
                    case "PlateLegs": return new Server.Items.PlateLegs();
                    case "PlateArms": return new Server.Items.PlateArms();
                    case "PlateGloves": return new Server.Items.PlateGloves();
                    case "ChainLegs": return new Server.Items.ChainLegs();
                    case "StuddedChest": return new Server.Items.StuddedChest();
                    case "StuddedLegs": return new Server.Items.StuddedLegs();
                    case "LeatherChest": return new Server.Items.LeatherChest();
                    case "LeatherLegs": return new Server.Items.LeatherLegs();
                    case "LeatherGloves": return new Server.Items.LeatherGloves();
                    case "CloseHelm": return new Server.Items.CloseHelm();
                    case "LeatherCap": return new Server.Items.LeatherCap();
                    case "Robe": return new Server.Items.Robe(Utility.RandomNeutralHue());
                    case "MonkRobe": return new Server.Items.MonkRobe();
                    case "FancyShirt": return new Server.Items.FancyShirt();
                    case "Shirt": return new Server.Items.Shirt();
                    case "Doublet": return new Server.Items.Doublet();
                    case "LongPants": return new Server.Items.LongPants();
                    case "ShortPants": return new Server.Items.ShortPants();
                    case "FancyDress": return new Server.Items.FancyDress();
                    case "GildedDress": return new Server.Items.GildedDress();
                    case "Cloak": return new Server.Items.Cloak();
                    case "Boots": return new Server.Items.Boots();
                    case "Sandals": return new Server.Items.Sandals();
                    case "WizardsHat": return new Server.Items.WizardsHat(Utility.RandomBlueHue());
                    case "FeatheredHat": return new Server.Items.FeatheredHat();
                    case "FloppyHat": return new Server.Items.FloppyHat();
                    case "SkullCap": return new Server.Items.SkullCap();
                    case "TricorneHat": return new Server.Items.TricorneHat();
                    case "Bandana": return new Server.Items.Bandana();
                    case "Circlet": return new Server.Items.Circlet();
                    case "FullApron": return new Server.Items.FullApron();
                    case "HalfApron": return new Server.Items.HalfApron();
                    default: return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Command to open the LLM NPC Gump
    /// </summary>
    public class LLMNpcGumpCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("LLMMenu", AccessLevel.GameMaster, LLMMenu_OnCommand);
            CommandSystem.Register("SpawnLLM", AccessLevel.GameMaster, LLMMenu_OnCommand);
        }

        [Usage("LLMMenu")]
        [Aliases("SpawnLLM")]
        [Description("Opens the LLM NPC spawner menu.")]
        private static void LLMMenu_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new LLMNpcGump());
        }
    }
}
