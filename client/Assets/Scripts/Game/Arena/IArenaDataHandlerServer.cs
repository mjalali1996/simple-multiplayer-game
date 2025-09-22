using System;
using System.Collections.Generic;
using Game.Entities;

namespace Game.Arena
{
    public interface IArenaDataHandlerServer
    {
        public static IArenaDataHandlerServer Instance { get; protected set; }

        public event Action OnFulled;
        public event Action<Player> OnFinished;

        public IReadOnlyList<Player> GetPlayers();
        public IReadOnlyList<Player> GetEnemyPlayers(int playerId);
        public IReadOnlyList<Entity> GetPlayerEntities(Player player);
    }
}