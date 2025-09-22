using System;

namespace Game
{
    public interface IGameData
    {
        public static IGameData Instance { get; protected set; }

        public event Action<GameState> OnStateChanged;
        public GameData Data { get; }
    }
}