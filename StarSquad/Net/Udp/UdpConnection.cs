using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using StarSquad.Net.Packet;
using UnityEngine;

namespace StarSquad.Net.Udp
{
    public class UdpConnection
    {
        private bool running;
        
        private UdpClient socket;
        private IPEndPoint endPoint;
        
        // read
        private Thread readThread;
        
        // write
        private ByteBuf writeBuf;

        public readonly UdpPacketHandler handler;
        
        public UdpConnection(short id)
        {
            this.handler = new UdpPacketHandler(id);
        }
        
        public void Start(string host, int port, int localPort)
        {
            Debug.Log("Start UDP at " + host + ":" + port);

            this.running = true;

            try
            {
                this.socket = new UdpClient(localPort);
                this.socket.Connect(host, port);

                // read thread
                this.readThread = new Thread(this.StartReadThread);
                this.readThread.IsBackground = true;
                this.readThread.Start();

                this.writeBuf = new ByteBuf(1024);
                
                this.handler.StartPingUdp();
            }
            catch (Exception e)
            {
                this.handler.OnFail();
                
                Debug.Log("Failed to start udp");
                Debug.Log(e);
                this.Stop();
            }
        }

        public void Stop()
        {
            this.running = false;

            // silently close socket
            try
            {
                this.socket.Close();
            }
            catch (Exception e)
            {
            }
            
            this.readThread?.Interrupt();
        }

        public void SendPacket(OutgoingPacket packet)
        {
            if (!this.handler.udpConfirmed && packet is not PingOutgoingPacket) return;
            
            try
            {
                this.writeBuf.SetWriterIndex(0);
                this.writeBuf.WriteShort(this.handler.id);

                if (this.handler.useEncryption)
                {
                    // TODO: encrypt
                }

                packet.Write(this.writeBuf);

                // TODO: make sure same buffer can be used after writing (so its not stored in mem)
                /*
                // copy data from write buf to new fix size arr
                var data = new byte[this.writeBuf.GetWriterIndex()];
                Array.Copy(this.writeBuf.buffer, data, data.Length);
                
                // write data to socket
                this.socket.Send(data, data.Length);
                */

                this.socket.Send(this.writeBuf.buffer, this.writeBuf.GetWriterIndex());
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("Failed to send udp packet " + packet); ;
            }
        }

        private void StartReadThread()
        {
            while (this.running)
            {
                try
                {
                    var remoteEnd = new IPEndPoint(IPAddress.Any, 0); // TODO: make this member of class
                    
                    byte[] data;
                    try
                    {
                        data = this.socket.Receive(ref remoteEnd);
                    }
                    catch (ThreadInterruptedException)
                    {
                        this.running = false;
                        Debug.Log("udp read thread interrupted");
                        return;
                    }
                    
                    var buf = new ByteBuf(data); // wrapped buf around data

                    this.handler.HandlePacket(buf);
                }
                catch (Exception e)
                {
                    Debug.Log("Failed to read udp packet->");
                    Debug.Log(e);
                }
            }
        }

        public void Tick()
        {
            this.handler.Tick();
        }
    }
}