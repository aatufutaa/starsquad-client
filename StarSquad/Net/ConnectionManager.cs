using StarSquad.Loader;
using StarSquad.Net.Confirm;
using StarSquad.Net.Packet;
using StarSquad.Net.Packet.Lobby;
using StarSquad.Net.Tcp;
using UnityEngine;

namespace StarSquad.Net
{
    public class ConnectionManager
    {
        // tcp
        private string host;
        private int port;

        private TcpConnection tcp;
        public PacketConfirmManager packetConfirmManager;

        // udp
        private int udpPort;

        private bool connectedAtLeastOnce;
        private bool reconnecting;
        private float lastReconnect;
        private int connectCounter;
        private float lastConnectReset;
        private int reconnectCounter;

        public void Connect(string host, int port, PacketManager packetManager)
        {
            if (host.Equals("127.0.0.1") && Application.platform == RuntimePlatform.Android)
            {
                host = "10.0.2.2";
            }

            if (true)
            {
                host = "192.168.0.103";
            }

            this.tcp?.OnError("change server", true);

            this.host = host;
            this.port = port;

            this.connectedAtLeastOnce = false;

            if (packetManager is ConfirmPacketManager confirmPacketManager)
                this.packetConfirmManager = new PacketConfirmManager(confirmPacketManager);

            this.TryToConnect(packetManager);
        }

        public void OnConnect()
        {
            this.connectedAtLeastOnce = true;
            this.reconnectCounter = 0;
        }

        public void DoNotTryToReconnect()
        {
            this.tcp.disconnected = true;
        }

        private void TryToConnect(PacketManager packetManager)
        {
            if (++this.connectCounter > 10 || ++this.reconnectCounter > 5)
            {
                // might be in a connect loop
                Debug.Log("Connected too many times!!!");

                this.ShowConnectError();
            }
            else
            {
                this.tcp = new TcpConnection(packetManager);
                this.tcp.StartAsync(this.host, this.port);
            }
        }

        public void Tick()
        {
            if (this.packetConfirmManager != null && !this.packetConfirmManager.canSendPackets)
            {
                LoaderManager.instance.badInternet.Show();
            }

            if (Time.time - this.lastConnectReset > 2f)
            {
                this.lastConnectReset = Time.time;
                this.connectCounter = 0;
            }

            if (this.tcp == null) return;

            if (this.reconnecting)
            {
                if (Time.time - this.lastReconnect < 1f) return;
                this.lastReconnect = Time.time;

                this.reconnecting = false;

                this.TryToConnect(this.tcp.packetManager);

                return;
            }

            if (this.tcp.disconnected) return;

            this.tcp.Tick();
            
            if (!this.tcp.mainThreadDisconnect) return;

            if (this.tcp.canReconnect)
            {
                Debug.Log("Reconnecting...");

                this.reconnecting = true;

                if (this.tcp.packetManager is LobbyPacketManager)
                    PacketConfirmManager.Get().OnDisconnect();
            }
            else
            {
                Debug.Log("cant reconnect!");

                this.ShowConnectError();
            }
        }

        private void ShowConnectError()
        {
            this.tcp = null;

            const string lostConnection = "Lost connection with server. Check your internet connection and try again.";
            const string cantConnect = "Can't connect with server. Check your internet connection and try again.";
            
            LoaderManager.instance.nativeDialogManager.ShowDialog("Connection Error", this.connectedAtLeastOnce
                    ? lostConnection
                    : cantConnect,
                "Try again",
                button => { LoaderManager.instance.Reload(); });
        }

        public void Disconnect(string msg)
        {
            this.tcp?.OnError(msg, false);
        }

        public void SafeDisconnect(string msg)
        {
            this.tcp?.OnError(msg, true);
        }

        public void SendPacket(OutgoingPacket packet)
        {
            this.tcp?.SendPacket(packet);
        }

        public void EnableEncryption()
        {
            this.tcp.EnableEncryption();
        }

        // UDP
        public void InitUdp(int port)
        {
            this.udpPort = port;
        }

        public void ConnectUdp(short id)
        {
            this.tcp.StartUdp(this.udpPort, id);
        }

        public void SendUdpPacket(OutgoingPacket packet)
        {
            this.tcp?.SendUdpPacket(packet);
        }

        public bool IsFallbackToTcp()
        {
            return this.tcp == null || this.tcp.IsFallbackToTcp();
        }
    }
}