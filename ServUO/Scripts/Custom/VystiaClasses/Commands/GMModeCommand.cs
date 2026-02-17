using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Custom.VystiaClasses.Commands
{
    /// <summary>
    /// GM utility: instantly max stats/skills for testing.
    /// </summary>
    public static class GMModeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("GMMode", AccessLevel.GameMaster, OnGMMode);
        }

        [Usage("GMMode")]
        [Description("Sets target player to 120.0 in all skills and 100000 Str/Dex/Int with full HP/Stam/Mana (testing utility). Target a player or yourself.")]
        private static void OnGMMode(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target a player to apply GMMode, or target yourself.");
            e.Mobile.Target = new GMModeTarget();
        }

        private static void ApplyGMMode(PlayerMobile pm)
        {
            if (pm == null)
                return;

            try
            {
                // Stats
                pm.RawStr = 100000;
                pm.RawDex = 100000;
                pm.RawInt = 100000;

                // Set stat caps to allow the high values
                pm.StrCap = 100000;
                pm.DexCap = 100000;
                pm.IntCap = 100000;

                // Update totals to recalculate HP/Stam/Mana based on new stats
                pm.UpdateTotals();

                // Set to full after totals are updated
                pm.Hits = pm.HitsMax;
                pm.Stam = pm.StamMax;
                pm.Mana = pm.ManaMax;

                // Skills: raise per-skill caps and total cap so Base can actually stick.
                int skillCount = pm.Skills.Length;
                pm.Skills.Cap = 1200 * skillCount; // fixed-point (x10): 120.0 per skill

                for (int i = 0; i < skillCount; i++)
                {
                    var sk = pm.Skills[i];
                    if (sk == null)
                        continue;

                    sk.Cap = 120.0;
                    sk.Base = 120.0;
                }

                pm.SendMessage("GMMode applied: 120.0 all skills, 100000 Str/Dex/Int, full HP/Stam/Mana.");
            }
            catch (Exception ex)
            {
                pm.SendMessage($"GMMode failed: {ex.Message}");
            }
        }

        private class GMModeTarget : Target
        {
            public GMModeTarget() : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile pm)
                {
                    ApplyGMMode(pm);
                    from.SendMessage($"GMMode applied to {pm.Name}.");
                }
                else if (targeted is Mobile m)
                {
                    from.SendMessage("GMMode can only be applied to player characters.");
                }
                else
                {
                    from.SendMessage("Invalid target. Target a player character.");
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.SendMessage("GMMode targeting cancelled.");
            }
        }
    }
}


