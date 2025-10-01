using CardGame;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MemoryGame
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameUI;
        [SerializeField] private WinPanel winPanel;
        [SerializeField] private Button startButton;

        private GameController _gameController;
        
        [Inject]
        private void Construct(GameController gameController)
        {
            _gameController = gameController;
        }
        
        private void Start()
        {
            startButton.onClick.AddListener(() =>
            {
                StartNextGame();
                startButton.gameObject.SetActive(false);
                gameUI.SetActive(true);
            });
            _gameController.OnGameEnded += ShowWinPanel;
        }

        private void ShowWinPanel()
        {
            winPanel.Show(StartNextGame);
        }

        private void StartNextGame()
        {
            _gameController.StartGame();
        }
    }
}