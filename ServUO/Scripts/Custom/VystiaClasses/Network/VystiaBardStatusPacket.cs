using Server.Network;

namespace Server.Custom.VystiaClasses.Network
{
    public sealed class VystiaBardStatusPacket : Packet
    {
        public const ushort SubCommand = 0xBEEF;
        public const ushort PayloadType = 0x0001;

        public VystiaBardStatusPacket(bool enabled, int concentration, int concentrationMax, int crescendo, int crescendoMax)
            : base(0xBF)
        {
            EnsureCapacity(13);

            m_Stream.Write(unchecked((short)SubCommand));
            m_Stream.Write((short)PayloadType);
            m_Stream.Write((byte)(enabled ? 1 : 0));
            m_Stream.Write((ushort)concentration);
            m_Stream.Write((ushort)concentrationMax);
            m_Stream.Write((ushort)crescendo);
            m_Stream.Write((ushort)crescendoMax);
        }
    }
}
