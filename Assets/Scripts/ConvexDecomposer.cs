using System.Collections.Generic;
using UnityEngine;

public class ConvexDecomposer
{
    public static List<Vector2[]> SplitToConvex(Vector2[] points)
    {
        //if less than 3 points => not a polygon
        if (points.Length < 3) return new List<Vector2[]>();

        //if the object is already convex => return as is
        if (IsConvex(points))
        {
            return new List<Vector2[]> { points };
        }

        //if the object is already convex => run Ear Clipping
        return Triangulate(points);
    }

    //some math magic to check whether the object is convex
    public static bool IsConvex(Vector2[] points)
    {
        if (points.Length < 3) return false;
        bool res = false;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 p = points[i];
            Vector2 tmp = points[(i + 1) % points.Length];
            Vector2 v = tmp - p;
            Vector2 u = points[(i + 2) % points.Length] - tmp;

            if (i == 0) res = (v.x * u.y - v.y * u.x) >= 0;
            else if (res != (v.x * u.y - v.y * u.x >= 0)) return false;
        }
        return true;
    }

    //separating a concave object into convex parts
    private static List<Vector2[]> Triangulate(Vector2[] points)
    {
        List<Vector2[]> triangles = new List<Vector2[]>();
        List<int> indices = new List<int>();
        for (int i = 0; i < points.Length; i++) indices.Add(i);

        //determining the direction (Clockwise/Counter-Clockwise)
        bool isClockwise = GetArea(points) < 0;

        int indexCount = indices.Count;
        int iterations = 0;
        int maxIterations = indexCount * indexCount; //infinite cycle protection

        //more math magic
        while (indexCount > 2 && iterations < maxIterations)
        {
            iterations++;
            for (int i = 0; i < indexCount; i++)
            {
                int a = indices[i];
                int b = indices[(i + 1) % indexCount];
                int c = indices[(i + 2) % indexCount];

                if (IsEar(points[a], points[b], points[c], points, indices, isClockwise))
                {
                    triangles.Add(new Vector2[] { points[a], points[b], points[c] });
                    indices.RemoveAt((i + 1) % indexCount);
                    indexCount--;
                    break;
                }
            }
        }
        return triangles;
    }

    private static bool IsEar(Vector2 a, Vector2 b, Vector2 c, Vector2[] allPoints, List<int> currentIndices, bool isClockwise)
    {
        //check whether the angle is convex relative to the direction of the traverse
        float cross = (b.x - a.x) * (c.y - b.y) - (b.y - a.y) * (c.x - b.x);
        if (isClockwise && cross > 0) return false;
        if (!isClockwise && cross < 0) return false;

        //check whether there are other points inside this triangle
        for (int i = 0; i < currentIndices.Count; i++)
        {
            Vector2 p = allPoints[currentIndices[i]];
            if (p == a || p == b || p == c) continue;
            if (PointInTriangle(p, a, b, c)) return false;
        }
        return true;
    }

    private static float GetArea(Vector2[] points)
    {
        float area = 0;
        for (int i = 0; i < points.Length; i++)
        {
            int j = (i + 1) % points.Length;
            area += points[i].x * points[j].y;
            area -= points[j].x * points[i].y;
        }
        return area / 2f;
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = (p.x - b.x) * (a.y - b.y) - (a.x - b.x) * (p.y - b.y);
        float d2 = (p.x - c.x) * (b.y - c.y) - (b.x - c.x) * (p.y - c.y);
        float d3 = (p.x - a.x) * (c.y - a.y) - (c.x - a.x) * (p.y - a.y);

        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(hasNeg && hasPos);
    }
}