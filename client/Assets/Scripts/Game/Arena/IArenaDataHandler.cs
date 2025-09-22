using System.Collections.Generic;

namespace Game.Arena
{
    public interface IArenaDataHandler
    {
        public static IArenaDataHandler Instance { get; protected set; }
        public int ArenaZoneSize { get; }
        public IZone GetZone(int zoneId);
        public Player GetOwnerPlayer();
        public Player GetPlayer(int playerId);
        public IReadOnlyList<Player> GetPlayers();
        public IReadOnlyList<Player> GetEnemyPlayers(int playerId);
    }
}