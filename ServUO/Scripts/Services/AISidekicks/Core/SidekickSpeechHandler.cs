using System;
using Server;
using Server.Mobiles;
using Server.Services.AISidekicks.UI;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Handles speech/command parsing for sidekicks
    /// Processes commands BEFORE LLM conversation
    /// </summary>
    public static class SidekickSpeechHandler
    {
        /// <summary>
        /// Check if speech is a pet command
        /// </summary>
        public static bool IsPetCommand(string speech, string sidekickName)
        {
            Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand called - Speech: '{speech}', SidekickName: '{sidekickName}'");
            
            if (string.IsNullOrWhiteSpace(speech))
            {
                Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: Speech is null/empty - returning false");
                return false;
            }

            string lowerSpeech = speech.ToLower().Trim();
            Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: Lower speech: '{lowerSpeech}'");

            // Common pet command keywords
            string[] petCommands = {
                "follow", "stay", "guard", "attack", "kill", "come", "all follow",
                "all stay", "all guard", "all attack", "all kill", "all come",
                "stop", "heel", "back", "drop", "get", "transfer", "release",
                "friend", "unfriend", "patrol", "follow me", "stay here",
                "guard me", "all guard me", "all follow me", "mount", "mount up"
            };

            // Check if speech starts with sidekick name followed by command
            if (!string.IsNullOrEmpty(sidekickName))
            {
                string nameLower = sidekickName.ToLower();
                Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: Checking name prefix - NameLower: '{nameLower}'");
                if (lowerSpeech.StartsWith(nameLower))
                {
                    string remainder = lowerSpeech.Substring(nameLower.Length).Trim();
                    Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: Name match found, remainder: '{remainder}'");
                    if (string.IsNullOrWhiteSpace(remainder))
                    {
                        Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: Just name, no command - returning false");
                        return false; // Just saying the name isn't a command
                    }

                    foreach (string cmd in petCommands)
                    {
                        if (remainder == cmd || remainder.StartsWith(cmd + " ") || remainder.StartsWith(cmd + ","))
                        {
                            Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: Command match found: '{cmd}' - returning true");
                            return true;
                        }
                    }
                    Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: No command match in remainder");
                }
            }

            // Check if speech is just a command (when sidekick is control target)
            foreach (string cmd in petCommands)
            {
                if (lowerSpeech == cmd || lowerSpeech.StartsWith(cmd + " ") || lowerSpeech.StartsWith(cmd + ","))
                {
                    Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: Direct command match: '{cmd}' - returning true");
                    return true;
                }
            }

            Console.WriteLine($"[SidekickSpeechHandler] IsPetCommand: No matches - returning false");
            return false;
        }

        /// <summary>
        /// Check if speech is a bracket command (e.g., "[command")
        /// </summary>
        public static bool IsBracketCommand(string speech)
        {
            if (string.IsNullOrWhiteSpace(speech))
                return false;

            return speech.Trim().StartsWith("[");
        }

        /// <summary>
        /// Process a pet command for a sidekick
        /// </summary>
        public static bool ProcessCommand(BaseSidekick sidekick, Mobile from, string speech)
        {
            Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand called - Sidekick: {sidekick?.Name}, From: {from?.Name}, Speech: '{speech}'");
            
            if (sidekick == null || from == null || string.IsNullOrWhiteSpace(speech))
            {
                Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Invalid parameters - returning false");
                return false;
            }

            // Only owner can give commands
            Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: ControlMaster check - Sidekick.ControlMaster: {sidekick.ControlMaster?.Name}, From: {from.Name}, Match: {sidekick.ControlMaster == from}");
            if (sidekick.ControlMaster != from)
            {
                Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Not owner - returning false");
                return false;
            }

            string lowerSpeech = speech.ToLower().Trim();
            string sidekickName = sidekick.Name?.ToLower() ?? "";
            Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Lower speech: '{lowerSpeech}', Sidekick name: '{sidekickName}'");

            // Remove sidekick name if present
            if (!string.IsNullOrEmpty(sidekickName) && lowerSpeech.StartsWith(sidekickName))
            {
                lowerSpeech = lowerSpeech.Substring(sidekickName.Length).Trim();
                Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Removed name, new lowerSpeech: '{lowerSpeech}'");
            }
            // Handle "all" commands - strip "all " prefix
            else if (lowerSpeech.StartsWith("all "))
            {
                lowerSpeech = lowerSpeech.Substring(4).Trim();
                Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Removed 'all' prefix, new lowerSpeech: '{lowerSpeech}'");
            }

            // Process commands
            if (lowerSpeech == "follow me" || lowerSpeech == "heel")
            {
                Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Processing FOLLOW ME command");
                // Ensure Controlled is true before setting order
                if (!sidekick.Controlled)
                {
                    sidekick.Controlled = true;
                }
                sidekick.ControlOrder = OrderType.Follow;
                sidekick.ControlTarget = from;
                
                // Update following state in sidekick
                if (sidekick is AutonomousSidekick autoSidekick)
                {
                    autoSidekick.IsFollowing = true;
                }
                
                Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Set ControlOrder=Follow, ControlTarget={from.Name}");
                from.SendMessage($"{sidekick.Name} will now follow you.");
                return true;
            }
            else if (lowerSpeech == "follow")
            {
                // "follow" without target = follow me (owner)
                Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Processing FOLLOW command (no target = follow owner)");
                // Ensure Controlled is true before setting order
                if (!sidekick.Controlled)
                {
                    sidekick.Controlled = true;
                }
                sidekick.ControlOrder = OrderType.Follow;
                sidekick.ControlTarget = from;
                
                // Update following state in sidekick
                if (sidekick is AutonomousSidekick autoSidekick)
                {
                    autoSidekick.IsFollowing = true;
                }
                
                Console.WriteLine($"[SidekickSpeechHandler] ProcessCommand: Set ControlOrder=Follow, ControlTarget={from.Name}");
                from.SendMessage($"{sidekick.Name} will now follow you.");
                return true;
            }
            else if (lowerSpeech == "stay" || lowerSpeech == "stay here")
            {
                sidekick.ControlOrder = OrderType.Stay;
                sidekick.ControlTarget = null;
                
                // Update following state in sidekick
                if (sidekick is AutonomousSidekick autoSidekick)
                {
                    autoSidekick.IsFollowing = false;
                }
                
                from.SendMessage($"{sidekick.Name} will stay here.");
                return true;
            }
            else if (lowerSpeech == "guard" || lowerSpeech == "guard me")
            {
                sidekick.ControlOrder = OrderType.Guard;
                sidekick.ControlTarget = from;
                from.SendMessage($"{sidekick.Name} will guard you.");
                return true;
            }
            else if (lowerSpeech == "attack" || lowerSpeech == "kill" || lowerSpeech.StartsWith("attack ") || lowerSpeech.StartsWith("kill "))
            {
                // Attack command - always open targeting cursor (no auto-attack)
                // Auto-attack only happens when there's an active aggressor (handled in OnAggressiveAction/OnDamage)
                if (sidekick.AIObject != null)
                {
                    from.SendMessage($"Target who {sidekick.Name} should attack.");
                    from.Target = new Server.Targets.AIControlMobileTarget(sidekick.AIObject, OrderType.Attack);
                }
                return true;
            }
            else if (lowerSpeech == "come")
            {
                sidekick.ControlOrder = OrderType.Come;
                sidekick.ControlTarget = from;
                from.SendMessage($"{sidekick.Name} will come to you.");
                return true;
            }
            else if (lowerSpeech == "stop")
            {
                sidekick.ControlOrder = OrderType.Stop;
                sidekick.ControlTarget = null;
                
                // Update following state in sidekick
                if (sidekick is AutonomousSidekick autoSidekick)
                {
                    autoSidekick.IsFollowing = false;
                }
                
                from.SendMessage($"{sidekick.Name} will stop.");
                return true;
            }
            else if (lowerSpeech == "release")
            {
                sidekick.ControlOrder = OrderType.None;
                sidekick.ControlTarget = null;
                sidekick.Controlled = false;
                sidekick.ControlMaster = null;
                
                // Update following state in sidekick
                if (sidekick is AutonomousSidekick autoSidekick)
                {
                    autoSidekick.IsFollowing = false;
                    autoSidekick.Owner = null; // Clear owner relationship
                }
                
                from.SendMessage($"{sidekick.Name} has been released.");
                return true;
            }
            else if (lowerSpeech == "transfer")
            {
                if (sidekick.AIObject != null)
                {
                     // Create targeting cursor directly
                     from.SendMessage($"Target who to transfer {sidekick.Name} to.");
                     from.Target = new Server.Targets.AIControlMobileTarget(sidekick.AIObject, OrderType.Transfer);
                }
                return true;
            }
            else if (lowerSpeech == "mount" || lowerSpeech == "mount up")
            {
                // Mount command - open mount selection menu
                if (sidekick is AutonomousSidekick autoSidekick)
                {
                    from.SendMessage($"Target a mountable creature for {sidekick.Name}.");
                    from.Target = new SidekickMountTarget(from as PlayerMobile, autoSidekick);
                }
                return true;
            }

            return false;
        }
        
        // No wrapper sync needed - BaseSidekick now inherits from BaseCreature directly
    }
}

