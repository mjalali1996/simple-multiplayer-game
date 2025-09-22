using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Nakama;
using UnityEngine;

namespace Network.NakamaAdapter
{
    public class DeviceNakamaConnection : INakamaConnection, IDisposable
    {
        public event EventHandler ConnectedEventHandler;
        public event EventHandler DisconnectedEventHandler;

        public IClient Client { get; }
        public ISocket Socket { get; }
        public ISession Session { get; private set; }

        private readonly int _connectTimeout = 5;
        private NetworkConfig _connectionData;

        private string _sessionToken;
        
        private bool _hasValues;

        public DeviceNakamaConnection(NetworkConfig connectionData, bool log = true)
        {
            _connectionData = connectionData;
            Client = new Client("http", connectionData.Address, connectionData.Port, connectionData.Key)
            {
#if UNITY_EDITOR
                Logger = log ? new UnityLogger() : null
#endif
            };
            Socket = Client.NewSocket();
            Socket.Connected += SocketOnConnected;
        }

        private void SocketOnConnected()
        {
            RaiseConnectedEvent();
        }

        public async Task<bool> EnsureConnection()
        {
            try
            {
                if (!await EnsureSession())
                {
                    Debug.LogError("Failed to create session");
                    RaiseDisconnectedEvent();
                    return false;
                }

                if (Socket.IsConnected)
                    return true;

                for (var i = 0; i < 5; i++)
                {
                    await ConnectAsync();

                    if (Socket.IsConnected)
                        return true;
                }

                Debug.LogError("Failed to connect to Nakama after 5 tries");
                RaiseDisconnectedEvent();
                return false;
            }

            catch (Exception)
            {
                RaiseDisconnectedEvent();
                return false;
            }
        }

        private async Task<bool> EnsureSession()
        {
            try
            {
                if (Session != null && !Session.HasExpired(DateTime.UtcNow))
                    return true;

                bool success = TryToRestoreSession() || await AuthenticateDevice();
                if (success)
                    Debug.Log("User Id: " + Session.UserId);
                return success;
            }
            catch (SocketException)
            {
                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (WebException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> TryReconnect()
        {
            try
            {
                await ConnectAsync();
            }
            catch (Exception)
            {
                // ignored
            }

            return Socket.IsConnected;
        }

        public void Dispose()
        {
            Socket.CloseAsync();
        }

        protected virtual async void RaiseConnectedEvent()
        {
            await UniTask.SwitchToMainThread();
            ConnectedEventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual async void RaiseDisconnectedEvent()
        {
            await UniTask.SwitchToMainThread();
            DisconnectedEventHandler?.Invoke(this, EventArgs.Empty);
        }


        private Task ConnectAsync()
        {
            return Socket.ConnectAsync(Session, connectTimeout: _connectTimeout);
        }

        private bool TryToRestoreSession()
        {
            Session = Nakama.Session.Restore(_sessionToken);
            return Session != null && !Session.HasExpired(DateTime.UtcNow);
        }

        private async Task<bool> AuthenticateDevice()
        {
            var deviceId = SystemInfo.deviceUniqueIdentifier + _connectionData.Salt;
            Session = await Client.AuthenticateDeviceAsync(deviceId);
            _sessionToken = Session.AuthToken;
            return Session != null && !Session.HasExpired(DateTime.UtcNow);
        }
    }
}