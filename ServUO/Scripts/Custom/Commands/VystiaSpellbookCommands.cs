using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Targeting;

namespace Server.Custom.Commands
{
    /// <summary>
    /// GM Commands for spawning Vystia magic spellbooks
    /// Usage: [spellbook <type>  or  [sb <type>
    /// Example: [spellbook ice  or  [sb druid
    /// </summary>
    public class VystiaSpellbookCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpellBook", AccessLevel.GameMaster, new CommandEventHandler(SpellBook_OnCommand));
            CommandSystem.Register("SB", AccessLevel.GameMaster, new CommandEventHandler(SpellBook_OnCommand)); // Short alias
        }

        [Usage("SpellBook <type>")]
        [Aliases("SB")]
        [Description("Spawns a Vystia magic spellbook. Types: ice, nature, hex, elemental, dark, divination, necro, summoning, shamanic, enchanting, illusion")]
        public static void SpellBook_OnCommand(CommandEventArgs e)
        {
            if (e.Length < 1)
            {
                e.Mobile.SendMessage(0x35, "Usage: [spellbook <type>");
                e.Mobile.SendMessage(0x35, "Types: ice, nature, hex, elemental, dark, divination, necro, summoning, shamanic, enchanting, illusion");
                e.Mobile.SendMessage(0x35, "Short alias: [sb <type>");
                return;
            }

            string type = e.GetString(0).ToLower();
            Item spellbook = null;

            switch (type)
            {
                case "ice":
                case "icemage":
                case "frosthold":
                    spellbook = new IceMageSpellbook();
                    break;

                case "nature":
                case "druid":
                case "verdantpeak":
                    spellbook = new DruidSpellbook();
                    break;

                case "hex":
                case "witch":
                case "shadowfen":
                    spellbook = new WitchSpellbook();
                    break;

                case "elemental":
                case "sorcerer":
                case "fire":
                case "emberlands":
                    spellbook = new SorcererSpellbook();
                    break;

                case "dark":
                case "warlock":
                case "shadowvoid":
                    spellbook = new WarlockSpellbook();
                    break;

                case "divination":
                case "oracle":
                case "crystal":
                    spellbook = new OracleSpellbook();
                    break;

                case "necro":
                case "necromancy":
                case "necromancer":
                    spellbook = new VystiaNecromancerSpellbook();
                    break;

                case "summoning":
                case "summoner":
                case "underwater":
                    spellbook = new SummonerSpellbook();
                    break;

                case "shamanic":
                case "shaman":
                case "skyreach":
                    spellbook = new ShamanSpellbook();
                    break;

#if VYSTIA_SONGWEAVING
                case "songweaving":
                case "songbook":
                case "bard":
                    spellbook = new SongweavingSpellbook();
                    break;
#endif

                case "enchanting":
                case "enchanter":
                    spellbook = new EnchanterSpellbook();
                    break;

                case "illusion":
                case "illusionist":
                    spellbook = new IllusionistSpellbook();
                    break;

                default:
                    e.Mobile.SendMessage(0x22, $"Unknown spellbook type: {type}");
                    e.Mobile.SendMessage(0x35, "Valid types: ice, nature, hex, elemental, dark, divination, necro, summoning, shamanic, enchanting, illusion");
                    return;
            }

            if (spellbook != null)
            {
                e.Mobile.AddToBackpack(spellbook);
                e.Mobile.SendMessage(0x55, $"A {spellbook.Name} has been added to your backpack.");
            }
        }
    }
}
