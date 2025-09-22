using Game.Entities;

namespace Game.Arena
{
    public interface IArena
    {
        public static IArena Instance { get; protected set; }
        public ArenaSpawner Spawner { get; }
        public void AddPlayer(Player player);
        public void AddEntity(Player player, Entity entity);
        public void RemoveEntity(Entity entity);
        int GetNewPlayerId();
        int GetNewPlayerZoneId();
    }
}