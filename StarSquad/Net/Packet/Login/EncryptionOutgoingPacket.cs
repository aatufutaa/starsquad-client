namespace StarSquad.Net.Packet.Login
{
    public class EncryptionOutgoingPacket : OutgoingPacket
    {
        private readonly byte[] key;

        public EncryptionOutgoingPacket(byte[] key)
        {
            this.key = key;
        }
        
        public void Write(ByteBuf buf)
        {
            buf.WriteByteArray(this.key, 0, this.key.Length);
        }
    }
}