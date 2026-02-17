using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a giant vystia spider corpse")]
    public class VystiaGiantSpider : BaseCreature
    {
        [Constructable]
        public VystiaGiantSpider() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a giant vystia spider";
            Body = 173;
            BaseSoundID = 0x388;

            SetStr(180, 230);
            SetDex(120, 150);
            SetInt(50, 70);

            SetHits(180, 240);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 75, 85);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 65.0, 85.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 85.0, 105.0);
            SetSkill(SkillName.Poisoning, 85.0, 105.0);

            Fame = 5500;
            Karma = -5500;
            VirtualArmor = 42;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 90.0;

            PackItem(new SpidersSilk(Utility.RandomMinMax(8, 15)));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            if (Utility.RandomDouble() < 0.30)
                PackItem(new SpidersSilk(Utility.RandomMinMax(10, 20)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            // Web entangle
            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x22, "The giant spider ensnares you in webbing!");
                defender.Freeze(TimeSpan.FromSeconds(2.0));
                defender.AddStatMod(new StatMod(StatType.Dex, "GiantSpiderWeb", -25, TimeSpan.FromSeconds(12)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Web spray
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoWebSpray();
            }
        }

        private void DoWebSpray()
        {
            if (Map == null)
                return;

            Say("*The giant spider sprays webbing*");
            PlaySound(0x388);

            foreach (Mobile m in GetMobilesInRange(4))
            {
                if (m != this && CanBeHarmful(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature bc && bc.Controlled))
                    {
                        DoHarmful(m);
                        m.FixedParticles(0x376A, 9, 32, 5007, 0, 0, EffectLayer.Waist);

                        // Slow and minor damage
                        m.AddStatMod(new StatMod(StatType.Dex, "WebSpray", -30, TimeSpan.FromSeconds(10)));
                        m.Freeze(TimeSpan.FromSeconds(1.5));
                        m.SendMessage(0x22, "You are covered in sticky webbing!");
                    }
                }
            }
        }

        public override Poison PoisonImmune => Poison.Greater;
        public override Poison HitPoison => Poison.Greater;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public VystiaGiantSpider(Serial serial) : base(serial)
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
