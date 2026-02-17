using System;
using Server.Custom.VystiaClasses.Religion;

namespace Server.Custom.VystiaClasses.Religion
{
    /// <summary>
    /// Interface for items that have been blessed at a shrine
    /// </summary>
    public interface IBlessedItem
    {
        /// <summary>
        /// The religion that blessed this item
        /// </summary>
        VystiaReligion BlessingReligion { get; set; }

        /// <summary>
        /// The type of blessing (Standard or Critical)
        /// </summary>
        BlessingType BlessingType { get; set; }

        /// <summary>
        /// When the item was blessed
        /// </summary>
        DateTime BlessedDate { get; set; }
    }

    /// <summary>
    /// Type of blessing applied to an item
    /// </summary>
    public enum BlessingType
    {
        None = 0,
        Standard = 1,    // Normal blessing
        Critical = 2     // Critical blessing (enhanced effects)
    }
}
