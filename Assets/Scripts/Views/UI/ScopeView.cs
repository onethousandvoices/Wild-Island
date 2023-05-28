using UnityEngine;
using UnityEngine.UI;

namespace Views.UI
{
    public class ScopeView : MonoBehaviour
    {
        [SerializeField] private Image _pickImage;

        public void EnablePickButton(bool state)
        {
            if (_pickImage.gameObject.activeSelf == state)
                return;
            _pickImage.gameObject.SetActive(state);
        }
    }
}