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
    /// Types of crowd control effects
    /// </summary>
    public enum CCType
    {
        Stun,           // Complete incapacitation
        Freeze,         // Cannot move, can still cast/attack
        Root,           // Cannot move, can cast/attack
        Silence,        // Cannot cast spells
        Fear,           // Forced fleeing
        Sleep,          // Incapacitated, breaks on damage
        Charm,          // Attacks allies
        Knockback,      // Pushed away
        Knockdown,      // Prone on ground
        Slow,           // Reduced movement/attack speed
        Blind,          // Reduced hit chance
        Disarm,         // Cannot use weapons
        Pacify,         // Cannot attack
        Confuse,        // Random actions
        Polymorph       // Transformed, cannot use abilities
    }

    /// <summary>
    /// Categories for diminishing returns
    /// </summary>
    public enum DRCategory
    {
        Stun,           // Stun, Knockdown
        Incapacitate,   // Fear, Sleep, Charm
        Root,           // Root, Freeze
        Silence,        // Silence, Pacify, Disarm
        None            // Slow, Blind (no DR)
    }

    #endregion

    #region CC Entry

    /// <summary>
    /// Represents an active CC effect on a target
    /// </summary>
    public class CCEntry
    {
        public CCType Type { get; set; }
        public Mobile Target { get; set; }
        public Mobile Source { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public TimeSpan OriginalDuration { get; set; }
        public int DRLevel { get; set; }    // 0 = full, 1 = 50%, 2 = 25%, 3+ = immune

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public TimeSpan RemainingDuration => IsExpired ? TimeSpan.Zero : ExpiresAt - DateTime.UtcNow;

        public CCEntry(CCType type, Mobile target, Mobile source, TimeSpan duration, int drLevel)
        {
            Type = type;
            Target = target;
            Source = source;
            AppliedAt = DateTime.UtcNow;
            OriginalDuration = duration;
            DRLevel = drLevel;

            // Apply DR to duration
            TimeSpan actualDuration = ApplyDR(duration, drLevel);
            ExpiresAt = DateTime.UtcNow + actualDuration;
        }

        private TimeSpan ApplyDR(TimeSpan baseDuration, int level)
        {
            switch (level)
            {
                case 0: return baseDuration;                                        // 100%
                case 1: return TimeSpan.FromTicks(baseDuration.Ticks / 2);         // 50%
                case 2: return TimeSpan.FromTicks(baseDuration.Ticks / 4);         // 25%
                default: return TimeSpan.Zero;                                      // Immune
            }
        }
    }

    /// <summary>
    /// Tracks DR history for a target
    /// </summary>
    public class DRHistory
    {
        public DRCategory Category { get; set; }
        public int Level { get; set; }              // Current DR level
        public DateTime LastApplied { get; set; }
        public DateTime ImmunityExpires { get; set; }

        public const double DR_RESET_TIME = 15.0;   // Seconds before DR resets
        public const double IMMUNITY_DURATION = 15.0; // Seconds of immunity after max DR

        public DRHistory(DRCategory category)
        {
            Category = category;
            Level = 0;
            LastApplied = DateTime.MinValue;
            ImmunityExpires = DateTime.MinValue;
        }

        public bool IsImmune => DateTime.UtcNow < ImmunityExpires;

        public void IncrementDR()
        {
            // Check if DR has reset
            if ((DateTime.UtcNow - LastApplied).TotalSeconds > DR_RESET_TIME)
            {
                Level = 0;
            }

            Level++;
            LastApplied = DateTime.UtcNow;

            // At level 3, grant immunity
            if (Level >= 3)
            {
                ImmunityExpires = DateTime.UtcNow + TimeSpan.FromSeconds(IMMUNITY_DURATION);
            }
        }

        public int GetCurrentLevel()
        {
            // Check if DR has reset
            if ((DateTime.UtcNow - LastApplied).TotalSeconds > DR_RESET_TIME)
            {
                Level = 0;
            }

            return Level;
        }
    }

    #endregion

    #region CC Manager

    /// <summary>
    /// Manages all crowd control effects and diminishing returns
    /// </summary>
    public class CrowdControlManager
    {
        private static CrowdControlManager m_Instance;
        public static CrowdControlManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new CrowdControlManager();
                return m_Instance;
            }
        }

        // Target -> List of active CCs
        private Dictionary<Mobile, List<CCEntry>> m_ActiveCC;

        // Target -> DR history per category
        private Dictionary<Mobile, Dictionary<DRCategory, DRHistory>> m_DRHistory;

        private Timer m_TickTimer;

        public CrowdControlManager()
        {
            m_ActiveCC = new Dictionary<Mobile, List<CCEntry>>();
            m_DRHistory = new Dictionary<Mobile, Dictionary<DRCategory, DRHistory>>();

            // Tick every 250ms for responsive CC handling
            m_TickTimer = Timer.DelayCall(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250), ProcessTicks);
        }

        #region DR Category Mapping

        private DRCategory GetDRCategory(CCType type)
        {
            switch (type)
            {
                case CCType.Stun:
                case CCType.Knockdown:
                    return DRCategory.Stun;

                case CCType.Fear:
                case CCType.Sleep:
                case CCType.Charm:
                case CCType.Confuse:
                    return DRCategory.Incapacitate;

                case CCType.Root:
                case CCType.Freeze:
                    return DRCategory.Root;

                case CCType.Silence:
                case CCType.Pacify:
                case CCType.Disarm:
                    return DRCategory.Silence;

                case CCType.Slow:
                case CCType.Blind:
                case CCType.Knockback:
                case CCType.Polymorph:
                default:
                    return DRCategory.None;
            }
        }

        #endregion

        #region CC Application

        /// <summary>
        /// Apply a CC effect to a target with DR calculation
        /// </summary>
        public CCEntry ApplyCC(Mobile target, Mobile source, CCType type, TimeSpan duration)
        {
            if (target == null || target.Deleted || !target.Alive)
                return null;

            DRCategory drCategory = GetDRCategory(type);
            int drLevel = 0;

            // Check for immunity and DR level
            if (drCategory != DRCategory.None)
            {
                DRHistory history = GetDRHistory(target, drCategory);

                if (history.IsImmune)
                {
                    target.SendMessage(0x3B2, "You are immune to {0}!", type);
                    source?.SendMessage(0x22, "{0} is immune to {1}!", target.Name, type);
                    return null;
                }

                drLevel = history.GetCurrentLevel();

                if (drLevel >= 3)
                {
                    target.SendMessage(0x3B2, "You are immune to {0}!", type);
                    source?.SendMessage(0x22, "{0} is immune to {1}!", target.Name, type);
                    return null;
                }

                history.IncrementDR();
            }

            // Create CC entry
            CCEntry entry = new CCEntry(type, target, source, duration, drLevel);

            if (entry.RemainingDuration <= TimeSpan.Zero)
            {
                target.SendMessage(0x3B2, "You resist the {0} effect!", type);
                return null;
            }

            // Get or create CC list for target
            if (!m_ActiveCC.TryGetValue(target, out List<CCEntry> ccList))
            {
                ccList = new List<CCEntry>();
                m_ActiveCC[target] = ccList;
            }

            // Remove existing CC of same type
            ccList.RemoveAll(c => c.Type == type);

            // Add new CC
            ccList.Add(entry);

            // Apply the actual effect
            ApplyCCEffect(entry);

            // Notify
            string drText = drLevel > 0 ? string.Format(" (DR {0})", drLevel) : "";
            target.SendMessage(0x22, "You are {0}!{1}", GetCCDescription(type), drText);
            source?.SendMessage(0x3B2, "{0} is {1}!{2}", target.Name, GetCCDescription(type), drText);

            return entry;
        }

        private void ApplyCCEffect(CCEntry entry)
        {
            Mobile target = entry.Target;
            TimeSpan duration = entry.RemainingDuration;

            switch (entry.Type)
            {
                case CCType.Stun:
                case CCType.Knockdown:
                    target.Freeze(duration);
                    target.FixedParticles(0x376A, 9, 32, 5030, 0, 0, EffectLayer.Waist);
                    target.PlaySound(0x204);
                    break;

                case CCType.Freeze:
                    target.Freeze(duration);
                    target.FixedParticles(0x376A, 9, 32, 5030, 1153, 0, EffectLayer.Waist);
                    target.PlaySound(0x204);
                    break;

                case CCType.Root:
                    // Can't move but can attack/cast
                    target.CantWalk = true;
                    Timer.DelayCall(duration, () =>
                    {
                        if (target != null && !target.Deleted)
                            target.CantWalk = false;
                    });
                    target.FixedParticles(0x376A, 9, 32, 5030, 2010, 0, EffectLayer.Waist);
                    break;

                case CCType.Silence:
                    // Apply silence debuff via buff system
                    VystiaBuffManager.Instance.ApplyBuff(target, entry.Source, VystiaBuffType.Silenced, duration);
                    target.FixedParticles(0x376A, 9, 32, 5030, 1154, 0, EffectLayer.Head);
                    break;

                case CCType.Fear:
                    // Force fleeing behavior
                    if (target is BaseCreature bc)
                    {
                        bc.BeginFlee(duration);
                    }
                    else
                    {
                        // For players, apply fear debuff
                        VystiaBuffManager.Instance.ApplyBuff(target, entry.Source, VystiaBuffType.Pacified, duration);
                    }
                    target.FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Head);
                    target.PlaySound(0x1D3);
                    break;

                case CCType.Sleep:
                    target.Freeze(duration);
                    // Sleep breaks on damage - handled in ProcessIncomingDamage
                    target.FixedParticles(0x376A, 9, 32, 5030, 2073, 0, EffectLayer.Head);
                    break;

                case CCType.Blind:
                    VystiaBuffManager.Instance.ApplyBuff(target, entry.Source, VystiaBuffType.DexterityDebuff, duration, 20);
                    target.FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Head);
                    break;

                case CCType.Slow:
                    VystiaBuffManager.Instance.ApplyBuff(target, entry.Source, VystiaBuffType.SlowDebuff, duration, 30);
                    target.FixedParticles(0x376A, 9, 32, 5030, 2073, 0, EffectLayer.Waist);
                    break;

                case CCType.Knockback:
                    // Knockback is instant, apply push
                    if (entry.Source != null)
                    {
                        Direction pushDir = entry.Source.GetDirectionTo(target);
                        PushMobile(target, pushDir, 3);
                    }
                    break;

                case CCType.Disarm:
                    // Disarm the target's weapon temporarily
                    DisarmTarget(target, duration);
                    break;

                case CCType.Polymorph:
                    // Transform target
                    VystiaBuffManager.Instance.ApplyBuff(target, entry.Source, VystiaBuffType.WolfForm, duration);
                    break;
            }
        }

        private void PushMobile(Mobile m, Direction dir, int tiles)
        {
            if (m == null || m.Map == null)
                return;

            int xOffset = 0, yOffset = 0;

            switch (dir & Direction.Mask)
            {
                case Direction.North: yOffset = -1; break;
                case Direction.South: yOffset = 1; break;
                case Direction.West: xOffset = -1; break;
                case Direction.East: xOffset = 1; break;
                case Direction.Up: xOffset = -1; yOffset = -1; break;
                case Direction.Down: xOffset = 1; yOffset = 1; break;
                case Direction.Left: xOffset = -1; yOffset = 1; break;
                case Direction.Right: xOffset = 1; yOffset = -1; break;
            }

            Point3D newLoc = m.Location;
            for (int i = 0; i < tiles; i++)
            {
                Point3D testLoc = new Point3D(newLoc.X + xOffset, newLoc.Y + yOffset, newLoc.Z);
                if (m.Map.CanFit(testLoc.X, testLoc.Y, testLoc.Z, 16, false, false))
                {
                    newLoc = testLoc;
                }
                else
                {
                    break;
                }
            }

            if (newLoc != m.Location)
            {
                m.MoveToWorld(newLoc, m.Map);
                m.PlaySound(0x11C);
            }
        }

        private void DisarmTarget(Mobile target, TimeSpan duration)
        {
            if (target == null)
                return;

            Item weapon = target.FindItemOnLayer(Layer.OneHanded);
            if (weapon == null)
                weapon = target.FindItemOnLayer(Layer.TwoHanded);

            if (weapon != null && target.Backpack != null)
            {
                target.Backpack.DropItem(weapon);
                target.SendMessage(0x22, "You have been disarmed!");

                // Auto re-equip after duration (simplified - just notify)
                Timer.DelayCall(duration, () =>
                {
                    target?.SendMessage(0x3B2, "You can equip weapons again.");
                });
            }
        }

        private string GetCCDescription(CCType type)
        {
            switch (type)
            {
                case CCType.Stun: return "stunned";
                case CCType.Freeze: return "frozen";
                case CCType.Root: return "rooted";
                case CCType.Silence: return "silenced";
                case CCType.Fear: return "feared";
                case CCType.Sleep: return "asleep";
                case CCType.Charm: return "charmed";
                case CCType.Knockback: return "knocked back";
                case CCType.Knockdown: return "knocked down";
                case CCType.Slow: return "slowed";
                case CCType.Blind: return "blinded";
                case CCType.Disarm: return "disarmed";
                case CCType.Pacify: return "pacified";
                case CCType.Confuse: return "confused";
                case CCType.Polymorph: return "polymorphed";
                default: return type.ToString().ToLower();
            }
        }

        #endregion

        #region CC Removal

        public void RemoveCC(Mobile target, CCType type, string reason = "")
        {
            if (!m_ActiveCC.TryGetValue(target, out List<CCEntry> ccList))
                return;

            CCEntry entry = ccList.Find(c => c.Type == type);
            if (entry != null)
            {
                ccList.Remove(entry);
                RemoveCCEffect(entry);

                if (!string.IsNullOrEmpty(reason))
                {
                    target.SendMessage(0x3B2, "{0} broken: {1}", type, reason);
                }
                else
                {
                    target.SendMessage(0x3B2, "You are no longer {0}.", GetCCDescription(type));
                }
            }

            if (ccList.Count == 0)
            {
                m_ActiveCC.Remove(target);
            }
        }

        public void RemoveAllCC(Mobile target, bool fromDispel = false)
        {
            if (!m_ActiveCC.TryGetValue(target, out List<CCEntry> ccList))
                return;

            foreach (CCEntry entry in ccList.ToArray())
            {
                RemoveCCEffect(entry);
            }

            ccList.Clear();
            m_ActiveCC.Remove(target);

            target.SendMessage(0x3B2, "All crowd control effects removed!");
        }

        private void RemoveCCEffect(CCEntry entry)
        {
            Mobile target = entry.Target;
            if (target == null || target.Deleted)
                return;

            switch (entry.Type)
            {
                case CCType.Stun:
                case CCType.Freeze:
                case CCType.Knockdown:
                case CCType.Sleep:
                    target.Frozen = false;
                    break;

                case CCType.Root:
                    target.CantWalk = false;
                    break;

                case CCType.Silence:
                    VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.Silenced);
                    break;

                case CCType.Fear:
                    // Fear removal - creature stops fleeing naturally when effect ends
                    VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.Pacified);
                    break;

                case CCType.Blind:
                    VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.DexterityDebuff);
                    break;

                case CCType.Slow:
                    VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.SlowDebuff);
                    break;

                case CCType.Polymorph:
                    VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.WolfForm);
                    break;
            }
        }

        #endregion

        #region CC Queries

        public bool HasCC(Mobile target, CCType type)
        {
            if (!m_ActiveCC.TryGetValue(target, out List<CCEntry> ccList))
                return false;

            return ccList.Exists(c => c.Type == type && !c.IsExpired);
        }

        public bool HasAnyCC(Mobile target)
        {
            if (!m_ActiveCC.TryGetValue(target, out List<CCEntry> ccList))
                return false;

            return ccList.Exists(c => !c.IsExpired);
        }

        public List<CCEntry> GetActiveCC(Mobile target)
        {
            if (!m_ActiveCC.TryGetValue(target, out List<CCEntry> ccList))
                return new List<CCEntry>();

            return ccList.FindAll(c => !c.IsExpired);
        }

        public bool IsImmune(Mobile target, CCType type)
        {
            DRCategory category = GetDRCategory(type);
            if (category == DRCategory.None)
                return false;

            DRHistory history = GetDRHistory(target, category);
            return history.IsImmune || history.GetCurrentLevel() >= 3;
        }

        #endregion

        #region DR Management

        private DRHistory GetDRHistory(Mobile target, DRCategory category)
        {
            if (!m_DRHistory.TryGetValue(target, out Dictionary<DRCategory, DRHistory> history))
            {
                history = new Dictionary<DRCategory, DRHistory>();
                m_DRHistory[target] = history;
            }

            if (!history.TryGetValue(category, out DRHistory drHistory))
            {
                drHistory = new DRHistory(category);
                history[category] = drHistory;
            }

            return drHistory;
        }

        public int GetDRLevel(Mobile target, CCType type)
        {
            DRCategory category = GetDRCategory(type);
            if (category == DRCategory.None)
                return 0;

            DRHistory history = GetDRHistory(target, category);
            return history.GetCurrentLevel();
        }

        public void ResetDR(Mobile target, DRCategory? category = null)
        {
            if (!m_DRHistory.TryGetValue(target, out Dictionary<DRCategory, DRHistory> history))
                return;

            if (category.HasValue)
            {
                history.Remove(category.Value);
            }
            else
            {
                history.Clear();
            }
        }

        #endregion

        #region Damage Interaction

        /// <summary>
        /// Called when target takes damage - breaks sleep and similar effects
        /// </summary>
        public void OnDamageTaken(Mobile target, int damage, Mobile source)
        {
            if (!m_ActiveCC.TryGetValue(target, out List<CCEntry> ccList))
                return;

            List<CCEntry> toRemove = new List<CCEntry>();

            foreach (CCEntry entry in ccList)
            {
                switch (entry.Type)
                {
                    case CCType.Sleep:
                        toRemove.Add(entry);
                        break;

                    case CCType.Charm:
                        if (damage > 10) // Only high damage breaks charm
                            toRemove.Add(entry);
                        break;
                }
            }

            foreach (CCEntry entry in toRemove)
            {
                RemoveCC(target, entry.Type, "damage taken");
            }
        }

        #endregion

        #region Tick Processing

        private void ProcessTicks()
        {
            List<Mobile> emptyTargets = new List<Mobile>();

            foreach (var kvp in m_ActiveCC)
            {
                Mobile target = kvp.Key;
                List<CCEntry> ccList = kvp.Value;

                if (target == null || target.Deleted)
                {
                    emptyTargets.Add(target);
                    continue;
                }

                List<CCEntry> expired = new List<CCEntry>();

                foreach (CCEntry entry in ccList)
                {
                    if (entry.IsExpired)
                    {
                        expired.Add(entry);
                    }
                }

                foreach (CCEntry entry in expired)
                {
                    RemoveCCEffect(entry);
                    ccList.Remove(entry);
                    target.SendMessage(0x3B2, "You are no longer {0}.", GetCCDescription(entry.Type));
                }

                if (ccList.Count == 0)
                {
                    emptyTargets.Add(target);
                }
            }

            foreach (Mobile target in emptyTargets)
            {
                m_ActiveCC.Remove(target);
            }
        }

        #endregion

        #region GM Commands

        public static void Initialize()
        {
            CommandSystem.Register("ApplyCC", AccessLevel.GameMaster, ApplyCC_OnCommand);
            CommandSystem.Register("RemoveCC", AccessLevel.GameMaster, RemoveCC_OnCommand);
            CommandSystem.Register("ListCC", AccessLevel.GameMaster, ListCC_OnCommand);
            CommandSystem.Register("CheckDR", AccessLevel.GameMaster, CheckDR_OnCommand);
            CommandSystem.Register("ResetDR", AccessLevel.GameMaster, ResetDR_OnCommand);
        }

        [Usage("ApplyCC <type> [duration]")]
        [Description("Applies a CC effect to a target")]
        private static void ApplyCC_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage("Usage: ApplyCC <type> [duration]");
                e.Mobile.SendMessage("Types: Stun, Freeze, Root, Silence, Fear, Sleep, Slow, Blind, Knockback, Disarm");
                return;
            }

            if (!Enum.TryParse(e.Arguments[0], true, out CCType type))
            {
                e.Mobile.SendMessage("Invalid CC type: {0}", e.Arguments[0]);
                return;
            }

            int duration = 5;
            if (e.Arguments.Length >= 2)
            {
                int.TryParse(e.Arguments[1], out duration);
            }

            e.Mobile.SendMessage("Target a creature to apply {0} for {1} seconds:", type, duration);
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    CCEntry entry = Instance.ApplyCC(m, e.Mobile, type, TimeSpan.FromSeconds(duration));
                    if (entry != null)
                    {
                        e.Mobile.SendMessage("Applied {0} to {1} (DR level: {2}, actual duration: {3:F1}s)",
                            type, m.Name, entry.DRLevel, entry.RemainingDuration.TotalSeconds);
                    }
                }
            });
        }

        [Usage("RemoveCC <type>")]
        [Description("Removes a specific CC from a target")]
        private static void RemoveCC_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length < 1)
            {
                e.Mobile.SendMessage("Usage: RemoveCC <type> (or 'all')");
                return;
            }

            bool removeAll = e.Arguments[0].ToLower() == "all";
            CCType type = CCType.Stun;

            if (!removeAll && !Enum.TryParse(e.Arguments[0], true, out type))
            {
                e.Mobile.SendMessage("Invalid CC type: {0}", e.Arguments[0]);
                return;
            }

            e.Mobile.SendMessage("Target a creature to remove CC:");
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    if (removeAll)
                    {
                        Instance.RemoveAllCC(m, true);
                        e.Mobile.SendMessage("Removed all CC from {0}", m.Name);
                    }
                    else
                    {
                        Instance.RemoveCC(m, type, "GM command");
                        e.Mobile.SendMessage("Removed {0} from {1}", type, m.Name);
                    }
                }
            });
        }

        [Usage("ListCC")]
        [Description("Lists all active CC effects on a target")]
        private static void ListCC_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target a creature to list CC:");
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    List<CCEntry> ccList = Instance.GetActiveCC(m);
                    e.Mobile.SendMessage("=== Active CC on {0} ({1}) ===", m.Name, ccList.Count);

                    foreach (CCEntry entry in ccList)
                    {
                        e.Mobile.SendMessage("  {0}: {1:F1}s remaining (DR level {2})",
                            entry.Type, entry.RemainingDuration.TotalSeconds, entry.DRLevel);
                    }
                }
            });
        }

        [Usage("CheckDR")]
        [Description("Shows DR levels for all categories on a target")]
        private static void CheckDR_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target a creature to check DR:");
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    e.Mobile.SendMessage("=== DR Levels for {0} ===", m.Name);

                    foreach (DRCategory cat in Enum.GetValues(typeof(DRCategory)))
                    {
                        if (cat == DRCategory.None)
                            continue;

                        int level = Instance.GetDRLevel(m, CCType.Stun); // Just need any CC to check category
                        bool immune = Instance.IsImmune(m, CCType.Stun);

                        e.Mobile.SendMessage("  {0}: Level {1}{2}",
                            cat, level, immune ? " (IMMUNE)" : "");
                    }
                }
            });
        }

        [Usage("ResetDR")]
        [Description("Resets DR levels on a target")]
        private static void ResetDR_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target a creature to reset DR:");
            e.Mobile.Target = new InternalTarget((target) =>
            {
                if (target is Mobile m)
                {
                    Instance.ResetDR(m);
                    e.Mobile.SendMessage("Reset all DR levels for {0}", m.Name);
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
