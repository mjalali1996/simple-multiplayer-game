using Game.Arena;
using Game.Entities;
using UnityEngine;

namespace Game.Commands
{
    public class Despawn : ICommand
    {
        private readonly IArena _arena;
        private readonly Entity _data;

        public Despawn(IArena arena, Entity data)
        {
            _arena = arena;
            _data = data;
        }

        public void Execute()
        {
            _arena.RemoveEntity(_data);
            if (_data is MonoBehaviour monoBehaviour)
                Object.Destroy(monoBehaviour);
        }
    }
}