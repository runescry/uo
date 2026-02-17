#if VYSTIA_SONGWEAVING
using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Songweaving;

namespace Server.Commands
{
    public class SongweavingCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("song", AccessLevel.Player, Song_OnCommand);
            CommandSystem.Register("songweave", AccessLevel.Player, Song_OnCommand);
            CommandSystem.Register("songbar", AccessLevel.Player, SongBar_OnCommand);
            CommandSystem.Register("finale", AccessLevel.Player, Finale_OnCommand);
            CommandSystem.Register("finales", AccessLevel.Player, Finales_OnCommand);
            CommandSystem.Register("songmastery", AccessLevel.Player, SongMastery_OnCommand);
            CommandSystem.Register("songmasteryadd", AccessLevel.GameMaster, SongMasteryAdd_OnCommand);
            CommandSystem.Register("crescendo", AccessLevel.GameMaster, Crescendo_OnCommand);
        }

        [Usage("song <name>")]
        [Description("Performs a Songweaving song (Bard only).")]
        private static void Song_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            if (e.Arguments.Length < 1)
            {
                pm.SendMessage("Usage: [song <name> (discordant note, song of courage, lullaby, inspire accuracy, song of healing, dirge of weakness, song of illumination, song of swiftness, song of provocation)");
                return;
            }

            string key = e.Arguments[0];
            ISongweavingSong song = SongweavingRegistry.GetByKey(key);
            if (song == null)
            {
                pm.SendMessage("Unknown song. Try: discordant note, song of courage, lullaby, inspire accuracy, song of healing, dirge of weakness, song of illumination, song of swiftness, song of provocation.");
                return;
            }

            song.Begin(pm);
        }

        [Usage("songbar")]
        [Description("Opens the Songweaving hotbar.")]
        private static void SongBar_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            pm.CloseGump(typeof(Server.Custom.VystiaClasses.Gumps.SongweavingHotbarGump));
            pm.SendGump(new Server.Custom.VystiaClasses.Gumps.SongweavingHotbarGump(pm));
        }

        [Usage("finale <name>")]
        [Description("Performs a Songweaving finale (Bard only).")]
        private static void Finale_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            if (e.Arguments.Length < 1)
            {
                pm.SendMessage("Usage: [finale <name> (sharpnote, interlude, rally, fortissimo, soothing, symphony)");
                return;
            }

            string key = e.Arguments[0];
            var finale = Server.Custom.VystiaClasses.Systems.SongweavingFinaleSystem.GetByKey(key);
            if (finale == null)
            {
                pm.SendMessage("Unknown finale. Try: sharpnote, interlude, rally, fortissimo, soothing, symphony.");
                return;
            }

            Server.Custom.VystiaClasses.Systems.SongweavingFinaleSystem.BeginFinale(pm, finale);
        }

        [Usage("finales")]
        [Description("Opens the Songweaving finales list (Bard only).")]
        private static void Finales_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            pm.CloseGump(typeof(Server.Custom.VystiaClasses.Gumps.SongweavingFinaleGump));
            pm.SendGump(new Server.Custom.VystiaClasses.Gumps.SongweavingFinaleGump(pm));
        }

        [Usage("songmastery [status|<song> <potency|duration>]")]
        [Description("View or spend Song Mastery points (Bard only).")]
        private static void SongMastery_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            if (pm.VystiaClassV2 != Server.Custom.VystiaClasses.PlayerClassTypeV2.Bard)
            {
                pm.SendMessage("Only bards can use Song Mastery.");
                return;
            }

            if (e.Arguments.Length == 0 || string.Equals(e.Arguments[0], "status", StringComparison.OrdinalIgnoreCase))
            {
                Server.Custom.VystiaClasses.Systems.SongweavingMasterySystem.SendStatus(pm);
                pm.SendMessage("Usage: [songmastery <song> <potency|duration>");
                return;
            }

            if (e.Arguments.Length < 2)
            {
                pm.SendMessage("Usage: [songmastery <song> <potency|duration>");
                return;
            }

            string songKey = e.Arguments[0];
            bool potency = string.Equals(e.Arguments[1], "potency", StringComparison.OrdinalIgnoreCase);
            bool duration = string.Equals(e.Arguments[1], "duration", StringComparison.OrdinalIgnoreCase);
            if (!potency && !duration)
            {
                pm.SendMessage("Second argument must be potency or duration.");
                return;
            }

            if (Server.Custom.VystiaClasses.Songweaving.SongweavingRegistry.GetByKey(songKey) == null)
            {
                pm.SendMessage("Unknown song key.");
                return;
            }

            if (Server.Custom.VystiaClasses.Systems.SongweavingMasterySystem.TrySpendPoint(pm, songKey, potency))
            {
                pm.SendMessage($"Song Mastery upgraded: {songKey} ({(potency ? "Potency" : "Duration")}).");
            }
            else
            {
                pm.SendMessage("Unable to spend mastery point (no points or maxed).");
            }
        }

        [Usage("songmasteryadd <points>")]
        [Description("GM: add Song Mastery points to a Bard.")]
        private static void SongMasteryAdd_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            if (e.Arguments.Length < 1 || !int.TryParse(e.Arguments[0], out int points))
            {
                pm.SendMessage("Usage: [songmasteryadd <points>");
                return;
            }

            pm.SongMasteryPoints = Math.Max(0, pm.SongMasteryPoints + points);
            pm.SendMessage($"Song Mastery points now: {pm.SongMasteryPoints}");
        }

        [Usage("crescendo <value|reset>")]
        [Description("GM: set Bard Crescendo to a value (optionally overriding max). Use 'reset' to restore normal max.")]
        private static void Crescendo_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm == null)
                return;

            if (e.Arguments.Length < 1)
            {
                pm.SendMessage("Usage: [crescendo <value|reset>");
                return;
            }

            var resource = Server.Custom.VystiaClasses.Systems.VystiaResourceManager.GetResource<Server.Custom.VystiaClasses.Systems.CrescendoResource>(pm);
            if (resource == null)
            {
                pm.SendMessage("Crescendo resource not found.");
                return;
            }

            if (string.Equals(e.Arguments[0], "reset", StringComparison.OrdinalIgnoreCase))
            {
                resource.ClearMaxOverride();
                resource.SetToMax();
                Server.Custom.VystiaClasses.Systems.VystiaResourceManager.SendBardStatus(pm);
                pm.SendMessage("Crescendo reset to normal max.");
                return;
            }

            if (!int.TryParse(e.Arguments[0], out int value))
            {
                pm.SendMessage("Usage: [crescendo <value|reset>");
                return;
            }

            value = Math.Max(0, value);

            if (value > resource.Maximum)
                resource.SetMaxOverride(value);

            resource.Current = value;
            Server.Custom.VystiaClasses.Systems.VystiaResourceManager.SendBardStatus(pm);
            pm.SendMessage($"Crescendo set to {resource.Current}/{resource.Maximum}.");
        }
    }
}
#endif
