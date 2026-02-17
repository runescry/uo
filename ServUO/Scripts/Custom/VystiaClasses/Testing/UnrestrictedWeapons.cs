/*
 * Unrestricted Weapons
 *
 * These are test versions of race-restricted weapons (Gargish, Elven)
 * that override RequiredRace to return null, allowing any race to equip them.
 *
 * Used by [SpawnWeaponChests command for testing.
 */

using System;
using Server;
using Server.Items;

namespace Server.Custom.VystiaClasses.Testing
{
    #region Unrestricted Gargish Swords

    public class UnrestrictedGargishKatana : GargishKatana
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishKatana() : base()
        {
            Name = "Gargish Katana [Unrestricted]";
        }

        public UnrestrictedGargishKatana(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishTalwar : GargishTalwar
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishTalwar() : base()
        {
            Name = "Gargish Talwar [Unrestricted]";
        }

        public UnrestrictedGargishTalwar(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishDaisho : GargishDaisho
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishDaisho() : base()
        {
            Name = "Gargish Daisho [Unrestricted]";
        }

        public UnrestrictedGargishDaisho(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedBloodBlade : BloodBlade
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedBloodBlade() : base()
        {
            Name = "Blood Blade [Unrestricted]";
        }

        public UnrestrictedBloodBlade(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedDreadSword : DreadSword
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedDreadSword() : base()
        {
            Name = "Dread Sword [Unrestricted]";
        }

        public UnrestrictedDreadSword(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGlassSword : GlassSword
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGlassSword() : base()
        {
            Name = "Glass Sword [Unrestricted]";
        }

        public UnrestrictedGlassSword(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Unrestricted Gargish Axes

    public class UnrestrictedGargishAxe : GargishAxe
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishAxe() : base()
        {
            Name = "Gargish Axe [Unrestricted]";
        }

        public UnrestrictedGargishAxe(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishBattleAxe : GargishBattleAxe
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishBattleAxe() : base()
        {
            Name = "Gargish Battle Axe [Unrestricted]";
        }

        public UnrestrictedGargishBattleAxe(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishBardiche : GargishBardiche
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishBardiche() : base()
        {
            Name = "Gargish Bardiche [Unrestricted]";
        }

        public UnrestrictedGargishBardiche(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Unrestricted Gargish Maces

    public class UnrestrictedGargishMaul : GargishMaul
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishMaul() : base()
        {
            Name = "Gargish Maul [Unrestricted]";
        }

        public UnrestrictedGargishMaul(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishWarHammer : GargishWarHammer
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishWarHammer() : base()
        {
            Name = "Gargish War Hammer [Unrestricted]";
        }

        public UnrestrictedGargishWarHammer(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedDiscMace : DiscMace
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedDiscMace() : base()
        {
            Name = "Disc Mace [Unrestricted]";
        }

        public UnrestrictedDiscMace(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Unrestricted Gargish Knives

    public class UnrestrictedGargishDagger : GargishDagger
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishDagger() : base()
        {
            Name = "Gargish Dagger [Unrestricted]";
        }

        public UnrestrictedGargishDagger(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishKryss : GargishKryss
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishKryss() : base()
        {
            Name = "Gargish Kryss [Unrestricted]";
        }

        public UnrestrictedGargishKryss(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Unrestricted Gargish Polearms & Spears

    public class UnrestrictedGargishScythe : GargishScythe
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishScythe() : base()
        {
            Name = "Gargish Scythe [Unrestricted]";
        }

        public UnrestrictedGargishScythe(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishPike : GargishPike
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishPike() : base()
        {
            Name = "Gargish Pike [Unrestricted]";
        }

        public UnrestrictedGargishPike(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishWarFork : GargishWarFork
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishWarFork() : base()
        {
            Name = "Gargish War Fork [Unrestricted]";
        }

        public UnrestrictedGargishWarFork(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedGargishLance : GargishLance
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishLance() : base()
        {
            Name = "Gargish Lance [Unrestricted]";
        }

        public UnrestrictedGargishLance(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Unrestricted Gargish Staves

    public class UnrestrictedGargishGnarledStaff : GargishGnarledStaff
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedGargishGnarledStaff() : base()
        {
            Name = "Gargish Gnarled Staff [Unrestricted]";
        }

        public UnrestrictedGargishGnarledStaff(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Unrestricted Gargish Thrown Weapons

    public class UnrestrictedBoomerang : Boomerang
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedBoomerang() : base()
        {
            Name = "Boomerang [Unrestricted]";
        }

        public UnrestrictedBoomerang(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedCyclone : Cyclone
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedCyclone() : base()
        {
            Name = "Cyclone [Unrestricted]";
        }

        public UnrestrictedCyclone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedSoulGlaive : SoulGlaive
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedSoulGlaive() : base()
        {
            Name = "Soul Glaive [Unrestricted]";
        }

        public UnrestrictedSoulGlaive(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion

    #region Unrestricted Elven Weapons

    public class UnrestrictedElvenSpellblade : ElvenSpellblade
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedElvenSpellblade() : base()
        {
            Name = "Elven Spellblade [Unrestricted]";
        }

        public UnrestrictedElvenSpellblade(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedRadiantScimitar : RadiantScimitar
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedRadiantScimitar() : base()
        {
            Name = "Radiant Scimitar [Unrestricted]";
        }

        public UnrestrictedRadiantScimitar(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedElvenCompositeLongbow : ElvenCompositeLongbow
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedElvenCompositeLongbow() : base()
        {
            Name = "Elven Composite Longbow [Unrestricted]";
        }

        public UnrestrictedElvenCompositeLongbow(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedMagicalShortbow : MagicalShortbow
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedMagicalShortbow() : base()
        {
            Name = "Magical Shortbow [Unrestricted]";
        }

        public UnrestrictedMagicalShortbow(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedLeafblade : Leafblade
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedLeafblade() : base()
        {
            Name = "Leafblade [Unrestricted]";
        }

        public UnrestrictedLeafblade(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedElvenMachete : ElvenMachete
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedElvenMachete() : base()
        {
            Name = "Elven Machete [Unrestricted]";
        }

        public UnrestrictedElvenMachete(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UnrestrictedWildStaff : WildStaff
    {
        public override Race RequiredRace => null;
        public override bool CanBeWornByGargoyles => true;

        [Constructable]
        public UnrestrictedWildStaff() : base()
        {
            Name = "Wild Staff [Unrestricted]";
        }

        public UnrestrictedWildStaff(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    #endregion
}
