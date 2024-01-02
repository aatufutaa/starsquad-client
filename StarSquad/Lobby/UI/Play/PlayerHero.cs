using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Play
{
    public class PlayerHero : MonoBehaviour
    {
        public TMP_Text nameText;

        public void SetUsername(string name)
        {
            this.nameText.text = name;
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }
    }
}