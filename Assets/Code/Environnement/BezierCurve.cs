using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    public Vector3[] points;

    public BezierCurve()
    {
        points = new Vector3[4];
    }

    public BezierCurve(Vector3[] points)
    {
        this.points = points;
    }

    public Vector3 StartPosition
    {
        get { return points[0]; }
    }

    public Vector3 EndPosition
    {
        get { return points[3]; }
    }

    public Vector3 GetSegment(float Time)
    {
        Time = Mathf.Clamp01(Time);
        float time = 1 - Time;
        return (time * time * time * points[0])
            + (3 * time * time * Time * points[1])
            + (3 * time * Time * Time * points[2])
            + (3 * Time * Time * Time * points[3]);
    }

    public Vector3[] GetSegments(int subdivision)
    {
        Vector3[] segments = new Vector3[subdivision];

        float time;
        for (int i = 0; i < subdivision; i++)
        {
            time = (float)i / subdivision;
            segments[i] = GetSegment(time);
        }

        return segments;
    }
}
