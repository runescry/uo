using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia ogre corpse")]
    public class VystiaOgre : BaseCreature
    {
        [Constructable]
        public VystiaOgre() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia ogre";
            Body = 1;
            BaseSoundID = 427;

            SetStr(220, 280);
            SetDex(70, 90);
            SetInt(50, 70);

            SetHits(220, 300);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 75.0, 95.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 6000;
            Karma = -6000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackGold(150, 250);

            if (Utility.RandomDouble() < 0.25)
                PackItem(new Club());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Ground slam
            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x22, "The ogre's mighty blow staggers you!");
                defender.FixedParticles(0x36BD, 20, 10, 5044, 0, 0, EffectLayer.Head);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(10, 16);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                defender.Freeze(TimeSpan.FromSeconds(1.0));
            }
        }

        public override bool CanRummageCorpses => true;
        public override int Meat => 5;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public VystiaOgre(Serial serial) : base(serial)
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
