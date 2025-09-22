using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        private void Start()
        {
            _button.onClick.AddListener(OnCardButtonClicked);
            _button.interactable = true;
        }

        private void OnCardButtonClicked()
        {
            throw new System.NotImplementedException();
        }
    }
}