/*
 * Vystia Class System v2.0
 * Threat System
 *
 * Tracks threat per mob per player for Knight class taunt mechanics.
 * Allows Knights to control enemy targeting through threat generation.
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Systems
{
    /// <summary>
    /// Manages threat tracking for Knight class taunt mechanics
    /// </summary>
    public static class ThreatSystem
    {
        // Track threat per mob per player
        private static Dictionary<BaseCreature, Dictionary<Mobile, int>> m_ThreatTable =
            new Dictionary<BaseCreature, Dictionary<Mobile, int>>();

        /// <summary>
        /// Add threat to a mob for a specific player
        /// </summary>
        public static void AddThreat(BaseCreature mob, Mobile player, int amount)
        {
            if (mob == null || player == null || amount <= 0)
                return;

            if (!m_ThreatTable.ContainsKey(mob))
            {
                m_ThreatTable[mob] = new Dictionary<Mobile, int>();
            }

            if (!m_ThreatTable[mob].ContainsKey(player))
            {
                m_ThreatTable[mob][player] = 0;
            }

            // ChivalricArts skill increases threat generation
            if (player is PlayerMobile pm && pm.Skills != null)
            {
                double chivalricArts = pm.Skills[SkillName.ChivalricArts].Value;
                if (chivalricArts > 0)
                {
                    // +1% threat per 10 skill (10% at GM)
                    double threatBonus = 1.0 + (chivalricArts * 0.001);
                    amount = (int)(amount * threatBonus);
                }
            }

            m_ThreatTable[mob][player] += amount;
        }

        /// <summary>
        /// Get threat value for a specific mob-player pair
        /// </summary>
        public static int GetThreat(BaseCreature mob, Mobile player)
        {
            if (mob == null || player == null)
                return 0;

            if (!m_ThreatTable.ContainsKey(mob))
                return 0;

            if (!m_ThreatTable[mob].ContainsKey(player))
                return 0;

            return m_ThreatTable[mob][player];
        }

        /// <summary>
        /// Get the player with the highest threat for a mob
        /// </summary>
        public static Mobile GetHighestThreat(BaseCreature mob)
        {
            if (mob == null || !m_ThreatTable.ContainsKey(mob))
                return null;

            var threatDict = m_ThreatTable[mob];
            if (threatDict.Count == 0)
                return null;

            Mobile highestThreatPlayer = null;
            int highestThreat = 0;

            foreach (var kvp in threatDict)
            {
                if (kvp.Value > highestThreat && kvp.Key != null && kvp.Key.Alive)
                {
                    highestThreat = kvp.Value;
                    highestThreatPlayer = kvp.Key;
                }
            }

            return highestThreatPlayer;
        }

        /// <summary>
        /// Challenge ability: Force mob to attack Knight for duration
        /// </summary>
        public static void Challenge(BaseCreature mob, Mobile knight, TimeSpan duration)
        {
            if (mob == null || knight == null)
                return;

            // Set massive threat to ensure Knight is targeted
            AddThreat(mob, knight, 100000);

            // Force mob to target Knight
            if (mob.Combatant != knight)
            {
                mob.Combatant = knight;
            }

            // Set a timer to reduce threat after duration
            Timer.DelayCall(duration, () =>
            {
                if (m_ThreatTable.ContainsKey(mob) && m_ThreatTable[mob].ContainsKey(knight))
                {
                    // Reduce threat by 50% after challenge expires
                    m_ThreatTable[mob][knight] = m_ThreatTable[mob][knight] / 2;
                }
            });
        }

        /// <summary>
        /// Clear threat table for a mob (when it dies or despawns)
        /// </summary>
        public static void ClearThreat(BaseCreature mob)
        {
            if (mob != null && m_ThreatTable.ContainsKey(mob))
            {
                m_ThreatTable.Remove(mob);
            }
        }

        /// <summary>
        /// Clear all threat for a player (when they die or log out)
        /// </summary>
        public static void ClearThreatForPlayer(Mobile player)
        {
            if (player == null)
                return;

            var mobsToRemove = new List<BaseCreature>();

            foreach (var mobKvp in m_ThreatTable)
            {
                if (mobKvp.Value.ContainsKey(player))
                {
                    mobKvp.Value.Remove(player);
                    
                    // Remove mob entry if no players have threat
                    if (mobKvp.Value.Count == 0)
                    {
                        mobsToRemove.Add(mobKvp.Key);
                    }
                }
            }

            foreach (var mob in mobsToRemove)
            {
                m_ThreatTable.Remove(mob);
            }
        }

        /// <summary>
        /// Decay threat over time (called periodically)
        /// </summary>
        public static void DecayThreat(BaseCreature mob, int decayAmount = 1)
        {
            if (mob == null || !m_ThreatTable.ContainsKey(mob))
                return;

            var playersToRemove = new List<Mobile>();

            foreach (var playerKvp in m_ThreatTable[mob])
            {
                if (playerKvp.Key == null || !playerKvp.Key.Alive)
                {
                    playersToRemove.Add(playerKvp.Key);
                    continue;
                }

                int newThreat = Math.Max(0, playerKvp.Value - decayAmount);
                m_ThreatTable[mob][playerKvp.Key] = newThreat;

                if (newThreat == 0)
                {
                    playersToRemove.Add(playerKvp.Key);
                }
            }

            foreach (var player in playersToRemove)
            {
                m_ThreatTable[mob].Remove(player);
            }

            // Remove mob entry if no players have threat
            if (m_ThreatTable[mob].Count == 0)
            {
                m_ThreatTable.Remove(mob);
            }
        }
    }
}
