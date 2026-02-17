using System;
using Server;

namespace Server.Items.Vystia
{
    // ============================================================================
    // REGIONAL SWORDS
    // ============================================================================

    public class IcicleBlade : Longsword
    {
        [Constructable]
        public IcicleBlade()
        {
            Name = "Icicle Blade";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public IcicleBlade(Serial serial) : base(serial) { }

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

    public class WintersEdge : VikingSword
    {
        [Constructable]
        public WintersEdge()
        {
            Name = "Winter's Edge";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public WintersEdge(Serial serial) : base(serial) { }

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

    public class Frostbite : Cutlass
    {
        [Constructable]
        public Frostbite()
        {
            Name = "Frostbite";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public Frostbite(Serial serial) : base(serial) { }

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

    public class GlacierShard : Kryss
    {
        [Constructable]
        public GlacierShard()
        {
            Name = "Glacier Shard";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public GlacierShard(Serial serial) : base(serial) { }

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

    public class FlameTongue : Katana
    {
        [Constructable]
        public FlameTongue()
        {
            Name = "Flame Tongue";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
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

    public class MagmaBlade : Scimitar
    {
        [Constructable]
        public MagmaBlade()
        {
            Name = "Magma Blade";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
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

    public class PhoenixWing : Broadsword
    {
        [Constructable]
        public PhoenixWing()
        {
            Name = "Phoenix Wing";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
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

    public class LavaEdge : Longsword
    {
        [Constructable]
        public LavaEdge()
        {
            Name = "Lava Edge";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
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

    public class CrystalShard : Kryss
    {
        [Constructable]
        public CrystalShard()
        {
            Name = "Crystal Shard";
            Hue = 1154;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistEnergyBonus = 5;

            // Element damage
            AosElementDamages.Energy = 50;
            AosElementDamages.Physical = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Crystal Crafted");
        }

        public CrystalShard(Serial serial) : base(serial) { }

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

    public class PrismBlade : Longsword
    {
        [Constructable]
        public PrismBlade()
        {
            Name = "Prism Blade";
            Hue = 1154;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistEnergyBonus = 5;

            // Element damage
            AosElementDamages.Energy = 50;
            AosElementDamages.Physical = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Crystal Crafted");
        }

        public PrismBlade(Serial serial) : base(serial) { }

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

    public class RefractionEdge : Katana
    {
        [Constructable]
        public RefractionEdge()
        {
            Name = "Refraction Edge";
            Hue = 1154;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistEnergyBonus = 5;

            // Element damage
            AosElementDamages.Energy = 50;
            AosElementDamages.Physical = 50;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Crystal Crafted");
        }

        public RefractionEdge(Serial serial) : base(serial) { }

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

    public class ClockworkSword : Broadsword
    {
        [Constructable]
        public ClockworkSword()
        {
            Name = "Clockwork Sword";
            Hue = 2401;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.WeaponSpeed = 10;

            // Element damage
            AosElementDamages.Energy = 30;
            AosElementDamages.Physical = 70;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Ironclad Crafted");
        }

        public ClockworkSword(Serial serial) : base(serial) { }

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

    public class GearBlade : VikingSword
    {
        [Constructable]
        public GearBlade()
        {
            Name = "Gear Blade";
            Hue = 2401;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.WeaponSpeed = 10;

            // Element damage
            AosElementDamages.Energy = 30;
            AosElementDamages.Physical = 70;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Ironclad Crafted");
        }

        public GearBlade(Serial serial) : base(serial) { }

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

    public class SteamSaber : Cutlass
    {
        [Constructable]
        public SteamSaber()
        {
            Name = "Steam Saber";
            Hue = 2401;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.WeaponSpeed = 10;

            // Element damage
            AosElementDamages.Energy = 30;
            AosElementDamages.Physical = 70;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Ironclad Crafted");
        }

        public SteamSaber(Serial serial) : base(serial) { }

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

    public class ShadowFang : Kryss
    {
        [Constructable]
        public ShadowFang()
        {
            Name = "Shadow Fang";
            Hue = 1109;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.AttackChance = 5;

            // Element damage
            AosElementDamages.Cold = 40;
            AosElementDamages.Poison = 20;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Shadow Crafted");
        }

        public ShadowFang(Serial serial) : base(serial) { }

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

    public class VoidEdge : Katana
    {
        [Constructable]
        public VoidEdge()
        {
            Name = "Void Edge";
            Hue = 1109;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.AttackChance = 5;

            // Element damage
            AosElementDamages.Cold = 40;
            AosElementDamages.Poison = 20;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Shadow Crafted");
        }

        public VoidEdge(Serial serial) : base(serial) { }

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

    public class DarkBlade : Scimitar
    {
        [Constructable]
        public DarkBlade()
        {
            Name = "Dark Blade";
            Hue = 1109;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.AttackChance = 5;

            // Element damage
            AosElementDamages.Cold = 40;
            AosElementDamages.Poison = 20;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Shadow Crafted");
        }

        public DarkBlade(Serial serial) : base(serial) { }

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

    // ============================================================================
    // REGIONAL AXES
    // ============================================================================

    public class FrozenCleaver : BattleAxe
    {
        [Constructable]
        public FrozenCleaver()
        {
            Name = "Frozen Cleaver";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public FrozenCleaver(Serial serial) : base(serial) { }

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

    public class IceShardAxe : DoubleAxe
    {
        [Constructable]
        public IceShardAxe()
        {
            Name = "Ice Shard Axe";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public IceShardAxe(Serial serial) : base(serial) { }

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

    public class GlacialHatchet : Hatchet
    {
        [Constructable]
        public GlacialHatchet()
        {
            Name = "Glacial Hatchet";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public GlacialHatchet(Serial serial) : base(serial) { }

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

    public class MoltenAxe : TwoHandedAxe
    {
        [Constructable]
        public MoltenAxe()
        {
            Name = "Molten Axe";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
        }

        public MoltenAxe(Serial serial) : base(serial) { }

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

    public class FlameCleaver : WarAxe
    {
        [Constructable]
        public FlameCleaver()
        {
            Name = "Flame Cleaver";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
        }

        public FlameCleaver(Serial serial) : base(serial) { }

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

    public class LavaPick : Pickaxe
    {
        [Constructable]
        public LavaPick()
        {
            Name = "Lava Pick";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
        }

        public LavaPick(Serial serial) : base(serial) { }

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

    public class GearAxe : ExecutionersAxe
    {
        [Constructable]
        public GearAxe()
        {
            Name = "Gear Axe";
            Hue = 2401;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.WeaponSpeed = 10;

            // Element damage
            AosElementDamages.Energy = 30;
            AosElementDamages.Physical = 70;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Ironclad Crafted");
        }

        public GearAxe(Serial serial) : base(serial) { }

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

    public class SteamCleaver : BattleAxe
    {
        [Constructable]
        public SteamCleaver()
        {
            Name = "Steam Cleaver";
            Hue = 2401;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.WeaponSpeed = 10;

            // Element damage
            AosElementDamages.Energy = 30;
            AosElementDamages.Physical = 70;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Ironclad Crafted");
        }

        public SteamCleaver(Serial serial) : base(serial) { }

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

    // ============================================================================
    // REGIONAL MACES
    // ============================================================================

    public class GlacialHammer : WarHammer
    {
        [Constructable]
        public GlacialHammer()
        {
            Name = "Glacial Hammer";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public GlacialHammer(Serial serial) : base(serial) { }

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

    public class FrostMaul : Maul
    {
        [Constructable]
        public FrostMaul()
        {
            Name = "Frost Maul";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public FrostMaul(Serial serial) : base(serial) { }

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

    public class IceClub : Club
    {
        [Constructable]
        public IceClub()
        {
            Name = "Ice Club";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public IceClub(Serial serial) : base(serial) { }

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

    public class MoltenMace : WarMace
    {
        [Constructable]
        public MoltenMace()
        {
            Name = "Molten Mace";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
        }

        public MoltenMace(Serial serial) : base(serial) { }

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

    public class MagmaHammer : WarHammer
    {
        [Constructable]
        public MagmaHammer()
        {
            Name = "Magma Hammer";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
        }

        public MagmaHammer(Serial serial) : base(serial) { }

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

    public class PistonMace : WarMace
    {
        [Constructable]
        public PistonMace()
        {
            Name = "Piston Mace";
            Hue = 2401;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.WeaponSpeed = 10;

            // Element damage
            AosElementDamages.Energy = 30;
            AosElementDamages.Physical = 70;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Ironclad Crafted");
        }

        public PistonMace(Serial serial) : base(serial) { }

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

    public class SteamHammer : WarHammer
    {
        [Constructable]
        public SteamHammer()
        {
            Name = "Steam Hammer";
            Hue = 2401;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.WeaponSpeed = 10;

            // Element damage
            AosElementDamages.Energy = 30;
            AosElementDamages.Physical = 70;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Ironclad Crafted");
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

    // ============================================================================
    // REGIONAL POLEARMS
    // ============================================================================

    public class IceLance : Pike
    {
        [Constructable]
        public IceLance()
        {
            Name = "Ice Lance";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public IceLance(Serial serial) : base(serial) { }

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

    public class FrozenHalberd : Halberd
    {
        [Constructable]
        public FrozenHalberd()
        {
            Name = "Frozen Halberd";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public FrozenHalberd(Serial serial) : base(serial) { }

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

    public class LavaSpear : Spear
    {
        [Constructable]
        public LavaSpear()
        {
            Name = "Lava Spear";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
        }

        public LavaSpear(Serial serial) : base(serial) { }

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

    public class VolcanicPike : Pike
    {
        [Constructable]
        public VolcanicPike()
        {
            Name = "Volcanic Pike";
            Hue = 1358;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistFireBonus = 5;

            // Element damage
            AosElementDamages.Fire = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Emberlands Crafted");
        }

        public VolcanicPike(Serial serial) : base(serial) { }

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

    // ============================================================================
    // REGIONAL RANGED WEAPONS
    // ============================================================================

    public class FrostBow : Bow
    {
        [Constructable]
        public FrostBow()
        {
            Name = "Frost Bow";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public FrostBow(Serial serial) : base(serial) { }

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

    public class IceCrossbow : Crossbow
    {
        [Constructable]
        public IceCrossbow()
        {
            Name = "Ice Crossbow";
            Hue = 1152;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            WeaponAttributes.ResistColdBonus = 5;

            // Element damage
            AosElementDamages.Cold = 60;
            AosElementDamages.Physical = 40;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Frosthold Crafted");
        }

        public IceCrossbow(Serial serial) : base(serial) { }

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

    public class LivingBow : Bow
    {
        [Constructable]
        public LivingBow()
        {
            Name = "Living Bow";
            Hue = 2010;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.RegenHits = 1;

            // Element damage
            AosElementDamages.Poison = 40;
            AosElementDamages.Physical = 60;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Verdantpeak Crafted");
        }

        public LivingBow(Serial serial) : base(serial) { }

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

    public class NaturesCrossbow : Crossbow
    {
        [Constructable]
        public NaturesCrossbow()
        {
            Name = "Nature's Crossbow";
            Hue = 2010;

            // Regional weapon stats
            Attributes.WeaponDamage = 20; // +20% damage
            Attributes.RegenHits = 1;

            // Element damage
            AosElementDamages.Poison = 40;
            AosElementDamages.Physical = 60;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Verdantpeak Crafted");
        }

        public NaturesCrossbow(Serial serial) : base(serial) { }

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

    // ============================================================================
    // CLASS-SPECIFIC STARTING WEAPONS
    // ============================================================================

    public class VystiaLance : Lance
    {
        [Constructable]
        public VystiaLance()
        {
            Name = "Holy Lance";
            Hue = 1153; // Paladin hue

            // Starting weapon stats
            Attributes.WeaponDamage = 10;
            WeaponAttributes.HitLightning = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Paladin Starting Weapon");
        }

        public VystiaLance(Serial serial) : base(serial) { }

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

    public class VystiaScythe : Scythe
    {
        [Constructable]
        public VystiaScythe()
        {
            Name = "Soul Reaper";
            Hue = 1109; // Necromancer hue

            // Starting weapon stats
            Attributes.WeaponDamage = 10;
            WeaponAttributes.HitLowerDefend = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Necromancer Starting Weapon");
        }

        public VystiaScythe(Serial serial) : base(serial) { }

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

    public class VystiaYumi : Yumi
    {
        [Constructable]
        public VystiaYumi()
        {
            Name = "Desert Longbow";
            Hue = 1719; // Ranger hue

            // Starting weapon stats
            Attributes.WeaponDamage = 10;
            WeaponAttributes.HitLightning = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Ranger Starting Weapon");
        }

        public VystiaYumi(Serial serial) : base(serial) { }

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

    public class VystiaWildStaff : WildStaff
    {
        [Constructable]
        public VystiaWildStaff()
        {
            Name = "Verdant Staff";
            Hue = 2010; // Druid hue

            // Starting weapon stats
            Attributes.WeaponDamage = 10;
            WeaponAttributes.HitLightning = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Druid Starting Weapon");
        }

        public VystiaWildStaff(Serial serial) : base(serial) { }

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

}
