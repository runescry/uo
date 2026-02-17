using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// The Eternal Winter - Legendary Artifact Halberd
    /// Drop: Frost Father (Frozen Halls boss) - 2% chance
    ///
    /// From VYSTIA_EQUIPMENT_GUIDE.md:
    /// - Damage: 25-35
    /// - Specials: Hit Cold Area 50%, Cold Damage 100%
    /// </summary>
    public class TheEternalWinter : Halberd
    {
        public override int LabelNumber => 1; // Uses custom name

        [Constructable]
        public TheEternalWinter() : base()
        {
            Name = "The Eternal Winter";
            Hue = 1152; // Ice blue
            Weight = 12.0;

            // Artifact-level stats
            InitSkillBonuses();
            InitAttributes();
            InitWeaponAttributes();
        }

        private void InitSkillBonuses()
        {
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            SkillBonuses.SetValues(1, SkillName.Swords, 10.0);
        }

        private void InitAttributes()
        {
            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
            Attributes.DefendChance = 10;
            Attributes.Luck = 100;
        }

        private void InitWeaponAttributes()
        {
            WeaponAttributes.HitColdArea = 50;
            WeaponAttributes.ResistColdBonus = 15;

            // Base damage (25-35 as per guide)
            MinDamage = 25;
            MaxDamage = 35;

            // 100% Cold damage
            AosElementDamages.Cold = 100;
            AosElementDamages.Physical = 0;

            Slayer = SlayerName.FlameDousing; // Bonus vs fire creatures
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Artifact\tLegendary"); // ~1_val~: ~2_val~
            list.Add("Forged from the heart of eternal frost");
            list.Add("Dropped by the Frost Father");
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            // 15% chance for special freeze effect
            if (damageable is Mobile defender && Utility.RandomDouble() < 0.15)
            {
                defender.SendMessage(0x480, "The eternal cold seeps into your bones!");
                defender.Freeze(TimeSpan.FromSeconds(2));
                defender.FixedParticles(0x376A, 9, 32, 5030, 1152, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                // Apply cold damage over time
                if (attacker is PlayerMobile pm)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                    {
                        if (defender != null && !defender.Deleted && defender.Alive)
                        {
                            AOS.Damage(defender, attacker, Utility.RandomMinMax(5, 10), 0, 0, 100, 0, 0);
                        }
                    });
                }
            }
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public TheEternalWinter(Serial serial) : base(serial)
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
}
