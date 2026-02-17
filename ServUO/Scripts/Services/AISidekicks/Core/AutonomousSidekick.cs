using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Items;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Services.LLM;
using Server.Services.AISidekicks;

namespace Server.Services.AISidekicks
{
    /// <summary>
    /// An autonomous AI sidekick companion that talks (LLM), acts autonomously, and fights like a player.
    /// Sidekicks are true companions that start with player-like stats/skills and grow over time.
    /// </summary>
    public class AutonomousSidekick : BaseSidekick // ILLMConversational - DISABLED for testing
    {
        #region LLM Integration Fields

        private string m_Personality;
        private NPCPersonalities.PersonalityType m_PersonalityType;
        private NPCPersonalities.SpeechPattern m_SpeechPattern;
        private bool m_LLMConversationEnabled = false; // DISABLED for testing
        private int m_HearingRange; // Loaded from config
        private List<LoreEntry> m_KnowledgeBase;
        private string m_FormattedKnowledge;

        #endregion

        #region Companion System Fields

        private PlayerMobile m_Owner; // The player this sidekick belongs to
        private bool m_IsFollowing = true; // Currently following owner
        private int m_FollowDistance; // Preferred follow distance (loaded from config)

        #endregion

        #region Autonomous Behavior Fields

        private SidekickGoal m_CurrentGoal = SidekickGoal.FollowOwner;
        private DateTime m_LastDecisionTime = DateTime.UtcNow;
        private TimeSpan m_DecisionInterval; // How often to make autonomous decisions (loaded from config)

        #endregion

        #region Combat AI Fields

        private CombatStyle m_CombatStyle;
        private SidekickArchetypeType m_ArchetypeType;

        #endregion

        #region Pet/Mount Management Fields

        private List<BaseCreature> m_OwnedPets;
        private BaseCreature m_CurrentMount;

        #endregion

        #region Properties

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Owner
        {
            get { return m_Owner; }
            set
            {
                if (m_Owner != value)
                {
                    m_Owner = value;
                    if (value != null)
                    {
                        ControlMaster = value;
                        ControlOrder = OrderType.Follow;
                        ControlTarget = value;
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsFollowing
        {
            get { return m_IsFollowing; }
            set { m_IsFollowing = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FollowDistance
        {
            get { return m_FollowDistance; }
            set { m_FollowDistance = Math.Max(1, Math.Min(10, value)); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SidekickGoal CurrentGoal
        {
            get { return m_CurrentGoal; }
            set { m_CurrentGoal = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastDecisionTime
        {
            get { return m_LastDecisionTime; }
            set { m_LastDecisionTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan DecisionInterval
        {
            get { return m_DecisionInterval; }
            set { m_DecisionInterval = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SidekickArchetypeType ArchetypeType
        {
            get { return m_ArchetypeType; }
            set { m_ArchetypeType = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CombatStyle CombatStyle
        {
            get { return m_CombatStyle; }
            set { m_CombatStyle = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public List<BaseCreature> OwnedPets
        {
            get
            {
                if (m_OwnedPets == null)
                    m_OwnedPets = new List<BaseCreature>();
                return m_OwnedPets;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature CurrentMount
        {
            get { return m_CurrentMount; }
            set { m_CurrentMount = value; }
        }

        // ILLMConversational interface properties
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LLMConversationEnabled
        {
            get { return m_LLMConversationEnabled; }
            set { m_LLMConversationEnabled = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NPCPersonalities.PersonalityType PersonalityType
        {
            get { return m_PersonalityType; }
            set
            {
                m_PersonalityType = value;
                UpdatePersonality();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NPCPersonalities.SpeechPattern SpeechPattern
        {
            get { return m_SpeechPattern; }
            set { m_SpeechPattern = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HearingRange
        {
            get { return m_HearingRange; }
            set { m_HearingRange = Math.Max(1, Math.Min(20, value)); }
        }

        /// <summary>
        /// Override FreezeOnCast - mages should freeze while casting spells (like players do)
        /// </summary>
        public override bool FreezeOnCast
        {
            get
            {
                // Mages and hybrid casters should freeze on cast
                return m_CombatStyle == CombatStyle.Mage || m_CombatStyle == CombatStyle.Hybrid;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor - creates a Warrior sidekick
        /// </summary>
        [Constructable]
        public AutonomousSidekick() : this(SidekickArchetypeType.Warrior)
        {
        }

        /// <summary>
        /// Constructor with archetype
        /// </summary>
        [Constructable]
        public AutonomousSidekick(SidekickArchetypeType archetypeType) : base()
        {
            m_ArchetypeType = archetypeType;
            InitializeFromArchetype(archetypeType);
        }

        /// <summary>
        /// Constructor with archetype and owner
        /// </summary>
        public AutonomousSidekick(SidekickArchetypeType archetypeType, PlayerMobile owner) : this(archetypeType)
        {
            Owner = owner;
            SetupOwnerRelationship();
        }

        /// <summary>
        /// Constructor with archetype, owner, and appearance customization
        /// </summary>
        public AutonomousSidekick(SidekickArchetypeType archetypeType, PlayerMobile owner, 
            bool isMale, Race race, int skinHue, int hairItemID, int hairHue, int facialHairItemID, int facialHairHue) : base(AIType.AI_Use_Default, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            // Manual initialization instead of chaining to avoid Race/Equipment timing issues
            m_ArchetypeType = archetypeType;
            
            // Set Race FIRST
            Race = race ?? Race.Human;
            
            // Apply appearance (Body, Hair, etc.)
            ApplyAppearance(isMale, skinHue, hairItemID, hairHue, facialHairItemID, facialHairHue);

            // Initialize from archetype (Stats, Skills, Gear)
            InitializeFromArchetype(archetypeType);

            Owner = owner;
            SetupOwnerRelationship();
        }

        /// <summary>
        /// Serialization constructor - REQUIRED for ServUO serialization system
        /// </summary>
        public AutonomousSidekick(Serial serial) : base(serial)
        {
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize sidekick from archetype definition
        /// </summary>
        private void InitializeFromArchetype(SidekickArchetypeType archetypeType)
        {
            SidekickArchetype archetype = SidekickArchetype.GetArchetype(archetypeType);
            if (archetype == null)
            {
                Console.WriteLine($"[AutonomousSidekick] Warning: Invalid archetype {archetypeType}, using Warrior");
                archetype = SidekickArchetype.GetArchetype(SidekickArchetypeType.Warrior);
            }

            // Set as player-like for resurrection capability
            Player = true;

            // Initialize stats to 100/100/100 for all archetypes
            InitStats(100, 100, 100);
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[AutonomousSidekick.InitializeFromArchetype] {Name} - Set stats to 100/100/100");
            Utility.PopColor();

            // Initialize ALL skills to GM+ level (125.0)
            foreach (var skill in archetype.StartingSkills)
            {
                Skills[skill.SkillName].Base = 125.0;
            }
            
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[AutonomousSidekick.InitializeFromArchetype] {Name} - Set {archetype.StartingSkills.Count} skills to 125.0 for {archetypeType}");
            Utility.PopColor();


            // Set stat locks for growth direction
            StrLock = archetype.StatLocks.StrLock;
            DexLock = archetype.StatLocks.DexLock;
            IntLock = archetype.StatLocks.IntLock;

            // Set personality
            m_PersonalityType = archetype.PersonalityType;
            m_SpeechPattern = archetype.SpeechPattern;
            UpdatePersonality();

            // Set combat style
            m_CombatStyle = archetype.CombatStyle;
            
            // CRITICAL: Initialize MageAI AFTER CombatStyle is set
            // The AI needs to know the combat style to create the appropriate helpers
            if (AIObject is SidekickAI sidekickAI)
            {
                sidekickAI.InitializeMageAI();
            }

            // Enable SmartAI for mages by adding MageryMastery training ability
            // This allows access to high-level spells when Mana > 100
            if (m_CombatStyle == CombatStyle.Mage || m_CombatStyle == CombatStyle.Hybrid)
            {
                SetMagicalAbility(MagicalAbility.MageryMastery);
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine($"[AutonomousSidekick.InitializeFromArchetype] {Name} - Enabled MageryMastery (SmartAI mode) for better spell selection");
                Utility.PopColor();
            }

            // Set appropriate combat range based on archetype combat style
            // This ensures mages stay at casting range, archers at bow range, etc.
            if (m_CombatStyle == CombatStyle.Mage)
            {
                RangeFight = 10; // Spell casting range
            }
            else if (m_CombatStyle == CombatStyle.Archer)
            {
                RangeFight = 8; // Archery range
            }
            else
            {
                RangeFight = 1; // Melee range
            }

            // Set AI type - ForcedAI will override this to use SidekickAI
            // BaseCreature manages m_CurrentAI and m_DefaultAI internally
            // NOTE: Commented out to prevent ForcedAI recreation - BaseCreature constructor already calls this
            // ChangeAIType(archetype.BaseAIType);

            // Set default appearance (will be overridden if appearance parameters provided)
            // Only set if not already set (to preserve ApplyAppearance settings)
            if (Body == 0)
            {
                Body = Utility.RandomBool() ? 0x190 : 0x191; // Male or female
                Hue = Utility.RandomSkinHue();
            }

            // Set name based on archetype
            Name = GenerateName(archetype);

            // Set default properties
            Blessed = false; // Can be attacked
            CantWalk = false; // Can move
            FollowersMax = 0; // Not a pet, doesn't use follower slots
            
            // Load follow distance from config
            m_FollowDistance = SidekickConfig.FollowDistance;
            
            // Load decision interval from config
            m_DecisionInterval = TimeSpan.FromSeconds(SidekickConfig.DecisionInterval);
            
            // Load hearing range from config
            m_HearingRange = SidekickConfig.HearingRange;
            
            // Set player-like movement (walk/run like players) from config
            CurrentSpeed = SidekickConfig.WalkSpeed;
            PassiveSpeed = SidekickConfig.PassiveSpeed;
            ActiveSpeed = SidekickConfig.RunSpeed;

            // Create backpack for inventory access
            if (Backpack == null)
            {
                Backpack pack = new Backpack();
                pack.Movable = false;
                AddItem(pack);
            }

            // Initialize pet list
            if (m_OwnedPets == null)
                m_OwnedPets = new List<BaseCreature>();
            m_CurrentMount = null;

            // Equip starting equipment based on archetype
            EquipStartingGear(archetypeType);

            // Add standard items to all sidekicks: full spellbook, reagent bag, bandages, potions
            AddStandardItems();

            // Load knowledge base for LLM
            LoadKnowledgeBase();
        }

        /// <summary>
        /// Setup owner relationship and enable auto-follow
        /// </summary>
        private void SetupOwnerRelationship()
        {
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - Setting up relationship with owner: {m_Owner?.Name ?? "null"}");
            Utility.PopColor();
            
            if (m_Owner != null && !m_Owner.Deleted)
            {
                // CRITICAL: Set Controlled FIRST, before setting ControlOrder
                // This ensures BaseCreature recognizes it as a tamed pet
                Controlled = true; // Must be set for HandlesOnSpeech and AI to work
                ControlMaster = m_Owner;
                ControlOrder = OrderType.Follow;
                ControlTarget = m_Owner;
                
                // Enable auto-follow
                m_IsFollowing = true;
                
                // Set as tameable/controlled
                Tamable = true;
                MinTameSkill = 0; // Already tamed
                
                // Set loyalty (like pets)
                Loyalty = MaxLoyalty;
                
                // Ensure name color is correct for tamed pets
                // BaseCreature will set NameHue correctly when Controlled && Commandable
                // Force property update to refresh name display
                InvalidateProperties();
                Delta(MobileDelta.Noto);
                
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - AIObject: {AIObject != null}, Map: {Map}, Controlled: {Controlled}");
                Utility.PopColor();
                
                // Ensure AI is active
                if (AIObject == null)
                {
                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - WARNING: AIObject is null, calling ChangeAIType");
                    Utility.PopColor();
                    
                    ChangeAIType(AIType.AI_Melee); // Will be overridden by ForcedAI to SidekickAI
                }
                else
                {
                    // Ensure AI is processing
                    if (AIObject != null && AIObject.Action == ActionType.Guard)
                    {
                        AIObject.Action = ActionType.Wander;
                    }
                }
                
                // Force follow order to be processed after a short delay
                // This ensures the sidekick is fully initialized before following
                Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
                {
                    if (!Deleted && m_Owner != null && !m_Owner.Deleted)
                    {
                        Utility.PushColor(ConsoleColor.Cyan);
                        Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - DelayCall: AIObject={AIObject != null}, Timer={AIObject?.m_Timer != null}, Running={AIObject?.m_Timer?.Running ?? false}");
                        Utility.PopColor();
                        
                        // Set control properties
                        Controlled = true;
                        ControlMaster = m_Owner;
                        ControlOrder = OrderType.Follow;
                        ControlTarget = m_Owner;
                        m_IsFollowing = true;
                        
                        // Ensure AI timer is running
                        if (AIObject != null)
                        {
                            if (AIObject.m_Timer != null && !AIObject.m_Timer.Running)
                            {
                                Utility.PushColor(ConsoleColor.Green);
                                Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - Starting AI timer");
                                Utility.PopColor();
                                
                                AIObject.m_Timer.Start();
                        }
                        
                        // Ensure AI is processing
                            if (AIObject.Action == ActionType.Guard)
                        {
                            AIObject.Action = ActionType.Wander;
                            }
                        }
                    }
                });
                
                // Spawn horse mount after sidekick is placed in world
                // Use DelayCall to ensure Map and Location are valid
                Timer.DelayCall(TimeSpan.FromMilliseconds(1000), () =>
                {
                    if (!Deleted && m_Owner != null && !m_Owner.Deleted && 
                        Map != null && Map != Map.Internal && Location != Point3D.Zero)
                    {
                        // Only spawn horse if we don't already have one
                        if (m_CurrentMount == null || m_CurrentMount.Deleted)
                        {
                            try
                            {
                                Horse horse = new Horse();
                                horse.Controlled = true;
                                horse.ControlMaster = m_Owner;
                                horse.ControlOrder = OrderType.Follow;
                                horse.ControlTarget = m_Owner;
                                
                                // Spawn near sidekick
                                Point3D loc = Location;
                                int z = Map.GetAverageZ(loc.X, loc.Y);
                                horse.MoveToWorld(new Point3D(loc.X, loc.Y, z), Map);
                                
                                // CRITICAL: Add to owned pets list and set as mount
                                AddPet(horse);
                                SetMount(horse);
                                
                                // Mount the sidekick on the horse
                                if (horse is IMount mount)
                                {
                                    mount.Rider = this;
                                }
                                
                                Utility.PushColor(ConsoleColor.Green);
                                Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - Spawned and mounted on horse at {Location}");
                                Utility.PopColor();
                            }
                            catch (Exception ex)
                            {
                                Utility.PushColor(ConsoleColor.Red);
                                Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - ERROR spawning horse: {ex.Message}");
                                Utility.PopColor();
                            }
                        }
                    }
                    else
                    {
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - Skipping horse spawn: Deleted={Deleted}, Map={Map?.Name ?? "null"}, Location={Location}");
                        Utility.PopColor();
                    }
                });

                // Spawn bonded dragon for Tamer archetype
                if (m_ArchetypeType == SidekickArchetypeType.Tamer)
                {
                    Timer.DelayCall(TimeSpan.FromMilliseconds(1500), () =>
                    {
                        if (!Deleted && m_Owner != null && !m_Owner.Deleted &&
                            Map != null && Map != Map.Internal && Location != Point3D.Zero)
                        {
                            try
                            {
                                Dragon dragon = new Dragon();
                                dragon.Name = "Tamer's Dragon";

                                // Set up as bonded pet controlled by the sidekick's owner
                                dragon.Controlled = true;
                                dragon.ControlMaster = m_Owner;
                                dragon.ControlOrder = OrderType.Guard;
                                dragon.ControlTarget = this; // Guard the sidekick
                                dragon.IsBonded = true;
                                dragon.BondingBegin = DateTime.MinValue; // Already bonded
                                dragon.Loyalty = BaseCreature.MaxLoyalty;

                                // Spawn near sidekick (offset to avoid overlap)
                                Point3D loc = Location;
                                int offsetX = Utility.RandomMinMax(-2, 2);
                                int offsetY = Utility.RandomMinMax(-2, 2);
                                int z = Map.GetAverageZ(loc.X + offsetX, loc.Y + offsetY);
                                dragon.MoveToWorld(new Point3D(loc.X + offsetX, loc.Y + offsetY, z), Map);

                                // Add to sidekick's owned pets list
                                AddPet(dragon);

                                Utility.PushColor(ConsoleColor.Magenta);
                                Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - Spawned bonded dragon '{dragon.Name}' at {dragon.Location} for Tamer archetype");
                                Utility.PopColor();
                            }
                            catch (Exception ex)
                            {
                                Utility.PushColor(ConsoleColor.Red);
                                Console.WriteLine($"[AutonomousSidekick.SetupOwnerRelationship] {Name} - ERROR spawning dragon: {ex.Message}");
                                Utility.PopColor();
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Add standard items to all sidekicks: full spellbook (64 spells), reagent bag (50 of each), bandages, and potions
        /// </summary>
        private void AddStandardItems()
        {
            if (Backpack == null)
            {
                Console.WriteLine($"[AutonomousSidekick.AddStandardItems] {Name} - ERROR: No backpack!");
                return;
            }

            // 1. Full spellbook with all 64 spells
            Spellbook book = new Spellbook();
            book.Content = ulong.MaxValue; // All 64 spells
            EquipItem(book);

            // 2. Reagent bag with 50 of each STANDARD reagent (no necro reagents - those are archetype-specific)
            Bag reagentBag = new Bag();
            reagentBag.Name = "Reagent Bag";
            reagentBag.DropItem(new BlackPearl(50));
            reagentBag.DropItem(new Bloodmoss(50));
            reagentBag.DropItem(new Garlic(50));
            reagentBag.DropItem(new Ginseng(50));
            reagentBag.DropItem(new MandrakeRoot(50));
            reagentBag.DropItem(new Nightshade(50));
            reagentBag.DropItem(new SulfurousAsh(50));
            reagentBag.DropItem(new SpidersSilk(50));
            Backpack.DropItem(reagentBag);

            // 3. 150 bandages
            Bandage bandages = new Bandage(150);
            Backpack.DropItem(bandages);

            // 4. Greater potions
            AddPotionsToBackpack<GreaterHealPotion>(25);
            AddPotionsToBackpack<GreaterCurePotion>(25);
            AddPotionsToBackpack<GreaterAgilityPotion>(10);
            AddPotionsToBackpack<TotalRefreshPotion>(10);

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine($"[AutonomousSidekick.AddStandardItems] {Name} - Added: Spellbook(64), Reagents(50ea), Bandages(150), Potions(70 total)");
            Utility.PopColor();
        }

        /// <summary>
        /// Helper method to add potions to backpack with correct amount
        /// Potions don't have constructors that take amounts - must create and set Amount property
        /// If stackable (Core.ML), creates one stack; otherwise creates individual potions
        /// </summary>
        private void AddPotionsToBackpack<T>(int amount) where T : BasePotion, new()
        {
            if (Backpack == null || amount <= 0)
                return;

            if (Core.ML)
            {
                // ML expansion - potions are stackable, create one stack
                T potion = new T();
                potion.Amount = amount;
                Backpack.DropItem(potion);
            }
            else
            {
                // Pre-ML - potions not stackable, create individual items (max 20 to avoid spam)
                int toCreate = Math.Min(amount, 20);
                for (int i = 0; i < toCreate; i++)
                {
                    Backpack.DropItem(new T());
                }
            }
        }

        /// <summary>
        /// Equip starting gear based on archetype
        /// </summary>
        private void EquipStartingGear(SidekickArchetypeType archetypeType)
        {
            // Use archetype-specific loadouts
            switch (archetypeType)
            {
                case SidekickArchetypeType.Warrior:
                    EquipWarrior();
                    break;
                case SidekickArchetypeType.Mage:
                    EquipMage();
                    break;
                case SidekickArchetypeType.Archer:
                    EquipArcher();
                    break;
                case SidekickArchetypeType.Healer:
                    EquipHealer();
                    break;
                case SidekickArchetypeType.Paladin:
                    EquipPaladin();
                    break;
                case SidekickArchetypeType.Ranger:
                    EquipArcher(); // Ranger uses archer equipment
                    break;
                case SidekickArchetypeType.Thief:
                    EquipThief();
                    break;
                case SidekickArchetypeType.Necromancer:
                    EquipNecromancer();
                    break;
                case SidekickArchetypeType.Battlemage:
                    EquipBattlemage();
                    break;
                case SidekickArchetypeType.Cleric:
                    EquipHealer(); // Cleric uses healer equipment
                    break;
                case SidekickArchetypeType.Druid:
                    EquipDruid();
                    break;
                case SidekickArchetypeType.Tamer:
                    EquipTamer();
                    break;
                default:
                    EquipWarrior(); // Default fallback
                    break;
            }
            
            // Note: Horse spawning moved to SetupOwnerRelationship() to ensure
            // sidekick is placed in world (Map != Internal, Location valid) before spawning
        }

        /// <summary>
        /// Equip all sidekicks with standard gear: full spellbooks, horned leather armor, vanq weapons, potions, reagents, and bandages
        /// </summary>
        private void EquipStandardGear(SidekickArchetypeType archetypeType)
        {
            // 1. Full spellbooks regardless of class
            Spellbook regularBook = new Spellbook();
            // Regular spellbook has 64 spells, use ulong.MaxValue for all spells
            regularBook.Content = ulong.MaxValue;
            EquipItem(regularBook);

            NecromancerSpellbook necroBook = new NecromancerSpellbook();
            // Necromancer spellbook has 16 or 17 spells, use (1ul << BookCount) - 1
            int necroBookCount = necroBook.BookCount;
            necroBook.Content = (1ul << necroBookCount) - 1;
            if (Backpack != null)
                Backpack.DropItem(necroBook);

            // 2. 500 bandages
            if (Backpack != null)
            {
                Backpack.DropItem(new Bandage(500));
            }

            // 3. Bag with 250 of each reagent
            if (Backpack != null)
            {
                Bag reagentBag = new Bag();
                reagentBag.Name = "Reagent Bag";
                reagentBag.DropItem(new BlackPearl(250));
                reagentBag.DropItem(new Bloodmoss(250));
                reagentBag.DropItem(new Garlic(250));
                reagentBag.DropItem(new Ginseng(250));
                reagentBag.DropItem(new MandrakeRoot(250));
                reagentBag.DropItem(new Nightshade(250));
                reagentBag.DropItem(new SulfurousAsh(250));
                reagentBag.DropItem(new SpidersSilk(250));
                // Necromancer reagents
                reagentBag.DropItem(new BatWing(250));
                reagentBag.DropItem(new GraveDust(250));
                reagentBag.DropItem(new DaemonBlood(250));
                reagentBag.DropItem(new NoxCrystal(250));
                reagentBag.DropItem(new PigIron(250));
                Backpack.DropItem(reagentBag);
            }

            // 4. Full suit of Horned runic leather
            // 4. Full suit of armor (Horned Leather or Leaf for Elves)
            if (Race == Race.Elf)
            {
                EquipItem(new LeafChest());
                EquipItem(new LeafArms());
                EquipItem(new LeafLegs());
                EquipItem(new LeafGorget());
                EquipItem(new LeafGloves());
                // Elves don't wear caps usually, or Circlets?
                // EquipItem(new Circlet()); 
            }
            else
            {
                LeatherChest chest = new LeatherChest();
                chest.Resource = CraftResource.HornedLeather;
                EquipItem(chest);

                LeatherArms arms = new LeatherArms();
                arms.Resource = CraftResource.HornedLeather;
                EquipItem(arms);

                LeatherLegs legs = new LeatherLegs();
                legs.Resource = CraftResource.HornedLeather;
                EquipItem(legs);

                LeatherGorget gorget = new LeatherGorget();
                gorget.Resource = CraftResource.HornedLeather;
                EquipItem(gorget);

                LeatherGloves gloves = new LeatherGloves();
                gloves.Resource = CraftResource.HornedLeather;
                EquipItem(gloves);

                LeatherCap cap = new LeatherCap();
                cap.Resource = CraftResource.HornedLeather;
                EquipItem(cap);
            }

            // Boots (Elven Boots for Elves?)
            if (Race == Race.Elf)
                EquipItem(new ElvenBoots());
            else
                EquipItem(new Boots());

            // 5. Vanq weapons for each class
            BaseWeapon weapon = GetClassWeapon(archetypeType);
            if (weapon != null)
            {
                weapon.DamageLevel = WeaponDamageLevel.Vanq;
                weapon.Quality = ItemQuality.Exceptional;
                EquipItem(weapon);
            }

            // 6. Greater potions only - no regular/lesser potions
            // Note: Potions don't have constructors that take amounts, must set Amount after creation
            if (Backpack != null)
            {
                AddPotionsToBackpack<GreaterHealPotion>(50);
                AddPotionsToBackpack<GreaterCurePotion>(50);
                AddPotionsToBackpack<GreaterAgilityPotion>(20);
                AddPotionsToBackpack<TotalRefreshPotion>(20);
            }
        }

        /// <summary>
        /// Get the appropriate vanquishing weapon for the sidekick's class
        /// </summary>
        private BaseWeapon GetClassWeapon(SidekickArchetypeType archetypeType)
        {
            switch (archetypeType)
            {
                case SidekickArchetypeType.Warrior:
                case SidekickArchetypeType.Paladin:
                case SidekickArchetypeType.Battlemage:
                    return new Longsword();
                case SidekickArchetypeType.Archer:
                case SidekickArchetypeType.Ranger:
                    return new Bow();
                case SidekickArchetypeType.Thief:
                    return new Dagger();
                case SidekickArchetypeType.Mage:
                case SidekickArchetypeType.Healer:
                case SidekickArchetypeType.Cleric:
                case SidekickArchetypeType.Druid:
                    return new GnarledStaff();
                case SidekickArchetypeType.Necromancer:
                    return new GnarledStaff();
                case SidekickArchetypeType.Tamer:
                    return new GnarledStaff();
                default:
                    return new Longsword();
            }
        }

        private void EquipWarrior()
        {
            // Basic armor
            EquipItem(new StuddedChest());
            EquipItem(new StuddedArms());
            EquipItem(new StuddedLegs());
            EquipItem(new StuddedGorget());
            EquipItem(new StuddedGloves());
            
            // Weapon - Double Axe for Lumberjack build (2-handed, uses Swords skill + Lumberjacking bonus)
            BaseWeapon weapon = new DoubleAxe();
            weapon.DamageLevel = WeaponDamageLevel.Vanq;
            weapon.Quality = ItemQuality.Exceptional;
            EquipItem(weapon);
            
            // No shield - Double Axe is two-handed
            
            // Boots
            EquipItem(new Boots());
            
            // Note: Bandages are added by AddStandardItems()
        }

        private void EquipMage()
        {
            // Robe and hat
            EquipItem(new Robe(Utility.RandomBlueHue()));
            EquipItem(new WizardsHat(Utility.RandomBlueHue()));
            EquipItem(new Sandals());
            
            // Note: Full spellbook, reagent bag, and bandages are added by AddStandardItems()
        }

        private void EquipArcher()
        {
            // Light armor
            EquipItem(new LeatherChest());
            EquipItem(new LeatherArms());
            EquipItem(new LeatherLegs());
            EquipItem(new LeatherGorget());
            EquipItem(new LeatherGloves());
            
            // Bow - Vanquishing quality
            BaseWeapon weapon = new Bow();
            weapon.DamageLevel = WeaponDamageLevel.Vanq;
            weapon.Quality = ItemQuality.Exceptional;
            EquipItem(weapon);
            
            // Arrows in backpack
            if (Backpack != null)
            {
                Backpack.DropItem(new Arrow(500));
            }
            // Note: Bandages are added by AddStandardItems()
            
            // Boots
            EquipItem(new Boots());
        }

        private void EquipHealer()
        {
            // Robe
            EquipItem(new Robe(Utility.RandomPinkHue()));
            EquipItem(new Sandals());
            
            // Note: Bandages are added by AddStandardItems()
        }

        private void EquipPaladin()
        {
            // Plate armor
            EquipItem(new PlateChest());
            EquipItem(new PlateArms());
            EquipItem(new PlateLegs());
            EquipItem(new PlateGorget());
            EquipItem(new PlateGloves());
            
            // Weapon
            EquipItem(new Longsword());
            
            // Shield
            EquipItem(new OrderShield());
            
            // Boots
            EquipItem(new Boots());
        }

        private void EquipThief()
        {
            // Light armor
            EquipItem(new LeatherChest());
            EquipItem(new LeatherArms());
            EquipItem(new LeatherLegs());
            EquipItem(new LeatherGorget());
            EquipItem(new LeatherGloves());
            
            // Dagger
            EquipItem(new Dagger());
            
            // Lockpicks in backpack
            if (Backpack != null)
            {
                Backpack.DropItem(new Lockpick(50));
            }
            
            // Boots
            EquipItem(new Boots());
        }

        private void EquipNecromancer()
        {
            // Dark robe
            EquipItem(new Robe(Utility.RandomMinMax(0x0001, 0x0003))); // Dark colors
            EquipItem(new Sandals());
            
            // Necromancer spellbook (in addition to standard spellbook)
            NecromancerSpellbook necroBook = new NecromancerSpellbook();
            int necroBookCount = necroBook.BookCount;
            necroBook.Content = (1ul << necroBookCount) - 1; // All necromancer spells
            if (Backpack != null)
                Backpack.DropItem(necroBook);
            
            // Necromancer-specific reagents (only Necromancer archetype gets these)
            if (Backpack != null)
            {
                Bag necroReagentBag = new Bag();
                necroReagentBag.Name = "Necro Reagent Bag";
                necroReagentBag.DropItem(new BatWing(50));
                necroReagentBag.DropItem(new GraveDust(50));
                necroReagentBag.DropItem(new DaemonBlood(50));
                necroReagentBag.DropItem(new NoxCrystal(50));
                necroReagentBag.DropItem(new PigIron(50));
                Backpack.DropItem(necroReagentBag);
            }
            
            // Note: Standard spellbook, reagent bag, and bandages are added by AddStandardItems()
        }

        private void EquipBattlemage()
        {
            // Studded armor
            EquipItem(new StuddedChest());
            EquipItem(new StuddedArms());
            EquipItem(new StuddedLegs());
            
            // Weapon
            EquipItem(new Longsword());
            
            // Boots
            EquipItem(new Boots());
            
            // Note: Full spellbook, reagent bag, and bandages are added by AddStandardItems()
        }

        private void EquipDruid()
        {
            // Robe
            EquipItem(new Robe(Utility.RandomGreenHue()));
            EquipItem(new Sandals());
            
            // Note: Full spellbook, reagent bag, and bandages are added by AddStandardItems()
        }

        private void EquipTamer()
        {
            // Light armor
            EquipItem(new LeatherChest());
            EquipItem(new LeatherArms());
            EquipItem(new LeatherLegs());
            EquipItem(new LeatherGorget());
            EquipItem(new LeatherGloves());
            
            // Whip or staff
            EquipItem(new GnarledStaff());
            
            // Boots
            EquipItem(new Boots());
            
            // Note: Horse mount is spawned in EquipStartingGear() for all archetypes
        }

        /// <summary>
        /// Apply appearance customization
        /// </summary>
        private void ApplyAppearance(bool isMale, int skinHue, int hairItemID, int hairHue, int facialHairItemID, int facialHairHue)
        {
            // Set body type
            Body = isMale ? 0x190 : 0x191;
            
            // Set skin hue
            if (skinHue > 0)
                Hue = skinHue;
            else
                Hue = Utility.RandomSkinHue();

            // Set hair
            if (hairItemID > 0)
            {
                HairItemID = hairItemID;
                HairHue = hairHue > 0 ? hairHue : Utility.RandomHairHue();
            }
            else
            {
                HairItemID = Utility.RandomList(0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x204A);
                HairHue = Utility.RandomHairHue();
            }

            // Set facial hair (male only)
            if (isMale)
            {
                if (facialHairItemID > 0)
                {
                    FacialHairItemID = facialHairItemID;
                    FacialHairHue = facialHairHue > 0 ? facialHairHue : HairHue;
                }
                else
                {
                    FacialHairItemID = Utility.RandomList(0x203E, 0x203F, 0x2040, 0x2041, 0x204B, 0x204C, 0x204D);
                    FacialHairHue = HairHue;
                }
            }
            else
            {
                FacialHairItemID = 0;
                FacialHairHue = 0;
            }
        }

        /// <summary>
        /// Update personality string from personality type and speech pattern
        /// </summary>
        private void UpdatePersonality()
        {
            m_Personality = NPCPersonalities.GetPersonalityPrompt(m_PersonalityType, m_SpeechPattern);
        }

        /// <summary>
        /// Generate a name for the sidekick based on archetype
        /// Uses the syllable-based name generator for unique first + last names
        /// </summary>
        private string GenerateName(SidekickArchetype archetype)
        {
            // Use the new syllable-based generator for full names (first + last)
            bool isMale = Utility.RandomBool();
            return SidekickNameGenerator.GenerateFullName(archetype.Type, isMale);
        }

        /// <summary>
        /// Load knowledge base for LLM conversations
        /// </summary>
        private void LoadKnowledgeBase()
        {
            try
            {
                // Infer role from personality
                NPCKnowledgeSystem.NPCRole role = NPCKnowledgeSystem.InferRoleFromPersonality(m_PersonalityType);

                // Get location name for context
                string locationName = NPCPersonalities.GetLocationName(this);

                // Load knowledge base (role + location specific)
                if (this.Map != null && this.Map != Map.Internal)
                {
                    m_KnowledgeBase = NPCKnowledgeSystem.GetNPCKnowledge(role, locationName, this.Location, this.Map);
                }
                else
                {
                    // Load only role-based knowledge for now, will reload when placed
                    var allLore = SimpleLoreSystem.GetAllLore();
                    var roleKnowledge = NPCKnowledgeSystem.GetRoleKnowledge(role, allLore);
                    var locationKnowledge = allLore.Where(l =>
                        l.Category == "Dungeon" ||
                        (l.Category == "Location" && l.Importance >= 8)
                    ).ToList();
                    m_KnowledgeBase = new List<LoreEntry>();
                    m_KnowledgeBase.AddRange(roleKnowledge);
                    m_KnowledgeBase.AddRange(locationKnowledge);
                    m_KnowledgeBase = m_KnowledgeBase.GroupBy(l => l.ID).Select(g => g.First()).ToList();
                }

                // Pre-format for prompts
                m_FormattedKnowledge = NPCKnowledgeSystem.FormatKnowledgeForPrompt(m_KnowledgeBase);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AutonomousSidekick] Error loading knowledge base for {Name}: {ex.Message}");
                m_KnowledgeBase = new List<LoreEntry>();
                m_FormattedKnowledge = "";
            }
        }

        #endregion

        #region ILLMConversational Implementation


        /// <summary>
        /// Determines if this sidekick should handle the conversation
        /// </summary>
        public bool ShouldHandleConversation(SpeechEventArgs e)
        {
            // LLM DISABLED FOR TESTING
            return false;
            
            /* LLM CODE COMMENTED OUT
            if (!m_LLMConversationEnabled)
                return false;

            // Don't handle commands (they start with '[')
            if (e.Speech != null && SidekickSpeechHandler.IsBracketCommand(e.Speech))
                return false;

            // Don't handle pet commands - process them first
            if (e.Speech != null && SidekickSpeechHandler.IsPetCommand(e.Speech, Name))
                return false;

            // Only respond to owner or nearby players
            if (e.Mobile == m_Owner || (e.Mobile is PlayerMobile && InRange(e.Mobile, m_HearingRange)))
            {
                return true;
            }

            return false;
            */
        }

        /// <summary>
        /// Handles the conversation using LLM
        /// </summary>
        public void HandleConversation(SpeechEventArgs e)
        {
            // LLM DISABLED FOR TESTING
            return;
            
            /* LLM CODE COMMENTED OUT
            if (!m_LLMConversationEnabled)
                return;

            // Use LLMConversationHelper to process conversation
            LLMConversationHelper.ProcessConversation(this, e.Mobile, e.Speech);
            */
        }

        #endregion

        #region Speech Handling

        /// <summary>
        /// Override OnSpeech to handle player speech
        /// IMPORTANT: Check for commands FIRST, then process pet commands, then handle LLM conversation
        /// </summary>
        public override void OnSpeech(SpeechEventArgs e)
        {
            Console.WriteLine($"[AutonomousSidekick] OnSpeech called - Sidekick: {Name}, From: {e.Mobile?.Name}, Speech: '{e.Speech}'");
            
            // Check if this is a pet command - if so, skip LLM entirely
            bool isCommand = false;
            if (e.Speech != null)
            {
                // Check for bracket commands
                bool isBracket = SidekickSpeechHandler.IsBracketCommand(e.Speech);
                Console.WriteLine($"[AutonomousSidekick] OnSpeech: Bracket command check: {isBracket}");
                if (isBracket)
                {
                    isCommand = true;
                }
                
                // Check for pet commands and process them
                bool isPetCmd = SidekickSpeechHandler.IsPetCommand(e.Speech, Name);
                Console.WriteLine($"[AutonomousSidekick] OnSpeech: Pet command check: {isPetCmd}");
                
                bool processed = false;
                if (isPetCmd)
                {
                    isCommand = true;
                    Console.WriteLine($"[AutonomousSidekick] OnSpeech: Processing pet command...");
                    // Process the command
                    processed = SidekickSpeechHandler.ProcessCommand(this, e.Mobile, e.Speech);
                    Console.WriteLine($"[AutonomousSidekick] OnSpeech: Command processed: {processed}");
                    
                    // CRITICAL: After setting ControlOrder, immediately process it via Obey()
                    // This matches how standard pets work - BaseAI.OnSpeech sets ControlOrder and Obey() processes it
                    if (processed && AIObject != null && Controlled && Commandable)
                    {
                        Console.WriteLine($"[AutonomousSidekick] OnSpeech: Calling Obey() to process ControlOrder: {ControlOrder}");
                        AIObject.Obey();
                    }
                }
                
                // If not processed by custom handler (either not a pet command, or ProcessCommand returned false),
                // pass to AIObject to handle standard BaseAI commands (friend, patrol, etc.)
                if (!processed && AIObject != null)
                {
                    AIObject.OnSpeech(e);
                }
            }
            else
            {
                Console.WriteLine($"[AutonomousSidekick] OnSpeech: Speech is null");
            }

            Console.WriteLine($"[AutonomousSidekick] OnSpeech: isCommand={isCommand}, calling base.OnSpeech");
            // Call base OnSpeech (Mobile's implementation)
            base.OnSpeech(e);

            // LLM CONVERSATION DISABLED FOR TESTING
            // Only handle LLM conversation if it's NOT a command
            // if (!isCommand)
            // {
            //     bool shouldHandle = ShouldHandleConversation(e);
            //     Console.WriteLine($"[AutonomousSidekick] OnSpeech: ShouldHandleConversation: {shouldHandle}");
            //     if (shouldHandle)
            //     {
            //         Console.WriteLine($"[AutonomousSidekick] OnSpeech: Handling LLM conversation");
            //         HandleConversation(e);
            //     }
            // }
            // else
            // {
            //     Console.WriteLine($"[AutonomousSidekick] OnSpeech: Skipping LLM conversation (isCommand=true)");
            // }
        }

        #endregion

        #region Paperdoll & Inventory Access

        /// <summary>
        /// Override to allow owner to open paperdoll
        /// </summary>
        public override bool CanPaperdollBeOpenedBy(Mobile from)
        {
            // Allow owner to open paperdoll, or use base behavior (for self or human body)
            if (from == m_Owner)
                return true;

            return base.CanPaperdollBeOpenedBy(from);
        }

        /// <summary>
        /// Override to allow owner to equip items on sidekick
        /// </summary>
        public override bool AllowEquipFrom(Mobile from)
        {
            // Allow owner to equip items
            if (from == m_Owner)
                return true;

            return base.AllowEquipFrom(from);
        }

        /// <summary>
        /// Override OnDoubleClick to open paperdoll for owner
        /// </summary>
        public override void OnDoubleClick(Mobile from)
        {
            // If owner double-clicks, open paperdoll
            if (from == m_Owner && CanPaperdollBeOpenedBy(from))
            {
                DisplayPaperdollTo(from);
                return;
            }

            // Otherwise use base behavior
            base.OnDoubleClick(from);
        }

        #endregion

        #region Serialization

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer); // BaseSidekick handles ControlMaster, ControlOrder, etc.
            writer.Write((int)1); // Version (bumped to 1 for pet/mount data)

            // LLM data
            writer.Write(m_Personality);
            writer.Write((int)m_PersonalityType);
            writer.Write((int)m_SpeechPattern);
            writer.Write(m_LLMConversationEnabled);
            writer.Write(m_HearingRange);

            // Companion data
            writer.Write(m_Owner);
            writer.Write(m_IsFollowing);
            writer.Write(m_FollowDistance);

            // Autonomous behavior data
            writer.Write((int)m_CurrentGoal);
            writer.Write(m_LastDecisionTime);
            writer.Write(m_DecisionInterval);

            // Combat data
            writer.Write((int)m_CombatStyle);
            writer.Write((int)m_ArchetypeType);

            // Pet/Mount data (version 1+)
            if (m_OwnedPets == null)
                m_OwnedPets = new List<BaseCreature>();
            writer.Write(m_OwnedPets.Count);
            foreach (BaseCreature pet in m_OwnedPets)
            {
                writer.Write(pet);
            }
            writer.Write(m_CurrentMount);
        }

        public override void Deserialize(GenericReader reader)
        {
            try
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                Console.WriteLine($"[AutonomousSidekick] Deserialize - Version: {version}, Serial: {Serial}");

                switch (version)
            {
                case 1:
                    // Read in same order as Serialize writes
                    // LLM data
                    m_Personality = reader.ReadString();
                    m_PersonalityType = (NPCPersonalities.PersonalityType)reader.ReadInt();
                    m_SpeechPattern = (NPCPersonalities.SpeechPattern)reader.ReadInt();
                    m_LLMConversationEnabled = reader.ReadBool();
                    m_HearingRange = reader.ReadInt();

                    // Companion data
                    m_Owner = reader.ReadMobile() as PlayerMobile;
                    m_IsFollowing = reader.ReadBool();
                    m_FollowDistance = reader.ReadInt();

                    // Autonomous behavior data
                    m_CurrentGoal = (SidekickGoal)reader.ReadInt();
                    m_LastDecisionTime = reader.ReadDateTime();
                    m_DecisionInterval = reader.ReadTimeSpan();

                    // Combat data
                    m_CombatStyle = (CombatStyle)reader.ReadInt();
                    m_ArchetypeType = (SidekickArchetypeType)reader.ReadInt();

                    // Pet/Mount data (version 1+)
                    int petCount = reader.ReadInt();
                    m_OwnedPets = new List<BaseCreature>();
                    for (int i = 0; i < petCount; i++)
                    {
                        BaseCreature pet = reader.ReadMobile() as BaseCreature;
                        if (pet != null && !pet.Deleted)
                            m_OwnedPets.Add(pet);
                    }
                    m_CurrentMount = reader.ReadMobile() as BaseCreature;
                    break;
                case 0:
                    // LLM data
                    m_Personality = reader.ReadString();
                    m_PersonalityType = (NPCPersonalities.PersonalityType)reader.ReadInt();
                    m_SpeechPattern = (NPCPersonalities.SpeechPattern)reader.ReadInt();
                    m_LLMConversationEnabled = reader.ReadBool();
                    m_HearingRange = reader.ReadInt();

                    // Companion data
                    m_Owner = reader.ReadMobile() as PlayerMobile;
                    m_IsFollowing = reader.ReadBool();
                    m_FollowDistance = reader.ReadInt();

                    // Autonomous behavior data
                    m_CurrentGoal = (SidekickGoal)reader.ReadInt();
                    m_LastDecisionTime = reader.ReadDateTime();
                    m_DecisionInterval = reader.ReadTimeSpan();

                    // Combat data
                    m_CombatStyle = (CombatStyle)reader.ReadInt();
                    m_ArchetypeType = (SidekickArchetypeType)reader.ReadInt();

                    // Initialize pet list if null (for version 0)
                    if (m_OwnedPets == null)
                        m_OwnedPets = new List<BaseCreature>();
                    m_CurrentMount = null;
                    break;
            }

            // Restore owner relationship
            if (m_Owner != null)
            {
                ControlMaster = m_Owner;
                ControlOrder = m_IsFollowing ? OrderType.Follow : OrderType.Stay;
                ControlTarget = m_Owner;
                Console.WriteLine($"[AutonomousSidekick] Deserialize - Restored owner relationship: {m_Owner.Name}");
            }

            // Reload knowledge base
            LoadKnowledgeBase();
            
            // CRITICAL: Initialize AI after deserialization
            // Use delayed call to ensure we're fully loaded and on a valid map
            Timer.DelayCall(TimeSpan.FromMilliseconds(500), () =>
            {
                if (!Deleted && Map != null && Map != Map.Internal)
                {
                    Console.WriteLine($"[AutonomousSidekick] Deserialize - Initializing AI after load - Map: {Map?.Name}, Location: {Location}");
                    // Use AI property to get current AI type, or use archetype default
                    if (AI != AIType.AI_Use_Default)
                    {
                        ChangeAIType(AI);
                    }
                    else
                    {
                        // Use default AI type from archetype
                        SidekickArchetype archetype = SidekickArchetype.GetArchetype(m_ArchetypeType);
                        if (archetype != null)
                        {
                            ChangeAIType(archetype.BaseAIType);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"[AutonomousSidekick] Deserialize - Skipping AI init - Deleted: {Deleted}, Map: {Map?.Name}");
                }
            });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AutonomousSidekick] Deserialize ERROR - Serial: {Serial}, Exception: {ex.Message}");
                Console.WriteLine($"[AutonomousSidekick] Stack trace: {ex.StackTrace}");
                throw; // Re-throw to let ServUO handle it
            }
        }

        #endregion

        #region AI Override

        // BaseSidekick now handles ForcedAI to create SidekickAI
        // No need to override here

        /// <summary>
        /// Delete sidekick when released
        /// </summary>
        public override bool DeleteOnRelease
        {
            get { return true; }
        }

        /// <summary>
        /// Add a pet to sidekick's collection
        /// </summary>
        public void AddPet(BaseCreature pet)
        {
            if (pet != null && !pet.Deleted && !OwnedPets.Contains(pet))
            {
                OwnedPets.Add(pet);
                pet.ControlMaster = this;
                pet.ControlOrder = OrderType.Follow;
                pet.ControlTarget = this;
            }
        }

        /// <summary>
        /// Remove a pet from sidekick's collection
        /// </summary>
        public void RemovePet(BaseCreature pet)
        {
            if (pet != null && OwnedPets.Contains(pet))
            {
                OwnedPets.Remove(pet);
                if (m_CurrentMount == pet)
                {
                    m_CurrentMount = null;
                }
                pet.ControlMaster = null;
            }
        }

        /// <summary>
        /// Set active mount
        /// </summary>
        public void SetMount(BaseCreature mount)
        {
            if (mount == null)
            {
                m_CurrentMount = null;
                return;
            }

            // Add to owned pets if not already there
            if (!OwnedPets.Contains(mount))
            {
                AddPet(mount);
            }

            m_CurrentMount = mount;
        }

        #endregion

        #region Death & Resurrection

        /// <summary>
        /// Override OnDeath to set Player=true for resurrection capability
        /// </summary>
        public override void OnDeath(Container c)
        {
            // Mark as player-like to prevent deletion and enable resurrection
            if (!Player)
            {
                Player = true;
            }

            base.OnDeath(c);
        }

        /// <summary>
        /// Override CheckResurrect to allow owner to resurrect
        /// </summary>
        public override bool CheckResurrect()
        {
            // Allow resurrection if owner is nearby or by owner's command
            if (m_Owner != null && !m_Owner.Deleted)
            {
                if (m_Owner.InRange(this, 12))
                {
                    return true;
                }
                // Allow owner to resurrect from anywhere
                return true;
            }

            return base.CheckResurrect();
        }

        /// <summary>
        /// Override OnAfterResurrect to restore stats and provide commentary
        /// </summary>
        public override void OnAfterResurrect()
        {
            base.OnAfterResurrect();

            // Restore to full health/mana/stam
            Hits = HitsMax;
            Stam = StamMax;
            Mana = ManaMax;

            // LLM commentary (if enabled)
            if (m_LLMConversationEnabled && m_Owner != null)
            {
                // Simple response for now - can be enhanced with LLM later
                Say("Thank you for bringing me back, master!");
            }
        }

        #endregion
        #region PvP & Combat Overrides

        /// <summary>
        /// Override CanBeHarmful to allow attacking anyone if explicitly commanded by master
        /// This fixes PvP issues where sidekicks wouldn't attack innocent players even when commanded
        /// </summary>
        public override bool CanBeHarmful(IDamageable target, bool message, bool ignoreOurBlessedness)
        {
            // Always allow attacking if explicitly commanded by master (except master themselves)
            if (ControlMaster != null && ControlOrder == OrderType.Attack && ControlTarget == target)
            {
                if (target == ControlMaster)
                    return false;
                    
                // If commanded to attack, we allow it regardless of notoriety
                // This allows "all kill" to work on blue players (will flag criminal)
                return true;
            }
            
            return base.CanBeHarmful(target, message, ignoreOurBlessedness);
        }

        /// <summary>
        /// Override OnDamage to automatically defend when attacked
        /// This ensures sidekicks fight back even when not explicitly told to attack
        /// </summary>
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            // Call base implementation first
            base.OnDamage(amount, from, willKill);
            
            // If we're being attacked and not already fighting back, defend ourselves
            if (from != null && !from.Deleted && from.Alive && !willKill && 
                CanBeHarmful(from, false) && from != ControlMaster)
            {
                // If we don't have a combatant, or our combatant is not the attacker, switch to defending
                if (Combatant == null || Combatant != from)
                {
                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine($"[AutonomousSidekick.OnDamage] {Name} - Taking damage from {from.Name}, defending automatically");
                    Utility.PopColor();
                    
                    // Set combatant and warmode
                    Combatant = from;
                    Warmode = true;
                    
                    // If we're in Follow/Stop/Stay/Guard/None mode, switch to Attack mode to defend ourselves
                    // Guard mode sidekicks should also defend when attacked
                    if (ControlOrder == OrderType.Follow || ControlOrder == OrderType.Stop || 
                        ControlOrder == OrderType.Stay || ControlOrder == OrderType.Guard || 
                        ControlOrder == OrderType.None)
                    {
                        ControlOrder = OrderType.Attack;
                        ControlTarget = from;
                        
                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine($"[AutonomousSidekick.OnDamage] {Name} - Changed ControlOrder to Attack {from.Name} (was {ControlOrder})");
                        Utility.PopColor();
                    }
                    
                    // Ensure AI is in combat mode
                    if (AIObject != null)
                    {
                        AIObject.Action = ActionType.Combat;
                    }
                }
            }
        }

        #endregion

    }

    /// <summary>
    /// Goal types for autonomous behavior
    /// </summary>
    public enum SidekickGoal
    {
        FollowOwner,        // Default: follow player
        Combat,             // Engage in combat
        AssistOwner,         // Help player (heal, buff, etc.)
        Explore,            // Explore nearby area
        Loot,               // Collect items from defeated enemies
        Guard,              // Guard a location
        Socialize,          // Talk to nearby NPCs
        Rest,               // Rest when low on health/mana
        Investigate         // Investigate interesting objects/events
    }
}

