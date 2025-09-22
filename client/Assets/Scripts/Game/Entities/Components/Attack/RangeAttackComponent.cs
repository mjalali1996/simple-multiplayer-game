using Game.Arena;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.Components.Attack
{
    public class RangeAttackComponent : AttackComponent
    {
        [SerializeField] private int _projectileAttackId;
        private float Speed => 1 / Interval;
        private float _timer;
        [ReadOnly, ShowInInspector] private bool _attacked;

        private void Update()
        {
            if (!IsServer) return;

            if (!CanAttack())
            {
                _timer = 0;
            }
            else
            {
                _timer += Time.deltaTime * Speed;

                if (!_attacked)
                {
                    _attacked = true;
                    InstantiateProjectile();
                }

                if (_timer >= 1)
                {
                    _timer = 0;
                    _attacked = false;
                }
            }
        }

        private void InstantiateProjectile()
        {
            if (!IsServer) return;

            var attack = IArena.Instance.Spawner.CreateAttackProjectile(_projectileAttackId, transform.position);

            attack.SetTarget(Target, Damage);
        }
    }
}