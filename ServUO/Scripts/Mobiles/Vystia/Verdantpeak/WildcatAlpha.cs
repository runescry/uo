using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wildcat alpha corpse")]
    public class WildcatAlpha : BaseCreature
    {
        [Constructable]
        public WildcatAlpha() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a wildcat alpha";
            Body = 0xD6;
            Hue = 2010;
            BaseSoundID = 0x69;

            SetStr(200, 250);
            SetDex(130, 160);
            SetInt(50, 70);

            SetHits(180, 230);
            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 55.0, 75.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 6000;
            Karma = -6000;
            VirtualArmor = 40;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 90.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new LivingOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new NatureforgedIngot());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new LivingOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x3B2, "The wildcat alpha's razor claws tear into you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2010, 0, EffectLayer.Waist);

                // Deep wounds - heavy bleed
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 5, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(4, 8), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Pounce attack
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                Mobile target = Combatant as Mobile;
                if (target != null && !target.Deleted && target.Alive)
                {
                    Say("*The wildcat alpha pounces!*");
                    DoHarmful(target);
                    int damage = Utility.RandomMinMax(20, 35);
                    AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
                    target.SendMessage(0x3B2, "The wildcat alpha pounces on you!");
                    target.Freeze(TimeSpan.FromSeconds(1.5));
                }
            }
        }

        public override int Meat => 1;
        public override int Hides => 10;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Feline;

        public WildcatAlpha(Serial serial) : base(serial)
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
