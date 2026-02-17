using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    #region Sun Blade

    /// <summary>
    /// Sun Blade - Desert regional sword
    /// </summary>
    public class SunBlade : Longsword
    {
        [Constructable]
        public SunBlade() : base()
        {
            Name = "Sun Blade";
            Hue = 2305; // Desert gold
            Weight = 5.0; // Lightweight

            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 15;

            WeaponAttributes.HitFireArea = 15;
            WeaponAttributes.ResistFireBonus = 10;
            WeaponAttributes.LowerStatReq = 20; // Easier to wield

            MinDamage = 14;
            MaxDamage = 18;

            AosElementDamages.Fire = 40;
            AosElementDamages.Physical = 60;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sunforged Ingots");
        }

        public SunBlade(Serial serial) : base(serial) { }

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

    #region Dune Scimitar

    /// <summary>
    /// Dune Scimitar - Desert regional sword
    /// </summary>
    public class DuneScimitar : Scimitar
    {
        [Constructable]
        public DuneScimitar() : base()
        {
            Name = "Dune Scimitar";
            Hue = 2305;
            Weight = 4.0;

            Attributes.WeaponDamage = 30;
            Attributes.WeaponSpeed = 20;
            Attributes.AttackChance = 10;

            WeaponAttributes.ResistFireBonus = 10;
            WeaponAttributes.LowerStatReq = 30;

            MinDamage = 13;
            MaxDamage = 17;

            AosElementDamages.Fire = 50;
            AosElementDamages.Physical = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sunforged Ingots");
        }

        public DuneScimitar(Serial serial) : base(serial) { }

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

    #region Mirage Stiletto

    /// <summary>
    /// Mirage Stiletto - Desert regional dagger
    /// Fast weapon with blind effect
    /// </summary>
    public class MirageStiletto : Dagger
    {
        [Constructable]
        public MirageStiletto() : base()
        {
            Name = "Mirage Stiletto";
            Hue = 2305;
            Weight = 0.5; // Very light

            Attributes.WeaponDamage = 20;
            Attributes.WeaponSpeed = 25;
            Attributes.AttackChance = 15;

            WeaponAttributes.LowerStatReq = 50;

            MinDamage = 10;
            MaxDamage = 13;

            AosElementDamages.Fire = 30;
            AosElementDamages.Physical = 70;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 10% chance to blind
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.10)
            {
                defender.SendMessage(0x22, "The desert sun blinds you!");
                // Apply accuracy penalty
                defender.FixedParticles(0x376A, 9, 32, 5030, 2305, 0, EffectLayer.Head);
                defender.PlaySound(0x1E3);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("10% chance to blind target");
        }

        public MirageStiletto(Serial serial) : base(serial) { }

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

    #region Sandstorm Axe

    /// <summary>
    /// Sandstorm Axe - Desert regional axe
    /// Light but powerful
    /// </summary>
    public class SandstormAxe : BattleAxe
    {
        [Constructable]
        public SandstormAxe() : base()
        {
            Name = "Sandstorm Axe";
            Hue = 2305;
            Weight = 5.0; // Lighter than normal

            Attributes.WeaponDamage = 35;
            Attributes.WeaponSpeed = 10;

            WeaponAttributes.HitFireArea = 30;
            WeaponAttributes.ResistFireBonus = 15;
            WeaponAttributes.LowerStatReq = 25;

            MinDamage = 18;
            MaxDamage = 24;

            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Desert");
            list.Add("Crafted from Sunforged Ingots");
        }

        public SandstormAxe(Serial serial) : base(serial) { }

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
