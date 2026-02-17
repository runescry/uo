using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
    /// <summary>
    /// Frost Father's Avatar - Divine Blessing (Frosthold)
    /// Associated with Frosthelm religion
    /// </summary>
    [CorpseName("the remains of the Avatar")]
    public class FrostFathersAvatar : BaseAncientBeing
    {
        public override AncientBeingRole[] Roles => new[] { AncientBeingRole.DivineBlessing };
        public override Server.Custom.VystiaClasses.Religion.VystiaReligion AssociatedReligion => 
            Server.Custom.VystiaClasses.Religion.VystiaReligion.FrosthelmFaith;

        [Constructable]
        public FrostFathersAvatar() : base(AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2.0)
        {
            Name = "Frost Father's Avatar";
            Title = "the Divine";
            Body = 400;
            Hue = 1152; // Ice blue
            BaseSoundID = 0;

            SetStr(500, 600);
            SetDex(100, 150);
            SetInt(200, 250);

            SetHits(1000, 1200);

            SetResistance(ResistanceType.Cold, 100, 100);

            Fame = 50000;
            Karma = 50000;

            VirtualArmor = 80;
        }

        public FrostFathersAvatar(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}

