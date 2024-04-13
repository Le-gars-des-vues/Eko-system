using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreaturePathfinding : MonoBehaviour
{
    const float MIN_PATH_UPDATE_TIME = 0.2f;
    const float PATH_UPDATE_MOVE_THRESHOLD = 0.5f;

    [Header("Follow Path Varialbes")]
    public Transform pathTarget;
    [SerializeField] float turnDist = 5;
    [SerializeField] float stoppingDistance = 1;
    public float speedPercent;
    [SerializeField] float pathFollowThreshold = 3.5f;
    [SerializeField] float pathFollowMaxDistance = 15f;
    //[SerializeField] float reachedEndOfPathThreshold = 0.7f;
    float closestWaypointDistance = 1000f;
    Coroutine pathFollow;
    Coroutine pathUpdate;

    [Header("Path Variables")]
    public Pathway path;
    public int pathIndex;

    [Header("Checks")]
    public bool reachEndOfPath = false;
    CreatureState state;

    private void OnEnable()
    {
        state = GetComponent<CreatureState>();
    }

    public void NewTarget(GameObject _target)
    {
        //StopPathFinding();
        state.isPathfinding = true;
        //pathTarget = _target.transform;
        pathUpdate = StartCoroutine(UpdatePath());
    }

    public void StopPathFinding()
    {
        if (state.debug)
            Debug.Log(gameObject.transform.parent.gameObject.name + " Stopped Pathfinding");
        //path = null;
        state.isPathfinding = false;
        if (pathUpdate != null)
            StopCoroutine(pathUpdate);
        if (pathFollow != null)
            StopCoroutine(pathFollow);
    }

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Pathway(waypoints, transform.position, turnDist, stoppingDistance);
            if (pathFollow != null)
                StopCoroutine(pathFollow);
            pathFollow = StartCoroutine(FollowPath());
        }
        else
        {
            Debug.Log(gameObject.transform.parent.gameObject.name + " has a problem with his pathfinding!");
            ReachedEndOfPath();
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
            PathRequestManager.RequestPath(transform.position, pathTarget.position, OnPathFound, GetComponent<CreatureState>().isFlying);

            float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
            Vector2 targetPosOld = pathTarget.position;

            while (true)
            {
                yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
                //Debug.Log((new Vector2(target.position.x, transform.position.y) - targetPosOld).sqrMagnitude);
                if (((Vector2)pathTarget.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    if (state.debug)
                        Debug.Log("Changed path");
                    PathRequestManager.RequestPath(transform.position, pathTarget.position, OnPathFound, GetComponent<CreatureState>().isFlying);
                    targetPosOld = pathTarget.position;
                }
            }
        }
    }

    public void ReachedEndOfPath()
    {
        reachEndOfPath = true;
        if (pathUpdate != null)
            StopCoroutine(pathUpdate);
        if (pathFollow != null)
            StopCoroutine(pathFollow);
        state.isPathfinding = false;
    }

    IEnumerator FollowPath()
    {
        //state.isPathfinding = true;
        pathIndex = 0;

        speedPercent = 1;

        while (state.isPathfinding)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            bool groundCheck = Physics2D.Raycast(transform.position, (path.lookPoints[pathIndex] - (Vector2)transform.position), pathFollowThreshold, LayerMask.GetMask("Ground"));
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos) && Vector2.Distance(transform.position, path.lookPoints[pathIndex]) < pathFollowThreshold && !groundCheck)
            {
                if (state.debug)
                {
                    Debug.Log("is pathfinding");
                    Debug.Log(pathIndex);
                    Debug.Log(path.finishLineIndex);
                }
                if (pathIndex == path.finishLineIndex)
                {
                    if (state.debug)
                        Debug.Log(gameObject.name + "Reached end of path1!");
                    ReachedEndOfPath();
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
                        if (state.debug)
                            Debug.Log(gameObject.name + "Reached end of path2!");
                        ReachedEndOfPath();
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
        if (path != null && state.isPathfinding)
        {
            if (path != null)
            {
                path.DrawWithGizmos();
                Gizmos.color = Color.green;
                if (state.debug)
                    Gizmos.DrawLine(path.lookPoints[pathIndex], transform.position);
            }
        }
    }
}
