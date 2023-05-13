using UnityEngine;
using UnityEngine.InputSystem;
using Views.UI.Inventory;
using Zenject;

namespace WildIsland.Processors
{
    public class PlayerInventoryProcessor : BaseProcessor, IPlayerInventory, IInitializable
    {
        [Inject] private InventoryView _inventoryView;

        public void Initialize()
        {
            
        }

        public void ShowInventory(InputAction.CallbackContext obj)
        {
            _inventoryView.ShowHide();
        }
    }

    public interface IPlayerInventory
    {
        public void ShowInventory(InputAction.CallbackContext obj);
    }
}