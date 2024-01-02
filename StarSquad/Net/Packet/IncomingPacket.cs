namespace StarSquad.Net.Packet
{
    public interface IncomingPacket
    {
        void Read(ByteBuf buf);
        void Handle();
    }
}