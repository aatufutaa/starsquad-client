using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Util;
using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Tooltip
{
    public class TooltipBase : CustomButton
    {
        public TMP_Text text;

        private bool showing;
        private float timer;
        private bool animate;

        private void Awake()
        {
            this.onClick.AddListener(() => LobbyManager.instance.tooltipManager.CloseActive(false));
        }

        protected override void Update()
        {
            base.Update();
            
            if (!this.animate) return;
            
            const float scaleTimer = 0.2f;
            if (this.showing)
            {
                this.timer += Time.deltaTime;
                if (this.timer > scaleTimer)
                {
                    this.timer = scaleTimer;
                    this.animate = false;
                    return;
                }
            }
            else
            {
                this.timer -= Time.deltaTime;
                if (this.timer <= 0f)
                {
                    this.timer = 0f;
                    this.gameObject.SetActive(false);
                }
            }

            var p = this.timer / scaleTimer;
            if (p > 0.5f)
            {
                p -= 0.5f;
                p /= 0.5f;
                p = 1f - p;
            }
            else
            {
                p /= 0.5f;
            }

            p = MathHelper.SmoothLerp(p);

            var s = 1f + 0.08f * p;

            this.transform.localScale = new Vector3(s, s, s);
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
            this.showing = true;
            this.animate = true;
        }

        public void Hide(bool instant)
        {
            if (instant)
            {
                this.gameObject.SetActive(false);
                this.timer = 0f;
                return;
            }
            this.animate = true;
            this.showing = false;
        }
    }
}