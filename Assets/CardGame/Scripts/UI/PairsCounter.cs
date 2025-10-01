using CardGame;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PairsCounter : MonoBehaviour
{
    [SerializeField] private Text counterText;
    
    private GameController _gameController;
    private int _maxPairs;
        
    [Inject]
    private void Construct(GameController gameController)
    {
        _gameController = gameController;
    }
    
    private void Start()
    {
        _gameController.OnGameStarted += ResetCounter;
        _gameController.OnCardsDisappeared += DecreaseCounter;
        ResetCounter();
    }

    private void DecreaseCounter()
    {
        counterText.text = $"{_gameController.CollectedPairs}/{_maxPairs}";
    }

    private void ResetCounter()
    {
        _maxPairs = _gameController.GameConfig.CardsPairs;
        DecreaseCounter();
    }
}
