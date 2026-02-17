using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a void stalker corpse")]
    public class VoidStalker : BaseCreature
    {
        [Constructable]
        public VoidStalker() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a void stalker";
            Body = 0x190;
            Hue = 1109;
            BaseSoundID = 0x482;

            SetStr(280, 350);
            SetDex(150, 180);
            SetInt(100, 140);

            SetHits(300, 380);
            SetDamage(18, 26);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 30);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.MagicResist, 85.0, 105.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);
            SetSkill(SkillName.Hiding, 100.0, 120.0);
            SetSkill(SkillName.Stealth, 100.0, 120.0);

            Fame = 12000;
            Karma = -12000;
            VirtualArmor = 52;

            AddItem(new Cloak(1109));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);

            PackItem(new ObsidianOre(Utility.RandomMinMax(5, 10)));
            PackItem(new ShadowforgedIngot(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomDouble() < 0.15)
                PackItem(new VoidDust());

            if (Utility.RandomDouble() < 0.10)
                PackItem(new VoidDust());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.30)
            {
                defender.SendMessage(0x480, "The void stalker's touch drains your essence!");
                defender.FixedParticles(0x374A, 10, 15, 5028, 1109, 0, EffectLayer.Waist);
                defender.PlaySound(0x1FB);

                int damage = Utility.RandomMinMax(12, 20);
                AOS.Damage(defender, this, damage, 0, 0, 50, 0, 50);

                // Essence drain - heals self
                Hits = Math.Min(HitsMax, Hits + damage / 2);

                // Brief confusion
                defender.AddStatMod(new StatMod(StatType.Int, "VoidConfusion", -15, TimeSpan.FromSeconds(8)));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Shadow step - teleport behind target
            if (Combatant is Mobile target && Utility.RandomDouble() < 0.03)
            {
                DoShadowStep(target);
            }
        }

        private void DoShadowStep(Mobile target)
        {
            if (Map == null || target == null || !CanBeHarmful(target))
                return;

            Say("*The void stalker melts into shadow*");
            PlaySound(0x482);

            // Visual effect at old location
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 1109, 0, 5039, 0);

            // Teleport behind target
            Point3D newLocation = target.Location;
            int xOffset = target.Direction == Direction.North ? 0 : target.Direction == Direction.South ? 0 : target.Direction == Direction.East ? -1 : 1;
            int yOffset = target.Direction == Direction.North ? 1 : target.Direction == Direction.South ? -1 : 0;

            newLocation.X += xOffset;
            newLocation.Y += yOffset;

            if (Map.CanFit(newLocation, 16, false, false))
            {
                Location = newLocation;
            }

            // Visual effect at new location
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 1109, 0, 5039, 0);

            // Backstab damage
            DoHarmful(target);
            int damage = Utility.RandomMinMax(25, 40);
            AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);
            target.SendMessage(0x480, "The void stalker appears behind you!");
        }

        public override bool AlwaysMurderer => true;
        public override int TreasureMapLevel => 4;

        public VoidStalker(Serial serial) : base(serial)
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
