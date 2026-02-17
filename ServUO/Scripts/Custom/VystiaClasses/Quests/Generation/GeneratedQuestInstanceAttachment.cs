using System;
using System.Collections.Generic;
using Server;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;

namespace Server.Custom.VystiaClasses.Quests.Generation
{
    public sealed class GeneratedQuestInstance
    {
        public int QuestId;
        public DateTime ExpiresAtUtc;
        public List<int> SpawnedSerials = new List<int>();
    }

    /// <summary>
    /// Per-player (or per-party-leader) persisted instance state for LLM-generated ephemeral quests.
    /// Stored as an XmlSpawner2 attachment for restart safety.
    /// </summary>
    public sealed class GeneratedQuestInstanceAttachment : XmlAttachment
    {
        public const string AttachmentName = "VystiaGeneratedQuestInstances";

        private List<GeneratedQuestInstance> _instances = new List<GeneratedQuestInstance>();

        public List<GeneratedQuestInstance> Instances => _instances;

        public GeneratedQuestInstanceAttachment()
        {
            Name = AttachmentName;
        }

        public GeneratedQuestInstanceAttachment(ASerial serial)
            : base(serial)
        {
        }

        public static GeneratedQuestInstanceAttachment GetOrCreate(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            var a = XmlAttach.FindAttachment(pm, typeof(GeneratedQuestInstanceAttachment), AttachmentName) as GeneratedQuestInstanceAttachment;
            if (a != null)
                return a;

            a = new GeneratedQuestInstanceAttachment();
            XmlAttach.AttachTo(pm, a);
            return a;
        }

        public static GeneratedQuestInstanceAttachment Get(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            return XmlAttach.FindAttachment(pm, typeof(GeneratedQuestInstanceAttachment), AttachmentName) as GeneratedQuestInstanceAttachment;
        }

        public void AddInstance(CompiledQuestInstance compiled)
        {
            if (compiled == null)
                return;

            var inst = new GeneratedQuestInstance
            {
                QuestId = compiled.QuestId,
                ExpiresAtUtc = compiled.ExpiresAtUtc,
                SpawnedSerials = new List<int>(compiled.SpawnedSerials ?? new List<int>()),
            };

            _instances.Add(inst);
        }

        /// <summary>
        /// Remove all instances for a specific quest ID
        /// </summary>
        public void RemoveInstancesForQuest(int questID)
        {
            _instances.RemoveAll(inst => inst.QuestId == questID);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_instances.Count);
            foreach (var inst in _instances)
            {
                writer.Write(inst.QuestId);
                writer.Write(inst.ExpiresAtUtc);

                writer.Write(inst.SpawnedSerials.Count);
                for (int i = 0; i < inst.SpawnedSerials.Count; i++)
                    writer.Write(inst.SpawnedSerials[i]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    _instances = new List<GeneratedQuestInstance>();
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        var inst = new GeneratedQuestInstance();
                        inst.QuestId = reader.ReadInt();
                        inst.ExpiresAtUtc = reader.ReadDateTime();

                        int sc = reader.ReadInt();
                        inst.SpawnedSerials = new List<int>(sc);
                        for (int j = 0; j < sc; j++)
                            inst.SpawnedSerials.Add(reader.ReadInt());

                        _instances.Add(inst);
                    }
                    Name = AttachmentName;
                    break;
            }
        }
    }
}


