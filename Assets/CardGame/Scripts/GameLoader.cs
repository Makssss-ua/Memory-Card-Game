using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CardGame
{
    public class GameLoader : MonoBehaviour
    {
        [Inject] private AssetsService _assetsService;
        [Inject] private SceneTransitionService  _transitions;

        private void Start()
        {
            StartCoroutine(LoadGame());
        }
        
        private IEnumerator LoadGame()
        {
            yield return _assetsService.LoadImagesAsync().ToCoroutine();
            
            _transitions.ChangeScene("CardGame");
        }
    }
}
