using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Services.AISidekicks;
using Server.Services.AISidekicks.UI;
using Server.Targeting;
using Server.Custom.VystiaClasses.Quests;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Command to spawn AI sidekicks
    /// </summary>
    public class SpawnSidekickCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnSidekick", AccessLevel.GameMaster, SpawnSidekick_OnCommand);
            CommandSystem.Register("CreateSidekick", AccessLevel.Player, CreateSidekick_OnCommand);
            CommandSystem.Register("Sidekick", AccessLevel.Player, SidekickMenu_OnCommand);
            CommandSystem.Register("Sidekicks", AccessLevel.Player, SidekickMenu_OnCommand); // Alias
            CommandSystem.Register("RemoveSidekick", AccessLevel.Player, RemoveSidekick_OnCommand);
            CommandSystem.Register("ReleaseSidekick", AccessLevel.Player, RemoveSidekick_OnCommand); // Alias
            CommandSystem.Register("SidekickEquipment", AccessLevel.Player, SidekickEquipment_OnCommand);
            CommandSystem.Register("SidekickMount", AccessLevel.Player, SidekickMount_OnCommand);
            CommandSystem.Register("sm", AccessLevel.GameMaster, SpawnMage_OnCommand); // Shortcut for spawning mage
            CommandSystem.Register("SpawnMage", AccessLevel.GameMaster, SpawnMage_OnCommand); // Alias
            CommandSystem.Register("st", AccessLevel.GameMaster, SpawnTamer_OnCommand); // Shortcut for spawning tamer
            CommandSystem.Register("SpawnTamer", AccessLevel.GameMaster, SpawnTamer_OnCommand); // Alias
            CommandSystem.Register("sh", AccessLevel.GameMaster, SpawnHealer_OnCommand); // Shortcut for spawning healer
            CommandSystem.Register("SpawnHealer", AccessLevel.GameMaster, SpawnHealer_OnCommand); // Alias
            CommandSystem.Register("sn", AccessLevel.GameMaster, SpawnNecro_OnCommand); // Shortcut for spawning necromancer
            CommandSystem.Register("SpawnNecro", AccessLevel.GameMaster, SpawnNecro_OnCommand); // Alias
            CommandSystem.Register("ol", AccessLevel.GameMaster, SpawnOgreLord_OnCommand); // Shortcut for spawning ogre lord at cursor
            CommandSystem.Register("OgreLord", AccessLevel.GameMaster, SpawnOgreLord_OnCommand); // Alias
            CommandSystem.Register("pvp", AccessLevel.GameMaster, SpawnPvP_OnCommand); // Spawn mage vs necro for testing
        }

        [Usage("CreateSidekick")]
        [Description("Opens a gump to create a new AI sidekick companion.")]
        private static void CreateSidekick_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || !(from is PlayerMobile))
            {
                from.SendMessage("You must be a player to use this command.");
                return;
            }

            from.SendGump(new UI.AdvancedSidekickCreationGump((PlayerMobile)from));
        }

        [Usage("Sidekick")]
        [Description("Opens the main sidekick management menu with all available options.")]
        private static void SidekickMenu_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || !(from is PlayerMobile))
            {
                from.SendMessage("You must be a player to use this command.");
                return;
            }

            from.SendGump(new UI.SidekickMainMenuGump((PlayerMobile)from));
        }

        [Usage("SpawnMage [player]")]
        [Description("Shortcut command to spawn a Mage sidekick. If player is specified, assigns sidekick to that player. Otherwise assigns to command user.")]
        private static void SpawnMage_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Get target player (if specified)
            PlayerMobile targetPlayer = null;
            if (e.Length >= 1)
            {
                string playerName = e.GetString(0);
                targetPlayer = FindPlayerByName(playerName);
                if (targetPlayer == null)
                {
                    from.SendMessage($"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile pm)
            {
                targetPlayer = pm;
            }

            if (targetPlayer == null)
            {
                from.SendMessage("You must be a player or specify a player name.");
                return;
            }

            // Get archetype definition
            SidekickArchetype archetypeDef = SidekickArchetype.GetArchetype(SidekickArchetypeType.Mage);
            if (archetypeDef == null)
            {
                from.SendMessage("Failed to load Mage archetype definition.");
                return;
            }

            // Spawn the mage sidekick
            AutonomousSidekick sidekick = new AutonomousSidekick(SidekickArchetypeType.Mage, targetPlayer);

            // Get proper ground Z level at spawn location
            Point3D loc = from.Location;
            int z = from.Map.GetAverageZ(loc.X, loc.Y);
            Point3D spawnLoc = new Point3D(loc.X, loc.Y, z);

            sidekick.MoveToWorld(spawnLoc, from.Map);

            // Set custom AI (SidekickAI)
            sidekick.ChangeAIType(AIType.AI_Use_Default);

            from.SendMessage($"Spawned {sidekick.Name} (Mage) for {targetPlayer.Name} at {spawnLoc}");
            targetPlayer.SendMessage($"A Mage sidekick named {sidekick.Name} has been assigned to you!");
            VystiaQuestSystem.OnSidekickRecruited(targetPlayer, SidekickArchetypeType.Mage.ToString());
        }

        [Usage("st [player]")]
        [Description("Shortcut command to spawn a Tamer sidekick. If player is specified, assigns sidekick to that player. Otherwise assigns to command user.")]
        private static void SpawnTamer_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Get target player (if specified)
            PlayerMobile targetPlayer = null;
            if (e.Length >= 1)
            {
                string playerName = e.GetString(0);
                targetPlayer = FindPlayerByName(playerName);
                if (targetPlayer == null)
                {
                    from.SendMessage($"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile pm)
            {
                targetPlayer = pm;
            }

            if (targetPlayer == null)
            {
                from.SendMessage("You must be a player or specify a player name.");
                return;
            }

            // Get archetype definition
            SidekickArchetype archetypeDef = SidekickArchetype.GetArchetype(SidekickArchetypeType.Tamer);
            if (archetypeDef == null)
            {
                from.SendMessage("Failed to load Tamer archetype definition.");
                return;
            }

            // Spawn the tamer sidekick
            AutonomousSidekick sidekick = new AutonomousSidekick(SidekickArchetypeType.Tamer, targetPlayer);

            // Get proper ground Z level at spawn location
            Point3D loc = from.Location;
            int z = from.Map.GetAverageZ(loc.X, loc.Y);
            Point3D spawnLoc = new Point3D(loc.X, loc.Y, z);

            sidekick.MoveToWorld(spawnLoc, from.Map);

            // Set custom AI (SidekickAI)
            sidekick.ChangeAIType(AIType.AI_Use_Default);

            from.SendMessage($"Spawned {sidekick.Name} (Tamer) for {targetPlayer.Name} at {spawnLoc}");
            targetPlayer.SendMessage($"A Tamer sidekick named {sidekick.Name} has been assigned to you!");
            VystiaQuestSystem.OnSidekickRecruited(targetPlayer, SidekickArchetypeType.Tamer.ToString());
        }

        [Usage("sh [player]")]
        [Description("Shortcut command to spawn a Healer sidekick. If player is specified, assigns sidekick to that player. Otherwise assigns to command user.")]
        private static void SpawnHealer_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Get target player (if specified)
            PlayerMobile targetPlayer = null;
            if (e.Length >= 1)
            {
                string playerName = e.GetString(0);
                targetPlayer = FindPlayerByName(playerName);
                if (targetPlayer == null)
                {
                    from.SendMessage($"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile pm)
            {
                targetPlayer = pm;
            }

            if (targetPlayer == null)
            {
                from.SendMessage("You must be a player or specify a player name.");
                return;
            }

            // Get archetype definition
            SidekickArchetype archetypeDef = SidekickArchetype.GetArchetype(SidekickArchetypeType.Healer);
            if (archetypeDef == null)
            {
                from.SendMessage("Failed to load Healer archetype definition.");
                return;
            }

            // Spawn the healer sidekick
            AutonomousSidekick sidekick = new AutonomousSidekick(SidekickArchetypeType.Healer, targetPlayer);

            // Get proper ground Z level at spawn location
            Point3D loc = from.Location;
            int z = from.Map.GetAverageZ(loc.X, loc.Y);
            Point3D spawnLoc = new Point3D(loc.X, loc.Y, z);

            sidekick.MoveToWorld(spawnLoc, from.Map);

            // Set custom AI (SidekickAI)
            sidekick.ChangeAIType(AIType.AI_Use_Default);

            from.SendMessage($"Spawned {sidekick.Name} (Healer) for {targetPlayer.Name} at {spawnLoc}");
            targetPlayer.SendMessage($"A Healer sidekick named {sidekick.Name} has been assigned to you!");
            VystiaQuestSystem.OnSidekickRecruited(targetPlayer, SidekickArchetypeType.Healer.ToString());
        }

        [Usage("sn [player]")]
        [Description("Shortcut command to spawn a Necromancer sidekick. If player is specified, assigns sidekick to that player. Otherwise assigns to command user.")]
        private static void SpawnNecro_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Get target player (if specified)
            PlayerMobile targetPlayer = null;
            if (e.Length >= 1)
            {
                string playerName = e.GetString(0);
                targetPlayer = FindPlayerByName(playerName);
                if (targetPlayer == null)
                {
                    from.SendMessage($"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile pm)
            {
                targetPlayer = pm;
            }

            if (targetPlayer == null)
            {
                from.SendMessage("You must be a player or specify a player name.");
                return;
            }

            // Get archetype definition
            SidekickArchetype archetypeDef = SidekickArchetype.GetArchetype(SidekickArchetypeType.Necromancer);
            if (archetypeDef == null)
            {
                from.SendMessage("Failed to load Necromancer archetype definition.");
                return;
            }

            // Spawn the necromancer sidekick
            AutonomousSidekick sidekick = new AutonomousSidekick(SidekickArchetypeType.Necromancer, targetPlayer);

            // Get proper ground Z level at spawn location
            Point3D loc = from.Location;
            int z = from.Map.GetAverageZ(loc.X, loc.Y);
            Point3D spawnLoc = new Point3D(loc.X, loc.Y, z);

            sidekick.MoveToWorld(spawnLoc, from.Map);

            // Set custom AI (SidekickAI)
            sidekick.ChangeAIType(AIType.AI_Use_Default);

            from.SendMessage($"Spawned {sidekick.Name} (Necromancer) for {targetPlayer.Name} at {spawnLoc}");
            targetPlayer.SendMessage($"A Necromancer sidekick named {sidekick.Name} has been assigned to you!");
            VystiaQuestSystem.OnSidekickRecruited(targetPlayer, SidekickArchetypeType.Necromancer.ToString());
        }

        [Usage("ol")]
        [Description("Spawns an Arctic Ogre Lord at the targeted location.")]
        private static void SpawnOgreLord_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Target the location to spawn an Arctic Ogre Lord.");
            from.Target = new OgreLordSpawnTarget();
        }

        /// <summary>
        /// Target for spawning an Arctic Ogre Lord
        /// </summary>
        private class OgreLordSpawnTarget : Target
        {
            public OgreLordSpawnTarget() : base(-1, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                {
                    from.SendMessage("Invalid target.");
                    return;
                }

                Point3D loc;
                if (p is Item item)
                {
                    loc = item.GetWorldLocation();
                }
                else
                {
                    loc = new Point3D(p);
                }

                // Get proper ground Z level
                int z = from.Map.GetAverageZ(loc.X, loc.Y);
                Point3D spawnLoc = new Point3D(loc.X, loc.Y, z);

                // Spawn the Arctic Ogre Lord
                ArcticOgreLord ogre = new ArcticOgreLord();
                ogre.MoveToWorld(spawnLoc, from.Map);

                from.SendMessage($"Spawned Arctic Ogre Lord at {spawnLoc}");
            }
        }

        [Usage("pvp")]
        [Description("Spawns a Mage 3 tiles west and a Necromancer 3 tiles east of you, and makes them fight each other.")]
        private static void SpawnPvP_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!(from is PlayerMobile player))
            {
                from.SendMessage("You must be a player to use this command.");
                return;
            }

            Point3D loc = from.Location;
            Map map = from.Map;

            // Calculate spawn positions - 3 tiles east and west
            Point3D westLoc = new Point3D(loc.X - 3, loc.Y, map.GetAverageZ(loc.X - 3, loc.Y));
            Point3D eastLoc = new Point3D(loc.X + 3, loc.Y, map.GetAverageZ(loc.X + 3, loc.Y));

            // Spawn Mage to the west
            AutonomousSidekick mage = new AutonomousSidekick(SidekickArchetypeType.Mage, player);
            mage.MoveToWorld(westLoc, map);
            mage.ChangeAIType(AIType.AI_Use_Default);

            // Spawn Necromancer to the east
            AutonomousSidekick necro = new AutonomousSidekick(SidekickArchetypeType.Necromancer, player);
            necro.MoveToWorld(eastLoc, map);
            necro.ChangeAIType(AIType.AI_Use_Default);

            from.SendMessage($"Spawned {mage.Name} (Mage) at west and {necro.Name} (Necromancer) at east.");

            // Make them fight each other after a short delay to let them initialize
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (mage != null && !mage.Deleted && necro != null && !necro.Deleted)
                {
                    // Set them to attack each other
                    mage.ControlTarget = necro;
                    mage.ControlOrder = OrderType.Attack;
                    mage.Combatant = necro;
                    mage.Warmode = true;

                    necro.ControlTarget = mage;
                    necro.ControlOrder = OrderType.Attack;
                    necro.Combatant = mage;
                    necro.Warmode = true;

                    from.SendMessage($"{mage.Name} and {necro.Name} are now fighting!");
                }
            });
        }

        [Usage("SpawnSidekick [archetype] [player]")]
        [Description("Spawns an AI sidekick with specified archetype. If player is specified, assigns sidekick to that player. Otherwise assigns to command user.")]
        private static void SpawnSidekick_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length < 1)
            {
                from.SendMessage("=== Spawn AI Sidekick ===");
                from.SendMessage("Usage: [SpawnSidekick <archetype> [player]");
                from.SendMessage("");
                from.SendMessage("Archetypes: Warrior, Mage, Archer, Healer, Paladin, Ranger, Thief, Necromancer, Battlemage, Cleric, Druid, Tamer");
                from.SendMessage("");
                from.SendMessage("Example: [SpawnSidekick Warrior");
                from.SendMessage("Example: [SpawnSidekick Mage [player name]");
                from.SendMessage("");
                from.SendMessage("If player is not specified, sidekick is assigned to you.");
                return;
            }

            string archetypeStr = e.GetString(0);

            // Parse archetype type
            SidekickArchetypeType archetype;
            if (!Enum.TryParse(archetypeStr, true, out archetype))
            {
                from.SendMessage($"Invalid archetype: {archetypeStr}");
                from.SendMessage("Valid: Warrior, Mage, Archer, Healer, Paladin, Ranger, Thief, Necromancer, Battlemage, Cleric, Druid, Tamer");
                return;
            }

            // Get target player (if specified)
            PlayerMobile targetPlayer = null;
            if (e.Length >= 2)
            {
                string playerName = e.GetString(1);
                targetPlayer = FindPlayerByName(playerName);
                if (targetPlayer == null)
                {
                    from.SendMessage($"Player '{playerName}' not found.");
                    return;
                }
            }
            else if (from is PlayerMobile pm)
            {
                targetPlayer = pm;
            }

            if (targetPlayer == null)
            {
                from.SendMessage("You must be a player or specify a player name.");
                return;
            }

            // Check if player already has a sidekick (if max is 1)
            // This could be enhanced to check config for max sidekicks per player
            // For now, we'll allow multiple sidekicks

            // Get archetype definition
            SidekickArchetype archetypeDef = SidekickArchetype.GetArchetype(archetype);
            if (archetypeDef == null)
            {
                from.SendMessage($"Failed to load archetype definition for {archetype}.");
                return;
            }

            // Spawn the sidekick
            AutonomousSidekick sidekick = new AutonomousSidekick(archetype, targetPlayer);

            // Get proper ground Z level at spawn location
            Point3D loc = from.Location;
            int z = from.Map.GetAverageZ(loc.X, loc.Y);
            Point3D spawnLoc = new Point3D(loc.X, loc.Y, z);

            sidekick.MoveToWorld(spawnLoc, from.Map);

            // Set custom AI (SidekickAI)
            sidekick.ChangeAIType(AIType.AI_Use_Default);
            // Note: The AI will be set to SidekickAI when the creature is created
            // We may need to override ChangeAIType or set it manually
            // For now, this is a placeholder - the AI assignment needs to be handled

            from.SendMessage($"Spawned {sidekick.Name} ({archetype}) for {targetPlayer.Name} at {spawnLoc}");
            targetPlayer.SendMessage($"A {archetype} sidekick named {sidekick.Name} has been assigned to you!");
            VystiaQuestSystem.OnSidekickRecruited(targetPlayer, archetype.ToString());
        }

        /// <summary>
        /// Find a player by name
        /// </summary>
        private static PlayerMobile FindPlayerByName(string name)
        {
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is PlayerMobile pm && pm.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return pm;
                }
            }
            return null;
        }

        [Usage("RemoveSidekick [name]")]
        [Description("Removes/releases your AI sidekick. If name is specified, removes that specific sidekick. Otherwise removes the first sidekick you own.")]
        private static void RemoveSidekick_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || !(from is PlayerMobile))
            {
                from.SendMessage("You must be a player to use this command.");
                return;
            }

            PlayerMobile player = (PlayerMobile)from;
            AutonomousSidekick sidekickToRemove = null;

            // If name specified, find that specific sidekick
            if (e.Length >= 1)
            {
                string sidekickName = e.GetString(0);
                
                foreach (Mobile m in World.Mobiles.Values)
                {
                    if (m is AutonomousSidekick sidekick && 
                        sidekick.Owner == player && 
                        !sidekick.Deleted &&
                        sidekick.Name.Equals(sidekickName, StringComparison.OrdinalIgnoreCase))
                    {
                        sidekickToRemove = sidekick;
                        break;
                    }
                }

                if (sidekickToRemove == null)
                {
                    from.SendMessage($"You don't have a sidekick named '{sidekickName}'.");
                    return;
                }
            }
            else
            {
                // Find first sidekick owned by player
                foreach (Mobile m in World.Mobiles.Values)
                {
                    if (m is AutonomousSidekick sidekick && 
                        sidekick.Owner == player && 
                        !sidekick.Deleted)
                    {
                        sidekickToRemove = sidekick;
                        break;
                    }
                }

                if (sidekickToRemove == null)
                {
                    from.SendMessage("You don't have any sidekicks.");
                    return;
                }
            }

            // Release the sidekick
            if (sidekickToRemove != null)
            {
                string sidekickName = sidekickToRemove.Name;
                sidekickToRemove.ControlOrder = OrderType.Release;
                sidekickToRemove.Say("Farewell, master. It was an honor serving you.");
                
                // Delete immediately (since DeleteOnRelease is true)
                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    if (sidekickToRemove != null && !sidekickToRemove.Deleted)
                    {
                        sidekickToRemove.Delete();
                    }
                });

                from.SendMessage($"You have released {sidekickName}. They will be removed shortly.");
            }
        }

        [Usage("SidekickEquipment [name]")]
        [Description("Opens the equipment management gump for your sidekick.")]
        private static void SidekickEquipment_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || !(from is PlayerMobile))
            {
                from.SendMessage("You must be a player to use this command.");
                return;
            }

            PlayerMobile player = (PlayerMobile)from;
            AutonomousSidekick sidekick = null;

            // If name specified, find that specific sidekick
            if (e.Length >= 1)
            {
                string sidekickName = e.GetString(0);
                
                foreach (Mobile m in World.Mobiles.Values)
                {
                    if (m is AutonomousSidekick sk && 
                        sk.Owner == player && 
                        !sk.Deleted &&
                        sk.Name.Equals(sidekickName, StringComparison.OrdinalIgnoreCase))
                    {
                        sidekick = sk;
                        break;
                    }
                }

                if (sidekick == null)
                {
                    from.SendMessage($"You don't have a sidekick named '{sidekickName}'.");
                    return;
                }
            }
            else
            {
                // Find first sidekick owned by player
                foreach (Mobile m in World.Mobiles.Values)
                {
                    if (m is AutonomousSidekick sk && 
                        sk.Owner == player && 
                        !sk.Deleted)
                    {
                        sidekick = sk;
                        break;
                    }
                }

                if (sidekick == null)
                {
                    from.SendMessage("You don't have any sidekicks.");
                    return;
                }
            }

            if (sidekick != null)
            {
                from.SendGump(new UI.SidekickEquipmentGump(player, sidekick));
            }
        }

        [Usage("SidekickMount [name]")]
        [Description("Opens the mount/pet management gump for your sidekick.")]
        private static void SidekickMount_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || !(from is PlayerMobile))
            {
                from.SendMessage("You must be a player to use this command.");
                return;
            }

            PlayerMobile player = (PlayerMobile)from;
            AutonomousSidekick sidekick = null;

            // If name specified, find that specific sidekick
            if (e.Length >= 1)
            {
                string sidekickName = e.GetString(0);
                
                foreach (Mobile m in World.Mobiles.Values)
                {
                    if (m is AutonomousSidekick sk && 
                        sk.Owner == player && 
                        !sk.Deleted &&
                        sk.Name.Equals(sidekickName, StringComparison.OrdinalIgnoreCase))
                    {
                        sidekick = sk;
                        break;
                    }
                }

                if (sidekick == null)
                {
                    from.SendMessage($"You don't have a sidekick named '{sidekickName}'.");
                    return;
                }
            }
            else
            {
                // Find first sidekick owned by player
                foreach (Mobile m in World.Mobiles.Values)
                {
                    if (m is AutonomousSidekick sk && 
                        sk.Owner == player && 
                        !sk.Deleted)
                    {
                        sidekick = sk;
                        break;
                    }
                }

                if (sidekick == null)
                {
                    from.SendMessage("You don't have any sidekicks.");
                    return;
                }
            }

            if (sidekick != null)
            {
                from.SendGump(new UI.SidekickMountGump(player, sidekick));
            }
        }
    }
}

