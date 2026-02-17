/*
 * Vystia Religion System Commands
 * GM commands for testing and managing religion
 */

using System;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Custom.VystiaClasses.Commands
{
    public static class ReligionCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("Religion", AccessLevel.Player, new CommandEventHandler(Religion_OnCommand));
            CommandSystem.Register("SetReligion", AccessLevel.GameMaster, new CommandEventHandler(SetReligion_OnCommand));
            CommandSystem.Register("SetPiety", AccessLevel.GameMaster, new CommandEventHandler(SetPiety_OnCommand));
            CommandSystem.Register("AddPiety", AccessLevel.GameMaster, new CommandEventHandler(AddPiety_OnCommand));
            CommandSystem.Register("Pray", AccessLevel.Player, new CommandEventHandler(Pray_OnCommand));
            CommandSystem.Register("Tithe", AccessLevel.Player, new CommandEventHandler(Tithe_OnCommand));
        }

        /// <summary>
        /// [Religion - Shows current religion and piety status
        /// </summary>
        [Usage("Religion")]
        [Description("Shows your current religion and piety status.")]
        private static void Religion_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                var religion = VystiaPiety.GetReligion(pm);
                var piety = VystiaPiety.GetPietyValue(pm);
                var tier = VystiaPiety.GetPietyTier(pm);

                if (religion == VystiaReligion.None)
                {
                    pm.SendMessage("You follow no religion.");
                    pm.SendMessage("Available religions:");
                    pm.SendMessage("  1. {0}", ReligionData.GetDisplayName(VystiaReligion.FrosthelmFaith));
                    pm.SendMessage("  2. {0}", ReligionData.GetDisplayName(VystiaReligion.SuryasSandscript));
                    pm.SendMessage("  3. {0}", ReligionData.GetDisplayName(VystiaReligion.LunarasCovenant));
                    pm.SendMessage("  4. {0}", ReligionData.GetDisplayName(VystiaReligion.CelestisArcanum));
                    pm.SendMessage("  5. {0}", ReligionData.GetDisplayName(VystiaReligion.OceanasCovenant));
                    pm.SendMessage("  6. {0}", ReligionData.GetDisplayName(VystiaReligion.CogsmithCreed));
                    pm.SendMessage("Use [SetReligion <1-6> to convert (GM only for now).");
                    return;
                }

                var info = ReligionData.GetInfo(religion);

                pm.SendMessage("=== Religion Status ===");
                pm.SendMessage("Religion: {0}", ReligionData.GetDisplayName(religion));
                pm.SendMessage("Piety: {0}/1000 ({1})", piety, tier);

                if (info != null)
                {
                    pm.SendMessage("Description: {0}", info.Description);

                    // Show unlocked bonuses
                    pm.SendMessage("--- Bonuses ---");
                    if (tier >= PietyTier.Initiate && info.PassiveBonuses.Length > 0)
                        pm.SendMessage("  Passive: {0}", info.PassiveBonuses[0]);
                    if (tier >= PietyTier.Devoted && info.PassiveBonuses.Length > 1)
                        pm.SendMessage("  Passive: {0}", info.PassiveBonuses[1]);

                    // Show devotion powers
                    pm.SendMessage("--- Devotion Powers ---");
                    if (tier >= PietyTier.Devoted && info.DevotionPowers.Length > 0)
                        pm.SendMessage("  [Unlocked] {0}", info.DevotionPowers[0]);
                    else if (info.DevotionPowers.Length > 0)
                        pm.SendMessage("  [Locked] {0} (200 piety)", info.DevotionPowers[0]);

                    if (tier >= PietyTier.Faithful && info.DevotionPowers.Length > 1)
                        pm.SendMessage("  [Unlocked] {0}", info.DevotionPowers[1]);
                    else if (info.DevotionPowers.Length > 1)
                        pm.SendMessage("  [Locked] {0} (500 piety)", info.DevotionPowers[1]);

                    if (tier >= PietyTier.Exalted && info.DevotionPowers.Length > 2)
                        pm.SendMessage("  [Unlocked] {0}", info.DevotionPowers[2]);
                    else if (info.DevotionPowers.Length > 2)
                        pm.SendMessage("  [Locked] {0} (900 piety)", info.DevotionPowers[2]);
                }
            }
        }

        /// <summary>
        /// [SetReligion <1-6> - Sets religion (GM command)
        /// </summary>
        [Usage("SetReligion <religion id 1-6>")]
        [Description("Converts player to a religion. 0 to remove.")]
        private static void SetReligion_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length < 1)
                {
                    pm.SendMessage("Usage: [SetReligion <1-6> (0 to remove)");
                    pm.SendMessage("  1. {0}", ReligionData.GetDisplayName(VystiaReligion.FrosthelmFaith));
                    pm.SendMessage("  2. {0}", ReligionData.GetDisplayName(VystiaReligion.SuryasSandscript));
                    pm.SendMessage("  3. {0}", ReligionData.GetDisplayName(VystiaReligion.LunarasCovenant));
                    pm.SendMessage("  4. {0}", ReligionData.GetDisplayName(VystiaReligion.CelestisArcanum));
                    pm.SendMessage("  5. {0}", ReligionData.GetDisplayName(VystiaReligion.OceanasCovenant));
                    pm.SendMessage("  6. {0}", ReligionData.GetDisplayName(VystiaReligion.CogsmithCreed));
                    return;
                }

                if (int.TryParse(e.Arguments[0], out int religionId))
                {
                    if (religionId < 0 || religionId > 6)
                    {
                        pm.SendMessage("Invalid religion ID. Use 0-6.");
                        return;
                    }

                    VystiaReligion religion = (VystiaReligion)religionId;
                    VystiaPiety.ConvertToReligion(pm, religion);
                }
                else
                {
                    pm.SendMessage("Invalid number format.");
                }
            }
        }

        /// <summary>
        /// [SetPiety <amount> - Sets exact piety value (GM command)
        /// </summary>
        [Usage("SetPiety <amount>")]
        [Description("Sets exact piety value (0-1000).")]
        private static void SetPiety_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length < 1)
                {
                    pm.SendMessage("Usage: [SetPiety <amount>");
                    return;
                }

                if (int.TryParse(e.Arguments[0], out int amount))
                {
                    amount = Math.Max(0, Math.Min(1000, amount));
                    var data = VystiaPiety.GetPiety(pm);
                    if (data != null)
                    {
                        data.Piety = amount;
                        pm.SendMessage("Piety set to {0}.", amount);
                    }
                }
            }
        }

        /// <summary>
        /// [AddPiety <amount> - Adds/removes piety (GM command)
        /// </summary>
        [Usage("AddPiety <amount>")]
        [Description("Adds (or removes if negative) piety.")]
        private static void AddPiety_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length < 1)
                {
                    pm.SendMessage("Usage: [AddPiety <amount>");
                    return;
                }

                if (int.TryParse(e.Arguments[0], out int amount))
                {
                    VystiaPiety.AddPiety(pm, amount, "GM command");
                }
            }
        }

        /// <summary>
        /// [Pray - Daily prayer for piety
        /// </summary>
        [Usage("Pray")]
        [Description("Pray to your deity for piety (once per day).")]
        private static void Pray_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                PietyActions.Pray(pm);
            }
        }

        /// <summary>
        /// [Tithe <amount> - Donate gold for piety
        /// </summary>
        [Usage("Tithe <gold amount>")]
        [Description("Donate gold as a tithe (+1 piety per 100 gold).")]
        private static void Tithe_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (e.Arguments.Length < 1)
                {
                    pm.SendMessage("Usage: [Tithe <gold amount>");
                    pm.SendMessage("You receive +1 piety per 100 gold donated.");
                    return;
                }

                if (int.TryParse(e.Arguments[0], out int amount))
                {
                    PietyActions.Tithe(pm, amount);
                }
            }
        }
    }
}

