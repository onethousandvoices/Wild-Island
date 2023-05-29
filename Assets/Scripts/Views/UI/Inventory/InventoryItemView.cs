using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Views.UI.Inventory
{
    public class InventoryItemView : MonoBehaviour
    {
        [field: SerializeField] public Vector2 Size { get; private set; } = Vector2.one;
        [SerializeField] private PointerEventsReceiver _events;

        private Action<InventoryItemView, PointerEventData> _onStartDragCallback;
        private Action<InventoryItemView, PointerEventData> _onDragCallback;
        private Action<InventoryItemView, PointerEventData> _onEndDragCallback;
        private Action<InventoryItemView, PointerEventData> _onClickCallback;

        private CellView[] _occupiedCells;
        private CellView _cellPair;

        private const int _cellSize = 98;

        public RectTransform RT { get; private set; }

        public InventoryItemView Init(Action<InventoryItemView, PointerEventData> onStartDrag, Action<InventoryItemView, PointerEventData> onDrag,
                                      Action<InventoryItemView, PointerEventData> onEndDrag, Action<InventoryItemView, PointerEventData> onClick)
        {
            RT = GetComponent<RectTransform>();
            
            _events.BeginDragEvent += OnBeginDragEvent;
            _events.DragEvent += OnDragEvent;
            _events.EndDragEvent += OnEndDragEvent;
            _events.ClickEvent += OnClickEvent;

            _onStartDragCallback = onStartDrag;
            _onDragCallback = onDrag;
            _onEndDragCallback = onEndDrag;
            _onClickCallback = onClick;

            return this;
        }

        private void OnBeginDragEvent(PointerEventData obj)
            => _onStartDragCallback?.Invoke(this, obj);

        private void OnDragEvent(PointerEventData obj)
            => _onDragCallback?.Invoke(this, obj);

        private void OnEndDragEvent(PointerEventData obj)
            => _onEndDragCallback?.Invoke(this, obj);

        private void OnClickEvent(PointerEventData obj)
            => _onClickCallback?.Invoke(this, obj);

        private void SetCellPair(CellView cell)
        {
            _cellPair = cell;
            ResetPosition();
        }

        public void OccupyCells(CellView pair, params CellView[] occupiedCells)
        {
            _occupiedCells = occupiedCells;
            SetCellPair(pair);
        }

        public void FreeCells()
            => OccupyInner(false);

        private void OccupyInner(bool state)
        {
            foreach (CellView cellView in _occupiedCells)
                cellView.SetOccupied(state ? this : null);
        }

        private void ResetPosition()
        {
            _cellPair.SetOccupied(this);
            OccupyInner(true);
            transform.SetParent(_cellPair.transform, false);
            RT.anchoredPosition = Vector2.zero;
        }
        
        private void OnValidate()
        {
            _events ??= GetComponentInChildren<PointerEventsReceiver>();

            Image img = GetComponentInChildren<Image>();
            if (img == null)
                return;

            RectTransform imgRT = img.GetComponent<RectTransform>();

            float sizeX = Size.x * _cellSize;
            float sizeY = Size.y * _cellSize;
            imgRT.sizeDelta = new Vector2(sizeX, sizeY);

            float deltaX = (sizeX - _cellSize) / 2;
            float deltaY = (sizeY - _cellSize) / 2;
            imgRT.anchoredPosition = new Vector3(deltaX, -deltaY);
        }
    }
}