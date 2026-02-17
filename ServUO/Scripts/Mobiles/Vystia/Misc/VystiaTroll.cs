using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia troll corpse")]
    public class VystiaTroll : BaseCreature
    {
        [Constructable]
        public VystiaTroll() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia troll";
            Body = 53;
            BaseSoundID = 461;

            SetStr(200, 260);
            SetDex(80, 100);
            SetInt(50, 70);

            SetHits(200, 280);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 75.0, 95.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 5500;
            Karma = -5500;
            VirtualArmor = 42;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackGold(120, 220);
        }

        public override void OnThink()
        {
            base.OnThink();

            // Regeneration
            if (Hits < HitsMax && Utility.RandomDouble() < 0.05)
            {
                Hits = Math.Min(HitsMax, Hits + Utility.RandomMinMax(5, 10));

                if (Utility.RandomDouble() < 0.3)
                    Say("*The troll's wounds begin to close*");
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Rend attack
            if (Utility.RandomDouble() < 0.20)
            {
                defender.SendMessage(0x22, "The troll's claws rend your flesh!");
                DoBleedEffect(defender);
            }
        }

        private void DoBleedEffect(Mobile target)
        {
            if (target == null || target.Deleted || !target.Alive)
                return;

            int ticks = 4;
            int bleedDamage = Utility.RandomMinMax(3, 5);

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

        public override bool CanRummageCorpses => true;
        public override int Meat => 4;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public VystiaTroll(Serial serial) : base(serial)
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
