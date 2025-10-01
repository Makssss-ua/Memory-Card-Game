using System;
using CardGame;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MemoryGame
{
    public abstract class CardView : MonoBehaviour
    {
        public abstract bool IsInteractable { get; set; }
        public abstract bool IsSelected { get; }
        public abstract Sprite IconSprite { get; }
        public abstract Action<CardView> OnSelected { get; set; }
        public abstract Action<CardView> OnHided { get; set; }
        public abstract void Init(Sprite iconSprite);
        public abstract void Select();
        public abstract void Deselect();
        public abstract void Hide();
    }

    public class Card : CardView
    {
        [SerializeField] private GameObject front;
        [SerializeField] private GameObject back;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool _isInteractable = true;
        private bool _isSelected;
        private Sprite _iconSprite;
        private float _animationDuration = 0.3f;
        
        public override bool IsInteractable
        {
            get => _isInteractable;
            set => _isInteractable = value;
        }
        
        public override bool IsSelected => _isSelected;
        public override Sprite IconSprite => _iconSprite;
        public override Action<CardView> OnSelected { get; set; }
        public override Action<CardView> OnHided { get; set; }
        
        
        [Inject]
        private void Construct(GameConfig gameConfig)
        {
            _animationDuration = gameConfig.CardFlipAnim;
        }
        
        public override void Init(Sprite iconSprite)
        {
            back.SetActive(true);
            front.SetActive(false);
            OnSelected = null;
            OnHided = null;
            _isSelected = false;
            _isInteractable = true;
            canvasGroup.alpha = 1;
            icon.sprite = iconSprite;
            _iconSprite = iconSprite;
        }
        
        private void Start()
        {
            button.onClick.AddListener(Select);
        }
        
        public override void Select()
        {
            if (!_isInteractable || _isSelected) return;
            
            OnSelected?.Invoke(this);
            transform.DORotate(new Vector3(0, 90, 0), _animationDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                back.SetActive(false);
                front.SetActive(true);
                transform.DORotate(Vector3.zero, _animationDuration);
                _isSelected = true;
            });
        }

        public override void Deselect()
        {
            if (!_isInteractable || !_isSelected) return;
            
            transform.DORotate(new Vector3(0, 90, 0), _animationDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                back.SetActive(true);
                front.SetActive(false);
                transform.DORotate(Vector3.zero, _animationDuration);
                _isSelected = false;
            });
        }

        public override void Hide()
        {
            _isInteractable = false;
            canvasGroup.DOFade(0, _animationDuration)
                .OnComplete(() =>
                {
                    OnHided?.Invoke(this);
                });
        }

        private void OnDestroy()
        {
            OnSelected = null;
            OnHided = null;
        }
    }
}
