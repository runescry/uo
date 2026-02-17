using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a deepwater shark corpse")]
    public class DeepwaterShark : BaseCreature
    {
        [Constructable]
        public DeepwaterShark() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a deepwater shark";
            Body = 0x21;
            Hue = 1365;
            BaseSoundID = 0x16A;

            SetStr(280, 340);
            SetDex(110, 140);
            SetInt(40, 60);

            SetHits(300, 380);
            SetDamage(18, 26);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Cold, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 95.0, 115.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 48;

            CanSwim = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(4, 8)));
            PackItem(new VoidforgedIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ObsidianOre());

            if (Utility.RandomDouble() < 0.06)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.40)
            {
                defender.SendMessage(0x22, "The deepwater shark's bite tears deep into your flesh!");
                defender.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
                defender.PlaySound(0x1E4);

                int damage = Utility.RandomMinMax(10, 18);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Severe bleeding
                DoSevereBleed(defender);
            }
        }

        private void DoSevereBleed(Mobile target)
        {
            if (target == null || target.Deleted || !target.Alive)
                return;

            target.SendMessage(0x22, "You are bleeding heavily!");

            int ticks = 6;
            int bleedDamage = Utility.RandomMinMax(5, 9);

            for (int i = 0; i < ticks; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1.5 * (i + 1)), () =>
                {
                    if (target != null && !target.Deleted && target.Alive)
                    {
                        target.Damage(bleedDamage, this);
                        target.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
                    }
                });
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Blood frenzy - gets stronger when enemy is bleeding (low HP)
            if (Combatant is Mobile target && target.Hits < target.HitsMax / 2)
            {
                if (Utility.RandomDouble() < 0.05)
                {
                    Say("*The shark senses blood and enters a frenzy*");
                    PlaySound(0x16A);

                    // Temporary damage boost
                    SetDamage(22, 32);

                    Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                    {
                        SetDamage(18, 26);
                    });
                }
            }
        }

        public override int Meat => 10;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public DeepwaterShark(Serial serial) : base(serial)
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
