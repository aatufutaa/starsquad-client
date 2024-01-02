using System;
using StarSquad.Lobby.UI.Button;
using UnityEngine;

namespace StarSquad.Lobby.UI.Screens.Settings
{
    public class SettingsButton : CustomButton
    {
        public GameObject on;
        public GameObject off;

        protected bool allowSame = true;
        [NonSerialized] public bool state = true;

        protected override void HandleClick()
        {
            if (!this.allowSame && this.state) return;
            this.SetEnabled(!this.state);
            base.HandleClick();
        }

        public void SetEnabled(bool enabled)
        {
            this.state = enabled;
            this.on.SetActive(this.state);
            this.off.SetActive(!this.state);
        }
    }
}