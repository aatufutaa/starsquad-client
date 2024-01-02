using UnityEngine;

namespace StarSquad.Lobby.UI.Screens
{
    public class GameScreen : MonoBehaviour
    {
        protected bool IsShowing()
        {
            return this.gameObject.activeSelf;
        }

        public virtual void Show()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}