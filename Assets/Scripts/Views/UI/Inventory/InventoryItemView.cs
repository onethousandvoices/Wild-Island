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

        private RectTransform _rect;
        private CellView[] _occupiedCells;

        private const int _cellSize = 98;

        public RectTransform RT => _rect ??= GetComponent<RectTransform>();
        public CellView CellPair { get; private set; }

        public void Init()
        {
            _events.BeginDragEvent += OnBeginDragEvent;
            _events.DragEvent += OnDragEvent;
            _events.EndDragEvent += OnEndDragEvent;
            _events.ClickEvent += OnClickEvent;
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
            CellPair = cell;
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
            CellPair.SetOccupied(this);
            OccupyInner(true);
            transform.SetParent(CellPair.transform, false);
            RT.anchoredPosition = Vector2.zero;
        }

        public void SetOnStartDragCallback(Action<InventoryItemView, PointerEventData> callback)
            => _onStartDragCallback = callback;

        public void SetOnDragCallback(Action<InventoryItemView, PointerEventData> callback)
            => _onDragCallback = callback;

        public void SetOnEndDragCallback(Action<InventoryItemView, PointerEventData> callback)
            => _onEndDragCallback = callback;

        public void SetOnClickCallback(Action<InventoryItemView, PointerEventData> callback)
            => _onClickCallback = callback;

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