namespace StarSquad.Net.Packet.Login
{
    public class RequestAccessKeyOutgoingPacket : OutgoingPacket
    {
        private readonly string key;

        public RequestAccessKeyOutgoingPacket(string key)
        {
            this.key = key;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.key);
        }
    }
}