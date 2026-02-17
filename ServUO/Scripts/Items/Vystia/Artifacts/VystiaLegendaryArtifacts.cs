using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    #region Heartwood Core - Ancient Treant Drop

    /// <summary>
    /// Heartwood Core - Legendary Artifact
    /// The mythical source of the forest's vitality and magic
    /// Drop: Ancient Treant (Verdantpeak boss) - 1% chance
    /// </summary>
    public class HeartwoodCore : Item
    {
        private DateTime m_NextSummon;

        [Constructable]
        public HeartwoodCore() : base(0x1F1C) // Heart-shaped graphic
        {
            Name = "Heartwood Core";
            Hue = 2010; // Forest Green
            Weight = 5.0;
            LootType = LootType.Blessed;

            m_NextSummon = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary");
            list.Add("The heart of the ancient forest");
            list.Add("Dropped by the Ancient Treant");
            list.Add(1060659, "+2 HP Regeneration when held");
            list.Add("Can summon a Treant Guardian once per day");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            if (DateTime.UtcNow < m_NextSummon)
            {
                TimeSpan remaining = m_NextSummon - DateTime.UtcNow;
                from.SendMessage(0x3B2, "You must wait {0} minutes before summoning again.", (int)remaining.TotalMinutes + 1);
                return;
            }

            // Summon a Treant Guardian
            BaseCreature treant = new Reaper();
            treant.Name = "a treant guardian";
            treant.Hue = 2010;
            treant.SetControlMaster(from);
            treant.Controlled = true;
            treant.ControlOrder = OrderType.Guard;

            Point3D loc = from.Location;
            treant.MoveToWorld(loc, from.Map);

            from.SendMessage(0x3B2, "A treant guardian rises to protect you!");
            from.FixedParticles(0x376A, 9, 32, 5030, 2010, 0, EffectLayer.Waist);
            from.PlaySound(0x1F7);

            m_NextSummon = DateTime.UtcNow + TimeSpan.FromHours(24);

            // Treant lasts 10 minutes
            Timer.DelayCall(TimeSpan.FromMinutes(10), () =>
            {
                if (treant != null && !treant.Deleted)
                {
                    treant.Say("*The guardian returns to the forest*");
                    treant.Delete();
                }
            });
        }

        public HeartwoodCore(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_NextSummon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_NextSummon = reader.ReadDateTime();
        }
    }

    #endregion

    #region Magma Heart - Volcano Wyrm Drop

    /// <summary>
    /// Magma Heart - Legendary Artifact
    /// A legendary forge component powered by natural lava
    /// Drop: Volcano Wyrm (Emberlands boss) - 1% chance
    /// </summary>
    public class MagmaHeart : Item
    {
        [Constructable]
        public MagmaHeart() : base(0x1F19) // Ore/heart graphic
        {
            Name = "Magma Heart";
            Hue = 1358; // Molten Orange
            Weight = 10.0;
            LootType = LootType.Blessed;
            Light = LightType.Circle300;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary");
            list.Add("The burning heart of the volcano");
            list.Add("Dropped by the Volcano Wyrm");
            list.Add("+50% Smithing Success when in pack");
            list.Add("Provides infinite forge fuel nearby");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            from.SendMessage(0x22, "The Magma Heart pulses with intense heat. Keep it in your pack while smithing for a bonus.");
            from.FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Waist);
            from.PlaySound(0x208);
        }

        public MagmaHeart(Serial serial) : base(serial) { }

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

    #region Luminous Scepter - Crystal Drake Alpha Drop

    /// <summary>
    /// Luminous Scepter - Legendary Artifact Weapon
    /// Harnesses the sun's power to unprecedented degree
    /// Drop: Crystal Drake Alpha (Crystal Barrens boss) - 1% chance
    /// </summary>
    public class LuminousScepter : Scepter
    {
        private DateTime m_NextHeal;

        [Constructable]
        public LuminousScepter() : base()
        {
            Name = "Luminous Scepter";
            Hue = 1153; // Bright Gold
            Weight = 8.0;
            Light = LightType.Circle300;

            // Artifact-level stats
            SkillBonuses.SetValues(0, SkillName.Magery, 15.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 15.0);

            Attributes.SpellDamage = 25;
            Attributes.CastSpeed = 2;
            Attributes.CastRecovery = 3;
            Attributes.LowerManaCost = 15;
            Attributes.LowerRegCost = 20;
            Attributes.Luck = 150;
            Attributes.BonusInt = 10;

            WeaponAttributes.MageWeapon = 25;
            WeaponAttributes.HitLightning = 50;
            WeaponAttributes.ResistEnergyBonus = 15;

            MinDamage = 15;
            MaxDamage = 25;

            // 50% Physical, 50% Energy
            AosElementDamages.Physical = 50;
            AosElementDamages.Energy = 50;

            Slayer = SlayerName.Exorcism; // Bonus vs undead

            m_NextHeal = DateTime.UtcNow;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Harnesses the power of the sun");
            list.Add("Dropped by the Crystal Drake Alpha");
            list.Add("Can cast Greater Heal once per day");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack) && Parent != from)
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            if (DateTime.UtcNow < m_NextHeal)
            {
                TimeSpan remaining = m_NextHeal - DateTime.UtcNow;
                from.SendMessage(0x35, "You must wait {0} hours before using the healing power again.", (int)remaining.TotalHours + 1);
                return;
            }

            from.Target = new LuminousHealTarget(this);
            from.SendMessage(0x35, "Select a target to heal with the Scepter's light.");
        }

        public void DoHeal(Mobile from, Mobile target)
        {
            int heal = Utility.RandomMinMax(60, 100);
            target.Heal(heal);

            target.FixedParticles(0x376A, 9, 32, 5030, 1153, 0, EffectLayer.Waist);
            target.PlaySound(0x202);

            if (target == from)
                from.SendMessage(0x35, "The luminous light restores {0} health!", heal);
            else
            {
                from.SendMessage(0x35, "You heal {0} for {1} health!", target.Name, heal);
                target.SendMessage(0x35, "{0} heals you with luminous light for {1} health!", from.Name, heal);
            }

            m_NextHeal = DateTime.UtcNow + TimeSpan.FromHours(24);
        }

        private class LuminousHealTarget : Target
        {
            private LuminousScepter m_Scepter;

            public LuminousHealTarget(LuminousScepter scepter) : base(10, false, TargetFlags.Beneficial)
            {
                m_Scepter = scepter;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile target)
                {
                    if (target.Alive)
                        m_Scepter.DoHeal(from, target);
                    else
                        from.SendMessage("That target is dead.");
                }
            }
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public LuminousScepter(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_NextHeal);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_NextHeal = reader.ReadDateTime();
        }
    }

    #endregion

    #region Mirror of Truth - Timeworn Lich Drop

    /// <summary>
    /// Mirror of Truth - Legendary Artifact
    /// Reveals deepest desires and fears of those who look into it
    /// Drop: Timeworn Lich (Shadow Void boss) - 1% chance
    /// </summary>
    public class MirrorOfTruth : Item
    {
        [Constructable]
        public MirrorOfTruth() : base(0x1F17) // Mirror/reflective graphic
        {
            Name = "Mirror of Truth";
            Hue = 1109; // Shadow Purple
            Weight = 3.0;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary");
            list.Add("Reveals what is hidden");
            list.Add("Dropped by the Timeworn Lich");
            list.Add("Use to reveal hidden players nearby");
            list.Add("Costs 5 HP to use");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            if (from.Hits <= 5)
            {
                from.SendMessage(0x22, "You are too weak to peer into the mirror.");
                return;
            }

            // Cost of truth
            from.Hits -= 5;
            from.SendMessage(0x455, "The mirror drains your vitality as it reveals the truth...");

            from.FixedParticles(0x376A, 9, 32, 5030, 1109, 0, EffectLayer.Waist);
            from.PlaySound(0x1F4);

            int revealed = 0;

            foreach (Mobile m in from.GetMobilesInRange(10))
            {
                if (m != from && m.Hidden)
                {
                    m.RevealingAction();
                    m.SendMessage(0x455, "The Mirror of Truth reveals your presence!");
                    m.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Head);
                    revealed++;
                }
            }

            if (revealed > 0)
                from.SendMessage(0x35, "The mirror reveals {0} hidden presence(s)!", revealed);
            else
                from.SendMessage(0x35, "The mirror reveals nothing hidden nearby.");

            // Show alignment of target
            from.Target = new TruthTarget();
            from.SendMessage(0x35, "Select a target to reveal their true nature.");
        }

        private class TruthTarget : Target
        {
            public TruthTarget() : base(10, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile target)
                {
                    string alignment;
                    if (target.Karma > 2500)
                        alignment = "radiates with noble light";
                    else if (target.Karma > 0)
                        alignment = "has a good heart";
                    else if (target.Karma == 0)
                        alignment = "is neutral in spirit";
                    else if (target.Karma > -2500)
                        alignment = "has darkness in their heart";
                    else
                        alignment = "is shrouded in evil";

                    from.SendMessage(0x455, "{0} {1}.", target.Name, alignment);

                    if (target is BaseCreature bc)
                    {
                        if (bc.Controlled)
                            from.SendMessage(0x35, "This creature serves a master.");
                        else if (bc.Summoned)
                            from.SendMessage(0x35, "This creature is summoned.");
                    }
                }
            }
        }

        public MirrorOfTruth(Serial serial) : base(serial) { }

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
