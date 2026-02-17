using System;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items.VystiaClassItems
{
    /// <summary>
    /// Shapeshifting Totem - Druid ability item
    /// Allows transformation into animal forms
    /// </summary>
    public class ShapeshiftTotem : Item
    {
        [Constructable]
        public ShapeshiftTotem() : base(0x1F1C) // Totem item ID
        {
            Name = "Shapeshifting Totem";
            Hue = 2010; // Forest green
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1070722, "Transform into animal forms");
            list.Add("Forms: Bear, Wolf, Hawk, Human");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            from.SendGump(new ShapeshiftGump(from, this));
        }

        public ShapeshiftTotem(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    /// <summary>
    /// Gump for selecting shapeshift form
    /// </summary>
    public class ShapeshiftGump : Gump
    {
        private Mobile m_From;
        private ShapeshiftTotem m_Totem;

        public ShapeshiftGump(Mobile from, ShapeshiftTotem totem) : base(50, 50)
        {
            m_From = from;
            m_Totem = totem;

            AddPage(0);
            AddBackground(0, 0, 400, 350, 9200);
            AddLabel(140, 20, 2010, "Shapeshifting Totem");

            // Bear form
            AddButton(50, 60, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(90, 60, 0, "Bear Form");
            AddHtml(90, 80, 280, 40, "<BASEFONT COLOR=#FFFFFF>+20 STR, +10 DEX<br>Enhanced melee combat</BASEFONT>", false, false);

            // Wolf form
            AddButton(50, 130, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(90, 130, 0, "Wolf Form");
            AddHtml(90, 150, 280, 40, "<BASEFONT COLOR=#FFFFFF>+15 DEX, +10 STR<br>Enhanced tracking & stealth</BASEFONT>", false, false);

            // Hawk form
            AddButton(50, 200, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(90, 200, 0, "Hawk Form");
            AddHtml(90, 220, 280, 40, "<BASEFONT COLOR=#FFFFFF>+20 DEX, -10 STR<br>Enhanced vision & mobility</BASEFONT>", false, false);

            // Human form
            AddButton(50, 270, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(90, 270, 0, "Human Form");
            AddHtml(90, 290, 280, 40, "<BASEFONT COLOR=#FFFFFF>Return to normal form</BASEFONT>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_From.Deleted || m_Totem == null || m_Totem.Deleted)
                return;

            if (!m_Totem.IsChildOf(m_From.Backpack))
            {
                m_From.SendLocalizedMessage(1042001);
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // Bear
                    TransformTo(m_From, 211, "Bear", 20, 10, 0); // Grizzly bear body
                    break;
                case 2: // Wolf
                    TransformTo(m_From, 225, "Wolf", 10, 15, 0); // Timber wolf body
                    break;
                case 3: // Hawk
                    TransformTo(m_From, 5, "Hawk", -10, 20, 0); // Bird body
                    break;
                case 4: // Human
                    RestoreHuman(m_From);
                    break;
            }
        }

        private void TransformTo(Mobile m, int bodyValue, string formName, int strBonus, int dexBonus, int intBonus)
        {
            // Remove existing shapeshift mods
            RemoveShapeshiftMods(m);

            // Transform body
            m.BodyMod = bodyValue;
            m.HueMod = 2010; // Forest green tint

            // Apply stat bonuses
            if (strBonus != 0)
                m.AddStatMod(new StatMod(StatType.Str, "Shapeshift_Str", strBonus, TimeSpan.Zero));
            if (dexBonus != 0)
                m.AddStatMod(new StatMod(StatType.Dex, "Shapeshift_Dex", dexBonus, TimeSpan.Zero));
            if (intBonus != 0)
                m.AddStatMod(new StatMod(StatType.Int, "Shapeshift_Int", intBonus, TimeSpan.Zero));

            // Visual effect
            m.FixedParticles(0x373A, 10, 15, 5018, 2010, 0, EffectLayer.Waist);
            m.PlaySound(0x5C3);

            m.SendMessage(0x3F, $"You shift into {formName} form!");
        }

        private void RestoreHuman(Mobile m)
        {
            // Remove shapeshift mods
            RemoveShapeshiftMods(m);

            // Restore body
            m.BodyMod = 0;
            m.HueMod = -1;

            // Visual effect
            m.FixedParticles(0x373A, 10, 15, 5018, 2010, 0, EffectLayer.Waist);
            m.PlaySound(0x5C3);

            m.SendMessage(0x3F, "You return to your human form.");
        }

        private void RemoveShapeshiftMods(Mobile m)
        {
            m.RemoveStatMod("Shapeshift_Str");
            m.RemoveStatMod("Shapeshift_Dex");
            m.RemoveStatMod("Shapeshift_Int");
        }
    }
}
