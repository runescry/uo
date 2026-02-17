// Vystia Faction Token Currency System
// 7 faction-specific tokens used as currency for faction vendors

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Items.Vystia
{
    #region Base Faction Token

    /// <summary>
    /// Base class for faction currency tokens
    /// Tokens are earned from reputation gains and spent at faction vendors
    /// </summary>
    public abstract class BaseFactionToken : Item
    {
        public abstract VystiaFaction TokenFaction { get; }
        public abstract string FactionName { get; }

        public BaseFactionToken(int amount = 1) : base(0x2AAA) // Coin graphic
        {
            Name = FactionName + " Token";
            Hue = GetFactionHue();
            Stackable = true;
            Amount = amount;
            Weight = 0.02;
        }

        public BaseFactionToken(Serial serial) : base(serial) { }

        protected abstract int GetFactionHue();

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm)
            {
                var info = FactionData.GetInfo(TokenFaction);
                var tier = VystiaReputation.GetFactionTier(pm, TokenFaction);
                pm.SendMessage(info?.Hue ?? 0, "You have {0} {1} tokens. Your standing: {2}", Amount, FactionName, tier);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Faction\t{0}", FactionName);
            list.Add("Currency for faction vendors");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Frostguard Token

    /// <summary>
    /// Frostguard faction currency token
    /// </summary>
    public class FrostguardToken : BaseFactionToken
    {
        public override VystiaFaction TokenFaction => VystiaFaction.Frostguard;
        public override string FactionName => "Frostguard";

        [Constructable]
        public FrostguardToken() : this(1) { }

        [Constructable]
        public FrostguardToken(int amount) : base(amount)
        {
            ItemID = 0x2AAA;
        }

        public FrostguardToken(Serial serial) : base(serial) { }

        protected override int GetFactionHue() => 1150; // Ice blue

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

    #region Flame Legion Token

    /// <summary>
    /// Flame Legion faction currency token
    /// </summary>
    public class FlameLegionToken : BaseFactionToken
    {
        public override VystiaFaction TokenFaction => VystiaFaction.FlameLegion;
        public override string FactionName => "Flame Legion";

        [Constructable]
        public FlameLegionToken() : this(1) { }

        [Constructable]
        public FlameLegionToken(int amount) : base(amount)
        {
            ItemID = 0x2AAA;
        }

        public FlameLegionToken(Serial serial) : base(serial) { }

        protected override int GetFactionHue() => 1358; // Fire red

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

    #region Greenward Token

    /// <summary>
    /// Greenward faction currency token
    /// </summary>
    public class GreenwardToken : BaseFactionToken
    {
        public override VystiaFaction TokenFaction => VystiaFaction.Greenward;
        public override string FactionName => "Greenward";

        [Constructable]
        public GreenwardToken() : this(1) { }

        [Constructable]
        public GreenwardToken(int amount) : base(amount)
        {
            ItemID = 0x2AAA;
        }

        public GreenwardToken(Serial serial) : base(serial) { }

        protected override int GetFactionHue() => 2010; // Forest green

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

    #region Arcane Conclave Token

    /// <summary>
    /// Arcane Conclave faction currency token
    /// </summary>
    public class ArcaneConclaveToken : BaseFactionToken
    {
        public override VystiaFaction TokenFaction => VystiaFaction.ArcaneConclave;
        public override string FactionName => "Arcane Conclave";

        [Constructable]
        public ArcaneConclaveToken() : this(1) { }

        [Constructable]
        public ArcaneConclaveToken(int amount) : base(amount)
        {
            ItemID = 0x2AAA;
        }

        public ArcaneConclaveToken(Serial serial) : base(serial) { }

        protected override int GetFactionHue() => 1154; // Crystal blue

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

    #region Technoguild Token

    /// <summary>
    /// Technoguild faction currency token
    /// </summary>
    public class TechnoguildToken : BaseFactionToken
    {
        public override VystiaFaction TokenFaction => VystiaFaction.Technoguild;
        public override string FactionName => "Technoguild";

        [Constructable]
        public TechnoguildToken() : this(1) { }

        [Constructable]
        public TechnoguildToken(int amount) : base(amount)
        {
            ItemID = 0x2AAA;
        }

        public TechnoguildToken(Serial serial) : base(serial) { }

        protected override int GetFactionHue() => 2305; // Metallic

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

    #region Sandwalkers Token

    /// <summary>
    /// Sandwalkers faction currency token
    /// </summary>
    public class SandwalkersToken : BaseFactionToken
    {
        public override VystiaFaction TokenFaction => VystiaFaction.Sandwalkers;
        public override string FactionName => "Sandwalkers";

        [Constructable]
        public SandwalkersToken() : this(1) { }

        [Constructable]
        public SandwalkersToken(int amount) : base(amount)
        {
            ItemID = 0x2AAA;
        }

        public SandwalkersToken(Serial serial) : base(serial) { }

        protected override int GetFactionHue() => 1719; // Sand/tan

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

    #region Voidborn Token

    /// <summary>
    /// Voidborn faction currency token
    /// </summary>
    public class VoidbornToken : BaseFactionToken
    {
        public override VystiaFaction TokenFaction => VystiaFaction.Voidborn;
        public override string FactionName => "Voidborn";

        [Constructable]
        public VoidbornToken() : this(1) { }

        [Constructable]
        public VoidbornToken(int amount) : base(amount)
        {
            ItemID = 0x2AAA;
        }

        public VoidbornToken(Serial serial) : base(serial) { }

        protected override int GetFactionHue() => 1109; // Dark purple

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

    #region Token Helper System

    /// <summary>
    /// Helper class for faction token operations
    /// </summary>
    public static class FactionTokenSystem
    {
        /// <summary>
        /// Get the token type for a specific faction
        /// </summary>
        public static Type GetTokenType(VystiaFaction faction)
        {
            switch (faction)
            {
                case VystiaFaction.Frostguard: return typeof(FrostguardToken);
                case VystiaFaction.FlameLegion: return typeof(FlameLegionToken);
                case VystiaFaction.Greenward: return typeof(GreenwardToken);
                case VystiaFaction.ArcaneConclave: return typeof(ArcaneConclaveToken);
                case VystiaFaction.Technoguild: return typeof(TechnoguildToken);
                case VystiaFaction.Sandwalkers: return typeof(SandwalkersToken);
                case VystiaFaction.Voidborn: return typeof(VoidbornToken);
                default: return null;
            }
        }

        /// <summary>
        /// Grant faction tokens to a player
        /// </summary>
        public static bool GiveTokens(PlayerMobile pm, VystiaFaction faction, int amount)
        {
            if (pm == null || faction == VystiaFaction.None || amount <= 0)
                return false;

            Type tokenType = GetTokenType(faction);
            if (tokenType == null)
                return false;

            BaseFactionToken token = (BaseFactionToken)Activator.CreateInstance(tokenType, new object[] { amount });
            if (token == null)
                return false;

            pm.Backpack?.DropItem(token);

            var info = FactionData.GetInfo(faction);
            pm.SendMessage(info?.Hue ?? 0, "You receive {0} {1} token(s).", amount, info?.Name ?? faction.ToString());

            return true;
        }

        /// <summary>
        /// Check if player has enough faction tokens
        /// </summary>
        public static bool HasTokens(PlayerMobile pm, VystiaFaction faction, int amount)
        {
            if (pm?.Backpack == null)
                return false;

            Type tokenType = GetTokenType(faction);
            if (tokenType == null)
                return false;

            int total = 0;
            foreach (Item item in pm.Backpack.Items)
            {
                if (item.GetType() == tokenType)
                    total += item.Amount;
            }

            return total >= amount;
        }

        /// <summary>
        /// Consume faction tokens from player
        /// </summary>
        public static bool ConsumeTokens(PlayerMobile pm, VystiaFaction faction, int amount)
        {
            if (pm?.Backpack == null)
                return false;

            Type tokenType = GetTokenType(faction);
            if (tokenType == null)
                return false;

            return pm.Backpack.ConsumeTotal(tokenType, amount);
        }

        /// <summary>
        /// Get total token count for a faction
        /// </summary>
        public static int GetTokenCount(PlayerMobile pm, VystiaFaction faction)
        {
            if (pm?.Backpack == null)
                return 0;

            Type tokenType = GetTokenType(faction);
            if (tokenType == null)
                return 0;

            int total = 0;
            foreach (Item item in pm.Backpack.Items)
            {
                if (item.GetType() == tokenType)
                    total += item.Amount;
            }

            return total;
        }

        /// <summary>
        /// Convert reputation gain to token rewards
        /// Called when reputation is gained to grant bonus tokens
        /// </summary>
        public static void OnReputationGain(PlayerMobile pm, VystiaFaction faction, int repGain, string source)
        {
            if (pm == null || faction == VystiaFaction.None || repGain <= 0)
                return;

            // Token rewards based on reputation source
            int tokenReward = 0;

            // Quest rewards give 1 token per 50 reputation
            if (source.Contains("quest"))
                tokenReward = repGain / 50;
            // Boss kills give 1 token per 25 reputation
            else if (source.Contains("boss"))
                tokenReward = repGain / 25;
            // Donations give 1 token per 100 reputation
            else if (source.Contains("donation"))
                tokenReward = repGain / 100;
            // Other sources give 1 token per 75 reputation
            else
                tokenReward = repGain / 75;

            if (tokenReward > 0)
                GiveTokens(pm, faction, tokenReward);
        }
    }

    #endregion
}
