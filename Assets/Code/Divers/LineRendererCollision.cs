using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class LineRendererCollision : MonoBehaviour
{
    EdgeCollider2D edgeCollider;
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetEdgeCollider(line);
    }

    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector2 lineRendererPoint = transform.InverseTransformPoint(lineRenderer.GetPosition(i));
            edges.Add(lineRendererPoint);
        }
        edgeCollider.SetPoints(edges);
    }
}
