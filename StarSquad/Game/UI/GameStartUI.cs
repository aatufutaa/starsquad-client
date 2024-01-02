using TMPro;
using UnityEngine;

namespace StarSquad.Game.UI
{
    public class GameStartUI : MonoBehaviour
    {
        public GameObject title;
        private bool playTitleAnimation;
        private float titleAnimationTimer;
        
        public GameObject start;
        public TMP_Text timeText;

        public GameObject go;

        private float timer;
        private bool playGoAnimation;

        private bool hidden = true;
        
        public void SetTime(int seconds)
        {
            if (seconds == 0)
            {
                this.Hide();
                this.PlayGoAnimation();
            }
            else
            {
                if (this.hidden)
                {
                    this.PlayTitleAnimation();
                    this.hidden = false;
                    this.start.SetActive(true);
                }
               
                this.timeText.text = seconds + "s";
            }
        }

        private void PlayTitleAnimation()
        {
            this.title.SetActive(true);

            this.playTitleAnimation = true;
            this.titleAnimationTimer = 0f;
        }

        private void PlayGoAnimation()
        {
            this.timer = 0f;
            this.playGoAnimation = true;
            this.go.SetActive(true);
        }

        public void Hide()
        {
            if (this.hidden) return;
            this.hidden = true;
            this.start.SetActive(false);
        }

        private void Update()
        {
            if (this.playTitleAnimation)
            {
                this.titleAnimationTimer += Time.deltaTime;
                if (this.titleAnimationTimer >= 1f)
                {
                    this.title.SetActive(false);

                    this.playTitleAnimation = false;
                }
            }
            
            if (!this.playGoAnimation) return;

            this.timer += Time.deltaTime;

            if (this.timer > 1f)
            {
                this.go.SetActive(false);
                this.playGoAnimation = false;
            }
        }
    }
}