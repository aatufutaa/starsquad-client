namespace StarSquad.Net.Packet.Login
{
    public class HelloOutgoingPacket : OutgoingPacket
    {
        private readonly int versionMajor;
        private readonly int versionMinor;
        private readonly bool android;

        public HelloOutgoingPacket(int versionMajor, int versionMinor, bool android)
        {
            this.versionMajor = versionMajor;
            this.versionMinor = versionMinor;
            this.android = android;
        }

        public void Write(ByteBuf buf)
        {
            buf.WriteInt(this.versionMajor);
            buf.WriteInt(this.versionMinor);
            buf.WriteBool(this.android);
        }
    }
}