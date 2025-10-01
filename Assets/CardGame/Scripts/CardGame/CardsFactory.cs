using System.Collections.Generic;
using MemoryGame;
using UnityEngine;

namespace CardGame
{
    public class CardsFactory<T> where T : CardView
    {
        private readonly T _cardViewPrefab;
        private readonly Transform _parentTransform;
        private readonly int _cardsPairs;
        private readonly List<Sprite> _cardsIcons;

        public CardsPool<T> CardsPool { get; private set; }
        
        public CardsFactory(T cardViewPrefab, Transform parentTransform, int cardsPairs, List<Sprite> cardsIcons)
        {
            _cardViewPrefab = cardViewPrefab;
            _parentTransform = parentTransform;
            _cardsPairs = cardsPairs;
            
            _cardsIcons = cardsIcons;
            
            if(_cardsIcons.Count < _cardsPairs)
                Debug.LogError("Not enough card icons to create pairs!");

            CardsPool = new CardsPool<T>(cardViewPrefab, parentTransform, cardsPairs * 2);
        }

        public List<T> CreateCards()
        {
            var spritePairs = CreateSpritePairs();

            ShuffleSprites(spritePairs);

            var createdCards = CardsPool.GetCards(_cardsPairs * 2);
            for (int i = 0; i < createdCards.Count; i++)
            {
                var card = createdCards[i];
                card.Init(spritePairs[i]);
            }

            return createdCards;
            
            List<Sprite> CreateSpritePairs()
            {
                var availableSprites = new List<Sprite>(_cardsIcons);
                var sp = new List<Sprite>();
            
                for (int i = 0; i < _cardsPairs; i++)
                {
                    int index = Random.Range(0, availableSprites.Count);
                    var icon = availableSprites[index];
                
                    sp.Add(icon);
                    sp.Add(icon);
                
                    availableSprites.RemoveAt(index);
                }
                
                return sp;
            }
            
            void ShuffleSprites(List<Sprite> spriteList)
            {
                for (int i = spriteList.Count - 1; i > 0; i--)
                {
                    int randomIndex = Random.Range(0, i + 1);
                    (spriteList[i], spriteList[randomIndex]) = (spriteList[randomIndex], spriteList[i]);
                }
            }
        }
    }
    
    public class CardsPool<T> where T : CardView
    {
        private readonly T _cardViewPrefab;
        private readonly Transform _parentTransform;
        private readonly int _initialSize;

        private readonly Queue<T> _pool = new Queue<T>();

        public CardsPool(T cardViewPrefab, Transform parentTransform, int initialSize)
        {
            _cardViewPrefab = cardViewPrefab;
            _parentTransform = parentTransform;
            _initialSize = initialSize;
        }

        public T GetCard()
        {
            if (_pool.Count > 0)
            {
                var card = _pool.Dequeue();
                return card;
            }
            else
            {
                var card = Object.Instantiate(_cardViewPrefab, _parentTransform);
                return card;
            }
        }

        public List<T> GetCards(int count)
        {
            var cards = new List<T>();
            for (int i = 0; i < count; i++)
            {
                cards.Add(GetCard());
            }
            return cards;
        }
        
        public void ReturnCard(T card)
        {
            card.IsInteractable = false;
            _pool.Enqueue(card);
        }
    }
}