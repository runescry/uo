using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia wolf corpse")]
    public class VystiaWolf : BaseCreature
    {
        [Constructable]
        public VystiaWolf() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia wolf";
            Body = 23;
            BaseSoundID = 0xE5;

            SetStr(90, 120);
            SetDex(100, 130);
            SetInt(40, 60);

            SetHits(80, 110);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 45.0, 65.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 65.0, 85.0);

            Fame = 2000;
            Karma = 0;
            VirtualArmor = 28;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 65.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);

            if (Utility.RandomDouble() < 0.30)
                PackItem(new Hides(Utility.RandomMinMax(3, 6)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Bleeding bite
            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x22, "The wolf's bite causes bleeding!");
                DoBleedEffect(defender);
            }
        }

        private void DoBleedEffect(Mobile target)
        {
            if (target == null || target.Deleted || !target.Alive)
                return;

            int ticks = 3;
            int bleedDamage = Utility.RandomMinMax(2, 4);

            for (int i = 0; i < ticks; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(2.0 * (i + 1)), () =>
                {
                    if (target != null && !target.Deleted && target.Alive)
                    {
                        target.Damage(bleedDamage, this);
                        target.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
                    }
                });
            }
        }

        public override int Meat => 2;
        public override int Hides => 6;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Canine;

        public VystiaWolf(Serial serial) : base(serial)
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
