using System;
using Server;
using Server.Commands;
using Server.Spells.VystiaSpells.IceMage;
using Server.Targeting;

namespace Server.Commands
{
    public class TestIceSpellCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("frostmeteor", AccessLevel.Player, new CommandEventHandler(FrostMeteor_OnCommand));
            CommandSystem.Register("iceage", AccessLevel.Player, new CommandEventHandler(IceAge_OnCommand));
            CommandSystem.Register("blizzard", AccessLevel.Player, new CommandEventHandler(Blizzard_OnCommand));
            CommandSystem.Register("absolutezero", AccessLevel.Player, new CommandEventHandler(AbsoluteZero_OnCommand));
        }

        [Usage("frostmeteor")]
        [Description("Casts Frost Meteor (Circle 7 Ice Magic spell) - massive AoE cold damage")]
        public static void FrostMeteor_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Create and cast the spell
            FrostMeteorSpell spell = new FrostMeteorSpell(from, null);
            spell.Cast();

            from.SendMessage(0x481, "Casting Frost Meteor...");
        }

        [Usage("iceage")]
        [Description("Casts Ice Age (Circle 8 Ice Magic spell) - ultimate freezing spell")]
        public static void IceAge_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Create and cast the spell
            IceAgeSpell spell = new IceAgeSpell(from, null);
            spell.Cast();

            from.SendMessage(0x481, "Casting Ice Age...");
        }

        [Usage("blizzard")]
        [Description("Casts Blizzard (Circle 6 Ice Magic spell) - AoE cold damage over time")]
        public static void Blizzard_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Create and cast the spell
            BlizzardSpell spell = new BlizzardSpell(from, null);
            spell.Cast();

            from.SendMessage(0x481, "Casting Blizzard...");
        }

        [Usage("absolutezero")]
        [Description("Casts Absolute Zero (Circle 6 Ice Magic spell) - freezes and damages target")]
        public static void AbsoluteZero_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Create and cast the spell
            AbsoluteZeroSpell spell = new AbsoluteZeroSpell(from, null);
            spell.Cast();

            from.SendMessage(0x481, "Casting Absolute Zero...");
        }
    }
}
