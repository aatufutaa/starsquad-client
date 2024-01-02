using TMPro;
using UnityEngine;

namespace StarSquad.Game.UI
{
    public class GameResultsUI : MonoBehaviour
    {
        public TMP_Text placingText;
        
        public TMP_Text killsText;
        
        public TMP_Text trophiesText;

        public GameObject token;
        public TMP_Text tokensText;

        private bool skip;
        
        public void SetResults(int place, int kills, int trophies, int tokens)
        {
            this.skip = place == -1;
            if (this.skip) return;
        
            this.placingText.text = string.Format(this.placingText.text, place);
            
            this.killsText.text = string.Format(this.killsText.text, kills);

            this.trophiesText.text = "" + (trophies >= 0 ? "+" : "") + trophies;

            if (tokens == 0)
            {
                this.token.SetActive(false);
            }
            else
            {
                this.tokensText.text = "+" + tokens;
            }
        }

        public void Show()
        {
            if (this.skip)
            {
                GameManager.instance.SendHome();
                return;
            }
            this.gameObject.SetActive(true);
            
            GameManager.instance.heroPlayer.Play(GameManager.instance.gameWorld.thePlayer.hero.id, true);
        }
    }
}