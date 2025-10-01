using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CardGame
{
    public class SceneTransitionService : MonoBehaviour
    {
        private TransitionView.Pool _pool;
        private Coroutine _current;
        private bool _isBusy;

        [Inject]
        public void Construct(TransitionView.Pool pool)
        {
            _pool = pool;
        }

        public void ChangeScene(
            string sceneName,
            Action exitLoadingAction = null,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (_isBusy)
            {
                Debug.LogWarning("[SceneTransitionService] Already running transition.");
                return;
            }

            _current = StartCoroutine(ChangeRoutine(sceneName, mode, exitLoadingAction));
        }

        private IEnumerator ChangeRoutine(string sceneName, LoadSceneMode mode, Action exitLoadingAction)
        {
            _isBusy = true;

            var view = _pool.Spawn();
            DontDestroyOnLoad(view.gameObject);

            yield return view.FadeIn();

            var op = SceneManager.LoadSceneAsync(sceneName, mode);
            op.allowSceneActivation = false;
            while (op.progress < 0.9f)
                yield return null;

            op.allowSceneActivation = true;
            yield return null;

            yield return view.FadeOut(exitLoadingAction);

            _pool.Despawn(view);

            _isBusy = false;
            _current = null;
        }
    }
}