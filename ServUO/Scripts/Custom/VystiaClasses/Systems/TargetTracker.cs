using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Custom.VystiaClasses.Systems
{
    #region Enums

    /// <summary>
    /// Types of stacks that can be tracked per-target
    /// </summary>
    public enum StackType
    {
        Chill,          // Ice Mage - 5 = Frozen
        Curse,          // Witch - various curse effects
        Combo,          // Rogue - finisher damage
        Pursuit,        // Bounty Hunter - tracking damage
        Bleed,          // Physical DoT
        Poison,         // Poison DoT
        Burn,           // Fire DoT
        Corruption,     // Warlock - shadow damage
        Weaken,         // Reduced stats
        Vulnerability,  // Increased damage taken
        Expose,         // Reduced armor/resists
        Mark            // Generic marked state
    }

    /// <summary>
    /// Types of marks that can be applied
    /// </summary>
    public enum MarkType
    {
        None,
        BountyMark,     // Bounty Hunter - pursuit generation
        HuntersMark,    // Ranger - damage bonus
        DeathMark,      // Rogue/Assassin - execute threshold
        SoulMark,       // Warlock - soul shard on death
        Hex,            // Witch - curse amplification
        Divine          // Paladin - holy damage bonus
    }

    #endregion

    #region Stack Data

    /// <summary>
    /// Represents a single stack entry on a target
    /// </summary>
    public class StackEntry
    {
        public StackType Type { get; set; }
        public int Stacks { get; set; }
        public int MaxStacks { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Mobile AppliedBy { get; set; }

        // Optional DoT data
        public int DamagePerTick { get; set; }
        public TimeSpan TickInterval { get; set; }
        public DateTime LastTick { get; set; }

        public StackEntry(StackType type, int stacks, int maxStacks, TimeSpan duration, Mobile appliedBy)
        {
            Type = type;
            Stacks = stacks;
            MaxStacks = maxStacks;
            AppliedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow + duration;
            AppliedBy = appliedBy;
            DamagePerTick = 0;
            TickInterval = TimeSpan.Zero;
            LastTick = DateTime.UtcNow;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public TimeSpan RemainingDuration => IsExpired ? TimeSpan.Zero : ExpiresAt - DateTime.UtcNow;

        public void Refresh(TimeSpan duration)
        {
            ExpiresAt = DateTime.UtcNow + duration;
        }

        public void AddStacks(int amount)
        {
            Stacks = Math.Min(Stacks + amount, MaxStacks);
        }

        public int ConsumeStacks(int amount = -1)
        {
            if (amount < 0 || amount > Stacks)
            {
                int consumed = Stacks;
                Stacks = 0;
                return consumed;
            }
            Stacks -= amount;
            return amount;
        }
    }

    /// <summary>
    /// Represents a mark on a target
    /// </summary>
    public class MarkEntry
    {
        public MarkType Type { get; set; }
        public Mobile MarkedBy { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int BonusDamagePercent { get; set; }

        public MarkEntry(MarkType type, Mobile markedBy, TimeSpan duration, int bonusDamage = 0)
        {
            Type = type;
            MarkedBy = markedBy;
            AppliedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow + duration;
            BonusDamagePercent = bonusDamage;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public TimeSpan RemainingDuration => IsExpired ? TimeSpan.Zero : ExpiresAt - DateTime.UtcNow;
    }

    #endregion

    #region Target State

    /// <summary>
    /// Represents all tracked state for a single target from a single attacker's perspective
    /// </summary>
    public class TargetState
    {
        public Mobile Target { get; private set; }
        public Mobile Attacker { get; private set; }

        private Dictionary<StackType, StackEntry> m_Stacks;
        private MarkEntry m_Mark;
        private DateTime m_LastHitTime;
        private int m_TotalDamageDealt;

        // Threshold constants
        public const int CHILL_FROZEN_THRESHOLD = 5;
        public const int CURSE_MAX_THRESHOLD = 5;
        public const int COMBO_MAX = 5;
        public const int PURSUIT_MAX = 10;
        public const int BLEED_MAX = 5;
        public const int CORRUPTION_MAX = 3;

        public TargetState(Mobile target, Mobile attacker)
        {
            Target = target;
            Attacker = attacker;
            m_Stacks = new Dictionary<StackType, StackEntry>();
            m_Mark = null;
            m_LastHitTime = DateTime.MinValue;
            m_TotalDamageDealt = 0;
        }

        #region Stack Management

        public int GetStacks(StackType type)
        {
            if (m_Stacks.TryGetValue(type, out StackEntry entry))
            {
                if (entry.IsExpired)
                {
                    m_Stacks.Remove(type);
                    return 0;
                }
                return entry.Stacks;
            }
            return 0;
        }

        public StackEntry GetStackEntry(StackType type)
        {
            if (m_Stacks.TryGetValue(type, out StackEntry entry))
            {
                if (entry.IsExpired)
                {
                    m_Stacks.Remove(type);
                    return null;
                }
                return entry;
            }
            return null;
        }

        public void AddStacks(StackType type, int amount, TimeSpan duration, int maxStacks = -1)
        {
            if (maxStacks < 0)
            {
                maxStacks = GetDefaultMaxStacks(type);
            }

            if (m_Stacks.TryGetValue(type, out StackEntry entry))
            {
                if (entry.IsExpired)
                {
                    entry = new StackEntry(type, amount, maxStacks, duration, Attacker);
                    m_Stacks[type] = entry;
                }
                else
                {
                    entry.AddStacks(amount);
                    entry.Refresh(duration);
                }
            }
            else
            {
                entry = new StackEntry(type, amount, maxStacks, duration, Attacker);
                m_Stacks[type] = entry;
            }

            // Check for threshold triggers
            CheckStackThreshold(type, entry.Stacks);
        }

        public int ConsumeStacks(StackType type, int amount = -1)
        {
            if (m_Stacks.TryGetValue(type, out StackEntry entry))
            {
                if (entry.IsExpired)
                {
                    m_Stacks.Remove(type);
                    return 0;
                }

                int consumed = entry.ConsumeStacks(amount);
                if (entry.Stacks <= 0)
                {
                    m_Stacks.Remove(type);
                }
                return consumed;
            }
            return 0;
        }

        public void ClearStacks(StackType type)
        {
            m_Stacks.Remove(type);
        }

        public void ClearAllStacks()
        {
            m_Stacks.Clear();
        }

        private int GetDefaultMaxStacks(StackType type)
        {
            switch (type)
            {
                case StackType.Chill:
                    return CHILL_FROZEN_THRESHOLD;
                case StackType.Curse:
                    return CURSE_MAX_THRESHOLD;
                case StackType.Combo:
                    return COMBO_MAX;
                case StackType.Pursuit:
                    return PURSUIT_MAX;
                case StackType.Bleed:
                    return BLEED_MAX;
                case StackType.Corruption:
                    return CORRUPTION_MAX;
                default:
                    return 10;
            }
        }

        private void CheckStackThreshold(StackType type, int currentStacks)
        {
            // Trigger events when thresholds are reached
            switch (type)
            {
                case StackType.Chill:
                    if (currentStacks >= CHILL_FROZEN_THRESHOLD)
                    {
                        OnFrozenTriggered();
                    }
                    break;
                case StackType.Combo:
                    if (currentStacks >= COMBO_MAX)
                    {
                        OnMaxComboReached();
                    }
                    break;
            }
        }

        #endregion

        #region Threshold Events

        private void OnFrozenTriggered()
        {
            if (Target == null || Target.Deleted)
                return;

            // Apply frozen CC effect
            Target.Freeze(TimeSpan.FromSeconds(3));
            Target.SendMessage(0x22, "You have been frozen solid!");

            // Visual effect
            Effects.SendLocationParticles(
                EffectItem.Create(Target.Location, Target.Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 1153, 0, 5052, 0);
            Effects.PlaySound(Target.Location, Target.Map, 0x204);

            // Don't consume stacks here - let abilities decide
        }

        private void OnMaxComboReached()
        {
            if (Attacker != null)
            {
                Attacker.SendMessage(0x3B2, "Combo points maxed! Use a finisher!");
            }
        }

        #endregion

        #region Mark Management

        public bool HasMark => m_Mark != null && !m_Mark.IsExpired;
        public MarkType CurrentMark => m_Mark?.Type ?? MarkType.None;
        public MarkEntry Mark => (m_Mark != null && !m_Mark.IsExpired) ? m_Mark : null;

        public void ApplyMark(MarkType type, TimeSpan duration, int bonusDamagePercent = 0)
        {
            m_Mark = new MarkEntry(type, Attacker, duration, bonusDamagePercent);

            if (Target != null && !Target.Deleted)
            {
                Target.SendMessage(0x22, "You have been marked!");

                // Visual effect
                Effects.SendTargetParticles(Target, 0x374A, 10, 15, GetMarkHue(type), 0, 5013, EffectLayer.Waist, 0);
            }
        }

        public void RemoveMark()
        {
            m_Mark = null;
        }

        public bool IsMarkedBy(Mobile mobile)
        {
            return m_Mark != null && !m_Mark.IsExpired && m_Mark.MarkedBy == mobile;
        }

        private int GetMarkHue(MarkType type)
        {
            switch (type)
            {
                case MarkType.BountyMark:
                    return 1157; // Orange
                case MarkType.HuntersMark:
                    return 2010; // Green
                case MarkType.DeathMark:
                    return 1109; // Black
                case MarkType.SoulMark:
                    return 1175; // Purple
                case MarkType.Hex:
                    return 2073; // Murky green
                case MarkType.Divine:
                    return 1153; // White/gold
                default:
                    return 0;
            }
        }

        #endregion

        #region Combat Tracking

        public DateTime LastHitTime => m_LastHitTime;
        public int TotalDamageDealt => m_TotalDamageDealt;
        public TimeSpan TimeSinceLastHit => DateTime.UtcNow - m_LastHitTime;
        public bool IsInCombat => TimeSinceLastHit.TotalSeconds < 10;

        public void RecordHit(int damage)
        {
            m_LastHitTime = DateTime.UtcNow;
            m_TotalDamageDealt += damage;
        }

        #endregion

        #region DoT Processing

        public void ProcessDoTs()
        {
            List<StackType> toRemove = new List<StackType>();

            foreach (var kvp in m_Stacks)
            {
                StackEntry entry = kvp.Value;

                if (entry.IsExpired)
                {
                    toRemove.Add(kvp.Key);
                    continue;
                }

                if (entry.DamagePerTick > 0 && entry.TickInterval > TimeSpan.Zero)
                {
                    if (DateTime.UtcNow - entry.LastTick >= entry.TickInterval)
                    {
                        ApplyDoTDamage(entry);
                        entry.LastTick = DateTime.UtcNow;
                    }
                }
            }

            foreach (var type in toRemove)
            {
                m_Stacks.Remove(type);
            }
        }

        private void ApplyDoTDamage(StackEntry entry)
        {
            if (Target == null || Target.Deleted || !Target.Alive)
                return;

            int damage = entry.DamagePerTick * entry.Stacks;

            // Determine damage type based on stack type
            int physDmg = 0, fireDmg = 0, coldDmg = 0, poisDmg = 0, energyDmg = 0;

            switch (entry.Type)
            {
                case StackType.Bleed:
                    physDmg = 100;
                    break;
                case StackType.Burn:
                    fireDmg = 100;
                    break;
                case StackType.Poison:
                    poisDmg = 100;
                    break;
                case StackType.Corruption:
                    energyDmg = 50;
                    poisDmg = 50; // Shadow damage approximated
                    break;
                default:
                    physDmg = 100;
                    break;
            }

            if (Attacker != null && !Attacker.Deleted)
            {
                AOS.Damage(Target, Attacker, damage, physDmg, fireDmg, coldDmg, poisDmg, energyDmg);
            }
        }

        public void SetDoT(StackType type, int damagePerTick, TimeSpan tickInterval)
        {
            if (m_Stacks.TryGetValue(type, out StackEntry entry))
            {
                entry.DamagePerTick = damagePerTick;
                entry.TickInterval = tickInterval;
                entry.LastTick = DateTime.UtcNow;
            }
        }

        #endregion

        #region Serialization

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)1); // version

            // Stacks
            writer.Write(m_Stacks.Count);
            foreach (var kvp in m_Stacks)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value.Stacks);
                writer.Write(kvp.Value.MaxStacks);
                writer.Write(kvp.Value.ExpiresAt);
                writer.Write(kvp.Value.AppliedBy);
                writer.Write(kvp.Value.DamagePerTick);
                writer.Write(kvp.Value.TickInterval);
            }

            // Mark
            writer.Write(m_Mark != null);
            if (m_Mark != null)
            {
                writer.Write((int)m_Mark.Type);
                writer.Write(m_Mark.MarkedBy);
                writer.Write(m_Mark.ExpiresAt);
                writer.Write(m_Mark.BonusDamagePercent);
            }

            // Combat
            writer.Write(m_LastHitTime);
            writer.Write(m_TotalDamageDealt);
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            // Stacks
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                StackType type = (StackType)reader.ReadInt();
                int stacks = reader.ReadInt();
                int maxStacks = reader.ReadInt();
                DateTime expires = reader.ReadDateTime();
                Mobile appliedBy = reader.ReadMobile();
                int dmgPerTick = reader.ReadInt();
                TimeSpan tickInterval = reader.ReadTimeSpan();

                if (expires > DateTime.UtcNow)
                {
                    TimeSpan remaining = expires - DateTime.UtcNow;
                    StackEntry entry = new StackEntry(type, stacks, maxStacks, remaining, appliedBy);
                    entry.DamagePerTick = dmgPerTick;
                    entry.TickInterval = tickInterval;
                    m_Stacks[type] = entry;
                }
            }

            // Mark
            if (reader.ReadBool())
            {
                MarkType markType = (MarkType)reader.ReadInt();
                Mobile markedBy = reader.ReadMobile();
                DateTime markExpires = reader.ReadDateTime();
                int bonusDmg = reader.ReadInt();

                if (markExpires > DateTime.UtcNow)
                {
                    TimeSpan remaining = markExpires - DateTime.UtcNow;
                    m_Mark = new MarkEntry(markType, markedBy, remaining, bonusDmg);
                }
            }

            // Combat
            m_LastHitTime = reader.ReadDateTime();
            m_TotalDamageDealt = reader.ReadInt();
        }

        #endregion
    }

    #endregion

    #region Target Tracker Manager

    /// <summary>
    /// Manages per-target tracking for all attackers
    /// </summary>
    public class VystiaTargetTracker
    {
        private static VystiaTargetTracker m_Instance;
        public static VystiaTargetTracker Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new VystiaTargetTracker();
                return m_Instance;
            }
        }

        // Dictionary: Target -> (Attacker -> TargetState)
        private Dictionary<Mobile, Dictionary<Mobile, TargetState>> m_TargetStates;
        private Timer m_CleanupTimer;
        private Timer m_DoTTimer;

        public VystiaTargetTracker()
        {
            m_TargetStates = new Dictionary<Mobile, Dictionary<Mobile, TargetState>>();

            // Cleanup expired entries every 30 seconds
            m_CleanupTimer = Timer.DelayCall(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30), CleanupExpired);

            // Process DoTs every second
            m_DoTTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), ProcessAllDoTs);
        }

        #region State Access

        public TargetState GetState(Mobile attacker, Mobile target)
        {
            if (attacker == null || target == null)
                return null;

            if (!m_TargetStates.TryGetValue(target, out Dictionary<Mobile, TargetState> attackerStates))
            {
                attackerStates = new Dictionary<Mobile, TargetState>();
                m_TargetStates[target] = attackerStates;
            }

            if (!attackerStates.TryGetValue(attacker, out TargetState state))
            {
                state = new TargetState(target, attacker);
                attackerStates[attacker] = state;
            }

            return state;
        }

        public bool HasState(Mobile attacker, Mobile target)
        {
            if (m_TargetStates.TryGetValue(target, out Dictionary<Mobile, TargetState> attackerStates))
            {
                return attackerStates.ContainsKey(attacker);
            }
            return false;
        }

        public void ClearState(Mobile attacker, Mobile target)
        {
            if (m_TargetStates.TryGetValue(target, out Dictionary<Mobile, TargetState> attackerStates))
            {
                attackerStates.Remove(attacker);
                if (attackerStates.Count == 0)
                {
                    m_TargetStates.Remove(target);
                }
            }
        }

        public void ClearAllStatesForTarget(Mobile target)
        {
            m_TargetStates.Remove(target);
        }

        public void ClearAllStatesFromAttacker(Mobile attacker)
        {
            List<Mobile> emptyTargets = new List<Mobile>();

            foreach (var kvp in m_TargetStates)
            {
                kvp.Value.Remove(attacker);
                if (kvp.Value.Count == 0)
                {
                    emptyTargets.Add(kvp.Key);
                }
            }

            foreach (Mobile target in emptyTargets)
            {
                m_TargetStates.Remove(target);
            }
        }

        #endregion

        #region Quick Access Methods

        public int GetStacks(Mobile attacker, Mobile target, StackType type)
        {
            TargetState state = GetState(attacker, target);
            return state?.GetStacks(type) ?? 0;
        }

        public void AddStacks(Mobile attacker, Mobile target, StackType type, int amount, TimeSpan duration)
        {
            TargetState state = GetState(attacker, target);
            state?.AddStacks(type, amount, duration);
        }

        public int ConsumeStacks(Mobile attacker, Mobile target, StackType type, int amount = -1)
        {
            TargetState state = GetState(attacker, target);
            return state?.ConsumeStacks(type, amount) ?? 0;
        }

        public bool IsFrozen(Mobile attacker, Mobile target)
        {
            return GetStacks(attacker, target, StackType.Chill) >= TargetState.CHILL_FROZEN_THRESHOLD;
        }

        public void ApplyMark(Mobile attacker, Mobile target, MarkType type, TimeSpan duration, int bonusDamage = 0)
        {
            TargetState state = GetState(attacker, target);
            state?.ApplyMark(type, duration, bonusDamage);
        }

        public bool IsMarked(Mobile attacker, Mobile target)
        {
            TargetState state = GetState(attacker, target);
            return state?.HasMark ?? false;
        }

        public MarkType GetMarkType(Mobile attacker, Mobile target)
        {
            TargetState state = GetState(attacker, target);
            return state?.CurrentMark ?? MarkType.None;
        }

        public void RecordHit(Mobile attacker, Mobile target, int damage)
        {
            TargetState state = GetState(attacker, target);
            state?.RecordHit(damage);
        }

        #endregion

        #region DoT and Cleanup

        private void ProcessAllDoTs()
        {
            foreach (var targetKvp in m_TargetStates)
            {
                foreach (var attackerKvp in targetKvp.Value)
                {
                    attackerKvp.Value.ProcessDoTs();
                }
            }
        }

        private void CleanupExpired()
        {
            List<Mobile> emptyTargets = new List<Mobile>();

            foreach (var targetKvp in m_TargetStates)
            {
                Mobile target = targetKvp.Key;

                // Remove states for deleted/dead targets
                if (target == null || target.Deleted)
                {
                    emptyTargets.Add(target);
                    continue;
                }

                List<Mobile> emptyAttackers = new List<Mobile>();

                foreach (var attackerKvp in targetKvp.Value)
                {
                    Mobile attacker = attackerKvp.Key;
                    TargetState state = attackerKvp.Value;

                    // Remove states for deleted attackers
                    if (attacker == null || attacker.Deleted)
                    {
                        emptyAttackers.Add(attacker);
                        continue;
                    }

                    // Remove stale combat states (no interaction in 5 minutes)
                    if (state.TimeSinceLastHit.TotalMinutes > 5 && !state.HasMark)
                    {
                        // Check if any stacks remain
                        bool hasActiveStacks = false;
                        foreach (StackType type in Enum.GetValues(typeof(StackType)))
                        {
                            if (state.GetStacks(type) > 0)
                            {
                                hasActiveStacks = true;
                                break;
                            }
                        }

                        if (!hasActiveStacks)
                        {
                            emptyAttackers.Add(attacker);
                        }
                    }
                }

                foreach (Mobile attacker in emptyAttackers)
                {
                    targetKvp.Value.Remove(attacker);
                }

                if (targetKvp.Value.Count == 0)
                {
                    emptyTargets.Add(target);
                }
            }

            foreach (Mobile target in emptyTargets)
            {
                m_TargetStates.Remove(target);
            }
        }

        #endregion

        #region GM Commands

        public static void Initialize()
        {
            CommandSystem.Register("GetTargetStacks", AccessLevel.GameMaster, GetTargetStacks_OnCommand);
            CommandSystem.Register("SetTargetStacks", AccessLevel.GameMaster, SetTargetStacks_OnCommand);
            CommandSystem.Register("ClearTargetStacks", AccessLevel.GameMaster, ClearTargetStacks_OnCommand);
            CommandSystem.Register("TestMark", AccessLevel.GameMaster, TestMark_OnCommand);
        }

        [Usage("GetTargetStacks")]
        [Description("Shows all stack states on a target from your perspective")]
        private static void GetTargetStacks_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target a creature to view its stacks:");
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    TargetState state = Instance.GetState(e.Mobile, m);
                    e.Mobile.SendMessage("=== Target State for {0} ===", m.Name);

                    foreach (StackType type in Enum.GetValues(typeof(StackType)))
                    {
                        int stacks = state.GetStacks(type);
                        if (stacks > 0)
                        {
                            StackEntry entry = state.GetStackEntry(type);
                            e.Mobile.SendMessage("  {0}: {1} stacks (expires in {2:F1}s)",
                                type, stacks, entry.RemainingDuration.TotalSeconds);
                        }
                    }

                    if (state.HasMark)
                    {
                        e.Mobile.SendMessage("  MARK: {0} (+{1}% damage)",
                            state.CurrentMark, state.Mark.BonusDamagePercent);
                    }

                    e.Mobile.SendMessage("  Total Damage Dealt: {0}", state.TotalDamageDealt);
                    e.Mobile.SendMessage("  In Combat: {0}", state.IsInCombat);
                }
            });
        }

        [Usage("SetTargetStacks <type> <amount>")]
        [Description("Sets stack count on a target")]
        private static void SetTargetStacks_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 2)
            {
                e.Mobile.SendMessage("Usage: SetTargetStacks <type> <amount>");
                e.Mobile.SendMessage("Types: Chill, Curse, Combo, Pursuit, Bleed, Poison, Burn, Corruption");
                return;
            }

            if (!Enum.TryParse(e.Arguments[0], true, out StackType type))
            {
                e.Mobile.SendMessage("Invalid stack type: {0}", e.Arguments[0]);
                return;
            }

            if (!int.TryParse(e.Arguments[1], out int amount))
            {
                e.Mobile.SendMessage("Invalid amount: {0}", e.Arguments[1]);
                return;
            }

            e.Mobile.SendMessage("Target a creature to set {0} to {1} stacks:", type, amount);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    TargetState state = Instance.GetState(e.Mobile, m);
                    state.ClearStacks(type);
                    if (amount > 0)
                    {
                        state.AddStacks(type, amount, TimeSpan.FromSeconds(60));
                    }
                    e.Mobile.SendMessage("Set {0} to {1} stacks on {2}", type, amount, m.Name);
                }
            });
        }

        [Usage("ClearTargetStacks")]
        [Description("Clears all stacks on a target")]
        private static void ClearTargetStacks_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target a creature to clear all stacks:");
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    TargetState state = Instance.GetState(e.Mobile, m);
                    state.ClearAllStacks();
                    state.RemoveMark();
                    e.Mobile.SendMessage("Cleared all stacks and marks on {0}", m.Name);
                }
            });
        }

        [Usage("TestMark <type>")]
        [Description("Applies a test mark to a target")]
        private static void TestMark_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage("Usage: TestMark <type>");
                e.Mobile.SendMessage("Types: BountyMark, HuntersMark, DeathMark, SoulMark, Hex, Divine");
                return;
            }

            if (!Enum.TryParse(e.Arguments[0], true, out MarkType type))
            {
                e.Mobile.SendMessage("Invalid mark type: {0}", e.Arguments[0]);
                return;
            }

            e.Mobile.SendMessage("Target a creature to apply {0}:", type);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    Instance.ApplyMark(e.Mobile, m, type, TimeSpan.FromSeconds(30), 25);
                    e.Mobile.SendMessage("Applied {0} to {1} for 30 seconds (+25% damage)", type, m.Name);
                }
            });
        }

        private class InternalTarget : Server.Targeting.Target
        {
            private Action<object> m_Callback;

            public InternalTarget(Action<object> callback) : base(12, false, Server.Targeting.TargetFlags.None)
            {
                m_Callback = callback;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                m_Callback?.Invoke(targeted);
            }
        }

        #endregion
    }

    #endregion
}
