using UnityEngine;

namespace Views.UI.Inventory
{
    public class InventoryView : MonoBehaviour
    {
        [field: SerializeField] public RectTransform Bg { get; private set; }
        [field: SerializeField] public RectTransform CellsHolder { get; private set; }
        
        public bool ShowHide()
        {
            switch (gameObject.activeSelf)
            {
                case true:
                    gameObject.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case false:
                    gameObject.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    break;
            }

            return gameObject.activeSelf;
        }
    }
}