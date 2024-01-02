using StarSquad.Lobby.Confirm.Friend;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Friends
{
    public class IncomingFriendInviteItem : CustomButton
    {
        public TMP_Text username;
        public TMP_Text rating;

        public CustomButton acceptButton;
        public CustomButton rejectButton;

        private string playerId;

        private void Awake()
        {
            this.acceptButton.onClick.AddListener(this.HandleAccept);
            this.rejectButton.onClick.AddListener(this.HandleReject);
            
            this.onClick.AddListener(this.HandleProfile);
        }

        public void SetData(string playerId, string name, int rating)
        {
            this.playerId = playerId;

            this.username.text = name;
            this.rating.text = "" + rating;
        }

        private void HandleAccept()
        {
            PacketConfirmManager.Get().Send(new AcceptInviteOutgoingPacket(this.playerId, true));
        }

        private void HandleReject()
        {
            PacketConfirmManager.Get().Send(new AcceptInviteOutgoingPacket(this.playerId, false));
        }

        private void HandleProfile()
        {
            ScreenManager.GetScreenManager().ShowProfile(this.playerId);
        }
    }
}