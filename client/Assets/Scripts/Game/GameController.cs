using System;
using Game.Arena;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public enum GameState
    {
        WaitingForPlayers,
        Started,
        Finished,
    }

    public class GameManager : NetworkBehaviour, IGameData
    {
        public event Action<GameState> OnStateChanged;

        public GameData Data => _netGameData.Value;

        [SerializeField] private CanvasGroup _waitingForPlayersCanvasGroup;
        [SerializeField] private CanvasGroup _winCanvasGroup;
        [SerializeField] private CanvasGroup _loseCanvasGroup;

        private NetworkVariable<GameData> _netGameData = new();

        private void Awake()
        {
            IGameData.Instance = this;

            _waitingForPlayersCanvasGroup.alpha = 1;
            _winCanvasGroup.alpha = 0;
            _loseCanvasGroup.alpha = 0;
            enabled = true;
        }

        public override void OnNetworkSpawn()
        {
            _netGameData.OnValueChanged += OnGameStateChanged;
        }

        private void Start()
        {
            IArenaDataHandlerServer.Instance.OnFulled += OnArenaFulled;
            IArenaDataHandlerServer.Instance.OnFinished += OnArenaFinished;
        }

        private void OnArenaFulled()
        {
            if (!IsServer) return;

            SetGameState(new GameData()
            {
                State = GameState.Started
            });
        }

        private void OnArenaFinished(Player player)
        {
            if (!IsServer) return;

            SetWinner(player.Id);
        }

        private void SetGameState(GameData data)
        {
            if (!IsServer) return;

            _netGameData.Value = data;
        }

        private void OnGameStateChanged(GameData oldValue, GameData newValue)
        {
            if (IsClient)
            {
                if (newValue.State == GameState.Started)
                    _waitingForPlayersCanvasGroup.alpha = 0;

                if (newValue.State == GameState.Finished)
                {
                    var localPlayer = IArenaDataHandler.Instance.GetOwnerPlayer();
                    var win = localPlayer.Id == newValue.WinnerId;
                    ShowFinishUI(win);

                    Debug.Log($"Local Player id: {localPlayer.Id}");
                    Debug.Log($"Winner id: {newValue.WinnerId}");
                }
            }

            OnStateChanged?.Invoke(newValue.State);
        }

        private void SetWinner(int playerId)
        {
            if (!IsServer) return;

            SetGameState(new GameData()
            {
                State = GameState.Finished,
                WinnerId = playerId
            });
        }

        private void ShowFinishUI(bool win)
        {
            _winCanvasGroup.alpha = win ? 1 : 0;
            _loseCanvasGroup.alpha = win ? 0 : 1;
        }
    }
}