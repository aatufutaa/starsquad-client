using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using StarSquad.Loader;
using StarSquad.Net.Packet;
using StarSquad.Net.Udp;
using UnityEngine;

namespace StarSquad.Net.Tcp
{
    public class TcpConnection
    {
        public bool running; // task, read and maybe main thread will be writing to this
        private readonly BlockingCollection<Action> taskQueue = new();
        private Thread taskThread;

        private Socket socket;
        public bool useEncryption;
        public bool canReconnect;
        public bool disconnected;

        // write
        private const int MaxWriteSize = 1024 * 4;
        private ByteBuf writeBuf;
        private byte[] encryptBuf;

        // read
        private Thread readThread;
        private byte[] readBuffer;
        private ByteBuf readBuf;
        private int nextPacketSize;
        private bool readingPacket;
        private ByteBuf decryptBuf;

        public readonly PacketManager packetManager;

        // udp
        private string host;
        private UdpConnection udp;

        public bool mainThreadDisconnect;

        public TcpConnection(PacketManager packetManager)
        {
            this.packetManager = packetManager;
        }

        // TCP
        public void StartAsync(string host, int port)
        {
            Debug.Log("Start TCP at " + host + ":" + port);

            this.host = host; // save for udp

            // start task thread
            this.running = true;
            this.taskThread = new Thread(this.StartTaskThread);
            this.taskThread.IsBackground = true; // close at stop
            this.taskThread.Start();

            // connect
            this.taskQueue.Add(() =>
            {
                try
                {
                    this.socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
                    {
                        ReceiveBufferSize = 1024,
                        SendBufferSize = 1024,
                        NoDelay = true,
                        ReceiveTimeout = 20000
                    };

                    var res = this.socket.BeginConnect(host, port, null, null);
                    if (!res.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2))) throw new Exception("timed out");
                    this.socket.EndConnect(res);

                    LoaderManager.instance.networkManager.connectionManager.OnConnect();

                    // socket connected -> start reading packets
                    this.OnConnected();
                }
                catch (Exception e)
                {
                    Debug.Log("Failed to connect!");
                    Debug.Log(e);

                    this.OnError("Failed to connect -> " + e.Message, false);
                }
            });
        }

        public void SendPacket(OutgoingPacket packet)
        {
            this.taskQueue.Add(() =>
            {
                if (!this.running) return;

                try
                {
                    this.writeBuf.SetWriterIndex(2); // skip packet size
                    this.packetManager.Write(packet, this.writeBuf);

                    var packetSize = this.writeBuf.GetWriterIndex() - 2; // take off length

                    byte[] writeBuf;

                    if (this.useEncryption)
                    {
                        if (this.encryptBuf == null || this.encryptBuf.Length < 2 + packetSize)
                        {
                            this.encryptBuf = new byte[2 + packetSize];
                        }

                        LoaderManager.instance.networkManager.sessionManager.encryption.encrypt.Crypt(
                            this.writeBuf.buffer,
                            2,
                            this.encryptBuf,
                            2,
                            packetSize);

                        writeBuf = this.encryptBuf;
                    }
                    else
                    {
                        writeBuf = this.writeBuf.buffer;
                    }

                    // add packet length as big endian
                    writeBuf[0] = (byte)(packetSize >> 8);
                    writeBuf[1] = (byte)packetSize;

                    this.socket.Send(writeBuf, 0, packetSize + 2, SocketFlags.None);
                }
                catch (Exception e)
                {
                    Debug.Log("Send packet failed");
                    Debug.Log(e);

                    this.OnError("write error -> " + e.Message, false);
                }
            });
        }

        private void OnConnected()
        {
            Debug.Log("Connected");

            // start write queue
            this.writeBuf = new ByteBuf(MaxWriteSize);

            // start read queue
            this.readThread = new Thread(this.StartReadThread);
            this.readThread.IsBackground = true;
            this.readThread.Start();

            this.packetManager.OnConnected(); // send first packet
        }

        // TASK QUEUe
        private void StartTaskThread()
        {
            try
            {
                while (this.running)
                {
                    Action task;
                    try
                    {
                        task = this.taskQueue.Take();
                    }
                    catch (ThreadInterruptedException)
                    {
                        this.running = false;
                        Debug.Log("Task thread interrupted");
                        return;
                    }

                    task.Invoke();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Task queue failed");
                Debug.Log(e);

                this.OnError("Task queue failed -> " + e.Message, false);
            }

            Debug.Log("Task thread stopped");
        }

        private void StartReadThread()
        {
            this.readBuffer = new byte[1024];
            this.readBuf = new ByteBuf(1024);

            while (this.running)
            {
                try
                {
                    this.Read();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    this.OnError("read error -> " + e.Message, false);
                }
            }
        }

        private void Read()
        {
            var length = this.socket.Receive(this.readBuffer, 0, this.readBuffer.Length, SocketFlags.None);

            if (length <= 0)
                throw new Exception("read 0");

            this.readBuf.EnsureCapacity(length);
            this.readBuf.WriteByteArray(this.readBuffer, 0, length);

            // process read data
            while (true)
            {
                if (this.readingPacket)
                {
                    if (this.readBuf.GetWriterIndex() < this.nextPacketSize + 2) return; // doesnt have full packet

                    ByteBuf buf;

                    if (this.useEncryption)
                    {
                        if (this.decryptBuf == null)
                        {
                            this.decryptBuf = new ByteBuf(this.nextPacketSize + 2);
                        }
                        else
                        {
                            this.decryptBuf.ResizeBuf(this.nextPacketSize + 2);
                        }

                        LoaderManager.instance.networkManager.sessionManager.encryption.decrypt.Crypt(
                            this.readBuf.buffer,
                            2,
                            this.decryptBuf.buffer,
                            2,
                            this.nextPacketSize);
                        buf = this.decryptBuf;
                    }
                    else
                    {
                        buf = this.readBuf;
                    }

                    buf.SetReaderIndex(2); // skip packet size

                    var packet = this.packetManager.Read(buf);
                    var readBytes = buf.GetReaderIndex() - 2;

                    if (readBytes != this.nextPacketSize)
                    {
                        throw new Exception("read wrong amount of bytes! reading " + packet + " size=" +
                                            this.nextPacketSize + " read=" +
                                            readBytes);
                    }

                    this.readBuf.Slice(this.nextPacketSize + 2); // move bytes in buf to front

                    LoaderManager.instance.mainTaskQueue.RunOnMainThread(() => packet.Handle()); // run packet

                    this.readingPacket = false;

                    continue;
                }

                if (this.readBuf.GetWriterIndex() < 2) return; // if not packet size

                // read packet size as big endian
                this.nextPacketSize = (ushort)(this.readBuf.buffer[1] + (this.readBuf.buffer[0] << 8));
                this.readingPacket = true;
            }
        }

        public void EnableEncryption()
        {
            LoaderManager.instance.networkManager.sessionManager.encryption.encrypt.Init();
            LoaderManager.instance.networkManager.sessionManager.encryption.decrypt.Init();

            this.useEncryption = true;

            Debug.Log("Encryption has been enabled");
        }

        public void OnError(string msg, bool safeDisconnect)
        {
            if (safeDisconnect)
                this.disconnected = true;

            // TODO: test if its possible to lock socket (when its being read)
            //lock (this.socket)
            // {
            if (!this.running) return;

            this.running = false;
            //}

            this.taskThread.Interrupt();
            this.readThread?.Interrupt(); // ? because if socket never connected

            this.udp?.Stop();

            // silently close socket
            try
            {
                this.socket.Close();
            }
            catch (Exception e)
            {
            }

            Debug.Log("TCP has been stopped due to error! -> " + msg);
            // TODO: try to reconnect

            this.canReconnect = !safeDisconnect && !msg.Contains("read 0") && !msg.Contains("bad packet");

            LoaderManager.instance.mainTaskQueue.RunOnMainThread(() => { this.mainThreadDisconnect = true; });
        }

        // UDP
        public void StartUdp(int port, short udpId)
        {
            this.udp = new UdpConnection(udpId);
            var localPort = ((IPEndPoint)this.socket.LocalEndPoint).Port;

            this.taskQueue.Add(() =>
                this.udp.Start(this.host, port, localPort)
            );
        }

        public void SendUdpPacket(OutgoingPacket packet)
        {
            this.taskQueue.Add(() =>
            {
                if (!this.running) return;

                if (this.udp == null) return;

                this.udp.SendPacket(packet);
            });
        }

        public bool IsFallbackToTcp()
        {
            return this.udp == null || this.udp.handler.fallbackToTcp;
        }

        public void Tick()
        {
            if (!this.running) return;
            this.udp?.Tick();
        }
    }
}