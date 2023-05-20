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
        [Inject] private InventoryItemView _itemPrefab;

        private CellView[] _allCells;
        private CellView[,] _cells;
        private ToolView[] _tools;
        private List<InventoryItemView> _items;
        private List<CellView> _affectedCells;
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
                    CellView cell = _allCells[index];
                    _cells[i, j] = cell;
                    cell.SetCoordinates(new Vector2(j, i));
                    index++;
                }
            }

            _tools = _inventoryView.GetComponentsInChildren<ToolView>();
            _hands = _inventoryView.GetComponentInChildren<HandsView>();

            _items = new List<InventoryItemView> { Object.Instantiate(_itemPrefab, _inventoryView.Bg) };
            _affectedCells = new List<CellView>();

            _items[0].Init();
            TryFitItem(_items[0]);

            foreach (InventoryItemView testItem in _items)
            {
                testItem.SetOnStartDragCallback(OnItemStartDrag);
                testItem.SetOnDragCallback(OnItemDrag);
                testItem.SetOnEndDragCallback(OnItemEndDrag);
                testItem.SetOnClickCallback(OnItemClick);
            }
        }

        public void ShowInventory(InputAction.CallbackContext obj)
            => IsInventoryShown = _inventoryView.ShowHide();

        private void OnItemStartDrag(InventoryItemView item, PointerEventData data)
        {
            item.FreeCells();
            item.transform.parent = _inventoryView.Bg;
        }

        private void OnItemDrag(InventoryItemView item, PointerEventData data)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(item.RT, data.position, data.pressEventCamera, out Vector3 globalMousePos))
                item.RT.position = globalMousePos;
            // Debug.Log(IsPlaceable(item) ? "can be placed" : "CANT be placed");
        }

        private void OnItemEndDrag(InventoryItemView item, PointerEventData data)
        {
            TryFitItem(item);
        }

        private void OnItemClick(InventoryItemView item, PointerEventData data) { }

        private void TryFitItem(InventoryItemView item)
        {
            CellView closest = ClosestCell(item);
            
            switch (ItemSizeCheck(closest, item))
            {
                case true:
                    item.OccupyCells(closest, _affectedCells.ToArray());
                    break;
                case false:
                    item.ResetPosition();
                    break;
            }
        }

        private CellView ClosestCell(InventoryItemView item)
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

        private bool ItemSizeCheck(CellView cell, InventoryItemView item)
        {
            _affectedCells.Clear();

            Vector2 targetCoordinate = cell.Coordinates - Vector2.one + item.Size;
            int targetY = (int)targetCoordinate.y;
            int targetX = (int)targetCoordinate.x;

            for (int y = (int)cell.Coordinates.y; y <= targetY; y++)
            {
                for (int x = (int)cell.Coordinates.x; x <= targetX; x++)
                {
                    if (y >= _height || x >= _width || y < 0 || x < 0)
                        return false;
                    
                    CellView affectedCell = _cells[y, x];
                    if (affectedCell.IsOccupied)
                        return false;
                    
                    _affectedCells.Add(affectedCell);
                }
            }

            return true;
        }

        public void AutoFitItem(InventoryItemView item)
        {
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    CellView cell = _cells[i, j];

                    if (cell.IsOccupied)
                        continue;

                    // if (item.Size == Vector2.one)
                    // {
                        // cell.SetOccupied(item);
                        // return;
                    // }
                }
            }
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

        private bool IsPlaceable(InventoryItemView item)
            => _allCells.Select(cell => cell.RT.Contains(item.RT) && !cell.IsOccupied).FirstOrDefault();
    }

    public interface IPlayerInventory
    {
        public void ShowInventory(InputAction.CallbackContext obj);
        public void AutoFitItem(InventoryItemView item);
        public bool IsInventoryShown { get; }
    }
}