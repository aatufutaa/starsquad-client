using UnityEngine;
using UnityEngine.EventSystems;

namespace StarSquad.Lobby.UI.Tooltip
{
    public class TooltipCloseListener : MonoBehaviour, IPointerDownHandler, IBeginDragHandler
    {
        public void OnPointerDown(PointerEventData data)
        {
            TooltipManager.Get().CloseActive();
        }

        public void OnBeginDrag(PointerEventData data)
        {
            TooltipManager.Get().CloseActive();
        }
    }
}