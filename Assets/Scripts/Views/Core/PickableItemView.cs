using UnityEngine;
using Views.UI.Inventory;

namespace WildIsland.Views
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickableItemView : MonoBehaviour
    {
        [field: SerializeField] public InventoryItemView InventoryItemView { get; private set; }
    }
}