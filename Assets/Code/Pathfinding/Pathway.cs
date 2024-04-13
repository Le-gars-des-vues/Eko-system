using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathway
{
    public readonly Vector2[] lookPoints;
    public readonly Line[] turnBoundaries;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;

    public Pathway(Vector2[] waypoints, Vector2 startPos, float turnDist, float stoppingDistance)
    {
        lookPoints = waypoints;
        turnBoundaries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = startPos;
        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = lookPoints[i];
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDist;
            turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDist);
            previousPoint = turnBoundaryPoint;
        }

        float distanceFromEndPoint = 0;
        for (int i = lookPoints.Length - 1; i > 0; i--)
        {
            distanceFromEndPoint += Vector2.Distance(lookPoints[i], lookPoints[i - 1]);
            if (distanceFromEndPoint > stoppingDistance)
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.green;
        foreach (Vector2 p in lookPoints)
        {
            Gizmos.DrawCube(p, new Vector2(0.5f, 0.5f));
        }

        
        Gizmos.color = Color.white;
        foreach (Line l in turnBoundaries)
        {
            l.DrawWithGizmos(10);
        }
        
    }
}
