using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
    /// <summary>
    /// Great Machinist's Construct - Divine Blessing (Ironclad)
    /// Associated with Cogsmith religion
    /// </summary>
    [CorpseName("the remains of the Construct")]
    public class GreatMachinistsConstruct : BaseAncientBeing
    {
        public override AncientBeingRole[] Roles => new[] { AncientBeingRole.DivineBlessing };
        public override Server.Custom.VystiaClasses.Religion.VystiaReligion AssociatedReligion => 
            Server.Custom.VystiaClasses.Religion.VystiaReligion.CogsmithCreed;

        [Constructable]
        public GreatMachinistsConstruct() : base(AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2.0)
        {
            Name = "Great Machinist's Construct";
            Title = "the Divine";
            Body = 400;
            Hue = 1109; // Metallic
            BaseSoundID = 0;

            SetStr(500, 600);
            SetDex(100, 150);
            SetInt(200, 250);

            SetHits(1000, 1200);

            Fame = 50000;
            Karma = 50000;

            VirtualArmor = 80;
        }

        public GreatMachinistsConstruct(Serial serial) : base(serial) { }

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

