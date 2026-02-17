using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks.UI
{
    /// <summary>
    /// Target for selecting a mount for a sidekick
    /// </summary>
    public class SidekickMountTarget : Target
    {
        private AutonomousSidekick m_Sidekick;
        private PlayerMobile m_Player;

        public SidekickMountTarget(PlayerMobile player, AutonomousSidekick sidekick) : base(10, false, TargetFlags.None)
        {
            m_Player = player;
            m_Sidekick = sidekick;
            Console.WriteLine($"[SidekickMountTarget] Constructor called - Player: {player?.Name}, Sidekick: {sidekick?.Name}, TargetID: {TargetID}");
        }
        
        protected override void OnTarget(Mobile from, object targeted)
        {
            Console.WriteLine($"[SidekickMountTarget] OnTarget called - From: {from?.Name}, Sidekick: {m_Sidekick?.Name}, Target: {targeted?.GetType().Name}");
            
            if (from == null || m_Sidekick == null || m_Sidekick.Deleted)
            {
                Console.WriteLine($"[SidekickMountTarget] OnTarget: Invalid parameters - returning");
                return;
            }

            if (targeted is BaseCreature creature)
            {
                Console.WriteLine($"[SidekickMountTarget] OnTarget: Target is BaseCreature - Name: {creature.Name}, IsIMount: {creature is IMount}, Controlled: {creature.Controlled}, ControlMaster: {creature.ControlMaster?.Name}");
                
                // Check if creature is a valid mount (implements IMount or has mount item)
                if (!(creature is IMount) && creature.Backpack == null)
                {
                    Console.WriteLine($"[SidekickMountTarget] OnTarget: Not a mountable creature");
                    from.SendMessage("That is not a mountable creature.");
                    return;
                }

                // Check if creature is tamed and controlled by player
                if (creature.Controlled && creature.ControlMaster == m_Player)
                {
                    Console.WriteLine($"[SidekickMountTarget] OnTarget: Creature is controlled by player");
                    
                    // Check if creature is a valid mount
                    if (!(creature is IMount))
                    {
                        Console.WriteLine($"[SidekickMountTarget] OnTarget: Creature is not IMount");
                        from.SendMessage("That creature is not mountable.");
                        return;
                    }

                    // Dismount current mount if any
                    if (m_Sidekick.Mount != null)
                    {
                        Console.WriteLine($"[SidekickMountTarget] OnTarget: Dismounting current mount: {m_Sidekick.Mount.GetType().Name}");
                        m_Sidekick.Mount.Rider = null;
                    }

                    // Add to owned pets if not already there
                    if (!m_Sidekick.OwnedPets.Contains(creature))
                    {
                        Console.WriteLine($"[SidekickMountTarget] OnTarget: Adding creature to owned pets");
                        m_Sidekick.AddPet(creature);
                    }

                    // Set as sidekick's mount
                    Console.WriteLine($"[SidekickMountTarget] OnTarget: Setting mount");
                    m_Sidekick.SetMount(creature);

                    // Actually mount the sidekick
                    IMount mount = creature as IMount;
                    if (mount != null)
                    {
                        Console.WriteLine($"[SidekickMountTarget] OnTarget: Mounting sidekick - Mount: {mount.GetType().Name}, Sidekick: {m_Sidekick.Name}");
                        mount.Rider = m_Sidekick;
                        Console.WriteLine($"[SidekickMountTarget] OnTarget: Mount.Rider set to: {mount.Rider?.Name}");
                        from.SendMessage($"{m_Sidekick.Name} has mounted {creature.Name}.");
                    }
                    else
                    {
                        Console.WriteLine($"[SidekickMountTarget] OnTarget: Mount is null after cast");
                        from.SendMessage($"{m_Sidekick.Name} will now use {creature.Name} as a mount.");
                    }
                }
                else
                {
                    Console.WriteLine($"[SidekickMountTarget] OnTarget: Creature not controlled by player - Controlled: {creature.Controlled}, ControlMaster: {creature.ControlMaster?.Name}, Player: {m_Player?.Name}");
                    from.SendMessage("You must own and control that creature to assign it as a mount.");
                }
            }
            else
            {
                Console.WriteLine($"[SidekickMountTarget] OnTarget: Target is not BaseCreature - Type: {targeted?.GetType().Name}");
                from.SendMessage("You must target a mountable creature.");
            }
        }

        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            Console.WriteLine($"[SidekickMountTarget] OnTargetCancel called - From: {from?.Name}, CancelType: {cancelType}");
            if (from != null && from is PlayerMobile player)
            {
                from.SendMessage("Mount selection cancelled.");
                // Optionally reopen the menu
                Timer.DelayCall(TimeSpan.FromMilliseconds(100), () =>
                {
                    if (player != null && !player.Deleted)
                    {
                        player.SendGump(new SidekickMainMenuGump(player));
                    }
                });
            }
        }

        protected override void OnTargetFinish(Mobile from)
        {
            Console.WriteLine($"[SidekickMountTarget] OnTargetFinish called - From: {from?.Name}");
            // Target finished - could reopen menu if needed
            // For now, just log it
        }
    }
}

