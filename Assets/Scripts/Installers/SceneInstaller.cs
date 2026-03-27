using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private Transform blockSpawnPoint;
    [SerializeField] private GameObject[] blockPrefabs;
    public override void InstallBindings()
    {
        Container.Bind<Transform>()
            .WithId("BlockSpawnPoint")
            .FromInstance(blockSpawnPoint)
            .AsCached();

        Container.Bind<GameObject[]>()
            .WithId("BlockPrefabs")
            .FromInstance(blockPrefabs)
            .AsCached();



        Container.Bind<EventHandler>()
            .FromNew()
            .AsSingle()
            .NonLazy();

        Container.Bind<BlockSpawner>()
            .FromNew()
            .AsSingle()
            .NonLazy();
    }
}