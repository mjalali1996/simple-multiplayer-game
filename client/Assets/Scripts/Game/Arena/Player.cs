using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Cards;
using Game.Inputs;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace Game.Arena
{
    public class Player : NetworkBehaviour
    {
        public event EventHandler<SpawnInput> SpawnInput;
        public event Action<Card, Card> CardsChanged;

        [ReadOnly, SerializeField] private int _id;
        public int Id => _id;

        [ReadOnly, SerializeField] private int _zoneId;
        public int ZoneId => _zoneId;

        [SerializeField] private string _userid;
        public string Userid => _userid;


        [SerializeField] private Card[] _initCards;
        [SerializeField] private LayerMask _zonePlaneLayer;

        private NetworkVariable<int> _netPlayerId = new();
        private NetworkVariable<int> _netZoneId = new();
        private NetworkList<Card> _netCards = new();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                foreach (var card in _initCards)
                {
                    _netCards.Add(card);
                }

                var playerId = IArena.Instance.GetNewPlayerId();
                var zoneId = IArena.Instance.GetNewPlayerZoneId();

                _netPlayerId.Value = playerId;
                _id = playerId;

                _netZoneId.Value = zoneId;
                _zoneId = zoneId;
            }

            if (IsClient)
            {
                _netCards.OnListChanged += OnCardsChanged;
                _id = _netPlayerId.Value;
                _zoneId = _netZoneId.Value;
            }

            IArena.Instance.AddPlayer(this);
        }


        public List<Card> GetCards()
        {
            var cards = new List<Card>();
            foreach (var card in _netCards)
            {
                cards.Add(card);
            }

            return cards;
        }


        public void SpawnCard(int cardId, Vector3 position)
        {
            if (GetCard(cardId, out var card, out _) && card.Cooldown == 0)
            {
                card.Cooldown = card.MaxCooldown;
                CardsChanged?.Invoke(card, card);
            }

            SpawnCardServerRpc(cardId, position);
        }

        [ServerRpc]
        private void SpawnCardServerRpc(int cardId, Vector3 position)
        {
            if (!IsServer) return;

            if (!GetCard(cardId, out var card, out var cardIndex)) return;

            if (card.Cooldown > 0) return;

            var zone = IArenaDataHandler.Instance.GetZone(_zoneId);
            if (!zone.Bounds.Contains(position))
            {
                SetCardCooldown(cardIndex, card, 0);
                return;
            }

            var input = new SpawnInput(card, position);
            SpawnInput?.Invoke(this, input);

            CooldownCard(cardIndex, card, card.MaxCooldown).Forget();
        }

        public void SetUserId(string userId)
        {
            _userid = userId;
            SetUserIdServerRpc(userId);
        }

        [ServerRpc]
        private void SetUserIdServerRpc(string userId)
        {
            _userid = userId;
        }

        private async UniTask CooldownCard(int cardIndex, Card card, float cooldown)
        {
            if (!IsServer) return;
            while (cooldown > 0)
            {
                cooldown = Mathf.Clamp(cooldown - Time.deltaTime, 0, float.MaxValue);

                SetCardCooldown(cardIndex, card, cooldown);
                await UniTask.Yield();
            }
        }

        private void SetCardCooldown(int cardIndex, Card card, float cooldown)
        {
            card.Cooldown = cooldown;
            _netCards[cardIndex] = card;
        }

        private bool GetCard(int cardId, out Card card, out int cardIndex)
        {
            card = new Card();
            cardIndex = 0;
            var found = false;
            for (int i = 0; i < _netCards.Count; i++)
            {
                cardIndex = i;
                card = _netCards[i];
                if (card.Id == cardId)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        private void OnCardsChanged(NetworkListEvent<Card> changeEvent)
        {
            CardsChanged?.Invoke(changeEvent.PreviousValue, changeEvent.Value);
        }
    }
}