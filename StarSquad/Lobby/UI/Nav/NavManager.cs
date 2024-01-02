using StarSquad.Loader;
using StarSquad.Lobby.UI.Button;
using StarSquad.Lobby.UI.Shop;
using StarSquad.Lobby.UI.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Nav
{
    public class NavManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform canvas;
        public RectTransform content;

        public LayoutElement[] items;

        public Transform selected;
        private float selectedArea;
        private float selectedAreaStart;

        private ScrollRect scrollRect;

        private int selectedPage = 2;

        private bool dragging;
        private float from = 0.5f;
        private float time;
        private bool updating;

        private float selectedWidth;
        private float selectedY;
        private float notSelectedY;

        private NavButton[] buttons;

        public class NavButton
        {
            public LayoutElement layoutElement;
            public Transform icon;
            public TMP_Text text;
            public float width;
        }

        private void Start()
        {
            var width = this.canvas.rect.width;
            this.content.sizeDelta = new Vector2(width * 5, 0);

            this.scrollRect = this.GetComponent<ScrollRect>();

            this.buttons = new NavButton[this.items.Length];

            for (var i = 0; i < this.items.Length; i++)
            {
                var item = this.items[i];

                var button = item.transform.GetChild(0).GetComponent<CustomButton>();

                var page = i;

                button.onClick.AddListener(() => { this.SwitchPage(page); });

                var btn = new NavButton();
                btn.layoutElement = item;
                btn.icon = button.transform.GetChild(0);
                btn.text = btn.icon.GetChild(1).GetComponent<TMP_Text>();
                btn.width = item.flexibleWidth;
                this.buttons[i] = btn;

                if (i != 2) btn.text.alpha = 0f;
            }

            this.selectedArea = 1080 - this.selected.GetComponent<RectTransform>().rect.width;
            this.selectedAreaStart = -this.selectedArea / 2;

            this.selectedWidth = this.items[2].flexibleWidth;
            var selectedY = this.buttons[2].icon.localPosition.y;
            var notSelectedY = this.buttons[0].icon.localPosition.y;

            var d = selectedY - notSelectedY;
            this.notSelectedY = notSelectedY;
            this.selectedY = d;
        }

        private void SwitchPage(int page)
        {
            if (page < 0) page = 0;
            else if (page > 4) page = 4;
            
            this.selectedPage = page;

            this.from = this.scrollRect.horizontalNormalizedPosition;
            this.time = 0f;
            this.updating = true;

            for (var i = 0; i < this.items.Length; i++)
            {
                this.buttons[i].width = this.items[i].flexibleWidth;
            }
        }

        private void Update()
        {
            if (!this.dragging && this.updating)
            {
                this.time += Time.deltaTime;
                const float pageSwitchTime = 0.3f;
                var p = this.time / pageSwitchTime;
                if (p > 1f)
                {
                    p = 1f;
                    this.updating = false;
                }

                var smoothP = MathHelper.SmoothLerp(p);

                var target = this.selectedPage * 0.25f;
                var current = this.from + (target - this.from) * smoothP;

                this.scrollRect.horizontalNormalizedPosition = current;

                for (var i = 0; i < this.items.Length; i++)
                {
                    var btn = this.buttons[i];

                    var from = btn.width;
                    var to = i == this.selectedPage ? this.selectedWidth : 1f;

                    btn.layoutElement.flexibleWidth = from + (to - from) * p;

                    var textP = (btn.layoutElement.flexibleWidth-1f) / (this.selectedWidth-1f);
                    var text = btn.text;
                    text.alpha = textP;

                    btn.icon.localPosition = new Vector2(0, this.notSelectedY + this.selectedY * textP);
                }
            }
            else if (!this.dragging) return;

            this.selected.localPosition =
                new Vector3(this.selectedAreaStart + this.selectedArea * this.scrollRect.horizontalNormalizedPosition,
                    this.selected.localPosition.y);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.dragging = true;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.dragging = false;
            
            int page;
            
            var vel = this.scrollRect.velocity.x;
            if (Mathf.Abs(vel) > 1000)
            {
                page = this.selectedPage + (vel > 0 ? -1 : 1);
            }
            else
            {
                page = Mathf.RoundToInt(this.scrollRect.horizontalNormalizedPosition / 0.25f);
            }

            this.SwitchPage(page);
        }

        public void ShowGems()
        {
            if (LoaderManager.IsUsingNet())
                LoaderManager.instance.alertManager.Alert(
                    "You don't have enough gems! You can buy more from the shop");
            
            this.SwitchPage(0);
            
            ShopManager.Get().ShowGems();
        }

        public static NavManager Get()
        {
            return LobbyManager.instance.navManager;
        }
    }
}