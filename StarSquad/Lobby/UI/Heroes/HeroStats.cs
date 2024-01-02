using TMPro;
using UnityEngine;

namespace StarSquad.Lobby.UI.Heroes
{
    public class HeroStats : MonoBehaviour
    {
        public Transform slider;
        private float timer;
        private bool animating;

        private Vector3 start;

        public TMP_Text amountText;
        private int amount;

        private int target;

        private void Start()
        {
            this.start = this.slider.transform.localPosition;
        }

        public void SetAmount(int amount)
        {
            this.amount = amount;
            this.amountText.text = "" + amount;
        }

        public void Upgrade(int value)
        {
            this.target = value;

            this.timer = 0f;
            this.animating = true;
        }

        private void Update()
        {
            if (!this.animating) return;

            this.timer += Time.deltaTime;

            const float sliderTime = 1f;
            var p = this.timer / sliderTime;
            if (p > 1f)
            {
                this.animating = false;

                this.SetAmount(this.target);
            }
            else
            {
                var newValue = this.amount + (int)((this.target - this.amount) * p);
                this.amountText.text = "" + newValue;
            }

            const float moveArea = 700;
            this.slider.localPosition = this.start + new Vector3(moveArea * p, 0);
        }
    }
}