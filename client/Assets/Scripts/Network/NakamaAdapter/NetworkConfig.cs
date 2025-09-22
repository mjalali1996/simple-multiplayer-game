using Common;
using UnityEngine;

namespace Network.NakamaAdapter
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName = "Config/Network Config")]
    public class NetworkConfig : ScriptableSingleton<NetworkConfig>
    {
        [SerializeField] private string _address = "127.0.0.1";
        public string Address => _address;
        [SerializeField] private int _port = 7350;
        public int Port => _port;

        [SerializeField] private string _key = "defaultkey";
        public string Key => _key;

        [SerializeField] private string _salt;
        public string Salt => _salt;

        public void Set(string address, int port, string key, string salt)
        {
            _address = address;
            _port = port;
            _key = key;
            _salt = salt;
        }
    }
}