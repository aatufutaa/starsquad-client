using StarSquad.Game;
using StarSquad.Loader;
using StarSquad.Lobby;
using StarSquad.Net.Packet.Game;
using StarSquad.Net.Packet.Lobby;
using StarSquad.Net.Packet.Login;
using StarSquad.Net.Session;
using UnityEngine;

namespace StarSquad.Net
{
    public class NetworkManager
    {
        public readonly SessionManager sessionManager;
        
        public readonly ConnectionManager connectionManager;
        
        public NetworkManager()
        {
            Debug.Log("Starting net");
            
            this.sessionManager = new SessionManager();

            this.connectionManager = new ConnectionManager();
            this.connectionManager.Connect(LoaderConstants.LoginServer, LoaderConstants.LoginServerPort, new LoginPacketManager());  // connect to login server
        }

        public void Tick()
        {
            this.connectionManager.Tick();
        }

        private void StopLobby()
        {
            // TODO:
            LobbyManager.Stop();
        }
        
        private void StopGame()
        {
            GameManager.Stop();
        }

        public void Stop()
        {
            // stop lobby
            this.StopLobby();
            
            // stop game
            this.StopGame();

            this.connectionManager.Disconnect("Stop called");
        }

        public void ChangeServer(int serverType, string host, int port)
        {
            Debug.Log("Changing server to " + host + ":" + port + " (type=" + serverType + ")");
            
            this.connectionManager.DoNotTryToReconnect();
            
            const int lobby = 0;
            //const int game = 1;
            
            LoaderManager.instance.ShowLoader(serverType == lobby, () =>
            {
                // after loading fade in is done
                
                // no need to disconnect (call connect does it)

                this.sessionManager.dataLoaded = false; // reset data load on server change

                this.StopLobby();
                this.StopGame();
                
                this.connectionManager.Connect(host, port,
                    serverType == lobby ? new LobbyPacketManager() : new GamePacketManager());
            });
        }

        public static NetworkManager GetNet()
        {
            return LoaderManager.instance.networkManager;
        }
    }
}