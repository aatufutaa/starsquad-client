using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StarSquad.Game.UI
{
    public class WinnersUI : MonoBehaviour
    {
        public TMP_Text text;
        private string originalText;

        private void Awake()
        {
            this.originalText = this.text.text;
        }

        public void SetWinners(List<string> names)
        {
            var list = "";
            for (var i = 0; i < names.Count; i++)
            {
                list += names[i];
                if (i != names.Count - 1)
                {
                    list += ", ";
                }
            }
            
            this.text.gameObject.SetActive(true);
            this.text.text = string.Format(this.originalText, list);
        }
    }
}