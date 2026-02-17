/*
 * Necromancer Class Pets
 * Undead minions for the Necromancer class
 *
 * Summon Types:
 * - Skeleton Warrior (basic melee)
 * - Zombie (tank, disease)
 * - Skeletal Mage (caster)
 * - Wraith (ethereal, life drain)
 * - Bone Knight (elite melee)
 */

using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Pets
{
    #region Skeleton Warrior

    /// <summary>
    /// Summoned Skeleton - Basic melee undead
    /// </summary>
    [CorpseName("a skeleton corpse")]
    public class SummonedSkeleton : VystiaSummonedPet
    {
        [Constructable]
        public SummonedSkeleton() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedSkeleton(VystiaPetTier tier)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = GetTierName(tier) + " Skeleton Warrior";
            Body = 50; // Skeleton body
            BaseSoundID = 0x48D;
            Hue = 0; // Bone white

            PetType = VystiaPetType.UndeadMinion;
            PetTier = tier;

            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(100 + (int)tier * 20);
            SetDex(100 + (int)tier * 15);
            SetInt(30 + (int)tier * 10);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 100); // Immune to poison
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 40.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 60.0 + (int)tier *10);
            SetSkill(SkillName.Wrestling, 60.0 + (int)tier *10);
            SetSkill(SkillName.Swords, 60.0 + (int)tier *10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 20 + (int)tier * 5;

            // Give equipment based on tier
            EquipSkeleton(tier);
        }

        private void EquipSkeleton(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser:
                    AddItem(new Cutlass());
                    break;
                case VystiaPetTier.Standard:
                    AddItem(new Longsword());
                    AddItem(new WoodenShield());
                    break;
                case VystiaPetTier.Greater:
                    AddItem(new VikingSword());
                    AddItem(new HeaterShield());
                    AddItem(new ChainChest());
                    break;
                case VystiaPetTier.Superior:
                    AddItem(new Halberd());
                    AddItem(new PlateChest());
                    AddItem(new PlateHelm());
                    break;
                case VystiaPetTier.Legendary:
                    AddItem(new Bardiche());
                    AddItem(new PlateChest());
                    AddItem(new PlateLegs());
                    AddItem(new PlateArms());
                    AddItem(new BoneHelm());
                    break;
            }
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Lesser";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Greater";
                case VystiaPetTier.Superior: return "Elite";
                case VystiaPetTier.Legendary: return "Bone Champion";
                default: return "";
            }
        }

        public SummonedSkeleton(Serial serial) : base(serial)
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

    #region Zombie

    /// <summary>
    /// Summoned Zombie - Tank undead with disease
    /// </summary>
    [CorpseName("a zombie corpse")]
    public class SummonedZombie : VystiaSummonedPet
    {
        [Constructable]
        public SummonedZombie() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedZombie(VystiaPetTier tier)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.5)
        {
            Name = GetTierName(tier) + " Zombie";
            Body = 3; // Zombie body
            BaseSoundID = 471;
            Hue = 2967; // Sickly green-gray

            PetType = VystiaPetType.UndeadMinion;
            PetTier = tier;

            // Zombies get bonus HP
            int hp = (int)(VystiaPetStats.GetBaseHP(tier) * 1.3);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);
            // But less damage
            minDam = (int)(minDam * 0.8);
            maxDam = (int)(maxDam * 0.8);

            SetStr(150 + (int)tier * 25);
            SetDex(40 + (int)tier * 5);
            SetInt(20 + (int)tier * 5);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 35, 50);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 30, 50);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 30.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 50.0 + (int)tier *10);
            SetSkill(SkillName.Wrestling, 70.0 + (int)tier *10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 35 + (int)tier * 8;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Decayed";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Bloated";
                case VystiaPetTier.Superior: return "Plague";
                case VystiaPetTier.Legendary: return "Abomination";
                default: return "";
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Chance to inflict poison/disease
            double chance = 0.15 + ((int)PetTier * 0.05);
            if (Utility.RandomDouble() < chance)
            {
                Poison poison = PetTier >= VystiaPetTier.Greater ? Poison.Greater : Poison.Regular;
                defender.ApplyPoison(this, poison);
                defender.SendMessage("The zombie's rotting touch infects you!");
            }
        }

        public SummonedZombie(Serial serial) : base(serial)
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

    #region Skeletal Mage

    /// <summary>
    /// Summoned Skeletal Mage - Undead caster
    /// </summary>
    [CorpseName("a skeletal mage corpse")]
    public class SummonedSkeletalMage : VystiaSummonedPet
    {
        [Constructable]
        public SummonedSkeletalMage() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedSkeletalMage(VystiaPetTier tier)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = GetTierName(tier) + " Skeletal Mage";
            Body = 148; // Skeletal mage body
            BaseSoundID = 451;
            Hue = 1109; // Dark purple

            PetType = VystiaPetType.UndeadMinion;
            PetTier = tier;

            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(60 + (int)tier * 10);
            SetDex(80 + (int)tier * 10);
            SetInt(150 + (int)tier * 25);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Cold, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 70.0 + (int)tier *10);
            SetSkill(SkillName.Magery, 70.0 + (int)tier *10);
            SetSkill(SkillName.Necromancy, 60.0 + (int)tier *10);
            SetSkill(SkillName.SpiritSpeak, 60.0 + (int)tier *10);
            SetSkill(SkillName.MagicResist, 60.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 40.0 + (int)tier *5);
            SetSkill(SkillName.Wrestling, 40.0 + (int)tier *5);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 25 + (int)tier * 5;

            AddItem(new Robe(1109));
            AddItem(new WizardsHat(1109));
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Apprentice";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Greater";
                case VystiaPetTier.Superior: return "Master";
                case VystiaPetTier.Legendary: return "Archlich";
                default: return "";
            }
        }

        public SummonedSkeletalMage(Serial serial) : base(serial)
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

    #region Wraith

    /// <summary>
    /// Summoned Wraith - Ethereal undead with life drain
    /// </summary>
    [CorpseName("a wraith corpse")]
    public class SummonedWraith : VystiaSummonedPet
    {
        [Constructable]
        public SummonedWraith() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedWraith(VystiaPetTier tier)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.15, 0.3)
        {
            Name = GetTierName(tier) + " Wraith";
            Body = 26; // Wraith body
            BaseSoundID = 0x482;
            Hue = 1150; // Ghostly blue-white

            PetType = VystiaPetType.UndeadServant;
            PetTier = tier;

            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(80 + (int)tier * 15);
            SetDex(120 + (int)tier * 15);
            SetInt(120 + (int)tier * 20);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Cold, 50);
            SetDamageType(ResistanceType.Energy, 50);

            // High resistances but weak to fire
            SetResistance(ResistanceType.Physical, 35, 55);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 50, 70);

            SetSkill(SkillName.EvalInt, 60.0 + (int)tier *10);
            SetSkill(SkillName.Magery, 60.0 + (int)tier *10);
            SetSkill(SkillName.MagicResist, 70.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 60.0 + (int)tier *10);
            SetSkill(SkillName.Wrestling, 60.0 + (int)tier *10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 30 + (int)tier * 5;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Shade";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Greater";
                case VystiaPetTier.Superior: return "Spectral";
                case VystiaPetTier.Legendary: return "Death";
                default: return "";
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Life drain effect
            if (Utility.RandomDouble() < 0.25 + ((int)PetTier * 0.05))
            {
                int drain = Utility.RandomMinMax(5, 10 + (int)PetTier * 3);
                defender.Damage(drain, this);
                Hits = Math.Min(HitsMax, Hits + drain);

                defender.FixedParticles(0x374A, 10, 15, 5013, 1150, 0, EffectLayer.Waist);
                defender.PlaySound(0x1F1);
                defender.SendMessage("The wraith drains your life force!");
            }
        }

        public SummonedWraith(Serial serial) : base(serial)
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

    #region Bone Knight

    /// <summary>
    /// Summoned Bone Knight - Elite undead warrior
    /// </summary>
    [CorpseName("a bone knight corpse")]
    public class SummonedBoneKnight : VystiaSummonedPet
    {
        [Constructable]
        public SummonedBoneKnight() : this(VystiaPetTier.Greater)
        {
        }

        public SummonedBoneKnight(VystiaPetTier tier)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.3)
        {
            // Minimum tier is Greater for Bone Knight
            if (tier < VystiaPetTier.Greater)
                tier = VystiaPetTier.Greater;

            Name = GetTierName(tier) + " Bone Knight";
            Body = 57; // Bone knight body
            BaseSoundID = 451;
            Hue = 0;

            PetType = VystiaPetType.UndeadServant;
            PetTier = tier;

            // Bone Knights are elite - bonus stats
            int hp = (int)(VystiaPetStats.GetBaseHP(tier) * 1.2);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);
            minDam = (int)(minDam * 1.2);
            maxDam = (int)(maxDam * 1.2);

            SetStr(200 + (int)tier * 30);
            SetDex(100 + (int)tier * 15);
            SetInt(50 + (int)tier * 10);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Cold, 20);

            SetResistance(ResistanceType.Physical, 40, 55);
            SetResistance(ResistanceType.Fire, 20, 35);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 30, 45);

            SetSkill(SkillName.MagicResist, 60.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 80.0 + (int)tier *10);
            SetSkill(SkillName.Wrestling, 80.0 + (int)tier *10);
            SetSkill(SkillName.Swords, 80.0 + (int)tier *10);
            SetSkill(SkillName.Anatomy, 60.0 + (int)tier *10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 45 + (int)tier * 10;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Greater: return "";
                case VystiaPetTier.Superior: return "Champion";
                case VystiaPetTier.Legendary: return "Death Knight";
                default: return "";
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Bone Shatter - chance to reduce target armor
            if (Utility.RandomDouble() < 0.15 + ((int)PetTier * 0.03))
            {
                defender.PlaySound(0x1FB);
                defender.FixedParticles(0x37B9, 10, 5, 5052, EffectLayer.Head);
                defender.SendMessage("The bone knight's strike shatters your defenses!");
                // Note: Could apply armor debuff using VystiaBuffSystem
            }
        }

        public SummonedBoneKnight(Serial serial) : base(serial)
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
}
