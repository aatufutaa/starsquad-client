using System.Collections.Generic;
using StarSquad.Loader;
using StarSquad.Lobby;
using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Net.Confirm
{
    public class PacketConfirmManager
    {
        public bool canSendPackets = true; // true by default because on connected does not get called unless reconnect
        private readonly Queue<IncomingPacket> queuedIncoming = new();
        
        private readonly Queue<QueuedPacket> outgoing = new();

        public int latestIncomingId;
        private int latestOutgoingId;

        public readonly ConfirmPacketManager packetManager;
        
        public PacketConfirmManager(ConfirmPacketManager packetManager)
        {
            this.packetManager = packetManager;
        }

        // when reconnect send packets that were not sent
        public void OnConnected(int latestServerId)
        {
            this.ServerConfirm(latestServerId); // remove accepted

            // resend new packet
            // do not remove the packets though before server confirm
            foreach (var packet in this.outgoing)
            {
                NetworkManager.GetNet().connectionManager.SendPacket(new ConfirmOutgoingPacket(packet.id, packet.packet));
            }

            this.canSendPackets = true;
            
            // process incoming packets
            while (this.queuedIncoming.Count > 0)
            {
                this.queuedIncoming.Dequeue().Handle();
            }
        }

        public void OnDisconnect()
        {
            this.canSendPackets = false;
        }
        
        // when server tells you can now remove old packets
        public void ServerConfirm(int latestServerId)
        {
            while (this.outgoing.Count > 0)
            {
                // remove accepted packets
                var packet = this.outgoing.Peek();
                if (packet.id <= latestServerId)
                {
                    this.outgoing.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        public void Handle(int id, IncomingPacket packet)
        {
            if (this.latestIncomingId + 1 != id)
            {
                // TODO: disconnect
                Debug.Log("cant handle incoming wrong id " + id + " expect at " + this.latestIncomingId + 1);
                return;
            }
            
            this.latestIncomingId = id;
            
            if (!this.canSendPackets)
            {
                this.queuedIncoming.Enqueue(packet);   
            }
            else
            {
                packet.Handle();
            }
        }

        public void Send(OutgoingPacket packet)
        {
            var id = ++this.latestOutgoingId;
            var queuedPacket = new QueuedPacket(id, packet);
            this.outgoing.Enqueue(queuedPacket);

            if (this.canSendPackets)
                NetworkManager.GetNet().connectionManager.SendPacket(new ConfirmOutgoingPacket(id, packet));
        }

        public class QueuedPacket
        {
            public int id;
            public OutgoingPacket packet;

            public QueuedPacket(int id, OutgoingPacket packet)
            {
                this.id = id;
                this.packet = packet;
            }
        }

        public static PacketConfirmManager Get()
        {
            return LoaderManager.instance.networkManager.connectionManager.packetConfirmManager;
        }
    }
}