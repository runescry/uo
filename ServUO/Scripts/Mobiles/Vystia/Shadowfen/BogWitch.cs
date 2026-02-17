using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a bog witch corpse")]
    public class BogWitch : BaseCreature
    {
        [Constructable]
        public BogWitch() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a bog witch";
            Body = 26;
            Hue = 2073;
            BaseSoundID = 0x482;

            SetStr(100, 130);
            SetDex(90, 110);
            SetInt(200, 250);

            SetHits(120, 160);
            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Poison, 70);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 100.0);
            SetSkill(SkillName.Meditation, 60.0, 80.0);
            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 50.0, 70.0);
            SetSkill(SkillName.Poisoning, 90.0, 110.0);

            Fame = 7000;
            Karma = -7000;
            VirtualArmor = 35;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls);

            PackItem(new BogIronOre(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ShadowforgedIngot(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new BogIronOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.05)
                PackItem(new VoidDust());

            PackItem(new Nightshade(Utility.RandomMinMax(3, 8)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x3B2, "The bog witch curses you with swamp sickness!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2073, 0, EffectLayer.Waist);

                // Curse - weakness
                defender.AddStatMod(new StatMod(StatType.Str, "BogWitchCurse", -20, TimeSpan.FromSeconds(30)));
                defender.ApplyPoison(this, Poison.Greater);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Summon will-o-wisps
            if (Combatant != null && Utility.RandomDouble() < 0.01)
            {
                SummonWisp();
            }
        }

        private void SummonWisp()
        {
            if (Map == null)
                return;

            Say("*The bog witch summons swamp spirits*");

            BaseCreature wisp = new Wisp();
            wisp.Name = "a swamp wisp";
            wisp.Hue = 2073;

            Point3D loc = Location;
            wisp.MoveToWorld(loc, Map);
            wisp.Combatant = Combatant;

            Effects.SendLocationParticles(
                EffectItem.Create(loc, Map, EffectItem.DefaultDuration),
                0x376A, 9, 32, 2073, 0, 5030, 0);
        }

        public override Poison HitPoison => Poison.Greater;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public BogWitch(Serial serial) : base(serial)
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
