using System;
using Cysharp.Threading.Tasks;
using Definitions;
using Game;
using Network.NakamaAdapter;
using Network.NakamaAdapter.MatchMaking;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Main
{
    public class MainMenuController : MonoBehaviour
    {
        private const float ReferenceWidth = 480f;
        private const float ReferenceHeight = 640f;

        private static bool _connected = false;

        private INakamaConnection _nakamaConnection;
        private IMatchmakingApi _matchmakingApi;
        private UserData _userData;
        private UserWallet _userWallet;

        private string _address;
        private int _port;
        private string _salt;


        private bool _isConnected = false;
        private bool _isFindingMatch = false;


        [Inject]
        private void Init(INakamaConnection nakamaConnection, IMatchmakingApi matchmakingApi)
        {
            _nakamaConnection = nakamaConnection;
            _matchmakingApi = matchmakingApi;
        }

        private void Awake()
        {
            _address = NetworkConfig.Instance.Address;
            _port = NetworkConfig.Instance.Port;
            _salt = NetworkConfig.Instance.Salt;

            _matchmakingApi.MatchmakingStarted += OnMatchmakingStarted;
            _matchmakingApi.MatchmakingCanceled += OnMatchmakingCanceled;
            _matchmakingApi.Matched += MatchmakingApiOnMatched;

            _userData = UserData.Instance;
            _userWallet = UserWallet.Instance;
        }

        private void Start()
        {
            if (!_connected) return;
            Connect().Forget();
        }

        private async UniTask Connect()
        {
            var connected = await _nakamaConnection.EnsureConnection();
            if (!connected) return;
            var account = await _nakamaConnection.Client.GetAccountAsync(_nakamaConnection.Session);

            _userData.Set(account.User.Id, account.User.Username, 0);
            _userWallet.Dispatch(account.Wallet);

            _connected = _isConnected = true;
        }

        private async UniTask FindMatch()
        {
            await _matchmakingApi.StartMatchmaking(2, 2);
        }

        private async UniTask CancelFindingMatch()
        {
            await _matchmakingApi.CancelMatchmaking();
        }

        private void OnMatchmakingStarted()
        {
            _isFindingMatch = true;
        }

        private void OnMatchmakingCanceled()
        {
            _isFindingMatch = false;
        }

        [Button]
        private void MatchmakingApiOnMatched(StartingMatchData data)
        {
            var args = new CommandLine.Args
            {
                Mode = "client",
                IpAddress = NetworkConfig.Instance.Address,
                Port = (ushort)data.Port
            };
            CommandLine.Arguments = args;

            SceneManager.LoadScene(sceneBuildIndex: 1, LoadSceneMode.Single);
        }

        void OnGUI()
        {
            var scaleX = Screen.width / ReferenceWidth;
            var scaleY = Screen.height / ReferenceHeight;
            var scale = Mathf.Min(scaleX, scaleY);

            // Scale all GUI
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));


            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = (int)(20 * scale),
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = (int)(18 * scale),
                alignment = TextAnchor.MiddleCenter
            };

            var width = ReferenceWidth / 2;
            var height = ReferenceHeight / 2;
            var x = (ReferenceWidth - width) / 2f;
            var y = (ReferenceHeight - height) / 2f;

            GUILayout.BeginArea(new Rect(x, y, width, height));

            GUILayout.FlexibleSpace();

            if (!_isConnected)
            {
                GUILayout.Label("IP Address:");
                _address = GUILayout.TextField(_address);

                GUILayout.Label("Port:");
                var portText = GUILayout.TextField(_port.ToString());
                _port = ushort.Parse(portText);

                GUILayout.Label("Salt:");
                _salt = GUILayout.TextField(_salt);

                var changed = _address != NetworkConfig.Instance.Address
                              || _port != NetworkConfig.Instance.Port
                              || _salt != NetworkConfig.Instance.Salt;
                if (changed && GUILayout.Button("Submit"))
                {
                    NetworkConfig.Instance.Set(_address, _port, NetworkConfig.Instance.Key, _salt);
                    SceneManager.LoadScene(sceneBuildIndex: 0, LoadSceneMode.Single);
                }

                if (!changed && GUILayout.Button("Connect"))
                {
                    Connect().Forget();
                }
            }
            else
            {
                if (_isFindingMatch)
                {
                    GUILayout.Label("Finding Match...", labelStyle, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Cancel", buttonStyle, GUILayout.ExpandWidth(true)))
                    {
                        CancelFindingMatch().Forget();
                    }
                }
                else
                {
                    GUILayout.Label("Connected", labelStyle, GUILayout.ExpandWidth(true));
                    GUILayout.Label("Wins: " + _userWallet.Wins, labelStyle, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Find Match", buttonStyle, GUILayout.ExpandWidth(true)))
                    {
                        FindMatch().Forget();
                    }
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }
    }
}