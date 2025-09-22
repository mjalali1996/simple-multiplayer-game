using Game.Entities.Components;
using UnityEngine;

namespace Game.Entities.Projectiles.Attacks
{
    public class ProjectileAttack : Entity
    {
        [SerializeField] private float _hitDistance;

        private MovementComponent _movement;
        private HealthComponent _targetHealth;

        private int _damage;

        public void SetTarget(Entity targetEntity, int damage)
        {
            if (!IsServer) return;

            _movement = GetComponent<MovementComponent>();
            _movement.SetMoveState(true);
            _movement.SetTarget(targetEntity);

            targetEntity.TryGetComponent(out _targetHealth);
            _damage = damage;
        }

        private void Update()
        {
            if (!IsServer) return;

            var distance = _movement.GetDistanceFromTarget();
            if (distance > _hitDistance) return;

            if (_targetHealth) _targetHealth.Damage(_damage);
            Destroy(gameObject);
        }
    }
}