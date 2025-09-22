using System;
using Unity.Netcode;

namespace Game
{
    [Serializable]
    public struct GameData : INetworkSerializable
    {
        public GameState State;
        public int WinnerId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref State);
            serializer.SerializeValue(ref WinnerId);
        }
    }
}