using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BattlePanel : MonoBehaviour
    {
        [SerializeField] private Button[] _cardButtons;

        public void UpdateTowerHealth(int playerHealth, int enemyHealth)
        {
        }

        public void UpdateCardButton(int cardIndex, bool isAvailable)
        {
            _cardButtons[cardIndex].interactable = isAvailable;
        }
    }
}