using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MonkeyAI : MonoBehaviour
{
    [Header("Pathfinding Variables")]
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;
    private Seeker seeker;
    private Rigidbody2D rb;
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float nextWaypointDistance = 3f;

    [Header("Ground Variables")]
    [SerializeField] private float groundCheckLenght;
    [SerializeField] private float monkeyHeight;
    [SerializeField] private float forwardGroundOffset1;
    [SerializeField] private float forwardGroundOffset2;
    [SerializeField] private float forwardGroundOffset3;
    [SerializeField] private float maxHoleDistanceOffset;

    [Header("Climb Variables")]
    [SerializeField] private Transform tempTarget;
    [SerializeField] private Vector2 climbUpOffsets;
    [SerializeField] private Vector2 climbDownOffsets;
    private bool isTurningCorner;
    private bool isClimbingUp;
    private bool isClimbingDown;


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
            RaycastHit2D maxHoleDistance = Physics2D.Raycast(new Vector2(transform.position.x + forwardGroundCheck3, transform.position.y), Vector2.down, groundCheckLenght, LayerMask.GetMask("Ground"));
            if (maxHoleDistance.collider != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(path.vectorPath[currentWaypoint].x, maxHoleDistance.point.y + monkeyHeight), Time.deltaTime * speed);
            }
            else
            {
                if (path.vectorPath[currentWaypoint].y - transform.position.y > 0)
                {
                    RaycastHit2D climbUpCheck = Physics2D.Raycast(new Vector2(transform.position.x + climbUpOffsets.x, transform.position.y + climbUpOffsets.y), Vector2.left, groundCheckLenght, LayerMask.GetMask("Ground"));
                    if (climbUpCheck.collider != null)
                    {
                        if (!isTurningCorner)
                        {
                            tempTarget.position = new Vector2(transform.position.x + climbUpOffsets.x, transform.position.y + climbUpOffsets.y);
                            isTurningCorner = true;
                            isClimbingUp = true;
                            isClimbingDown = false;
                        }
                    }
                }
                else
                {
                    RaycastHit2D climbDownCheck = Physics2D.Raycast(new Vector2(transform.position.x + climbUpOffsets.x, transform.position.y - climbUpOffsets.y), Vector2.left, groundCheckLenght, LayerMask.GetMask("Ground"));
                    if (climbDownCheck.collider != null)
                    {
                        if (!isTurningCorner)
                        {
                            tempTarget.position = new Vector2(transform.position.x + climbUpOffsets.x, transform.position.y - climbUpOffsets.y);
                            isTurningCorner = true;
                            isClimbingDown = true;
                            isClimbingDown = false;
                        }
                    }
                }

                if (isTurningCorner)
                {
                    transform.position = Vector2.MoveTowards(transform.position, tempTarget.position, Time.deltaTime * speed);
                    if (Vector2.Distance(transform.position, tempTarget.position) < 0.1f)
                        isTurningCorner = false;
                }

                if (isClimbingUp && !isTurningCorner)
                {
                    RaycastHit2D climbUpHeight = Physics2D.Raycast(new Vector2(tempTarget.position.x + climbUpOffsets.x, tempTarget.position.y + (climbUpOffsets.y * 2)), Vector2.left, groundCheckLenght, LayerMask.GetMask("Ground"));
                    if (climbUpHeight.collider != null)
                    {
                        tempTarget.position = new Vector2(tempTarget.position.x, climbUpOffsets.y);
                        transform.position = Vector2.MoveTowards(transform.position, new Vector2(tempTarget.position.x, path.vectorPath[currentWaypoint].y), Time.deltaTime * speed);
                    }
                }
                else if (isClimbingDown && !isTurningCorner)
                {
                    RaycastHit2D climbDownHeight = Physics2D.Raycast(new Vector2(tempTarget.position.x + climbUpOffsets.x, tempTarget.position.y - (climbUpOffsets.y * 2)), Vector2.left, groundCheckLenght, LayerMask.GetMask("Ground"));
                    if (climbDownHeight.collider != null)
                    {
                        tempTarget.position = new Vector2(tempTarget.position.x, climbDownOffsets.y);
                        transform.position = Vector2.MoveTowards(transform.position, new Vector2(tempTarget.position.x, path.vectorPath[currentWaypoint].y), Time.deltaTime * speed);
                    }
                }
            }
        }
        */
        /*
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position);
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
        */

        RaycastHit2D forwardGroundCheck1 = Physics2D.Raycast(new Vector2(transform.position.x + forwardGroundOffset1, transform.position.y), Vector2.down, groundCheckLenght, LayerMask.GetMask("Ground"));
        RaycastHit2D forwardGroundCheck2 = Physics2D.Raycast(new Vector2(transform.position.x + forwardGroundOffset2, transform.position.y), Vector2.down, groundCheckLenght, LayerMask.GetMask("Ground"));
        if (forwardGroundCheck1.collider != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(path.vectorPath[currentWaypoint].x, forwardGroundCheck1.point.y + monkeyHeight), Time.deltaTime * speed);
        }
        else if (forwardGroundCheck2.collider != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(path.vectorPath[currentWaypoint].x, forwardGroundCheck1.point.y + monkeyHeight), Time.deltaTime * speed);
        }

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
        Gizmos.DrawRay(new Vector2(transform.position.x + forwardGroundOffset1, transform.position.y), Vector2.down * groundCheckLenght);
        Gizmos.DrawRay(new Vector2(transform.position.x + maxHoleDistanceOffset, transform.position.y), Vector2.down * groundCheckLenght);
        Gizmos.DrawRay(new Vector2(transform.position.x + climbUpOffsets.x, transform.position.y + climbUpOffsets.y), Vector2.left * groundCheckLenght);
        Gizmos.DrawRay(new Vector2(transform.position.x + climbDownOffsets.x, transform.position.y + climbDownOffsets.y), Vector2.left * groundCheckLenght);
    }
}
