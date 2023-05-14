using UnityEngine;

namespace Views.UI.Inventory
{
    public class InventoryView : MonoBehaviour
    {
        public void ShowHide()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}