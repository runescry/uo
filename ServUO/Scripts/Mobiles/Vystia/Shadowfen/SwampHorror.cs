using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a swamp horror corpse")]
    public class SwampHorror : BaseCreature
    {
        [Constructable]
        public SwampHorror() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a swamp horror";
            Body = 312;
            Hue = 2073;
            BaseSoundID = 0x165;

            SetStr(280, 350);
            SetDex(80, 100);
            SetInt(100, 130);

            SetHits(260, 330);
            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 85, 95);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);

            Fame = 9000;
            Karma = -9000;
            VirtualArmor = 50;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new BogIronOre(Utility.RandomMinMax(4, 8)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(2, 4)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new BogIronOre(Utility.RandomMinMax(1, 2)));

            if (Utility.RandomDouble() < 0.10)
                PackItem(new VoidDust());

            if (Utility.RandomDouble() < 0.05)
                PackItem(new VoidDust());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x3B2, "The swamp horror's tentacles wrap around you!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2073, 0, EffectLayer.Waist);

                // Grapple - freeze and DoT
                defender.Freeze(TimeSpan.FromSeconds(2));
                defender.ApplyPoison(this, Poison.Greater);

                Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 4, () =>
                {
                    if (defender != null && !defender.Deleted && defender.Alive)
                    {
                        AOS.Damage(defender, this, Utility.RandomMinMax(5, 10), 50, 0, 0, 50, 0);
                    }
                });
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Toxic cloud AoE
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoToxicCloud();
            }
        }

        private void DoToxicCloud()
        {
            if (Map == null)
                return;

            Say("*The swamp horror releases a toxic cloud*");
            PlaySound(0x231);

            foreach (Mobile m in GetMobilesInRange(4))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x374A, 10, 15, 5028, 68, 0, EffectLayer.Waist);
                        m.ApplyPoison(this, Poison.Deadly);

                        int damage = Utility.RandomMinMax(15, 25);
                        AOS.Damage(m, this, damage, 0, 0, 0, 100, 0);
                        m.SendMessage(0x3B2, "The toxic cloud chokes you!");
                    }
                }
            }
        }

        public override Poison HitPoison => Poison.Deadly;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public SwampHorror(Serial serial) : base(serial)
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
