using UnityEngine;

namespace StarSquad.Lobby.Hero
{
    public class LobbyHeroBase : MonoBehaviour
    {
        public SkinnedMeshRenderer face;

        private float nextBlink;

        private bool blinking;
        private float blinkingTimer;

        public bool blinkEnabled = true;

        private void Update()
        {
            this.nextBlink -= Time.deltaTime;

            if (this.blinking)
            {
                this.blinkingTimer += Time.deltaTime;

                const float blinkingTime = 0.3f;
                var p = this.blinkingTimer / blinkingTime;

                if (p > 1f)
                {
                    this.blinking = false;
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
                
                var value = p * 100f;
                
                this.face.SetBlendShapeWeight(0, value);
                this.face.SetBlendShapeWeight(1, value);
            }
            
            if (this.nextBlink < 0f)
            {
                this.blinking = true;
                this.blinkingTimer = 0f;
                this.nextBlink = Random.Range(1.5f, 5f);
            }
        }
    }
}