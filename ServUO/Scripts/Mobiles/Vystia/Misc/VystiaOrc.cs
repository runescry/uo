using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia orc corpse")]
    public class VystiaOrc : BaseCreature
    {
        [Constructable]
        public VystiaOrc() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia orc";
            Body = 17;
            BaseSoundID = 0x45A;

            SetStr(140, 180);
            SetDex(90, 110);
            SetInt(40, 60);

            SetHits(120, 160);
            SetDamage(10, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 55.0, 75.0);
            SetSkill(SkillName.Tactics, 70.0, 90.0);
            SetSkill(SkillName.Wrestling, 65.0, 85.0);
            SetSkill(SkillName.Swords, 70.0, 90.0);

            Fame = 3500;
            Karma = -3500;
            VirtualArmor = 32;

            // Equipment
            if (Utility.RandomBool())
                AddItem(new Axe());
            else
                AddItem(new WarMace());

            AddItem(new OrcHelm());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Average);

            PackGold(80, 150);

            if (Utility.RandomDouble() < 0.30)
                PackItem(new ShadowIronOre(Utility.RandomMinMax(2, 5)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x22, "The orc's brutal strike dazes you!");
                defender.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Head);
                defender.PlaySound(0x1E1);

                // Brief daze
                defender.Freeze(TimeSpan.FromSeconds(0.8));
            }
        }

        public override bool CanRummageCorpses => true;
        public override int Meat => 1;
        public override bool AlwaysMurderer => true;

        public VystiaOrc(Serial serial) : base(serial)
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
