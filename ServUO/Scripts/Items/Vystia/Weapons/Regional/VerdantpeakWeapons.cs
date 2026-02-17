using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    #region Nature's Blade

    /// <summary>
    /// Nature's Blade - Verdantpeak regional sword
    /// </summary>
    public class NaturesBlade : Longsword
    {
        [Constructable]
        public NaturesBlade() : base()
        {
            Name = "Nature's Blade";
            Hue = 2010; // Forest green
            Weight = 6.0;

            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 10;
            Attributes.RegenHits = 2;

            WeaponAttributes.SelfRepair = 3;
            WeaponAttributes.ResistPoisonBonus = 10;

            MinDamage = 14;
            MaxDamage = 18;

            AosElementDamages.Poison = 40;
            AosElementDamages.Physical = 60;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Natureforged Ingots");
        }

        public NaturesBlade(Serial serial) : base(serial) { }

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

    #region Thorn Edge

    /// <summary>
    /// Thorn Edge - Verdantpeak regional sword
    /// </summary>
    public class ThornEdge : Broadsword
    {
        [Constructable]
        public ThornEdge() : base()
        {
            Name = "Thorn Edge";
            Hue = 2010;
            Weight = 6.0;

            Attributes.WeaponDamage = 30;
            Attributes.DefendChance = 10;
            Attributes.RegenHits = 2;

            WeaponAttributes.SelfRepair = 5;

            MinDamage = 15;
            MaxDamage = 19;

            AosElementDamages.Poison = 50;
            AosElementDamages.Physical = 50;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 10% chance for thorns to deal damage back
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.10)
            {
                // Apply thorn damage
                AOS.Damage(defender, attacker, Utility.RandomMinMax(3, 8), 0, 0, 0, 100, 0);
                defender.SendMessage(0x22, "Thorns pierce your flesh!");
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("10% chance for thorn damage");
        }

        public ThornEdge(Serial serial) : base(serial) { }

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

    #region Seedling Knife

    /// <summary>
    /// Seedling Knife - Verdantpeak regional dagger
    /// Fast weapon with regeneration
    /// </summary>
    public class SeedlingKnife : Dagger
    {
        [Constructable]
        public SeedlingKnife() : base()
        {
            Name = "Seedling Knife";
            Hue = 2010;
            Weight = 1.0;

            Attributes.WeaponDamage = 20;
            Attributes.WeaponSpeed = 20;
            Attributes.RegenHits = 3;
            Attributes.RegenStam = 2;

            WeaponAttributes.SelfRepair = 5;

            MinDamage = 10;
            MaxDamage = 13;

            AosElementDamages.Poison = 30;
            AosElementDamages.Physical = 70;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("The forest heals its allies");
        }

        public SeedlingKnife(Serial serial) : base(serial) { }

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

    #region Ironbark Maul

    /// <summary>
    /// Ironbark Maul - Verdantpeak regional hammer
    /// Heavy living wood weapon
    /// </summary>
    public class IronbarkMaul : WarHammer
    {
        [Constructable]
        public IronbarkMaul() : base()
        {
            Name = "Ironbark Maul";
            Hue = 2010;
            Weight = 10.0;

            Attributes.WeaponDamage = 35;
            Attributes.BonusStr = 5;
            Attributes.RegenHits = 3;

            WeaponAttributes.SelfRepair = 5;
            WeaponAttributes.HitPhysicalArea = 25;

            MinDamage = 18;
            MaxDamage = 24;

            AosElementDamages.Physical = 100;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Verdantpeak");
            list.Add("Crafted from Living Wood");
        }

        public IronbarkMaul(Serial serial) : base(serial) { }

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
