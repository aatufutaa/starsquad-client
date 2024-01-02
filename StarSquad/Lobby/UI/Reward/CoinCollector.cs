using System;
using System.Collections;
using StarSquad.Lobby.UI.Header;
using StarSquad.Lobby.UI.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Reward
{
    public class CoinCollector : MonoBehaviour
    {
        private TMP_Text text;

        private Coin[] pos;
        private CollectorTargetItem endPos;

        private float collectTimer;
        private bool playingCollect;

        private int coinsToGive;
        private float coinsPerCollect;
        private float coinsToGive2;
        private int collectedCount;

        public void Init(int coins, CollectorTargetItem target)
        {
            this.pos = new Coin[this.transform.childCount - 1];
            this.endPos = target;

            var timePerItem = 0.5f / this.pos.Length;

            for (var i = 0; i < this.pos.Length; i++)
            {
                this.pos[i] = new Coin(timePerItem * i, this.transform.GetChild(i), 
                    this.transform.InverseTransformPoint(this.endPos.GetTargetPos()));
            }

            this.text = this.transform.GetChild(this.pos.Length).GetComponent<TMP_Text>();
            this.text.text = "+" + coins;

            this.coinsToGive = coins;
            this.coinsPerCollect = coins / (float)this.pos.Length;
            this.collectedCount = this.pos.Length;
        }

        private void Start()
        {
            if (this.pos != null) return;
            this.Init(10, GameObject.Find("Coins (1)").transform.Find("Coin").GetComponent<CollectorTargetItem>());
        }

        public class Coin
        {
            private readonly float startTime;

            private Transform item;

            private readonly Image image;
            private readonly Vector3 target;

            private float timer;

            private Vector3 fromPos;
            private readonly Vector3 endPos;

            private int stage;

            private bool ended;

            public Coin(float startTime, Transform transform, Vector3 endPos)
            {
                this.startTime = startTime;

                this.item = transform;

                this.image = transform.GetComponent<Image>();
                this.target = transform.localPosition;

                this.endPos = endPos;

                transform.localPosition = Vector3.zero;
                this.SetAlpha(0f);
            }

            private void SetAlpha(float v)
            {
                this.image.color = new Color(1f, 1f, 1f, v);
            }

            public int Update()
            {
                if (this.ended) return 2;

                this.timer += Time.deltaTime;

                if (this.timer < this.startTime) return 0;

                var time = this.timer - this.startTime;

                const float timeToMoveToTarget = 0.5f;
                if (this.stage == 0)
                {
                    const float fadeInTime = 0.1f;
                    var alpha = time / fadeInTime;
                    if (alpha > 1f)
                        alpha = 1f;
                    this.SetAlpha(alpha);

                    var p = time / timeToMoveToTarget;
                    if (p > 1f)
                    {
                        p = 1f;
                        this.stage = 1;
                    }

                    p = MathHelper.SmoothLerp(p);

                    this.item.localPosition = this.target * p;

                    if (this.stage == 1)
                        this.fromPos = this.item.localPosition;
                    return 0;
                }

                time -= timeToMoveToTarget;

                const float waitStill = 0.4f;
                if (time < waitStill) return 0;
                time -= waitStill;

                const float timeToMoveToEndPos = 1f;
                var from = this.fromPos;
                var to = this.endPos;

                var p1 = time / timeToMoveToEndPos;
                if (p1 > 1f)
                {
                    p1 = 1f;
                    this.ended = true;
                    this.SetAlpha(0f);
                }

                p1 *= p1;
                this.item.localPosition = from + (to - from) * p1;

                return this.ended ? 1 : 0;
            }
        }

        private float lastCollect;

        private void PlayCollect(int coins)
        {
            var time = Time.time;
            if (time - this.lastCollect > 0.05f)
            {
                this.lastCollect = time;
                this.playingCollect = true;
                this.collectTimer = 0f;
                
                LobbyManager.instance.audioManager.PlaySound("collect");
            }

            this.endPos.onCollect.Invoke(coins);
        }

        private float timer;

        private void Update()
        {
            var done = true;
            var playCollect = false;
            var count = 0;
            foreach (var coin in this.pos)
            {
                var res = coin.Update();
                if (res == 0)
                {
                    done = false;
                }
                else if (res == 1)
                {
                    playCollect = true;
                    ++count;
                }
            }

            this.timer += Time.deltaTime;
            const float textFadeInTime = 0.5f;
            const float textStillTime = 1f;
            const float textFadeOutTime = 0.4f;
            float textAlpha;
            if (this.timer <= textFadeInTime)
            {
                textAlpha = this.timer / textFadeInTime;
            }
            else if (this.timer <= textStillTime)
            {
                textAlpha = 1f;
            }
            else
            {
                textAlpha = 1f - (this.timer - (textFadeInTime + textStillTime)) / textFadeOutTime;
            }

            this.text.alpha = textAlpha;
            var moveTextP = this.timer / (textFadeInTime + textStillTime + textFadeOutTime);
            moveTextP = MathHelper.SmoothLerp(moveTextP);
            var textFrom = Vector3.zero;
            var textTo = textFrom + new Vector3(0, 50, 0);
            var now = textFrom + (textTo - textFrom) * moveTextP;
            this.text.transform.localPosition = now;

            if (playCollect)
            {
                this.coinsToGive2 += count * this.coinsPerCollect;
                var giveCoins = (int)this.coinsToGive2;
                if (giveCoins > 0)
                {
                    this.coinsToGive2 -= giveCoins;
                    this.coinsToGive -= giveCoins;
                }

                if ((this.collectedCount -= count) == 0)
                {
                    giveCoins += this.coinsToGive;
                }

                this.PlayCollect(giveCoins);
            }

            if (this.playingCollect)
            {
                this.collectTimer += Time.deltaTime;

                var p = this.collectTimer / 0.2f;
                if (p > 1f)
                {
                    this.playingCollect = false;
                    p = 1f;
                }

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

                p = 1f + 0.3f * p;

                this.endPos.transform.localScale = new Vector3(p, p, p);

                return;
            }

            if (done) Destroy(this.gameObject);
        }
    }
}