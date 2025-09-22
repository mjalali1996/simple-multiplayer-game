using Game.Arena;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Arena _arena;
        [SerializeField] private ArenaSpawner _arenaSpawner;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Arena>().FromInstance(_arena).AsSingle();
            Container.Bind<ArenaSpawner>().FromInstance(_arenaSpawner).AsSingle();
            Container.Bind<Player>().FromComponentsInHierarchy().AsSingle();
        }
    }
}