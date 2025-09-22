using System;
using Unity.Netcode;

namespace Game.Cards
{
    [Serializable]
    public struct Card : INetworkSerializable, IEquatable<Card>
    {
        public int Id;
        public int EntityId;
        public float MaxCooldown;
        public float Cooldown;

        public bool Equals(Card other)
        {
            return Id == other.Id && EntityId == other.EntityId && Cooldown.Equals(other.Cooldown) &&
                   Cooldown.Equals(other.Cooldown);
        }

        public override bool Equals(object obj)
        {
            return obj is Card other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, EntityId, MaxCooldown, Cooldown);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref EntityId);
            serializer.SerializeValue(ref MaxCooldown);
            serializer.SerializeValue(ref Cooldown);
        }
    }
}