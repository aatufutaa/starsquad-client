using StarSquad.Lobby.UI.Button;
using StarSquad.Net.Confirm;
using StarSquad.Net.Packet.Lobby.Misc;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens
{
    public class NameScreen : GameScreen
    {
        public TMP_InputField input;
        public ActiveButton button;

        private void Awake()
        {
            this.input.onValueChanged.AddListener(this.HandleInput);

            this.button.Init();
            this.button.SetDisabled();
            this.button.onClick.AddListener(this.HandleOk);
        }

        private void HandleInput(string input)
        {
            if (this.TestName(input))
            {
                this.button.SetEnabled();
            }
            else
            {
                this.button.SetDisabled();
            }
        }

        private bool TestName(string name)
        {
            if (name.Length >= 3 && name.Length <= 16)
            {
                // test for characters

                return true;
            }

            return false;
        }

        private void HandleOk()
        {
            var name = this.input.text;

            if (!this.TestName(name))
            {
                // TODO: say msg
                return;
            }

            Debug.Log("name " + name);

            PacketConfirmManager.Get().Send(new SetNameOutgoingPacket(name));
        }
    }
}