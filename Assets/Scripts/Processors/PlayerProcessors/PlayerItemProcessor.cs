using UnityEngine;
using UnityEngine.InputSystem;
using Views.UI;
using WildIsland.Views;
using Zenject;

namespace WildIsland.Processors
{
    public class PlayerItemProcessor : BaseProcessor, IPlayerProcessor, IInitializable, ITickable, IPickButtonListener
    {
        [Inject] private Camera _camera;
        [Inject] private ScopeView _scopeView;
        [Inject] private IPlayerInventory _playerInventory;

        private PickableItemView _currentItem;
        private LayerMask _pickableLayer;
        private const float _range = 7.5f;

        public void Initialize()
            => _pickableLayer = LayerMask.GetMask("Pickables");

        public void Tick()
        {
            _scopeView.EnablePickButton(CheckItems());
        }

        private bool CheckItems()
        {
            Ray ray = _camera.ScreenPointToRay(_scopeView.transform.position);
            Physics.Raycast(ray, out RaycastHit hit, _range, _pickableLayer);
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * _range, Color.magenta);
#endif
            if (hit.transform == null)
                return false;

            _currentItem = hit.transform.GetComponent<PickableItemView>();
            return _currentItem != null;
        }

        public void OnPickButton(InputAction.CallbackContext obj)
        {
            if (_currentItem == null)
                return;
            
            _playerInventory.AutoFitItem(_currentItem.InventoryItemView);
            Object.Destroy(_currentItem.gameObject);
        }
    }

    public interface IPickButtonListener
    {
        public void OnPickButton(InputAction.CallbackContext obj);
    }
}