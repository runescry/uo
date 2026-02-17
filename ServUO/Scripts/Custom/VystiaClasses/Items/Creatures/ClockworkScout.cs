using System;
using Server;
using Server.Mobiles;

namespace Server.Items.VystiaClassItems
{
    /// <summary>
    /// Clockwork Scout - Basic artificer construct
    /// Auto-despawns after 10 minutes
    /// </summary>
    [CorpseName("a pile of scrap metal")]
    public class ClockworkScout : BaseCreature
    {
        private Timer m_DespawnTimer;

        [Constructable]
        public ClockworkScout() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "clockwork scout";
            Body = 752; // Golem body
            Hue = 2305; // Metallic
            BaseSoundID = 0x233;

            SetStr(80, 100);
            SetDex(60, 80);
            SetInt(30, 50);

            SetHits(60, 80);
            SetDamage(8, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 100); // Immune to poison
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.Wrestling, 60.0, 80.0);

            Fame = 1000;
            Karma = 0;

            VirtualArmor = 35;

            ControlSlots = 2;
            MinTameSkill = 0; // Controlled by artificer device
            Tamable = true; // Must be tamable to accept orders

            // Start despawn timer (10 minutes)
            StartDespawnTimer();
        }

        public override bool Commandable => true; // Must be commandable to follow orders

        private void StartDespawnTimer()
        {
            m_DespawnTimer = Timer.DelayCall(TimeSpan.FromMinutes(10), () =>
            {
                if (this != null && !this.Deleted)
                {
                    if (ControlMaster != null && !ControlMaster.Deleted)
                    {
                        ControlMaster.SendMessage(0x3B2, "Your clockwork construct's power runs out and it collapses.");
                    }

                    Effects.SendLocationParticles(
                        EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                        0x3728, 10, 10, 2305, 0, 5052, 0);

                    PlaySound(0x22F);

                    Delete();
                }
            });
        }

        public override bool DeleteCorpseOnDeath => true;

        public override bool IsDispellable => true;

        public override void OnDelete()
        {
            if (m_DespawnTimer != null)
            {
                m_DespawnTimer.Stop();
                m_DespawnTimer = null;
            }

            base.OnDelete();
        }

        public ClockworkScout(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            // Restart despawn timer on server restart
            StartDespawnTimer();
        }
    }
}
