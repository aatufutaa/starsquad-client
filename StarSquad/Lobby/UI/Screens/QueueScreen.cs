using StarSquad.Loader;
using StarSquad.Lobby.Confirm.Packet;
using StarSquad.Lobby.UI.Button;
using StarSquad.Net.Confirm;
using TMPro;

namespace StarSquad.Lobby.UI.Screens
{
    public class QueueScreen : GameScreen
    {
        public ActiveButton leaveButton;
        public TMP_Text statusText;

        public void Init()
        {
            this.leaveButton.onClick.AddListener(this.HandleLeave);
        }

        public void OnLeave()
        {
            ScreenManager.GetScreenManager().queue.Hide();
        }

        public void SetParty()
        {
            this.leaveButton.gameObject.SetActive(false);
        }

        private void HandleLeave()
        {
            this.leaveButton.SetDisabled();

            if (!LoaderManager.IsUsingNet())
            {
                this.OnLeave();
            }
            else
            {
                PacketConfirmManager.Get().Send(new LeaveQueueOutgoingPacket());
            }
        }

        public void UpdatePlayerCount(int players, int maxPlayers)
        {
            this.statusText.text = players + "/" + maxPlayers;
        }

        public void ResetLeaveButton()
        {
            this.leaveButton.SetEnabled();
        }

        public override void Show()
        {
            if (this.IsShowing()) return;

            this.statusText.text = "";

            base.Show();

            this.leaveButton.gameObject.SetActive(true);

            this.leaveButton.SetEnabled();
        }
    }
}