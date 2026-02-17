using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    #region Clockwork Blade

    /// <summary>
    /// Clockwork Blade - Ironclad regional sword
    /// </summary>
    public class ClockworkBlade : Longsword
    {
        [Constructable]
        public ClockworkBlade() : base()
        {
            Name = "Clockwork Blade";
            Hue = 2401; // Clockwork bronze
            Weight = 6.0;

            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 10;

            WeaponAttributes.HitLightning = 20;
            WeaponAttributes.SelfRepair = 5;
            WeaponAttributes.DurabilityBonus = 50;

            MinDamage = 14;
            MaxDamage = 18;

            AosElementDamages.Energy = 40;
            AosElementDamages.Physical = 60;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Clockwork Ingots");
        }

        public ClockworkBlade(Serial serial) : base(serial) { }

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

    #region Gear Edge

    /// <summary>
    /// Gear Edge - Ironclad regional sword
    /// </summary>
    public class GearEdge : Broadsword
    {
        [Constructable]
        public GearEdge() : base()
        {
            Name = "Gear Edge";
            Hue = 2401;
            Weight = 6.0;

            Attributes.WeaponDamage = 30;
            Attributes.DefendChance = 10;

            WeaponAttributes.HitLowerDefend = 30;
            WeaponAttributes.SelfRepair = 5;
            WeaponAttributes.DurabilityBonus = 75;

            MinDamage = 15;
            MaxDamage = 19;

            AosElementDamages.Energy = 50;
            AosElementDamages.Physical = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Crafted from Clockwork Ingots");
        }

        public GearEdge(Serial serial) : base(serial) { }

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

    #region Piston Punch

    /// <summary>
    /// Piston Punch - Ironclad regional kryss
    /// Fast mechanical weapon
    /// </summary>
    public class PistonPunch : Kryss
    {
        [Constructable]
        public PistonPunch() : base()
        {
            Name = "Piston Punch";
            Hue = 2401;
            Weight = 1.0;

            Attributes.WeaponDamage = 20;
            Attributes.WeaponSpeed = 25;
            Attributes.AttackChance = 10;

            WeaponAttributes.HitLowerAttack = 30;
            WeaponAttributes.SelfRepair = 5;

            MinDamage = 10;
            MaxDamage = 13;

            AosElementDamages.Energy = 30;
            AosElementDamages.Physical = 70;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 12% chance for gear grind bonus damage
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.12)
            {
                AOS.Damage(defender, attacker, Utility.RandomMinMax(5, 10), 50, 0, 0, 0, 50);
                defender.SendMessage(0x22, "Gears grind into your flesh!");
                defender.PlaySound(0x2A1);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("12% chance for gear damage");
        }

        public PistonPunch(Serial serial) : base(serial) { }

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

    #region Steam Hammer

    /// <summary>
    /// Steam Hammer - Ironclad regional hammer
    /// Heavy steam-powered weapon
    /// </summary>
    public class SteamHammer : WarHammer
    {
        [Constructable]
        public SteamHammer() : base()
        {
            Name = "Steam Hammer";
            Hue = 2401;
            Weight = 10.0;

            Attributes.WeaponDamage = 40;
            Attributes.BonusStr = 5;

            WeaponAttributes.HitLightning = 40;
            WeaponAttributes.SelfRepair = 5;
            WeaponAttributes.DurabilityBonus = 100;

            MinDamage = 20;
            MaxDamage = 26;

            AosElementDamages.Energy = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Ironclad Empire");
            list.Add("Powered by Steam Core");
        }

        public SteamHammer(Serial serial) : base(serial) { }

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
