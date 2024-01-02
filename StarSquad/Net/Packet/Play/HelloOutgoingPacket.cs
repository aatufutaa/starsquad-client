namespace StarSquad.Net.Packet.Play
{
    public class HelloOutgoingPacket : OutgoingPacket
    {
        private readonly string secret;

        public HelloOutgoingPacket(string secret)
        {
            this.secret = secret;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteString(this.secret);
        }
    }
}