using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CardGame
{
    [DisallowMultipleComponent]
    public class TransitionView : MonoBehaviour
    {
        [SerializeField] private Image transitionImage;

        [SerializeField] private float _fadeInDuration = 0.35f;
        [SerializeField] private float _fadeOutDuration = 0.35f;

        Sequence _activeSeq;

        public IEnumerator FadeIn()
        {
            KillActive();
            _activeSeq = DOTween.Sequence().SetUpdate(true);
            _activeSeq.Append(transitionImage.DOFade(1f, _fadeInDuration));
            yield return _activeSeq.WaitForCompletion();
        }

        public IEnumerator FadeOut(Action exitLoadingAction)
        {
            exitLoadingAction?.Invoke();

            KillActive();
            _activeSeq = DOTween.Sequence().SetUpdate(true);
            _activeSeq.Append(transitionImage.DOFade(0f, _fadeOutDuration));
            yield return _activeSeq.WaitForCompletion();
        }

        private void OnDisable() => KillActive();
        private void OnDestroy() => KillActive();

        private void KillActive()
        {
            if (_activeSeq != null && _activeSeq.IsActive()) _activeSeq.Kill();
        }

        public class Pool : MonoMemoryPool<TransitionView>
        {
            protected override void OnSpawned(TransitionView item)
            {
                base.OnSpawned(item);
                item.gameObject.SetActive(true);
                item.transitionImage.color = new Color(0f, 0f, 0f, 0f);
            }

            protected override void OnDespawned(TransitionView item)
            {
                base.OnDespawned(item);
                item.gameObject.SetActive(false);
            }
        }
    }
}