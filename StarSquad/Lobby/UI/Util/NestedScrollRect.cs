using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarSquad.Lobby.UI.Util
{
    public class NestedScrollRect : ScrollRect
    {
        private bool routeToParent;

        private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
        {
            var parent = transform.parent;
            while (parent != null)
            {
                foreach (var component in parent.GetComponents<Component>())
                {
                    if (component is T)
                        action((T)(IEventSystemHandler)component);
                }

                parent = parent.parent;
            }
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            DoForParents<IInitializePotentialDragHandler>(parent => { parent.OnInitializePotentialDrag(eventData); });
            base.OnInitializePotentialDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (this.routeToParent)
                DoForParents<IDragHandler>(parent => { parent.OnDrag(eventData); });
            else
                base.OnDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!this.horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                this.routeToParent = true;
            else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                this.routeToParent = true;
            else
                this.routeToParent = false;

            if (this.routeToParent)
                DoForParents<IBeginDragHandler>(parent => { parent.OnBeginDrag(eventData); });
            else
                base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (this.routeToParent)
                DoForParents<IEndDragHandler>(parent => { parent.OnEndDrag(eventData); });
            else
                base.OnEndDrag(eventData);
            this.routeToParent = false;
        }
    }
}