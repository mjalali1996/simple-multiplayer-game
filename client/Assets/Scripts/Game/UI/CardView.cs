using System;
using Game.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<int, Vector3> OnSpawning;
        public event Action<int, Vector3> OnSpawnRequested;
        public int CardId { get; private set; }

        [SerializeField] private Image _cooldownImage;
        [SerializeField] private TMP_Text _cooldownText;
        [SerializeField] private TMP_Text _nameText;

        private bool _dragging;
        private float _maxCoolDown;
        private float _cooldown;
        private LayerMask _playerZoneMask;


        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragging) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var success = TryToGetWorldPosition(Input.mousePosition, out Vector3 pos);
            if (!success) return;

            OnSpawning?.Invoke(CardId, pos);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragging) return;

            _dragging = false;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            var success = TryToGetWorldPosition(Input.mousePosition, out Vector3 pos);
            if (!success) return;

            OnSpawnRequested?.Invoke(CardId, pos);
        }

        public void Initialize(Card card, LayerMask playerZoneMask)
        {
            _playerZoneMask = playerZoneMask;
            CardId = card.Id;
            _nameText.text = card.Id == 0 ? "Melee" : "Range";
        }

        public void SetCooldown(float cooldown, float maxCooldown)
        {
            _cooldown = cooldown;
            _maxCoolDown = maxCooldown;
        }

        private void Update()
        {
            if (_cooldown == 0)
            {
                SetCooldown(false);
                return;
            }

            SetCooldown(true);

            _cooldown = Mathf.Clamp(_cooldown - Time.deltaTime, 0, _maxCoolDown);
            _cooldownImage.fillAmount = _cooldown / _maxCoolDown;
            _cooldownText.text = _cooldown.ToString("F2");
        }

        private void SetCooldown(bool value)
        {
            _cooldownImage.enabled = value;
            _cooldownText.enabled = value;
        }

        private bool TryToGetWorldPosition(Vector2 screenPos, out Vector3 position)
        {
            var ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit, 1000, _playerZoneMask))
            {
                position = hit.point;
                return true;
            }

            position = Vector3.zero;
            return false;
        }
    }
}