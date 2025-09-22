using Unity.Netcode;
using UnityEngine;

namespace Game.Entities.Components.Attack
{
    [RequireComponent(typeof(Entity))]
    public abstract class AttackComponent : NetworkBehaviour
    {
        [SerializeField] private int _attackDamage;
        public int Damage => _attackDamage;

        [SerializeField] private float _attackRange;
        public float Range => _attackRange;

        [SerializeField] private float _attackInterval;
        public float Interval => _attackInterval;

        public Entity Entity { get; private set; }
        private Transform _transform;

        public Entity Target { get; private set; }
        protected Transform TargetTransform { get; private set; }
        protected HealthComponent TargetHealthComponent { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            Entity = GetComponent<Entity>();
            Entity.TryGetComponent(out _transform);
        }

        public void SetTarget(Entity target)
        {
            if (!IsServer) return;

            Target = target;
            if (target)
            {
                target.TryGetComponent(out Transform targetTransform);
                TargetTransform = targetTransform;
                target.TryGetComponent(out HealthComponent targetHealth);
                TargetHealthComponent = targetHealth;
            }
            else
            {
                TargetTransform = null;
                TargetHealthComponent = null;
            }
        }

        public bool CanAttack()
        {
            if (!IsServer) return false;

            if (TargetHealthComponent == null) return false;
            if (TargetTransform == null) return false;
            if (TargetHealthComponent.CurrentHealth <= 0) return false;

            var attackRange = _attackRange;
            var distance = Vector3.Distance(_transform.position, TargetTransform.position);

            return distance <= attackRange;
        }
    }
}