using TMPro;

namespace StarSquad.Lobby.UI.Screens.Settings
{
    public class LocationButton : SettingsButton
    {
        public TMP_Text text;

        public void Awake()
        {
            this.allowSame = false;
        }
    }
}