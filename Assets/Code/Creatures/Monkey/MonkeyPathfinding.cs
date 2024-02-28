using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonkeyPathfinding : MonoBehaviour
{
    const float MIN_PATH_UPDATE_TIME = 0.2f;
    const float PATH_UPDATE_MOVE_THRESHOLD = 0.5f;

    public Transform pathTarget;
    public float followPathSpeed;
    [SerializeField] float turnDist = 5;
    [SerializeField] float stoppingDistance = 10;

    public Path path;
    public int pathIndex;

    public bool isPathfinding;
    public float speedPercent;

    [SerializeField] MonkeyMovement monkey;
    [SerializeField] float pathFollowThreshold;
    [SerializeField] float pathFollowMaxDistance;
    float closestWaypointDistance = 1000f;

    public void NewTarget(GameObject _target)
    {
        StopCoroutine(UpdatePath());
        isPathfinding = true;
        pathTarget = _target.transform;
        StartCoroutine(UpdatePath());
    }

    public void StopPathFinding()
    {
        StopCoroutine(UpdatePath());
        isPathfinding = false;
    }

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDist, stoppingDistance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        
        if (pathTarget != null)
        {
            PathRequestManager.RequestPath(new PathRequest(transform.position, pathTarget.position, OnPathFound), false);

            float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
            Vector2 targetPosOld = pathTarget.position;

            while (true)
            {
                yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
                //Debug.Log((new Vector2(target.position.x, transform.position.y) - targetPosOld).sqrMagnitude);
                if ((new Vector2(pathTarget.position.x, pathTarget.position.y) - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(transform.position, pathTarget.position, OnPathFound), false);
                    targetPosOld = new Vector2(transform.position.x, transform.position.y);
                }
            }
        }
    }

    IEnumerator FollowPath()
    {
        isPathfinding = true;
        pathIndex = 0;
        //transform.LookAt(path.lookPoints[0]);

        speedPercent = 1;

        while (isPathfinding)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            //Debug.Log(Vector2.Distance(transform.position, path.lookPoints[pathIndex]));
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos) && Vector2.Distance(transform.position, path.lookPoints[pathIndex]) < pathFollowThreshold)
            {
                if (pathIndex == path.finishLineIndex)
                {
                    isPathfinding = false;
                    break;
                }
                else
                    pathIndex++;
            }

            if (isPathfinding)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDistance > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos / stoppingDistance));
                    if (speedPercent < 0.01f)
                        isPathfinding = false;
                }

                //Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - new Vector2(transform.position.x, transform.position.y));
                //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            }

            if (Vector2.Distance(transform.position, path.lookPoints[pathIndex]) > pathFollowMaxDistance)
            {
                for (int i = pathIndex; i < path.lookPoints.Length; i++)
                {
                    float dist = Vector2.Distance(path.lookPoints[i], transform.position);
                    if (dist < closestWaypointDistance)
                    {
                        closestWaypointDistance = dist;
                        pathIndex = i;
                    }
                }
            }
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
            Gizmos.color = Color.green;
            Gizmos.DrawLine(path.lookPoints[pathIndex], transform.position);
        }
    }
}
