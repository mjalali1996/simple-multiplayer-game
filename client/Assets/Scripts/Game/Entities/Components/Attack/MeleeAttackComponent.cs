using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace Game.Entities.Components.Attack
{
    public class MeleeAttackComponent : AttackComponent
    {
        [SerializeField] private Transform _weapon;
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _endPosition;

        private float Speed => 1 / Interval;
        private float _timer;
        [ReadOnly, ShowInInspector] private bool _attacked;

        private NetworkVariable<Vector3> _netAnimation = new();

        private void Update()
        {
            if (IsServer) ServerUpdate();


            if (IsClient) _weapon.localPosition = _netAnimation.Value;
        }

        private void ServerUpdate()
        {
            if (!CanAttack())
            {
                _timer = 0;
                _weapon.localPosition = _startPosition;
            }
            else
            {
                _timer += Time.deltaTime * Speed;
                if (_timer <= 0.5f)
                {
                    _weapon.localPosition = Vector3.Lerp(_startPosition, _endPosition, _timer * 2);
                }
                else
                {
                    if (!_attacked)
                    {
                        _attacked = true;
                        TargetHealthComponent.Damage(Damage);
                    }

                    _weapon.localPosition = Vector3.Lerp(_endPosition, _startPosition, (_timer - 0.5f) * 2);
                }

                if (_timer >= 1)
                {
                    _timer = 0;
                    _attacked = false;
                }

                _netAnimation.Value = _weapon.localPosition;
            }
        }
    }
}