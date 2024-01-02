using StarSquad.Lobby.UI.Util;
using UnityEngine;

namespace StarSquad.Loader
{
    public class BadInternet : MonoBehaviour
    {
        public Transform icon;
        private float timer;

        private float lastShowTime;

        public void Show()
        {
            this.lastShowTime = 0f;
            if (this.gameObject.activeSelf) return;
            this.timer = 0f;
            this.gameObject.SetActive(true);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            this.lastShowTime += dt;

            if (this.lastShowTime > 0.4f)
            {
                this.gameObject.SetActive(false);
                return;
            }

            this.timer += dt;

            const float full = 1f;
            const float scaleTimer = 0.5f;

            if (this.timer > full)
            {
                this.timer = 0f;
            }

            var p = this.timer / scaleTimer;

            if (p >= 1f)
            {
                p = 1f;
            }

            p = MathHelper.SmoothLerp(p);

            if (p <= 0.5f)
            {
                p /= 0.5f;
            }
            else
            {
                p -= 0.5f;
                p /= 0.5f;
                p = 1f - p;
            }

            const float scale = 0.1f;
            var s = 1f + scale * p;
            this.icon.localScale = new Vector3(s, s, s);
        }
    }
}