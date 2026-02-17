using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // LIVING RING
    // Nature-blessed ring mail
    // ============================================================================

    public class LivingRingTunic : RingmailChest
    {
        [Constructable]
        public LivingRingTunic()
        {
            Name = "Living Ring Tunic";
            Hue = 2010;

            // Nature-blessed ring mail
            Attributes.RegenHits = 2;
            Attributes.Luck = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("VERDANTPEAK Ring Armor");
        }

        public LivingRingTunic(Serial serial) : base(serial) { }

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

    public class LivingRingSleeves : RingmailArms
    {
        [Constructable]
        public LivingRingSleeves()
        {
            Name = "Living Ring Sleeves";
            Hue = 2010;

            // Nature-blessed ring mail
            Attributes.RegenHits = 2;
            Attributes.Luck = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("VERDANTPEAK Ring Armor");
        }

        public LivingRingSleeves(Serial serial) : base(serial) { }

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

    public class LivingRingGloves : RingmailGloves
    {
        [Constructable]
        public LivingRingGloves()
        {
            Name = "Living Ring Gloves";
            Hue = 2010;

            // Nature-blessed ring mail
            Attributes.RegenHits = 2;
            Attributes.Luck = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("VERDANTPEAK Ring Armor");
        }

        public LivingRingGloves(Serial serial) : base(serial) { }

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

    public class LivingRingLegs : RingmailLegs
    {
        [Constructable]
        public LivingRingLegs()
        {
            Name = "Living Ring Legs";
            Hue = 2010;

            // Nature-blessed ring mail
            Attributes.RegenHits = 2;
            Attributes.Luck = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("VERDANTPEAK Ring Armor");
        }

        public LivingRingLegs(Serial serial) : base(serial) { }

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

    // ============================================================================
    // STEAM RING
    // Clockwork-enhanced ring mail
    // ============================================================================

    public class SteamRingTunic : RingmailChest
    {
        [Constructable]
        public SteamRingTunic()
        {
            Name = "Steam Ring Tunic";
            Hue = 2401;

            // Clockwork-enhanced ring mail
            Attributes.BonusStr = 3;
            Attributes.WeaponSpeed = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Ring Armor");
        }

        public SteamRingTunic(Serial serial) : base(serial) { }

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

    public class SteamRingSleeves : RingmailArms
    {
        [Constructable]
        public SteamRingSleeves()
        {
            Name = "Steam Ring Sleeves";
            Hue = 2401;

            // Clockwork-enhanced ring mail
            Attributes.BonusStr = 3;
            Attributes.WeaponSpeed = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Ring Armor");
        }

        public SteamRingSleeves(Serial serial) : base(serial) { }

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

    public class SteamRingGloves : RingmailGloves
    {
        [Constructable]
        public SteamRingGloves()
        {
            Name = "Steam Ring Gloves";
            Hue = 2401;

            // Clockwork-enhanced ring mail
            Attributes.BonusStr = 3;
            Attributes.WeaponSpeed = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Ring Armor");
        }

        public SteamRingGloves(Serial serial) : base(serial) { }

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

    public class SteamRingLegs : RingmailLegs
    {
        [Constructable]
        public SteamRingLegs()
        {
            Name = "Steam Ring Legs";
            Hue = 2401;

            // Clockwork-enhanced ring mail
            Attributes.BonusStr = 3;
            Attributes.WeaponSpeed = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("IRONCLAD Ring Armor");
        }

        public SteamRingLegs(Serial serial) : base(serial) { }

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

}
