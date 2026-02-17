using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    #region Flame Tongue

    /// <summary>
    /// Flame Tongue - Emberlands regional sword
    /// </summary>
    public class FlameTongue : Longsword
    {
        [Constructable]
        public FlameTongue() : base()
        {
            Name = "Flame Tongue";
            Hue = 1358; // Fire orange
            Weight = 6.0;

            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 10;

            WeaponAttributes.HitFireArea = 20;
            WeaponAttributes.ResistFireBonus = 10;

            MinDamage = 14;
            MaxDamage = 18;

            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Emberforged Ingots");
        }

        public FlameTongue(Serial serial) : base(serial) { }

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

    #region Magma Blade

    /// <summary>
    /// Magma Blade - Emberlands regional sword
    /// </summary>
    public class MagmaBlade : Broadsword
    {
        [Constructable]
        public MagmaBlade() : base()
        {
            Name = "Magma Blade";
            Hue = 1359; // Deep fire red
            Weight = 6.0;

            Attributes.WeaponDamage = 30;
            Attributes.DefendChance = 10;

            WeaponAttributes.HitFireArea = 25;
            WeaponAttributes.HitFireball = 20;
            WeaponAttributes.ResistFireBonus = 10;

            MinDamage = 15;
            MaxDamage = 19;

            AosElementDamages.Fire = 70;
            AosElementDamages.Physical = 30;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Emberforged Ingots");
        }

        public MagmaBlade(Serial serial) : base(serial) { }

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

    #region Phoenix Wing

    /// <summary>
    /// Phoenix Wing - Emberlands regional katana
    /// Fast weapon with life steal
    /// </summary>
    public class PhoenixWing : Katana
    {
        [Constructable]
        public PhoenixWing() : base()
        {
            Name = "Phoenix Wing";
            Hue = 1358;
            Weight = 5.0;

            Attributes.WeaponDamage = 20;
            Attributes.WeaponSpeed = 15;
            Attributes.AttackChance = 10;
            Attributes.RegenHits = 2;

            WeaponAttributes.HitFireball = 25;
            WeaponAttributes.ResistFireBonus = 10;

            MinDamage = 11;
            MaxDamage = 15;

            AosElementDamages.Fire = 80;
            AosElementDamages.Physical = 20;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 8% chance to heal on hit
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.08)
            {
                int heal = Utility.RandomMinMax(5, 10);
                attacker.Heal(heal);
                attacker.SendMessage(0x22, "The phoenix flame restores you!");
                attacker.FixedParticles(0x376A, 9, 32, 5030, 1358, 0, EffectLayer.Waist);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("8% chance to heal on hit");
        }

        public PhoenixWing(Serial serial) : base(serial) { }

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

    #region Lava Edge

    /// <summary>
    /// Lava Edge - Emberlands regional axe
    /// Heavy hitting fire damage weapon
    /// </summary>
    public class LavaEdge : BattleAxe
    {
        [Constructable]
        public LavaEdge() : base()
        {
            Name = "Lava Edge";
            Hue = 1359;
            Weight = 8.0;

            Attributes.WeaponDamage = 35;
            Attributes.BonusStr = 5;

            WeaponAttributes.HitFireArea = 35;
            WeaponAttributes.ResistFireBonus = 15;

            MinDamage = 18;
            MaxDamage = 24;

            AosElementDamages.Fire = 75;
            AosElementDamages.Physical = 25;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Emberlands");
            list.Add("Crafted from Emberforged Ingots");
        }

        public LavaEdge(Serial serial) : base(serial) { }

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
