using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreaturePathfinding : MonoBehaviour
{
    const float MIN_PATH_UPDATE_TIME = 0.2f;
    const float PATH_UPDATE_MOVE_THRESHOLD = 0.5f;

    [Header("Follow Path Varialbes")]
    public Transform pathTarget;
    public float followPathSpeed;
    [SerializeField] float turnDist = 5;
    [SerializeField] float stoppingDistance = 1;
    public float speedPercent;
    [SerializeField] float pathFollowThreshold = 3.5f;
    [SerializeField] float pathFollowMaxDistance = 15f;
    [SerializeField] float reachedEndOfPathThreshold = 0.7f;
    float closestWaypointDistance = 1000f;

    [Header("Path Variables")]
    public Path path;
    public int pathIndex;

    [Header("Checks")]
    public bool isFlying;
    public bool reachEndOfPath = false;
    CreatureState state;
    bool isStopped;

    private void OnEnable()
    {
        state = GetComponent<CreatureState>();
    }

    public void NewTarget(GameObject _target)
    {
        StopPathFinding();
        state.isPathfinding = true;
        pathTarget = _target.transform;
        StartCoroutine(UpdatePath());
    }

    public void StopPathFinding()
    {
        state.isPathfinding = false;
        pathTarget = null;
        isStopped = true;
        StopAllCoroutines();
    }

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDist, stoppingDistance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
            Debug.Log("Problem with pathfinding");
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        
        if (pathTarget != null)
        {
            PathRequestManager.RequestPath(new PathRequest(transform.position, pathTarget.position, OnPathFound), isFlying);

            float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
            Vector2 targetPosOld = pathTarget.position;

            while (true)
            {
                yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
                //Debug.Log((new Vector2(target.position.x, transform.position.y) - targetPosOld).sqrMagnitude);
                if ((new Vector2(pathTarget.position.x, pathTarget.position.y) - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    //Debug.Log("Changed path");
                    PathRequestManager.RequestPath(new PathRequest(transform.position, pathTarget.position, OnPathFound), isFlying);
                    targetPosOld = new Vector2(transform.position.x, transform.position.y);
                }
            }
        }
    }

    IEnumerator FollowPath()
    {
        state.isPathfinding = true;
        pathIndex = 0;

        speedPercent = 1;

        while (state.isPathfinding)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            //Debug.Log(Vector2.Distance(transform.position, path.lookPoints[pathIndex]));
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos) && Vector2.Distance(transform.position, path.lookPoints[pathIndex]) < pathFollowThreshold)
            {
                //Debug.Log(pathIndex);
                //Debug.Log(path.finishLineIndex);
                if (pathIndex == path.finishLineIndex)
                {
                    Debug.Log("Reached end of path1!");
                    reachEndOfPath = true;
                    StopPathFinding();
                    break;
                }
                else
                    pathIndex++;
            }

            if (state.isPathfinding)
            {
                if ((pathIndex >= path.slowDownIndex && stoppingDistance > 0) || (pathIndex == 0))
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos / stoppingDistance));
                    if (speedPercent < 0.5f)
                    {
                        Debug.Log("Reached end of path2!");
                        reachEndOfPath = true;
                        StopPathFinding();
                    }
                }
            }
            
            if (Vector2.Distance(transform.position, path.lookPoints[pathIndex]) > pathFollowMaxDistance && pathIndex != 0)
            {
                for (int i = pathIndex; i < path.lookPoints.Length; i++)
                {
                    float dist = Vector2.Distance(path.lookPoints[i], transform.position);
                    if (dist < closestWaypointDistance)
                    {
                        //Debug.Log("Changed path index");
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
        if (state != null && state.isPathfinding)
        {
            if (path != null)
            {
                path.DrawWithGizmos();
                Gizmos.color = Color.green;
                Gizmos.DrawLine(path.lookPoints[pathIndex], transform.position);
            }
        }
    }
}
