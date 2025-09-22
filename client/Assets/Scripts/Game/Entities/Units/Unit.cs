using System;
using System.Linq;
using Game.Arena;
using Game.Entities.Components;
using Game.Entities.Components.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.Units
{
    public class Unit : Entity
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        [ShowInInspector] private Entity _target;
        public Entity Target => _target;

        private MovementComponent _movementComponent;
        protected MovementComponent MovementComponent => _movementComponent;

        private AttackComponent _attackComponent;
        protected AttackComponent AttackComponent => _attackComponent;


        protected override void OnNetworkPostSpawn()
        {
            if (!IsServer)
            {
                if (TryGetComponent<Rigidbody>(out var rb)) Destroy(rb);
                return;
            }

            TryGetComponent(out _movementComponent);
            TryGetComponent(out _attackComponent);
        }

        private Entity GetTarget()
        {
            if (!IsServer) return null;
            try
            {
                var enemy = IArenaDataHandlerServer.Instance.GetEnemyPlayers(OwnerId).FirstOrDefault();
                if (enemy == null) return null;
                var enemyEntities = IArenaDataHandlerServer.Instance.GetPlayerEntities(enemy);

                var targetDistance = float.MaxValue;
                Entity target = null;
                foreach (var enemyEntity in enemyEntities)
                {
                    if (enemyEntity == null || enemyEntity.gameObject == null) continue;
                    if (!enemyEntity.TryGetComponent<Transform>(out var transformComponent)) continue;


                    if (gameObject == null) return null;


                    var distance = Vector3.Distance(transform.position, transformComponent.position);
                    if (targetDistance > distance)
                    {
                        target = enemyEntity;
                        targetDistance = distance;
                    }
                }

                return target;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void FindNewTarget()
        {
            if (!IsServer) return;

            if (_target != null) _target.OnDeath -= TargetOnDeath;

            _target = GetTarget();
            _movementComponent.SetTarget(_target);
            _attackComponent.SetTarget(_target);

            if (_target) _target.OnDeath += TargetOnDeath;
        }

        private void TargetOnDeath(Entity entity)
        {
            if (!IsServer) return;
            FindNewTarget();
        }

        protected virtual void Update()
        {
            if (IsServer)
            {
                if (_target != null) return;

                FindNewTarget();
            }

            if (IsClient)
            {
                var player = IArenaDataHandler.Instance.GetPlayer(OwnerId);
                var zoneMat = IArenaDataHandler.Instance.GetZone(player.ZoneId).Material;
                if (_meshRenderer.material == zoneMat) return;
                _meshRenderer.material = zoneMat;
            }
        }
    }
}