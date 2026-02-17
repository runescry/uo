using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a coral golem corpse")]
    public class CoralGolem : BaseCreature
    {
        [Constructable]
        public CoralGolem() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a coral golem";
            Body = 752;
            Hue = 1365;
            BaseSoundID = 541;

            SetStr(260, 320);
            SetDex(70, 90);
            SetInt(50, 70);

            SetHits(260, 320);
            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Cold, 30);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 75.0, 95.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 8000;
            Karma = -8000;
            VirtualArmor = 52;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(5, 10)));
            PackItem(new VoidforgedIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.12)
                PackItem(new ObsidianOre());

            if (Utility.RandomDouble() < 0.06)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The coral golem's jagged edges cut deep!");
                defender.FixedParticles(0x377A, 244, 25, 9950, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E1);

                // Bleeding from coral cuts
                DoBleedEffect(defender);
            }
        }

        private void DoBleedEffect(Mobile target)
        {
            if (target == null || target.Deleted || !target.Alive)
                return;

            target.SendMessage(0x22, "You are bleeding from coral cuts!");

            int ticks = 4;
            int bleedDamage = Utility.RandomMinMax(4, 7);

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

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Coral shard explosion when damaged
            if (from != null && Utility.RandomDouble() < 0.15)
            {
                from.SendMessage(0x480, "Coral shards explode from the golem!");
                from.FixedParticles(0x36BD, 20, 10, 5044, 1365, 0, EffectLayer.Head);
                from.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(from, this, damage, 100, 0, 0, 0, 0);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public CoralGolem(Serial serial) : base(serial)
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
