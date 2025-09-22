using Network.NakamaAdapter;
using Network.NakamaAdapter.MatchMaking;
using UnityEngine;
using Zenject;

namespace Network
{
    public class NetworkInstaller : MonoInstaller
    {
        [SerializeField] private NetworkConfig _networkConfig;

        public override void InstallBindings()
        {
            Container.Bind<NetworkConfig>().FromInstance(_networkConfig).AsSingle();
            Container.Bind<INakamaConnection>().To<DeviceNakamaConnection>().AsSingle();
            Container.Bind<IMatchmakingApi>().To<MatchMaker>().AsSingle();
        }
    }
}