using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    #region Void Blade

    /// <summary>
    /// Void Blade - Obsidian regional sword
    /// </summary>
    public class VoidBlade : Longsword
    {
        [Constructable]
        public VoidBlade() : base()
        {
            Name = "Void Blade";
            Hue = 1109; // Shadow black
            Weight = 6.0;

            Attributes.WeaponDamage = 30;
            Attributes.WeaponSpeed = 5;

            WeaponAttributes.HitLeechMana = 25;
            WeaponAttributes.HitLeechHits = 15;

            MinDamage = 15;
            MaxDamage = 20;

            AosElementDamages.Energy = 70;
            AosElementDamages.Physical = 30;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Voidforged Ingots");
        }

        public VoidBlade(Serial serial) : base(serial) { }

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

    #region Obsidian Edge

    /// <summary>
    /// Obsidian Edge - Obsidian regional sword
    /// </summary>
    public class ObsidianEdge : Broadsword
    {
        [Constructable]
        public ObsidianEdge() : base()
        {
            Name = "Obsidian Edge";
            Hue = 1109;
            Weight = 6.0;

            Attributes.WeaponDamage = 35;
            Attributes.SpellDamage = 10;

            WeaponAttributes.HitLeechMana = 30;
            WeaponAttributes.HitDispel = 20;

            MinDamage = 16;
            MaxDamage = 21;

            AosElementDamages.Energy = 80;
            AosElementDamages.Physical = 20;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Crafted from Voidforged Ingots");
        }

        public ObsidianEdge(Serial serial) : base(serial) { }

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

    #region Shadow Fang Dagger

    /// <summary>
    /// Shadow Fang Dagger - Obsidian regional dagger
    /// Fast weapon with mana drain
    /// </summary>
    public class ShadowFangDagger : Dagger
    {
        [Constructable]
        public ShadowFangDagger() : base()
        {
            Name = "Shadow Fang";
            Hue = 1109;
            Weight = 1.0;

            Attributes.WeaponDamage = 20;
            Attributes.WeaponSpeed = 20;
            Attributes.SpellDamage = 5;
            Attributes.LowerManaCost = 5;

            WeaponAttributes.HitLeechMana = 40;

            MinDamage = 10;
            MaxDamage = 14;

            AosElementDamages.Energy = 90;
            AosElementDamages.Physical = 10;
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 15% chance to drain mana
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.15)
            {
                int manaDrain = Utility.RandomMinMax(10, 20);
                if (defender.Mana >= manaDrain)
                {
                    defender.Mana -= manaDrain;
                    attacker.Mana += manaDrain;
                    defender.SendMessage(0x22, "The void drains your magical essence!");
                    defender.FixedParticles(0x374A, 10, 15, 5013, 1109, 0, EffectLayer.Waist);
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("15% chance to drain mana");
        }

        public ShadowFangDagger(Serial serial) : base(serial) { }

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

    #region Void Crusher

    /// <summary>
    /// Void Crusher - Obsidian regional mace
    /// Heavy void-infused weapon
    /// </summary>
    public class VoidCrusher : WarMace
    {
        [Constructable]
        public VoidCrusher() : base()
        {
            Name = "Void Crusher";
            Hue = 1109;
            Weight = 9.0;

            Attributes.WeaponDamage = 40;
            Attributes.SpellDamage = 15;
            Attributes.BonusStr = 5;

            WeaponAttributes.HitLeechMana = 35;
            WeaponAttributes.HitLeechHits = 25;
            WeaponAttributes.HitDispel = 30;

            MinDamage = 20;
            MaxDamage = 28;

            AosElementDamages.Energy = 85;
            AosElementDamages.Physical = 15;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Region: Obsidian Wastes");
            list.Add("Infused with Void Energy");
        }

        public VoidCrusher(Serial serial) : base(serial) { }

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
