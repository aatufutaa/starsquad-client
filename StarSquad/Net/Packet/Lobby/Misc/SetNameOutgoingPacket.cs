namespace StarSquad.Net.Packet.Lobby.Misc
{
    public class SetNameOutgoingPacket : OutgoingPacket
    {
        private readonly string name;

        public SetNameOutgoingPacket(string name)
        {
            this.name = name;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.name);
        }
    }
}