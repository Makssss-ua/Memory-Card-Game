using MemoryGame;
using UnityEngine;
using Zenject;

namespace CardGame
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private CardView cardPrefab;

        public override void InstallBindings()
        {
            Container.Bind<GameConfig>().FromInstance(gameConfig).AsSingle();
            Container.Bind<GameController>().FromInstance(gameController).AsSingle();
            Container.Bind<CardView>().FromInstance(cardPrefab).AsSingle();
            Container.ResolveType<AssetsService>();
        }
    }
}
