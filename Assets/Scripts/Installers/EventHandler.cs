using System;

public class EventHandler
{
    public event Action<PhysicsEntity> OnRegisterPhysicsEntity;
    public event Action OnSpawnBlock;

    public void RegisterPhysicsEntity(PhysicsEntity entity)
    {
        OnRegisterPhysicsEntity?.Invoke(entity);
    }
    public void SpawnBlock()
    {
        OnSpawnBlock?.Invoke();
    }
}
