using StarSquad.Lobby.Confirm.Friend;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Screens;
using StarSquad.Net.Confirm;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Friends
{
    public class OutgoingFriendInviteItem : CustomButton
    {
        public TMP_Text username;
        public TMP_Text rating;

        public CustomButton cancelButton;

        private string playerId;
        
        private void Awake()
        {
            this.cancelButton.onClick.AddListener(this.HandleCancel);
            
            this.onClick.AddListener(this.HandleProfile);
        }

        private void HandleCancel()
        {
            PacketConfirmManager.Get().Send(new CancelFriendInviteOutgoingPacket(this.playerId));
        }

        public void SetData(string playerId, string name, int rating)
        {
            this.playerId = playerId;
            
            this.username.text = name;
            this.rating.text = "" + rating;
        }

        private void HandleProfile()
        {
            ScreenManager.GetScreenManager().ShowProfile(this.playerId);
        }
    }
}