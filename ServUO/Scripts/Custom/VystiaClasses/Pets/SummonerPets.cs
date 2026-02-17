/*
 * Summoner Class Pets
 * Elemental and creature summons for the Summoner class
 *
 * Summon Types:
 * - Fire Elemental (damage focus)
 * - Ice Elemental (slow/control)
 * - Earth Elemental (tank)
 * - Storm Elemental (AoE)
 * - Water Elemental (healing)
 */

using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Pets
{
    #region Fire Elemental

    /// <summary>
    /// Summoned Fire Elemental - High damage, fire attacks
    /// </summary>
    [CorpseName("a fire elemental corpse")]
    public class SummonedFireElemental : VystiaSummonedPet
    {
        [Constructable]
        public SummonedFireElemental() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedFireElemental(VystiaPetTier tier)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = GetTierName(tier) + " Fire Elemental";
            Body = 15;
            BaseSoundID = 838;
            Hue = 1161; // Bright orange/red

            PetType = VystiaPetType.SummonedElemental;
            PetTier = tier;

            // Base stats (will be scaled by summoner)
            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(200 + (int)tier * 25);
            SetDex(100 + (int)tier * 10);
            SetInt(100 + (int)tier * 15);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Fire, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 70, 90);
            SetResistance(ResistanceType.Cold, 5, 15);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 60.0 + (int)tier *10);
            SetSkill(SkillName.Magery, 60.0 + (int)tier *10);
            SetSkill(SkillName.MagicResist, 60.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 60.0 + (int)tier *10);
            SetSkill(SkillName.Wrestling, 60.0 + (int)tier *10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 30 + (int)tier * 5;

            AddItem(new LightSource());
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Lesser";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Greater";
                case VystiaPetTier.Superior: return "Superior";
                case VystiaPetTier.Legendary: return "Ancient";
                default: return "";
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Fire aura effect
            if (Combatant != null && Utility.RandomDouble() < 0.1)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 1161, 0, 5052, 0);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Chance to ignite target
            if (Utility.RandomDouble() < 0.15 + ((int)PetTier * 0.05))
            {
                defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                defender.PlaySound(0x208);
                AOS.Damage(defender, this, Utility.RandomMinMax(5, 10 + (int)PetTier * 3), 0, 100, 0, 0, 0);
            }
        }

        public SummonedFireElemental(Serial serial) : base(serial)
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

    #region Ice Elemental

    /// <summary>
    /// Summoned Ice Elemental - Control focus, slows enemies
    /// </summary>
    [CorpseName("an ice elemental corpse")]
    public class SummonedIceElemental : VystiaSummonedPet
    {
        [Constructable]
        public SummonedIceElemental() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedIceElemental(VystiaPetTier tier)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = GetTierName(tier) + " Ice Elemental";
            Body = 161; // Ice elemental body
            BaseSoundID = 268;
            Hue = 1152; // Ice blue

            PetType = VystiaPetType.SummonedElemental;
            PetTier = tier;

            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(150 + (int)tier * 20);
            SetDex(80 + (int)tier * 10);
            SetInt(150 + (int)tier * 20);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Cold, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 70, 90);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 70.0 + (int)tier *10);
            SetSkill(SkillName.Magery, 70.0 + (int)tier *10);
            SetSkill(SkillName.MagicResist, 70.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 50.0 + (int)tier *10);
            SetSkill(SkillName.Wrestling, 50.0 + (int)tier *10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 35 + (int)tier * 5;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Lesser";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Greater";
                case VystiaPetTier.Superior: return "Superior";
                case VystiaPetTier.Legendary: return "Primordial";
                default: return "";
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Chance to freeze/slow target
            if (Utility.RandomDouble() < 0.20 + ((int)PetTier * 0.05))
            {
                defender.FixedParticles(0x374A, 10, 15, 5021, 1152, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E5);

                // Apply slow effect (reduce dex temporarily)
                if (defender is Mobile m)
                {
                    m.SendMessage("You feel your movements slow as ice spreads across your limbs!");
                    // Note: Could add a proper slow debuff here using VystiaBuffSystem
                }
            }
        }

        public SummonedIceElemental(Serial serial) : base(serial)
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

    #region Earth Elemental

    /// <summary>
    /// Summoned Earth Elemental - Tank focus, high defense
    /// </summary>
    [CorpseName("an earth elemental corpse")]
    public class SummonedEarthElemental : VystiaSummonedPet
    {
        [Constructable]
        public SummonedEarthElemental() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedEarthElemental(VystiaPetTier tier)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = GetTierName(tier) + " Earth Elemental";
            Body = 14;
            BaseSoundID = 268;
            Hue = 2413; // Brown/stone color

            PetType = VystiaPetType.SummonedElemental;
            PetTier = tier;

            // Earth elementals get bonus HP
            int hp = (int)(VystiaPetStats.GetBaseHP(tier) * 1.5);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(250 + (int)tier * 30);
            SetDex(60 + (int)tier * 5);
            SetInt(50 + (int)tier * 10);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Physical, 100);

            // High physical resistance
            SetResistance(ResistanceType.Physical, 50, 70);
            SetResistance(ResistanceType.Fire, 20, 35);
            SetResistance(ResistanceType.Cold, 20, 35);
            SetResistance(ResistanceType.Poison, 40, 55);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 60.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 80.0 + (int)tier *10);
            SetSkill(SkillName.Wrestling, 80.0 + (int)tier *10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 50 + (int)tier * 10;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Lesser";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Greater";
                case VystiaPetTier.Superior: return "Superior";
                case VystiaPetTier.Legendary: return "Mountain";
                default: return "";
            }
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            base.OnDamagedBySpell(caster);

            // Reflect some damage back as physical
            if (Utility.RandomDouble() < 0.1 + ((int)PetTier * 0.03))
            {
                int reflect = Utility.RandomMinMax(5, 15 + (int)PetTier * 3);
                AOS.Damage(caster, this, reflect, 100, 0, 0, 0, 0);
                caster.SendMessage("Stone shards fly back at you!");
                PlaySound(0x1F1);
            }
        }

        public SummonedEarthElemental(Serial serial) : base(serial)
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

    #region Storm Elemental

    /// <summary>
    /// Summoned Storm Elemental - AoE lightning damage
    /// </summary>
    [CorpseName("a storm elemental corpse")]
    public class SummonedStormElemental : VystiaSummonedPet
    {
        private DateTime m_NextStormBurst;

        [Constructable]
        public SummonedStormElemental() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedStormElemental(VystiaPetTier tier)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.15, 0.3)
        {
            Name = GetTierName(tier) + " Storm Elemental";
            Body = 13; // Air elemental body
            BaseSoundID = 655;
            Hue = 1165; // Electric blue

            PetType = VystiaPetType.SummonedElemental;
            PetTier = tier;

            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);

            SetStr(150 + (int)tier * 20);
            SetDex(150 + (int)tier * 15);
            SetInt(100 + (int)tier * 15);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Energy, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 70, 90);

            SetSkill(SkillName.EvalInt, 70.0 + (int)tier *10);
            SetSkill(SkillName.Magery, 70.0 + (int)tier *10);
            SetSkill(SkillName.MagicResist, 60.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 60.0 + (int)tier *10);
            SetSkill(SkillName.Wrestling, 60.0 + (int)tier *10);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 25 + (int)tier * 5;

            m_NextStormBurst = DateTime.UtcNow;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Lesser";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Greater";
                case VystiaPetTier.Superior: return "Superior";
                case VystiaPetTier.Legendary: return "Tempest";
                default: return "";
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Storm Burst AoE ability
            if (Combatant != null && DateTime.UtcNow >= m_NextStormBurst)
            {
                DoStormBurst();
                m_NextStormBurst = DateTime.UtcNow + TimeSpan.FromSeconds(15 - (int)PetTier);
            }
        }

        private void DoStormBurst()
        {
            PlaySound(0x29);
            FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);

            int range = 3 + (int)PetTier;
            int damage = 10 + (int)PetTier * 5;

            foreach (Mobile m in GetMobilesInRange(range))
            {
                if (m != this && m != ControlMaster && m != SummonMaster && CanBeHarmful(m))
                {
                    DoHarmful(m);
                    m.BoltEffect(1165);
                    AOS.Damage(m, this, Utility.RandomMinMax(damage / 2, damage), 0, 0, 0, 0, 100);
                }
            }
        }

        public SummonedStormElemental(Serial serial) : base(serial)
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

            m_NextStormBurst = DateTime.UtcNow;
        }
    }

    #endregion

    #region Water Elemental

    /// <summary>
    /// Summoned Water Elemental - Healing and support
    /// </summary>
    [CorpseName("a water elemental corpse")]
    public class SummonedWaterElemental : VystiaSummonedPet
    {
        private DateTime m_NextHeal;

        [Constructable]
        public SummonedWaterElemental() : this(VystiaPetTier.Standard)
        {
        }

        public SummonedWaterElemental(VystiaPetTier tier)
            : base(AIType.AI_Healer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = GetTierName(tier) + " Water Elemental";
            Body = 16;
            BaseSoundID = 278;
            Hue = 1154; // Blue

            PetType = VystiaPetType.SummonedElemental;
            PetTier = tier;

            int hp = VystiaPetStats.GetBaseHP(tier);
            var (minDam, maxDam) = VystiaPetStats.GetBaseDamage(tier);
            // Water elementals do less damage
            minDam = (int)(minDam * 0.7);
            maxDam = (int)(maxDam * 0.7);

            SetStr(100 + (int)tier * 15);
            SetDex(100 + (int)tier * 10);
            SetInt(200 + (int)tier * 25);

            SetHits(hp);
            SetDamage(minDam, maxDam);

            SetDamageType(ResistanceType.Cold, 50);
            SetDamageType(ResistanceType.Physical, 50);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 10, 25);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 50, 65);
            SetResistance(ResistanceType.Energy, 30, 45);

            SetSkill(SkillName.EvalInt, 80.0 + (int)tier *10);
            SetSkill(SkillName.Magery, 80.0 + (int)tier *10);
            SetSkill(SkillName.Meditation, 70.0 + (int)tier *10);
            SetSkill(SkillName.MagicResist, 70.0 + (int)tier *10);
            SetSkill(SkillName.Tactics, 40.0 + (int)tier *5);
            SetSkill(SkillName.Wrestling, 40.0 + (int)tier *5);

            Fame = 0;
            Karma = 0;

            ControlSlots = VystiaPetStats.GetControlSlots(tier);

            VirtualArmor = 30 + (int)tier * 5;

            m_NextHeal = DateTime.UtcNow;
        }

        private string GetTierName(VystiaPetTier tier)
        {
            switch (tier)
            {
                case VystiaPetTier.Lesser: return "Lesser";
                case VystiaPetTier.Standard: return "";
                case VystiaPetTier.Greater: return "Greater";
                case VystiaPetTier.Superior: return "Superior";
                case VystiaPetTier.Legendary: return "Tidal";
                default: return "";
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Heal master if injured
            if (DateTime.UtcNow >= m_NextHeal)
            {
                Mobile master = ControlMaster ?? SummonMaster;
                if (master != null && master.Hits < master.HitsMax * 0.75 && InRange(master, 10))
                {
                    DoHealMaster(master);
                    m_NextHeal = DateTime.UtcNow + TimeSpan.FromSeconds(10 - (int)PetTier);
                }
            }
        }

        private void DoHealMaster(Mobile master)
        {
            int healAmount = 15 + (int)PetTier * 10 + Utility.RandomMinMax(5, 15);

            master.Heal(healAmount);
            master.FixedParticles(0x376A, 9, 32, 5005, 1154, 0, EffectLayer.Waist);
            master.PlaySound(0x1F2);

            this.FixedParticles(0x376A, 9, 32, 5005, 1154, 0, EffectLayer.Waist);
        }

        public SummonedWaterElemental(Serial serial) : base(serial)
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

            m_NextHeal = DateTime.UtcNow;
        }
    }

    #endregion
}
