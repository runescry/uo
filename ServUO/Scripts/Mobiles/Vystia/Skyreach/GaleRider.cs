using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gale rider corpse")]
    public class GaleRider : BaseCreature
    {
        [Constructable]
        public GaleRider() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gale rider";
            Body = 400;
            Hue = 1281;

            SetStr(200, 250);
            SetDex(140, 170);
            SetInt(120, 150);

            SetHits(180, 240);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 30);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.EvalInt, 75.0, 95.0);
            SetSkill(SkillName.Magery, 75.0, 95.0);
            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 75.0, 95.0);
            SetSkill(SkillName.Swords, 80.0, 100.0);

            Fame = 10000;
            Karma = -10000;
            VirtualArmor = 45;

            // Equipment
            AddItem(new Cloak(1281));
            AddItem(new LeatherChest());
            AddItem(new LeatherLegs());
            AddItem(new LeatherGloves());
            AddItem(new Boots(1281));

            if (Utility.RandomBool())
                AddItem(new Scimitar());
            else
                AddItem(new Longsword());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);

            PackItem(new CrystalOre(Utility.RandomMinMax(4, 8)));
            PackItem(new CrystallineIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new StormCrystal());

            if (Utility.RandomDouble() < 0.10)
                PackItem(new StormCrystal());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x480, "The gale rider's strike carries the force of the wind!");
                defender.FixedParticles(0x376A, 9, 32, 5007, 1281, 0, EffectLayer.Waist);
                defender.PlaySound(0x10B);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Push back effect
                defender.Freeze(TimeSpan.FromSeconds(0.8));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Wind blade attack
            if (Combatant != null && Utility.RandomDouble() < 0.03)
            {
                DoWindBlade();
            }
        }

        private void DoWindBlade()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null || !CanBeHarmful(target))
                return;

            Say("*The gale rider sends forth a blade of wind*");
            PlaySound(0x10B);

            DoHarmful(target);

            // Moving projectile effect
            MovingParticles(target, 0x1BFE, 7, 0, false, true, 1281, 0, 9502, 1, 0, EffectLayer.Head, 0x100);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (target != null && !target.Deleted && target.Alive)
                {
                    target.FixedParticles(0x376A, 9, 32, 5007, 1281, 0, EffectLayer.Waist);

                    int damage = Utility.RandomMinMax(25, 40);
                    AOS.Damage(target, this, damage, 50, 0, 25, 0, 25);
                    target.SendMessage(0x480, "The wind blade cuts through you!");
                }
            });
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public GaleRider(Serial serial) : base(serial)
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
