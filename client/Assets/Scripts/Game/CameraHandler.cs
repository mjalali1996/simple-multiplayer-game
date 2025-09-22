using System.Linq;
using Game.Arena;
using UnityEngine;

namespace Game
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private IGameData _gameData;
        private IArenaDataHandler _arena;

        private void Start()
        {
            _gameData = IGameData.Instance;
            _gameData.OnStateChanged += OnGameStateChanged;

            _arena = IArenaDataHandler.Instance;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state != GameState.Started) return;
            var localPlayer = _arena.GetPlayers().FirstOrDefault(p => p.IsOwner);
            if (localPlayer == null) return;

            var zone = _arena.GetZone(localPlayer.ZoneId);
            zone.SetCamera(_camera);
        }
    }
}