using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Base class for sidekicks - inherits from BaseCreature for native pet command support
    /// Provides player-like behavior with proper death handling and command processing
    /// </summary>
    public abstract class BaseSidekick : BaseCreature
    {
        #region Fields

        public new const int MaxLoyalty = 100;
        
        // Cache the ForcedAI instance to prevent recreation and state loss
        private BaseAI m_ForcedAI;

        #endregion

        #region AI Properties

        // BaseCreature already has AIObject, AI, and ForcedAI properties
        // Override ForcedAI to create SidekickAI for AutonomousSidekick
        // IMPORTANT: Cache the instance to prevent recreation on every ChangeAIType() call
        protected override BaseAI ForcedAI
        {
            get 
            { 
                // Create SidekickAI once and cache it
                if (m_ForcedAI == null && this is AutonomousSidekick autonomousSidekick)
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[BaseSidekick.ForcedAI] Creating new SidekickAI for {Name}");
                    Utility.PopColor();
                    
                    m_ForcedAI = new SidekickAI(autonomousSidekick);
                }
                else if (m_ForcedAI != null)
                {
                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[BaseSidekick.ForcedAI] Returning cached SidekickAI for {Name}, Timer Running: {m_ForcedAI.m_Timer?.Running ?? false}");
                    Utility.PopColor();
                }
                
                return m_ForcedAI; 
            }
        }

        #endregion

        #region Constructors

        public BaseSidekick(AIType ai = AIType.AI_Use_Default, FightMode fightMode = FightMode.Closest, int rangePerception = 12, int rangeFight = 1, double activeSpeed = 0.2, double passiveSpeed = 0.4)
            : base(ai, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed)
        {
            // Pet control setup
            Tamable = true;
            MinTameSkill = 0; // Already tamed
            Loyalty = MaxLoyalty;
            Controlled = false;
            ControlOrder = OrderType.None;
            
            // CRITICAL: Set IsBonded to true to enable proper death handling (prevents deletion, allows resurrection)
            IsBonded = true;
            
            // Commandable is read-only in BaseCreature, but defaults to true for controlled creatures
            // Sidekicks don't use follower slots
            FollowersMax = 0;
        }

        /// <summary>
        /// Serialization constructor - REQUIRED for ServUO serialization system
        /// </summary>
        public BaseSidekick(Serial serial) : base(serial)
        {
        }

        #endregion

        #region AI Methods

        // BaseCreature already has ChangeAIType method
        // ForcedAI override handles SidekickAI creation

        public override bool CheckIdle()
        {
            return false;
        }

        // BaseCreature already has AcquireFocusMob method

        #endregion

        // BaseCreature already has DebugSay methods
        // We can use them directly or add new ones if needed

        #region Overrides

        // BaseCreature already has HandlesOnSpeech - it will handle pet commands automatically
        // We can override if needed for custom behavior

        // BaseCreature already handles OnDelete, OnAfterSpawn, and MoveToWorld properly
        // We can override OnDeath to ensure IsBonded handling

        #region Death Handling
        
        /// <summary>
        /// Override OnDeath to ensure IsBonded handling works properly
        /// BaseCreature.OnDeath checks IsBonded and prevents deletion if true
        /// </summary>
        public override void OnDeath(Container c)
                {
            // Ensure IsBonded is set (should already be set in constructor, but double-check)
            if (!IsBonded)
                    {
                IsBonded = true;
            }

            // Call base implementation - it will handle bonded pet death properly
            base.OnDeath(c);
                    }

        #endregion

        #endregion

        #region Serialization

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // Version

            // BaseCreature already serializes pet control properties
            // We just need to ensure IsBonded is serialized (BaseCreature handles this)
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            // Ensure IsBonded is set after deserialization
            if (!IsBonded)
            {
                IsBonded = true;
            }
        }

        #endregion
    }
}
