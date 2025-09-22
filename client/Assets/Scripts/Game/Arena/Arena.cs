using System;
using System.Collections.Generic;
using System.Linq;
using Game.Commands;
using Game.Entities;
using Game.Entities.Tower;
using Game.Inputs;
using Unity.Netcode;
using UnityEngine;

namespace Game.Arena
{
    public class Arena : MonoBehaviour, IArena, IArenaDataHandler, IArenaDataHandlerServer
    {
        public event Action OnFulled;
        public event Action<Player> OnFinished;

        [SerializeField] private ArenaSpawner _arenaSpawner;
        public ArenaSpawner Spawner => _arenaSpawner;
        public int ArenaZoneSize => _zones.Capacity;

        [SerializeReference] private List<Zone> _zones;
        [SerializeField] private List<Player> _players;

        private readonly Dictionary<Player, IZone> _playersZone = new();
        private readonly Dictionary<Player, List<Entity>> _playersEntities = new();

        private void Awake()
        {
            IArena.Instance = this;
            IArenaDataHandler.Instance = this;
            IArenaDataHandlerServer.Instance = this;
        }

        public void AddPlayer(Player player)
        {
            if (ArenaZoneSize <= _playersZone.Count) return;

            _players.Add(player);

            if (!NetworkManager.Singleton.IsServer) return;

            var playerIndex = _players.IndexOf(player);
            var playerZone = _zones[playerIndex];

            _playersZone.Add(player, playerZone);
            _playersEntities.Add(player, new List<Entity>());

            playerZone.Tower.Initialize(player);
            AddEntity(player, playerZone.Tower);

            player.SpawnInput += PlayerSpawnInput;

            if (_players.Count == ArenaZoneSize) OnFulled?.Invoke();
        }

        public void AddEntity(Player player, Entity entity)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            if (!_players.Contains(player)) return;

            if (!_playersEntities.TryGetValue(player, out var playerEntity)) return;
            if (playerEntity.Contains(entity)) return;

            entity.OnDeath += EntityOnDeath;
            _playersEntities[player].Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            var player = GetPlayer(entity.OwnerId);
            if (entity is Tower tower)
            {
                RemoveEntities(_playersEntities[player]);
                _playersEntities[player].Clear();

                var enemyPlayer = GetEnemyPlayers(tower.OwnerId).First();
                Debug.Log($"Arena Winner is: {enemyPlayer.Id}");
                OnFinished?.Invoke(enemyPlayer);
            }
            else
            {
                RemoveEntities(new[] { entity });
                _playersEntities[player].Remove(entity);
            }
        }

        private void RemoveEntities(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.OnDeath -= EntityOnDeath;
                _arenaSpawner.Despawn(entity);
            }
        }

        public int GetNewPlayerId()
        {
            return _players.Count;
        }

        public int GetNewPlayerZoneId()
        {
            var newPlayerId = GetNewPlayerId();
            return _zones[newPlayerId].Id;
        }

        public Player GetOwnerPlayer()
        {
            return _players.FirstOrDefault(p => p.IsOwner);
        }

        public Player GetPlayer(int playerId)
        {
            return _players.First(p => p.Id == playerId);
        }

        public IReadOnlyList<Player> GetPlayers()
        {
            return _players;
        }

        public IReadOnlyList<Player> GetEnemyPlayers(int playerId)
        {
            return _players.Where(p => p.Id != playerId).ToList();
        }

        public IReadOnlyList<Entity> GetPlayerEntities(Player player)
        {
            if (!NetworkManager.Singleton.IsServer) return null;

            return _playersEntities[player];
        }

        public IZone GetZone(int zoneId)
        {
            return _zones.FirstOrDefault(p => p.Id == zoneId);
        }

        private void EntityOnDeath(Entity entity)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            RemoveEntity(entity);
        }

        private void PlayerSpawnInput(object sender, SpawnInput e)
        {
            if (sender is not Player player) return;

            var command = new Spawn(this, player, e);
            command.Execute();
        }
    }
}