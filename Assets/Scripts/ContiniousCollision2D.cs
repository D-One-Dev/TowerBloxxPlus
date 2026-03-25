using UnityEngine;
using System.Linq;

public class ContinuousCollision2D
{
    public struct CollisionResult
    {
        public bool IsColliding;
        public float Time; //a frame's fraction (0..1)
        public Vector2 Normal;
    }

    public static CollisionResult CheckCollision(Vector2[] polyA, Vector2 velocityA, Vector2[] polyB, Vector2 velocityB)
    {
        Vector2 relativeVelocity = (velocityA - velocityB) * Time.fixedDeltaTime;
        float tMin = float.NegativeInfinity;
        float tMax = float.PositiveInfinity;
        Vector2 hitNormal = Vector2.zero;

        //getting all normals (axes for check)
        Vector2[] axes = GetAxes(polyA).Concat(GetAxes(polyB)).ToArray();

        foreach (Vector2 axis in axes)
        {
            //projecting polygons on the axis
            var projA = Project(polyA, axis);
            var projB = Project(polyB, axis);

            //projecting relative velocity on the axis
            float velProj = Vector2.Dot(relativeVelocity, axis);

            //calculating distance between projections
            float distStart = GetDistance(projA, projB);

            //if they are already colliding
            if (distStart < 0)
            {
                //default SAR or tMax reduction
            }
            else if (Mathf.Abs(velProj) > 0.0001f)
            {
                //collision's start and end time on this axis
                float t0 = (projB.Min - projA.Max) / velProj;
                float t1 = (projB.Max - projA.Min) / velProj;

                if (t0 > t1) Swap(ref t0, ref t1);

                if (t0 > tMin)
                {
                    tMin = t0;
                    hitNormal = axis;
                }
                tMax = Mathf.Min(tMax, t1);
            }
            else if (distStart > 0)
            {
                //no speed and not currently colliding => collision impossible
                return new CollisionResult { IsColliding = false };
            }
        }

        //if collision's time interval on all axes is valid
        if (tMin >= 0 && tMin <= 1 && tMin <= tMax)
        {
            return new CollisionResult { IsColliding = true, Time = tMin, Normal = hitNormal.normalized };
        }

        return new CollisionResult { IsColliding = false };
    }

    private static (float Min, float Max) Project(Vector2[] vertices, Vector2 axis)
    {
        float min = Vector2.Dot(vertices[0], axis);
        float max = min;
        for (int i = 1; i < vertices.Length; i++)
        {
            float p = Vector2.Dot(vertices[i], axis);
            min = Mathf.Min(min, p);
            max = Mathf.Max(max, p);
        }
        return (min, max);
    }

    private static Vector2[] GetAxes(Vector2[] vertices)
    {
        Vector2[] axes = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 edge = vertices[(i + 1) % vertices.Length] - vertices[i];
            axes[i] = new Vector2(-edge.y, edge.x).normalized; // Перпендикуляр
        }
        return axes;
    }

    private static float GetDistance((float Min, float Max) a, (float Min, float Max) b)
    {
        if (a.Max < b.Min) return b.Min - a.Max;
        if (b.Max < a.Min) return a.Min - b.Max;
        return -1; //colliding
    }

    private static void Swap(ref float a, ref float b)
    {
        (b, a) = (a, b);
    }
}