using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a merfolk corpse")]
    public class Merfolk : BaseCreature
    {
        [Constructable]
        public Merfolk() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a merfolk warrior";
            Body = 0x190;
            Hue = 1365;

            SetStr(150, 190);
            SetDex(110, 140);
            SetInt(100, 130);

            SetHits(140, 180);
            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 70.0, 90.0);
            SetSkill(SkillName.Magery, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 65.0, 85.0);
            SetSkill(SkillName.Tactics, 75.0, 95.0);
            SetSkill(SkillName.Wrestling, 70.0, 90.0);
            SetSkill(SkillName.Fencing, 75.0, 95.0);

            Fame = 6000;
            Karma = -6000;
            VirtualArmor = 38;

            CanSwim = true;

            // Equipment
            AddItem(new Spear());

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                AddItem(new LeatherBustierArms());
                AddItem(new LeatherSkirt());
            }
            else
            {
                AddItem(new LeatherChest());
                AddItem(new LeatherLegs());
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);

            PackItem(new ObsidianOre(Utility.RandomMinMax(2, 5)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new ObsidianOre());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new VoidforgedIngot());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x480, "The merfolk's weapon strikes with tidal force!");
                defender.FixedParticles(0x3818, 10, 15, 5052, 1365, 0, EffectLayer.Waist);
                defender.PlaySound(0x026);

                int damage = Utility.RandomMinMax(6, 12);
                AOS.Damage(defender, this, damage, 0, 0, 100, 0, 0);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Water bolt ability
            if (Combatant != null && Utility.RandomDouble() < 0.03)
            {
                DoWaterBolt();
            }
        }

        private void DoWaterBolt()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The merfolk hurls a bolt of water*");
            PlaySound(0x026);

            DoHarmful(target);

            MovingParticles(target, 0x36D4, 7, 0, false, true, 1365, 0, 9502, 1, 0, EffectLayer.Waist, 0x100);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (target != null && !target.Deleted && target.Alive)
                {
                    target.FixedParticles(0x3818, 10, 15, 5052, 1365, 0, EffectLayer.Waist);

                    int damage = Utility.RandomMinMax(18, 28);
                    AOS.Damage(target, this, damage, 0, 0, 100, 0, 0);
                    target.SendMessage(0x480, "The water bolt strikes you!");
                }
            });
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public Merfolk(Serial serial) : base(serial)
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
