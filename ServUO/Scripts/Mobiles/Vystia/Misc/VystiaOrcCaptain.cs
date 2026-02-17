using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vystia orc captain corpse")]
    public class VystiaOrcCaptain : BaseCreature
    {
        [Constructable]
        public VystiaOrcCaptain() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vystia orc captain";
            Body = 7;
            BaseSoundID = 0x45A;

            SetStr(200, 260);
            SetDex(100, 130);
            SetInt(60, 80);

            SetHits(200, 280);
            SetDamage(14, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 85.0, 105.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);
            SetSkill(SkillName.Swords, 85.0, 105.0);

            Fame = 6500;
            Karma = -6500;
            VirtualArmor = 45;

            // Equipment
            AddItem(new DoubleAxe());
            AddItem(new OrcHelm());
            AddItem(new ChainChest());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Rich);

            PackGold(150, 250);

            if (Utility.RandomDouble() < 0.40)
                PackItem(new ShadowIronOre(Utility.RandomMinMax(3, 7)));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.25)
            {
                defender.SendMessage(0x22, "The orc captain's powerful strike sends you reeling!");
                defender.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Head);
                defender.PlaySound(0x1E1);

                int damage = Utility.RandomMinMax(8, 14);
                AOS.Damage(defender, this, damage, 100, 0, 0, 0, 0);

                // Stagger
                defender.Freeze(TimeSpan.FromSeconds(1.2));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Rally nearby orcs
            if (Combatant != null && Utility.RandomDouble() < 0.02)
            {
                DoRallyCry();
            }
        }

        private void DoRallyCry()
        {
            if (Map == null)
                return;

            Say("*The orc captain rallies the troops!*");
            PlaySound(0x45A);

            foreach (Mobile m in GetMobilesInRange(8))
            {
                if (m is VystiaOrc || m is VystiaOrcCaptain)
                {
                    if (m != this && m is BaseCreature bc)
                    {
                        // Buff nearby orcs
                        bc.Hits = Math.Min(bc.HitsMax, bc.Hits + 20);
                    }
                }
            }
        }

        public override bool CanRummageCorpses => true;
        public override int Meat => 1;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 2;

        public VystiaOrcCaptain(Serial serial) : base(serial)
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
