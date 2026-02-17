using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Prompts;

namespace Server.Custom.VystiaClasses.Religion
{
    /// <summary>
    /// Shrine functions available to players
    /// </summary>
    public enum ShrineFunction
    {
        Pray,
        Tithe,
        Pilgrimage,
        RefreshBlessings,
        RechargePowers,
        BlessItem,
        Resurrect
    }

    /// <summary>
    /// Shrine item for blessing items
    /// Players can use the shrine to bless weapons, armor, and jewelry
    /// </summary>
    public class VystiaShrine : Item
    {
        private VystiaReligion m_Religion;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaReligion Religion
        {
            get { return m_Religion; }
            set
            {
                m_Religion = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public VystiaShrine(VystiaReligion religion)
            : base(0x1EA7) // Default shrine graphic
        {
            m_Religion = religion;
            Name = GetShrineName(religion);
            Hue = ReligionData.GetInfo(religion)?.Hue ?? 0;
            Movable = false;
            Weight = 0;
        }

        public VystiaShrine(Serial serial)
            : base(serial)
        {
        }

        private string GetShrineName(VystiaReligion religion)
        {
            switch (religion)
            {
                case VystiaReligion.FrosthelmFaith:
                case VystiaReligion.SuryasSandscript:
                case VystiaReligion.LunarasCovenant:
                case VystiaReligion.CelestisArcanum:
                case VystiaReligion.OceanasCovenant:
                case VystiaReligion.CogsmithCreed:
                    return string.Concat("Shrine of ", ReligionData.GetLoreName(religion));
                default:
                    return "Shrine";
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !(from is PlayerMobile))
                return;

            PlayerMobile pm = from as PlayerMobile;

            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage(0x22, "You are too far away to use the shrine.");
                return;
            }

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != m_Religion)
            {
                pm.SendMessage(0x22, "You must follow {0} to use this shrine.", ReligionData.GetDisplayName(m_Religion));
                return;
            }

            // Show shrine menu with all available functions
            ShowShrineMenu(pm, pietyData);
        }

        private void ShowShrineMenu(PlayerMobile pm, PietyData pietyData)
        {
            // Create menu based on piety tier
            var tier = ReligionData.GetTier(pietyData.Piety);
            bool canPray = pietyData.Piety >= 1; // Initiate
            bool canTithe = pietyData.Piety >= 1; // Initiate
            bool canPilgrimage = pietyData.CanPilgrimage(); // Weekly pilgrimage
            bool canRefreshBlessings = pietyData.Piety >= 50; // Adherent
            bool canRechargePowers = pietyData.Piety >= 200; // Devoted
            bool canBlessItems = pietyData.Piety >= 500; // Zealot
            bool canResurrect = pietyData.Piety >= 900; // Champion

            pm.SendGump(new ShrineMenuGump(pm, this, canPray, canTithe, canPilgrimage, canRefreshBlessings, canRechargePowers, canBlessItems, canResurrect));
        }

        private class BlessItemTarget : Target
        {
            private VystiaShrine m_Shrine;

            public BlessItemTarget(VystiaShrine shrine)
                : base(2, false, TargetFlags.None)
            {
                m_Shrine = shrine;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || !(from is PlayerMobile))
                    return;

                PlayerMobile pm = from as PlayerMobile;

                if (targeted == null || !(targeted is Item))
                {
                    pm.SendMessage(0x22, "You can only bless items.");
                    return;
                }

                Item item = targeted as Item;

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

                // Check if already blessed by this religion
                if (item is IBlessedItem blessedItem && blessedItem.BlessingReligion == m_Shrine.Religion)
                {
                    pm.SendMessage(0x35, "This item is already blessed by {0}.", m_Shrine.GetShrineName(m_Shrine.Religion));
                    return;
                }

                // Attempt to bless
                bool success = VystiaBlessedItemSystem.BlessItem(pm, item, m_Shrine.Religion);

                if (success)
                {
                    pm.SendMessage(0x35, "You have successfully blessed the item!");
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (from is PlayerMobile pm)
                {
                    pm.SendMessage(0x22, "Blessing cancelled.");
                }
            }
        }

        /// <summary>
        /// Handle shrine function selection
        /// </summary>
        public void HandleShrineFunction(PlayerMobile pm, ShrineFunction function)
        {
            if (pm == null)
                return;

            var pietyData = VystiaPiety.GetPiety(pm);
            if (pietyData == null || pietyData.Religion != m_Religion)
            {
                pm.SendMessage(0x22, "You must follow {0} to use this shrine.", GetShrineName(m_Religion));
                return;
            }

            switch (function)
            {
                case ShrineFunction.Pray:
                    PietyActions.Pray(pm);
                    break;

                case ShrineFunction.Tithe:
                    pm.SendMessage(0x35, "How much gold would you like to tithe? (1 piety per 100g, max 30 piety per day)");
                    pm.Prompt = new TithePrompt(pm);
                    break;

                case ShrineFunction.Pilgrimage:
                    PietyActions.PerformPilgrimage(pm, m_Religion);
                    break;

                case ShrineFunction.RefreshBlessings:
                    if (pietyData.Piety < 50)
                    {
                        pm.SendMessage(0x22, "You need at least 50 piety (Adherent tier) to refresh blessings.");
                        return;
                    }
                    VystiaBlessedItemSystem.RefreshBlessings(pm);
                    break;

                case ShrineFunction.RechargePowers:
                    if (pietyData.Piety < 200)
                    {
                        pm.SendMessage(0x22, "You need at least 200 piety (Devoted tier) to recharge devotion powers.");
                        return;
                    }
                    VystiaDevotionPowers.RechargeAllPowers(pm);
                    break;

                case ShrineFunction.BlessItem:
                    if (pietyData.Piety < 500)
                    {
                        pm.SendMessage(0x22, "You need at least 500 piety (Zealot tier) to bless items.");
                        return;
                    }
                    pm.SendMessage(0x35, "Target an item to bless it with the power of {0}.", GetShrineName(m_Religion));
                    pm.Target = new BlessItemTarget(this);
                    break;

                case ShrineFunction.Resurrect:
                    if (pietyData.Piety < 900)
                    {
                        pm.SendMessage(0x22, "You need at least 900 piety (Champion tier) to use free resurrection.");
                        return;
                    }
                    if (!pm.Alive)
                    {
                        ResurrectAtShrine(pm);
                    }
                    else
                    {
                        pm.SendMessage(0x35, "You are already alive. Free resurrection is available when you die.");
                    }
                    break;
            }
        }

        private void ResurrectAtShrine(PlayerMobile pm)
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
            pm.SendMessage(0x35, "The power of {0} has restored you to life!", GetShrineName(m_Religion));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Religion\t{0}", GetShrineName(m_Religion));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write((int)m_Religion);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Religion = (VystiaReligion)reader.ReadInt();
        }

        #region Shrine Menu Gump

        private class ShrineMenuGump : Gump
        {
            private PlayerMobile m_Player;
            private VystiaShrine m_Shrine;
            private bool m_CanPray, m_CanTithe, m_CanPilgrimage, m_CanRefresh, m_CanRecharge, m_CanBless, m_CanResurrect;

            public ShrineMenuGump(PlayerMobile player, VystiaShrine shrine, bool canPray, bool canTithe, bool canPilgrimage, bool canRefresh, bool canRecharge, bool canBless, bool canResurrect)
                : base(50, 50)
            {
                m_Player = player;
                m_Shrine = shrine;
                m_CanPray = canPray;
                m_CanTithe = canTithe;
                m_CanPilgrimage = canPilgrimage;
                m_CanRefresh = canRefresh;
                m_CanRecharge = canRecharge;
                m_CanBless = canBless;
                m_CanResurrect = canResurrect;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);
                AddBackground(0, 0, 300, 400, 9200);
                AddLabel(20, 20, 0, shrine.GetShrineName(shrine.Religion));

                int y = 60;
                if (m_CanPray)
                {
                    AddButton(20, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
                    AddLabel(60, y, 0, "Pray (+10 piety, daily)");
                    y += 30;
                }

                if (m_CanTithe)
                {
                    AddButton(20, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
                    AddLabel(60, y, 0, "Tithe (1 piety per 100g)");
                    y += 30;
                }

                if (m_CanPilgrimage)
                {
                    AddButton(20, y, 4005, 4007, 7, GumpButtonType.Reply, 0);
                    AddLabel(60, y, 0, "Pilgrimage (+75 piety, weekly)");
                    y += 30;
                }

                if (m_CanRefresh)
                {
                    TimeSpan cooldown = VystiaBlessedItemSystem.GetBlessingRefreshCooldown(player);
                    string refreshText = cooldown > TimeSpan.Zero 
                        ? string.Format("Refresh Blessings ({0:F1}h cooldown)", cooldown.TotalHours)
                        : "Refresh Blessings (Ready)";
                    AddButton(20, y, 4005, 4007, 3, GumpButtonType.Reply, 0);
                    AddLabel(60, y, cooldown > TimeSpan.Zero ? 0x22 : 0, refreshText);
                    y += 30;
                }

                if (m_CanRecharge)
                {
                    AddButton(20, y, 4005, 4007, 4, GumpButtonType.Reply, 0);
                    AddLabel(60, y, 0, "Recharge Devotion Powers");
                    y += 30;
                }

                if (m_CanBless)
                {
                    AddButton(20, y, 4005, 4007, 5, GumpButtonType.Reply, 0);
                    AddLabel(60, y, 0, "Bless Item");
                    y += 30;
                }

                if (m_CanResurrect)
                {
                    AddButton(20, y, 4005, 4007, 6, GumpButtonType.Reply, 0);
                    AddLabel(60, y, 0, "Free Resurrection");
                    y += 30;
                }
            }

            public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
            {
                if (m_Player == null || m_Shrine == null)
                    return;

                switch (info.ButtonID)
                {
                    case 1:
                        m_Shrine.HandleShrineFunction(m_Player, ShrineFunction.Pray);
                        break;
                    case 2:
                        m_Shrine.HandleShrineFunction(m_Player, ShrineFunction.Tithe);
                        break;
                    case 3:
                        m_Shrine.HandleShrineFunction(m_Player, ShrineFunction.RefreshBlessings);
                        break;
                    case 4:
                        m_Shrine.HandleShrineFunction(m_Player, ShrineFunction.RechargePowers);
                        break;
                    case 5:
                        m_Shrine.HandleShrineFunction(m_Player, ShrineFunction.BlessItem);
                        break;
                    case 6:
                        m_Shrine.HandleShrineFunction(m_Player, ShrineFunction.Resurrect);
                        break;
                    case 7:
                        m_Shrine.HandleShrineFunction(m_Player, ShrineFunction.Pilgrimage);
                        break;
                }
            }
        }

        private class TithePrompt : Prompt
        {
            private PlayerMobile m_Player;

            public TithePrompt(PlayerMobile player)
            {
                m_Player = player;
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

                PietyActions.Tithe(pm, goldAmount);
            }
        }

        #endregion
    }
}

