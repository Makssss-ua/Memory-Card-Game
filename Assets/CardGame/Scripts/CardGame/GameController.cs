using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MemoryGame;
using UnityEngine;
using Zenject;

namespace CardGame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private Transform cardContentParent;

        private CardView _firstSelectedCard;
        private CardView _secondSelectedCard;
        
        private CardView _cardViewPrefab;
        
        public Action OnGameStarted;
        public Action OnCardsDisappeared;
        public Action OnGameEnded;
        
        private GameConfig _gameConfig;
        public GameConfig GameConfig => _gameConfig;
        public int CollectedPairs => _currentCards.Count(x => !x.IsInteractable) / 2;
        
        private CardsFactory<CardView> _cardsFactory;
        private List<CardView> _currentCards = new List<CardView>();
        private List<Sprite> _cardIcons = new List<Sprite>();
        
        [Inject]
        public void Construct(GameConfig gameConfig, CardView cardView, AssetsService assetsService, DiContainer container)
        {
            _gameConfig = gameConfig;
            _cardViewPrefab = cardView;
            _cardIcons = assetsService.CardSprites;
        }
        
        private void Start()
        {
            if (_gameConfig == null)
            {
                Debug.LogError("GameConfig is not assigned in GameController.");
                return;
            }
            
            OnCardsDisappeared += CheckGameOver;
            
            _cardsFactory = new CardsFactory<CardView>(_cardViewPrefab, cardContentParent, _gameConfig.CardsPairs, _cardIcons);
        }
        
        public void StartGame()
        {
            Debug.Log("Starting Game");
            _currentCards = _cardsFactory.CreateCards();
            InitActionsToCards(_currentCards);
            
            OnGameStarted?.Invoke();
        }

        private void CheckGameOver()
        {
            if (_currentCards.All(x=> !x.IsInteractable))
            {
                GameOver();
            }
        }
        
        private void GameOver()
        {
            Debug.Log("Game Over");
            OnGameEnded?.Invoke();
        }
        
        private void InitActionsToCards(List<CardView> cards)
        {
            foreach (var card in cards)
            {
                card.OnSelected += OnCardSelected;
                card.OnHided += _cardsFactory.CardsPool.ReturnCard;
            }
        }
        
        private void OnCardSelected(CardView selectedCard)
        {
            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = selectedCard;
                return;
            }
            else
            {
                if(selectedCard == _firstSelectedCard) return;
            }

            if (_secondSelectedCard == null)
            {
                _secondSelectedCard = selectedCard;
            }
            else
            {
                return;
            }

            StartCoroutine(CheckPairs(_firstSelectedCard, _secondSelectedCard));
            
            _firstSelectedCard = null;
            _secondSelectedCard = null;
        }

        private IEnumerator CheckPairs(CardView fCard, CardView sCard)
        {
            while (!fCard.IsSelected || !sCard.IsSelected)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(_gameConfig.DeselectCardsDelay);
            
            if (fCard.IconSprite == sCard.IconSprite)
            {
                Debug.Log("Cards Matched");
                fCard.Hide();
                sCard.Hide();
                OnCardsDisappeared?.Invoke();
            }
            else
            {
                fCard.Deselect();
                sCard.Deselect();
            }
        }

        private void OnDestroy()
        {
            OnGameStarted = null;
            OnCardsDisappeared = null;
            OnGameEnded = null;
        }
    }
}