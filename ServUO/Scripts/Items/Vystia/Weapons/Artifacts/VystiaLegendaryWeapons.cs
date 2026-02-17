using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    #region Phoenix Ascension - Volcano Wyrm Drop

    /// <summary>
    /// Phoenix Ascension - Legendary Artifact Katana
    /// Drop: Volcano Wyrm (Emberlands boss) - 2% chance
    /// </summary>
    public class PhoenixAscension : Katana
    {
        [Constructable]
        public PhoenixAscension() : base()
        {
            Name = "Phoenix Ascension";
            Hue = 1358; // Fire orange
            Weight = 6.0;

            // Artifact-level stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            SkillBonuses.SetValues(1, SkillName.Swords, 10.0);

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 25;
            Attributes.AttackChance = 15;
            Attributes.Luck = 100;
            Attributes.RegenHits = 3;

            WeaponAttributes.HitFireArea = 50;
            WeaponAttributes.HitFireball = 40;
            WeaponAttributes.ResistFireBonus = 15;

            MinDamage = 18;
            MaxDamage = 22;

            // 100% Fire damage
            AosElementDamages.Fire = 100;
            AosElementDamages.Physical = 0;

            Slayer = SlayerName.WaterDissipation; // Bonus vs water creatures
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Rises from the ashes of defeat");
            list.Add("Dropped by the Volcano Wyrm");
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 10% chance for phoenix rebirth effect - heals attacker
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.10)
            {
                int heal = Utility.RandomMinMax(15, 25);
                attacker.Heal(heal);
                attacker.SendMessage(0x22, "The phoenix flame restores your vitality!");
                attacker.FixedParticles(0x376A, 9, 32, 5030, 1358, 0, EffectLayer.Waist);
                attacker.PlaySound(0x208);
            }
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

    #endregion

    #region The Cogmaster - Forge Master Drop

    /// <summary>
    /// The Cogmaster - Legendary Artifact War Hammer
    /// Drop: Forge Master (Ironclad boss) - 2% chance
    /// </summary>
    public class TheCogmaster : WarHammer
    {
        [Constructable]
        public TheCogmaster() : base()
        {
            Name = "The Cogmaster";
            Hue = 2401; // Clockwork bronze
            Weight = 10.0;

            // Artifact-level stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            SkillBonuses.SetValues(1, SkillName.Macing, 10.0);

            Attributes.WeaponDamage = 60;
            Attributes.WeaponSpeed = 10;
            Attributes.BonusStr = 8;
            Attributes.Luck = 100;

            WeaponAttributes.HitLightning = 45;
            WeaponAttributes.HitLowerDefend = 40;
            WeaponAttributes.SelfRepair = 5;

            MinDamage = 20;
            MaxDamage = 28;

            // 50% Physical, 50% Energy
            AosElementDamages.Physical = 50;
            AosElementDamages.Energy = 50;

            Slayer = SlayerName.ElementalBan; // Bonus vs elementals
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary");
            list.Add("The gears of war never stop turning");
            list.Add("Dropped by the Forge Master");
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 12% chance for gear grind effect - armor ignore
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.12)
            {
                defender.SendMessage(0x22, "The clockwork gears grind through your defenses!");
                AOS.Damage(defender, attacker, Utility.RandomMinMax(10, 20), 100, 0, 0, 0, 0);
                defender.FixedParticles(0x3779, 1, 15, 9502, 2401, 0, EffectLayer.Head);
                defender.PlaySound(0x2A1);
            }
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

    #endregion

    #region Prismatic Edge - Crystal Dragon Drop

    /// <summary>
    /// Prismatic Edge - Legendary Artifact Longsword
    /// Drop: Crystal Dragon (Crystal Barrens boss) - 2% chance
    /// </summary>
    public class PrismaticEdge : Longsword
    {
        [Constructable]
        public PrismaticEdge() : base()
        {
            Name = "Prismatic Edge";
            Hue = 0; // Prismatic/rainbow effect via property
            Weight = 6.0;

            // Artifact-level stats
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            SkillBonuses.SetValues(1, SkillName.Swords, 10.0);

            Attributes.WeaponDamage = 45;
            Attributes.WeaponSpeed = 20;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 1;
            Attributes.Luck = 150;

            WeaponAttributes.HitMagicArrow = 40;
            WeaponAttributes.MageWeapon = 20;

            MinDamage = 16;
            MaxDamage = 22;

            // Split elemental damage
            AosElementDamages.Physical = 20;
            AosElementDamages.Fire = 20;
            AosElementDamages.Cold = 20;
            AosElementDamages.Poison = 20;
            AosElementDamages.Energy = 20;

            Slayer = SlayerName.Exorcism; // Bonus vs undead
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Refracts all elements of destruction");
            list.Add("Dropped by the Crystal Dragon");
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 15% chance for prismatic burst - random elemental damage
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.15)
            {
                int element = Utility.Random(5);
                int damage = Utility.RandomMinMax(8, 15);

                switch (element)
                {
                    case 0:
                        AOS.Damage(defender, attacker, damage, 100, 0, 0, 0, 0);
                        defender.FixedParticles(0x36BD, 20, 10, 5044, 0, 0, EffectLayer.Head);
                        break;
                    case 1:
                        AOS.Damage(defender, attacker, damage, 0, 100, 0, 0, 0);
                        defender.FixedParticles(0x36BD, 20, 10, 5044, 1358, 0, EffectLayer.Head);
                        break;
                    case 2:
                        AOS.Damage(defender, attacker, damage, 0, 0, 100, 0, 0);
                        defender.FixedParticles(0x36BD, 20, 10, 5044, 1152, 0, EffectLayer.Head);
                        break;
                    case 3:
                        AOS.Damage(defender, attacker, damage, 0, 0, 0, 100, 0);
                        defender.FixedParticles(0x36BD, 20, 10, 5044, 1167, 0, EffectLayer.Head);
                        break;
                    case 4:
                        AOS.Damage(defender, attacker, damage, 0, 0, 0, 0, 100);
                        defender.FixedParticles(0x36BD, 20, 10, 5044, 1153, 0, EffectLayer.Head);
                        break;
                }

                defender.SendMessage(0x35, "Prismatic energy courses through you!");
                defender.PlaySound(0x211);
            }
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

    #endregion

    #region Voidcaller - Shadow Lich Drop

    /// <summary>
    /// Voidcaller - Legendary Artifact Quarter Staff
    /// Drop: Shadow Lich (Shadowfen boss) - 2% chance
    /// </summary>
    public class Voidcaller : QuarterStaff
    {
        [Constructable]
        public Voidcaller() : base()
        {
            Name = "Voidcaller";
            Hue = 1109; // Shadow black
            Weight = 4.0;

            // Artifact-level stats
            SkillBonuses.SetValues(0, SkillName.Magery, 15.0);
            SkillBonuses.SetValues(1, SkillName.Necromancy, 15.0);

            Attributes.WeaponDamage = 40;
            Attributes.SpellDamage = 15;
            Attributes.CastRecovery = 3;
            Attributes.LowerManaCost = 10;
            Attributes.Luck = 100;

            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.HitLeechHits = 30;
            WeaponAttributes.HitDispel = 25;

            MinDamage = 14;
            MaxDamage = 18;

            // 100% Energy (void) damage
            AosElementDamages.Energy = 100;
            AosElementDamages.Physical = 0;

            Slayer = SlayerName.Silver; // Bonus vs undead
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Whispers secrets from the void");
            list.Add("Dropped by the Shadow Lich");
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 10% chance for void touch - mana drain
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.10)
            {
                int manaDrain = Utility.RandomMinMax(20, 35);
                if (defender.Mana >= manaDrain)
                {
                    defender.Mana -= manaDrain;
                    attacker.Mana += manaDrain;
                    defender.SendMessage(0x22, "The void drains your magical essence!");
                    attacker.SendMessage(0x35, "You absorb magical essence from the void!");
                    defender.FixedParticles(0x374A, 10, 15, 5013, 1109, 0, EffectLayer.Waist);
                    defender.PlaySound(0x1F8);
                }
            }
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

    #endregion
}
