#if VYSTIA_SONGWEAVING
using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Custom.VystiaClasses.Systems;
using Server.Custom.VystiaClasses.Songweaving;

namespace Server.Custom.VystiaClasses.Systems
{
    public enum FinaleTargetType
    {
        None,
        SingleTarget,
        Area
    }

    public class FinaleDefinition
    {
        public string Key { get; }
        public string Name { get; }
        public string Summary { get; }
        public int Cost { get; }
        public FinaleTargetType TargetType { get; }
        public int Range { get; }

        public FinaleDefinition(string key, string name, string summary, int cost, FinaleTargetType targetType, int range = 12)
        {
            Key = key;
            Name = name;
            Summary = summary;
            Cost = cost;
            TargetType = targetType;
            Range = range;
        }
    }

    public static class SongweavingFinaleSystem
    {
        private static readonly Dictionary<Mobile, DateTime> FinaleCooldowns = new Dictionary<Mobile, DateTime>();
        private static readonly TimeSpan FinaleCooldown = TimeSpan.FromSeconds(15);

        private static readonly List<FinaleDefinition> Finales = new List<FinaleDefinition>
        {
            new FinaleDefinition("sharpnote", "Sharp Note", "Single target sonic strike.", 5, FinaleTargetType.SingleTarget),
            new FinaleDefinition("interlude", "Mesmerise", "Paralyze a target briefly.", 5, FinaleTargetType.SingleTarget),
            new FinaleDefinition("rally", "Cacophony", "AoE sonic damage (6 tiles).", 10, FinaleTargetType.Area, 12),
            new FinaleDefinition("fortissimo", "Fortissimo", "AoE sonic burst (5 tiles).", 15, FinaleTargetType.Area, 12),
            new FinaleDefinition("soothing", "Soothing Chorus", "Party heal burst.", 15, FinaleTargetType.None),
            new FinaleDefinition("symphony", "Symphony of Destruction", "AoE sonic damage (8 tiles).", 20, FinaleTargetType.Area, 12)
        };

        public static List<FinaleDefinition> GetAll() => new List<FinaleDefinition>(Finales);

        public static FinaleDefinition GetByKey(string key)
        {
            foreach (var finale in Finales)
            {
                if (string.Equals(finale.Key, key, StringComparison.OrdinalIgnoreCase))
                    return finale;
            }

            return null;
        }

        public static void BeginFinale(PlayerMobile caster, FinaleDefinition finale)
        {
            if (caster == null || finale == null)
                return;

            if (!CanPerform(caster, finale))
                return;

            switch (finale.TargetType)
            {
                case FinaleTargetType.SingleTarget:
                    caster.Target = new FinaleTarget(caster, finale);
                    caster.SendMessage($"Select a target for {finale.Name}.");
                    break;
                case FinaleTargetType.Area:
                    caster.Target = new FinaleLocationTarget(caster, finale);
                    caster.SendMessage($"Select a location for {finale.Name}.");
                    break;
                default:
                    PerformFinale(caster, finale, null, null);
                    break;
            }
        }

        public static bool CanPerform(PlayerMobile caster, FinaleDefinition finale)
        {
            if (caster.VystiaClassV2 != PlayerClassTypeV2.Bard)
            {
                caster.SendMessage("Only bards can perform finales.");
                return false;
            }

            if (GetCooldownRemaining(caster) > TimeSpan.Zero)
            {
                caster.SendMessage("You must wait before performing another finale.");
                return false;
            }

            if (!SongweavingSystem.CanSpendCrescendo(caster, finale.Cost))
            {
                caster.SendMessage("You lack the Crescendo to perform that finale.");
                return false;
            }

            return true;
        }

        private static void PerformFinale(PlayerMobile caster, FinaleDefinition finale, Mobile target, Point3D? location)
        {
            if (caster == null || finale == null)
                return;

            if (!SongweavingSystem.TrySpendCrescendo(caster, finale.Cost))
            {
                caster.SendMessage("You lack the Crescendo to perform that finale.");
                return;
            }

            ApplyCooldown(caster);

            switch (finale.Key)
            {
                case "sharpnote":
                    if (target != null)
                    {
                        caster.DoHarmful(target);
                        AOS.Damage(target, caster, Utility.RandomMinMax(30, 50), 0, 0, 0, 0, 100);
                        target.FixedParticles(0x36BD, 20, 10, 5044, 0x481, 0, EffectLayer.Head);
                        target.PlaySound(0x1F1);
                    }
                    break;
                case "interlude":
                    if (target != null)
                    {
                        caster.DoHarmful(target);
                        target.Freeze(TimeSpan.FromSeconds(4));
                        target.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Head);
                        target.PlaySound(0x204);
                        target.SendMessage("You are mesmerised by a haunting cadence!");
                    }
                    break;
                case "rally":
                    if (location.HasValue)
                    {
                        ApplyAreaDamage(caster, location.Value, 6, 100, 140);
                    }
                    break;
                case "fortissimo":
                    if (location.HasValue)
                    {
                        ApplyAreaDamage(caster, location.Value, 5, 80, 120);
                    }
                    break;
                case "soothing":
                    foreach (var member in SongweavingParty.GetPartyTargets(caster, 8))
                    {
                        int heal = Utility.RandomMinMax(50, 80);
                        member.Heal(heal, caster, false);
                        member.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                        member.SendMessage("A soothing chorus restores your vitality.");
                    }
                    break;
                case "symphony":
                    if (location.HasValue)
                    {
                        ApplyAreaDamage(caster, location.Value, 8, 150, 200);
                    }
                    break;
            }
        }

        private static void ApplyAreaDamage(PlayerMobile caster, Point3D loc, int radius, int minDamage, int maxDamage)
        {
            if (caster.Map == null)
                return;

            foreach (Mobile m in caster.Map.GetMobilesInRange(loc, radius))
            {
                if (m == null || m == caster || !caster.CanBeHarmful(m))
                    continue;

                caster.DoHarmful(m);
                int damage = Utility.RandomMinMax(minDamage, maxDamage);
                AOS.Damage(m, caster, damage, 0, 0, 0, 0, 100);
                m.FixedParticles(0x36BD, 20, 10, 5044, 0x481, 0, EffectLayer.Head);
                m.PlaySound(0x1F1);
            }
        }

        private class FinaleTarget : Target
        {
            private readonly PlayerMobile m_Caster;
            private readonly FinaleDefinition m_Finale;

            public FinaleTarget(PlayerMobile caster, FinaleDefinition finale)
                : base(finale.Range, false, TargetFlags.Harmful)
            {
                m_Caster = caster;
                m_Finale = finale;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Caster == null || m_Finale == null)
                    return;

                if (targeted is Mobile target)
                {
                    PerformFinale(m_Caster, m_Finale, target, null);
                }
                else
                {
                    m_Caster.SendMessage("That is not a valid target.");
                }
            }
        }

        private class FinaleLocationTarget : Target
        {
            private readonly PlayerMobile m_Caster;
            private readonly FinaleDefinition m_Finale;

            public FinaleLocationTarget(PlayerMobile caster, FinaleDefinition finale)
                : base(finale.Range, true, TargetFlags.Harmful)
            {
                m_Caster = caster;
                m_Finale = finale;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Caster == null || m_Finale == null)
                    return;

                if (targeted is IPoint3D point)
                {
                    Point3D loc = new Point3D(point);
                    PerformFinale(m_Caster, m_Finale, null, loc);
                }
                else
                {
                    m_Caster.SendMessage("That is not a valid location.");
                }
            }
        }

        private static TimeSpan GetCooldownRemaining(PlayerMobile caster)
        {
            if (caster == null)
                return TimeSpan.Zero;

            if (FinaleCooldowns.TryGetValue(caster, out DateTime nextAllowed))
            {
                if (DateTime.UtcNow < nextAllowed)
                    return nextAllowed - DateTime.UtcNow;
            }

            return TimeSpan.Zero;
        }

        private static void ApplyCooldown(PlayerMobile caster)
        {
            if (caster == null)
                return;

            FinaleCooldowns[caster] = DateTime.UtcNow + FinaleCooldown;
        }
    }
}
#endif
