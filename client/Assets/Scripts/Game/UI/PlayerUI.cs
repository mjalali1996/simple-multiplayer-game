using System.Collections.Generic;
using System.Linq;
using Definitions;
using Game.Arena;
using Game.Cards;
using UnityEngine;
using Zenject;

namespace Game.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CardView _cardViewPrefab;
        [SerializeField] private Transform _cardsContainer;
        private readonly Dictionary<int, CardView> _cardViews = new();

        private IGameData _gameData;
        private IArenaDataHandler _arena;

        private Player _localPlayer;

        private void Awake()
        {
            SetActive(false);
        }

        private void Start()
        {
            _gameData = IGameData.Instance;
            _gameData.OnStateChanged += OnGameStateChanged;

            _arena = IArenaDataHandler.Instance;
        }

        private void OnGameStateChanged(GameState state)
        {
            SetActive(state == GameState.Started);

            if (state != GameState.Started) return;

            _localPlayer = _arena.GetPlayers().FirstOrDefault(p => p.IsOwner);
            if (_localPlayer == null)
            {
                SetActive(false);
                return;
            }

            var zone = _arena.GetZone(_localPlayer.ZoneId);

            var cards = _localPlayer.GetCards();
            foreach (var card in cards)
            {
                CreateCard(card, zone.LayerMask);
            }

            _localPlayer.CardsChanged += OnCardsChanged;

            _localPlayer.SetUserId(UserData.Instance.UserId);
        }

        private void CreateCard(Card card, LayerMask playerZoneMask)
        {
            var cardView = Instantiate(_cardViewPrefab, _cardsContainer);
            cardView.Initialize(card, playerZoneMask);
            cardView.OnSpawnRequested += OnSpawnRequested;
            _cardViews.Add(card.Id, cardView);
        }

        private void OnSpawnRequested(int cardId, Vector3 position)
        {
            _localPlayer?.SpawnCard(cardId, position);
        }

        private void SetActive(bool value)
        {
            _canvasGroup.alpha = value ? 1 : 0;
            _canvasGroup.blocksRaycasts = value;
            _canvasGroup.interactable = value;
        }

        private void OnCardsChanged(Card previousCard, Card newCard)
        {
            if (previousCard.Id != newCard.Id) return;

            var cardView = _cardViews[newCard.Id];
            cardView.SetCooldown(newCard.Cooldown, newCard.MaxCooldown);
        }
    }
}