using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

namespace Network.NakamaAdapter.MatchMaking
{
    public class MatchMaker : IMatchmakingApi, IDisposable
    {
        public bool IsInMatchmaking { get; private set; }
        public bool IsMatched { get; private set; }
        public event Action MatchmakingStarted;
        public event Action MatchmakingCanceled;
        public event Action<StartingMatchData> Matched;

        private readonly INakamaConnection _nakama;

        private IMatchmakerTicket _matchmakingTicket;
        private CancellationTokenSource _cancellationToken;

        public MatchMaker(INakamaConnection nakama)
        {
            _nakama = nakama;
            nakama.Socket.ReceivedMatchmakerMatched += SocketOnReceivedMatchmakerMatched;
        }

        private async void SocketOnReceivedMatchmakerMatched(IMatchmakerMatched obj)
        {
            IsMatched = true;
            IsInMatchmaking = false;
            _matchmakingTicket = null;
            var info = JsonConvert.DeserializeObject<StartingMatchData>(obj.MatchId);
            await UniTask.SwitchToMainThread();
            Matched?.Invoke(info);
        }


        public void Dispose()
        {
            _nakama.Socket.ReceivedMatchmakerMatched -= SocketOnReceivedMatchmakerMatched;
        }

        public async Task<bool> StartMatchmaking(int min, int max)
        {
            if (_matchmakingTicket != null)
                throw new InvalidOperationException("Already in matchmaking");

            _cancellationToken = new CancellationTokenSource();

            IsMatched = false;

            if (!await _nakama.EnsureConnection())
            {
                Debug.LogError("Failed to establish socket connection");
                return false;
            }

            Debug.Log($"Adding to matchmaking ({min},{max})");

            try
            {
                _ = Task.Run(async () =>
                {
                    _matchmakingTicket = await _nakama.Socket.AddMatchmakerAsync("", min, max);
                    _cancellationToken.Token.ThrowIfCancellationRequested();

                    IsInMatchmaking = true;
                    await UniTask.SwitchToMainThread();
                    MatchmakingStarted?.Invoke();
                }, _cancellationToken.Token);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task RemoveFromMatchMaker()
        {
            await _nakama.Socket.RemoveMatchmakerAsync(_matchmakingTicket);
        }

        public async Task CancelMatchmaking()
        {
            if (IsMatched)
                return;

            if (_matchmakingTicket != null)
            {
                try
                {
                    if (!await _nakama.EnsureConnection())
                        Debug.LogError("Socket connection is lost!");

                    _cancellationToken.Cancel();
                    await RemoveFromMatchMaker();
                    Debug.Log("Canceled matchmaking");
                    await UniTask.SwitchToMainThread();
                    MatchmakingCanceled?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            _matchmakingTicket = null;
            IsInMatchmaking = false;
        }
    }
}