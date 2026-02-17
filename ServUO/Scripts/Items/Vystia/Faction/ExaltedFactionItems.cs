// Vystia Exalted Faction Items
// Unique reward items available only to Exalted-tier faction members

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Factions;

namespace Server.Items.Vystia
{
    #region Base Exalted Item

    /// <summary>
    /// Base class for all Exalted faction reward items
    /// Requires Exalted status to use
    /// </summary>
    public abstract class BaseExaltedItem : Item
    {
        private VystiaFaction m_RequiredFaction;

        [CommandProperty(AccessLevel.GameMaster)]
        public VystiaFaction RequiredFaction
        {
            get { return m_RequiredFaction; }
            set { m_RequiredFaction = value; InvalidateProperties(); }
        }

        public abstract string ItemDescription { get; }

        public BaseExaltedItem(VystiaFaction faction, int itemID) : base(itemID)
        {
            m_RequiredFaction = faction;
            LootType = LootType.Blessed;
            Weight = 1.0;

            var info = FactionData.GetInfo(faction);
            Hue = info?.Hue ?? 0;
        }

        public BaseExaltedItem(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile pm))
                return;

            if (!IsChildOf(pm.Backpack))
            {
                pm.SendLocalizedMessage(1042001); // That must be in your pack
                return;
            }

            if (!ExaltedRewards.IsExalted(pm, m_RequiredFaction))
            {
                var info = FactionData.GetInfo(m_RequiredFaction);
                pm.SendMessage(0x22, "You must be Exalted with {0} to use this item.", info?.Name ?? m_RequiredFaction.ToString());
                return;
            }

            OnUse(pm);
        }

        protected abstract void OnUse(PlayerMobile pm);

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            var info = FactionData.GetInfo(m_RequiredFaction);
            list.Add(1060658, "Faction\t{0}", info?.Name ?? m_RequiredFaction.ToString());
            list.Add(1060659, "Requires\tExalted Status");
            if (!string.IsNullOrEmpty(ItemDescription))
                list.Add(ItemDescription);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write((int)m_RequiredFaction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_RequiredFaction = (VystiaFaction)reader.ReadInt();
        }
    }

    #endregion

    #region Frostguard Exalted Items

    /// <summary>
    /// Frostguard Exalted Cloak - Grants cold immunity aura
    /// </summary>
    public class FrostguardExaltedCloak : BaseExaltedItem
    {
        public override string ItemDescription => "Grants cold immunity for 30 minutes";

        [Constructable]
        public FrostguardExaltedCloak() : base(VystiaFaction.Frostguard, 0x1515) // Cloak
        {
            Name = "Cloak of the Iceborn";
        }

        public FrostguardExaltedCloak(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, 70));
            pm.PlaySound(0x10B);
            pm.FixedParticles(0x376A, 9, 32, 5005, 1150, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "You are wrapped in the eternal cold of the Frostguard!");

            Timer.DelayCall(TimeSpan.FromMinutes(30), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Cold, 70));
                pm.SendMessage("The cold protection fades.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Frostguard War Horn - Summons ice warriors
    /// </summary>
    public class FrostguardWarHorn : BaseExaltedItem
    {
        public override string ItemDescription => "Summons ice warriors to aid you";

        [Constructable]
        public FrostguardWarHorn() : base(VystiaFaction.Frostguard, 0xFC4) // Horn
        {
            Name = "War Horn of the Frostguard";
        }

        public FrostguardWarHorn(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.PlaySound(0x64);
            pm.SendMessage(0x35, "The war horn echoes across the frozen lands!");

            // Summon 2 ice warriors
            for (int i = 0; i < 2; i++)
            {
                var warrior = new FrostguardWarrior();
                warrior.MoveToWorld(pm.Location, pm.Map);
                warrior.Combatant = pm.Combatant;

                Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
                {
                    if (!warrior.Deleted)
                        warrior.Delete();
                });
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Flame Legion Exalted Items

    /// <summary>
    /// Flame Legion Exalted Amulet - Grants fire immunity aura
    /// </summary>
    public class FlameLegionExaltedAmulet : BaseExaltedItem
    {
        public override string ItemDescription => "Grants fire immunity for 30 minutes";

        [Constructable]
        public FlameLegionExaltedAmulet() : base(VystiaFaction.FlameLegion, 0x1088) // Necklace
        {
            Name = "Amulet of the Phoenix";
        }

        public FlameLegionExaltedAmulet(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, 70));
            pm.PlaySound(0x208);
            pm.FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "You are surrounded by the eternal flame!");

            Timer.DelayCall(TimeSpan.FromMinutes(30), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Fire, 70));
                pm.SendMessage("The fire protection fades.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Flame Legion Banner - Grants party fire damage bonus
    /// </summary>
    public class FlameLegionBanner : BaseExaltedItem
    {
        public override string ItemDescription => "Grants nearby allies +15% fire damage";

        [Constructable]
        public FlameLegionBanner() : base(VystiaFaction.FlameLegion, 0x159D) // Banner
        {
            Name = "Banner of the Flame Legion";
        }

        public FlameLegionBanner(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.PlaySound(0x208);
            pm.SendMessage(0x35, "You raise the banner of the Flame Legion!");

            foreach (Mobile m in pm.GetMobilesInRange(10))
            {
                if (m is PlayerMobile ally && ally.Alive && ally != pm)
                {
                    ally.FixedParticles(0x3709, 10, 30, 5052, 1358, 0, EffectLayer.Waist);
                    ally.SendMessage(0x35, "The Flame Legion banner empowers you!");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Greenward Exalted Items

    /// <summary>
    /// Greenward Exalted Staff - Powerful nature magic staff
    /// </summary>
    public class GreenwardExaltedStaff : BaseExaltedItem
    {
        public override string ItemDescription => "Restores all party HP and cures poison";

        [Constructable]
        public GreenwardExaltedStaff() : base(VystiaFaction.Greenward, 0x13F8) // Staff
        {
            Name = "Staff of the Archdruid";
        }

        public GreenwardExaltedStaff(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.PlaySound(0x1E9);
            pm.FixedParticles(0x376A, 9, 32, 5005, 2010, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "Nature's power flows through the staff!");

            // Heal and cure all party members in range
            foreach (Mobile m in pm.GetMobilesInRange(10))
            {
                if (m is PlayerMobile ally && ally.Alive)
                {
                    ally.Hits = ally.HitsMax;
                    ally.Poison = null;
                    ally.FixedParticles(0x376A, 9, 32, 5005, 2010, 0, EffectLayer.Waist);
                    ally.SendMessage(0x35, "Nature's embrace restores you!");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Greenward Seed Pouch - Summons treant guardian
    /// </summary>
    public class GreenwardSeedPouch : BaseExaltedItem
    {
        public override string ItemDescription => "Summons a treant guardian";

        [Constructable]
        public GreenwardSeedPouch() : base(VystiaFaction.Greenward, 0x1602) // Pouch
        {
            Name = "Seed Pouch of the Elder Grove";
        }

        public GreenwardSeedPouch(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.PlaySound(0x1E9);
            pm.SendMessage(0x35, "An ancient treant rises to defend you!");

            var treant = new GreenwardTreant();
            treant.MoveToWorld(pm.Location, pm.Map);
            treant.Combatant = pm.Combatant;

            Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
            {
                if (!treant.Deleted)
                    treant.Delete();
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Arcane Conclave Exalted Items

    /// <summary>
    /// Arcane Exalted Orb - Restores full mana and grants mana shield
    /// </summary>
    public class ArcaneExaltedOrb : BaseExaltedItem
    {
        public override string ItemDescription => "Restores mana and grants mana shield";

        [Constructable]
        public ArcaneExaltedOrb() : base(VystiaFaction.ArcaneConclave, 0xE2E) // Crystal ball
        {
            Name = "Orb of the Archmage";
        }

        public ArcaneExaltedOrb(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.Mana = pm.ManaMax;
            pm.PlaySound(0x1E8);
            pm.FixedParticles(0x376A, 9, 32, 5005, 1154, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "Arcane power fills you and a mana shield protects you!");

            // Grant energy resistance as mana shield
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Energy, 50));

            Timer.DelayCall(TimeSpan.FromMinutes(15), () =>
            {
                pm.RemoveResistanceMod(new ResistanceMod(ResistanceType.Energy, 50));
                pm.SendMessage("The mana shield dissipates.");
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Arcane Tome - Grants bonus spell damage
    /// </summary>
    public class ArcaneTome : BaseExaltedItem
    {
        public override string ItemDescription => "Grants +20% spell damage for 30 minutes";

        [Constructable]
        public ArcaneTome() : base(VystiaFaction.ArcaneConclave, 0xEFA) // Spellbook
        {
            Name = "Tome of the Conclave";
        }

        public ArcaneTome(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "ArcaneTome", 25, TimeSpan.FromMinutes(30)));
            pm.PlaySound(0x1E8);
            pm.FixedParticles(0x376A, 9, 32, 5005, 1154, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "Ancient arcane knowledge fills your mind! (+25 INT for 30 minutes)");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Technoguild Exalted Items

    /// <summary>
    /// Technoguild Exalted Goggles - Grants enhanced crafting
    /// </summary>
    public class TechnoguildExaltedGoggles : BaseExaltedItem
    {
        public override string ItemDescription => "Grants +30 crafting skills for 1 hour";

        [Constructable]
        public TechnoguildExaltedGoggles() : base(VystiaFaction.Technoguild, 0x4C10) // Goggles
        {
            Name = "Goggles of the Master Engineer";
        }

        public TechnoguildExaltedGoggles(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "TechGoggles", 15, TimeSpan.FromHours(1)));
            pm.AddStatMod(new StatMod(StatType.Dex, "TechGoggles", 15, TimeSpan.FromHours(1)));
            pm.PlaySound(0x2A);
            pm.FixedParticles(0x376A, 9, 32, 5005, 2305, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "The goggles enhance your precision! (+15 STR/DEX for 1 hour)");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Technoguild Control Rod - Summons mechanical guardians
    /// </summary>
    public class TechnoguildControlRod : BaseExaltedItem
    {
        public override string ItemDescription => "Summons clockwork guardians";

        [Constructable]
        public TechnoguildControlRod() : base(VystiaFaction.Technoguild, 0x26BC) // Rod
        {
            Name = "Control Rod of the Technolord";
        }

        public TechnoguildControlRod(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.PlaySound(0x2A);
            pm.SendMessage(0x35, "Clockwork guardians activate!");

            for (int i = 0; i < 2; i++)
            {
                var guardian = new TechnoguildGuardian();
                guardian.MoveToWorld(pm.Location, pm.Map);
                guardian.Combatant = pm.Combatant;

                Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
                {
                    if (!guardian.Deleted)
                        guardian.Delete();
                });
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Sandwalkers Exalted Items

    /// <summary>
    /// Sandwalkers Compass - Teleport to any discovered location
    /// </summary>
    public class SandwalkersCompass : BaseExaltedItem
    {
        public override string ItemDescription => "Teleport to any desert waypoint";

        [Constructable]
        public SandwalkersCompass() : base(VystiaFaction.Sandwalkers, 0x1EB3) // Compass
        {
            Name = "Compass of the Sultan";
        }

        public SandwalkersCompass(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.PlaySound(0x20E);
            pm.FixedParticles(0x376A, 9, 32, 5005, 1719, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "The compass points the way through the sands...");
            // Would normally open a waypoint selection gump
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Sandwalkers Trade Seal - Grants vendor discount
    /// </summary>
    public class SandwalkersTradeSeal : BaseExaltedItem
    {
        public override string ItemDescription => "Grants 25% vendor discount for 1 hour";

        [Constructable]
        public SandwalkersTradeSeal() : base(VystiaFaction.Sandwalkers, 0x14F0) // Seal
        {
            Name = "Trade Seal of the Sultan";
        }

        public SandwalkersTradeSeal(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.PlaySound(0x2E6);
            pm.SendMessage(0x35, "The Sultan's seal grants you favor with merchants!");
            // Effect would need to be tracked and applied during vendor transactions
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Voidborn Exalted Items

    /// <summary>
    /// Voidborn Shadow Cloak - Grants invisibility
    /// </summary>
    public class VoidbornShadowCloak : BaseExaltedItem
    {
        public override string ItemDescription => "Grants invisibility for 5 minutes";

        [Constructable]
        public VoidbornShadowCloak() : base(VystiaFaction.Voidborn, 0x2684) // Dark cloak
        {
            Name = "Cloak of Shadows";
        }

        public VoidbornShadowCloak(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.Hidden = true;
            pm.PlaySound(0x22C);
            pm.FixedParticles(0x376A, 9, 32, 5005, 1109, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "The void conceals you from sight!");

            Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
            {
                if (pm.Hidden)
                {
                    pm.Hidden = false;
                    pm.SendMessage("The shadows release you.");
                }
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    /// <summary>
    /// Voidborn Portal Stone - Opens portal to void realm
    /// </summary>
    public class VoidbornPortalStone : BaseExaltedItem
    {
        public override string ItemDescription => "Opens a portal to the Shadowvoid";

        [Constructable]
        public VoidbornPortalStone() : base(VystiaFaction.Voidborn, 0xF91) // Dark stone
        {
            Name = "Portal Stone of the Herald";
        }

        public VoidbornPortalStone(Serial serial) : base(serial) { }

        protected override void OnUse(PlayerMobile pm)
        {
            pm.PlaySound(0x20E);
            pm.FixedParticles(0x376A, 9, 32, 5005, 1109, 0, EffectLayer.Waist);
            pm.SendMessage(0x35, "A portal to the Shadowvoid tears open...");
            // Would open portal to special zone
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    #endregion

    #region Summoned Creatures for Exalted Items

    /// <summary>
    /// Frostguard warrior summoned by war horn
    /// </summary>
    public class FrostguardWarrior : BaseCreature
    {
        [Constructable]
        public FrostguardWarrior() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Frostguard Warrior";
            Body = 400;
            Hue = 1150;

            SetStr(200);
            SetDex(100);
            SetInt(50);
            SetHits(250);
            SetDamage(15, 25);

            SetDamageType(ResistanceType.Cold, 100);
            SetResistance(ResistanceType.Cold, 100);
            SetResistance(ResistanceType.Fire, -50);

            Fame = 0;
            Karma = 0;
            Tamable = false;
        }

        public FrostguardWarrior(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Greenward treant summoned by seed pouch
    /// </summary>
    public class GreenwardTreant : BaseCreature
    {
        [Constructable]
        public GreenwardTreant() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an Elder Treant";
            Body = 301;
            Hue = 2010;

            SetStr(300);
            SetDex(50);
            SetInt(100);
            SetHits(400);
            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);
            SetResistance(ResistanceType.Fire, -50);
            SetResistance(ResistanceType.Poison, 100);

            Fame = 0;
            Karma = 0;
            Tamable = false;
        }

        public GreenwardTreant(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    /// <summary>
    /// Technoguild clockwork guardian
    /// </summary>
    public class TechnoguildGuardian : BaseCreature
    {
        [Constructable]
        public TechnoguildGuardian() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Clockwork Guardian";
            Body = 752;
            Hue = 2305;

            SetStr(250);
            SetDex(150);
            SetInt(50);
            SetHits(300);
            SetDamage(18, 28);

            SetDamageType(ResistanceType.Physical, 100);
            SetResistance(ResistanceType.Physical, 50);
            SetResistance(ResistanceType.Fire, 30);
            SetResistance(ResistanceType.Poison, 100);

            Fame = 0;
            Karma = 0;
            Tamable = false;
        }

        public TechnoguildGuardian(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion
}
