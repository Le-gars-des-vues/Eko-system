using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MonkeyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float nextWaypointDistance = 3f;

    [SerializeField] private float groundCheckLenght;
    [SerializeField] private float monkeyHeight;
    [SerializeField] private float forwardGroundCheckOffset;
    [SerializeField] private float maxHoleDistanceOffset;
    [SerializeField] private Vector2 climbUpOffsets;
    [SerializeField] private Vector2 climbDownOffsets;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;

    private Seeker seeker;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void OnPathComplete(Path pathCompleted)
    {
        if (!pathCompleted.error)
        {
            path = pathCompleted;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        //transform.position = Vector2.MoveTowards(transform.position, ((Vector2)path.vectorPath[currentWaypoint]), Time.deltaTime * speed);

        /*
        RaycastHit2D forwardGroundCheck = Physics2D.Raycast(new Vector2(transform.position.x + forwardGroundCheckOffset, transform.position.y), Vector2.down, groundCheckLenght, LayerMask.GetMask("Ground"));
        if (forwardGroundCheck.collider != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(path.vectorPath[currentWaypoint].x, forwardGroundCheck.point.y + monkeyHeight), Time.deltaTime * speed);
        }
        else
        {
            RaycastHit2D maxHoleDistance = Physics2D.Raycast(new Vector2(transform.position.x + maxHoleDistanceOffset, transform.position.y), Vector2.down, groundCheckLenght, LayerMask.GetMask("Ground"));
            if (maxHoleDistance.collider != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(path.vectorPath[currentWaypoint].x, maxHoleDistance.point.y + monkeyHeight), Time.deltaTime * speed);
            }
            else
            {
                if (path.vectorPath[currentWaypoint].y > transform.position.y)
                {

                }
            }
        }
        */

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position);
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
            currentWaypoint++;
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector2(transform.position.x + forwardGroundCheckOffset, transform.position.y), Vector2.down * groundCheckLenght);
        Gizmos.DrawRay(new Vector2(transform.position.x + maxHoleDistanceOffset, transform.position.y), Vector2.down * groundCheckLenght);
    }
}
