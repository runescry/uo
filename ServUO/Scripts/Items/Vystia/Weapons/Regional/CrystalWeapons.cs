using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    #region Crystal Blade

    /// <summary>
    /// Crystal Blade - Crystal Barrens regional sword
    /// </summary>
    public class CrystalBlade : Longsword
    {
        [Constructable]
        public CrystalBlade() : base()
        {
            Name = "Crystal Blade";
            Hue = 1154; // Crystal blue
            Weight = 6.0;

            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 10;
            Attributes.SpellChanneling = 1;

            WeaponAttributes.HitMagicArrow = 20;
            WeaponAttributes.ResistEnergyBonus = 10;

            MinDamage = 14;
            MaxDamage = 18;

            AosElementDamages.Energy = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystalline Ingots");
        }

        public CrystalBlade(Serial serial) : base(serial) { }

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

    #region Prism Shard

    /// <summary>
    /// Prism Shard - Crystal Barrens regional sword
    /// </summary>
    public class PrismShard : Broadsword
    {
        [Constructable]
        public PrismShard() : base()
        {
            Name = "Prism Shard";
            Hue = 1154;
            Weight = 6.0;

            Attributes.WeaponDamage = 30;
            Attributes.BonusInt = 5;
            Attributes.SpellChanneling = 1;

            WeaponAttributes.HitLightning = 25;
            WeaponAttributes.ResistEnergyBonus = 10;

            MinDamage = 15;
            MaxDamage = 19;

            AosElementDamages.Energy = 70;
            AosElementDamages.Physical = 30;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystalline Ingots");
        }

        public PrismShard(Serial serial) : base(serial) { }

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

    #region Ley Cutter

    /// <summary>
    /// Ley Cutter - Crystal Barrens regional dagger
    /// Mage weapon with mana restoration
    /// </summary>
    public class LeyCutter : Dagger
    {
        [Constructable]
        public LeyCutter() : base()
        {
            Name = "Ley Cutter";
            Hue = 1156; // Ley blue
            Weight = 1.0;

            Attributes.WeaponDamage = 15;
            Attributes.WeaponSpeed = 15;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 1;
            Attributes.LowerManaCost = 5;

            WeaponAttributes.HitLeechMana = 25;
            WeaponAttributes.MageWeapon = 15;

            MinDamage = 10;
            MaxDamage = 13;

            AosElementDamages.Energy = 80;
            AosElementDamages.Physical = 20;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 10% chance to restore mana
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.10)
            {
                int mana = Utility.RandomMinMax(5, 15);
                attacker.Mana += mana;
                attacker.SendMessage(0x35, "Ley energy flows into you!");
                attacker.FixedParticles(0x376A, 9, 32, 5030, 1156, 0, EffectLayer.Waist);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("10% chance to restore mana");
        }

        public LeyCutter(Serial serial) : base(serial) { }

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

    #region Resonance Maul

    /// <summary>
    /// Resonance Maul - Crystal Barrens regional hammer
    /// Heavy hitting energy damage weapon
    /// </summary>
    public class ResonanceMaul : WarHammer
    {
        [Constructable]
        public ResonanceMaul() : base()
        {
            Name = "Resonance Maul";
            Hue = 1154;
            Weight = 10.0;

            Attributes.WeaponDamage = 35;
            Attributes.BonusStr = 5;

            WeaponAttributes.HitLightning = 35;
            WeaponAttributes.ResistEnergyBonus = 15;

            MinDamage = 18;
            MaxDamage = 24;

            AosElementDamages.Energy = 75;
            AosElementDamages.Physical = 25;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Crystal Barrens");
            list.Add("Crafted from Crystalline Ingots");
        }

        public ResonanceMaul(Serial serial) : base(serial) { }

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
