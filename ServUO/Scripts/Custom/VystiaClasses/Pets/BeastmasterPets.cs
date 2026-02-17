/*
 * Beastmaster Class Pets
 * Tameable beasts for the Beastmaster class
 *
 * These are permanent pets that use the standard UO taming system
 * They can bond with their owner and persist after logout
 *
 * Beast Types:
 * - Frost Wolf (Frosthold - fast, pack hunter)
 * - Ember Cat (Emberlands - fire attacks)
 * - Desert Raptor (Desert - high damage)
 * - Swamp Basilisk (Shadowfen - poison)
 * - Forest Bear (Verdantpeak - tank)
 * - Crystal Drake (Crystal Barrens - magic)
 */

using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Pets
{
    #region Frost Wolf

    /// <summary>
    /// Frost Wolf - Fast Frosthold beast, pack hunting bonus
    /// Tameable by Beastmasters
    /// </summary>
    [CorpseName("a frost wolf corpse")]
    public class FrostWolf : BaseCreature
    {
        private int m_PackBonus;

        [Constructable]
        public FrostWolf() : base(AIType.AI_Animal, FightMode.Closest, 10, 1, 0.15, 0.3)
        {
            Name = "a frost wolf";
            Body = 23; // Wolf body
            BaseSoundID = 0xE5;
            Hue = 1152; // Ice blue

            SetStr(100, 130);
            SetDex(120, 150);
            SetInt(40, 60);

            SetHits(90, 120);
            SetMana(0);

            SetDamage(10, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 40);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 1500;
            Karma = 0;

            VirtualArmor = 25;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 75.0;

            m_PackBonus = 0;
        }

        public override int Meat { get { return 2; } }
        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Canine; } }

        public override void OnThink()
        {
            base.OnThink();

            // Pack bonus - stronger when multiple frost wolves nearby
            if (Controlled && Combatant != null)
            {
                int nearbyWolves = 0;
                foreach (Mobile m in GetMobilesInRange(5))
                {
                    if (m is FrostWolf fw && fw != this && fw.Controlled && fw.ControlMaster == ControlMaster)
                        nearbyWolves++;
                }

                if (nearbyWolves != m_PackBonus)
                {
                    m_PackBonus = nearbyWolves;
                    if (m_PackBonus > 0)
                    {
                        // Apply pack bonus (up to +30% damage per additional wolf, max 3)
                        int bonus = Math.Min(3, m_PackBonus) * 3;
                        SetDamage(10 + bonus, 18 + bonus);
                    }
                    else
                    {
                        SetDamage(10, 18);
                    }
                }
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Frost bite - chance to slow
            if (Utility.RandomDouble() < 0.15)
            {
                defender.SendMessage("The frost wolf's bite chills you to the bone!");
                defender.FixedParticles(0x374A, 10, 15, 5021, 1152, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E5);
            }
        }

        public FrostWolf(Serial serial) : base(serial)
        {
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

    #endregion

    #region Ember Cat

    /// <summary>
    /// Ember Cat - Emberlands fire cat, high agility
    /// </summary>
    [CorpseName("an ember cat corpse")]
    public class EmberCat : BaseCreature
    {
        [Constructable]
        public EmberCat() : base(AIType.AI_Animal, FightMode.Closest, 10, 1, 0.1, 0.2)
        {
            Name = "an ember cat";
            Body = 201; // Panther/big cat body
            BaseSoundID = 0x69;
            Hue = 1161; // Fiery orange

            SetStr(90, 110);
            SetDex(140, 170);
            SetInt(50, 70);

            SetHits(80, 100);
            SetMana(0);

            SetDamage(12, 20);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 60, 80);
            SetResistance(ResistanceType.Cold, 5, 15);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 40.0, 60.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 1800;
            Karma = 0;

            VirtualArmor = 20;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 80.0;
        }

        public override int Meat { get { return 2; } }
        public override int Hides { get { return 6; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Feline; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Fire claws - chance to ignite
            if (Utility.RandomDouble() < 0.20)
            {
                defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                defender.PlaySound(0x208);
                AOS.Damage(defender, this, Utility.RandomMinMax(5, 12), 0, 100, 0, 0, 0);
                defender.SendMessage("The ember cat's claws leave burning wounds!");
            }
        }

        public EmberCat(Serial serial) : base(serial)
        {
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

    #endregion

    #region Desert Raptor

    /// <summary>
    /// Desert Raptor - Desert region, high damage hunter
    /// </summary>
    [CorpseName("a desert raptor corpse")]
    public class DesertRaptor : BaseCreature
    {
        [Constructable]
        public DesertRaptor() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.3)
        {
            Name = "a desert raptor";
            Body = 0xCE; // Raptor body
            BaseSoundID = 0x5A;
            Hue = 2413; // Sandy brown

            SetStr(130, 160);
            SetDex(110, 140);
            SetInt(30, 50);

            SetHits(110, 140);
            SetMana(0);

            SetDamage(15, 25);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 40, 55);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 55.0, 75.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);

            Fame = 2200;
            Karma = 0;

            VirtualArmor = 30;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 90.0;
        }

        public override int Meat { get { return 3; } }
        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Rending claws - chance for bleed
            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage("The raptor's claws leave deep, bleeding wounds!");
                defender.PlaySound(0x133);

                // Apply bleed damage over time
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 5, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, 3, 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public DesertRaptor(Serial serial) : base(serial)
        {
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

    #endregion

    #region Swamp Basilisk

    /// <summary>
    /// Swamp Basilisk - Shadowfen, poison and petrification
    /// </summary>
    [CorpseName("a swamp basilisk corpse")]
    public class SwampBasilisk : BaseCreature
    {
        private DateTime m_NextPetrifyGaze;

        [Constructable]
        public SwampBasilisk() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a swamp basilisk";
            Body = 0x5A; // Lizard/reptile body
            BaseSoundID = 0x5A;
            Hue = 2967; // Murky green

            SetStr(120, 150);
            SetDex(80, 100);
            SetInt(60, 80);

            SetHits(100, 130);
            SetMana(50);

            SetDamage(12, 20);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 70, 90);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 75.0, 95.0);
            SetSkill(SkillName.Wrestling, 75.0, 95.0);
            SetSkill(SkillName.Poisoning, 70.0, 90.0);

            Fame = 2000;
            Karma = 0;

            VirtualArmor = 35;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 85.0;

            m_NextPetrifyGaze = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        }

        public override int Meat { get { return 2; } }
        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Spined; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override Poison HitPoison { get { return Poison.Regular; } }

        public override void OnThink()
        {
            base.OnThink();

            // Petrifying Gaze ability when controlled
            if (Controlled && Combatant != null && DateTime.UtcNow >= m_NextPetrifyGaze)
            {
                DoPetrifyGaze();
                m_NextPetrifyGaze = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            }
        }

        private void DoPetrifyGaze()
        {
            Mobile target = Combatant as Mobile;
            if (target == null || !InRange(target, 8))
                return;

            Emote("*the basilisk's eyes glow*");
            FixedParticles(0x376A, 9, 32, 5030, 2967, 0, EffectLayer.Waist);
            PlaySound(0x1FB);

            if (Utility.RandomDouble() < 0.30)
            {
                target.SendMessage("The basilisk's gaze momentarily freezes you in place!");
                target.Freeze(TimeSpan.FromSeconds(2));
                target.FixedParticles(0x376A, 9, 32, 5030, 2101, 0, EffectLayer.Waist);
            }
        }

        public SwampBasilisk(Serial serial) : base(serial)
        {
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

            m_NextPetrifyGaze = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        }
    }

    #endregion

    #region Forest Bear

    /// <summary>
    /// Forest Bear - Verdantpeak, tanky nature beast
    /// </summary>
    [CorpseName("a forest bear corpse")]
    public class ForestBear : BaseCreature
    {
        [Constructable]
        public ForestBear() : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.25, 0.45)
        {
            Name = "a forest bear";
            Body = 212; // Bear body
            BaseSoundID = 0xA3;
            Hue = 2129; // Forest green-brown

            SetStr(180, 220);
            SetDex(60, 80);
            SetInt(40, 60);

            SetHits(150, 190);
            SetMana(0);

            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 60);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 40, 55);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 1800;
            Karma = 0;

            VirtualArmor = 40;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 75.0;
        }

        public override int Meat { get { return 4; } }
        public override int Hides { get { return 12; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.FruitsAndVegies | FoodType.Fish; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Bear; } }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Regeneration when low health (controlled only)
            if (Controlled && Hits < HitsMax * 0.3 && Utility.RandomDouble() < 0.15)
            {
                int heal = Utility.RandomMinMax(10, 25);
                Hits = Math.Min(HitsMax, Hits + heal);
                FixedParticles(0x376A, 9, 32, 5005, 2129, 0, EffectLayer.Waist);
                PlaySound(0x1F2);
            }
        }

        public ForestBear(Serial serial) : base(serial)
        {
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

    #endregion

    #region Crystal Drake

    /// <summary>
    /// Crystal Drake - Crystal Barrens, magic attacks
    /// Hardest to tame, most powerful
    /// </summary>
    [CorpseName("a crystal drake corpse")]
    public class CrystalDrake : BaseCreature
    {
        private DateTime m_NextCrystalBreath;

        [Constructable]
        public CrystalDrake() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a crystal drake";
            Body = 60; // Drake body
            BaseSoundID = 362;
            Hue = 1153; // Crystal/prismatic

            SetStr(160, 200);
            SetDex(100, 130);
            SetInt(140, 180);

            SetHits(140, 180);
            SetMana(200);

            SetDamage(16, 26);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Energy, 70);

            SetResistance(ResistanceType.Physical, 40, 55);
            SetResistance(ResistanceType.Fire, 30, 45);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 35, 50);
            SetResistance(ResistanceType.Energy, 60, 80);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 3500;
            Karma = 0;

            VirtualArmor = 45;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 100.0;

            m_NextCrystalBreath = DateTime.UtcNow + TimeSpan.FromSeconds(8);
        }

        public override int Meat { get { return 4; } }
        public override int Hides { get { return 14; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }
        public override bool CanFly { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();

            // Crystal Breath ability
            if (Combatant != null && DateTime.UtcNow >= m_NextCrystalBreath)
            {
                DoCrystalBreath();
                m_NextCrystalBreath = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            }
        }

        private void DoCrystalBreath()
        {
            Mobile target = Combatant as Mobile;
            if (target == null || !InRange(target, 8))
                return;

            Direction = GetDirectionTo(target);
            BreathAttack(target);
        }

        public void BreathAttack(Mobile target)
        {
            Effects.SendMovingParticles(this, target, 0x36D4, 5, 0, false, false, 1153, 0, 9502, 0x100, 0, (EffectLayer)255, 0x100);
            PlaySound(0x227);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (target == null || target.Deleted || !CanBeHarmful(target))
                    return;

                int damage = Utility.RandomMinMax(25, 40);
                AOS.Damage(target, this, damage, 0, 0, 0, 0, 100);
                target.FixedParticles(0x374A, 10, 15, 5032, 1153, 0, EffectLayer.Head);
            });
        }

        public CrystalDrake(Serial serial) : base(serial)
        {
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

            m_NextCrystalBreath = DateTime.UtcNow + TimeSpan.FromSeconds(8);
        }
    }

    #endregion
}
