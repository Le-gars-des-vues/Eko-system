using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonkeyPathfinding : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float followPathSpeed;
    Rigidbody2D rb;
    Vector2[] path;
    int targetIndex;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound, false);
    }

    public void OnPathFound(Vector2[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = path[0];

        while (true)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), currentWaypoint) < 0.85f)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    targetIndex = 0;
                    path = new Vector2[0];
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            //transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, followPathSpeed * Time.deltaTime);
            //FaceWaypoint();

            Vector2 direction = (currentWaypoint - rb.position).normalized;
            Vector2 force = new Vector2(direction.x, direction.y * 0.8f) * followPathSpeed * Time.deltaTime;
            rb.AddForce(force);

            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(path[i], Vector2.one * 0.2f);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }

    void FaceWaypoint()
    {
        Vector2 direction = path[targetIndex] - new Vector2(transform.position.x, transform.position.y).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
