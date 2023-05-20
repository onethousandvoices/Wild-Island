using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views.UI.Inventory
{
    public class InventoryItemViewBase : MonoBehaviour
    {
        [field: SerializeField] public Vector2 Size { get; private set; } = Vector2.one;

        protected Action<InventoryItemViewBase, PointerEventData> OnStartDragCallback;
        protected Action<InventoryItemViewBase, PointerEventData> OnDragCallback;
        protected Action<InventoryItemViewBase, PointerEventData> OnEndDragCallback;
        protected Action<InventoryItemViewBase, PointerEventData> OnClickCallback;

        private RectTransform _rect;
        
        public RectTransform RT => _rect ??= GetComponent<RectTransform>();
        public CellView CellPair { get; private set; }

        public void SetCell(CellView cell)
        {
            CellPair = cell;
            ResetPosition();
        }

        public void ResetPosition()
        {
            transform.parent = CellPair.transform;
            RT.anchoredPosition = Vector2.zero;
        }

        public void SetOnStartDragCallback(Action<InventoryItemViewBase, PointerEventData> callback)
            => OnStartDragCallback = callback;

        public void SetOnDragCallback(Action<InventoryItemViewBase, PointerEventData> callback)
            => OnDragCallback = callback;

        public void SetOnEndDragCallback(Action<InventoryItemViewBase, PointerEventData> callback)
            => OnEndDragCallback = callback;

        public void SetOnClickCallback(Action<InventoryItemViewBase, PointerEventData> callback)
            => OnClickCallback = callback;
    }

    public class InventoryItemView : InventoryItemViewBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public void OnDrag(PointerEventData eventData)
            => OnDragCallback?.Invoke(this, eventData);

        public void OnEndDrag(PointerEventData eventData)
            => OnEndDragCallback?.Invoke(this, eventData);

        public void OnPointerClick(PointerEventData eventData)
            => OnClickCallback?.Invoke(this, eventData);

        public void OnBeginDrag(PointerEventData eventData)
            => OnStartDragCallback?.Invoke(this, eventData);
    }
}