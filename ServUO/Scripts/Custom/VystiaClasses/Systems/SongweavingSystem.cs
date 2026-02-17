#if VYSTIA_SONGWEAVING
using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.VystiaClasses.Systems
{
    public enum SongweavingResult
    {
        Success,
        FailedSkill,
        FailedMusicianship,
        FailedRequirement,
        FailedConcentration,
        InvalidTarget,
        OnCooldown,
        InvalidCaster
    }

    public enum SongType
    {
        Provocation,
        Peacemaking,
        Discordance,
        Courage,
        Swiftness,
        Mending,
        Requiem,
        Light,
        Fortune
    }

    public static class SongweavingSystem
    {
        private static readonly Dictionary<Mobile, DateTime> Cooldowns = new Dictionary<Mobile, DateTime>();
        private static readonly Dictionary<PlayerMobile, Dictionary<string, ActiveSongCast>> ActiveSongsByCaster =
            new Dictionary<PlayerMobile, Dictionary<string, ActiveSongCast>>();
        private static readonly Dictionary<Mobile, Dictionary<SongType, ActiveSongEffect>> ActiveSongsByTarget =
            new Dictionary<Mobile, Dictionary<SongType, ActiveSongEffect>>();
        private static readonly Dictionary<PlayerMobile, Timer> ConcentrationRegenTimers =
            new Dictionary<PlayerMobile, Timer>();

        public static TimeSpan GetCooldownRemaining(PlayerMobile caster)
        {
            if (caster == null)
                return TimeSpan.Zero;

            DateTime nextAllowed;
            if (Cooldowns.TryGetValue(caster, out nextAllowed))
            {
                if (DateTime.UtcNow < nextAllowed)
                    return nextAllowed - DateTime.UtcNow;
            }

            return TimeSpan.Zero;
        }

        public static SongweavingResult TryPerformSong(
            PlayerMobile caster,
            double songDifficulty,
            double requiredSongweaving,
            int crescendoGain,
            TimeSpan cooldown)
        {
            if (caster == null || caster.Deleted)
                return SongweavingResult.InvalidCaster;

            if (caster.Skills[SkillName.Songweaving].Value < requiredSongweaving)
                return SongweavingResult.FailedRequirement;

            if (IsOnCooldown(caster, cooldown))
                return SongweavingResult.OnCooldown;

            if (!CheckSkill(caster, SkillName.Musicianship, songDifficulty))
                return SongweavingResult.FailedMusicianship;

            if (!CheckSkill(caster, SkillName.Songweaving, songDifficulty))
                return SongweavingResult.FailedSkill;

            ApplyCrescendo(caster, crescendoGain);
            SetCooldown(caster, cooldown);
            SongweavingMasterySystem.AwardMastery(caster, 1);

            return SongweavingResult.Success;
        }

        public static bool TrySpendCrescendo(PlayerMobile caster, int amount)
        {
            if (caster == null || amount <= 0)
                return false;

            if (VystiaResourceManager.GetResource(caster, ResourceType.Crescendo) is CrescendoResource crescendo)
                return crescendo.Spend(amount);

            return false;
        }

        public static bool CanSpendCrescendo(PlayerMobile caster, int amount)
        {
            if (caster == null || amount <= 0)
                return false;

            if (VystiaResourceManager.GetResource(caster, ResourceType.Crescendo) is CrescendoResource crescendo)
                return crescendo.CanSpend(amount);

            return false;
        }

        public static void ApplyCooldown(PlayerMobile caster, TimeSpan cooldown)
        {
            SetCooldown(caster, cooldown);
        }

        public static void StartChanneling(PlayerMobile caster)
        {
            if (caster == null || caster.Deleted)
                return;

            if (VystiaResourceManager.GetResource(caster, ResourceType.Crescendo) is CrescendoResource crescendo)
                crescendo.StartChanneling();
        }

        public static void StopChanneling(PlayerMobile caster)
        {
            if (caster == null || caster.Deleted)
                return;

            if (VystiaResourceManager.GetResource(caster, ResourceType.Crescendo) is CrescendoResource crescendo)
                crescendo.StopChanneling();
        }

        private static bool CheckSkill(PlayerMobile caster, SkillName skill, double difficulty)
        {
            double minSkill = difficulty - 25.0;
            double maxSkill = difficulty + 25.0;
            return caster.CheckSkill(skill, minSkill, maxSkill);
        }

        private static void ApplyCrescendo(PlayerMobile caster, int amount)
        {
            if (caster == null || amount <= 0)
                return;

            if (VystiaResourceManager.GetResource(caster, ResourceType.Crescendo) is CrescendoResource crescendo)
                crescendo.Generate(amount);
        }

        private static bool IsOnCooldown(PlayerMobile caster, TimeSpan cooldown)
        {
            if (cooldown <= TimeSpan.Zero)
                return false;

            DateTime nextAllowed;
            if (Cooldowns.TryGetValue(caster, out nextAllowed))
            {
                if (DateTime.UtcNow < nextAllowed)
                    return true;
            }

            return false;
        }

        private static void SetCooldown(PlayerMobile caster, TimeSpan cooldown)
        {
            if (cooldown <= TimeSpan.Zero)
                return;

            Cooldowns[caster] = DateTime.UtcNow + cooldown;
        }

        public static int GetConcentrationCost(PlayerMobile caster, string songKey, int baseCost)
        {
            if (caster == null)
                return baseCost;

            int durationLevel = SongweavingMasterySystem.GetDurationLevel(caster, songKey);
            int reduction = durationLevel * 2;
            int minCost = Math.Max(1, baseCost / 2);
            int cost = baseCost - reduction;
            return cost < minCost ? minCost : cost;
        }

        public static bool CanReserveConcentration(PlayerMobile caster, int cost)
        {
            if (caster == null)
                return false;

            return caster.Concentration >= cost;
        }

        public static void ReserveConcentration(PlayerMobile caster, string songKey, int cost)
        {
            if (caster == null || cost <= 0)
                return;

            caster.Concentration = Math.Max(0, caster.Concentration - cost);
            caster.ActiveSongConcentration[songKey] = cost;
        }

        public static void ReleaseConcentration(PlayerMobile caster, string songKey)
        {
            if (caster == null || string.IsNullOrEmpty(songKey))
                return;

            if (caster.ActiveSongConcentration.TryGetValue(songKey, out int cost))
            {
                caster.Concentration = Math.Min(caster.MaxConcentration, caster.Concentration + cost);
                caster.ActiveSongConcentration.Remove(songKey);
            }

            if (caster.ActiveSongConcentration.Count == 0)
            {
                ScheduleConcentrationRestore(caster);
            }
        }

        private static void ScheduleConcentrationRestore(PlayerMobile caster)
        {
            if (caster == null)
                return;

            if (ConcentrationRegenTimers.TryGetValue(caster, out Timer existing))
            {
                existing.Stop();
                ConcentrationRegenTimers.Remove(caster);
            }

            Timer timer = Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
            {
                ConcentrationRegenTimers.Remove(caster);
                if (caster == null || caster.Deleted)
                    return;

                if (caster.ActiveSongConcentration.Count == 0 && caster.Combatant == null)
                {
                    caster.Concentration = caster.MaxConcentration;
                }
            });

            ConcentrationRegenTimers[caster] = timer;
        }

        internal static void RegisterSongCast(PlayerMobile caster, string songKey, SongType type, int cost, TimeSpan duration,
            List<Mobile> targets, Action<PlayerMobile, Mobile> removeEffect)
        {
            if (caster == null || string.IsNullOrEmpty(songKey))
                return;

            if (!ActiveSongsByCaster.TryGetValue(caster, out Dictionary<string, ActiveSongCast> songs))
            {
                songs = new Dictionary<string, ActiveSongCast>(StringComparer.OrdinalIgnoreCase);
                ActiveSongsByCaster[caster] = songs;
            }

            if (songs.TryGetValue(songKey, out ActiveSongCast existing))
            {
                if (TryGetSongBuff(existing.Type, out SongBuffInfo existingBuff))
                {
                    BuffInfo.RemoveBuff(caster, existingBuff.Icon);
                }

                existing.Cancel();
                songs.Remove(songKey);
            }

            var cast = new ActiveSongCast(caster, songKey, type, cost, duration, targets, removeEffect);
            songs[songKey] = cast;
            cast.Start();

            if (TryGetSongBuff(type, out SongBuffInfo buff))
            {
                BuffInfo.AddBuff(caster, new BuffInfo(buff.Icon, buff.TitleCliloc, buff.SecondaryCliloc, duration, caster));
            }
        }

        internal static void OnSongExpired(PlayerMobile caster, string songKey)
        {
            if (caster == null || string.IsNullOrEmpty(songKey))
                return;

            if (ActiveSongsByCaster.TryGetValue(caster, out Dictionary<string, ActiveSongCast> songs) &&
                songs.TryGetValue(songKey, out ActiveSongCast song))
            {
                if (TryGetSongBuff(song.Type, out SongBuffInfo buff))
                {
                    BuffInfo.RemoveBuff(caster, buff.Icon);
                }

                song.Expire();
                songs.Remove(songKey);
                if (songs.Count == 0)
                    ActiveSongsByCaster.Remove(caster);
            }
        }

        internal static bool TryApplyToTarget(
            PlayerMobile caster,
            Mobile target,
            SongType type,
            int potency,
            bool allowStackSameType,
            Action removeExistingEffect)
        {
            if (target == null || target.Deleted)
                return false;

            if (allowStackSameType)
                return true;

            if (!ActiveSongsByTarget.TryGetValue(target, out Dictionary<SongType, ActiveSongEffect> entries))
            {
                entries = new Dictionary<SongType, ActiveSongEffect>();
                ActiveSongsByTarget[target] = entries;
            }

            if (!allowStackSameType && entries.TryGetValue(type, out ActiveSongEffect existing))
            {
                if (existing.Caster != null)
                {
                    double existingSkill = existing.SkillValue;
                    double incomingSkill = caster != null ? caster.Skills[SkillName.Musicianship].Value : 0.0;

                    if (potency < existing.Potency ||
                        (potency == existing.Potency && incomingSkill <= existingSkill))
                    {
                        return false;
                    }
                }

                if (existing.RemoveEffect != null)
                    existing.RemoveEffect();

                entries.Remove(type);
            }

            entries[type] = new ActiveSongEffect
            {
                Type = type,
                Caster = caster,
                Potency = potency,
                SkillValue = caster != null ? caster.Skills[SkillName.Musicianship].Value : 0.0,
                RemoveEffect = removeExistingEffect
            };

            return true;
        }

        internal static void RemoveTargetEffect(PlayerMobile caster, Mobile target, SongType type, Action removeEffect)
        {
            if (target == null || target.Deleted)
                return;

            if (ActiveSongsByTarget.TryGetValue(target, out Dictionary<SongType, ActiveSongEffect> entries) &&
                entries.TryGetValue(type, out ActiveSongEffect existing))
            {
                if (existing.Caster == caster)
                {
                    removeEffect?.Invoke();
                    entries.Remove(type);
                }

                if (entries.Count == 0)
                    ActiveSongsByTarget.Remove(target);
            }
        }

        private struct SongBuffInfo
        {
            public BuffIcon Icon;
            public int TitleCliloc;
            public int SecondaryCliloc;
        }

        private static bool TryGetSongBuff(SongType type, out SongBuffInfo info)
        {
            switch (type)
            {
                case SongType.Courage:
                    info = new SongBuffInfo { Icon = BuffIcon.Inspire, TitleCliloc = 1115683, SecondaryCliloc = 1151951 };
                    return true;
                case SongType.Swiftness:
                    info = new SongBuffInfo { Icon = BuffIcon.Invigorate, TitleCliloc = 1115613, SecondaryCliloc = 1115730 };
                    return true;
                case SongType.Mending:
                    info = new SongBuffInfo { Icon = BuffIcon.Resilience, TitleCliloc = 1115614, SecondaryCliloc = 1115731 };
                    return true;
                case SongType.Peacemaking:
                    info = new SongBuffInfo { Icon = BuffIcon.Perseverance, TitleCliloc = 1115615, SecondaryCliloc = 1115732 };
                    return true;
                case SongType.Provocation:
                    info = new SongBuffInfo { Icon = BuffIcon.TribulationCaster, TitleCliloc = 1115740, SecondaryCliloc = 1151388 };
                    return true;
                case SongType.Discordance:
                    info = new SongBuffInfo { Icon = BuffIcon.DespairCaster, TitleCliloc = 1115741, SecondaryCliloc = 1115743 };
                    return true;
                case SongType.Requiem:
                    info = new SongBuffInfo { Icon = BuffIcon.DespairTarget, TitleCliloc = 1115741, SecondaryCliloc = 1115743 };
                    return true;
                case SongType.Light:
                    info = new SongBuffInfo { Icon = BuffIcon.NightSight, TitleCliloc = 1075643, SecondaryCliloc = 1075644 };
                    return true;
                case SongType.Fortune:
                    info = new SongBuffInfo { Icon = BuffIcon.Cunning, TitleCliloc = 1075843, SecondaryCliloc = 1075844 };
                    return true;
                default:
                    info = default;
                    return false;
            }
        }
    }

    internal class ActiveSongCast
    {
        public PlayerMobile Caster { get; }
        public string SongKey { get; }
        public SongType Type { get; }
        public int ConcentrationCost { get; }
        public DateTime ExpiresAt { get; private set; }
        public List<Mobile> Targets { get; }
        public Action<PlayerMobile, Mobile> RemoveEffect { get; }
        private Timer m_Timer;

        public ActiveSongCast(PlayerMobile caster, string songKey, SongType type, int cost, TimeSpan duration,
            List<Mobile> targets, Action<PlayerMobile, Mobile> removeEffect)
        {
            Caster = caster;
            SongKey = songKey;
            Type = type;
            ConcentrationCost = cost;
            ExpiresAt = DateTime.UtcNow + duration;
            Targets = targets ?? new List<Mobile>();
            RemoveEffect = removeEffect;
        }

        public void Start()
        {
            m_Timer = Timer.DelayCall(ExpiresAt - DateTime.UtcNow, () => SongweavingSystem.OnSongExpired(Caster, SongKey));
        }

        public void Cancel()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public void Expire()
        {
            Cancel();

            if (Caster != null && RemoveEffect != null)
            {
                foreach (Mobile target in Targets)
                {
                    SongweavingSystem.RemoveTargetEffect(Caster, target, Type, () => RemoveEffect(Caster, target));
                }
            }

            SongweavingSystem.ReleaseConcentration(Caster, SongKey);
        }
    }

    internal class ActiveSongEffect
    {
        public SongType Type { get; set; }
        public PlayerMobile Caster { get; set; }
        public int Potency { get; set; }
        public double SkillValue { get; set; }
        public Action RemoveEffect { get; set; }
    }

    public abstract class SongweavingSongBase
    {
        public abstract SongType Type { get; }
        public abstract string SongKey { get; }
        public abstract string Name { get; }
        public abstract string Summary { get; }
        public abstract double RequiredSongweaving { get; }
        public abstract double Difficulty { get; }
        public abstract int CrescendoGain { get; }
        public abstract int BaseDurationSeconds { get; }
        public abstract int BaseConcentrationCost { get; }
        public virtual TimeSpan Cooldown => TimeSpan.FromSeconds(7);
        public virtual bool RequiresTarget => false;
        public virtual bool AllowStackSameType => false;

        public SongweavingResult TryUse(PlayerMobile caster, Mobile target = null, IEnumerable<Mobile> targets = null)
        {
            if (RequiresTarget && target == null)
                return SongweavingResult.InvalidTarget;

            int cost = SongweavingSystem.GetConcentrationCost(caster, SongKey, BaseConcentrationCost);
            if (!SongweavingSystem.CanReserveConcentration(caster, cost))
            {
                OnFailure(caster, SongweavingResult.FailedConcentration);
                return SongweavingResult.FailedConcentration;
            }

            var result = SongweavingSystem.TryPerformSong(
                caster,
                GetDifficulty(caster, target),
                RequiredSongweaving,
                CrescendoGain,
                Cooldown);

            if (result == SongweavingResult.Success)
            {
                TimeSpan duration = SongweavingMasterySystem.ApplyDuration(
                    caster,
                    TimeSpan.FromSeconds(BaseDurationSeconds),
                    SongKey);

                List<Mobile> appliedTargets = new List<Mobile>();
                if (targets != null)
                {
                    foreach (Mobile m in targets)
                    {
                        if (TryApplyToTarget(caster, m, duration))
                            appliedTargets.Add(m);
                    }
                }
                else if (target != null)
                {
                    if (TryApplyToTarget(caster, target, duration))
                        appliedTargets.Add(target);
                }

                SongweavingSystem.ReserveConcentration(caster, SongKey, cost);
                SongweavingSystem.RegisterSongCast(caster, SongKey, Type, cost, duration, appliedTargets, RemoveEffect);
                OnSuccess(caster, appliedTargets);
            }
            else
            {
                OnFailure(caster, result);
            }

            return result;
        }

        private bool TryApplyToTarget(PlayerMobile caster, Mobile target, TimeSpan duration)
        {
            if (target == null)
                return false;

            int potency = GetPotency(caster);
            bool canApply = SongweavingSystem.TryApplyToTarget(
                caster,
                target,
                Type,
                potency,
                AllowStackSameType,
                () => RemoveEffect(caster, target));

            if (!canApply)
                return false;

            ApplyEffect(caster, target, duration, potency);
            return true;
        }

        protected virtual int GetPotency(PlayerMobile caster)
        {
            return SongweavingMasterySystem.ApplyPower(caster, 1, SongKey);
        }

        protected virtual double GetDifficulty(PlayerMobile caster, Mobile target)
        {
            return Difficulty;
        }

        protected abstract void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency);
        protected abstract void RemoveEffect(PlayerMobile caster, Mobile target);
        protected virtual void OnSuccess(PlayerMobile caster, List<Mobile> targets)
        {
        }

        protected virtual void OnFailure(PlayerMobile caster, SongweavingResult result)
        {
            if (caster == null)
                return;

            switch (result)
            {
                case SongweavingResult.InvalidTarget:
                    caster.SendMessage("You need a valid target for that song.");
                    break;
                case SongweavingResult.FailedRequirement:
                    caster.SendMessage("You lack the Songweaving skill to perform that song.");
                    break;
                case SongweavingResult.FailedConcentration:
                    caster.SendMessage("You cannot maintain focus on another song.");
                    break;
                case SongweavingResult.OnCooldown:
                    caster.SendMessage("You must wait before performing another song.");
                    break;
                case SongweavingResult.FailedMusicianship:
                    caster.SendMessage("Your performance falters.");
                    break;
                case SongweavingResult.FailedSkill:
                    caster.SendMessage("Your song fails to take hold.");
                    break;
            }
        }
    }
}
#endif
