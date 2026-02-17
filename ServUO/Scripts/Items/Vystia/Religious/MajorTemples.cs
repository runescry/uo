// Vystia Major Temples
// 6 major temple structures, one per religion, with enhanced effects

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Prompts;
using Server.Custom.VystiaClasses.Religion;
using Server.Network;

namespace Server.Items.Vystia
{
    #region Base Major Temple

    /// <summary>
    /// Base class for all Major Temples
    /// Provides enhanced shrine functions with temple-specific bonuses
    /// </summary>
    public abstract class BaseMajorTemple : Item
    {
        private VystiaReligion m_Religion;
        private DateTime m_LastRitualTime;
        private List<PlayerMobile> m_ActiveWorshippers;
        private Timer m_AuraTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaReligion Religion
        {
            get { return m_Religion; }
            set { m_Religion = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastRitualTime
        {
            get { return m_LastRitualTime; }
            set { m_LastRitualTime = value; }
        }

        public abstract string TempleName { get; }
        public abstract int TempleHue { get; }
        public abstract string BlessingName { get; }

        public BaseMajorTemple(VystiaReligion religion, int itemID)
            : base(itemID)
        {
            m_Religion = religion;
            Name = TempleName;
            Hue = TempleHue;
            Movable = false;
            Weight = 0;
            m_ActiveWorshippers = new List<PlayerMobile>();
            m_LastRitualTime = DateTime.MinValue;

            StartAuraTimer();
        }

        public BaseMajorTemple(Serial serial)
            : base(serial)
        {
            m_ActiveWorshippers = new List<PlayerMobile>();
        }

        private void StartAuraTimer()
        {
            m_AuraTimer?.Stop();
            m_AuraTimer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnAuraTick);
        }

        private void OnAuraTick()
        {
            if (Deleted || Map == null || Map == Map.Internal)
                return;

            // Heal nearby followers every 5 seconds
            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m is PlayerMobile pm && pm.Alive)
                {
                    var pietyData = VystiaPiety.GetPiety(pm);
                    if (pietyData != null && pietyData.Religion == m_Religion)
                    {
                        // Heal 1-3% HP based on piety tier
                        int healPercent = Math.Max(1, (int)ReligionData.GetTier(pietyData.Piety));
                        int healAmount = Math.Max(1, pm.HitsMax * healPercent / 100);

                        if (pm.Hits < pm.HitsMax)
                        {
                            pm.Hits = Math.Min(pm.HitsMax, pm.Hits + healAmount);
                            pm.FixedParticles(0x376A, 1, 10, 9916, TempleHue, 0, EffectLayer.Waist);
                        }
                    }
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !(from is PlayerMobile pm))
                return;

            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendMessage(0x22, "You are too far away to use the temple.");
                return;
            }

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != m_Religion)
            {
                pm.SendMessage(0x22, "You must follow the {0} to use this temple.", ReligionData.GetInfo(m_Religion)?.Name ?? "religion");
                return;
            }

            // Show temple menu with enhanced functions
            pm.SendGump(new TempleMenuGump(pm, this, pietyData));
        }

        /// <summary>
        /// Handle temple function selection
        /// </summary>
        public void HandleTempleFunction(PlayerMobile pm, TempleFunction function)
        {
            if (pm == null)
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != m_Religion)
                return;

            switch (function)
            {
                case TempleFunction.Pray:
                    TemplePray(pm, pietyData);
                    break;

                case TempleFunction.Tithe:
                    pm.SendMessage(0x35, "How much gold would you like to tithe? (Temple bonus: 2 piety per 100g, max 60 piety per day)");
                    pm.Prompt = new TempleTithePrompt(pm, this);
                    break;

                case TempleFunction.Pilgrimage:
                    TemplePilgrimage(pm, pietyData);
                    break;

                case TempleFunction.TempleBlessing:
                    GrantTempleBlessing(pm, pietyData);
                    break;

                case TempleFunction.GroupRitual:
                    PerformGroupRitual(pm, pietyData);
                    break;

                case TempleFunction.RechargePowers:
                    if (pietyData.Piety < 100)
                    {
                        pm.SendMessage(0x22, "You need at least 100 piety to recharge powers at a temple.");
                        return;
                    }
                    VystiaDevotionPowers.RechargeAllPowers(pm);
                    break;

                case TempleFunction.BlessItem:
                    if (pietyData.Piety < 300)
                    {
                        pm.SendMessage(0x22, "You need at least 300 piety to bless items at a temple (reduced from shrine requirement).");
                        return;
                    }
                    pm.SendMessage(0x35, "Target an item to bless it with the power of {0}.", BlessingName);
                    pm.Target = new BlessItemTarget(this);
                    break;

                case TempleFunction.Resurrect:
                    // Free resurrection at temples (no piety requirement)
                    if (!pm.Alive)
                    {
                        ResurrectAtTemple(pm);
                    }
                    else
                    {
                        pm.SendMessage(0x35, "You are already alive. Free temple resurrection is available when you die.");
                    }
                    break;
            }
        }

        private void TemplePray(PlayerMobile pm, PietyData pietyData)
        {
            // Enhanced prayer at temple: +20 piety instead of +10
            if (!pietyData.CanPray())
            {
                pm.SendMessage(0x22, "You have already prayed today. Return tomorrow.");
                return;
            }

            pietyData.LastPrayer = DateTime.UtcNow;
            VystiaPiety.AddPiety(pm, 20, "temple prayer"); // Double piety at temple

            pm.PlaySound(0x214);
            pm.FixedParticles(0x376A, 9, 32, 5005, TempleHue, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "You pray at the temple and feel greatly blessed. (+20 piety)");
        }

        private void TemplePilgrimage(PlayerMobile pm, PietyData pietyData)
        {
            // Enhanced pilgrimage at temple: +150 piety instead of +75
            if (!pietyData.CanPilgrimage())
            {
                var timeUntil = TimeSpan.FromDays(7) - (DateTime.UtcNow - pietyData.LastPilgrimage);
                pm.SendMessage(0x22, "You cannot perform another pilgrimage yet. Return in {0:F1} days.", timeUntil.TotalDays);
                return;
            }

            pietyData.LastPilgrimage = DateTime.UtcNow;
            VystiaPiety.AddPiety(pm, 150, "temple pilgrimage"); // Double piety at temple

            pm.PlaySound(0x214);
            pm.FixedParticles(0x376A, 9, 32, 5005, TempleHue, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "You complete a sacred pilgrimage to the temple. (+150 piety)");
        }

        protected abstract void GrantTempleBlessing(PlayerMobile pm, PietyData pietyData);

        private void PerformGroupRitual(PlayerMobile pm, PietyData pietyData)
        {
            // Weekly group ritual - grants bonus to all followers nearby
            if (DateTime.UtcNow - m_LastRitualTime < TimeSpan.FromDays(7))
            {
                var nextRitual = m_LastRitualTime.AddDays(7);
                pm.SendMessage(0x22, "The temple ritual was performed recently. Next available: {0}", nextRitual.ToString("g"));
                return;
            }

            if (pietyData.Piety < 500)
            {
                pm.SendMessage(0x22, "You need at least 500 piety to lead a temple ritual.");
                return;
            }

            // Find all followers nearby
            List<PlayerMobile> participants = new List<PlayerMobile>();
            foreach (Mobile m in GetMobilesInRange(10))
            {
                if (m is PlayerMobile participant && participant.Alive)
                {
                    var participantPiety = VystiaPiety.GetPiety(participant);
                    if (participantPiety != null && participantPiety.Religion == m_Religion)
                    {
                        participants.Add(participant);
                    }
                }
            }

            if (participants.Count < 3)
            {
                pm.SendMessage(0x22, "A temple ritual requires at least 3 followers present.");
                return;
            }

            m_LastRitualTime = DateTime.UtcNow;

            // Grant bonus to all participants
            int bonusPiety = 50 + (participants.Count * 10); // 50 base + 10 per participant
            foreach (var participant in participants)
            {
                var participantPiety = VystiaPiety.GetPiety(participant);
                if (participantPiety != null)
                {
                    VystiaPiety.AddPiety(participant, bonusPiety, "temple ritual");
                    participant.PlaySound(0x214);
                    participant.FixedParticles(0x376A, 9, 32, 5005, TempleHue, 0, EffectLayer.Waist);
                    participant.SendMessage(0x35, "The temple ritual fills you with divine power! (+{0} piety)", bonusPiety);

                    // Grant 30-minute buff to all participants
                    ApplyRitualBuff(participant);
                }
            }

            // Visual effect on temple
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(2)), 0x376A, 10, 60, TempleHue, 0, 5039, 0);
            Effects.PlaySound(Location, Map, 0x214);
        }

        protected abstract void ApplyRitualBuff(PlayerMobile pm);

        private void ResurrectAtTemple(PlayerMobile pm)
        {
            if (pm == null || pm.Alive)
                return;

            if (pm.Map == null || !pm.Map.CanFit(pm.Location, 16, false, false))
            {
                pm.SendMessage(0x22, "You cannot be resurrected at this location.");
                return;
            }

            pm.Resurrect();
            pm.Hits = pm.HitsMax;
            pm.Mana = pm.ManaMax;
            pm.Stam = pm.StamMax;
            pm.PlaySound(0x214);
            pm.FixedEffect(0x376A, 10, 16);
            pm.SendMessage(0x35, "The temple of {0} has restored you to life!", BlessingName);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Temple\t{0}", ReligionData.GetInfo(m_Religion)?.Name ?? "Unknown");
            list.Add("Enhanced piety gains (2x)");
            list.Add("Healing aura for followers");
            list.Add("Free resurrection");
        }

        public override void OnDelete()
        {
            m_AuraTimer?.Stop();
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write((int)m_Religion);
            writer.Write(m_LastRitualTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Religion = (VystiaReligion)reader.ReadInt();
            m_LastRitualTime = reader.ReadDateTime();

            m_ActiveWorshippers = new List<PlayerMobile>();
            StartAuraTimer();
        }

        #region Nested Classes

        private class BlessItemTarget : Target
        {
            private BaseMajorTemple m_Temple;

            public BlessItemTarget(BaseMajorTemple temple)
                : base(3, false, TargetFlags.None)
            {
                m_Temple = temple;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(from is PlayerMobile pm) || targeted == null || !(targeted is Item item))
                    return;

                if (!item.IsChildOf(pm.Backpack) && !item.IsChildOf(pm.BankBox))
                {
                    pm.SendMessage(0x22, "The item must be in your backpack or bank box.");
                    return;
                }

                if (!VystiaBlessedItemSystem.CanBlessItem(item))
                {
                    pm.SendMessage(0x22, "This item cannot be blessed.");
                    return;
                }

                bool success = VystiaBlessedItemSystem.BlessItem(pm, item, m_Temple.Religion);
                if (success)
                {
                    pm.SendMessage(0x35, "The temple has blessed your item with divine power!");
                }
            }
        }

        private class TempleTithePrompt : Prompt
        {
            private PlayerMobile m_Player;
            private BaseMajorTemple m_Temple;

            public TempleTithePrompt(PlayerMobile player, BaseMajorTemple temple)
            {
                m_Player = player;
                m_Temple = temple;
            }

            public override void OnCancel(Mobile from)
            {
                if (from is PlayerMobile pm)
                    pm.SendMessage("Tithing cancelled.");
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!(from is PlayerMobile pm))
                    return;

                if (!int.TryParse(text, out int goldAmount) || goldAmount <= 0)
                {
                    pm.SendMessage(0x22, "Please enter a valid amount of gold.");
                    return;
                }

                // Enhanced tithe at temple: 2 piety per 100g, max 60 per day
                var pietyData = VystiaPiety.GetPiety(pm);
                if (pietyData == null)
                    return;

                int maxGold = 3000; // Max 3000g for 60 piety at temple
                int actualGold = Math.Min(goldAmount, maxGold);

                Container pack = pm.Backpack;
                if (pack == null || pack.ConsumeTotal(typeof(Gold), actualGold) == false)
                {
                    pm.SendMessage(0x22, "You do not have enough gold.");
                    return;
                }

                int pietyGained = actualGold / 50; // 2 piety per 100g = 1 per 50g
                VystiaPiety.AddPiety(pm, pietyGained, "temple tithe");

                pm.PlaySound(0x2E6);
                pm.SendMessage(0x35, "You tithe {0} gold to the temple. (+{1} piety)", actualGold, pietyGained);
            }
        }

        #endregion
    }

    #endregion

    #region Temple Functions Enum

    public enum TempleFunction
    {
        Pray,
        Tithe,
        Pilgrimage,
        TempleBlessing,
        GroupRitual,
        RechargePowers,
        BlessItem,
        Resurrect
    }

    #endregion

    #region Temple Menu Gump

    public class TempleMenuGump : Gump
    {
        private PlayerMobile m_Player;
        private BaseMajorTemple m_Temple;
        private PietyData m_PietyData;

        public TempleMenuGump(PlayerMobile player, BaseMajorTemple temple, PietyData pietyData)
            : base(50, 50)
        {
            m_Player = player;
            m_Temple = temple;
            m_PietyData = pietyData;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 350, 450, 9200);
            AddLabel(20, 20, temple.TempleHue - 1, temple.TempleName);
            AddLabel(20, 40, 0, string.Format("Your Piety: {0} ({1})", pietyData.Piety, ReligionData.GetTier(pietyData.Piety)));

            int y = 80;

            // Pray (always available)
            bool canPray = pietyData.CanPray();
            AddButton(20, y, canPray ? 4005 : 4006, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(60, y, canPray ? 0x35 : 0x22, canPray ? "Pray (+20 piety, enhanced)" : "Pray (already prayed today)");
            y += 35;

            // Tithe (always available)
            AddButton(20, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(60, y, 0x35, "Tithe (2 piety per 100g, enhanced)");
            y += 35;

            // Pilgrimage
            bool canPilgrimage = pietyData.CanPilgrimage();
            AddButton(20, y, canPilgrimage ? 4005 : 4006, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(60, y, canPilgrimage ? 0x35 : 0x22, canPilgrimage ? "Pilgrimage (+150 piety, enhanced)" : "Pilgrimage (on cooldown)");
            y += 35;

            // Temple Blessing (50+ piety)
            bool canBlessing = pietyData.Piety >= 50;
            AddButton(20, y, canBlessing ? 4005 : 4006, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(60, y, canBlessing ? 0x35 : 0x22, "Temple Blessing (50+ piety)");
            y += 35;

            // Group Ritual (500+ piety, weekly)
            bool canRitual = pietyData.Piety >= 500;
            AddButton(20, y, canRitual ? 4005 : 4006, 4007, 5, GumpButtonType.Reply, 0);
            AddLabel(60, y, canRitual ? 0x35 : 0x22, "Lead Group Ritual (500+ piety, weekly)");
            y += 35;

            // Recharge Powers (100+ piety at temple)
            bool canRecharge = pietyData.Piety >= 100;
            AddButton(20, y, canRecharge ? 4005 : 4006, 4007, 6, GumpButtonType.Reply, 0);
            AddLabel(60, y, canRecharge ? 0x35 : 0x22, "Recharge Devotion Powers (100+ piety)");
            y += 35;

            // Bless Item (300+ piety at temple, reduced from 500)
            bool canBless = pietyData.Piety >= 300;
            AddButton(20, y, canBless ? 4005 : 4006, 4007, 7, GumpButtonType.Reply, 0);
            AddLabel(60, y, canBless ? 0x35 : 0x22, "Bless Item (300+ piety)");
            y += 35;

            // Resurrect (free at temple)
            AddButton(20, y, 4005, 4007, 8, GumpButtonType.Reply, 0);
            AddLabel(60, y, 0x35, "Free Resurrection");
            y += 35;

            // Info
            y += 20;
            AddHtml(20, y, 310, 60, "<basefont color=#888888>Major temples provide enhanced piety gains, healing aura, reduced requirements for blessings, and free resurrection.</basefont>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null || m_Temple == null || m_Temple.Deleted)
                return;

            switch (info.ButtonID)
            {
                case 1: m_Temple.HandleTempleFunction(m_Player, TempleFunction.Pray); break;
                case 2: m_Temple.HandleTempleFunction(m_Player, TempleFunction.Tithe); break;
                case 3: m_Temple.HandleTempleFunction(m_Player, TempleFunction.Pilgrimage); break;
                case 4: m_Temple.HandleTempleFunction(m_Player, TempleFunction.TempleBlessing); break;
                case 5: m_Temple.HandleTempleFunction(m_Player, TempleFunction.GroupRitual); break;
                case 6: m_Temple.HandleTempleFunction(m_Player, TempleFunction.RechargePowers); break;
                case 7: m_Temple.HandleTempleFunction(m_Player, TempleFunction.BlessItem); break;
                case 8: m_Temple.HandleTempleFunction(m_Player, TempleFunction.Resurrect); break;
            }
        }
    }

    #endregion

    #region Frostfather Temple

    /// <summary>
    /// Temple of the Frostfather - Frosthold region major temple
    /// Provides ice/cold damage and resistance bonuses
    /// </summary>
    public class FrostfatherTemple : BaseMajorTemple
    {
        public override string TempleName => "Temple of the Frostfather";
        public override int TempleHue => 1150; // Ice blue
        public override string BlessingName => "the Eternal Winter";

        [Constructable]
        public FrostfatherTemple()
            : base(VystiaReligion.FrosthelmFaith, 0x14F0) // Stone altar
        {
        }

        public FrostfatherTemple(Serial serial) : base(serial) { }

        protected override void GrantTempleBlessing(PlayerMobile pm, PietyData pietyData)
        {
            if (pietyData.Piety < 50)
            {
                pm.SendMessage(0x22, "You need at least 50 piety to receive the temple blessing.");
                return;
            }

            // Grant 30-minute cold resistance and damage buff
            int tier = (int)ReligionData.GetTier(pietyData.Piety);
            int resistBonus = 5 + (tier * 3); // 8-17% cold resist
            int damageBonus = 3 + (tier * 2); // 5-11% cold damage

            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, resistBonus));
            pm.AddStatMod(new StatMod(StatType.Str, "FrostfatherBlessing", tier, TimeSpan.FromMinutes(30)));

            pm.PlaySound(0x10B); // Ice sound
            pm.FixedParticles(0x376A, 9, 32, 5005, 1150, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "The Frostfather's blessing surrounds you with icy power! (+{0}% Cold Resist, +{1} STR for 30 min)", resistBonus, tier);

            Timer.DelayCall(TimeSpan.FromMinutes(30), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Cold, resistBonus));
            });
        }

        protected override void ApplyRitualBuff(PlayerMobile pm)
        {
            // Ritual buff: Cold immunity for 1 hour
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, 20));
            pm.SendMessage(0x35, "You feel immune to the cold for 1 hour.");

            Timer.DelayCall(TimeSpan.FromHours(1), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Cold, 20));
                pm.SendMessage("The ritual's cold protection fades.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Emberheart Temple

    /// <summary>
    /// Temple of the Emberheart - Emberlands region major temple
    /// Provides fire damage and resistance bonuses
    /// </summary>
    public class EmberheartTemple : BaseMajorTemple
    {
        public override string TempleName => "Temple of the Emberheart";
        public override int TempleHue => 1358; // Fire red
        public override string BlessingName => "the Eternal Flame";

        [Constructable]
        public EmberheartTemple()
            : base(VystiaReligion.SuryasSandscript, 0x14F0) // Stone altar
        {
        }

        public EmberheartTemple(Serial serial) : base(serial) { }

        protected override void GrantTempleBlessing(PlayerMobile pm, PietyData pietyData)
        {
            if (pietyData.Piety < 50)
            {
                pm.SendMessage(0x22, "You need at least 50 piety to receive the temple blessing.");
                return;
            }

            int tier = (int)ReligionData.GetTier(pietyData.Piety);
            int resistBonus = 5 + (tier * 3);
            int damageBonus = 3 + (tier * 2);

            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, resistBonus));
            pm.AddStatMod(new StatMod(StatType.Int, "EmberheartBlessing", tier, TimeSpan.FromMinutes(30)));

            pm.PlaySound(0x208); // Fire sound
            pm.FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "The Emberheart's flame empowers you! (+{0}% Fire Resist, +{1} INT for 30 min)", resistBonus, tier);

            Timer.DelayCall(TimeSpan.FromMinutes(30), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Fire, resistBonus));
            });
        }

        protected override void ApplyRitualBuff(PlayerMobile pm)
        {
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, 20));
            pm.SendMessage(0x35, "The eternal flame shields you for 1 hour.");

            Timer.DelayCall(TimeSpan.FromHours(1), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Fire, 20));
                pm.SendMessage("The ritual's fire protection fades.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Greenward Temple

    /// <summary>
    /// Temple of the Greenward - Verdantpeak region major temple
    /// Provides nature/healing bonuses
    /// </summary>
    public class GreenwardTemple : BaseMajorTemple
    {
        public override string TempleName => "Temple of the Lunara's Covenant";
        public override int TempleHue => 2010; // Forest green
        public override string BlessingName => "Nature's Balance";

        [Constructable]
        public GreenwardTemple()
            : base(VystiaReligion.LunarasCovenant, 0x14F0)
        {
        }

        public GreenwardTemple(Serial serial) : base(serial) { }

        protected override void GrantTempleBlessing(PlayerMobile pm, PietyData pietyData)
        {
            if (pietyData.Piety < 50)
            {
                pm.SendMessage(0x22, "You need at least 50 piety to receive the temple blessing.");
                return;
            }

            int tier = (int)ReligionData.GetTier(pietyData.Piety);
            int resistBonus = 5 + (tier * 3);
            int regenBonus = 1 + tier; // HP regen bonus

            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Poison, resistBonus));

            pm.PlaySound(0x1E9); // Nature sound
            pm.FixedParticles(0x376A, 9, 32, 5005, 2010, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "Nature's blessing flows through you! (+{0}% Poison Resist, enhanced healing for 30 min)", resistBonus);

            Timer.DelayCall(TimeSpan.FromMinutes(30), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Poison, resistBonus));
            });
        }

        protected override void ApplyRitualBuff(PlayerMobile pm)
        {
            // Full heal and regeneration buff
            pm.Hits = pm.HitsMax;
            pm.Mana = pm.ManaMax;
            pm.Stam = pm.StamMax;
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Poison, 20));
            pm.SendMessage(0x35, "Nature fully restores you and protects against poison for 1 hour.");

            Timer.DelayCall(TimeSpan.FromHours(1), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Poison, 20));
                pm.SendMessage("The ritual's nature protection fades.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Crystalline Temple

    /// <summary>
    /// Temple of Celestis Arcanum - Crystal Barrens region major temple
    /// Provides arcane/mana bonuses
    /// </summary>
    public class CrystallineTemple : BaseMajorTemple
    {
        public override string TempleName => "Temple of Celestis Arcanum";
        public override int TempleHue => 1154; // Crystal blue
        public override string BlessingName => "Cosmic Knowledge";

        [Constructable]
        public CrystallineTemple()
            : base(VystiaReligion.CelestisArcanum, 0x14F0)
        {
        }

        public CrystallineTemple(Serial serial) : base(serial) { }

        protected override void GrantTempleBlessing(PlayerMobile pm, PietyData pietyData)
        {
            if (pietyData.Piety < 50)
            {
                pm.SendMessage(0x22, "You need at least 50 piety to receive the temple blessing.");
                return;
            }

            int tier = (int)ReligionData.GetTier(pietyData.Piety);
            int resistBonus = 5 + (tier * 3);
            int intBonus = 5 + (tier * 2);

            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Energy, resistBonus));
            pm.AddStatMod(new StatMod(StatType.Int, "CrystallineBlessing", intBonus, TimeSpan.FromMinutes(30)));

            pm.PlaySound(0x1E8); // Crystal sound
            pm.FixedParticles(0x376A, 9, 32, 5005, 1154, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "Cosmic knowledge fills your mind! (+{0}% Energy Resist, +{1} INT for 30 min)", resistBonus, intBonus);

            Timer.DelayCall(TimeSpan.FromMinutes(30), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Energy, resistBonus));
            });
        }

        protected override void ApplyRitualBuff(PlayerMobile pm)
        {
            pm.Mana = pm.ManaMax;
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Energy, 20));
            pm.AddStatMod(new StatMod(StatType.Int, "CrystallineRitual", 15, TimeSpan.FromHours(1)));
            pm.SendMessage(0x35, "Your mana is restored and your mind expanded for 1 hour.");

            Timer.DelayCall(TimeSpan.FromHours(1), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Energy, 20));
                pm.SendMessage("The ritual's arcane enhancement fades.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Voidwalker Temple

    /// <summary>
    /// Temple of the Voidwalker - ShadowVoid region major temple
    /// Provides stealth/shadow bonuses
    /// </summary>
    public class VoidwalkerTemple : BaseMajorTemple
    {
        public override string TempleName => "Temple of the Oceana's Covenant";
        public override int TempleHue => 1109; // Dark purple
        public override string BlessingName => "the Void";

        [Constructable]
        public VoidwalkerTemple()
            : base(VystiaReligion.OceanasCovenant, 0x14F0)
        {
        }

        public VoidwalkerTemple(Serial serial) : base(serial) { }

        protected override void GrantTempleBlessing(PlayerMobile pm, PietyData pietyData)
        {
            if (pietyData.Piety < 50)
            {
                pm.SendMessage(0x22, "You need at least 50 piety to receive the temple blessing.");
                return;
            }

            int tier = (int)ReligionData.GetTier(pietyData.Piety);
            int resistBonus = 3 + (tier * 2); // Physical resist
            int dexBonus = 5 + (tier * 2);

            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Physical, resistBonus));
            pm.AddStatMod(new StatMod(StatType.Dex, "VoidwalkerBlessing", dexBonus, TimeSpan.FromMinutes(30)));

            pm.PlaySound(0x22C); // Shadow sound
            pm.FixedParticles(0x376A, 9, 32, 5005, 1109, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "Shadow wraps around you! (+{0}% Physical Resist, +{1} DEX for 30 min)", resistBonus, dexBonus);

            Timer.DelayCall(TimeSpan.FromMinutes(30), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Physical, resistBonus));
            });
        }

        protected override void ApplyRitualBuff(PlayerMobile pm)
        {
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Physical, 15));
            pm.AddStatMod(new StatMod(StatType.Dex, "VoidwalkerRitual", 20, TimeSpan.FromHours(1)));
            pm.SendMessage(0x35, "The void embraces you with shadow armor for 1 hour.");

            Timer.DelayCall(TimeSpan.FromHours(1), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Physical, 15));
                pm.SendMessage("The ritual's shadow protection fades.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Cogsmith Creed Temple

    /// <summary>
    /// Temple of the Cogsmith Creed - Ironclad region major temple
    /// Provides crafting and armor bonuses
    /// </summary>
    public class CogsmithCreedTemple : BaseMajorTemple
    {
        public override string TempleName => "Temple of the Cogsmith Creed";
        public override int TempleHue => 2305; // Metallic
        public override string BlessingName => "the Machine Spirit";

        [Constructable]
        public CogsmithCreedTemple()
            : base(VystiaReligion.CogsmithCreed, 0x14F0)
        {
        }

        public CogsmithCreedTemple(Serial serial) : base(serial) { }

        protected override void GrantTempleBlessing(PlayerMobile pm, PietyData pietyData)
        {
            if (pietyData.Piety < 50)
            {
                pm.SendMessage(0x22, "You need at least 50 piety to receive the temple blessing.");
                return;
            }

            int tier = (int)ReligionData.GetTier(pietyData.Piety);
            int strBonus = 5 + (tier * 3);

            pm.AddStatMod(new StatMod(StatType.Str, "CogsmithCreedBlessing", strBonus, TimeSpan.FromMinutes(30)));

            pm.PlaySound(0x2A); // Anvil sound
            pm.FixedParticles(0x376A, 9, 32, 5005, 2305, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "The Machine Spirit empowers your craft! (+{0} STR, enhanced crafting for 30 min)", strBonus);
        }

        protected override void ApplyRitualBuff(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "CogsmithCreedRitual", 25, TimeSpan.FromHours(1)));
            pm.SendMessage(0x35, "The forge's power flows through you for 1 hour (+25 STR).");

            Timer.DelayCall(TimeSpan.FromHours(1), () =>
            {
                pm.SendMessage("The ritual's forge empowerment fades.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion
}

