using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a nature elemental corpse")]
    public class NatureElemental : BaseCreature
    {
        [Constructable]
        public NatureElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a nature elemental";
            Body = 161;
            Hue = 2010;
            BaseSoundID = 268;

            SetStr(200, 250);
            SetDex(90, 110);
            SetInt(150, 200);

            SetHits(180, 230);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Poison, 60);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 7000;
            Karma = -7000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);

            PackItem(new LivingOre(Utility.RandomMinMax(3, 6)));
            PackItem(new NatureforgedIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new LivingOre());

            if (Utility.RandomDouble() < 0.08)
                PackItem(new TreantHeart());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x3B2, "Nature's energy flows through you, draining your vitality!");
                defender.FixedParticles(0x376A, 9, 32, 5030, 2010, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(10, 18);
                AOS.Damage(defender, this, damage, 0, 0, 0, 100, 0);

                // Heal self
                int heal = damage / 2;
                Hits = Math.Min(Hits + heal, HitsMax);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Healing aura
            if (Utility.RandomDouble() < 0.02 && Hits < HitsMax)
            {
                int heal = Utility.RandomMinMax(15, 30);
                Hits = Math.Min(Hits + heal, HitsMax);
                FixedParticles(0x376A, 9, 32, 5005, 2010, 0, EffectLayer.Waist);
                Say("*The nature elemental absorbs life from the forest*");
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public NatureElemental(Serial serial) : base(serial)
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
