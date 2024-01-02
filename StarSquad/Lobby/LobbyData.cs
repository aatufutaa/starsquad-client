using StarSquad.Lobby.UI.Play;

namespace StarSquad.Lobby
{
    public class LobbyData
    {
        public string name;
        
        public int totalTrophies;

        public int selectedHero;

        public void OnNameChanged(string name)
        {
            this.name = name;
            
            //PlayListener.Get().playerHero.SetUsername(name);
        }
    }
}