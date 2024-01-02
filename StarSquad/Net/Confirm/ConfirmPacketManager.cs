using System;
using System.Collections.Generic;
using StarSquad.Net.Packet;
using StarSquad.Net.Packet.Play;

namespace StarSquad.Net.Confirm
{
    public abstract class ConfirmPacketManager : PacketManager
    {
        private readonly Dictionary<int, Type> incoming = new();
        private readonly Dictionary<Type, int> outgoing = new();

        protected ConfirmPacketManager()
        {
            this.RegisterIncoming(17, typeof(ConfirmIncomingPacket));
            this.RegisterOutgoing(17, typeof(ConfirmOutgoingPacket));
            this.RegisterIncoming(18, typeof(ConfirmPingIncomingPacket));
            this.RegisterOutgoing(18, typeof(ConfirmPingOutgoingPacket));
            this.RegisterIncoming(19, typeof(FlushConfirmIncomingPacket));
        }

        protected void RegisterConfirmIncoming(int id, Type t)
        {
            this.incoming.Add(id, t);
        }

        protected void RegisterConfirmOutgoing(int id, Type t)
        {
            this.outgoing.Add(t, id);
        }

        public int GetConfirmId(Type t)
        {
            return this.outgoing[t];
        }

        public Type GetConfirmType(int id)
        {
            return this.incoming[id];
        }
        
        public override void OnConnected()
        {
            NetworkManager.GetNet().connectionManager.SendPacket(new HelloOutgoingPacket(NetworkManager.GetNet().sessionManager.session));
        }
    }
}