using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonkeyPathfinding : MonoBehaviour
{
    const float MIN_PATH_UPDATE_TIME = 0.2f;
    const float PATH_UPDATE_MOVE_THRESHOLD = 0.5f;

    public Transform target;
    public float followPathSpeed;
    [SerializeField] float turnDist = 5;
    [SerializeField] float stoppingDistance = 10;

    public Path path;
    public int pathIndex;

    public bool isPathfinding;
    public float speedPercent;

    private void Start()
    {
        StartCoroutine(UpdatePath());
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
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound), false);

        float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
        Vector2 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);
            //Debug.Log((new Vector2(target.position.x, transform.position.y) - targetPosOld).sqrMagnitude);
            if ((new Vector2(target.position.x, target.position.y) - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound), false);
                targetPosOld = new Vector2(transform.position.x, transform.position.y);
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        pathIndex = 0;
        //transform.LookAt(path.lookPoints[0]);

        speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                    pathIndex++;
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDistance > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos / stoppingDistance));
                    if (speedPercent < 0.01f)
                        followingPath = false;
                }

                //Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - new Vector2(transform.position.x, transform.position.y));
                //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            }
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
