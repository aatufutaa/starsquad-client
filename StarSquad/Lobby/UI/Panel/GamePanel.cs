using UnityEngine;
using UnityEngine.EventSystems;

namespace StarSquad.Lobby.UI.Panel
{
    public class GamePanel : MonoBehaviour, IPointerDownHandler
    {
        private float timer;
        private bool animating;

        public virtual void Init()
        {
        }

        public void OnPointerDown(PointerEventData data)
        {
            this.Close();
        }

        private void Update()
        {
            if (!this.animating) return;

            this.timer += Time.deltaTime;

            const float scaleTime = 0.2f;
            var p = this.timer / scaleTime;
            if (p > 1f)
            {
                p = 0f;
                this.animating = false;
            }
            else if (p > 0.5f)
            {
                p = p - 0.5f;
                p = p / 0.5f;
                p = 1f - p;
            }
            else
            {
                p /= 0.5f;
            }

            const float addScale = 0.03f;
            var scale = 1f + addScale * p;
            this.transform.GetChild(0).transform.localScale = new Vector3(scale, scale, scale);
        }

        public void Show()
        {
            this.timer = 0f;
            this.animating = true;
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}