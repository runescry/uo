using System;
using Server;
using Server.Commands;
using Server.Spells;
using Server.Targeting;

namespace Server.Scripts.Commands
{
    public class CastSpellCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("CastSpell", AccessLevel.GameMaster, new CommandEventHandler(CastSpell_OnCommand));
            CommandSystem.Register("cs", AccessLevel.GameMaster, new CommandEventHandler(CastSpell_OnCommand));
        }

        [Usage("CastSpell <spellID>")]
        [Aliases("cs")]
        [Description("Casts a spell by its spell ID. Use [cs <id> to test spell registration.")]
        public static void CastSpell_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage(0x35, "Usage: [castspell <spellID>");
                e.Mobile.SendMessage("Examples:");
                e.Mobile.SendMessage("  [cs 1064  - Cast WeakCurse (Hex Magic Circle 1)");
                e.Mobile.SendMessage("  [cs 1000  - Cast FrostTouch (Ice Magic Circle 1)");
                e.Mobile.SendMessage("  [cs 1032  - Cast ThornDart (Nature Magic Circle 1)");
                e.Mobile.SendMessage("Spell ID Ranges:");
                e.Mobile.SendMessage("  Ice Magic: 1000-1031");
                e.Mobile.SendMessage("  Nature Magic: 1032-1063");
                e.Mobile.SendMessage("  Hex Magic: 1064-1095");
                e.Mobile.SendMessage("  Elemental Magic: 1096-1127");
                e.Mobile.SendMessage("  Dark Magic: 1128-1159");
                e.Mobile.SendMessage("  Divination: 1160-1191");
                e.Mobile.SendMessage("  Necromancy: 1192-1223");
                e.Mobile.SendMessage("  Summoning: 1224-1255");
                e.Mobile.SendMessage("  Shamanic: 1256-1287");
                e.Mobile.SendMessage("  Songweaving: 1384-1398");
                e.Mobile.SendMessage("  Enchanting: 1320-1351");
                e.Mobile.SendMessage("  Illusion: 1352-1383");
                return;
            }

            if (!int.TryParse(e.Arguments[0], out int spellID))
            {
                e.Mobile.SendMessage(0x35, "Invalid spell ID. Must be a number.");
                return;
            }

            // Attempt to create the spell using SpellRegistry
            Spell spell = SpellRegistry.NewSpell(spellID, e.Mobile, null);

            if (spell == null)
            {
                e.Mobile.SendMessage(0x35, $"ERROR: Spell ID {spellID} is NOT registered or failed to create!");
                e.Mobile.SendMessage("Check server console for details.");
                e.Mobile.SendMessage("Verify VystiaSpellInitializer.cs is registering this spell ID.");
                return;
            }

            e.Mobile.SendMessage(0x3B2, $"Spell ID {spellID} found: {spell.GetType().Name}");

            // Cast the spell
            try
            {
                e.Mobile.SendMessage(0x3B2, $"Casting {spell.GetType().Name}...");
                spell.Cast();
            }
            catch (Exception ex)
            {
                e.Mobile.SendMessage(0x35, $"ERROR casting spell: {ex.GetType().Name}");
                e.Mobile.SendMessage($"Message: {ex.Message}");
                Console.WriteLine($"[CastSpell] Exception casting spell {spellID}: {ex}");
            }
        }
    }
}
