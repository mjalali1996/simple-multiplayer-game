using System;
using System.Threading.Tasks;

namespace Network.NakamaAdapter.MatchMaking
{
    public interface IMatchmakingApi
    {
        event Action MatchmakingStarted;
        event Action MatchmakingCanceled;
        event Action<StartingMatchData> Matched;
        bool IsInMatchmaking { get; }
        bool IsMatched { get; }
        Task<bool> StartMatchmaking(int min, int max);
        Task CancelMatchmaking();
    }
}