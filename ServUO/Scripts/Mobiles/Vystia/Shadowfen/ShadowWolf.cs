using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a shadow wolf corpse")]
    public class ShadowWolf : BaseCreature
    {
        [Constructable]
        public ShadowWolf() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a shadow wolf";
            Body = 277;
            Hue = 2073;
            BaseSoundID = 0xE5;

            SetStr(130, 160);
            SetDex(120, 150);
            SetInt(40, 60);

            SetHits(100, 130);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);
            SetSkill(SkillName.Hiding, 80.0, 100.0);
            SetSkill(SkillName.Stealth, 80.0, 100.0);

            Fame = 4000;
            Karma = -4000;
            VirtualArmor = 32;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 80.0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);

            if (Utility.RandomDouble() < 0.15)
                PackItem(new BogIronOre(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new BogIronOre());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x3B2, "The shadow wolf's bite infects you with bog plague!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2073, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Regular);

                // Bleeding
                Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 3, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(3, 6), 100, 0, 0, 0, 0);
                    }
                });
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Shadow pounce - stealth ambush
            if (Combatant != null && Utility.RandomDouble() < 0.02 && !Hidden)
            {
                Hidden = true;
                Say("*The shadow wolf melts into darkness*");

                Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
                {
                    if (!Deleted && Combatant != null)
                    {
                        Mobile target = Combatant as Mobile;
                        if (target != null)
                        {
                            Hidden = false;
                            DoHarmful(target);
                            int damage = Utility.RandomMinMax(15, 25);
                            AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
                            target.SendMessage(0x3B2, "The shadow wolf ambushes you from the darkness!");
                        }
                    }
                });
            }
        }

        public override int Meat => 1;
        public override int Hides => 6;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Canine;

        public ShadowWolf(Serial serial) : base(serial)
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
