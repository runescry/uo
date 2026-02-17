using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // VYSTIA CLASS-SPECIFIC CLOTHING
    // ============================================================================

    #region Necromancer - Deathshroud Robe
    public class VystiaDeathshroudRobe : Robe
    {
        [Constructable]
        public VystiaDeathshroudRobe()
        {
            Name = "Deathshroud Robe";
            Hue = 1109; // Necromancer hue
            Attributes.BonusInt = 2;
            Attributes.RegenMana = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Necromancer Starting Robe");
        }

        public VystiaDeathshroudRobe(Serial serial) : base(serial) { }

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

    #region Necromancer - Deathbone Helm
    public class VystiaDeathboneHelm : BoneHelm
    {
        [Constructable]
        public VystiaDeathboneHelm()
        {
            Name = "Deathbone Helm";
            Hue = 1150; // Less harsh than white
            Attributes.BonusInt = 1;
            Attributes.RegenMana = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Necromancer Starting Helm");
        }

        public VystiaDeathboneHelm(Serial serial) : base(serial) { }

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

    #region Warlock - Shadowcloth Robe
    public class VystiaShadowclothRobe : Robe
    {
        [Constructable]
        public VystiaShadowclothRobe()
        {
            Name = "Hooded Shadowcloth Robe";
            ItemID = 0x2683; // Hooded robe
            Hue = 0x455; // Dark purple
            Attributes.BonusInt = 2;
            Attributes.RegenMana = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Warlock Starting Robe");
        }

        public VystiaShadowclothRobe(Serial serial) : base(serial) { }

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
