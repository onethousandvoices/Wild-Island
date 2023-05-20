using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views.UI.Inventory
{
    public class PointerEventsReceiver : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public event Action<PointerEventData> BeginDragEvent;
        public event Action<PointerEventData> DragEvent;
        public event Action<PointerEventData> EndDragEvent;
        public event Action<PointerEventData> ClickEvent;
        
        public void OnBeginDrag(PointerEventData eventData)
            => BeginDragEvent?.Invoke(eventData);

        public void OnDrag(PointerEventData eventData)
            => DragEvent?.Invoke(eventData);

        public void OnEndDrag(PointerEventData eventData)
            => EndDragEvent?.Invoke(eventData);

        public void OnPointerClick(PointerEventData eventData)
            => ClickEvent?.Invoke(eventData);
    }
}