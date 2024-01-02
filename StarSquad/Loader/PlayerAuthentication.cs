using StarSquad.Lobby.Confirm;
using StarSquad.Net;
using StarSquad.Net.Confirm;
using StarSquad.Net.Packet.Login;
using StarSquad.Net.Session;
using UnityEngine;

namespace StarSquad.Loader
{
    public class PlayerAuthentication
    {
        private static bool linked;

        public static void LoginToGameCenter()
        {
            if (linked) return;
            linked = true;
            Social.localUser.Authenticate(HandleAuth);
        }

        private static void HandleAuth(bool success)
        {
            if (!success)
            {
                Debug.Log("failed to login to gamae center");
                return;
            }

            var id = Social.localUser.id;
            Debug.Log("game center authentication success");

            PacketConfirmManager.Get().Send(new LinkGameCenterOutgoingPacket(id));
        }

        public static void ReadAndSendToken()
        {
            // get token from playerpref
            var token = PlayerPrefs.GetString("token", null);
            Debug.Log("token " + token);

            // first login?
            if (token == null)
            {
                // try to get token from keychain
#if UNITY_IOS
                // TODO: get token from keychain
#endif
            }

            NetworkManager.GetNet().connectionManager.SendPacket(new LoginOutgoingPacket(token));
            NetworkManager.GetNet().sessionManager.SetState(LoginState.Login);
        }

        public static void SaveToken(string token)
        {
            PlayerPrefs.SetString("token", token);
            PlayerPrefs.Save();
        }

        public static void SaveTokenOnAccountCreate(string token)
        {
            SaveToken(token);

#if UNITY_IOS
            // TODO: save token to keychain if not exist
// save token to keychain if not
#endif
        }
    }
}