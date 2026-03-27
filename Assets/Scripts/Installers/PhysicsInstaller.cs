using UnityEngine;
using Zenject;

public class PhysicsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        
        Container.BindInterfacesAndSelfTo<CustomPhysicsEngine>()
            .FromNew()
            .AsSingle()
            .NonLazy();
    }
}