using System;
using Server;
using Server.Commands;
using Server.Mobiles;

namespace Server.Commands
{
    public class VystiaSkillCommands
    {
        // Vystia skill IDs: 58-83 (26 skills total)
        private static readonly SkillName[] VystiaSkills = new SkillName[]
        {
            // Magic Skills (58-69)
            SkillName.Cryomancy,       // 58
            SkillName.Demonology,      // 59
            SkillName.NecromancyArts,  // 60
            SkillName.Druidism,        // 61
            SkillName.Elementalism,    // 62
#if VYSTIA_SONGWEAVING
            SkillName.Songweaving,     // 63
#endif
            SkillName.Hexcraft,        // 64
            SkillName.Divination,      // 65
            SkillName.Conjuration,     // 66
            SkillName.SpiritCalling,   // 67
            SkillName.Runeweaving,     // 68
            SkillName.IllusionMagic,   // 69

            // Martial Skills (70-83)
            SkillName.Berserking,      // 70
            SkillName.Subterfuge,      // 71
            SkillName.MartialArts,     // 72
            SkillName.ChivalricArts,   // 73
            SkillName.HolyDevotion,    // 74
            SkillName.Marksmanship,    // 75
            SkillName.CombatMastery,   // 76
            SkillName.Zealotry,        // 77
            SkillName.Manhunting,      // 78
            SkillName.BeastBonding,    // 79
            SkillName.Engineering,     // 80
            SkillName.Transmutation,   // 81
            SkillName.DivineGrace,     // 82
            SkillName.ArcaneStudies    // 83
        };

        public static void Initialize()
        {
            CommandSystem.Register("resetvystiaskills", AccessLevel.GameMaster, new CommandEventHandler(ResetVystiaSkills_OnCommand));
            CommandSystem.Register("rvs", AccessLevel.GameMaster, new CommandEventHandler(ResetVystiaSkills_OnCommand));
            CommandSystem.Register("setvystiaskills", AccessLevel.GameMaster, new CommandEventHandler(SetVystiaSkills_OnCommand));
            CommandSystem.Register("svs", AccessLevel.GameMaster, new CommandEventHandler(SetVystiaSkills_OnCommand));
            CommandSystem.Register("skillcap", AccessLevel.GameMaster, new CommandEventHandler(SkillCap_OnCommand));
            CommandSystem.Register("skillinfo", AccessLevel.GameMaster, new CommandEventHandler(SkillInfo_OnCommand));
        }

        [Usage("skillcap [value]")]
        [Description("Gets or sets your total skill cap. Default UO is 7000 (700.0 points). For Vystia with 84 skills, use 84000.")]
        public static void SkillCap_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length < 1)
            {
                from.SendMessage(0x3B2, "Current skill cap: {0} ({1:F1} points)", from.SkillsCap, from.SkillsCap / 10.0);
                from.SendMessage(0x3B2, "Current skill total: {0:F1} points", from.SkillsTotal / 10.0);
                from.SendMessage(0x3B2, "Usage: [skillcap <value> (e.g., [skillcap 84000 for 8400 points)");
                return;
            }

            int value;
            if (!int.TryParse(e.Arguments[0], out value))
            {
                from.SendMessage(0x22, "Invalid value. Use a number like 7000, 72000, or 84000.");
                return;
            }

            from.SkillsCap = value;
            from.SendMessage(0x35, "Skill cap set to {0} ({1:F1} points)", value, value / 10.0);
        }

        [Usage("skillinfo")]
        [Description("Shows your skill totals and cap information.")]
        public static void SkillInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            double total = 0;
            double vystiaTotal = 0;
            double uoTotal = 0;

            for (int i = 0; i < from.Skills.Length; i++)
            {
                Skill s = from.Skills[i];
                if (s != null)
                {
                    total += s.Base;
                    if (i >= 58 && i <= 83)
                        vystiaTotal += s.Base;
                    else
                        uoTotal += s.Base;
                }
            }

            from.SendMessage(0x3B2, "=== Skill Info ===");
            from.SendMessage(0x3B2, "Total Skills: {0:F1} / {1:F1} cap", total, from.SkillsCap / 10.0);
            from.SendMessage(0x3B2, "Standard UO Skills (0-57): {0:F1}", uoTotal);
            from.SendMessage(0x3B2, "Vystia Skills (58-83): {0:F1}", vystiaTotal);
            from.SendMessage(0x3B2, "Room for gains: {0:F1}", (from.SkillsCap / 10.0) - total);
        }

        [Usage("resetvystiaskills")]
        [Description("Resets all 26 Vystia skills to 0.0 for yourself. Alias: [rvs")]
        public static void ResetVystiaSkills_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            int count = 0;
            foreach (SkillName skillName in VystiaSkills)
            {
                Skill skill = from.Skills[skillName];
                if (skill != null)
                {
                    skill.Base = 0.0;
                    count++;
                }
            }

            from.SendMessage(0x35, "Reset {0} Vystia skills to 0.0", count);
        }

        [Usage("setvystiaskills <value>")]
        [Description("Sets all 26 Vystia skills to specified value. Alias: [svs <value>")]
        public static void SetVystiaSkills_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length < 1)
            {
                from.SendMessage(0x22, "Usage: [setvystiaskills <value> (e.g., [svs 100)");
                return;
            }

            double value;
            if (!double.TryParse(e.Arguments[0], out value))
            {
                from.SendMessage(0x22, "Invalid value. Use a number like 0, 50, or 100.");
                return;
            }

            value = Math.Max(0.0, Math.Min(100.0, value)); // Clamp to 0-100 (Vystia: 100 is GM, no power scrolls)

            int count = 0;
            foreach (SkillName skillName in VystiaSkills)
            {
                Skill skill = from.Skills[skillName];
                if (skill != null)
                {
                    skill.Base = value;
                    count++;
                }
            }

            from.SendMessage(0x35, "Set {0} Vystia skills to {1:F1}", count, value);
        }
    }
}
