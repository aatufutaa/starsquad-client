using StarSquad.Lobby.UI.Button;
using TMPro;
using UnityEngine;

namespace StarSquad.Loader.Dialog
{
    public class FallbackDialog : MonoBehaviour
    {
        public TMP_Text title;
        public TMP_Text msg;

        public CustomButton button;

        private int id;
        
        private void Awake()
        {
            this.button.gameObject.SetActive(false);
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public void SetTitle(string title)
        {
            this.title.text = title;
        }

        public void SetMsg(string msg)
        {
            this.msg.text = msg;
        }

        public void AddButton(string msg)
        {
            var button = Instantiate(this.button, this.transform);
            button.gameObject.SetActive(true);
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = msg;
            button.onClick.AddListener(() =>
            {
                Destroy(this.gameObject);
                GameObject.Find("DialogManager").SendMessage("HandleResponse", this.id + "," + 0);
            });
        }
    }
}