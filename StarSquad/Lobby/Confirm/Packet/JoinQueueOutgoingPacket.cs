using StarSquad.Net.Packet;

namespace StarSquad.Lobby.Confirm.Packet
{
    public class JoinQueueOutgoingPacket : OutgoingPacket
    {
        public enum QueueType
        {
            TowerWars,
            CandyRush,
            LastHeroStanding
        }

        private readonly QueueType queueType;

        public JoinQueueOutgoingPacket(QueueType queueType)
        {
            this.queueType = queueType;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteByte((byte)this.queueType);
        }
    }
}