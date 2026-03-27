using System.Linq;
using UnityEngine;
using Zenject;

public class BlockSpawner: System.IDisposable
{
    [Inject(Id = "BlockSpawnPoint")]
    private readonly Transform _blockSpawnPoint;
    [Inject(Id = "BlockPrefabs")]
    private readonly GameObject[] _blockPrefabs;

    private DiContainer _container;
    private EventHandler _eventHandler;

    [Inject]
    public void Construct(DiContainer container, EventHandler eventHandler)
    {
        _container = container;
        _eventHandler = eventHandler;
        _eventHandler.OnSpawnBlock += SpawnBlock;

        SpawnBlock();
    }

    private void SpawnBlock()
    {
        _container.InstantiatePrefab(_blockPrefabs[Random.Range(0, _blockPrefabs.Count())], _blockSpawnPoint.position,
            Quaternion.identity, _blockSpawnPoint);
    }

    public void Dispose()
    {
        _eventHandler.OnSpawnBlock -= SpawnBlock;
    }
}
