using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// Base class for all Frosthold regional weapons.
    /// These weapons deal cold damage and have frost-themed effects.
    /// </summary>
    public abstract class BaseFrostholdWeapon : BaseSword
    {
        public BaseFrostholdWeapon(int itemID) : base(itemID)
        {
            Hue = 1152; // Ice blue

            // All Frosthold weapons deal cold damage
            AosElementDamages.Cold = 50;
            AosElementDamages.Physical = 50;

            WeaponAttributes.ResistColdBonus = 5;
        }

        public BaseFrostholdWeapon(Serial serial) : base(serial) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 5% chance to slow target
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.05)
            {
                defender.SendMessage(0x480, "The cold numbs your limbs!");
                // Apply slight stamina drain as slow effect
                defender.Stam -= Utility.RandomMinMax(5, 10);
            }
        }

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

    #region Icicle Blade

    /// <summary>
    /// Icicle Blade - Frosthold regional sword
    /// Crafted from Frostforged Ingots
    /// </summary>
    public class IcicleBlade : Longsword
    {
        [Constructable]
        public IcicleBlade() : base()
        {
            Name = "Icicle Blade";
            Hue = 1152; // Ice blue
            Weight = 6.0;

            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 10;

            WeaponAttributes.HitColdArea = 20;
            WeaponAttributes.ResistColdBonus = 10;

            MinDamage = 14;
            MaxDamage = 18;

            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frostforged Ingots");
        }

        public IcicleBlade(Serial serial) : base(serial) { }

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

    #region Winter's Edge

    /// <summary>
    /// Winter's Edge - Frosthold regional sword
    /// </summary>
    public class WintersEdge : Broadsword
    {
        [Constructable]
        public WintersEdge() : base()
        {
            Name = "Winter's Edge";
            Hue = 1152;
            Weight = 6.0;

            Attributes.WeaponDamage = 30;
            Attributes.DefendChance = 10;

            WeaponAttributes.HitColdArea = 25;
            WeaponAttributes.ResistColdBonus = 10;

            MinDamage = 15;
            MaxDamage = 19;

            AosElementDamages.Cold = 70;
            AosElementDamages.Physical = 30;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frostforged Ingots");
        }

        public WintersEdge(Serial serial) : base(serial) { }

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

    #region Frostbite

    /// <summary>
    /// Frostbite - Frosthold regional dagger
    /// Fast weapon with freeze chance
    /// </summary>
    public class Frostbite : Dagger
    {
        [Constructable]
        public Frostbite() : base()
        {
            Name = "Frostbite";
            Hue = 1152;
            Weight = 1.0;

            Attributes.WeaponDamage = 20;
            Attributes.WeaponSpeed = 20;
            Attributes.AttackChance = 10;

            WeaponAttributes.ResistColdBonus = 10;

            MinDamage = 10;
            MaxDamage = 13;

            AosElementDamages.Cold = 80;
            AosElementDamages.Physical = 20;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 8% chance to briefly freeze
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.08)
            {
                defender.SendMessage(0x480, "Your blood freezes!");
                defender.Freeze(TimeSpan.FromSeconds(1));
                defender.FixedParticles(0x376A, 9, 32, 5030, 1152, 0, EffectLayer.Waist);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("8% chance to freeze target");
        }

        public Frostbite(Serial serial) : base(serial) { }

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

    #region Glacier Shard

    /// <summary>
    /// Glacier Shard - Frosthold regional axe
    /// Heavy hitting cold damage weapon
    /// </summary>
    public class GlacierShard : BattleAxe
    {
        [Constructable]
        public GlacierShard() : base()
        {
            Name = "Glacier Shard";
            Hue = 1152;
            Weight = 8.0;

            Attributes.WeaponDamage = 35;
            Attributes.BonusStr = 5;

            WeaponAttributes.HitColdArea = 35;
            WeaponAttributes.ResistColdBonus = 15;

            MinDamage = 18;
            MaxDamage = 24;

            AosElementDamages.Cold = 75;
            AosElementDamages.Physical = 25;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Frosthold");
            list.Add("Crafted from Frostforged Ingots");
        }

        public GlacierShard(Serial serial) : base(serial) { }

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
