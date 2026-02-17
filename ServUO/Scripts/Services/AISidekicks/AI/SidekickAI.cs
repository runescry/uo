using System;
using System.IO;
using Server.Mobiles;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// Custom AI for autonomous sidekicks
    /// Extends BaseAI to provide player-like combat and autonomous behavior
    /// Now works directly with BaseCreature (BaseSidekick inherits from BaseCreature)
    /// </summary>
    public class SidekickAI : BaseAI
    {
        private AutonomousSidekick m_Sidekick;
        private PlayerLikeCombatAI m_CombatAI;
        private DateTime m_LastRecallTime;
        private MageAI m_MageAI; // For spell casting when sidekick is a mage (legacy)
        private MageCombatAI m_MageCombatAI; // Advanced mage combat system
        private WarriorCombatAI m_WarriorCombatAI; // Warrior/melee combat
        private ArcherCombatAI m_ArcherCombatAI; // Archer/ranged combat
        private ThiefCombatAI m_ThiefCombatAI; // Thief/stealth combat
        private PaladinCombatAI m_PaladinCombatAI; // Paladin combat
        private HealerCombatAI m_HealerCombatAI; // Healer/support combat
        private TamerCombatAI m_TamerCombatAI; // Tamer pet command combat
        private NecromancerCombatAI m_NecromancerCombatAI; // Necromancer dark magic combat
        private DateTime m_NextCastTime;

        // Stuck detection for tight spaces
        private int m_ConsecutiveRetreatFailures = 0;
        private int m_LastRetreatDistance = 0;
        private DateTime m_LastRetreatAttempt = DateTime.MinValue;

        public SidekickAI(AutonomousSidekick sidekick) : base(sidekick)
        {
            m_Sidekick = sidekick;
            m_CombatAI = new PlayerLikeCombatAI(sidekick);
            
            // NOTE: Don't create MageAI here - CombatStyle is not set yet!
            // MageAI will be initialized after archetype is loaded via InitializeMageAI()
            
            // DIAGNOSTIC: Log AI initialization to server logs
            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine($"[SidekickAI.Constructor] {m_Sidekick.Name} - Timer Created: {m_Timer != null}, Timer Running: {m_Timer?.Running ?? false}, Map: {m_Sidekick.Map}, Action: {Action}");
            Utility.PopColor();
            
            // BaseAI constructor already creates and starts the timer
            // We don't need a custom timer - the base one works fine
        }
        
        /// <summary>
        /// Initialize combat AI after archetype is loaded
        /// Creates the appropriate combat AI based on archetype type
        /// </summary>
        public void InitializeMageAI()
        {
            InitializeCombatAI();
        }
        
        /// <summary>
        /// Initialize the correct combat AI based on archetype
        /// </summary>
        public void InitializeCombatAI()
        {
            if (m_Sidekick == null)
                return;
            
            var archetypeType = m_Sidekick.ArchetypeType;
            
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[SidekickAI.InitializeCombatAI] {m_Sidekick.Name} - Archetype: {archetypeType}, CombatStyle: {m_Sidekick.CombatStyle}");
            Utility.PopColor();
            
            // Create archetype-specific combat AI
            switch (archetypeType)
            {
                case SidekickArchetypeType.Warrior:
                    m_WarriorCombatAI = new WarriorCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created WarriorCombatAI for {m_Sidekick.Name}");
                    break;
                    
                case SidekickArchetypeType.Mage:
                case SidekickArchetypeType.Battlemage:
                case SidekickArchetypeType.Druid:
                    m_MageAI = new MageAI(m_Sidekick);
                    m_MageCombatAI = new MageCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created MageCombatAI for {m_Sidekick.Name}");
                    break;

                case SidekickArchetypeType.Necromancer:
                    m_NecromancerCombatAI = new NecromancerCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created NecromancerCombatAI for {m_Sidekick.Name}");
                    break;
                    
                case SidekickArchetypeType.Archer:
                case SidekickArchetypeType.Ranger:
                    m_ArcherCombatAI = new ArcherCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created ArcherCombatAI for {m_Sidekick.Name}");
                    break;
                    
                case SidekickArchetypeType.Healer:
                case SidekickArchetypeType.Cleric:
                    m_HealerCombatAI = new HealerCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created HealerCombatAI for {m_Sidekick.Name}");
                    break;
                    
                case SidekickArchetypeType.Paladin:
                    m_PaladinCombatAI = new PaladinCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created PaladinCombatAI for {m_Sidekick.Name}");
                    break;
                    
                case SidekickArchetypeType.Thief:
                    m_ThiefCombatAI = new ThiefCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created ThiefCombatAI for {m_Sidekick.Name}");
                    break;
                    
                case SidekickArchetypeType.Tamer:
                    m_TamerCombatAI = new TamerCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created TamerCombatAI for {m_Sidekick.Name}");
                    break;
                    
                default:
                    // Default fallback to warrior combat
                    m_WarriorCombatAI = new WarriorCombatAI(m_Sidekick);
                    Console.WriteLine($"[SidekickAI.InitializeCombatAI] Created default WarriorCombatAI for {m_Sidekick.Name}");
                    break;
            }
        }
        
        /// <summary>
        /// Get the active combat AI for bandage usage during follow
        /// </summary>
        private bool TryUseBandageFromCombatAI()
        {
            if (m_MageCombatAI != null) return m_MageCombatAI.TryUseBandagePublic();
            if (m_WarriorCombatAI != null) return m_WarriorCombatAI.TryUseBandagePublic();
            if (m_ArcherCombatAI != null) return m_ArcherCombatAI.TryUseBandagePublic();
            if (m_ThiefCombatAI != null) return m_ThiefCombatAI.TryUseBandagePublic();
            if (m_PaladinCombatAI != null) return m_PaladinCombatAI.TryUseBandagePublic();
            if (m_HealerCombatAI != null) return m_HealerCombatAI.TryUseBandagePublic();
            if (m_TamerCombatAI != null) return m_TamerCombatAI.TryUseBandagePublic();
            if (m_NecromancerCombatAI != null) return m_NecromancerCombatAI.TryUseBandagePublic();
            return false;
        }
        
        /// <summary>
        /// Check if any combat AI is active
        /// </summary>
        private bool HasCombatAI()
        {
            return m_MageCombatAI != null || m_WarriorCombatAI != null || m_ArcherCombatAI != null ||
                   m_ThiefCombatAI != null || m_PaladinCombatAI != null || m_HealerCombatAI != null ||
                   m_TamerCombatAI != null || m_NecromancerCombatAI != null;
        }
        
        /// <summary>
        /// Handle combat ending - check for aggressors first, then return to follow
        /// </summary>
        private void HandleCombatEnd(string reason)
        {
            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine($"[SidekickAI.HandleCombatEnd] {m_Sidekick.Name} - {reason}");
            Utility.PopColor();
            
            // FIRST: Check for any active aggressors before returning to owner
            Mobile aggressor = FindNearestAggressor();
            if (aggressor != null && aggressor.Alive && !aggressor.Deleted)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[SidekickAI.HandleCombatEnd] {m_Sidekick.Name} - Found aggressor {aggressor.Name}, switching target instead of returning");
                Utility.PopColor();

                // Support classes should NOT have Combatant set - they use their own positioning
                bool isSupportClass = m_TamerCombatAI != null || m_HealerCombatAI != null;
                if (!isSupportClass)
                {
                    m_Sidekick.Combatant = aggressor;
                }

                m_Sidekick.ControlTarget = aggressor;
                m_Sidekick.ControlOrder = OrderType.Attack;
                m_Sidekick.Warmode = true;

                // Stay in combat
                Action = ActionType.Combat;
                return;
            }
            
            // No aggressors - clear combat state and return to owner
            m_Sidekick.Combatant = null;
            m_Sidekick.Warmode = false;
            
            // Change ControlOrder to Follow when combat ends
            if (m_Sidekick.ControlOrder == OrderType.Attack || m_Sidekick.ControlOrder == OrderType.None)
            {
                if (m_Sidekick.Controlled && m_Sidekick.ControlMaster != null)
                {
                    m_Sidekick.ControlOrder = OrderType.Follow;
                    m_Sidekick.ControlTarget = m_Sidekick.ControlMaster;
                    
                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[SidekickAI.HandleCombatEnd] {m_Sidekick.Name} - No aggressors, returning to owner");
                    Utility.PopColor();
                }
            }
            
            Action = ActionType.Guard;
            Obey();
        }
        
        /// <summary>
        /// Override Think - no sync needed since m_Mobile is now the sidekick directly
        /// </summary>
        public override bool Think()
        {
            if (m_Sidekick.Deleted)
                return false;

            // Use sidekick's map/location for checks
            if (m_Sidekick.Map == null || m_Sidekick.Map == Map.Internal)
                return false;

            // DIAGNOSTIC: Log Think() execution to server logs
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[SidekickAI.Think] {m_Sidekick.Name} - Action: {Action}, Combatant: {m_Sidekick.Combatant?.Name ?? "null"}, ControlOrder: {m_Sidekick.ControlOrder}, Timer Running: {m_Timer?.Running ?? false}");
            Utility.PopColor();

            // CRITICAL: Check for aggressors BEFORE calling base.Think()
            // This ensures we detect aggressors even when in Guard/Wander mode
            if (m_Sidekick.Combatant == null && Action != ActionType.Combat)
            {
                Mobile aggressor = FindNearestAggressor();
                if (aggressor != null)
                {
                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine($"[SidekickAI.Think] {m_Sidekick.Name} - DETECTED AGGRESSOR in Think(): {aggressor.Name}, engaging combat");
                    Utility.PopColor();

                    // Support classes should NOT have Combatant set - they use their own positioning
                    bool isSupportClass = m_TamerCombatAI != null || m_HealerCombatAI != null;
                    if (!isSupportClass)
                    {
                        m_Sidekick.Combatant = aggressor;
                    }

                    m_Sidekick.Warmode = true;
                    Action = ActionType.Combat;
                    return true; // Don't call base.Think() - we're entering combat
                }
            }

            // Call base Think logic
            return base.Think();
        }

        /// <summary>
        /// Override OnAggressiveAction to ensure aggressors are properly detected
        /// This is called when the sidekick is attacked
        /// </summary>
        public override void OnAggressiveAction(Mobile aggressor)
        {
            if (m_Sidekick == null || m_Sidekick.Deleted || aggressor == null || aggressor.Deleted)
                return;

            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[SidekickAI.OnAggressiveAction] {m_Sidekick.Name} - ATTACKED by {aggressor.Name}!");
            Utility.PopColor();

            // Call base implementation first (handles standard aggressor detection)
            base.OnAggressiveAction(aggressor);

            // CRITICAL: Ensure we engage combat immediately, even if in Follow mode
            // Sidekicks should always defend themselves when attacked
            if (m_Sidekick.CanBeHarmful(aggressor, false) && aggressor != m_Sidekick.ControlMaster)
            {
                // Check if we already have a valid combat target
                bool hasValidTarget = m_Sidekick.ControlOrder == OrderType.Attack && 
                                     m_Sidekick.ControlTarget != null && 
                                     !m_Sidekick.ControlTarget.Deleted;
                
                // If ControlTarget is a Mobile, check if it's still alive
                if (hasValidTarget && m_Sidekick.ControlTarget is Mobile currentTarget)
                {
                    hasValidTarget = currentTarget.Alive;
                }
                
                // Only switch targets if:
                // 1. Not already in Attack mode
                // 2. Current target is dead/null/deleted
                if (!hasValidTarget)
                {
                    m_Sidekick.ControlOrder = OrderType.Attack;
                    m_Sidekick.ControlTarget = aggressor;
                    
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[SidekickAI.OnAggressiveAction] {m_Sidekick.Name} - Changed ControlOrder to Attack {aggressor.Name} (no valid target)");
                    Utility.PopColor();
                }
                else
                {
                    Utility.PushColor(ConsoleColor.DarkYellow);
                    Console.WriteLine($"[SidekickAI.OnAggressiveAction] {m_Sidekick.Name} - Already attacking {m_Sidekick.ControlTarget}, ignoring {aggressor.Name}");
                    Utility.PopColor();
                }
                
                // Set combatant only if we don't have one or current is dead
                bool hasValidCombatant = m_Sidekick.Combatant != null && 
                                        !m_Sidekick.Combatant.Deleted;
                
                if (hasValidCombatant && m_Sidekick.Combatant is Mobile currentCombatant)
                {
                    hasValidCombatant = currentCombatant.Alive;
                }
                
                if (!hasValidCombatant)
                {
                    // Support classes should NOT have Combatant set - they use their own positioning
                    bool isSupportClass = m_TamerCombatAI != null || m_HealerCombatAI != null;
                    if (!isSupportClass)
                    {
                        m_Sidekick.Combatant = aggressor;
                    }

                    m_Sidekick.Warmode = true;
                    Action = ActionType.Combat;

                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[SidekickAI.OnAggressiveAction] {m_Sidekick.Name} - Set Combatant to {aggressor.Name}, Warmode=true, Action=Combat (support={isSupportClass})");
                    Utility.PopColor();
                }
            }
        }

        #region Movement Redirects
        // Redirect all movement logic to the Sidekick mobile instead of the wrapper

        public override bool CheckMove()
        {
            // Check sidekick's movement timer
            return (Core.TickCount - NextMove >= 0);
        }

        public override bool DoMove(Direction d, bool badStateOk)
        {
            var res = DoMoveImpl(d);
            return (res == MoveResult.Success || res == MoveResult.SuccessAutoTurn ||
                    (badStateOk && res == MoveResult.BadState));
        }

        public override MoveResult DoMoveImpl(Direction d)
        {
            if (m_Sidekick == null || m_Sidekick.Deleted || m_Sidekick.Frozen || m_Sidekick.Paralyzed || !m_Sidekick.CanMove ||
                (m_Sidekick.Spell != null && m_Sidekick.Spell.IsCasting && m_Sidekick.FreezeOnCast))
            {
                return MoveResult.BadState;
            }

            // Use Sidekick's current speed
            // SpeedInfo.TransformMoveDelay expects BaseCreature, so we use the wrapper which syncs speeds
            // Or we can just use the logic directly if we want to be independent
            // For now, let's use the wrapper since it is a BaseCreature
            var delay = (int)(TransformMoveDelay(m_Sidekick.CurrentSpeed) * 1000);

            // CRITICAL: If Direction.Running is already explicitly set, preserve it
            // This ensures that when run=true is passed, we always run
            bool explicitRunning = (d & Direction.Running) != 0;

            var mounted = (m_Sidekick.Mounted || m_Sidekick.Flying);
            // Check run capability on sidekick (only if not already explicitly set)
            var running = explicitRunning || (CanRun && (mounted ? (delay < Mobile.WalkMount) : (delay < Mobile.WalkFoot)));

            if (running)
            {
                d |= Direction.Running;
            }

            m_Sidekick.Direction = d;
            NextMove += delay;

            if (Core.TickCount - NextMove > 0)
            {
                NextMove = Core.TickCount;
            }

            m_Sidekick.Pushing = false;

            // Perform the move on the sidekick
            if (!m_Sidekick.Move(d))
            {
                // Handle blocked movement logic (opening doors, destroying obstacles) if needed
                // For now, simple block check
                return MoveResult.Blocked;
            }

            return MoveResult.Success;
        }

        public override double TransformMoveDelay(double delay)
        {
            // Calculate delay based on sidekick directly (it's now a BaseCreature)
            delay = SpeedInfo.TransformMoveDelay(m_Sidekick, delay);

            if (double.IsNaN(delay))
            {
                return 1.0;
            }

            return delay;
        }

        public override void OnCurrentSpeedChanged()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer.Delay = TimeSpan.FromSeconds(Utility.RandomDouble());
                m_Timer.Interval = TimeSpan.FromSeconds(Math.Max(0.0, m_Sidekick.CurrentSpeed));
                m_Timer.Start();
            }
        }

        public override bool MoveTo(IPoint3D p, bool run, int range)
        {
            if (m_Sidekick.Deleted || !m_Sidekick.CanMove || p == null || (p is IDamageable && ((IDamageable)p).Deleted))
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[SidekickAI.MoveTo] {m_Sidekick.Name} - BLOCKED: Deleted={m_Sidekick.Deleted}, CanMove={m_Sidekick.CanMove}, Target={(p == null ? "null" : "valid")}");
                Utility.PopColor();
                return false;
            }

            // Use GetDistanceToSqrt for more accurate distance checking
            // InRange might be too lenient and prevent movement when we should move
            double distance = m_Sidekick.GetDistanceToSqrt(p);
            
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[SidekickAI.MoveTo] {m_Sidekick.Name} - Target: {(p is Mobile ? ((Mobile)p).Name : p.ToString())}, Distance: {distance:F1}, Range: {range}, Run: {run}, Direction: {m_Sidekick.Direction}");
            Utility.PopColor();
            
            if (distance <= range)
            {
                m_Path = null;
                return true;
            }

            if (m_Path != null && m_Path.Goal == p)
            {
                if (m_Path.Follow(run, 1))
                {
                    m_Path = null;
                    return true;
                }
            }
            else
            {
                // Get direction to target and add Running flag if needed
                Direction directionToTarget = m_Sidekick.GetDirectionTo(p);
                if (run)
                {
                    directionToTarget |= Direction.Running;
                }
                
                if (!DoMove(directionToTarget, run))
            {
                m_Path = new PathFollower(m_Sidekick, p);
                m_Path.Mover = DoMoveImpl;

                if (m_Path.Follow(run, 1))
                {
                    m_Path = null;
                    return true;
                }
            }
            else
            {
                m_Path = null;
                return true;
                }
            }

            return false;
        }

        public override bool WalkMobileRange(IPoint3D p, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax)
        {
            if (m_Sidekick.Deleted || !m_Sidekick.CanMove)
            {
                return false;
            }

            if (p != null)
            {
                for (var i = 0; i < iSteps; i++)
                {
                    // Get the curent distance
                    var iCurrDist = (int)m_Sidekick.GetDistanceToSqrt(p);

                    if (iCurrDist < iWantDistMin || iCurrDist > iWantDistMax)
                    {
                        var needCloser = (iCurrDist > iWantDistMax);
                        var needFurther = !needCloser;

                        if (needCloser && m_Path != null && m_Path.Goal == p)
                        {
                            if (m_Path.Follow(bRun, 1))
                            {
                                m_Path = null;
                            }
                        }
                        else
                        {
                            Direction dirTo;

                            if (iCurrDist > iWantDistMax)
                            {
                                dirTo = m_Sidekick.GetDirectionTo(p);
                            }
                            else
                            {
                                dirTo = Utility.GetDirection(p, m_Sidekick);
                            }

                            if (bRun)
                            {
                                dirTo |= Direction.Running;
                            }

                            if (!DoMove(dirTo, true) && needCloser)
                            {
                                m_Path = new PathFollower(m_Sidekick, p);
                                m_Path.Mover = DoMoveImpl;

                                if (m_Path.Follow(bRun, 1))
                                {
                                    m_Path = null;
                                }
                            }
                            else
                            {
                                m_Path = null;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                // Get the curent distance
                var iNewDist = (int)m_Sidekick.GetDistanceToSqrt(p);

                if (iNewDist >= iWantDistMin && iNewDist <= iWantDistMax)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        #endregion

        #region Mage Movement Methods (from MageAI)

        /// <summary>
        /// Run away from a target (for mage kiting)
        /// Handles wall collisions by trying alternative directions
        /// </summary>
        public void RunFrom(IDamageable target)
        {
            RunFrom(target, Point3D.Zero);
        }

        /// <summary>
        /// Run away from a target, optionally preferring a specific corner
        /// </summary>
        public void RunFrom(IDamageable target, Point3D preferredCorner)
        {
            if (target == null || target.Deleted)
                return;

            Point3D positionBeforeRetreat = m_Sidekick.Location;
            Direction baseRetreatDir = (Direction)(((int)m_Sidekick.GetDirectionTo(target) - 4) & (int)Direction.Mask);

            // Check for aggressors in the retreat path and change direction if needed
            Direction retreatDir = GetSafeRetreatDirection(baseRetreatDir);

            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - Retreating from {target}, direction: {retreatDir}, current position: {positionBeforeRetreat}");
            if (preferredCorner != Point3D.Zero)
            {
                Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - Preferred corner: {preferredCorner}");
            }
            Utility.PopColor();
            
            // PHASE 6: If preferred corner is provided, prioritize it
            if (preferredCorner != Point3D.Zero && m_Sidekick.Map != null)
            {
                int distanceToCorner = (int)m_Sidekick.GetDistanceToSqrt(preferredCorner);
                
                // If corner is within reasonable range, navigate to it
                if (distanceToCorner <= 8)
                {
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - CORNER-AWARE: Navigating to preferred corner at {preferredCorner} ({distanceToCorner} tiles)");
                    Utility.PopColor();
                    
                    // Use PathFollower to navigate to corner
                    IPoint3D cornerTarget = preferredCorner;
                    
                    if (m_Path != null && m_Path.Goal == cornerTarget)
                    {
                        // Continue following existing path to corner
                        if (m_Path.Follow(true, 3))
                        {
                            m_Path = null; // Reached corner
                        }
                    }
                    else
                    {
                        // Create new path to corner
                        m_Path = new PathFollower(m_Sidekick, cornerTarget);
                        m_Path.Mover = new MoveMethod(DoMoveImpl);
                        
                        if (m_Path.Follow(true, 3))
                        {
                            m_Path = null; // Reached corner
                        }
                    }
                    
                    return; // Corner navigation takes priority
                }
            }
            
            // First, try quick direct movement in retreat direction (fast path)
            Direction directionToTry = retreatDir | Direction.Running;
            MoveResult result = DoMoveImpl(directionToTry);
            
            if (result == MoveResult.Success || result == MoveResult.SuccessAutoTurn)
            {
                // Direct movement succeeded
                if (!DirectionLocked)
                {
                    m_Sidekick.Direction = directionToTry;
                }
                return;
            }
            
            // Direct movement blocked - try perpendicular and diagonal directions aggressively
            // This helps when stuck between a wall and the enemy
            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - Direct retreat blocked, trying all escape directions");
            Utility.PopColor();
            
            // Try all 8 directions (prioritize perpendicular, then diagonal, then others)
            // Order: left perp, right perp, left diag, right diag, then others
            int[] directionOffsets = new int[] { -2, 2, -1, 1, -3, 3, -4, 4 };
            
            foreach (int offset in directionOffsets)
            {
                Direction testDir = (Direction)(((int)retreatDir + offset) & (int)Direction.Mask);
                
                // Try multiple steps in this direction (up to 3 steps)
                for (int steps = 1; steps <= 3; steps++)
                {
                    result = DoMoveImpl(testDir | Direction.Running);
                    if (result == MoveResult.Success || result == MoveResult.SuccessAutoTurn)
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - Found escape via direction {testDir} ({steps} step(s))");
                        Utility.PopColor();
                        if (!DirectionLocked)
                        {
                            m_Sidekick.Direction = testDir | Direction.Running;
                        }
                        return;
                    }
                    else if (result == MoveResult.Blocked || result == MoveResult.BadState)
                    {
                        // This direction is blocked, try next
                        break;
                    }
                }
            }
            
            // Perpendicular directions also blocked - use PathFollower to navigate around obstacles
            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - Perpendicular directions blocked, calculating retreat point for PathFollower");
            Utility.PopColor();
            
            // Calculate multiple retreat points in different directions
            // Prioritize points that are both far from enemy AND accessible
            Point3D bestRetreatPoint = Point3D.Zero;
            int bestScore = 0;
            Mobile enemyMobile = target as Mobile;
            
            // PHASE 6: If preferred corner is provided but too far, still give it bonus score
            if (preferredCorner != Point3D.Zero && m_Sidekick.Map != null)
            {
                int distanceToCorner = (int)m_Sidekick.GetDistanceToSqrt(preferredCorner);
                int distanceFromEnemy = 0;
                if (enemyMobile != null)
                {
                    distanceFromEnemy = (int)Math.Sqrt(Math.Pow(preferredCorner.X - enemyMobile.X, 2) + Math.Pow(preferredCorner.Y - enemyMobile.Y, 2));
                }
                
                // Check if corner is accessible
                if (m_Sidekick.Map.CanFit(preferredCorner.X, preferredCorner.Y, preferredCorner.Z, 16, false, true, true))
                {
                    int cornerScore = distanceFromEnemy * 2; // Corner gets double score bonus
                    cornerScore += 20; // Additional bonus for being a corner
                    
                    if (cornerScore > bestScore)
                    {
                        bestScore = cornerScore;
                        bestRetreatPoint = preferredCorner;
                    }
                }
            }
            
            // CONTEXT-AWARE RETREAT: Detect if we're in an enclosed space (tunnel/corridor)
            // In enclosed spaces, try closer points first (more likely to be reachable)
            // In open areas, try far points first (for proper kiting distance)
            bool inEnclosedSpace = IsInEnclosedSpace();
            
            int currentDistanceToEnemy = 0;
            if (enemyMobile != null)
            {
                currentDistanceToEnemy = (int)m_Sidekick.GetDistanceToSqrt(enemyMobile);
            }
            
            // Target minimum safe distance: 15 tiles (MIN_SAFE_RANGE is 10, but we want buffer)
            int targetMinDistance = 15;
            int minDistance = inEnclosedSpace ? 3 : 5; // More lenient in enclosed spaces
            
            if (inEnclosedSpace)
            {
                // ENCLOSED SPACE: Try closer points first (5-9, then 10-14, then 15-25)
                // This is more efficient since far points are likely blocked by walls
                // FIXED: Phases are now properly sequential, not nested
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - In enclosed space, trying closer retreat points first");
                Utility.PopColor();
                
                // Phase 1: Try close distances (5-9 tiles) first
                for (int offset = 0; offset < 8; offset++)
                {
                    Direction testDir = (Direction)(((int)retreatDir + offset) & (int)Direction.Mask);
                    
                    for (int dist = 5; dist <= 9; dist++)
                    {
                        Point3D testPoint = CalculateRetreatPointInDirection(testDir, target, dist);
                        
                        if (testPoint != Point3D.Zero)
                        {
                            int distanceFromEnemy = 0;
                            if (enemyMobile != null)
                            {
                                distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - enemyMobile.X, 2) + Math.Pow(testPoint.Y - enemyMobile.Y, 2));
                            }
                            else if (target is Item item)
                            {
                                distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - item.X, 2) + Math.Pow(testPoint.Y - item.Y, 2));
                            }
                            
                            // Evaluate and score this retreat point
                            int score = 0;
                            if (distanceFromEnemy >= targetMinDistance)
                            {
                                score = distanceFromEnemy * 10 + 100; // HUGE bonus for safe distance
                            }
                            else
                            {
                                score = distanceFromEnemy * 2;
                            }
                            
                            if (offset == 2 || offset == 6) score += 20; // Perpendicular bonus
                            else if (offset == 1 || offset == 7) score += 10; // Diagonal bonus
                            
                            if (distanceFromEnemy < minDistance) score = 0;
                            
                            if (score > bestScore)
                            {
                                bestScore = score;
                                bestRetreatPoint = testPoint;
                            }
                        }
                    }
                }
                
                // Phase 2: Try medium distances (10-14 tiles) if no close point found
                if (bestRetreatPoint == Point3D.Zero)
                {
                    for (int offset = 0; offset < 8; offset++)
                    {
                        Direction testDir = (Direction)(((int)retreatDir + offset) & (int)Direction.Mask);
                        
                        for (int dist = 10; dist <= 14; dist++)
                        {
                            Point3D testPoint = CalculateRetreatPointInDirection(testDir, target, dist);
                            
                            if (testPoint != Point3D.Zero)
                            {
                                int distanceFromEnemy = 0;
                                if (enemyMobile != null)
                                {
                                    distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - enemyMobile.X, 2) + Math.Pow(testPoint.Y - enemyMobile.Y, 2));
                                }
                                else if (target is Item item)
                                {
                                    distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - item.X, 2) + Math.Pow(testPoint.Y - item.Y, 2));
                                }
                                
                                // Evaluate and score this retreat point
                                int score = 0;
                                if (distanceFromEnemy >= targetMinDistance)
                                {
                                    score = distanceFromEnemy * 10 + 100; // HUGE bonus for safe distance
                                }
                                else
                                {
                                    score = distanceFromEnemy * 2;
                                }
                                
                                if (offset == 2 || offset == 6) score += 20; // Perpendicular bonus
                                else if (offset == 1 || offset == 7) score += 10; // Diagonal bonus
                                
                                if (distanceFromEnemy < minDistance) score = 0;
                                
                                if (score > bestScore)
                                {
                                    bestScore = score;
                                    bestRetreatPoint = testPoint;
                                }
                            }
                        }
                    }
                }
                
                // Phase 3: Try far distances (15-25 tiles) as last resort
                if (bestRetreatPoint == Point3D.Zero)
                {
                    for (int offset = 0; offset < 8; offset++)
                    {
                        Direction testDir = (Direction)(((int)retreatDir + offset) & (int)Direction.Mask);
                        
                        for (int dist = 15; dist <= 25; dist += 2)
                        {
                            Point3D testPoint = CalculateRetreatPointInDirection(testDir, target, dist);
                            
                            if (testPoint != Point3D.Zero)
                            {
                                int distanceFromEnemy = 0;
                                if (enemyMobile != null)
                                {
                                    distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - enemyMobile.X, 2) + Math.Pow(testPoint.Y - enemyMobile.Y, 2));
                                }
                                else if (target is Item item)
                                {
                                    distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - item.X, 2) + Math.Pow(testPoint.Y - item.Y, 2));
                                }
                                
                                // Evaluate and score this retreat point
                                int score = 0;
                                if (distanceFromEnemy >= targetMinDistance)
                                {
                                    score = distanceFromEnemy * 10 + 100; // HUGE bonus for safe distance
                                }
                                else
                                {
                                    score = distanceFromEnemy * 2;
                                }
                                
                                if (offset == 2 || offset == 6) score += 20; // Perpendicular bonus
                                else if (offset == 1 || offset == 7) score += 10; // Diagonal bonus
                                
                                if (distanceFromEnemy < minDistance) score = 0;
                                
                                if (score > bestScore)
                                {
                                    bestScore = score;
                                    bestRetreatPoint = testPoint;
                                }
                            }
                        }
                    }
                }
            }
            {
                // OPEN AREA: Try far points first (15-25, then 10-14, then 5-9)
                // This prioritizes proper kiting distance in open spaces
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - In open area, trying far retreat points first");
                Utility.PopColor();
                
                // Phase 1: Try far distances (15-25 tiles) first
                for (int offset = 0; offset < 8; offset++)
                {
                    Direction testDir = (Direction)(((int)retreatDir + offset) & (int)Direction.Mask);
                    
                    for (int dist = 15; dist <= 25; dist += 2)
                    {
                        Point3D testPoint = CalculateRetreatPointInDirection(testDir, target, dist);
                        
                        if (testPoint != Point3D.Zero)
                        {
                            int distanceFromEnemy = 0;
                            if (enemyMobile != null)
                            {
                                distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - enemyMobile.X, 2) + Math.Pow(testPoint.Y - enemyMobile.Y, 2));
                            }
                            else if (target is Item item)
                            {
                                distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - item.X, 2) + Math.Pow(testPoint.Y - item.Y, 2));
                            }
                            
                            // Evaluate and score this retreat point
                            int score = 0;
                            if (distanceFromEnemy >= targetMinDistance)
                            {
                                score = distanceFromEnemy * 10 + 100; // HUGE bonus for safe distance
                            }
                            else
                            {
                                score = distanceFromEnemy * 2;
                            }
                            
                            if (offset == 2 || offset == 6) score += 20; // Perpendicular bonus
                            else if (offset == 1 || offset == 7) score += 10; // Diagonal bonus
                            
                            if (distanceFromEnemy < minDistance) score = 0;
                            
                            if (score > bestScore)
                            {
                                bestScore = score;
                                bestRetreatPoint = testPoint;
                            }
                        }
                    }
                }
                
                // Phase 2: Try medium distances (10-14 tiles) if no far point found
            if (bestRetreatPoint == Point3D.Zero)
            {
                for (int offset = 0; offset < 8; offset++)
                {
                    Direction testDir = (Direction)(((int)retreatDir + offset) & (int)Direction.Mask);
                    
                        for (int dist = 10; dist <= 14; dist++)
                    {
                        Point3D testPoint = CalculateRetreatPointInDirection(testDir, target, dist);
                        
                        if (testPoint != Point3D.Zero)
                        {
                            int distanceFromEnemy = 0;
                            if (enemyMobile != null)
                            {
                                distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - enemyMobile.X, 2) + Math.Pow(testPoint.Y - enemyMobile.Y, 2));
                            }
                            
                                // Evaluate and score this retreat point
                                int score = 0;
                                if (distanceFromEnemy >= targetMinDistance)
                                {
                                    score = distanceFromEnemy * 10 + 100; // HUGE bonus for safe distance
                                }
                                else
                                {
                                    score = distanceFromEnemy * 2;
                                }
                                
                                if (offset == 2 || offset == 6) score += 20; // Perpendicular bonus
                                else if (offset == 1 || offset == 7) score += 10; // Diagonal bonus
                            
                                if (distanceFromEnemy < minDistance) score = 0;
                            
                            if (score > bestScore)
                            {
                                bestScore = score;
                                bestRetreatPoint = testPoint;
                                }
                            }
                        }
                    }
                }
                
                // Phase 3: Try close distances (5-9 tiles) as last resort
                if (bestRetreatPoint == Point3D.Zero)
                {
                    for (int offset = 0; offset < 8; offset++)
                    {
                        Direction testDir = (Direction)(((int)retreatDir + offset) & (int)Direction.Mask);
                        
                        for (int dist = 5; dist <= 9; dist++)
                        {
                            Point3D testPoint = CalculateRetreatPointInDirection(testDir, target, dist);
                            
                            if (testPoint != Point3D.Zero)
                            {
                                int distanceFromEnemy = 0;
                                if (enemyMobile != null)
                                {
                                    distanceFromEnemy = (int)Math.Sqrt(Math.Pow(testPoint.X - enemyMobile.X, 2) + Math.Pow(testPoint.Y - enemyMobile.Y, 2));
                                }
                                
                                // Evaluate and score this retreat point
                                int score = 0;
                                if (distanceFromEnemy >= targetMinDistance)
                                {
                                    score = distanceFromEnemy * 10 + 100; // HUGE bonus for safe distance
                                }
                                else
                                {
                                    score = distanceFromEnemy * 2;
                                }
                                
                                if (offset == 2 || offset == 6) score += 20; // Perpendicular bonus
                                else if (offset == 1 || offset == 7) score += 10; // Diagonal bonus
                                
                                if (distanceFromEnemy < minDistance) score = 0;
                                
                                if (score > bestScore)
                                {
                                    bestScore = score;
                                    bestRetreatPoint = testPoint;
                                }
                            }
                        }
                    }
                }
            }
            
            if (bestRetreatPoint != Point3D.Zero)
            {
                int finalDistance = 0;
                if (enemyMobile != null)
                {
                    finalDistance = (int)Math.Sqrt(Math.Pow(bestRetreatPoint.X - enemyMobile.X, 2) + Math.Pow(bestRetreatPoint.Y - enemyMobile.Y, 2));
                }
                
                int distanceToRetreatPoint = (int)m_Sidekick.GetDistanceToSqrt(bestRetreatPoint);
                
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - Using PathFollower to retreat to {bestRetreatPoint} (distance to point: {distanceToRetreatPoint}, from enemy: {finalDistance}, score: {bestScore})");
                Utility.PopColor();
                
                // OPTIMIZATION: If retreat point is very close (1-2 tiles), try direct movement first
                if (distanceToRetreatPoint <= 2)
                {
                    Direction dirToPoint = m_Sidekick.GetDirectionTo(bestRetreatPoint);
                    result = DoMoveImpl(dirToPoint | Direction.Running);
                    if (result == MoveResult.Success || result == MoveResult.SuccessAutoTurn)
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - Direct movement to close retreat point succeeded");
                        Utility.PopColor();
                        if (!DirectionLocked)
                        {
                            m_Sidekick.Direction = dirToPoint | Direction.Running;
                        }
                        return;
                    }
                }
                
                // Use PathFollower for longer distances
                IPoint3D retreatTarget = bestRetreatPoint;
                
                // CRITICAL FIX: If retreat point is very far (>15 tiles), try a closer intermediate point first
                // PathFollower often fails on very long paths, so break it into smaller segments
                if (distanceToRetreatPoint > 15)
                {
                    // Find a closer intermediate point (5-10 tiles away)
                    Direction dirToRetreat = m_Sidekick.GetDirectionTo(bestRetreatPoint);
                    Point3D intermediatePoint = Point3D.Zero;
                    
                    // Try 5, 7, 10 tiles in the direction of retreat
                    for (int intermediateDist = 5; intermediateDist <= 10; intermediateDist += 2)
                    {
                        int x = m_Sidekick.X;
                        int y = m_Sidekick.Y;
                        for (int i = 0; i < intermediateDist; i++)
                        {
                            Server.Movement.Movement.Offset(dirToRetreat, ref x, ref y);
                        }
                        int z = m_Sidekick.Map != null ? m_Sidekick.Map.GetAverageZ(x, y) : m_Sidekick.Z;
                        Point3D testPoint = new Point3D(x, y, z);
                        
                        if (m_Sidekick.Map != null && m_Sidekick.Map.CanFit(x, y, z, 16, false, true, true))
                        {
                            intermediatePoint = testPoint;
                            break; // Found a valid intermediate point
                        }
                    }
                    
                    if (intermediatePoint != Point3D.Zero)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - Retreat point too far ({distanceToRetreatPoint} tiles), using intermediate point at {intermediatePoint} ({(int)m_Sidekick.GetDistanceToSqrt(intermediatePoint)} tiles)");
                        Utility.PopColor();
                        retreatTarget = intermediatePoint;
                    }
                }
                
                // Check if we already have a path to this point
                if (m_Path != null && m_Path.Goal == retreatTarget)
                {
                    // Continue following existing path - try to make progress
                    Point3D posBeforePath = m_Sidekick.Location;
                    if (m_Path.Follow(true, 3)) // Move up to 3 steps
                    {
                        m_Path = null; // Reached goal
                    }
                    else
                    {
                        // Check if we actually moved
                        if (m_Sidekick.Location == posBeforePath)
                        {
                            // PathFollower isn't making progress - try direct movement in retreat direction
                            Utility.PushColor(ConsoleColor.Yellow);
                            Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - PathFollower not making progress, trying direct movement");
                            Utility.PopColor();
                            
                            // Try direct movement in retreat direction as fallback
                            result = DoMoveImpl(retreatDir | Direction.Running);
                            if (result == MoveResult.Success || result == MoveResult.SuccessAutoTurn)
                            {
                                if (!DirectionLocked)
                                {
                                    m_Sidekick.Direction = retreatDir | Direction.Running;
                                }
                                return;
                            }
                            
                            // If direct movement failed, clear path and try again next tick
                                m_Path = null;
                        }
                    }
                }
                else
                {
                    // Create new path
                    m_Path = new PathFollower(m_Sidekick, retreatTarget);
                    m_Path.Mover = new MoveMethod(DoMoveImpl);
                    
                    // Try to follow the path - call multiple times to make progress
                    Point3D posBeforePath = m_Sidekick.Location;
                    if (m_Path.Follow(true, 3)) // Move up to 3 steps
                    {
                        m_Path = null; // Reached goal
                    }
                    else if (m_Sidekick.Location == posBeforePath)
                    {
                        // PathFollower didn't move us - try direct movement as fallback
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - PathFollower failed to move, trying direct movement");
                        Utility.PopColor();
                        
                        result = DoMoveImpl(retreatDir | Direction.Running);
                        if (result == MoveResult.Success || result == MoveResult.SuccessAutoTurn)
                        {
                            if (!DirectionLocked)
                            {
                                m_Sidekick.Direction = retreatDir | Direction.Running;
                            }
                            return;
                        }
                        
                        // If direct movement also failed, clear path
                        m_Path = null;
                    }
                }
            }
            else
            {
                // No valid retreat point found - try diagonal directions as last resort
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - No valid retreat point found, trying diagonal directions");
                Utility.PopColor();
                
                Direction leftDiag = (Direction)(((int)retreatDir - 1) & (int)Direction.Mask);
                Direction rightDiag = (Direction)(((int)retreatDir + 1) & (int)Direction.Mask);
                
                result = DoMoveImpl(leftDiag | Direction.Running);
                if (result == MoveResult.Blocked || result == MoveResult.BadState)
                {
                    result = DoMoveImpl(rightDiag | Direction.Running);
                }
                
                if (!DirectionLocked && (result == MoveResult.Success || result == MoveResult.SuccessAutoTurn))
                {
                    m_Sidekick.Direction = (result == MoveResult.Success ? leftDiag : rightDiag) | Direction.Running;
                }
                else
                {
                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine($"[SidekickAI.RunFrom] {m_Sidekick.Name} - COMPLETELY STUCK - cannot find any valid retreat path!");
                    Utility.PopColor();
                }
            }
        }
        
        /// <summary>
        /// Get a safe retreat direction by checking for aggressors in the path.
        /// If an aggressor is detected, try perpendicular directions instead.
        /// </summary>
        private Direction GetSafeRetreatDirection(Direction baseRetreatDir)
        {
            if (m_Sidekick.Map == null)
                return baseRetreatDir;

            // Check if there's an aggressor in the base retreat direction (10 tiles ahead)
            if (HasAggressorInDirection(baseRetreatDir, 10))
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[SidekickAI.GetSafeRetreatDirection] {m_Sidekick.Name} - Aggressor detected in retreat path, trying alternate direction");
                Utility.PopColor();

                // Try left perpendicular
                Direction leftDir = (Direction)(((int)baseRetreatDir - 2) & (int)Direction.Mask);
                if (!HasAggressorInDirection(leftDir, 10))
                {
                    return leftDir;
                }

                // Try right perpendicular
                Direction rightDir = (Direction)(((int)baseRetreatDir + 2) & (int)Direction.Mask);
                if (!HasAggressorInDirection(rightDir, 10))
                {
                    return rightDir;
                }

                // Try left diagonal
                Direction leftDiag = (Direction)(((int)baseRetreatDir - 1) & (int)Direction.Mask);
                if (!HasAggressorInDirection(leftDiag, 10))
                {
                    return leftDiag;
                }

                // Try right diagonal
                Direction rightDiag = (Direction)(((int)baseRetreatDir + 1) & (int)Direction.Mask);
                if (!HasAggressorInDirection(rightDiag, 10))
                {
                    return rightDiag;
                }

                // All directions have aggressors, use base direction anyway
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[SidekickAI.GetSafeRetreatDirection] {m_Sidekick.Name} - All retreat directions blocked by aggressors!");
                Utility.PopColor();
            }

            return baseRetreatDir;
        }

        /// <summary>
        /// Check if there's an aggressor in a given direction within specified distance
        /// </summary>
        private bool HasAggressorInDirection(Direction dir, int distance)
        {
            if (m_Sidekick.Map == null)
                return false;

            int x = m_Sidekick.X;
            int y = m_Sidekick.Y;

            // Check along the path in this direction
            for (int i = 0; i < distance; i++)
            {
                Server.Movement.Movement.Offset(dir, ref x, ref y);
            }

            // Look for hostile mobiles near the endpoint
            IPooledEnumerable eable = m_Sidekick.Map.GetMobilesInRange(new Point3D(x, y, m_Sidekick.Z), 3);

            foreach (Mobile m in eable)
            {
                if (m == m_Sidekick || m == m_Sidekick.ControlMaster)
                    continue;

                // Check if this mobile is hostile
                if (m is BaseCreature bc && (bc.IsEnemy(m_Sidekick) || bc.Combatant == m_Sidekick))
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[SidekickAI.HasAggressorInDirection] {m_Sidekick.Name} - Found aggressor {m.Name} in direction {dir}");
                    Utility.PopColor();
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        /// <summary>
        /// Detect if the sidekick is in an enclosed space (tunnel, corridor, etc.)
        /// by checking how many nearby directions are walkable
        /// </summary>
        private bool IsInEnclosedSpace()
        {
            if (m_Sidekick.Map == null)
                return false;
            
            // Check walkability in all 8 directions around the sidekick
            int walkableDirections = 0;
            int blockedDirections = 0;
            
            for (int i = 0; i < 8; i++)
            {
                Direction dir = (Direction)i;
                int x = m_Sidekick.X;
                int y = m_Sidekick.Y;
                
                // Check 2-3 tiles in this direction
                Server.Movement.Movement.Offset(dir, ref x, ref y);
                Server.Movement.Movement.Offset(dir, ref x, ref y);
                
                int z = m_Sidekick.Map.GetAverageZ(x, y);
                
                if (m_Sidekick.Map.CanFit(x, y, z, 16, false, false, true))
                {
                    walkableDirections++;
                }
                else
                {
                    blockedDirections++;
                }
            }
            
            // If 5+ directions are blocked, we're likely in an enclosed space
            // If 3 or fewer directions are walkable, we're in a narrow corridor
            bool inEnclosedSpace = blockedDirections >= 5 || walkableDirections <= 3;
            
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[SidekickAI.IsInEnclosedSpace] {m_Sidekick.Name} - Walkable: {walkableDirections}, Blocked: {blockedDirections}, Enclosed: {inEnclosedSpace}");
            Utility.PopColor();
            
            return inEnclosedSpace;
        }
        
        /// <summary>
        /// Calculate a retreat point in a specific direction
        /// </summary>
        private Point3D CalculateRetreatPointInDirection(Direction dir, IDamageable target, int distance)
        {
            if (target == null || target.Deleted || m_Sidekick.Map == null)
                return Point3D.Zero;
            
            int x = m_Sidekick.X;
            int y = m_Sidekick.Y;
            
            for (int i = 0; i < distance; i++)
            {
                Server.Movement.Movement.Offset(dir, ref x, ref y);
            }
            
            int z = m_Sidekick.Map.GetAverageZ(x, y);
            Point3D point = new Point3D(x, y, z);
            
            // Verify the point is valid and walkable
            if (m_Sidekick.Map.CanFit(x, y, z, 16, false, false, true))
            {
                return point;
            }
            
            return Point3D.Zero;
        }
        
        /// <summary>
        /// Calculate a retreat point away from target
        /// </summary>
        private Point3D CalculateRetreatPoint(IDamageable target, int distance)
        {
            if (target == null || target.Deleted)
                return Point3D.Zero;
                
            Direction awayDir = (Direction)(((int)m_Sidekick.GetDirectionTo(target) - 4) & (int)Direction.Mask);
            
            // Calculate point in retreat direction
            int x = m_Sidekick.X;
            int y = m_Sidekick.Y;
            
            for (int i = 0; i < distance; i++)
            {
                Server.Movement.Movement.Offset(awayDir, ref x, ref y);
            }
            
            int z = m_Sidekick.Map != null ? m_Sidekick.Map.GetAverageZ(x, y) : m_Sidekick.Z;
            return new Point3D(x, y, z);
        }
        
        /// <summary>
        /// Run in a specific direction (for mage kiting)
        /// From MageAI.Run()
        /// </summary>
        public void Run(Direction d)
        {
            if ((m_Sidekick.Spell != null && m_Sidekick.Spell.IsCasting) || 
                m_Sidekick.Paralyzed || 
                m_Sidekick.Frozen ||
                m_Sidekick.DisallowAllMoves)
            {
                Utility.PushColor(ConsoleColor.Gray);
                Console.WriteLine($"[SidekickAI.Run] {m_Sidekick.Name} - Cannot run: Casting={m_Sidekick.Spell?.IsCasting ?? false}, Paralyzed={m_Sidekick.Paralyzed}, Frozen={m_Sidekick.Frozen}");
                Utility.PopColor();
                return;
            }

            m_Sidekick.Direction = d | Direction.Running;

            bool moveSuccess = DoMove(m_Sidekick.Direction, true);
            
            Utility.PushColor(ConsoleColor.Cyan);
            Console.WriteLine($"[SidekickAI.Run] {m_Sidekick.Name} - DoMove result: {moveSuccess}, direction: {d}");
            Utility.PopColor();

            if (!moveSuccess)
            {
                // If we can't move, just face that direction
                if (!DirectionLocked)
                    m_Sidekick.Direction = d;
            }
        }
        
        #endregion

        /// <summary>
        /// Handle combat action - matches MeleeAI behavior exactly
        /// </summary>
        public override bool DoActionCombat()
        {
            try
            {
                // CRITICAL: Check for ControlOrder changes FIRST
                // If we have a command that is NOT Attack (and not None), we should stop combat and obey
                Utility.PushColor(ConsoleColor.Gray);
                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - ControlOrder check: Controlled={m_Sidekick.Controlled}, ControlOrder={m_Sidekick.ControlOrder}, Combatant={m_Sidekick.Combatant?.Name ?? "null"}");
                Utility.PopColor();
                
                if (m_Sidekick.Controlled && m_Sidekick.ControlOrder != OrderType.None && m_Sidekick.ControlOrder != OrderType.Attack)
                {
                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - ControlOrder changed to {m_Sidekick.ControlOrder}, stopping combat to obey");
                    Utility.PopColor();

                    // Stop combat
                    m_Sidekick.Combatant = null;
                    m_Sidekick.Warmode = false;
                    Action = ActionType.Guard;

                    // Execute the new order immediately
                    Obey();
                    return true;
                }

                // CRITICAL: Process any pending spell targets FIRST (this applies damage/effects)
                // When a spell is cast with spell.Cast(), it creates a Target object assigned to m_Mobile.Target
                // We must invoke that target with the combatant to actually apply the spell
                if (m_Sidekick.Target != null)
                {
                    ProcessSpellTarget();
                }

                // DIAGNOSTIC: Log combat action entry
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Combatant: {m_Sidekick.Combatant?.Name ?? "null"}, Warmode: {m_Sidekick.Warmode}, CombatStyle: {m_Sidekick.CombatStyle}");
                Utility.PopColor();

                // Track current combatant for re-engagement if they run off and return
                if (m_Sidekick.Combatant is Mobile currentCombatant && currentCombatant.Alive)
                {
                    m_LastCombatant = currentCombatant;
                    m_LastCombatantTime = DateTime.UtcNow;
                }
                
                // Use player-like combat AI for special abilities
            if (m_CombatAI != null)
            {
                m_CombatAI.OnThink();
            }

            // CRITICAL FIX: Support classes (Tamer, Healer) don't use Combatant
            // They use ControlTarget as the enemy reference instead
            bool isSupportClass = m_TamerCombatAI != null || m_HealerCombatAI != null;
            IDamageable c = m_Sidekick.Combatant;

            // For support classes, use ControlTarget if Combatant is null
            if (isSupportClass && c == null && m_Sidekick.ControlTarget is IDamageable controlTarget)
            {
                c = controlTarget;
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - SUPPORT CLASS: Using ControlTarget as combat reference: {(c as Mobile)?.Name ?? "unknown"}");
                Utility.PopColor();
            }

            if (c == null || c.Deleted || c.Map != m_Sidekick.Map || !c.Alive || (c is Mobile && ((Mobile)c).IsDeadBondedPet))
            {
                // SUPPORT CLASS FIX: Before ending combat, check if any pets are still fighting
                // For Tamer/Healer, their pets may still be engaged with other enemies
                if (isSupportClass)
                {
                    Mobile newTarget = FindPetCombatTarget();
                    if (newTarget != null)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - SUPPORT CLASS: Pet still fighting {newTarget.Name}, switching target!");
                        Utility.PopColor();

                        m_Sidekick.ControlTarget = newTarget;
                        // Don't end combat - continue with new target
                        c = newTarget;
                    }
                }

                // Re-check if target is still invalid after pet combat check
                if (c == null || c.Deleted || c.Map != m_Sidekick.Map || !c.Alive || (c is Mobile && ((Mobile)c).IsDeadBondedPet))
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Combatant gone/dead, ending combat");
                    Utility.PopColor();

                    m_Sidekick.Combatant = null;
                    m_Sidekick.Warmode = false;

                    // FIXED: Explicitly change ControlOrder to Follow when combat ends
                    if (m_Sidekick.ControlOrder == OrderType.Attack || m_Sidekick.ControlOrder == OrderType.None)
                    {
                        if (m_Sidekick.Controlled && m_Sidekick.ControlMaster != null)
                        {
                            m_Sidekick.ControlOrder = OrderType.Follow;
                            m_Sidekick.ControlTarget = m_Sidekick.ControlMaster;

                            Utility.PushColor(ConsoleColor.Cyan);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Changed ControlOrder to Follow owner");
                            Utility.PopColor();
                        }
                    }

                    // CRITICAL FIX: Set Action to Guard so DoActionGuard gets called on next tick
                    // DoActionGuard has the ControlOrder check that processes new commands
                    Action = ActionType.Guard;

                    // Also call Obey() immediately to process the order right away
                    Obey();
                    return true;
                }
            }

            // Use larger range for mages (they need to kite at distance)
            // PvP mages need even more range to handle players running off-screen then returning
            int effectiveRange = m_Sidekick.RangePerception;
            if (m_Sidekick is AutonomousSidekick autoSidekick && autoSidekick.CombatStyle == CombatStyle.Mage)
            {
                effectiveRange = 30; // Mages need large range for kiting and PvP (matches GIVE_UP_CHASE_DISTANCE + buffer)
            }

            if (!m_Sidekick.InRange(c, effectiveRange))
            {
                // They are somewhat far away, can we find something else?
                // For now, just check if they're too far
                double currentDistance = m_Sidekick.GetDistanceToSqrt(c);
                if (currentDistance > effectiveRange + 1)
                {
                    // CRITICAL: Check if we're still being attacked before ending combat
                    // If we have active aggressors, stay in Attack mode
                    bool hasActiveAggressors = false;
                    if (m_Sidekick.Aggressors != null && m_Sidekick.Aggressors.Count > 0)
                    {
                        foreach (AggressorInfo info in m_Sidekick.Aggressors)
                        {
                            if (info.Attacker != null && !info.Attacker.Deleted && info.Attacker.Alive &&
                                m_Sidekick.CanSee(info.Attacker) && m_Sidekick.CanBeHarmful(info.Attacker, false))
                            {
                                hasActiveAggressors = true;
                                // Switch to the active aggressor
                                m_Sidekick.Combatant = info.Attacker;
                                m_Sidekick.ControlTarget = info.Attacker;
                                Utility.PushColor(ConsoleColor.Yellow);
                                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Current combatant too far, switching to active aggressor: {info.Attacker.Name}");
                                Utility.PopColor();
                                return true; // Continue combat with new target
                            }
                        }
                    }

                    // BEFORE ending combat - check if the original combatant came back into LOS
                    // This handles the "player runs off then returns with combo" scenario
                    if (c != null && !c.Deleted && c.Alive && m_Sidekick.CanSee(c) && m_Sidekick.InLOS(c))
                    {
                        double losDistance = m_Sidekick.GetDistanceToSqrt(c);
                        // If they're back in LOS and within spell range, re-engage
                        if (losDistance <= 12) // Standard spell range
                        {
                            Utility.PushColor(ConsoleColor.Green);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Combatant {c.Name} returned to LOS at {losDistance:F0} tiles - re-engaging!");
                            Utility.PopColor();
                            m_Sidekick.Combatant = c;
                            m_Sidekick.Warmode = true;
                            return true;
                        }
                    }

                    // Only end combat if we're not being attacked
                    if (!hasActiveAggressors)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Combatant fled (too far at {currentDistance:F0} tiles), ending combat");
                        Utility.PopColor();

                        // Clear combatant
                        m_Sidekick.Combatant = null;
                        m_Sidekick.Warmode = false;

                        // If ControlOrder is still Attack, change it to Follow so sidekick returns to owner
                        if (m_Sidekick.ControlOrder == OrderType.Attack || m_Sidekick.ControlOrder == OrderType.None)
                        {
                            if (m_Sidekick.Controlled && m_Sidekick.ControlMaster != null)
                            {
                                m_Sidekick.ControlOrder = OrderType.Follow;
                                m_Sidekick.ControlTarget = m_Sidekick.ControlMaster;

                                Utility.PushColor(ConsoleColor.Cyan);
                                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Changed ControlOrder to Follow owner");
                                Utility.PopColor();
                            }
                        }

                        // CRITICAL FIX: Set Action to Guard so DoActionGuard gets called on next tick
                        // DoActionGuard has the ControlOrder check that processes new commands
                        Action = ActionType.Guard;

                        // Also call Obey() immediately to process the order right away
                        Obey();
                        return true;
                    }
                }
            }

            // For mages, use NEW advanced mage combat system
            if (m_MageCombatAI != null && m_Sidekick.CombatStyle == CombatStyle.Mage)
            {
                Utility.PushColor(ConsoleColor.Magenta);
                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING NEW ADVANCED MAGE COMBAT SYSTEM");
                Utility.PopColor();

                if (c is Mobile mobile)
                {
                    // Check if enemy is dead/gone BEFORE calling DoCombat
                    if (mobile.Deleted || !mobile.Alive || mobile.IsDeadBondedPet)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Enemy dead/gone before combat, ending combat");
                        Utility.PopColor();

                        m_Sidekick.Combatant = null;
                        m_Sidekick.Warmode = false;

                        // Change ControlOrder to Follow when combat ends
                        if (m_Sidekick.ControlOrder == OrderType.Attack || m_Sidekick.ControlOrder == OrderType.None)
                        {
                            if (m_Sidekick.Controlled && m_Sidekick.ControlMaster != null)
                            {
                                m_Sidekick.ControlOrder = OrderType.Follow;
                                m_Sidekick.ControlTarget = m_Sidekick.ControlMaster;

                                Utility.PushColor(ConsoleColor.Cyan);
                                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Changed ControlOrder to Follow owner");
                                Utility.PopColor();
                            }
                        }

                        Action = ActionType.Guard;
                        Obey();
                        return true;
                    }

                    // Use new advanced mage combat AI
                    bool combatHandled = m_MageCombatAI.DoCombat(mobile);
                    
                    // Check if enemy died during combat
                    if (mobile.Deleted || !mobile.Alive || mobile.IsDeadBondedPet)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Enemy died during combat, ending combat");
                        Utility.PopColor();

                        m_Sidekick.Combatant = null;
                        m_Sidekick.Warmode = false;

                        // Change ControlOrder to Follow when combat ends
                        if (m_Sidekick.ControlOrder == OrderType.Attack || m_Sidekick.ControlOrder == OrderType.None)
                        {
                            if (m_Sidekick.Controlled && m_Sidekick.ControlMaster != null)
                            {
                                m_Sidekick.ControlOrder = OrderType.Follow;
                                m_Sidekick.ControlTarget = m_Sidekick.ControlMaster;

                                Utility.PushColor(ConsoleColor.Cyan);
                                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Changed ControlOrder to Follow owner");
                                Utility.PopColor();
                            }
                        }

                        Action = ActionType.Guard;
                        Obey();
                        return true;
                    }
                    
                    // If MageCombatAI returned false due to ControlOrder change, process the new order
                    if (!combatHandled && m_Sidekick.Controlled && m_Sidekick.ControlOrder != OrderType.None && m_Sidekick.ControlOrder != OrderType.Attack)
            {
                        Utility.PushColor(ConsoleColor.Cyan);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - MageCombatAI stopped combat, processing new order: {m_Sidekick.ControlOrder}");
                        Utility.PopColor();
                        
                        // Clear combat state
                        m_Sidekick.Combatant = null;
                        m_Sidekick.Warmode = false;
                        Action = ActionType.Guard;
                        
                        // Process the new order
                        Obey();
                        return true;
                    }

                    // Always face target if still in combat
                    if (!DirectionLocked && m_Sidekick.Combatant != null)
                        m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);

                    return combatHandled;
                }
                else
                {
                    // For non-Mobile targets, use standard behavior
                    MoveTo(c, true, 10);
                }
            }
            // Legacy mage combat (if MageCombatAI not initialized but MageAI is)
            else if (m_MageAI != null && m_Sidekick.CombatStyle == CombatStyle.Mage)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING LEGACY MAGE COMBAT");
                Utility.PopColor();

                if (c is Mobile mobile)
                {
                    // CRITICAL: Check for healing BEFORE spell casting
                    // Heal aggressively when health drops below 60%
                    double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
                    const double healThreshold = 0.60; // Heal at 60% health
                    
                    if (healthPercent < healThreshold && !m_Sidekick.Summoned && 
                        DateTime.UtcNow > m_NextCastTime && m_Sidekick.Spell == null)
                {
                        Server.Spells.Spell healSpell = m_MageAI.GetHealSpell();
                        if (healSpell != null)
                        {
                            Utility.PushColor(ConsoleColor.Green);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Low health ({healthPercent:P0}), casting heal: {healSpell.Name}");
                            Utility.PopColor();
                            
                            healSpell.Cast();
                            var delay = healSpell.GetCastDelay() + healSpell.GetCastRecovery() + TimeSpan.FromSeconds(1.5);
                            m_NextCastTime = DateTime.UtcNow + delay;
                            
                            // If healing, face target and return (don't move)
                    if (!DirectionLocked)
                                m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                            return true;
                }
            }
                    
                    int currentDistance = (int)m_Sidekick.GetDistanceToSqrt(mobile);
                    const int castRange = 25;          // Cast spells at 20-25 tiles
                    const int minCastRange = 20;       // Minimum distance to cast
                    const int releaseRange = 10;      // Move in to 8-10 tiles to release spell
                    const int minReleaseRange = 8;     // Minimum distance to release
                    const int minSafeDistance = 5;     // If enemy within this, retreat immediately
                    const int lowManaThreshold = 40;   // Meditate if mana below this
                    
                    Utility.PushColor(ConsoleColor.Magenta);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Distance: {currentDistance}, CastRange: {minCastRange}-{castRange}, ReleaseRange: {minReleaseRange}-{releaseRange}, Mana: {m_Sidekick.Mana}/{m_Sidekick.ManaMax}, Health: {healthPercent:P0}");
                    Utility.PopColor();
                    
                    // If low on mana and far enough away, meditate
                    if (m_Sidekick.Mana < lowManaThreshold && currentDistance >= minSafeDistance && 
                        m_Sidekick.Skills[SkillName.Meditation].Value > 0 && !m_Sidekick.Meditating)
                    {
                        try
                        {
                            Utility.PushColor(ConsoleColor.Cyan);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Low mana ({m_Sidekick.Mana}/{m_Sidekick.ManaMax}), meditating to regen");
                            Utility.PopColor();
                            m_Sidekick.UseSkill(SkillName.Meditation);
                        }
                        catch (Exception ex)
                        {
                            Utility.PushColor(ConsoleColor.Red);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] Meditation error for {m_Sidekick.Name}: {ex.Message}");
                            Utility.PopColor();
                        }
                    }
                    
                    Server.Spells.Spell currentSpell = m_Sidekick.Spell as Server.Spells.Spell;
                    
                    // PHASE 1: If currently casting, freeze in place
                    if (currentSpell != null && currentSpell.IsCasting && m_Sidekick.FreezeOnCast)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Currently casting {currentSpell.Name}, freezing in place");
                        Utility.PopColor();
                        
                        if (!DirectionLocked)
                            m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                        return true;
                    }
                    
                    // PHASE 2: If spell is ready to release (Sequencing state), move in to release range
                    if (currentSpell != null && currentSpell.State == Server.Spells.SpellState.Sequencing)
                {
                        if (currentDistance < minReleaseRange || currentDistance > releaseRange)
                        {
                            Utility.PushColor(ConsoleColor.Cyan);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Spell {currentSpell.Name} ready to release, moving to release range ({minReleaseRange}-{releaseRange} tiles, current: {currentDistance})");
                            Utility.PopColor();
                            
                            // Move to optimal release range
                            int targetRange = (minReleaseRange + releaseRange) / 2; // 9 tiles
                            MoveTo(mobile, true, targetRange);
                            
                            if (!DirectionLocked)
                                m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                            return true;
            }
            else
            {
                            // In release range, stay put and let spell release
                            Utility.PushColor(ConsoleColor.Green);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - In release range ({currentDistance} tiles), spell will release");
                            Utility.PopColor();
                            
                            if (!DirectionLocked)
                                m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                            return true;
                        }
                    }
                    
                    // PHASE 3: If enemy too close, retreat immediately
                    if (currentDistance < minSafeDistance)
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Enemy too close ({currentDistance} < {minSafeDistance}), retreating immediately");
                        Utility.PopColor();
                        
                        RunFrom(mobile);
                        
                        if (!DirectionLocked)
                            m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                        return true;
                    }
                    
                    // PHASE 4: If at cast range (20-25 tiles) and no spell active, cast a spell
                    if (currentSpell == null && currentDistance >= minCastRange && currentDistance <= castRange)
                    {
                        bool cooldownCheck = DateTime.UtcNow > m_NextCastTime;
                        
                        if (cooldownCheck)
                        {
                            Server.Spells.Spell spell = null;
                            
                            if (m_Sidekick.Poisoned)
                            {
                                spell = m_MageAI.GetCureSpell();
                                Utility.PushColor(ConsoleColor.Cyan);
                                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Poisoned, attempting cure spell: {spell?.Name ?? "null"}");
                                Utility.PopColor();
                            }
                            
                            if (spell == null && c is IDamageable)
                            {
                                spell = m_MageAI.ChooseSpell(c as IDamageable);
                                Utility.PushColor(ConsoleColor.Cyan);
                                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - MageAI.ChooseSpell returned: {spell?.Name ?? "null"}");
                                Utility.PopColor();
                            }
                            
                            if (spell != null)
                            {
                                Utility.PushColor(ConsoleColor.Green);
                                Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - CASTING SPELL at {currentDistance} tiles: {spell.Name}");
                                Utility.PopColor();
                                
                                spell.Cast();
                                var delay = spell.GetCastDelay() + spell.GetCastRecovery() + TimeSpan.FromSeconds(1.5);
                                m_NextCastTime = DateTime.UtcNow + delay;
                                
                                // Reset stuck counter on successful cast
                                m_ConsecutiveRetreatFailures = 0;
                                
                                // Check if we're now casting
                                currentSpell = m_Sidekick.Spell as Server.Spells.Spell;
                                if (currentSpell != null && currentSpell.IsCasting)
                                {
                                    Utility.PushColor(ConsoleColor.Green);
                                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Spell {currentSpell.Name} is now casting, freezing");
                                    Utility.PopColor();
                                    
                                    if (!DirectionLocked)
                                        m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                                    return true;
                                }
                            }
                        }
                    }
                    
                    // PHASE 5: Movement logic based on distance with stuck detection
                    if (currentDistance < minCastRange)
                    {
                        // Too close to cast - retreat to cast range
                        // Check if we're stuck (can't retreat further)
                        bool isStuck = false;
                        if (m_LastRetreatAttempt != DateTime.MinValue && 
                            (DateTime.UtcNow - m_LastRetreatAttempt).TotalSeconds < 2.0)
                        {
                            // Recent retreat attempt - check if distance increased
                            if (currentDistance <= m_LastRetreatDistance)
                            {
                                m_ConsecutiveRetreatFailures++;
                                if (m_ConsecutiveRetreatFailures >= 3)
                                {
                                    isStuck = true;
                                    Utility.PushColor(ConsoleColor.Red);
                                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - STUCK DETECTED: Can't retreat further (distance: {currentDistance}, failures: {m_ConsecutiveRetreatFailures})");
                                    Utility.PopColor();
                                }
                            }
                            else
                            {
                                // Distance increased - reset stuck counter
                                m_ConsecutiveRetreatFailures = 0;
                            }
                        }
                        
                        // If stuck and at minimum safe distance (8+ tiles), allow casting
                        if (isStuck && currentDistance >= minReleaseRange)
                        {
                            Utility.PushColor(ConsoleColor.Yellow);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - STUCK MODE: Allowing cast at {currentDistance} tiles (minimum: {minReleaseRange})");
                            Utility.PopColor();
                            
                            // Try to cast at current distance
                            if (currentSpell == null && DateTime.UtcNow > m_NextCastTime)
                            {
                                Server.Spells.Spell spell = null;
                                
                                if (m_Sidekick.Poisoned)
                                {
                                    spell = m_MageAI.GetCureSpell();
                                }
                                
                                if (spell == null && c is IDamageable)
                                {
                                    spell = m_MageAI.ChooseSpell(c as IDamageable);
                                }
                                
                                if (spell != null)
                                {
                                    Utility.PushColor(ConsoleColor.Green);
                                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - CASTING SPELL (STUCK MODE) at {currentDistance} tiles: {spell.Name}");
                                    Utility.PopColor();
                                    
                                    spell.Cast();
                                    var delay = spell.GetCastDelay() + spell.GetCastRecovery() + TimeSpan.FromSeconds(1.5);
                                    m_NextCastTime = DateTime.UtcNow + delay;
                                    
                                    // Reset stuck counter on successful cast
                                    m_ConsecutiveRetreatFailures = 0;
                                    
                                    currentSpell = m_Sidekick.Spell as Server.Spells.Spell;
                                    if (currentSpell != null && currentSpell.IsCasting)
                                    {
                                        if (!DirectionLocked)
                                            m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                                        return true;
                                    }
                                }
                            }
                        }
                        else if (!isStuck)
                        {
                            // Not stuck - try to retreat
                            Utility.PushColor(ConsoleColor.Yellow);
                            Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Too close to cast ({currentDistance} < {minCastRange}), retreating to cast range");
                            Utility.PopColor();
                            
                            m_LastRetreatDistance = currentDistance;
                            m_LastRetreatAttempt = DateTime.UtcNow;
                            RunFrom(mobile);
                        }
                    }
                    else if (currentDistance > castRange)
                    {
                        // Too far - move into cast range
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - Too far to cast ({currentDistance} > {castRange}), moving to cast range");
                        Utility.PopColor();
                        
                        // Reset stuck counter when moving closer
                        m_ConsecutiveRetreatFailures = 0;
                        
                        int targetRange = (minCastRange + castRange) / 2; // 22-23 tiles
                        MoveTo(mobile, true, targetRange);
                    }
                    else
                    {
                        // In cast range - stay put (will cast in next phase)
                        // Reset stuck counter when in ideal range
                        m_ConsecutiveRetreatFailures = 0;
                        
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - In cast range ({currentDistance} tiles), staying put");
                        Utility.PopColor();
                    }
                    
                    // Always face target
                    if (!DirectionLocked)
                        m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                    
                    return true;
                }
                else
                {
                    // For non-Mobile targets, use standard behavior
                    MoveTo(c, true, 10);
                }
            }
            // Use archetype-specific combat AI
            else if (c is Mobile mobile)
            {
                // Check if enemy is dead/gone
                if (mobile.Deleted || !mobile.Alive || mobile.IsDeadBondedPet)
                {
                    HandleCombatEnd("Enemy dead/gone");
                    return true;
                }
                
                bool combatHandled = false;
                
                // Route to appropriate combat AI
                if (m_WarriorCombatAI != null)
                {
                    Utility.PushColor(ConsoleColor.DarkYellow);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING WARRIOR COMBAT AI");
                    Utility.PopColor();
                    combatHandled = m_WarriorCombatAI.DoCombat(mobile);
                }
                else if (m_ArcherCombatAI != null)
                {
                    Utility.PushColor(ConsoleColor.DarkGreen);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING ARCHER COMBAT AI");
                    Utility.PopColor();
                    combatHandled = m_ArcherCombatAI.DoCombat(mobile);
                }
                else if (m_ThiefCombatAI != null)
                {
                    Utility.PushColor(ConsoleColor.DarkGray);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING THIEF COMBAT AI");
                    Utility.PopColor();
                    combatHandled = m_ThiefCombatAI.DoCombat(mobile);
                }
                else if (m_PaladinCombatAI != null)
                {
                    Utility.PushColor(ConsoleColor.White);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING PALADIN COMBAT AI");
                    Utility.PopColor();
                    combatHandled = m_PaladinCombatAI.DoCombat(mobile);
                }
                else if (m_HealerCombatAI != null)
                {
                    Utility.PushColor(ConsoleColor.Cyan);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING HEALER COMBAT AI");
                    Utility.PopColor();
                    combatHandled = m_HealerCombatAI.DoCombat(mobile);
                }
                else if (m_TamerCombatAI != null)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING TAMER COMBAT AI");
                    Utility.PopColor();
                    combatHandled = m_TamerCombatAI.DoCombat(mobile);
                }
                else if (m_NecromancerCombatAI != null)
                {
                    Utility.PushColor(ConsoleColor.DarkMagenta);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING NECROMANCER COMBAT AI");
                    Utility.PopColor();
                    combatHandled = m_NecromancerCombatAI.DoCombat(mobile);
                }
                else
                {
                    // Fallback to basic melee combat
                    Utility.PushColor(ConsoleColor.Gray);
                    Console.WriteLine($"[SidekickAI.DoActionCombat] {m_Sidekick.Name} - USING BASIC MELEE (no combat AI)");
                    Utility.PopColor();
                    
                    int combatRange = m_Sidekick.RangeFight;
                    if (m_Sidekick.CombatStyle != CombatStyle.Melee)
                    {
                        combatRange = GetOptimalRange();
                    }
                    
                    MoveTo(c, true, combatRange);
                    combatHandled = true;
                }
                
                // Check if enemy died during combat
                if (mobile.Deleted || !mobile.Alive || mobile.IsDeadBondedPet)
                {
                    HandleCombatEnd("Enemy died during combat");
                    return true;
                }
                
                // Always face target if still in combat
                if (!DirectionLocked && m_Sidekick.Combatant != null)
                    m_Sidekick.Direction = m_Sidekick.GetDirectionTo(mobile);
                
                return combatHandled;
            }
            else
            {
                // For non-Mobile targets, use basic movement
                int combatRange = m_Sidekick.RangeFight;
                if (m_Sidekick.CombatStyle != CombatStyle.Melee)
                {
                    combatRange = GetOptimalRange();
                }
                MoveTo(c, true, combatRange);
            }

            return true;
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[SidekickAI.DoActionCombat] CRITICAL ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Utility.PopColor();
                
                // Force exit combat on error to prevent stuck state
                m_Sidekick.Combatant = null;
                m_Sidekick.Warmode = false;
                Action = ActionType.Wander;
                return true;
            }
        }

        /// <summary>
        /// Process pending spell targets - invokes the target with the combatant to apply spell effects/damage
        /// This is critical for NPC spell casting - without this, spells are initiated but never actually applied!
        /// </summary>
        private void ProcessSpellTarget()
        {
            try
            {
                var targ = m_Sidekick.Target;
                
                if (targ == null)
                    return;
                
                Utility.PushColor(ConsoleColor.Cyan);
                Console.WriteLine($"[SidekickAI.ProcessSpellTarget] {m_Sidekick.Name} - Processing target: {targ.GetType().Name}");
                Utility.PopColor();

                // Check for Teleport spell target - handle via MageCombatAI
                if (targ is Server.Spells.Third.TeleportSpell.InternalTarget && m_MageCombatAI != null)
                {
                    if (m_MageCombatAI.ProcessTeleportTarget())
                    {
                        Utility.PushColor(ConsoleColor.Cyan);
                        Console.WriteLine($"[SidekickAI.ProcessSpellTarget] {m_Sidekick.Name} - Teleport target processed by MageCombatAI");
                        Utility.PopColor();
                        return;
                    }
                }
                
                // For harmful spells, target the combatant
                bool harmful = (targ.Flags & Server.Targeting.TargetFlags.Harmful) != 0;
                bool beneficial = (targ.Flags & Server.Targeting.TargetFlags.Beneficial) != 0;
                
                if (harmful)
                {
                    var toTarget = m_Sidekick.Combatant;
                    
                    // Validate target is alive, not deleted, and can be harmed
                    if (toTarget != null && !toTarget.Deleted && toTarget.Alive && m_Sidekick.CanBeHarmful(toTarget, false))
                    {
                        string targetName = (toTarget is Mobile mob) ? mob.Name : "Unknown";
                        
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.ProcessSpellTarget] {m_Sidekick.Name} - Invoking HARMFUL target on: {targetName}");
                        Utility.PopColor();
                        
                        targ.Invoke(m_Sidekick, toTarget);
                    }
                    else
                    {
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine($"[SidekickAI.ProcessSpellTarget] {m_Sidekick.Name} - No valid combatant for harmful spell, canceling");
                        Utility.PopColor();
                        
                        targ.Cancel(m_Sidekick, Server.Targeting.TargetCancelType.Canceled);
                    }
                }
                else if (beneficial)
                {
                    // For beneficial spells, check if Tamer has a pending heal target (pet)
                    Mobile healTarget = m_Sidekick;

                    if (m_TamerCombatAI != null && m_TamerCombatAI.PendingHealTarget != null)
                    {
                        var pendingTarget = m_TamerCombatAI.PendingHealTarget;
                        if (!pendingTarget.Deleted && pendingTarget.Alive)
                        {
                            healTarget = pendingTarget;
                            Utility.PushColor(ConsoleColor.Green);
                            Console.WriteLine($"[SidekickAI.ProcessSpellTarget] {m_Sidekick.Name} - Invoking BENEFICIAL target on PET: {healTarget.Name}");
                            Utility.PopColor();
                        }
                        // Clear the pending target after use
                        m_TamerCombatAI.PendingHealTarget = null;
                    }
                    else
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.ProcessSpellTarget] {m_Sidekick.Name} - Invoking BENEFICIAL target on self");
                        Utility.PopColor();
                    }

                    targ.Invoke(m_Sidekick, healTarget);
                }
                else
                {
                    // For other spells (fields, summons, etc.), target at combatant's location
                    var toTarget = m_Sidekick.Combatant;
                    
                    if (toTarget != null && !toTarget.Deleted)
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.ProcessSpellTarget] {m_Sidekick.Name} - Invoking LOCATION target at: {toTarget.Location}");
                        Utility.PopColor();
                        
                        targ.Invoke(m_Sidekick, toTarget.Location);
                    }
                    else
                    {
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine($"[SidekickAI.ProcessSpellTarget] {m_Sidekick.Name} - No combatant, canceling target");
                        Utility.PopColor();
                        
                        targ.Cancel(m_Sidekick, Server.Targeting.TargetCancelType.Canceled);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[SidekickAI.ProcessSpellTarget] ERROR for {m_Sidekick.Name}: {ex.Message}");
                Console.WriteLine($"  Stack: {ex.StackTrace}");
                Utility.PopColor();
                
                // Clear the target to prevent repeated crashes
                if (m_Sidekick.Target != null)
                {
                    try
                    {
                        m_Sidekick.Target.Cancel(m_Sidekick, Server.Targeting.TargetCancelType.Canceled);
                    }
                    catch
                    {
                        // Ignore secondary exceptions
                    }
                }
            }
        }

        /// <summary>
        /// Handle guard action - check for orders like DoActionWander does
        /// </summary>
        public override bool DoActionGuard()
        {
            Utility.PushColor(ConsoleColor.Blue);
            Console.WriteLine($"[SidekickAI.DoActionGuard] {m_Sidekick?.Name ?? "null"} - ControlOrder: {m_Sidekick?.ControlOrder ?? OrderType.None}, Controlled: {m_Sidekick?.Controlled ?? false}");
            Utility.PopColor();
            
            // CRITICAL: Check for ControlOrder FIRST - if there's an order, process it
            // This allows commands to work even when Action is Guard (after combat ends)
            if (m_Sidekick != null && m_Sidekick.Controlled && m_Sidekick.ControlOrder != OrderType.None)
            {
                bool orderProcessed = Obey();
                if (orderProcessed)
                {
                    return true; // Order was processed
                }
            }

            // Check for new aggressors (use same logic as DoActionWander)
            Mobile aggressor = FindNearestAggressor();
            if (aggressor != null)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[SidekickAI.DoActionGuard] {m_Sidekick.Name} - DETECTED AGGRESSOR: {aggressor.Name}, engaging combat");
                Utility.PopColor();
                
                m_Sidekick.Combatant = aggressor;
                Action = ActionType.Combat;
                return true;
            }

            // Use base guard behavior
            return base.DoActionGuard();
        }

        // No need to override Obey, EndPickTarget, or OnSpeech anymore
        // BaseAI works directly with m_Mobile which is now the sidekick (BaseCreature)

        public override bool DoActionWander()
        {
            Utility.PushColor(ConsoleColor.Gray);
            Console.WriteLine($"[SidekickAI.DoActionWander] {m_Sidekick?.Name ?? "null"} - ControlOrder: {m_Sidekick?.ControlOrder ?? OrderType.None}");
            Utility.PopColor();
            
            // Check for ControlOrder FIRST - if there's an order, process it
            if (m_Sidekick != null && m_Sidekick.Controlled && m_Sidekick.ControlOrder != OrderType.None)
            {
                bool orderProcessed = Obey();
                if (orderProcessed)
                {
                    return true; // Order was processed, don't do autonomous behavior
                }
            }

            // First, check for aggressors attacking owner or sidekick
            Mobile aggressor = FindNearestAggressor();
            if (aggressor != null)
                {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"[SidekickAI.DoActionWander] {m_Sidekick.Name} - DETECTED AGGRESSOR: {aggressor.Name}, engaging combat");
                Utility.PopColor();
                
                m_Sidekick.Combatant = aggressor;
                    Action = ActionType.Combat;
                    return true;
                }

            // Make autonomous decisions
            if (m_Sidekick != null)
            {
                MakeAutonomousDecision();
            }

            // Standard wander - BaseCreature doesn't have AcquireFocusMob, use manual check
            // FocusMob acquisition is handled by autonomous decision making above

            return DoActionWanderInternal();
        }

        private bool DoActionWanderInternal()
        {
             // Re-implement basic wander logic using m_Sidekick
             if (CheckMove())
             {
                 if (!m_Sidekick.CheckIdle())
                 {
                     // WalkRandom(2, 2, 1); // Replaced with direct call
                     WalkRandomInHome(2, 2, 1);
                 }
             }
             return true;
        }

        public override void WalkRandomInHome(int iChanceToNotMove, int iChanceToDir, int iSteps)
        {
            // Simplified random walk
             if (m_Sidekick.Deleted || !m_Sidekick.CanMove)
                return;

             for (var i = 0; i < iSteps; i++)
             {
                 if (Utility.Random(8 * iChanceToNotMove) <= 8)
                 {
                     int rnd = Utility.Random(8);
                     DoMove((Direction)rnd, false);
                 }
             }
        }

        /// <summary>
        /// Find nearest aggressor attacking owner or sidekick
        /// </summary>
        // Track last combatant for re-engagement when they return
        private Mobile m_LastCombatant = null;
        private DateTime m_LastCombatantTime = DateTime.MinValue;
        private const double LAST_COMBATANT_MEMORY_SECONDS = 30.0; // Remember last combatant for 30 seconds

        public Mobile FindNearestAggressor()
        {
            if (m_Sidekick == null || m_Sidekick.Deleted)
                return null;

            Mobile nearestAggressor = null;
            double nearestDistance = double.MaxValue;

            // Use larger perception range for mages (they need to detect returning combatants from further away)
            int perceptionRange = m_Sidekick.RangePerception;
            if (m_Sidekick is AutonomousSidekick autoSidekick && autoSidekick.CombatStyle == CombatStyle.Mage)
            {
                perceptionRange = 18; // Increased perception for mages to catch returning combatants
            }

            // CRITICAL: Check if our last combatant has returned to LOS within spell range
            // This handles the "player runs off then returns" PvP scenario
            if (m_LastCombatant != null && !m_LastCombatant.Deleted && m_LastCombatant.Alive &&
                (DateTime.UtcNow - m_LastCombatantTime).TotalSeconds < LAST_COMBATANT_MEMORY_SECONDS)
            {
                if (m_Sidekick.CanSee(m_LastCombatant) && m_Sidekick.InLOS(m_LastCombatant) &&
                    m_Sidekick.CanBeHarmful(m_LastCombatant, false))
                {
                    double dist = m_Sidekick.GetDistanceToSqrt(m_LastCombatant);
                    if (dist <= 14) // Within spell range - re-engage immediately
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.FindNearestAggressor] {m_Sidekick.Name} - LAST COMBATANT RETURNED: {m_LastCombatant.Name} at {dist:F0} tiles - re-engaging!");
                        Utility.PopColor();
                        return m_LastCombatant;
                    }
                }
            }

            // CRITICAL: Check sidekick's own Aggressors list first (creatures attacking the sidekick)
            if (m_Sidekick.Aggressors != null && m_Sidekick.Aggressors.Count > 0)
            {
                foreach (AggressorInfo info in m_Sidekick.Aggressors)
                {
                    Mobile attacker = info.Attacker;
                    if (attacker != null && !attacker.Deleted && attacker.Alive && attacker != m_Sidekick &&
                        m_Sidekick.CanSee(attacker) && m_Sidekick.CanBeHarmful(attacker, false))
                    {
                        double dist = m_Sidekick.GetDistanceToSqrt(attacker);
                        if (dist <= perceptionRange && dist < nearestDistance)
                        {
                            nearestAggressor = attacker;
                            nearestDistance = dist;
                        }
                    }
                }
            }

            // Check sidekick's Aggressed list (creatures the sidekick has attacked)
            if (m_Sidekick.Aggressed != null && m_Sidekick.Aggressed.Count > 0)
            {
                foreach (AggressorInfo info in m_Sidekick.Aggressed)
                {
                    Mobile defender = info.Defender;
                    if (defender != null && !defender.Deleted && defender.Alive && defender != m_Sidekick &&
                        m_Sidekick.CanSee(defender) && m_Sidekick.CanBeHarmful(defender, false))
                    {
                        double dist = m_Sidekick.GetDistanceToSqrt(defender);
                        if (dist <= perceptionRange && dist < nearestDistance)
                        {
                            nearestAggressor = defender;
                            nearestDistance = dist;
                        }
                    }
                }
            }

            // Check for creatures attacking the owner
            if (m_Sidekick.Owner != null && !m_Sidekick.Owner.Deleted)
            {
            if (m_Sidekick.Owner.Combatant != null && m_Sidekick.Owner.Combatant.Alive)
            {
                Mobile ownerTarget = m_Sidekick.Owner.Combatant as Mobile;
                    if (ownerTarget != null && m_Sidekick.CanBeHarmful(ownerTarget, false) && m_Sidekick.CanSee(ownerTarget))
                {
                    double dist = m_Sidekick.GetDistanceToSqrt(ownerTarget);
                        if (dist <= perceptionRange && dist < nearestDistance)
                    {
                        nearestAggressor = ownerTarget;
                        nearestDistance = dist;
                    }
                }
            }

                // Check owner's Aggressors list
                if (m_Sidekick.Owner.Aggressors != null && m_Sidekick.Owner.Aggressors.Count > 0)
            {
                    foreach (AggressorInfo info in m_Sidekick.Owner.Aggressors)
                {
                        Mobile attacker = info.Attacker;
                        if (attacker != null && !attacker.Deleted && attacker.Alive && attacker != m_Sidekick &&
                            m_Sidekick.CanSee(attacker) && m_Sidekick.CanBeHarmful(attacker, false))
                    {
                            double dist = m_Sidekick.GetDistanceToSqrt(attacker);
                            if (dist <= perceptionRange && dist < nearestDistance)
                            {
                                nearestAggressor = attacker;
                        nearestDistance = dist;
                            }
                        }
                    }
                }
            }

            // Check nearby mobiles for hostile creatures (using FightMode.Closest logic)
            IPooledEnumerable eable = m_Sidekick.GetMobilesInRange(perceptionRange);
            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || !m.Alive || m == m_Sidekick || m == m_Sidekick.Owner)
                    continue;

                // Check if this mobile is attacking owner or sidekick
                bool isAttackingOwner = (m_Sidekick.Owner != null && m.Combatant == m_Sidekick.Owner);
                bool isAttackingSidekick = (m.Combatant == m_Sidekick);
                
                // Also check if it's a hostile creature (FightMode.Closest behavior)
                bool isHostile = false;
                if (m is BaseCreature bc && !bc.Controlled && !bc.Summoned)
                {
                    // Check if creature is hostile to us
                    isHostile = bc.IsEnemy(m_Sidekick) || bc.IsEnemy(m_Sidekick.ControlMaster);
                }
                
                if ((isAttackingOwner || isAttackingSidekick || isHostile) && m_Sidekick.CanBeHarmful(m, false) && m_Sidekick.CanSee(m))
                {
                    double dist = m_Sidekick.GetDistanceToSqrt(m);
                    if (dist < nearestDistance)
                    {
                        nearestAggressor = m;
                        nearestDistance = dist;
                    }
                }
            }
            eable.Free();

            if (nearestAggressor != null)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[SidekickAI.FindNearestAggressor] {m_Sidekick.Name} - Found aggressor: {nearestAggressor.Name} at distance {nearestDistance:F1}");
                Utility.PopColor();
            }

            return nearestAggressor;
        }

        /// <summary>
        /// Find what enemies our pets (or owner's pets) are currently fighting
        /// Used by support classes (Tamer/Healer) to detect ongoing combat when primary target dies
        /// </summary>
        private Mobile FindPetCombatTarget()
        {
            Mobile owner = m_Sidekick.ControlMaster ?? m_Sidekick.Owner;

            // First, check pets owned by this sidekick (Tamer's dragon)
            var nearbyMobiles = m_Sidekick.GetMobilesInRange(20);
            foreach (Mobile m in nearbyMobiles)
            {
                if (m is BaseCreature bc && bc.ControlMaster == m_Sidekick && bc.Alive && !bc.Deleted)
                {
                    // This is our pet - check what it's fighting
                    if (bc.Combatant is Mobile petTarget && petTarget.Alive && !petTarget.Deleted)
                    {
                        nearbyMobiles.Free();
                        Utility.PushColor(ConsoleColor.Cyan);
                        Console.WriteLine($"[SidekickAI.FindPetCombatTarget] {m_Sidekick.Name} - My pet {bc.Name} is fighting {petTarget.Name}");
                        Utility.PopColor();
                        return petTarget;
                    }

                    // Also check pet's aggressors
                    if (bc.Aggressors != null && bc.Aggressors.Count > 0)
                    {
                        foreach (AggressorInfo info in bc.Aggressors)
                        {
                            if (info.Attacker != null && !info.Attacker.Deleted && info.Attacker.Alive)
                            {
                                nearbyMobiles.Free();
                                Utility.PushColor(ConsoleColor.Cyan);
                                Console.WriteLine($"[SidekickAI.FindPetCombatTarget] {m_Sidekick.Name} - My pet {bc.Name} is being attacked by {info.Attacker.Name}");
                                Utility.PopColor();
                                return info.Attacker;
                            }
                        }
                    }
                }
            }
            nearbyMobiles.Free();

            // Second, check owner's pets (for Healer supporting owner's pets)
            if (owner is PlayerMobile pm)
            {
                foreach (Mobile m in pm.AllFollowers)
                {
                    if (m == m_Sidekick || m.Deleted || !m.Alive)
                        continue;

                    if (m is BaseCreature bc)
                    {
                        // Check what this pet is fighting
                        if (bc.Combatant is Mobile petTarget && petTarget.Alive && !petTarget.Deleted)
                        {
                            Utility.PushColor(ConsoleColor.Cyan);
                            Console.WriteLine($"[SidekickAI.FindPetCombatTarget] {m_Sidekick.Name} - Owner's pet {bc.Name} is fighting {petTarget.Name}");
                            Utility.PopColor();
                            return petTarget;
                        }

                        // Also check pet's aggressors
                        if (bc.Aggressors != null && bc.Aggressors.Count > 0)
                        {
                            foreach (AggressorInfo info in bc.Aggressors)
                            {
                                if (info.Attacker != null && !info.Attacker.Deleted && info.Attacker.Alive)
                                {
                                    Utility.PushColor(ConsoleColor.Cyan);
                                    Console.WriteLine($"[SidekickAI.FindPetCombatTarget] {m_Sidekick.Name} - Owner's pet {bc.Name} is being attacked by {info.Attacker.Name}");
                                    Utility.PopColor();
                                    return info.Attacker;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Handle follow order - CRITICAL: Called from Obey() for controlled pets
        /// Must handle movement directly
        /// </summary>
        public override bool DoOrderFollow()
        {
            // Process any pending spell targets (needed for recovery healing spells)
            if (m_Sidekick != null && m_Sidekick.Target != null)
            {
                ProcessSpellTarget();
            }

            // PRIORITY 0: Tamer - heal pets while following (post-combat recovery)
            if (m_Sidekick != null && m_TamerCombatAI != null)
            {
                // Call TamerCombatAI.DoCombat with null enemy to trigger recovery mode
                if (m_TamerCombatAI.DoCombat(null))
                {
                    // TamerCombatAI is handling pet recovery - don't interrupt
                    // But still need to follow owner, so don't return true
                }
            }

            // PRIORITY 1: Check if we need healing while following (ALL archetypes)
            if (m_Sidekick != null && HasCombatAI())
            {
                double healthPercent = (double)m_Sidekick.Hits / (double)m_Sidekick.HitsMax;
                const double LOW_HEALTH = 0.70; // 70% threshold

                // Heal if below LOW_HEALTH threshold
                if (healthPercent < LOW_HEALTH && m_Sidekick.Hits < m_Sidekick.HitsMax)
                {
                    // Try bandage first (mana efficient) - works for all archetypes
                    if (!m_Sidekick.Poisoned && TryUseBandageFromCombatAI())
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.DoOrderFollow] {m_Sidekick.Name} - Healing while following ({healthPercent:P0} health), using bandage");
                        Utility.PopColor();
                    }
                    // Try Greater Heal spell if have magery and bandage on cooldown
                    else if (m_Sidekick.Skills.Magery.Value >= 50 && m_Sidekick.Mana >= 11)
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.DoOrderFollow] {m_Sidekick.Name} - Healing while following ({healthPercent:P0} health), casting Greater Heal");
                        Utility.PopColor();

                        var healSpell = new Server.Spells.Fourth.GreaterHealSpell(m_Sidekick, null);
                        healSpell.Cast();
                    }
                }

                // Cure poison if poisoned
                if (m_Sidekick.Poisoned)
                {
                    // Try cure spell if have magery
                    if (m_Sidekick.Skills.Magery.Value >= 40 && m_Sidekick.Mana >= 6)
                    {
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[SidekickAI.DoOrderFollow] {m_Sidekick.Name} - Curing poison while following");
                        Utility.PopColor();

                        var cureSpell = new Server.Spells.Second.CureSpell(m_Sidekick, null);
                        cureSpell.Cast();
                    }
                    // Otherwise potions are handled by combat AI during combat
                }
            }

            // PRIORITY 2: Check for nearby threats and engage them proactively
            if (m_Sidekick != null && m_Sidekick.ControlOrder == OrderType.Follow)
            {
                Mobile nearestThreat = FindNearestThreat();
                if (nearestThreat != null)
                {
                    int distanceToThreat = (int)m_Sidekick.GetDistanceToSqrt(nearestThreat);

                    // Engage if threat is within 12 tiles (mage engagement range)
                    if (distanceToThreat <= 12)
                    {
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine($"[SidekickAI.DoOrderFollow] {m_Sidekick.Name} - Detected threat: {nearestThreat.Name} at {distanceToThreat} tiles, engaging!");
                        Utility.PopColor();

                        m_Sidekick.Combatant = nearestThreat;
                        m_Sidekick.ControlTarget = nearestThreat;
                        m_Sidekick.ControlOrder = OrderType.Attack;
                        m_Sidekick.Warmode = true;
                        Action = ActionType.Combat;
                        return true;
                    }
                }
            }
            
            // Use Owner for following - ControlTarget may still point to dead enemy
            // When ControlOrder is Follow, we should always follow the owner (ControlMaster)
            Mobile followTarget = m_Sidekick?.ControlMaster ?? m_Sidekick?.Owner;

            // Also ensure ControlTarget is set correctly for Follow order
            if (m_Sidekick != null && m_Sidekick.ControlOrder == OrderType.Follow && followTarget != null)
            {
                m_Sidekick.ControlTarget = followTarget;
            }

            if (m_Sidekick != null && followTarget != null && !followTarget.Deleted)
            {
                if (followTarget.Map != m_Sidekick.Map)
                {
                    // Target on different map
                    return false;
                }

                int distance = (int)m_Sidekick.GetDistanceToSqrt(followTarget);

                // If very far (> 50 tiles), cast Recall to catch up
                if (distance > 50)
                {
                    // Check if we already recently recalled to avoid spamming if blocked
                    if (DateTime.UtcNow - m_LastRecallTime > TimeSpan.FromSeconds(2))
                    {
                         // Simulate Recall
                        m_Sidekick.Say("Kal Ort Por");
                        m_Sidekick.PlaySound(0x1FC);
                        m_Sidekick.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                        
                        // Instant teleport for responsiveness (or could be delayed slightly)
                        m_Sidekick.MoveToWorld(followTarget.Location, followTarget.Map);
                        m_Sidekick.PlaySound(0x1FC);
                        m_LastRecallTime = DateTime.UtcNow;
                    }
                        return true;
                }

                // Maintain follow distance
                if (distance > m_Sidekick.FollowDistance)
                {
                    // Run if distance is greater than normal walk follow range
                    bool shouldRun = distance > 5;
                    
                    if (WalkMobileRange(followTarget, m_Sidekick.FollowDistance, shouldRun, 0, 1))
                    {
                        return true;
                    }
                    else
                    {
                        // If pathfinding failed and we are far, try to run directly
                        if (distance > 10)
                        {
                            RunTo(followTarget);
                            return true;
                        }
                    }
                }

                // If we're at the right distance, just face the target
                if (distance <= m_Sidekick.FollowDistance)
                {
                    m_Sidekick.Direction = m_Sidekick.GetDirectionTo(followTarget);
                }

                // Set Action to Wander so it continues processing
                Action = ActionType.Wander;
                return true;
            }

            // Set Action to Wander so it continues processing
            Action = ActionType.Wander;
            return base.DoOrderFollow();
        }

        /// <summary>
        /// Find the nearest hostile mobile that could be a threat
        /// </summary>
        private Mobile FindNearestThreat()
        {
            if (m_Sidekick == null || m_Sidekick.Map == null)
                return null;

            Mobile nearestThreat = null;
            int nearestDistance = int.MaxValue;

            // Search for nearby hostile mobiles
            IPooledEnumerable eable = m_Sidekick.Map.GetMobilesInRange(m_Sidekick.Location, 12);

            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted || m == m_Sidekick || !m.Alive)
                    continue;

                // Skip friendly targets (owner, other players in guild, etc.)
                if (m == m_Sidekick.ControlMaster || m == m_Sidekick.Owner)
                    continue;

                // Skip other sidekicks with same owner
                if (m is AutonomousSidekick otherSidekick && otherSidekick.ControlMaster == m_Sidekick.ControlMaster)
                    continue;

                // Skip players (unless they're aggressive)
                if (m is PlayerMobile pm)
                {
                    // Only engage players who have attacked us
                    bool isAggressor = false;
                    if (m_Sidekick.Aggressors != null)
                    {
                        foreach (AggressorInfo info in m_Sidekick.Aggressors)
                        {
                            if (info.Attacker == pm)
                            {
                                isAggressor = true;
                                break;
                            }
                        }
                    }
                    if (!isAggressor)
                        continue;
                }

                // Check if this is a hostile creature
                if (m is BaseCreature bc)
                {
                    // Skip tamed/controlled creatures
                    if (bc.Controlled || bc.Summoned)
                        continue;

                    // Must be hostile or already attacking us/owner
                    bool isHostile = bc.IsEnemy(m_Sidekick) || bc.IsEnemy(m_Sidekick.ControlMaster);
                    bool isAttackingUs = bc.Combatant == m_Sidekick || bc.Combatant == m_Sidekick.ControlMaster;

                    if (!isHostile && !isAttackingUs)
                        continue;
                }

                // Check line of sight
                if (!m_Sidekick.CanSee(m) || !m_Sidekick.InLOS(m))
                    continue;

                // Check if we can harm them
                if (!m_Sidekick.CanBeHarmful(m, false))
                    continue;

                int distance = (int)m_Sidekick.GetDistanceToSqrt(m);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestThreat = m;
                }
            }

            eable.Free();
            return nearestThreat;
        }

        /// <summary>
        /// Make autonomous decision based on situation
        /// </summary>
        private void MakeAutonomousDecision()
        {
            if (m_Sidekick == null)
                return;

            // Check if enough time has passed since last decision
            if (DateTime.UtcNow - m_Sidekick.LastDecisionTime < m_Sidekick.DecisionInterval)
                return;

            m_Sidekick.LastDecisionTime = DateTime.UtcNow;

            // Assess situation
            SituationAssessment situation = SidekickGoalSystem.AssessSituation(m_Sidekick);

            // Score goals
            var goalScores = SidekickGoalSystem.ScoreGoals(m_Sidekick, situation);

            // Select best goal
            SidekickGoal newGoal = SidekickGoalSystem.SelectBestGoal(goalScores);

            // Execute goal if changed
            if (newGoal != m_Sidekick.CurrentGoal)
            {
                m_Sidekick.CurrentGoal = newGoal;
                SidekickGoalSystem.ExecuteGoal(m_Sidekick, newGoal, situation);
            }
            else
            {
                // Continue executing current goal
                SidekickGoalSystem.ExecuteGoal(m_Sidekick, newGoal, situation);
            }
        }

        /// <summary>
        /// Get optimal combat range
        /// </summary>
        private int GetOptimalRange()
        {
            if (m_Sidekick == null)
                return 1;

            switch (m_Sidekick.CombatStyle)
            {
                case CombatStyle.Melee:
                    return 1;
                case CombatStyle.Archer:
                    return 5;
                case CombatStyle.Mage:
                    return 10;
                case CombatStyle.Hybrid:
                    return 5;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Get direction away from target
        /// </summary>
        private Direction GetDirectionAwayFrom(IDamageable target)
        {
            if (target is Mobile m)
            {
                Direction toward = m_Sidekick.GetDirectionTo(m);
                // Reverse direction
                return (Direction)(((int)toward + 4) % 8);
            }
            return m_Sidekick.Direction;
        }

        /// <summary>
        /// Run to a target
        /// </summary>
        private void RunTo(Mobile target)
        {
            if (target == null || target.Deleted)
                return;

            m_Sidekick.Direction = m_Sidekick.GetDirectionTo(target);
            if (m_Sidekick.CanMove)
            {
                m_Sidekick.Move(m_Sidekick.Direction);
            }
        }

        /// <summary>
        /// Override attack order - CRITICAL: For controlled pets, Obey() is called instead of Think()
        /// So we must handle combat directly here, not just set Action
        /// </summary>
        public override bool DoOrderAttack()
        {
            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine($"[SidekickAI.DoOrderAttack] {m_Sidekick.Name} - ControlTarget: {m_Sidekick.ControlTarget}, ControlOrder: {m_Sidekick.ControlOrder}, Current Combatant: {m_Sidekick.Combatant?.Name ?? "null"}");
            Utility.PopColor();
            
            // CRITICAL: Controlled pets call Obey() from AI timer, NOT Think()
            // So DoOrderAttack must handle combat directly by calling DoActionCombat()
            
            if (m_Sidekick.ControlTarget != null && m_Sidekick.ControlTarget is Mobile target && target.Alive)
            {
                if (m_Sidekick.CanBeHarmful(target, false))
                {
                    // CRITICAL FIX: Support classes (Tamer, Healer) should NOT have Combatant set
                    // Setting Combatant causes the base AI to try to move toward the enemy
                    // which conflicts with their support behavior (staying near pet/allies)
                    bool isSupportClass = m_TamerCombatAI != null || m_HealerCombatAI != null;

                    if (!isSupportClass)
                    {
                        // If already in combat with a different target, switch targets
                        if (m_Sidekick.Combatant != null && m_Sidekick.Combatant != target)
                        {
                            Utility.PushColor(ConsoleColor.Yellow);
                            Console.WriteLine($"[SidekickAI.DoOrderAttack] {m_Sidekick.Name} - Switching target from {m_Sidekick.Combatant.Name} to {target.Name}");
                            Utility.PopColor();
                        }

                        m_Sidekick.Combatant = target;
                    }
                    else
                    {
                        // Support class - clear Combatant to prevent unwanted movement toward enemy
                        m_Sidekick.Combatant = null;
                        Utility.PushColor(ConsoleColor.Cyan);
                        Console.WriteLine($"[SidekickAI.DoOrderAttack] {m_Sidekick.Name} - SUPPORT CLASS: Not setting Combatant (letting combat AI handle positioning)");
                        Utility.PopColor();
                    }
                    
                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine($"[SidekickAI.DoOrderAttack] {m_Sidekick.Name} - ATTACKING {target.Name}, Setting Warmode=true, Action=Combat, CALLING DoActionCombat");
                    Utility.PopColor();
                    
                    // Force combat mode immediately
                    m_Sidekick.Warmode = true;
                        Action = ActionType.Combat;
                    
                    // CRITICAL: For controlled pets, we must call DoActionCombat directly
                    // because Think() is never called - only Obey() is called
                    DoActionCombat();
                    
                    return true;
                }
                else
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.WriteLine($"[SidekickAI.DoOrderAttack] {m_Sidekick.Name} - CANNOT ATTACK {target.Name}");
                    Utility.PopColor();
                    return false;
                }
            }
            else
            {
                // FIX: Target is null, dead, or invalid - transition back to Follow mode
                bool targetIsNull = m_Sidekick.ControlTarget == null;
                bool targetNotMobile = !targetIsNull && !(m_Sidekick.ControlTarget is Mobile);
                bool targetDead = !targetIsNull && m_Sidekick.ControlTarget is Mobile deadTarget && !deadTarget.Alive;
                
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[SidekickAI.DoOrderAttack] {m_Sidekick.Name} - Target invalid (null={targetIsNull}, notMobile={targetNotMobile}, dead={targetDead}) - ending combat, returning to Follow");
                Utility.PopColor();
                
                // Clear combat state
                m_Sidekick.Combatant = null;
                m_Sidekick.Warmode = false;
                
                // Return to Follow mode
                if (m_Sidekick.Controlled && m_Sidekick.ControlMaster != null)
                {
                    m_Sidekick.ControlOrder = OrderType.Follow;
                    m_Sidekick.ControlTarget = m_Sidekick.ControlMaster;
                    Action = ActionType.Wander; // This will let DoOrderFollow take over
                }
            }
            
            return false;
        }

        /// <summary>
        /// Handle mount order (custom method, not in BaseAI)
        /// Note: Mounting is handled via pet commands, this is a placeholder for future implementation
        /// </summary>
        public bool DoOrderMount()
        {
            if (m_Sidekick == null || m_Sidekick.CurrentMount == null)
                return false;

            BaseCreature mount = m_Sidekick.CurrentMount;
            if (mount == null || mount.Deleted || !mount.Alive)
                return false;

            // Check if already mounted
            if (m_Mobile.Mount != null)
            {
                m_Mobile.DebugSay("I am already mounted");
                return true;
            }

            // For now, mounting is handled via standard pet mount commands
            // This method can be enhanced later to directly mount the creature
            m_Mobile.DebugSay("Mount order received for {0}", mount.Name);
            return true;
        }

        /// <summary>
        /// Handle dismount order (custom method, not in BaseAI)
        /// </summary>
        public bool DoOrderDismount()
        {
            if (m_Sidekick.Mount != null)
            {
                // Use standard dismount
                m_Sidekick.Mount.Rider = null;
                if (Core.Debug)
                    m_Sidekick.DebugSay("I have dismounted");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handle stay order
        /// </summary>
        public override bool DoOrderStay()
        {
            if (m_Sidekick == null)
                return true;
                
            if (Core.Debug)
                Console.WriteLine($"[SidekickAI] DoOrderStay called");
                
            // Stop moving
            m_Path = null;
            
            return true;
        }

        /// <summary>
        /// Handle stop order
        /// </summary>
        public override bool DoOrderStop()
        {
            if (m_Sidekick == null)
                return true;

            if (Core.Debug)
                Console.WriteLine($"[SidekickAI] DoOrderStop called");
            
            // Stop moving
            m_Path = null;
            
            // Clear combatant
            m_Sidekick.Combatant = null;
            m_Sidekick.Warmode = false;
            
            return true;
        }

        /// <summary>
        /// Handle come order
        /// </summary>
        public override bool DoOrderCome()
        {
            if (m_Sidekick == null || m_Sidekick.ControlMaster == null)
                return true;

            if (Core.Debug)
                Console.WriteLine($"[SidekickAI] DoOrderCome called");

            Mobile master = m_Sidekick.ControlMaster;
            
            if (master.Deleted || master.Map != m_Sidekick.Map)
                return true;
                
            // Move to master
            if (m_Sidekick.GetDistanceToSqrt(master) > 2)
            {
                MoveTo(master, true, 1);
            }
            
            return true;
        }
        
        /// <summary>
        /// Handle guard order
        /// </summary>
        public override bool DoOrderGuard()
        {
            if (m_Sidekick == null)
                return true;

            if (Core.Debug)
                Console.WriteLine($"[SidekickAI] DoOrderGuard called");
                
            Mobile controlMaster = m_Sidekick.ControlMaster;
            if (controlMaster == null || controlMaster.Deleted)
                return true;
                
            // Ensure we are attacking anything attacking the master
            Mobile aggressor = controlMaster.Combatant as Mobile;
            if (aggressor != null && m_Sidekick.CanBeHarmful(aggressor, false))
            {
                m_Sidekick.Combatant = aggressor;
                Action = ActionType.Combat;
            }
            
            return true;
        }

        /// <summary>
        /// Handle none order
        /// </summary>
        public override bool DoOrderNone()
        {
             if (Core.Debug)
                Console.WriteLine($"[SidekickAI] DoOrderNone called");
            return true;
        }
        
        /// <summary>
        /// Handle release order
        /// </summary>
        public override bool DoOrderRelease()
        {
            if (m_Sidekick == null)
                return true;
                
            if (Core.Debug)
                Console.WriteLine($"[SidekickAI] DoOrderRelease called");

            // Release the sidekick
            m_Sidekick.Controlled = false;
            m_Sidekick.ControlMaster = null;
            m_Sidekick.ControlOrder = OrderType.None;
            
            m_Sidekick.DebugSay("I have been released");
            
            return true;
        }

        /// <summary>
        /// Handle transfer order
        /// </summary>
        public override bool DoOrderTransfer()
        {
             if (Core.Debug)
                Console.WriteLine($"[SidekickAI] DoOrderTransfer called - Not fully implemented for Sidekick");
                
             // TODO: Implement transfer targeting
             if (m_Sidekick != null && m_Sidekick.ControlMaster != null)
             {
                 m_Sidekick.ControlMaster.SendMessage("Transferring sidekicks is not supported yet.");
             }
             
             return true;
        }
        
        public override bool DoOrderDrop()
        {
            return true;
        }
        
        public override bool DoOrderPatrol()
        {
            return true;
        }
        
        public override bool DoOrderFriend()
        {
            return true;
        }
        
        public override bool DoOrderUnfriend()
        {
            return true;
        }

        public override void Deactivate()
        {
            if (m_Timer != null)
                m_Timer.Stop();
        }

        public override void Activate()
        {
            if (m_Timer != null && !m_Timer.Running)
            {
                m_Timer.Delay = TimeSpan.Zero;
                m_Timer.Start();
            }
        }

    }
}
