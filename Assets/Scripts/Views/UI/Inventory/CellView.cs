using UnityEngine;

namespace Views.UI.Inventory
{
    public class CellView : MonoBehaviour
    {
        private RectTransform _rect;

        public RectTransform RT => _rect ??= GetComponent<RectTransform>();

        public bool IsOccupied { get; private set; }

        public void SetOccupied(bool state)
            => SetOccupiedInner(state);

        public void SetOccupied(InventoryItemViewBase item)
            => SetOccupiedInner(true, item);

        private void SetOccupiedInner(bool state, InventoryItemViewBase item = null)
        {
            IsOccupied = state;
            if (item == null)
                return;
            item.SetCell(this);
        }
    }
}