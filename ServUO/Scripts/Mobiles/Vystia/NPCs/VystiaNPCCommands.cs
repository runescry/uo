using Server;
using Server.Commands;

namespace Server.Mobiles
{
    /// <summary>
    /// GM Commands for spawning Vystia NPCs
    /// Generated: 13 NPCs
    /// </summary>
    public class VystiaNPCCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnVystiaLeader", AccessLevel.GameMaster, SpawnLeader_OnCommand);
            CommandSystem.Register("SpawnVystiaCreature", AccessLevel.GameMaster, SpawnCreature_OnCommand);
            CommandSystem.Register("SpawnVystiaQuestGiver", AccessLevel.GameMaster, SpawnQuestGiver_OnCommand);
        }

        [Usage("SpawnVystiaLeader <name>")]
        [Description("Spawns a Vystia faction leader at cursor location")]
        private static void SpawnLeader_OnCommand(CommandEventArgs e)
        {
            if (e.Length < 1)
            {
                e.Mobile.SendMessage("Usage: [SpawnVystiaLeader <name>");
                e.Mobile.SendMessage("Available: Garrick, Bjorn, Seraphina, Azir, Pyrus, Maris, Tideseeker, Eldur, Mara, Lumis, Nocturna, Orin, Faelar, Sylas, Iceshadow, Amaryllis, Emberon, Tarik, Kael, Frostbeard");
                return;
            }

            string name = e.GetString(0).ToLower();
            Mobile npc = null;

            switch (name)
            {
                case "garrick":
                    npc = new EmperorGarrickSteelarm();
                    break;
                case "bjorn":
                    npc = new ChieftainBjornFrostbeard();
                    break;
                case "seraphina":
                    npc = new ElderSeraphinaLeafwhisper();
                    break;
                case "azir":
                    npc = new SultanAziralRashid();
                    break;
                case "pyrus":
                    npc = new ArchmagePyrusAshborn();
                    break;
                case "maris":
                    npc = new AdmiralMarisHawkseye();
                    break;
                case "tideseeker":
                    npc = new LordMarinerTideseeker();
                    break;
                case "eldur":
                    npc = new HighGuardianEldurMountainborn();
                    break;
                case "mara":
                    npc = new ChiefMaraWildsong();
                    break;
                case "lumis":
                    npc = new ArchmageLumis();
                    break;
                case "nocturna":
                    npc = new SorceressNocturna();
                    break;
                case "orin":
                    npc = new SageOrin();
                    break;
                case "faelar":
                    npc = new DruidLordFaelar();
                    break;
                case "sylas":
                    npc = new GuardianSylas();
                    break;
                case "iceshadow":
                    npc = new QueenIceshadow();
                    break;
                case "amaryllis":
                    npc = new QueenAmaryllis();
                    break;
                case "emberon":
                    npc = new WarlordEmberonFlamefist();
                    break;
                case "tarik":
                    npc = new SheikhTarik();
                    break;
                case "kael":
                    npc = new WarlordKael();
                    break;
                case "frostbeard":
                case "king":
                    npc = new KingFrostbeard();
                    break;
                default:
                    e.Mobile.SendMessage("Unknown leader: {0}", name);
                    return;
            }

            if (npc != null)
            {
                npc.MoveToWorld(e.Mobile.Location, e.Mobile.Map);
                e.Mobile.SendMessage("Spawned {0}", npc.Name);
            }
        }

        [Usage("SpawnVystiaCreature <name>")]
        [Description("Spawns a talking creature at cursor location")]
        private static void SpawnCreature_OnCommand(CommandEventArgs e)
        {
            if (e.Length < 1)
            {
                e.Mobile.SendMessage("Usage: [SpawnVystiaCreature <name>");
                e.Mobile.SendMessage("Available: Frosthelm, Oakbark, Sphinx");
                return;
            }

            string name = e.GetString(0).ToLower();
            BaseCreature creature = null;

            switch (name)
            {
                case "frosthelm":
                    creature = new FrosthelmEternalWinter();
                    break;
                case "oakbark":
                    creature = new ElderOakbark();
                    break;
                case "sphinx":
                case "sphynx":
                    creature = new SphynxOfEmberlands();
                    break;
                default:
                    e.Mobile.SendMessage("Unknown creature: {0}", name);
                    return;
            }

            if (creature != null)
            {
                creature.MoveToWorld(e.Mobile.Location, e.Mobile.Map);
                e.Mobile.SendMessage("Spawned {0}", creature.Name);
            }
        }

        [Usage("SpawnVystiaQuestGiver <name>")]
        [Description("Spawns a Vystia quest giver at cursor location")]
        private static void SpawnQuestGiver_OnCommand(CommandEventArgs e)
        {
            if (e.Length < 1)
            {
                e.Mobile.SendMessage("Usage: [SpawnVystiaQuestGiver <name>");
                e.Mobile.SendMessage("Available: Chronicler, QuartermasterGrimwald, SageTheron");
                return;
            }

            string name = e.GetString(0).ToLower();
            Mobile npc = null;

            switch (name)
            {
                case "chronicler":
                    npc = new Chronicler();
                    break;
                case "grimwald":
                case "quartermaster":
                    npc = new QuartermasterGrimwald();
                    break;
                case "theron":
                case "sage":
                    npc = new SageTheron();
                    break;
                default:
                    e.Mobile.SendMessage("Unknown quest giver: {0}", name);
                    return;
            }

            if (npc != null)
            {
                npc.MoveToWorld(e.Mobile.Location, e.Mobile.Map);
                e.Mobile.SendMessage("Spawned {0}", npc.Name);
            }
        }
    }
}
