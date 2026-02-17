using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fen stalker corpse")]
    public class FenStalker : BaseCreature
    {
        [Constructable]
        public FenStalker() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a fen stalker";
            Body = 146;
            Hue = 2073;
            BaseSoundID = 456;

            SetStr(200, 250);
            SetDex(130, 160);
            SetInt(60, 80);

            SetHits(180, 230);
            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);
            SetSkill(SkillName.Hiding, 90.0, 110.0);
            SetSkill(SkillName.Stealth, 90.0, 110.0);

            Fame = 7000;
            Karma = -7000;
            VirtualArmor = 45;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);

            PackItem(new BogIronOre(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new ShadowforgedIngot(Utility.RandomMinMax(1, 2)));

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
                defender.SendMessage(0x3B2, "The fen stalker's venomous bite fills you with dread!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 2073, 0, EffectLayer.Waist);
                defender.ApplyPoison(this, Poison.Deadly);

                // Fear effect - stat reduction
                defender.AddStatMod(new StatMod(StatType.All, "FenStalkerFear", -15, TimeSpan.FromSeconds(15)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Ambush from hiding
            if (Combatant != null && !Hidden && Utility.RandomDouble() < 0.02)
            {
                Hidden = true;
                Say("*The fen stalker vanishes into the swamp*");

                Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
                {
                    if (!Deleted && Combatant != null)
                    {
                        Mobile target = Combatant as Mobile;
                        if (target != null)
                        {
                            Hidden = false;
                            MoveToWorld(target.Location, target.Map);
                            DoHarmful(target);
                            int damage = Utility.RandomMinMax(25, 40);
                            AOS.Damage(target, this, damage, 70, 0, 0, 30, 0);
                            target.SendMessage(0x3B2, "The fen stalker ambushes you!");
                            target.Freeze(TimeSpan.FromSeconds(1.5));
                        }
                    }
                });
            }
        }

        public override Poison HitPoison => Poison.Deadly;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 3;

        public FenStalker(Serial serial) : base(serial)
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
