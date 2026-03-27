using UnityEngine;
using System.Collections.Generic;
using static ContinuousCollision2D;
using Zenject;
using System;

public class CustomPhysicsEngine: IFixedTickable, IDisposable
{
    private List<PhysicsEntity> _entities;
    private EventHandler _eventHandler;

    [Inject]
    public void Construct(EventHandler eventHandler)
    {
        _eventHandler = eventHandler;
        _eventHandler.OnRegisterPhysicsEntity += RegisterPhysicsEntity;
    }

    private void RegisterPhysicsEntity(PhysicsEntity entity)
    {
        _entities ??= new List<PhysicsEntity>();
        _entities.Add(entity);
    }

    public void FixedTick()
    {
        foreach (PhysicsEntity entity in _entities)
        {
            PerformMovement(entity);
        }
    }

    private void PerformMovement(PhysicsEntity a)
    {
        float minTOI = 1.0f; // Time of Impact (0..1)
        bool hitOccurred = false;

        Vector2 movementStep = a.Velocity * Time.fixedDeltaTime;

        foreach (PhysicsEntity b in _entities)
        {
            if (a == b || !a.Simulated) continue;

            //checking each part of object A against each part of object B
            foreach (Vector2[] partA in a.decomposedParts)
            {
                Vector2[] vertsA = a.GetWorldVertices(partA);

                foreach (Vector2[] partB in b.decomposedParts)
                {
                    Vector2[] vertsB = b.GetWorldVertices(partB);

                    CollisionResult result = CheckCollision(vertsA, a.Velocity, vertsB, b.Velocity);

                    if (result.IsColliding && result.Time < minTOI)
                    {
                        minTOI = result.Time;
                        hitOccurred = true;
                    }
                }
            }
        }

        if (hitOccurred)
        {
            //moving object exactly to collision point with a tiny bias
            float safeTime = Mathf.Max(0, minTOI - 0.001f);
            a.transform.position += (Vector3)(a.Velocity * Time.fixedDeltaTime * safeTime);

            if (!a.IsFixed)
            {
                a.SetVelocity(Vector2.zero);
                a.PolygonCollider.enabled = true;
                a.SetSimulated(false);
                a.RB.simulated = true;
                _eventHandler.SpawnBlock();
            }
        }
        else
        {
            //free movement if no collision
            a.transform.position += (Vector3)movementStep;
        }
    }

    public void Dispose()
    {
        _eventHandler.OnRegisterPhysicsEntity -= RegisterPhysicsEntity;
    }
}