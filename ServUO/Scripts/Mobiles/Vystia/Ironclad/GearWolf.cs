using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gear wolf corpse")]
    public class GearWolf : BaseCreature
    {
        [Constructable]
        public GearWolf() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gear wolf";
            Body = 225;
            Hue = 2305;
            BaseSoundID = 0xE5;

            SetStr(150, 180);
            SetDex(120, 150);
            SetInt(30, 50);

            SetHits(120, 160);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 90);
            SetDamageType(ResistanceType.Energy, 10);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 55.0, 75.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackItem(new SteamworkOre(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ClockworkIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ClockworkSpring());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x25, "The gear wolf's mechanical jaws tear into you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2305, 0, EffectLayer.Waist);

                int damage = Utility.RandomMinMax(8, 15);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Armor shred effect
                defender.SendMessage(0x25, "Your armor is damaged by the mechanical bite!");
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
                    Say("*The gear wolf pounces with mechanical precision*");
                    DoHarmful(target);
                    int damage = Utility.RandomMinMax(18, 28);
                    AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
                    target.SendMessage(0x25, "The gear wolf pounces on you!");
                    target.Freeze(TimeSpan.FromSeconds(1));
                }
            }
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override PackInstinct PackInstinct => PackInstinct.Canine;

        public GearWolf(Serial serial) : base(serial)
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
