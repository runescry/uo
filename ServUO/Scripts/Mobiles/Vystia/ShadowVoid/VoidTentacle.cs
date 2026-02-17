using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a void tentacle remains")]
    public class VoidTentacle : BaseCreature
    {
        [Constructable]
        public VoidTentacle() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a void tentacle";
            Body = 0x11;
            Hue = 1109;
            BaseSoundID = 352;

            SetStr(150, 200);
            SetDex(80, 100);
            SetInt(50, 70);

            SetHits(160, 210);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 75.0, 95.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 38;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            PackItem(new ObsidianOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.08)
                PackItem(new ShadowforgedIngot());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new VoidDust());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.40)
            {
                defender.SendMessage(0x480, "The void tentacle constricts you!");
                defender.FixedParticles(0x376A, 9, 32, 5007, 1109, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E4);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Constrict
                defender.Freeze(TimeSpan.FromSeconds(2.0));

                // Continued constriction damage
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        defender.Damage(Utility.RandomMinMax(5, 8), this);
                    }
                });
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            // Reactive lash
            if (from != null && Utility.RandomDouble() < 0.20)
            {
                from.SendMessage(0x480, "The tentacle lashes back at you!");
                from.FixedParticles(0x377A, 244, 25, 9950, 1109, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(6, 10);
                AOS.Damage(from, this, damage, 100, 0, 0, 0, 0);
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;
        public override bool AlwaysMurderer => true;

        public VoidTentacle(Serial serial) : base(serial)
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
