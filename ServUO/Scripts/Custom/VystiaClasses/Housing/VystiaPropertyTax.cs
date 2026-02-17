/*
 * Vystia Property Tax System
 *
 * Handles weekly tax collection from house owners.
 * Tax rates are based on house size (defined in VystiaHousingCosts).
 *
 * Features:
 * - Weekly tax collection from owner's bank
 * - Grace period for late payments (7 days)
 * - House decay for non-payment after grace period
 * - Tax exemption for staff accounts
 * - GM commands for tax management
 */

using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Accounting;
using Server.Targeting;

namespace Server.Custom.VystiaClasses.Housing
{
    #region Tax Status

    /// <summary>
    /// Tax payment status for a house
    /// </summary>
    public enum TaxStatus
    {
        Current,        // Taxes paid, no issues
        Due,            // Tax collection pending
        Overdue,        // Past due, in grace period
        Delinquent,     // Grace period expired, house at risk
        Exempt          // Staff or special exemption
    }

    #endregion

    #region House Tax Record

    /// <summary>
    /// Stores tax information for a single house
    /// </summary>
    public class HouseTaxRecord
    {
        private Serial m_HouseSerial;
        private DateTime m_LastPayment;
        private DateTime m_NextDue;
        private int m_TotalPaid;
        private int m_MissedPayments;
        private bool m_IsExempt;

        public Serial HouseSerial { get { return m_HouseSerial; } }
        public DateTime LastPayment { get { return m_LastPayment; } set { m_LastPayment = value; } }
        public DateTime NextDue { get { return m_NextDue; } set { m_NextDue = value; } }
        public int TotalPaid { get { return m_TotalPaid; } set { m_TotalPaid = value; } }
        public int MissedPayments { get { return m_MissedPayments; } set { m_MissedPayments = value; } }
        public bool IsExempt { get { return m_IsExempt; } set { m_IsExempt = value; } }

        public HouseTaxRecord(Serial houseSerial)
        {
            m_HouseSerial = houseSerial;
            m_LastPayment = DateTime.UtcNow;
            m_NextDue = DateTime.UtcNow.AddDays(7);
            m_TotalPaid = 0;
            m_MissedPayments = 0;
            m_IsExempt = false;
        }

        public HouseTaxRecord(Serial houseSerial, GenericReader reader)
        {
            m_HouseSerial = houseSerial;

            int version = reader.ReadInt();

            m_LastPayment = reader.ReadDateTime();
            m_NextDue = reader.ReadDateTime();
            m_TotalPaid = reader.ReadInt();
            m_MissedPayments = reader.ReadInt();
            m_IsExempt = reader.ReadBool();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); // version

            writer.Write(m_LastPayment);
            writer.Write(m_NextDue);
            writer.Write(m_TotalPaid);
            writer.Write(m_MissedPayments);
            writer.Write(m_IsExempt);
        }

        public TaxStatus GetStatus()
        {
            if (m_IsExempt)
                return TaxStatus.Exempt;

            DateTime now = DateTime.UtcNow;

            if (now < m_NextDue)
                return TaxStatus.Current;

            if (now < m_NextDue.AddDays(VystiaPropertyTax.GracePeriodDays))
                return TaxStatus.Overdue;

            return TaxStatus.Delinquent;
        }

        public int GetDaysUntilDue()
        {
            TimeSpan remaining = m_NextDue - DateTime.UtcNow;
            return Math.Max(0, (int)remaining.TotalDays);
        }

        public int GetDaysOverdue()
        {
            if (DateTime.UtcNow < m_NextDue)
                return 0;

            TimeSpan overdue = DateTime.UtcNow - m_NextDue;
            return (int)overdue.TotalDays;
        }
    }

    #endregion

    #region Property Tax System

    /// <summary>
    /// Main property tax management system
    /// </summary>
    public static class VystiaPropertyTax
    {
        // Configuration
        public static readonly int GracePeriodDays = 7;
        public static readonly int MaxMissedPayments = 3;
        public static readonly TimeSpan CollectionInterval = TimeSpan.FromHours(1); // Check every hour

        private static Dictionary<Serial, HouseTaxRecord> m_TaxRecords = new Dictionary<Serial, HouseTaxRecord>();
        private static Timer m_CollectionTimer;
        private static bool m_Enabled = true;

        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        public static Dictionary<Serial, HouseTaxRecord> TaxRecords { get { return m_TaxRecords; } }

        #region Initialization

        public static void Initialize()
        {
            // Register commands
            CommandSystem.Register("TaxInfo", AccessLevel.Player, new CommandEventHandler(TaxInfo_OnCommand));
            CommandSystem.Register("TI", AccessLevel.Player, new CommandEventHandler(TaxInfo_OnCommand));
            CommandSystem.Register("PayTax", AccessLevel.Player, new CommandEventHandler(PayTax_OnCommand));
            CommandSystem.Register("PT", AccessLevel.Player, new CommandEventHandler(PayTax_OnCommand));
            CommandSystem.Register("TaxExempt", AccessLevel.GameMaster, new CommandEventHandler(TaxExempt_OnCommand));
            CommandSystem.Register("TE", AccessLevel.GameMaster, new CommandEventHandler(TaxExempt_OnCommand));
            CommandSystem.Register("ForceTaxCollection", AccessLevel.Administrator, new CommandEventHandler(ForceTaxCollection_OnCommand));
            CommandSystem.Register("FTC", AccessLevel.Administrator, new CommandEventHandler(ForceTaxCollection_OnCommand));
            CommandSystem.Register("TaxStatus", AccessLevel.GameMaster, new CommandEventHandler(TaxStatus_OnCommand));
            CommandSystem.Register("TS", AccessLevel.GameMaster, new CommandEventHandler(TaxStatus_OnCommand));

            // Start collection timer
            StartCollectionTimer();

            // Load tax records on server start
            EventSink.WorldSave += OnWorldSave;
            EventSink.WorldLoad += OnWorldLoad;
        }

        private static void StartCollectionTimer()
        {
            if (m_CollectionTimer != null)
                m_CollectionTimer.Stop();

            m_CollectionTimer = Timer.DelayCall(CollectionInterval, CollectionInterval, ProcessTaxCollection);
        }

        #endregion

        #region Tax Record Management

        /// <summary>
        /// Get or create tax record for a house
        /// </summary>
        public static HouseTaxRecord GetTaxRecord(BaseHouse house)
        {
            if (house == null || house.Deleted)
                return null;

            if (!m_TaxRecords.TryGetValue(house.Serial, out HouseTaxRecord record))
            {
                record = new HouseTaxRecord(house.Serial);
                m_TaxRecords[house.Serial] = record;

                // Check if owner is staff (exempt)
                if (house.Owner != null && house.Owner.AccessLevel >= AccessLevel.GameMaster)
                    record.IsExempt = true;
            }

            return record;
        }

        /// <summary>
        /// Remove tax record for a demolished house
        /// </summary>
        public static void RemoveTaxRecord(Serial houseSerial)
        {
            m_TaxRecords.Remove(houseSerial);
        }

        /// <summary>
        /// Set tax exemption status for a house
        /// </summary>
        public static void SetExempt(BaseHouse house, bool exempt)
        {
            HouseTaxRecord record = GetTaxRecord(house);
            if (record != null)
            {
                record.IsExempt = exempt;
            }
        }

        #endregion

        #region Tax Collection

        /// <summary>
        /// Process tax collection for all houses (called by timer)
        /// </summary>
        private static void ProcessTaxCollection()
        {
            if (!m_Enabled)
                return;

            DateTime now = DateTime.UtcNow;
            List<Serial> toRemove = new List<Serial>();
            List<BaseHouse> toProcess = new List<BaseHouse>();

            // Find houses needing tax processing
            foreach (var kvp in m_TaxRecords)
            {
                BaseHouse house = World.FindItem(kvp.Key) as BaseHouse;

                if (house == null || house.Deleted)
                {
                    toRemove.Add(kvp.Key);
                    continue;
                }

                HouseTaxRecord record = kvp.Value;

                if (record.IsExempt)
                    continue;

                if (now >= record.NextDue)
                {
                    toProcess.Add(house);
                }
            }

            // Clean up deleted houses
            foreach (Serial serial in toRemove)
            {
                m_TaxRecords.Remove(serial);
            }

            // Process tax collection
            foreach (BaseHouse house in toProcess)
            {
                CollectTax(house);
            }
        }

        /// <summary>
        /// Attempt to collect tax from a single house
        /// </summary>
        public static bool CollectTax(BaseHouse house)
        {
            if (house == null || house.Deleted)
                return false;

            HouseTaxRecord record = GetTaxRecord(house);
            if (record == null || record.IsExempt)
                return true;

            Mobile owner = house.Owner;
            if (owner == null)
            {
                // No owner, mark as missed
                record.MissedPayments++;
                record.NextDue = DateTime.UtcNow.AddDays(7);
                HandleMissedPayment(house, record);
                return false;
            }

            // Calculate tax amount
            int taxAmount = VystiaHousingCosts.GetHouseWeeklyTax(house);

            // Try to withdraw from bank
            if (Banker.Withdraw(owner, taxAmount, true))
            {
                // Successful payment
                record.LastPayment = DateTime.UtcNow;
                record.NextDue = DateTime.UtcNow.AddDays(7);
                record.TotalPaid += taxAmount;
                record.MissedPayments = 0;

                // Notify owner if online
                if (owner.NetState != null)
                {
                    owner.SendMessage(0x35, "Property tax of {0:N0} gold has been collected for your {1}.",
                        taxAmount, house.GetType().Name);
                }

                return true;
            }
            else
            {
                // Failed payment
                record.MissedPayments++;
                record.NextDue = DateTime.UtcNow.AddDays(7);

                // Notify owner if online
                if (owner.NetState != null)
                {
                    owner.SendMessage(0x22, "WARNING: Unable to collect property tax of {0:N0} gold! " +
                        "Missed payments: {1}/{2}",
                        taxAmount, record.MissedPayments, MaxMissedPayments);
                }

                HandleMissedPayment(house, record);
                return false;
            }
        }

        /// <summary>
        /// Handle a missed tax payment
        /// </summary>
        private static void HandleMissedPayment(BaseHouse house, HouseTaxRecord record)
        {
            Mobile owner = house.Owner;

            if (record.MissedPayments >= MaxMissedPayments)
            {
                // House is condemned - begin decay process
                if (owner != null && owner.NetState != null)
                {
                    owner.SendMessage(0x22, "CRITICAL: Your {0} has been condemned for tax delinquency! " +
                        "It will begin to decay.", house.GetType().Name);
                }

                // Enable decay (house will eventually collapse)
                // Note: In ServUO, setting DecayLevel or calling specific decay methods
                // would trigger this. For now we just notify.
                Console.WriteLine("[VystiaTax] House {0} (Owner: {1}) condemned for non-payment.",
                    house.Serial, owner?.Name ?? "None");
            }
            else
            {
                int remaining = MaxMissedPayments - record.MissedPayments;

                if (owner != null && owner.NetState != null)
                {
                    owner.SendMessage(0x22, "WARNING: {0} missed tax payment(s). " +
                        "{1} more before house condemnation!",
                        record.MissedPayments, remaining);
                }
            }
        }

        /// <summary>
        /// Allow player to manually pay tax early
        /// </summary>
        public static bool PayTaxEarly(Mobile from, BaseHouse house)
        {
            if (from == null || house == null || house.Deleted)
                return false;

            if (house.Owner != from && from.AccessLevel < AccessLevel.GameMaster)
            {
                from.SendMessage("You do not own this house.");
                return false;
            }

            HouseTaxRecord record = GetTaxRecord(house);
            if (record == null)
                return false;

            if (record.IsExempt)
            {
                from.SendMessage("This house is tax exempt.");
                return true;
            }

            TaxStatus status = record.GetStatus();
            if (status == TaxStatus.Current)
            {
                from.SendMessage("Taxes are already current. Next due in {0} days.", record.GetDaysUntilDue());
                return true;
            }

            int taxAmount = VystiaHousingCosts.GetHouseWeeklyTax(house);

            // Add late fee if overdue
            if (status == TaxStatus.Overdue || status == TaxStatus.Delinquent)
            {
                int lateFee = taxAmount / 4; // 25% late fee
                taxAmount += lateFee;
                from.SendMessage("Late fee of {0:N0} gold applied.", lateFee);
            }

            if (Banker.Withdraw(from, taxAmount, true))
            {
                record.LastPayment = DateTime.UtcNow;
                record.NextDue = DateTime.UtcNow.AddDays(7);
                record.TotalPaid += taxAmount;
                record.MissedPayments = Math.Max(0, record.MissedPayments - 1);

                from.SendMessage("Tax payment of {0:N0} gold accepted. Next due in 7 days.", taxAmount);
                return true;
            }
            else
            {
                from.SendMessage("You cannot afford the tax payment of {0:N0} gold.", taxAmount);
                return false;
            }
        }

        #endregion

        #region Persistence

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                "Saves/Vystia/PropertyTax.bin",
                writer =>
                {
                    writer.Write((int)0); // version
                    writer.Write(m_TaxRecords.Count);

                    foreach (var kvp in m_TaxRecords)
                    {
                        writer.Write(kvp.Key);
                        kvp.Value.Serialize(writer);
                    }
                }
            );
        }

        private static void OnWorldLoad()
        {
            Persistence.Deserialize(
                "Saves/Vystia/PropertyTax.bin",
                reader =>
                {
                    int version = reader.ReadInt();
                    int count = reader.ReadInt();

                    m_TaxRecords.Clear();

                    for (int i = 0; i < count; i++)
                    {
                        Serial serial = reader.ReadInt();
                        HouseTaxRecord record = new HouseTaxRecord(serial, reader);
                        m_TaxRecords[serial] = record;
                    }
                }
            );
        }

        #endregion

        #region Commands

        [Usage("TaxInfo")]
        [Description("Display tax information for your house(s).")]
        private static void TaxInfo_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            List<BaseHouse> houses = new List<BaseHouse>();

            // Find all houses owned by player
            foreach (BaseHouse house in BaseHouse.AllHouses)
            {
                if (house.Owner == from)
                    houses.Add(house);
            }

            if (houses.Count == 0)
            {
                from.SendMessage("You do not own any houses.");
                return;
            }

            from.SendMessage("=== Your Property Tax Status ===");

            foreach (BaseHouse house in houses)
            {
                HouseTaxRecord record = GetTaxRecord(house);
                if (record == null)
                    continue;

                TaxStatus status = record.GetStatus();
                int weeklyTax = VystiaHousingCosts.GetHouseWeeklyTax(house);
                VystiaHouseSize size = VystiaHousingCosts.GetSizeFromHouse(house);

                from.SendMessage("");
                from.SendMessage("{0} ({1})", house.GetType().Name, size);
                from.SendMessage("  Status: {0}", status);
                from.SendMessage("  Weekly Tax: {0:N0}g", weeklyTax);

                if (status == TaxStatus.Exempt)
                {
                    from.SendMessage("  This property is tax exempt.");
                }
                else if (status == TaxStatus.Current)
                {
                    from.SendMessage("  Next Due: {0} days", record.GetDaysUntilDue());
                }
                else
                {
                    from.SendMessage("  Days Overdue: {0}", record.GetDaysOverdue());
                    from.SendMessage("  Missed Payments: {0}/{1}", record.MissedPayments, MaxMissedPayments);
                }

                from.SendMessage("  Total Paid: {0:N0}g", record.TotalPaid);
            }
        }

        [Usage("PayTax")]
        [Description("Pay property tax early for a targeted house.")]
        private static void PayTax_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // If standing in own house, pay that one
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.Owner == from)
            {
                PayTaxEarly(from, house);
            }
            else
            {
                from.SendMessage("You must be standing in your house to pay taxes.");
            }
        }

        [Usage("TaxExempt")]
        [Description("Toggle tax exemption for a targeted house.")]
        private static void TaxExempt_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("Target a house to toggle tax exemption.");
            from.Target = new TaxExemptTarget();
        }

        private class TaxExemptTarget : Target
        {
            public TaxExemptTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseHouse house = null;

                if (targeted is BaseHouse)
                    house = (BaseHouse)targeted;
                else if (targeted is HouseSign)
                    house = ((HouseSign)targeted).Owner;

                if (house == null)
                    house = BaseHouse.FindHouseAt(from);

                if (house == null)
                {
                    from.SendMessage("That is not a house.");
                    return;
                }

                HouseTaxRecord record = GetTaxRecord(house);
                if (record != null)
                {
                    record.IsExempt = !record.IsExempt;
                    from.SendMessage("House tax exemption: {0}", record.IsExempt ? "ENABLED" : "DISABLED");
                }
            }
        }

        [Usage("ForceTaxCollection")]
        [Description("Force immediate tax collection for all houses.")]
        private static void ForceTaxCollection_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("Forcing tax collection for all houses...");
            ProcessTaxCollection();
            from.SendMessage("Tax collection complete.");
        }

        [Usage("TaxStatus")]
        [Description("View detailed tax status for a targeted house.")]
        private static void TaxStatus_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("Target a house to view tax status.");
            from.Target = new TaxStatusTarget();
        }

        private class TaxStatusTarget : Target
        {
            public TaxStatusTarget() : base(12, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseHouse house = null;

                if (targeted is BaseHouse)
                    house = (BaseHouse)targeted;
                else if (targeted is HouseSign)
                    house = ((HouseSign)targeted).Owner;

                if (house == null)
                    house = BaseHouse.FindHouseAt(from);

                if (house == null)
                {
                    from.SendMessage("That is not a house.");
                    return;
                }

                HouseTaxRecord record = GetTaxRecord(house);
                if (record == null)
                {
                    from.SendMessage("No tax record found for this house.");
                    return;
                }

                TaxStatus status = record.GetStatus();
                VystiaHouseSize size = VystiaHousingCosts.GetSizeFromHouse(house);
                int weeklyTax = VystiaHousingCosts.GetHouseWeeklyTax(house);

                from.SendMessage("=== Property Tax Status ===");
                from.SendMessage("House: {0} ({1})", house.GetType().Name, size);
                from.SendMessage("Owner: {0}", house.Owner?.Name ?? "None");
                from.SendMessage("Status: {0}", status);
                from.SendMessage("Tax Exempt: {0}", record.IsExempt);
                from.SendMessage("Weekly Tax: {0:N0}g", weeklyTax);
                from.SendMessage("Last Payment: {0}", record.LastPayment.ToShortDateString());
                from.SendMessage("Next Due: {0}", record.NextDue.ToShortDateString());
                from.SendMessage("Days Until Due: {0}", record.GetDaysUntilDue());
                from.SendMessage("Days Overdue: {0}", record.GetDaysOverdue());
                from.SendMessage("Missed Payments: {0}/{1}", record.MissedPayments, MaxMissedPayments);
                from.SendMessage("Total Paid: {0:N0}g", record.TotalPaid);
            }
        }

        #endregion
    }

    #endregion
}
