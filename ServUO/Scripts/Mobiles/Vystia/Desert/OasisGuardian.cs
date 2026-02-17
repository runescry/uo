using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an oasis guardian corpse")]
    public class OasisGuardian : BaseCreature
    {
        [Constructable]
        public OasisGuardian() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an oasis guardian";
            Body = 161;
            Hue = 1719;
            BaseSoundID = 268;

            SetStr(220, 280);
            SetDex(90, 110);
            SetInt(150, 200);

            SetHits(200, 260);
            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 30);
            SetDamageType(ResistanceType.Cold, 30);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);

            PackItem(new SandstoneOre(Utility.RandomMinMax(3, 6)));
            PackItem(new SunforgedIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new SandstoneOre(Utility.RandomMinMax(1, 2)));

            // REMOVED OLD REAGENT:
            // if (Utility.RandomDouble() < 0.08)
            //     PackItem(new EmberBloom());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new SunforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x5D, "The oasis guardian's elemental touch burns!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1719, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(10, 18);
                AOS.Damage(defender, this, damage, 0, 50, 0, 0, 50);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Healing aura for self
            if (Utility.RandomDouble() < 0.03 && Hits < HitsMax)
            {
                int heal = Utility.RandomMinMax(10, 25);
                Hits = Math.Min(Hits + heal, HitsMax);
                FixedParticles(0x376A, 9, 32, 5030, 37, 0, EffectLayer.Waist);
                Say("*The oasis guardian draws upon life force*");
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public OasisGuardian(Serial serial) : base(serial)
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
