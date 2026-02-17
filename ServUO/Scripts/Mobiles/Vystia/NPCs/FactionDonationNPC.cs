// Vystia Faction Donation NPC
// NPCs that accept gold donations for faction reputation

using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Mobiles.Vystia
{
    #region Base Donation NPC

    /// <summary>
    /// Base class for faction donation NPCs
    /// </summary>
    public abstract class BaseFactionDonationNPC : BaseVendor
    {
        public abstract VystiaFaction Faction { get; }
        public abstract string FactionTitle { get; }

        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        // Daily donation limit per player
        private static readonly Dictionary<PlayerMobile, Dictionary<VystiaFaction, DonationRecord>> s_DonationRecords
            = new Dictionary<PlayerMobile, Dictionary<VystiaFaction, DonationRecord>>();

        public const int DailyDonationLimit = 10000; // 10,000 gold max per day
        public const int MinDonation = 100; // Minimum 100 gold
        public const int RepPerGold = 50; // 50 rep per 1,000 gold (1 rep per 20 gold)

        public BaseFactionDonationNPC(string name) : base(null)
        {
            Name = name;
            Title = FactionTitle;

            var info = FactionData.GetInfo(Faction);
            if (info != null)
            {
                Hue = info.Hue;
            }
        }

        public BaseFactionDonationNPC(Serial serial) : base(serial) { }

        public override void InitSBInfo() { }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm && InRange(pm, 4))
            {
                pm.SendGump(new FactionDonationGump(pm, this));
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            if (e.Mobile is PlayerMobile pm && InRange(pm, 4))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("donate") || speech.Contains("contribution") || speech.Contains("give"))
                {
                    pm.SendGump(new FactionDonationGump(pm, this));
                }
                else if (speech.Contains("reputation") || speech.Contains("standing"))
                {
                    var tier = VystiaReputation.GetFactionTier(pm, Faction);
                    var rep = VystiaReputation.GetFactionReputation(pm, Faction);
                    var info = FactionData.GetInfo(Faction);

                    Say("Your standing with {0} is {1} ({2} reputation).", info?.Name, tier, rep);
                }
            }
        }

        /// <summary>
        /// Process a donation from a player
        /// </summary>
        public bool ProcessDonation(PlayerMobile pm, int amount)
        {
            if (amount < MinDonation)
            {
                pm.SendMessage(0x22, "The minimum donation is {0} gold.", MinDonation);
                return false;
            }

            // Check daily limit
            int donatedToday = GetDonatedToday(pm);
            int remainingLimit = DailyDonationLimit - donatedToday;

            if (remainingLimit <= 0)
            {
                pm.SendMessage(0x22, "You have reached your daily donation limit. Return tomorrow.");
                return false;
            }

            // Cap donation at remaining limit
            if (amount > remainingLimit)
            {
                amount = remainingLimit;
                pm.SendMessage(0x35, "Your donation has been capped at {0} gold (daily limit).", amount);
            }

            // Check if player has the gold
            Container pack = pm.Backpack;
            if (pack == null || !pack.ConsumeTotal(typeof(Gold), amount))
            {
                // Try bank
                if (!Banker.Withdraw(pm, amount))
                {
                    pm.SendMessage(0x22, "You do not have enough gold.");
                    return false;
                }
            }

            // Calculate reputation gain
            int repGain = (amount * RepPerGold) / 1000; // 50 rep per 1000 gold
            if (repGain < 1) repGain = 1;

            // Award reputation
            VystiaReputation.AddReputation(pm, Faction, repGain, "donation");

            // Record donation
            RecordDonation(pm, amount);

            // Visual effects
            pm.PlaySound(0x2E6); // Coin sound
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, FactionData.GetInfo(Faction)?.Hue ?? 0, 0, 5039, 0);

            var info = FactionData.GetInfo(Faction);
            Say("Thank you for your generous donation to {0}!", info?.Name);

            return true;
        }

        /// <summary>
        /// Get amount donated today by player
        /// </summary>
        public int GetDonatedToday(PlayerMobile pm)
        {
            if (!s_DonationRecords.TryGetValue(pm, out var factionRecords))
                return 0;

            if (!factionRecords.TryGetValue(Faction, out var record))
                return 0;

            // Check if record is from today
            if (record.Date.Date != DateTime.UtcNow.Date)
                return 0;

            return record.Amount;
        }

        /// <summary>
        /// Record a donation
        /// </summary>
        private void RecordDonation(PlayerMobile pm, int amount)
        {
            if (!s_DonationRecords.TryGetValue(pm, out var factionRecords))
            {
                factionRecords = new Dictionary<VystiaFaction, DonationRecord>();
                s_DonationRecords[pm] = factionRecords;
            }

            if (!factionRecords.TryGetValue(Faction, out var record) || record.Date.Date != DateTime.UtcNow.Date)
            {
                record = new DonationRecord { Date = DateTime.UtcNow, Amount = 0 };
                factionRecords[Faction] = record;
            }

            record.Amount += amount;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }

        private class DonationRecord
        {
            public DateTime Date;
            public int Amount;
        }
    }

    #endregion

    #region Faction-Specific Donation NPCs

    public class FrostguardDonationNPC : BaseFactionDonationNPC
    {
        public override VystiaFaction Faction => VystiaFaction.Frostguard;
        public override string FactionTitle => "Frostguard Quartermaster";

        [Constructable]
        public FrostguardDonationNPC() : base("Quartermaster Frost") { }
        public FrostguardDonationNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class FlameLegionDonationNPC : BaseFactionDonationNPC
    {
        public override VystiaFaction Faction => VystiaFaction.FlameLegion;
        public override string FactionTitle => "Flame Legion Quartermaster";

        [Constructable]
        public FlameLegionDonationNPC() : base("Quartermaster Ember") { }
        public FlameLegionDonationNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class GreenwardDonationNPC : BaseFactionDonationNPC
    {
        public override VystiaFaction Faction => VystiaFaction.Greenward;
        public override string FactionTitle => "Greenward Steward";

        [Constructable]
        public GreenwardDonationNPC() : base("Steward Oakhart") { }
        public GreenwardDonationNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ArcaneConclaveDonationNPC : BaseFactionDonationNPC
    {
        public override VystiaFaction Faction => VystiaFaction.ArcaneConclave;
        public override string FactionTitle => "Arcane Conclave Treasurer";

        [Constructable]
        public ArcaneConclaveDonationNPC() : base("Treasurer Crystalis") { }
        public ArcaneConclaveDonationNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TechnoguildDonationNPC : BaseFactionDonationNPC
    {
        public override VystiaFaction Faction => VystiaFaction.Technoguild;
        public override string FactionTitle => "Technoguild Bursar";

        [Constructable]
        public TechnoguildDonationNPC() : base("Bursar Cogsworth") { }
        public TechnoguildDonationNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class SandwalkersDonationNPC : BaseFactionDonationNPC
    {
        public override VystiaFaction Faction => VystiaFaction.Sandwalkers;
        public override string FactionTitle => "Sandwalkers Tribute Keeper";

        [Constructable]
        public SandwalkersDonationNPC() : base("Tribute Keeper Zephyr") { }
        public SandwalkersDonationNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class VoidbornDonationNPC : BaseFactionDonationNPC
    {
        public override VystiaFaction Faction => VystiaFaction.Voidborn;
        public override string FactionTitle => "Voidborn Collector";

        [Constructable]
        public VoidbornDonationNPC() : base("Collector Umbra") { }
        public VoidbornDonationNPC(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Donation Gump

    public class FactionDonationGump : Gump
    {
        private readonly PlayerMobile m_Player;
        private readonly BaseFactionDonationNPC m_NPC;

        private static readonly int[] DonationAmounts = { 100, 500, 1000, 2500, 5000, 10000 };

        public FactionDonationGump(PlayerMobile pm, BaseFactionDonationNPC npc) : base(50, 50)
        {
            m_Player = pm;
            m_NPC = npc;

            var faction = npc.Faction;
            var info = FactionData.GetInfo(faction);
            var tier = VystiaReputation.GetFactionTier(pm, faction);
            var rep = VystiaReputation.GetFactionReputation(pm, faction);
            int donatedToday = npc.GetDonatedToday(pm);
            int remaining = BaseFactionDonationNPC.DailyDonationLimit - donatedToday;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            // Background
            AddBackground(0, 0, 350, 400, 9270);

            // Title
            AddHtml(0, 15, 350, 20, Center(Color(info?.Name ?? "Faction", "#FFD700")), false, false);
            AddHtml(0, 35, 350, 20, Center("Donation Center"), false, false);

            // Divider
            AddImageTiled(25, 60, 300, 2, 2624);

            // Current standing
            AddHtml(25, 70, 300, 20, $"Your Standing: {Color(tier.ToString(), GetTierColor(tier))}", false, false);
            AddHtml(25, 90, 300, 20, $"Current Reputation: {rep:N0}", false, false);
            AddHtml(25, 110, 300, 20, $"Donated Today: {donatedToday:N0} / {BaseFactionDonationNPC.DailyDonationLimit:N0}", false, false);
            AddHtml(25, 130, 300, 20, $"Remaining Limit: {Color(remaining.ToString("N0"), remaining > 0 ? "#00FF00" : "#FF0000")}", false, false);

            // Divider
            AddImageTiled(25, 155, 300, 2, 2624);

            // Donation info
            AddHtml(25, 165, 300, 40, $"Donate gold to earn reputation with {info?.Name}.", false, false);
            AddHtml(25, 190, 300, 20, $"Rate: {BaseFactionDonationNPC.RepPerGold} reputation per 1,000 gold", false, false);

            // Divider
            AddImageTiled(25, 215, 300, 2, 2624);

            // Donation buttons
            AddHtml(25, 225, 300, 20, "Select Donation Amount:", false, false);

            int y = 250;
            for (int i = 0; i < DonationAmounts.Length; i++)
            {
                int amount = DonationAmounts[i];
                int repGain = (amount * BaseFactionDonationNPC.RepPerGold) / 1000;

                bool enabled = remaining >= amount;

                if (enabled)
                    AddButton(25, y, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                else
                    AddImage(25, y, 4005, 1000); // Greyed out

                AddHtml(60, y + 2, 280, 20, $"{amount:N0} gold ({Color($"+{repGain} rep", enabled ? "#00FF00" : "#888888")})", false, false);

                y += 25;
            }

            // Custom amount
            AddButton(25, y + 10, 4005, 4007, 100, GumpButtonType.Reply, 0);
            AddHtml(60, y + 12, 100, 20, "Custom Amount:", false, false);
            AddBackground(170, y + 8, 80, 24, 9350);
            AddTextEntry(175, y + 10, 70, 20, 0, 0, "1000");

            // Close button
            AddButton(140, 360, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(175, 362, 60, 20, "Close", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_NPC == null || m_NPC.Deleted)
                return;

            if (!m_Player.InRange(m_NPC, 4))
            {
                m_Player.SendMessage("You are too far away.");
                return;
            }

            int buttonID = info.ButtonID;

            if (buttonID == 0)
                return; // Closed

            int amount = 0;

            if (buttonID >= 1 && buttonID <= DonationAmounts.Length)
            {
                amount = DonationAmounts[buttonID - 1];
            }
            else if (buttonID == 100)
            {
                // Custom amount
                TextRelay entry = info.GetTextEntry(0);
                if (entry != null && int.TryParse(entry.Text, out int customAmount))
                {
                    amount = customAmount;
                }
            }

            if (amount > 0)
            {
                m_NPC.ProcessDonation(m_Player, amount);
            }

            // Re-open gump
            m_Player.SendGump(new FactionDonationGump(m_Player, m_NPC));
        }

        private static string Center(string text) => $"<CENTER>{text}</CENTER>";
        private static string Color(string text, string color) => $"<BASEFONT COLOR={color}>{text}</BASEFONT>";

        private static string GetTierColor(ReputationTier tier)
        {
            switch (tier)
            {
                case ReputationTier.Exalted: return "#FFD700";
                case ReputationTier.Honored: return "#00FF00";
                case ReputationTier.Allied: return "#4169E1";
                case ReputationTier.Friendly: return "#87CEEB";
                case ReputationTier.Neutral: return "#808080";
                case ReputationTier.Unfriendly: return "#FFA500";
                case ReputationTier.Hostile: return "#FF0000";
                default: return "#FFFFFF";
            }
        }
    }

    #endregion

    #region Spawn Commands

    public static class FactionDonationNPCCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnDonationNPC", AccessLevel.GameMaster, SpawnDonationNPC_OnCommand);
            CommandSystem.Register("SDN", AccessLevel.GameMaster, SpawnDonationNPC_OnCommand);
        }

        [Usage("SpawnDonationNPC <faction>")]
        [Description("Spawns a faction donation NPC. Factions: Frostguard, FlameLegion, Greenward, ArcaneConclave, Technoguild, Sandwalkers, Voidborn, All")]
        private static void SpawnDonationNPC_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            string arg = e.ArgString.Trim().ToLower();

            if (string.IsNullOrEmpty(arg))
            {
                from.SendMessage("Usage: [SpawnDonationNPC <faction>");
                from.SendMessage("Factions: Frostguard, FlameLegion, Greenward, ArcaneConclave, Technoguild, Sandwalkers, Voidborn, All");
                return;
            }

            List<BaseFactionDonationNPC> npcs = new List<BaseFactionDonationNPC>();

            if (arg == "all")
            {
                npcs.Add(new FrostguardDonationNPC());
                npcs.Add(new FlameLegionDonationNPC());
                npcs.Add(new GreenwardDonationNPC());
                npcs.Add(new ArcaneConclaveDonationNPC());
                npcs.Add(new TechnoguildDonationNPC());
                npcs.Add(new SandwalkersDonationNPC());
                npcs.Add(new VoidbornDonationNPC());
            }
            else
            {
                switch (arg)
                {
                    case "frostguard": npcs.Add(new FrostguardDonationNPC()); break;
                    case "flamelegion": npcs.Add(new FlameLegionDonationNPC()); break;
                    case "greenward": npcs.Add(new GreenwardDonationNPC()); break;
                    case "arcaneconclave": npcs.Add(new ArcaneConclaveDonationNPC()); break;
                    case "technoguild": npcs.Add(new TechnoguildDonationNPC()); break;
                    case "sandwalkers": npcs.Add(new SandwalkersDonationNPC()); break;
                    case "voidborn": npcs.Add(new VoidbornDonationNPC()); break;
                    default:
                        from.SendMessage("Unknown faction. Use: Frostguard, FlameLegion, Greenward, ArcaneConclave, Technoguild, Sandwalkers, Voidborn, All");
                        return;
                }
            }

            int x = from.X;
            int y = from.Y;

            foreach (var npc in npcs)
            {
                npc.MoveToWorld(new Point3D(x, y, from.Z), from.Map);
                x += 2;
            }

            from.SendMessage("{0} donation NPC(s) spawned.", npcs.Count);
        }
    }

    #endregion
}
