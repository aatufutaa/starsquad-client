using UnityEngine;

namespace StarSquad.Lobby.UI.Util
{
    public class SafeArea : MonoBehaviour
    {
        private RectTransform safeAreaRect;
        private Canvas canvas;
        private Rect lastSafeArea;

        public bool onlyTop;

        private void Start()
        {
            this.safeAreaRect = this.GetComponent<RectTransform>();
            this.canvas = this.GetComponentInParent<Canvas>();

            var safeArea = Screen.safeArea;
            var inverseSize = new Vector2(1f, 1f) / this.canvas.pixelRect.size;
            var newAnchorMin = Vector2.Scale(safeArea.position, inverseSize);
            var newAnchorMax = Vector2.Scale(safeArea.position + safeArea.size, inverseSize);

            if (this.onlyTop)
            {
                newAnchorMin.y = this.safeAreaRect.anchorMin.y;
            }

            this.safeAreaRect.anchorMin = newAnchorMin;
            this.safeAreaRect.anchorMax = newAnchorMax;

            this.safeAreaRect.offsetMin = Vector2.zero;
            this.safeAreaRect.offsetMax = Vector2.zero;
        }
    }
}