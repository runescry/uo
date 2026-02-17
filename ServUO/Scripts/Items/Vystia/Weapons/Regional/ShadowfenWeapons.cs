using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    #region Shadow Fang

    /// <summary>
    /// Shadow Fang - Shadowfen regional sword
    /// </summary>
    public class ShadowFang : Longsword
    {
        [Constructable]
        public ShadowFang() : base()
        {
            Name = "Shadow Fang";
            Hue = 1109; // Shadow black
            Weight = 6.0;

            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 10;

            WeaponAttributes.HitLeechHits = 20;
            WeaponAttributes.ResistPoisonBonus = 10;

            MinDamage = 14;
            MaxDamage = 18;

            AosElementDamages.Poison = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Shadowforged Ingots");
        }

        public ShadowFang(Serial serial) : base(serial) { }

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

    #region Bog Cleaver

    /// <summary>
    /// Bog Cleaver - Shadowfen regional sword
    /// </summary>
    public class BogCleaver : Broadsword
    {
        [Constructable]
        public BogCleaver() : base()
        {
            Name = "Bog Cleaver";
            Hue = 2212; // Bog green
            Weight = 6.0;

            Attributes.WeaponDamage = 30;
            Attributes.DefendChance = 10;

            WeaponAttributes.HitPoisonArea = 25;
            WeaponAttributes.ResistPoisonBonus = 10;

            MinDamage = 15;
            MaxDamage = 19;

            AosElementDamages.Poison = 70;
            AosElementDamages.Physical = 30;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Shadowforged Ingots");
        }

        public BogCleaver(Serial serial) : base(serial) { }

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

    #region Venom Sting

    /// <summary>
    /// Venom Sting - Shadowfen regional dagger
    /// Fast weapon with poison effect
    /// </summary>
    public class VenomSting : Dagger
    {
        [Constructable]
        public VenomSting() : base()
        {
            Name = "Venom Sting";
            Hue = 1109;
            Weight = 1.0;

            Attributes.WeaponDamage = 20;
            Attributes.WeaponSpeed = 20;
            Attributes.AttackChance = 10;

            WeaponAttributes.ResistPoisonBonus = 10;

            MinDamage = 10;
            MaxDamage = 13;

            AosElementDamages.Poison = 80;
            AosElementDamages.Physical = 20;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 15% chance to poison
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.15)
            {
                defender.ApplyPoison(attacker, Poison.Regular);
                defender.SendMessage(0x22, "Venom courses through your veins!");
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("15% chance to poison target");
        }

        public VenomSting(Serial serial) : base(serial) { }

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

    #region Mire Crusher

    /// <summary>
    /// Mire Crusher - Shadowfen regional mace
    /// Heavy hitting poison damage weapon
    /// </summary>
    public class MireCrusher : WarMace
    {
        [Constructable]
        public MireCrusher() : base()
        {
            Name = "Mire Crusher";
            Hue = 2212;
            Weight = 9.0;

            Attributes.WeaponDamage = 35;
            Attributes.BonusStr = 5;

            WeaponAttributes.HitPoisonArea = 35;
            WeaponAttributes.ResistPoisonBonus = 15;

            MinDamage = 18;
            MaxDamage = 24;

            AosElementDamages.Poison = 75;
            AosElementDamages.Physical = 25;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Shadowfen");
            list.Add("Crafted from Shadowforged Ingots");
        }

        public MireCrusher(Serial serial) : base(serial) { }

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
