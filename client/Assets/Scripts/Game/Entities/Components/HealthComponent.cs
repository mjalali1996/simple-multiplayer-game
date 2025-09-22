using Game.UI;
using Unity.Netcode;
using UnityEngine;

namespace Game.Entities.Components
{
    [RequireComponent(typeof(Entity))]
    public class HealthComponent : NetworkBehaviour
    {
        private Entity _entity;

        [SerializeField] private int _maxHealth;

        private int _currentHealth;
        public int CurrentHealth => _currentHealth;


        [SerializeField] private HealthBar _healthBar;

        private bool _dead;

        private NetworkVariable<int> _netHealth = new();

        public override void OnNetworkSpawn()
        {
            _healthBar.SetMaxHealth(_maxHealth);
            if (IsServer)
            {
                _entity = gameObject.GetComponent<Entity>();
                _currentHealth = _maxHealth;

                _netHealth.Value = CurrentHealth;
            }

            if (IsClient)
            {
                _healthBar.SetHealth(_netHealth.Value);
            }
        }

        public void Damage(int amount)
        {
            if (!IsServer) return;

            if (_currentHealth == 0) return;

            _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, _maxHealth);
            _netHealth.Value = _currentHealth;

            if (_currentHealth > 0) return;
            _entity.CallOnDeath();
        }

        private void Update()
        {
            if (!IsClient) return;

            _currentHealth = _netHealth.Value;
            _healthBar.SetHealth(_currentHealth);
        }
    }
}