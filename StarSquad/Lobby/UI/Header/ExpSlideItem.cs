using StarSquad.Lobby.UI.Util;
using UnityEngine;

namespace StarSquad.Lobby.UI.Header
{
    public class ExpSlideItem : CollectorTargetItem
    {
        private Vector3 to;
        private Vector3 from;
        private Vector3 gFrom;

        private float timer;
        private bool slidingExpIn;
        private float slideWaitTime;
        private bool animating;
        private const float SlideTime = 0.5f;
        private const float WaitStillTime = 2f;

        private void Start()
        {
            var d = new Vector3(300, 0, 0);
            this.to = this.transform.parent.localPosition;
            this.from = this.to - d;
            this.gFrom = this.transform.position;
            this.transform.parent.localPosition = this.from;
        }

        private void Update()
        {
            float p;
            if (this.slidingExpIn)
            {
                this.timer += Time.deltaTime;
                if (this.timer >= SlideTime)
                {
                    this.timer = SlideTime;
                    this.slideWaitTime += Time.deltaTime;
                    if (this.slideWaitTime > WaitStillTime)
                    {
                        this.slidingExpIn = false;
                    }
                }

                p = this.timer / SlideTime;
            }
            else
            {
                this.timer -= Time.deltaTime;
                if (this.timer <= 0f)
                {
                    this.timer = 0f;
                    this.transform.parent.gameObject.SetActive(false);
                }

                p = this.timer / SlideTime;
            }

            p = MathHelper.SmoothLerp(p);
            var pos = this.from + (this.to - this.from) * p;
            this.transform.parent.localPosition = pos;
        }

        public override void OnPreCollect()
        {
            base.OnPreCollect();
            this.transform.parent.gameObject.SetActive(true);
            this.slidingExpIn = true;
            this.slideWaitTime = 0f;
        }

        public override Vector3 GetTargetPos()
        {
            return this.gFrom;
        }
    }
}