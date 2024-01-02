using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Play
{
    public class NotificationCounter : MonoBehaviour
    {
        public TMP_Text text;
        
        public void SetCount(int count)
        {
            if (count == 0)
            {
                this.gameObject.SetActive(false);
                return;
            }

            this.gameObject.SetActive(true);
            this.text.text = "" + count;
        }
    }
}