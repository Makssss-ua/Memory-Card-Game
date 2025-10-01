using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CardGame
{
    public class ServicesInstaller : MonoInstaller<ServicesInstaller>
    {
        [SerializeField] private TransitionView transitionPrefab;

        public override void InstallBindings()
        {
            Debug.Log("ServicesInstaller: Installing services...");
            Container.Bind<SceneTransitionService>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            Container.BindMemoryPool<TransitionView, TransitionView.Pool>()
                .WithInitialSize(1)
                .FromComponentInNewPrefab(transitionPrefab)
                .UnderTransformGroup("Transitions");

            Container.BindInterfacesAndSelfTo<AssetsService>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}
