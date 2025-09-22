using System;

namespace Network.NakamaAdapter
{
    [Serializable]
    public class ConnectionConfig
    {
        public string Address = "127.0.0.1";
        public int Port = 7350;
        public string Key = "defaultkey";
        public string Salt = "salt";
    }
}