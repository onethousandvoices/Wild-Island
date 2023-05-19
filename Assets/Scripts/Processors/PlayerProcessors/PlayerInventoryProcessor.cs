using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Views.UI.Inventory;
using WildIsland.Extras;
using Zenject;
using Object = UnityEngine.Object;

namespace WildIsland.Processors
{
    public enum NeighbourSide : byte
    {
        Up,
        Down,
        Left,
        Right
    }

    public class PlayerInventoryProcessor : BaseProcessor, IPlayerInventory, IInitializable
    {
        [Inject] private InventoryView _inventoryView;
        [Inject] private InventoryItemViewBase _itemPrefab;

        private CellView[] _allCells;
        private CellView[,] _cells;
        private ToolView[] _tools;
        private List<InventoryItemViewBase> _items;
        private HandsView _hands;

        public bool IsInventoryShown { get; private set; }
        private const int _width = 6;
        private const int _height = 8;

        public void Initialize()
        {
            _allCells = _inventoryView.GetComponentsInChildren<CellView>();
            _cells = new CellView[_height, _width];

            int index = 0;

            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    _cells[i, j] = _allCells[index];
                    index++;
                }
            }

            _tools = _inventoryView.GetComponentsInChildren<ToolView>();
            _hands = _inventoryView.GetComponentInChildren<HandsView>();

            _items = new List<InventoryItemViewBase>
            {
                Object.Instantiate(_itemPrefab, _inventoryView.Bg),
                Object.Instantiate(_itemPrefab, _inventoryView.Bg),
            };

            _cells[0, 0].SetOccupied(_items[0]);
            _cells[1, 1].SetOccupied(_items[1]);

            foreach (InventoryItemViewBase testItem in _items)
            {
                testItem.SetOnStartDragCallback(OnItemStartDrag);
                testItem.SetOnDragCallback(OnItemDrag);
                testItem.SetOnEndDragCallback(OnItemEndDrag);
                testItem.SetOnClickCallback(OnItemClick);
            }
        }

        public void ShowInventory(InputAction.CallbackContext obj)
            => IsInventoryShown = _inventoryView.ShowHide();

        private void OnItemStartDrag(InventoryItemViewBase item, PointerEventData data)
        {
            item.CellPair.SetOccupied(false);
            item.transform.parent = _inventoryView.Bg;
        }
        
        private void OnItemDrag(InventoryItemViewBase item, PointerEventData data)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(item.RT, data.position, data.pressEventCamera, out Vector3 globalMousePos))
                item.RT.position = globalMousePos;
            // Debug.Log(IsPlaceable(item) ? "can be placed" : "CANT be placed");
        }

        private void OnItemEndDrag(InventoryItemViewBase item, PointerEventData data)
        {
            TryFitItem(item);
        }

        private void OnItemClick(InventoryItemViewBase item, PointerEventData data) { }

        public void AutoFitItem(InventoryItemViewBase item)
        {
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    CellView cell = _cells[i, j];

                    if (cell.IsOccupied)
                        continue;

                    if (item.Size == Vector2.one)
                    {
                        cell.SetOccupied(item);
                        return;
                    }
                }
            }
        }

        private void TryFitItem(InventoryItemViewBase item)
        {
            CellView closest = ClosestCell(item);
            
            switch (closest.IsOccupied)
            {
                case true:
                    item.ResetPosition();
                    break;
                case false:
                    closest.SetOccupied(item);
                    break;
            }
        }

        private CellView ClosestCell(InventoryItemViewBase item)
        {
            CellView closest = null;
            float distance = 999;
            
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    CellView cell = _cells[i, j];
                    
                    float distanceToItem = Vector2.Distance(cell.RT.position, item.RT.position);

                    if (distanceToItem > distance)
                        continue;
                    distance = distanceToItem;
                    closest = cell;
                }
            }

            return closest;
        }

        private CellView GetNeighbour(int x, int y, NeighbourSide side)
        {
            switch (side)
            {
                case NeighbourSide.Up:
                    int upNeighbourHeight = x - 1;
                    return upNeighbourHeight < 0 ? null : _cells[upNeighbourHeight, y];
                case NeighbourSide.Down:
                    int downNeighbourHeight = x + 1;
                    return downNeighbourHeight >= _height ? null : _cells[downNeighbourHeight, y];
                case NeighbourSide.Left:
                    int leftNeighbourWidth = y - 1;
                    return leftNeighbourWidth < 0 ? null : _cells[x, leftNeighbourWidth];
                case NeighbourSide.Right:
                    int rightNeighbourWidth = y + 1;
                    return rightNeighbourWidth >= _width ? null : _cells[x, rightNeighbourWidth];
            }
            return null;
        }

        private bool IsPlaceable(InventoryItemViewBase item)
            => _allCells.Select(cell => cell.RT.Contains(item.RT) && !cell.IsOccupied).FirstOrDefault();
    }

    public interface IPlayerInventory
    {
        public void ShowInventory(InputAction.CallbackContext obj);
        public void AutoFitItem(InventoryItemViewBase item);
        public bool IsInventoryShown { get; }
    }
}