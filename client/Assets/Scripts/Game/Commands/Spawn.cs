using Game.Arena;
using Game.Inputs;

namespace Game.Commands
{
    public class Spawn : ICommand
    {
        private readonly IArena _arena;
        private readonly Player _owner;
        private readonly SpawnInput _data;

        public Spawn(IArena arena, Player player, SpawnInput data)
        {
            _arena = arena;
            _owner = player;
            _data = data;
        }

        public void Execute()
        {
            var entity = _arena.Spawner.CreateEntity(_owner, _data.Card.EntityId, _data.Position);
            _arena.AddEntity(_owner, entity);
        }
    }
}