using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class PhysicsEntity : MonoBehaviour
{
    [SerializeField] private Vector2 velocity;
    [SerializeField] private PolygonCollider2D polygonCollider;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private bool isFixed;
    [SerializeField] private bool simulated = true;

    [HideInInspector] public Vector2 Velocity => velocity;
    [HideInInspector] public PolygonCollider2D PolygonCollider => polygonCollider;
    [HideInInspector] public Rigidbody2D RB => rb;
    [HideInInspector] public bool IsFixed => isFixed;
    [HideInInspector] public bool Simulated => simulated;

    //list of convex polygons
    [HideInInspector] public List<Vector2[]> decomposedParts = new List<Vector2[]>();

    void Awake()
    {
        if (polygonCollider == null) polygonCollider = GetComponent<PolygonCollider2D>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        //going through all paths if the collider has many
        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            Vector2[] path = polygonCollider.GetPath(i);

            //split concave object into convex parts
            var convexParts = ConvexDecomposer.SplitToConvex(path);
            decomposedParts.AddRange(convexParts);
        }
    }

    //transform a part's local coords to global
    public Vector2[] GetWorldVertices(Vector2[] localPart)
    {
        Vector2[] worldVerts = new Vector2[localPart.Length];
        for (int i = 0; i < localPart.Length; i++)
        {
            worldVerts[i] = transform.TransformPoint(localPart[i]);
        }
        return worldVerts;
    }

    public void SetVelocity(Vector2 vel)
    {
        velocity = vel;
    }

    public void SetSimulated(bool sim)
    {
        simulated = sim;
    }
}