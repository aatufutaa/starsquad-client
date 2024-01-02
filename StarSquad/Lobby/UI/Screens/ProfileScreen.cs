using StarSquad.Lobby.UI.Button;
using TMPro;

namespace StarSquad.Lobby.UI.Screens
{
    public class ProfileScreen : GameScreen
    {
        public CustomButton backButton;
        
        public TMP_Text playerId;
        public TMP_Text username;

        public TMP_Text rating;
        
        private void Awake()
        {
            this.backButton.onClick.AddListener(this.Hide);
        }
        
        public void SetPlayerId(string playerId)
        {
            this.playerId.text = playerId;
        }

        public void SetName(string name)
        {
            this.username.text = name;
        }

        public void SetRating(int rating)
        {
            this.rating.text = "" + rating;
        }
    }
}