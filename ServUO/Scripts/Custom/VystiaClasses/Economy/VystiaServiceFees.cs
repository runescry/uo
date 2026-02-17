using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using Server.Regions;

namespace Server.Custom.VystiaClasses.Economy
{
    /// <summary>
    /// Vystia Service Fees - Gold sinks for NPC services
    ///
    /// Resurrection Service:
    /// - Base cost: 50g
    /// - Per level (fame/karma based): +10g per level
    /// - NPC Healers charge this fee
    ///
    /// Travel Services:
    /// - Moongate travel: 100-250g based on distance
    /// - Recall failure insurance: 25g
    /// - Public moongate attendants charge fees
    ///
    /// Stabling:
    /// - Daily fee: 30g per pet
    /// - Weekly fee: 150g per pet (discounted)
    /// </summary>
    public static class VystiaServiceFees
    {
        #region Resurrection Fees

        public const int BaseResurrectionCost = 50;
        public const int CostPerFameLevel = 10;

        /// <summary>
        /// Calculate resurrection cost based on player's fame
        /// </summary>
        public static int CalculateResurrectionCost(Mobile m)
        {
            // Base cost + fame modifier
            int fameLevel = GetFameLevel(m.Fame);
            return BaseResurrectionCost + (fameLevel * CostPerFameLevel);
        }

        private static int GetFameLevel(int fame)
        {
            if (fame >= 15000) return 5;
            if (fame >= 10000) return 4;
            if (fame >= 5000) return 3;
            if (fame >= 2500) return 2;
            if (fame >= 1250) return 1;
            return 0;
        }

        /// <summary>
        /// Perform resurrection and charge fee
        /// </summary>
        public static bool DoResurrection(Mobile target, Mobile healer)
        {
            if (target == null || target.Alive)
                return false;

            PlayerMobile pm = target as PlayerMobile;
            if (pm == null)
                return false;

            int cost = CalculateResurrectionCost(pm);
            int playerGold = Banker.GetBalance(pm) + GetBackpackGold(pm);

            if (playerGold < cost)
            {
                if (healer != null)
                    healer.Say("I'm sorry, but you need {0:N0} gold for my services.", cost);
                pm.SendMessage(0x22, "You need {0:N0} gold to be resurrected.", cost);
                return false;
            }

            // Deduct gold
            if (!DeductGold(pm, cost))
            {
                pm.SendMessage(0x22, "Failed to deduct gold for resurrection.");
                return false;
            }

            // Perform resurrection
            pm.Resurrect();
            pm.Hits = pm.HitsMax / 2; // Resurrect at 50% health

            // Effects
            pm.PlaySound(0x214);
            pm.FixedEffect(0x376A, 10, 16);

            pm.SendMessage(0x35, "You have been resurrected for {0:N0} gold.", cost);

            if (healer != null)
                healer.Say("May the spirits guide you. Be more careful next time.");

            return true;
        }

        #endregion

        #region Travel Fees

        public const int ShortDistanceTravelCost = 100;
        public const int MediumDistanceTravelCost = 175;
        public const int LongDistanceTravelCost = 250;
        public const int RecallInsuranceCost = 25;

        /// <summary>
        /// Calculate travel cost based on distance
        /// </summary>
        public static int CalculateTravelCost(Point3D from, Point3D to)
        {
            double distance = GetDistance(from, to);

            if (distance < 500)
                return ShortDistanceTravelCost;
            if (distance < 1500)
                return MediumDistanceTravelCost;
            return LongDistanceTravelCost;
        }

        private static double GetDistance(Point3D a, Point3D b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Charge for moongate travel
        /// </summary>
        public static bool ChargeTravelFee(Mobile m, int cost, string destination)
        {
            PlayerMobile pm = m as PlayerMobile;
            if (pm == null)
                return true; // NPCs travel free

            int playerGold = Banker.GetBalance(pm) + GetBackpackGold(pm);

            if (playerGold < cost)
            {
                pm.SendMessage(0x22, "You need {0:N0} gold to travel to {1}.", cost, destination);
                return false;
            }

            if (!DeductGold(pm, cost))
            {
                pm.SendMessage(0x22, "Failed to deduct travel fee.");
                return false;
            }

            pm.SendMessage(0x35, "Travel fee of {0:N0} gold deducted for travel to {1}.", cost, destination);
            return true;
        }

        #endregion

        #region Stabling Fees

        public const int DailyStablingCost = 30;
        public const int WeeklyStablingCost = 150; // ~5 days worth, 2 days free

        /// <summary>
        /// Calculate stabling cost
        /// </summary>
        public static int CalculateStablingCost(int days)
        {
            if (days >= 7)
                return (days / 7) * WeeklyStablingCost + (days % 7) * DailyStablingCost;
            return days * DailyStablingCost;
        }

        /// <summary>
        /// Charge for pet stabling
        /// </summary>
        public static bool ChargeStablingFee(Mobile m, int cost, string petName)
        {
            PlayerMobile pm = m as PlayerMobile;
            if (pm == null)
                return false;

            int playerGold = Banker.GetBalance(pm) + GetBackpackGold(pm);

            if (playerGold < cost)
            {
                pm.SendMessage(0x22, "You need {0:N0} gold to stable {1}.", cost, petName);
                return false;
            }

            if (!DeductGold(pm, cost))
            {
                pm.SendMessage(0x22, "Failed to deduct stabling fee.");
                return false;
            }

            pm.SendMessage(0x35, "Stabling fee of {0:N0} gold charged for {1}.", cost, petName);
            return true;
        }

        #endregion

        #region Helper Methods

        private static int GetBackpackGold(Mobile m)
        {
            if (m.Backpack == null)
                return 0;

            Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
            int total = 0;
            foreach (Item g in gold)
                total += g.Amount;
            return total;
        }

        private static bool DeductGold(Mobile m, int amount)
        {
            // Try backpack first
            if (m.Backpack != null)
            {
                Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
                int backpackGold = 0;
                foreach (Item g in gold)
                    backpackGold += g.Amount;

                if (backpackGold >= amount)
                {
                    m.Backpack.ConsumeTotal(typeof(Gold), amount);
                    return true;
                }
                else if (backpackGold > 0)
                {
                    m.Backpack.ConsumeTotal(typeof(Gold), backpackGold);
                    amount -= backpackGold;
                }
            }

            // Remainder from bank
            return Banker.Withdraw(m, amount);
        }

        #endregion
    }

    #region Vystia Healer NPC

    /// <summary>
    /// Vystia Healer with resurrection service (for fee)
    /// </summary>
    [CorpseNameAttribute("corpse of a healer")]
    public class VystiaHealer : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public VystiaHealer() : base("the healer")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Healer";

            SetSkill(SkillName.Healing, 85.0, 100.0);
            SetSkill(SkillName.Anatomy, 75.0, 90.0);
            SetSkill(SkillName.SpiritSpeak, 65.0, 88.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBHealer());
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this.Location, 3))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("resurrect") || speech.Contains("rez") || speech.Contains("revive"))
                {
                    if (!from.Alive)
                    {
                        int cost = VystiaServiceFees.CalculateResurrectionCost(from);
                        SayTo(from, "I can restore you to life for {0:N0} gold. Say 'accept' to proceed.", cost);
                        from.SendGump(new VystiaResurrectionGump(this, from as PlayerMobile));
                    }
                    else
                    {
                        SayTo(from, "You appear to be quite alive already!");
                    }
                    e.Handled = true;
                }
                else if (speech.Contains("heal"))
                {
                    if (from.Hits < from.HitsMax)
                    {
                        // Free healing for the living (bandages cost gold elsewhere)
                        from.Hits = from.HitsMax;
                        SayTo(from, "There you go, good as new!");
                        from.PlaySound(0x1F2);
                    }
                    else
                    {
                        SayTo(from, "You're already in perfect health!");
                    }
                    e.Handled = true;
                }
                else if (speech.Contains("accept") && !from.Alive)
                {
                    VystiaServiceFees.DoResurrection(from, this);
                    e.Handled = true;
                }
            }

            base.OnSpeech(e);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 4))
            {
                if (!from.Alive)
                {
                    int cost = VystiaServiceFees.CalculateResurrectionCost(from);
                    SayTo(from, "Poor soul. I can bring you back for {0:N0} gold. Say 'resurrect' to proceed.", cost);
                }
                else
                {
                    SayTo(from, "Greetings! I can heal your wounds or resurrect the fallen. How may I help?");
                }
            }

            base.OnDoubleClick(from);
        }

        public VystiaHealer(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion

    #region Resurrection Gump

    public class VystiaResurrectionGump : Gump
    {
        private Mobile m_Healer;
        private PlayerMobile m_Player;

        public VystiaResurrectionGump(Mobile healer, PlayerMobile player) : base(100, 100)
        {
            m_Healer = healer;
            m_Player = player;

            if (player == null || player.Alive)
                return;

            int cost = VystiaServiceFees.CalculateResurrectionCost(player);
            int playerGold = Banker.GetBalance(player) + GetBackpackGold(player);
            bool canAfford = playerGold >= cost;

            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);

            AddBackground(0, 0, 300, 180, 9270);
            AddAlphaRegion(10, 10, 280, 160);

            AddHtml(15, 15, 270, 25, "<CENTER><BASEFONT COLOR=#FFD700><BIG>Resurrection Service</BIG></BASEFONT></CENTER>", false, false);

            string healerName = healer != null ? healer.Name : "the healer";
            AddHtml(15, 45, 270, 40, String.Format("<CENTER><BASEFONT COLOR=#FFFFFF>{0} offers to restore you to life.</BASEFONT></CENTER>", healerName), false, false);

            AddHtml(15, 85, 270, 20, String.Format("<CENTER><BASEFONT COLOR=#FFD700>Cost: {0:N0} gold</BASEFONT></CENTER>", cost), false, false);
            AddHtml(15, 105, 270, 20, String.Format("<CENTER><BASEFONT COLOR={0}>Your Gold: {1:N0}</BASEFONT></CENTER>",
                canAfford ? "#AAFFAA" : "#FF6666", playerGold), false, false);

            if (canAfford)
            {
                AddButton(50, 135, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtml(85, 138, 60, 20, "<BASEFONT COLOR=#AAFFAA>Accept</BASEFONT>", false, false);
            }

            AddButton(180, 135, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtml(215, 138, 60, 20, "<BASEFONT COLOR=#FFFFFF>Decline</BASEFONT>", false, false);
        }

        private int GetBackpackGold(Mobile m)
        {
            if (m.Backpack == null)
                return 0;

            Item[] gold = m.Backpack.FindItemsByType(typeof(Gold));
            int total = 0;
            foreach (Item g in gold)
                total += g.Amount;
            return total;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1 && m_Player != null && !m_Player.Alive)
            {
                VystiaServiceFees.DoResurrection(m_Player, m_Healer);
            }
        }
    }

    #endregion

    #region Moongate Attendant

    /// <summary>
    /// Vystia Moongate Attendant - Charges for moongate travel
    /// </summary>
    [CorpseNameAttribute("corpse of a mage")]
    public class VystiaMoongateAttendant : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override bool IsActiveVendor { get { return false; } }

        [Constructable]
        public VystiaMoongateAttendant() : base("the moongate attendant")
        {
            Name = NameList.RandomName(Female ? "female" : "male");
            Title = "the Moongate Attendant";

            SetSkill(SkillName.Magery, 75.0, 90.0);
            SetSkill(SkillName.EvalInt, 65.0, 80.0);
        }

        public override void InitSBInfo()
        {
            // Doesn't sell items
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this.Location, 3))
            {
                string speech = e.Speech.ToLower();

                if (speech.Contains("travel") || speech.Contains("gate") || speech.Contains("teleport"))
                {
                    SayTo(from, "The moongates require a fee to maintain. Short trips cost {0}g, medium {1}g, and long distance {2}g.",
                        VystiaServiceFees.ShortDistanceTravelCost,
                        VystiaServiceFees.MediumDistanceTravelCost,
                        VystiaServiceFees.LongDistanceTravelCost);
                    e.Handled = true;
                }
                else if (speech.Contains("cost") || speech.Contains("price") || speech.Contains("fee"))
                {
                    SayTo(from, "Travel fees: Short ({0}g), Medium ({1}g), Long ({2}g) based on distance.",
                        VystiaServiceFees.ShortDistanceTravelCost,
                        VystiaServiceFees.MediumDistanceTravelCost,
                        VystiaServiceFees.LongDistanceTravelCost);
                    e.Handled = true;
                }
            }

            base.OnSpeech(e);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 4))
            {
                SayTo(from, "Greetings, traveler. The moongates require a small fee to maintain their magic. Say 'travel' for rates.");
            }

            base.OnDoubleClick(from);
        }

        public VystiaMoongateAttendant(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    #endregion
}
