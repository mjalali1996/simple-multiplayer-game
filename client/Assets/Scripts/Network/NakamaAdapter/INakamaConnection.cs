using System;
using System.Threading.Tasks;
using Nakama;

namespace Network.NakamaAdapter
{
    public interface INakamaConnection
    {
        event EventHandler ConnectedEventHandler;
        event EventHandler DisconnectedEventHandler;
        IClient Client { get; }
        ISocket Socket { get; }
        ISession Session { get; }
        Task<bool> EnsureConnection();
    }
}