using System;
using System.Collections.Generic;
using System.Linq;
using Game.Entities;
using Game.Entities.Projectiles.Attacks;
using Unity.Netcode;
using UnityEngine;

namespace Game.Arena
{
    [Serializable]
    public class PrefabContainer<T>
    {
        public int Id;
        public T Prefab;
    }

    public class ArenaSpawner : MonoBehaviour
    {
        [SerializeReference] private List<PrefabContainer<NetworkObject>> _entitiesPrefabs;
        [SerializeReference] private List<PrefabContainer<NetworkObject>> _projectileAttacks;

        private NetworkSpawnManager _spawner;

        private NetworkSpawnManager Spawner
        {
            get
            {
                _spawner ??= NetworkManager.Singleton.SpawnManager;
                return _spawner;
            }
        }

        public Entity CreateEntity(Player owner, int id, Vector3 position)
        {
            var prefab = _entitiesPrefabs.First(c => c.Id == id).Prefab;
            var obj = Spawner.InstantiateAndSpawn(prefab, position: position);
            obj.transform.position = position;
            var entity = obj.GetComponent<Entity>();
            entity.Initialize(owner);

            return entity;
        }

        public ProjectileAttack CreateAttackProjectile(int id, Vector3 position)
        {
            var prefab = _projectileAttacks.First(c => c.Id == id).Prefab;
            var obj = Spawner.InstantiateAndSpawn(prefab, position: position);
            obj.transform.position = position;
            var attack = obj.GetComponent<ProjectileAttack>();
            return attack;
        }

        public void Despawn(Entity entity)
        {
            entity.GetComponent<NetworkObject>().Despawn();
        }
    }
}