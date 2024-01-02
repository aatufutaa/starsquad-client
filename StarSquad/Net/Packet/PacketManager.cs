using System;
using System.Collections.Generic;

namespace StarSquad.Net.Packet
{
    public abstract class PacketManager
    {
        private readonly Dictionary<int, Type> incoming = new Dictionary<int, Type>();
        private readonly Dictionary<Type, int> outgoing = new Dictionary<Type, int>();

        protected void RegisterIncoming(int id, Type packet)
        {
            this.incoming.Add(id, packet);
        }

        protected void RegisterOutgoing(int id, Type packet)
        {
            this.outgoing.Add(packet, id);
        }

        public void Write(OutgoingPacket packet, ByteBuf buf)
        {
            int packetId;
            if (!this.outgoing.TryGetValue(packet.GetType(), out packetId))
            {
                throw new Exception("write bad packet " + packet);
            }
            
            buf.WriteByte((byte)packetId);
            
            packet.Write(buf);
        }

        public IncomingPacket Read(ByteBuf buf)
        {
            var packetId = buf.ReadByte();
            
            if (!this.incoming.TryGetValue(packetId, out var type))
            {
                throw new Exception("read bad packet id " + packetId);
            }

            var packet = (IncomingPacket)Activator.CreateInstance(type);
            packet.Read(buf);
            
            return packet;
        }

        public abstract void OnConnected();
    }
}