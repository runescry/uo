#if VYSTIA_SONGWEAVING
using System;
using System.Collections.Generic;
using Server;
using Server.Engines.PartySystem;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Custom.VystiaClasses.Songweaving
{
    public interface ISongweavingSong
    {
        string Name { get; }
        string Summary { get; }
        void Begin(PlayerMobile caster);
    }

    public class SongEntry
    {
        public string Key { get; }
        public string DisplayName { get; }
        public string Summary { get; }
        public int IconItemId { get; }
        public int IconHue { get; }
        public ISongweavingSong Song { get; }

        public SongEntry(string key, string displayName, string summary, int iconItemId, int iconHue, ISongweavingSong song)
        {
            Key = key;
            DisplayName = displayName;
            Summary = summary;
            IconItemId = iconItemId;
            IconHue = iconHue;
            Song = song;
        }
    }

    public static class SongweavingRegistry
    {
        private static readonly Dictionary<string, SongEntry> Lookup = new Dictionary<string, SongEntry>();

        private static readonly List<SongEntry> Entries = new List<SongEntry>
        {
            new SongEntry("provocation", "Song of Provocation", "Creature vs creature.", 0x1F2D, 0x8A5, new SongProvocation()),
            new SongEntry("peacemaking", "Lullaby", "Pacify a target.", 0x1F2D, 0x8A2, new SongPeacemaking()),
            new SongEntry("discordance", "Discordant Note", "Weaken defenses.", 0x1F2D, 0x47E, new SongDiscordance()),
            new SongEntry("requiem", "Dirge of Weakness", "Sonic harm over time.", 0x1F2D, 0x455, new SongRequiem()),
            new SongEntry("mending", "Song of Healing", "Party heal over time.", 0x1F2D, 0x7D6, new SongMending()),
            new SongEntry("courage", "Song of Courage", "Party stat boost.", 0x1F2D, 0x1153, new SongCourage()),
            new SongEntry("swiftness", "Song of Swiftness", "Party speed boost.", 0x1F2D, 0x54E, new SongSwiftness()),
            new SongEntry("light", "Song of Illumination", "Night sight.", 0x1F2D, 0x481, new SongLight()),
            new SongEntry("fortune", "Inspire Accuracy", "Luck bonus.", 0x1F2D, 0x8FD, new SongFortune())
        };

        static SongweavingRegistry()
        {
            foreach (SongEntry entry in Entries)
            {
                RegisterAlias(entry.Key, entry);
                RegisterAlias(entry.DisplayName, entry);
            }

            RegisterAlias("SongOfProvocation", Entries[0]);
            RegisterAlias("Lullaby", Entries[1]);
            RegisterAlias("DiscordantNote", Entries[2]);
            RegisterAlias("DirgeOfWeakness", Entries[3]);
            RegisterAlias("SongOfHealing", Entries[4]);
            RegisterAlias("SongOfCourage", Entries[5]);
            RegisterAlias("SongOfSwiftness", Entries[6]);
            RegisterAlias("SongOfIllumination", Entries[7]);
            RegisterAlias("InspireAccuracy", Entries[8]);
        }

        public static List<SongEntry> GetAll() => new List<SongEntry>(Entries);

        public static ISongweavingSong GetByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            SongEntry entry = GetEntryByKey(key);
            return entry?.Song;
        }

        public static SongEntry GetEntryByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            string normalized = NormalizeKey(key);
            return Lookup.TryGetValue(normalized, out SongEntry entry) ? entry : null;
        }

        private static void RegisterAlias(string key, SongEntry entry)
        {
            string normalized = NormalizeKey(key);
            if (normalized.Length == 0)
                return;

            if (!Lookup.ContainsKey(normalized))
                Lookup[normalized] = entry;
        }

        private static string NormalizeKey(string key)
        {
            char[] buffer = key.Trim().ToLowerInvariant().ToCharArray();
            int write = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                char c = buffer[i];
                if (char.IsLetterOrDigit(c))
                {
                    buffer[write++] = c;
                }
            }

            return new string(buffer, 0, write);
        }
    }

    public static class SongweavingParty
    {
        public static IEnumerable<Mobile> GetPartyTargets(PlayerMobile caster, int range, bool includeSelf = true)
        {
            if (caster == null)
                yield break;

            if (includeSelf)
                yield return caster;

            Party party = caster.Party as Party;
            if (party == null)
                yield break;

            foreach (PartyMemberInfo pmi in party.Members)
            {
                Mobile member = pmi.Mobile;
                if (member == null || member == caster)
                    continue;

                if (member.Map == caster.Map && member.InRange(caster.Location, range))
                    yield return member;
            }
        }
    }

    public abstract class SingleTargetSong : SongweavingSongBase, ISongweavingSong
    {
        public override bool RequiresTarget => true;
        public abstract TargetFlags Flags { get; }
        public virtual int Range => 12;

        public void Begin(PlayerMobile caster)
        {
            if (caster == null)
                return;

            caster.Target = new SongTarget(caster, this);
            caster.SendMessage($"Select a target for {Name}.");
        }

        private class SongTarget : Target
        {
            private readonly PlayerMobile m_Caster;
            private readonly SingleTargetSong m_Song;

            public SongTarget(PlayerMobile caster, SingleTargetSong song)
                : base(song.Range, false, song.Flags)
            {
                m_Caster = caster;
                m_Song = song;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Caster == null || m_Song == null)
                    return;

                if (targeted is Mobile target)
                {
                    m_Song.TryUse(m_Caster, target);
                }
                else
                {
                    m_Caster.SendMessage("That is not a valid target.");
                }
            }
        }
    }

    public abstract class PartySong : SongweavingSongBase, ISongweavingSong
    {
        public abstract int Range { get; }

        public void Begin(PlayerMobile caster)
        {
            if (caster == null)
                return;

            List<Mobile> targets = new List<Mobile>(SongweavingParty.GetPartyTargets(caster, Range, true));
            TryUse(caster, null, targets);
        }
    }

    // ========================
    // CONTROL SONGS
    // ========================
    public class SongProvocation : SongweavingSongBase, ISongweavingSong
    {
        public override SongType Type => SongType.Provocation;
        public override string SongKey => "provocation";
        public override string Name => "Song of Provocation";
        public override string Summary => "Provokes a creature to attack another creature.";
        public override double RequiredSongweaving => 30.0;
        public override double Difficulty => 0.0;
        public override int CrescendoGain => 4;
        public override int BaseDurationSeconds => 20;
        public override int BaseConcentrationCost => 35;
        public override bool RequiresTarget => true;

        public void Begin(PlayerMobile caster)
        {
            if (caster == null)
                return;

            caster.Target = new ProvocationTarget(caster, this);
            caster.SendMessage("Select the creature to provoke.");
        }

        protected override double GetDifficulty(PlayerMobile caster, Mobile target)
        {
            if (target is BaseCreature bc)
                return bc.BardingDifficulty;
            return 60.0;
        }

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            if (caster == null || target == null)
                return;
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
        }

        private class ProvocationTarget : Target
        {
            private readonly PlayerMobile m_Caster;
            private readonly SongProvocation m_Song;
            private BaseCreature m_Provoked;

            public ProvocationTarget(PlayerMobile caster, SongProvocation song)
                : base(BaseInstrument.GetBardRange(caster, SkillName.Provocation), false, TargetFlags.Harmful)
            {
                m_Caster = caster;
                m_Song = song;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Caster == null || m_Song == null)
                    return;

                if (m_Provoked == null)
                {
                    if (targeted is BaseCreature bc)
                    {
                        if (bc.Unprovokable || bc.BardProvoked)
                        {
                            m_Caster.SendMessage("That creature is immune to provocation.");
                            return;
                        }

                        int cost = SongweavingSystem.GetConcentrationCost(m_Caster, m_Song.SongKey, m_Song.BaseConcentrationCost);
                        if (!SongweavingSystem.CanReserveConcentration(m_Caster, cost))
                        {
                            m_Caster.SendMessage("You cannot maintain focus on another song.");
                            return;
                        }

                        var result = SongweavingSystem.TryPerformSong(
                            m_Caster,
                            m_Song.GetDifficulty(m_Caster, bc),
                            m_Song.RequiredSongweaving,
                            m_Song.CrescendoGain,
                            m_Song.Cooldown);

                        if (result != SongweavingResult.Success)
                        {
                            m_Song.OnFailure(m_Caster, result);
                            return;
                        }

                        m_Provoked = bc;
                        m_Caster.SendMessage("Now select the target to attack.");
                        m_Caster.Target = this;
                    }
                    else
                    {
                        m_Caster.SendMessage("You must target a creature.");
                    }

                    return;
                }

                if (targeted is BaseCreature targetCreature && targetCreature != m_Provoked)
                {
                    m_Provoked.Provoke(m_Caster, targetCreature, true);
                    m_Caster.SendMessage("Your provocation takes hold.");

                    TimeSpan duration = SongweavingMasterySystem.ApplyDuration(
                        m_Caster,
                        TimeSpan.FromSeconds(m_Song.BaseDurationSeconds),
                        m_Song.SongKey);

                    int cost = SongweavingSystem.GetConcentrationCost(m_Caster, m_Song.SongKey, m_Song.BaseConcentrationCost);
                    SongweavingSystem.ReserveConcentration(m_Caster, m_Song.SongKey, cost);
                    SongweavingSystem.RegisterSongCast(m_Caster, m_Song.SongKey, m_Song.Type, cost, duration,
                        new List<Mobile> { m_Provoked }, m_Song.RemoveEffect);
                }
                else
                {
                    m_Caster.SendMessage("Invalid provocation target.");
                }
            }
        }
    }

    public class SongPeacemaking : SingleTargetSong
    {
        public override SongType Type => SongType.Peacemaking;
        public override string SongKey => "peacemaking";
        public override string Name => "Lullaby";
        public override string Summary => "Pacifies a creature or player for a short duration.";
        public override TargetFlags Flags => TargetFlags.Harmful;
        public override double RequiredSongweaving => 30.0;
        public override double Difficulty => 60.0;
        public override int CrescendoGain => 4;
        public override int BaseDurationSeconds => 20;
        public override int BaseConcentrationCost => 30;

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            if (target is BaseCreature bc)
            {
                if (bc.Uncalmable || bc.AreaPeaceImmune)
                {
                    caster.SendMessage("That creature resists your peace.");
                    return;
                }

                bc.Pacify(caster, DateTime.UtcNow + duration);
                caster.SendMessage("The creature is calmed.");
                return;
            }

            if (target is PlayerMobile pm)
            {
                pm.PeacedUntil = DateTime.UtcNow + duration;
                pm.Warmode = false;
                pm.SendMessage("You are calmed by a soothing song.");
                return;
            }

            caster.SendMessage("That target cannot be pacified.");
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
        }
    }

    // ========================
    // DEBUFF SONGS
    // ========================
    public class SongDiscordance : SingleTargetSong
    {
        public override SongType Type => SongType.Discordance;
        public override string SongKey => "discordance";
        public override string Name => "Discordant Note";
        public override string Summary => "Weakens a target's defenses.";
        public override TargetFlags Flags => TargetFlags.Harmful;
        public override double RequiredSongweaving => 50.0;
        public override double Difficulty => 80.0;
        public override int CrescendoGain => 4;
        public override int BaseDurationSeconds => 20;
        public override int BaseConcentrationCost => 30;

        protected override int GetPotency(PlayerMobile caster)
        {
            return SongweavingMasterySystem.ApplyPower(caster, 15, SongKey);
        }

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.Vulnerability, duration, potency);
            target.SendMessage("Discordant notes leave you vulnerable!");
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
            VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.Vulnerability);
        }
    }

    public class SongRequiem : SingleTargetSong
    {
        public override SongType Type => SongType.Requiem;
        public override string SongKey => "requiem";
        public override string Name => "Dirge of Weakness";
        public override string Summary => "Inflicts a lingering sonic harm over time.";
        public override TargetFlags Flags => TargetFlags.Harmful;
        public override double RequiredSongweaving => 70.0;
        public override double Difficulty => 100.0;
        public override int CrescendoGain => 4;
        public override int BaseDurationSeconds => 15;
        public override int BaseConcentrationCost => 20;
        public override bool AllowStackSameType => true;

        protected override int GetPotency(PlayerMobile caster)
        {
            return SongweavingMasterySystem.ApplyPower(caster, 5, SongKey);
        }

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.Corruption, duration, potency);
            target.SendMessage("A requiem gnaws at your vitality.");
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
        }
    }

    // ========================
    // SUPPORT SONGS
    // ========================
    public class SongMending : PartySong
    {
        public override SongType Type => SongType.Mending;
        public override string SongKey => "mending";
        public override string Name => "Song of Healing";
        public override string Summary => "Heals party members over time.";
        public override double RequiredSongweaving => 50.0;
        public override double Difficulty => 115.0;
        public override int CrescendoGain => 1;
        public override int BaseDurationSeconds => 10;
        public override int BaseConcentrationCost => 25;
        public override int Range => 6;

        protected override int GetPotency(PlayerMobile caster)
        {
            return SongweavingMasterySystem.ApplyPower(caster, 5, SongKey);
        }

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.Rejuvenation, duration, potency);
            target.SendMessage("A soothing melody mends your wounds.");
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
            VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.Rejuvenation);
        }
    }

    public class SongCourage : PartySong
    {
        public override SongType Type => SongType.Courage;
        public override string SongKey => "courage";
        public override string Name => "Song of Courage";
        public override string Summary => "Bolsters allies with renewed strength.";
        public override double RequiredSongweaving => 70.0;
        public override double Difficulty => 90.0;
        public override int CrescendoGain => 1;
        public override int BaseDurationSeconds => 30;
        public override int BaseConcentrationCost => 25;
        public override int Range => 8;

        protected override int GetPotency(PlayerMobile caster)
        {
            return SongweavingMasterySystem.ApplyPower(caster, 5, SongKey);
        }

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.AllStatsBuff, duration, potency);
            target.SendMessage("A song of courage emboldens you.");
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
            VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.AllStatsBuff);
        }
    }

    public class SongSwiftness : PartySong
    {
        public override SongType Type => SongType.Swiftness;
        public override string SongKey => "swiftness";
        public override string Name => "Song of Swiftness";
        public override string Summary => "Grants increased speed and agility.";
        public override double RequiredSongweaving => 70.0;
        public override double Difficulty => 90.0;
        public override int CrescendoGain => 1;
        public override int BaseDurationSeconds => 30;
        public override int BaseConcentrationCost => 25;
        public override int Range => 8;

        protected override int GetPotency(PlayerMobile caster)
        {
            return SongweavingMasterySystem.ApplyPower(caster, 5, SongKey);
        }

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            VystiaBuffManager.Instance.ApplyBuff(target, caster, VystiaBuffType.SongOfSwiftness, duration, potency);
            target.SendMessage("A song of swiftness quickens your pace.");
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
            VystiaBuffManager.Instance.RemoveBuff(target, VystiaBuffType.SongOfSwiftness);
        }
    }

    // ========================
    // UTILITY SONGS
    // ========================
    public class SongLight : PartySong
    {
        private static readonly HashSet<Mobile> s_LightSongOwners = new HashSet<Mobile>();

        public override SongType Type => SongType.Light;
        public override string SongKey => "light";
        public override string Name => "Song of Illumination";
        public override string Summary => "Grants night sight to nearby allies.";
        public override double RequiredSongweaving => 30.0;
        public override double Difficulty => 50.0;
        public override int CrescendoGain => 1;
        public override int BaseDurationSeconds => 20;
        public override int BaseConcentrationCost => 0;
        public override int Range => 6;

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            bool owns = target.BeginAction(typeof(LightCycle));
            if (owns)
                s_LightSongOwners.Add(target);

            target.LightLevel = LightCycle.DungeonLevel;
            target.CheckLightLevels(true);
            target.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
            target.PlaySound(0x1E3);
            BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.NightSight, 1075643));
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
            if (s_LightSongOwners.Remove(target))
            {
                target.EndAction(typeof(LightCycle));
                BuffInfo.RemoveBuff(target, BuffIcon.NightSight);
                target.CheckLightLevels(true);
            }
        }
    }

    public class SongFortune : PartySong
    {
        public override SongType Type => SongType.Fortune;
        public override string SongKey => "fortune";
        public override string Name => "Inspire Accuracy";
        public override string Summary => "A small boon to luck.";
        public override double RequiredSongweaving => 50.0;
        public override double Difficulty => 75.0;
        public override int CrescendoGain => 1;
        public override int BaseDurationSeconds => 600;
        public override int BaseConcentrationCost => 0;
        public override int Range => 6;

        protected override int GetPotency(PlayerMobile caster)
        {
            return SongweavingMasterySystem.ApplyPower(caster, 20, SongKey);
        }

        protected override void ApplyEffect(PlayerMobile caster, Mobile target, TimeSpan duration, int potency)
        {
            VystiaLuckBonusSystem.ApplyLuckBonus(target, potency, duration);
            target.SendMessage("Fate smiles upon you.");
        }

        protected override void RemoveEffect(PlayerMobile caster, Mobile target)
        {
        }
    }
}
#endif
