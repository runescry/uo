using System;
using Server;
using Server.Mobiles;

namespace Server.Items.Vystia
{
    // ============================================================================
    // LEGENDARY ARTIFACT WEAPONS
    // ============================================================================

    public class PhoenixAscension : Katana
    {
        [Constructable]
        public PhoenixAscension()
        {
            Name = "Phoenix Ascension";
            Hue = 1358;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
            Attributes.Luck = 100;

                        WeaponAttributes.HitFireball = 40;

            // Damage range
            MinDamage = 20;
            MaxDamage = 30;

            // Element damage
                        AosElementDamages.Fire = 100;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged in the heart of a dying phoenix");
            list.Add("Dropped by Volcano Wyrm");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public PhoenixAscension(Serial serial) : base(serial) { }

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

    public class TheCogmaster : WarHammer
    {
        [Constructable]
        public TheCogmaster()
        {
            Name = "The Cogmaster";
            Hue = 2401;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
            Attributes.Luck = 100;

                        WeaponAttributes.HitLightning = 30;
            Attributes.WeaponSpeed = 20;
            // Self-Repair handled by ServUO durability system

            // Damage range
            MinDamage = 22;
            MaxDamage = 28;

            // Element damage
                        AosElementDamages.Energy = 50;
            AosElementDamages.Physical = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Master artifact of the Technoguild");
            list.Add("Dropped by Forge Master");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TheCogmaster(Serial serial) : base(serial) { }

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

    public class PrismaticEdge : Longsword
    {
        [Constructable]
        public PrismaticEdge()
        {
            Name = "Prismatic Edge";
            Hue = 1154;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
            Attributes.Luck = 100;

            

            // Damage range
            MinDamage = 18;
            MaxDamage = 25;

            // Element damage
                        AosElementDamages.Physical = 20;
            AosElementDamages.Fire = 20;
            AosElementDamages.Cold = 20;
            AosElementDamages.Poison = 20;
            AosElementDamages.Energy = 20;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Refracted from pure crystal light");
            list.Add("Dropped by Crystal Dragon");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public PrismaticEdge(Serial serial) : base(serial) { }

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

    public class Voidcaller : QuarterStaff
    {
        [Constructable]
        public Voidcaller()
        {
            Name = "Voidcaller";
            Hue = 1109;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
            Attributes.Luck = 100;

                        Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
            WeaponAttributes.MageWeapon = 10;

            // Damage range
            MinDamage = 15;
            MaxDamage = 22;

            // Element damage
                        AosElementDamages.Cold = 50;
            AosElementDamages.Energy = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Staff channeling the power of the void");
            list.Add("Dropped by Shadow Lich");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public Voidcaller(Serial serial) : base(serial) { }

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

    public class TheEternalWinter : Halberd
    {
        [Constructable]
        public TheEternalWinter()
        {
            Name = "The Eternal Winter";
            Hue = 1152;

            // Legendary stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
            Attributes.Luck = 100;

            WeaponAttributes.HitColdArea = 50;
            Attributes.DefendChance = 10;

            // Damage range
            MinDamage = 25;
            MaxDamage = 35;

            // Element damage
            AosElementDamages.Cold = 100;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Forged from the eternal ice of Frosthold");
            list.Add("Dropped by Frost Father");
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TheEternalWinter(Serial serial) : base(serial) { }

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
