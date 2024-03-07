using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [Header("Trajectory Line Smoothess/Length")]
    [SerializeField] int segmentCount = 50;
    [SerializeField] float curveLength = 3.5f;

    Vector2[] segments;
    private LineRenderer line;
    ThrowableObject objectToThrow;

    const float TIME_CURVE_ADDITION = 0.5f;

    private void OnEnable()
    {
        //Initialise segments
        segments = new Vector2[segmentCount];
        objectToThrow = GetComponent<ThrowableObject>();

        //Grab line renderer and set its number of point
        line = GetComponent<LineRenderer>();
        line.positionCount = segmentCount;
    }

    public void CalculateTrajectory()
    {
        //Set the start position of the line renderer
        Vector2 startPos = transform.position;
        segments[0] = startPos;
        line.SetPosition(0, startPos);

        //Set the starting velocity;
        Vector2 startVelocity = ((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized * objectToThrow.force);

        for (int i = 1; i < segmentCount; i++)
        {
            //time offset for the "ghost" projectile
            float timeOFfset = (i * Time.fixedDeltaTime * curveLength);

            //gravity offset for the "ghost" projectile
            Vector2 gravityOffset = TIME_CURVE_ADDITION * Physics2D.gravity * 1 * Mathf.Pow(timeOFfset, 2);

            segments[i] = segments[0] + startVelocity * timeOFfset + gravityOffset;
            line.SetPosition(i, segments[i]);
        }
    }
}
