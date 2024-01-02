namespace StarSquad.Net.Packet
{
    public interface OutgoingPacket
    {
        void Write(ByteBuf buf);
    }
}