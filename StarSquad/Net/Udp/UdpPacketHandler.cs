using System.Collections.Generic;
using StarSquad.Game;
using StarSquad.Loader;
using StarSquad.Net.Packet;
using StarSquad.Net.Packet.Game;
using StarSquad.Net.Packet.Game.Hello;
using StarSquad.Net.Packet.Game.Misc;
using UnityEngine;

namespace StarSquad.Net.Udp
{
    public class UdpPacketHandler
    {
        public readonly short id;
        public bool useEncryption;

        public bool fallbackToTcp;

        public bool udpConfirmed;

        private float lastUdpPing;
        private bool pingUdp;
        private int pingCounter;

        private int nextServerTick;

        public UdpPacketHandler(short id)
        {
            this.id = id;
        }

        public void StartPingUdp()
        {
            this.pingUdp = true;
        }

        public void HandlePacket(ByteBuf buf)
        {
            var udpId = buf.ReadShort();
            if (udpId != this.id)
            {
                Debug.Log("skip wrong id udp packet " + udpId);
                return;
            }

            var pingPacket = buf.buffer.Length == 2;

            if (pingPacket)
            {
                this.udpConfirmed = true;
                return;
            }

            // TODO: decrypt
            
            var serverTick = buf.ReadShort();

            if (serverTick < this.nextServerTick)
            {
                Debug.Log("expired packet");
                return;
            }

            if (serverTick != this.nextServerTick)
            {
                Debug.Log("Missed udp packet");
            }

            this.nextServerTick = serverTick + 1;

            var count = buf.ReadByte();
            var entities = new List<TickIncomingPacket.EntityData>(count);
            for (var i = 0; i < count; i++)
            {
                var entityData = new TickIncomingPacket.EntityData();
                entityData.Read(buf);
                entities.Add(entityData);
            }

            LoaderManager.instance.mainTaskQueue.RunOnMainThread(() =>
                GameManager.instance.gameWorld.HandleEntityData(entities));
        }

        public void OnFail()
        {
            this.fallbackToTcp = true;
            LoaderManager.instance.networkManager.connectionManager.SendPacket(
                new UdpReadyOutgoingPacket(UdpReadyOutgoingPacket.FallbackToTcp));
        }

        public void Tick()
        {
            if (!this.pingUdp) return;

            if (this.udpConfirmed)
            {
                this.pingUdp = false;
                LoaderManager.instance.networkManager.connectionManager.SendPacket(
                    new UdpReadyOutgoingPacket(UdpReadyOutgoingPacket.Ok));
            }
            else if (Time.time - this.lastUdpPing >= 1f) // ping udp every 1s
            {
                // only ping 5 times
                if (this.pingCounter++ > 4)
                {
                    this.pingUdp = false;
                    this.fallbackToTcp = true;
                    Debug.Log("Cant connect with udp fall back");

                    this.OnFail();
                }
                else
                {
                    LoaderManager.instance.networkManager.connectionManager.SendUdpPacket(new PingOutgoingPacket());
                    this.lastUdpPing = Time.time;
                }
            }
        }
    }
}