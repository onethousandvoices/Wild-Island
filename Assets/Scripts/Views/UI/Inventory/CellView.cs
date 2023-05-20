using UnityEngine;
using UnityEngine.UI;

namespace Views.UI.Inventory
{
    public class CellView : MonoBehaviour
    {
        private RectTransform _rect;

        public RectTransform RT => _rect ??= GetComponent<RectTransform>();
        public Vector2 Coordinates { get; private set; }

        public bool IsOccupied { get; private set; }

        public void SetCoordinates(Vector2 coordinates)
            => Coordinates = coordinates;

        public void SetOccupied(bool state)
        {
            IsOccupied = state;
            GetComponent<Image>().color = IsOccupied ? Color.red : Color.white;
        }
    }
}