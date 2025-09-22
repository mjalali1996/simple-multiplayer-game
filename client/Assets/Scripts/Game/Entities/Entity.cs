using System;
using Game.Arena;
using Unity.Netcode;
using UnityEngine;

namespace Game.Entities
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class Entity : NetworkBehaviour
    {
        public event Action<Entity> OnDeath;

        private NetworkVariable<int> _netOwnerId = new();
        public int OwnerId => _netOwnerId.Value;

        private bool _died;

        public void Initialize(Player owner)
        {
            if (!IsServer) return;

            _netOwnerId.Value = owner.Id;
        }

        internal void CallOnDeath()
        {
            if (!IsServer) return;

            OnDeath?.Invoke(this);
        }

        public override void OnNetworkPreDespawn()
        {
            if (!IsClient) return;
            OnDeath?.Invoke(this);
        }
    }
}