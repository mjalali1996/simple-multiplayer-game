using System;
using Cysharp.Threading.Tasks;
using Game;
using Game.Arena;
using Game.Nakama;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using NetworkConfig = Network.NakamaAdapter.NetworkConfig;

namespace Network
{
    public class NetworkBootstrap : MonoBehaviour
    {
        private string _ipAddress = "127.0.0.1";
        private ushort _port = 7777;
        private string _gameId = "";
        private bool _created;

        void OnGUI()
        {
#if UNITY_EDITOR
            if (!NetworkManager.Singleton.IsListening && !_created)
            {
                GUILayout.BeginArea(new Rect(10, 10, 200, 200));
                GUILayout.Label("Network Bootstrap");

                if (GUILayout.Button("Start as Host")) StartHost();

                if (GUILayout.Button("Start as Server")) StartServer();

                GUILayout.Label("IP Address:");
                _ipAddress = GUILayout.TextField(_ipAddress);

                GUILayout.Label("Port:");
                var portText = GUILayout.TextField(_port.ToString());
                _port = ushort.Parse(portText);

                if (GUILayout.Button("Start as Client")) StartClient();

                GUILayout.EndArea();
            }
#endif
        }

        private void Start()
        {
            IGameData.Instance.OnStateChanged += InstanceOnOnStateChanged;
            ExtractEnvs();
        }

        private void InstanceOnOnStateChanged(GameState state)
        {
            if (state != GameState.Finished) return;

            SendMatchResult().Forget();
            KillScene(5).Forget();
        }

        private void ExtractEnvs()
        {
            if (CommandLine.Arguments == null)
            {
                var args = Environment.GetCommandLineArgs();
                // Example: -mode server -ip 192.168.1.10 -port 7777
                for (var i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-id":
                            if (i + 1 < args.Length)
                                _gameId = args[i + 1];
                            break;
                        case "-ip":
                            if (i + 1 < args.Length)
                                _ipAddress = args[i + 1];
                            break;

                        case "-port":
                            if (i + 1 < args.Length && ushort.TryParse(args[i + 1], out var p))
                                _port = p;
                            break;

                        case "-mode":
                            if (i + 1 < args.Length)
                            {
                                var mode = args[i + 1].ToLower();
                                switch (mode)
                                {
                                    case "host":
                                        StartHost();
                                        break;
                                    case "server":
                                        StartServer();
                                        break;
                                    case "client":
                                        StartClient();
                                        break;
                                }
                            }

                            break;
                    }
                }
            }
            else
            {
                _ipAddress = CommandLine.Arguments.IpAddress;
                _port = CommandLine.Arguments.Port;
                var mode = CommandLine.Arguments.Mode;
                CommandLine.Arguments = null;

                switch (mode)
                {
                    case "host":
                        StartHost();
                        break;
                    case "server":
                        StartServer();
                        break;
                    case "client":
                        StartClient();
                        break;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                KillScene(0.5f).Forget();
            }
        }

        private async UniTask SendMatchResult()
        {
#if UNITY_SERVER
            var winnetPlayer = IArenaDataHandler.Instance.GetPlayer(IGameData.Instance.Data.WinnerId);
            await SendEndGame(_gameId, winnetPlayer.Userid);
#endif
        }

        private async UniTask KillScene(float delay = 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            NetworkManager.Singleton.Shutdown();
            Destroy(gameObject);

#if !UNITY_SERVER
            SceneManager.LoadScene(0, LoadSceneMode.Single);
#else
            Application.Quit();
#endif
        }

        private async UniTask SendEndGame(string gameId, string winnerUserId)
        {
            var config = NetworkConfig.Instance;
            var nkApiService = new NkApiService(config.Address, config.Port);
            await nkApiService.SetEndMatchResult(new EndMatchData()
            {
                GameId = gameId,
                WinnerId = winnerUserId
            });

            await nkApiService.DisposeMatch(_gameId);
        }

        private void StartHost()
        {
            SetSetting();

            Debug.Log("Starting as Host...");
            NetworkManager.Singleton.StartHost();
            _created = true;
        }

        private void StartServer()
        {
            SetSetting();

            Debug.Log("Starting as Server...");
            NetworkManager.Singleton.StartServer();
            _created = true;
        }

        private void StartClient()
        {
            SetSetting();

            Debug.Log($"Starting as Client -> {_ipAddress}:{_port}");
            NetworkManager.Singleton.StartClient();
            _created = true;
        }

        private void SetSetting()
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(_ipAddress, _port, _ipAddress);
            Debug.Log($"game id set {_gameId}");
            Debug.Log($"Ip set {_ipAddress}");
            Debug.Log($"Port set {_port}");
        }
    }
}