namespace StarSquad.Net.Packet.Login
{
    public class LoginOutgoingPacket : OutgoingPacket
    {
        private readonly string token;

        public LoginOutgoingPacket(string token)
        {
            this.token = token;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteBool(this.token != null);
            if (this.token != null)
                buf.WriteString(this.token);
        }
    }
}