using UnityEngine;

namespace Views.UI.Inventory
{
    public class CellView : MonoBehaviour
    {
        private RectTransform _rect;

        public RectTransform RT => _rect ??= GetComponent<RectTransform>();
        public Vector2 Coordinates { get; private set; }

        public InventoryItemView OccupiedBy { get; private set; }

        public void SetCoordinates(Vector2 coordinates)
            => Coordinates = coordinates;

        public void SetOccupied(InventoryItemView item)
            => OccupiedBy = item;
    }
}