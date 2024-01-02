using System;
using StarSquad.Lobby.UI.Tooltip;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace StarSquad.Lobby.UI.Button
{
    public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent
        {
        }

        public ButtonClickedEvent onClick = new();

        private const bool FireClickAfterAnimation = false;

        private bool down;
        private bool animatingDown;

        private bool animating;
        private float time;

        private bool clicked;

        public bool closeTooltip = true;

        protected virtual void Update()
        {
            if (!this.animating) return;

            const float time = 0.1f;
            const float maxScale = 0.05f;

            if (this.animatingDown)
            {
                this.time += Time.deltaTime;
                if (this.time > time)
                {
                    this.time = time;
                    this.animatingDown = this.down;
                }
            }
            else
            {
                this.time -= Time.deltaTime;
                if (this.time <= 0f)
                {
                    this.time = 0f;
                    this.animating = false;
                }
            }

            var p = this.time / time;
            var scale = 1f - maxScale * p;

            this.transform.localScale = new Vector3(scale, scale, scale);

            if (FireClickAfterAnimation && !this.animating && this.clicked)
            {
                this.HandleClick();
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (FireClickAfterAnimation && this.animating && this.clicked) return;
            this.down = true;
            this.animatingDown = true;
            this.animating = true;
        }

        public void OnPointerUp(PointerEventData data)
        {
            this.down = false;
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (FireClickAfterAnimation)
            {
                this.clicked = true;
            }
            else
            {
                this.HandleClick();
            }
        }

        protected void PlayClickSound()
        {
            if (LobbyManager.instance)
                LobbyManager.instance.audioManager.PlaySound("click");
        }

        protected virtual void HandleClick()
        {
            this.clicked = false;

            if (this.closeTooltip) /* if do close tooltip */
            {
                if (LobbyManager.IsLobby())
                    TooltipManager.Get().CloseActive();
            }

            this.PlayClickSound();

            this.onClick.Invoke();
        }

        private void OnDisable()
        {
            this.down = false;
            this.animatingDown = false;
            this.animating = false;
            this.time = 0f;
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}