using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ankheg corpse")]
    public class Ankheg : BaseCreature
    {
        [Constructable]
        public Ankheg() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ankheg";
            Body = 173;
            Hue = 1719;
            BaseSoundID = 959;

            SetStr(250, 300);
            SetDex(80, 100);
            SetInt(40, 60);

            SetHits(200, 260);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Poisoning, 70.0, 90.0);
            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 7000;
            Karma = -7000;
            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new SandstoneOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new SunforgedIngot(Utility.RandomMinMax(1, 3)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new SandstoneOre(Utility.RandomMinMax(1, 2)));

            // REMOVED OLD REAGENT:
            // if (Utility.RandomDouble() < 0.05)
            //     PackItem(new EmberBloom());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x5D, "The ankheg spits acid at you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                defender.PlaySound(0x231);

                // Acid damage
                int damage = Utility.RandomMinMax(10, 20);
                AOS.Damage(defender, this, damage, 0, 0, 0, 100, 0);

                // Armor degradation message
                defender.SendMessage(0x5D, "The acid eats at your armor!");
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Burrow attack
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoBurrowAttack();
            }
        }

        private void DoBurrowAttack()
        {
            if (Map == null || Combatant == null)
                return;

            Mobile target = Combatant as Mobile;
            if (target == null)
                return;

            Say("*The ankheg burrows underground*");
            Hidden = true;
            FixedParticles(0x376A, 9, 32, 5030, 1719, 0, EffectLayer.Waist);

            Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
            {
                if (target != null && !target.Deleted && target.Alive && !Deleted)
                {
                    Hidden = false;
                    Effects.SendLocationParticles(
                        EffectItem.Create(target.Location, Map, EffectItem.DefaultDuration),
                        0x376A, 9, 32, 1719, 0, 5030, 0);

                    DoHarmful(target);
                    int damage = Utility.RandomMinMax(20, 35);
                    AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
                    target.SendMessage(0x5D, "The ankheg erupts from beneath you!");
                }
            });
        }

        public override Poison HitPoison => Poison.Greater;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public Ankheg(Serial serial) : base(serial)
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
