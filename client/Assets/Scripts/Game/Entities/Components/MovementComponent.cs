using System;
using Unity.Netcode;
using UnityEngine;

namespace Game.Entities.Components
{
    [RequireComponent(typeof(Entity))]
    public class MovementComponent : NetworkBehaviour
    {
        [SerializeField] private float _speed;
        public float Speed => _speed;

        private Entity _target;
        private Vector3 _targetPosition;
        private bool _move;

        private NetworkVariable<Vector3> _netPosition = new();
        private NetworkVariable<Vector3> _netRotation = new();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _netPosition.Value = transform.position;
                _netRotation.Value = transform.rotation.eulerAngles;
            }

            if (IsClient)
            {
                transform.position = _netPosition.Value;
                transform.rotation = Quaternion.Euler(_netRotation.Value);
            }
        }

        public void SetTarget(Entity target)
        {
            if (!IsServer) return;

            if (_target != null) _target.OnDeath -= TargetOnOnDeath;
            if (target == null) return;

            _target = target;
            _target.OnDeath += TargetOnOnDeath;
            _targetPosition = _target.transform.position;
        }

        private void TargetOnOnDeath(Entity entity)
        {
            entity.OnDeath -= TargetOnOnDeath;
            _target = null;
        }

        public float GetDistanceFromTarget()
        {
            if (!IsServer) return 0;
            var distance = Vector3.Distance(transform.position, _targetPosition);
            return distance;
        }

        public void SetMoveState(bool flag)
        {
            if (!IsServer) return;
            _move = flag;
        }

        private void Update()
        {
            ServerUpdate();

            ClientUpdate();
        }

        private void ServerUpdate()
        {
            if (!IsServer) return;
            try
            {
                if (_move)
                {
                    if (_target != null) _targetPosition = _target.transform.position;
                    transform.position =
                        Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * Speed);
                }

                if (_target != null) transform.LookAt(_target.transform);


                if (_netPosition.Value != transform.position)
                    _netPosition.Value = transform.position;

                if (_netRotation.Value != transform.rotation.eulerAngles)
                    _netRotation.Value = transform.rotation.eulerAngles;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void ClientUpdate()
        {
            if (!IsClient) return;
            transform.position = Vector3.Lerp(transform.position, _netPosition.Value, Time.deltaTime * 10f);

            var targetRot = Quaternion.Euler(_netRotation.Value);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }
}