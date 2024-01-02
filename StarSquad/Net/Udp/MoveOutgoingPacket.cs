using StarSquad.Net.Packet;
using StarSquad.Net.Packet.Game;
using StarSquad.Net.Packet.Game.Misc;

namespace StarSquad.Net.Udp
{
    public class MoveOutgoingPacket : OutgoingPacket
    {
        private readonly TickOutgoingPacket.PlayerInput playerInput;

        public MoveOutgoingPacket(TickOutgoingPacket.PlayerInput playerInput)
        {
            this.playerInput = playerInput;
        }

        public void Write(ByteBuf buf)
        {
            this.playerInput.Write(buf);
        }
    }
}