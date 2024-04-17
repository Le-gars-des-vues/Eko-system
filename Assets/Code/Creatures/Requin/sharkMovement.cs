using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sharkMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Transform target;

    [Header("Rotate Variables")]
    [SerializeField] float rotateSpeed = 5;

    [Header("Movement Variables")]
    [SerializeField] float moveSpeed;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float minMoveSpeed;
    [SerializeField] float slowDownThreshold;

    [Header("Checks")]
    public bool isStopped;
    public bool isFacingRight = true;
    public bool isFacingUp = true;
    public int facingDirection = 1;
    bool targetIsRight;
    bool targetIsUp;
    float dist = 0;

    [Header("Sight Variables")]
    public Transform head;
    public float startAngle;
    public float angleStep;
    public float sightAngle = 90;
    public float rayCount = 10;
    public float rayDistance = 5;

    [Header("BodyParts")]
    [SerializeField] Transform[] fins;
    [SerializeField] Transform body;

    [SerializeField] CreatureState state;
    [SerializeField] CreaturePathfinding pathfinding;

    [SerializeField] float movementThreshold = 1f; // Minimum movement distance to consider the creature stuck
    [SerializeField] float checkInterval = 15f; // Time interval to check for stuck condition
    private Vector3 lastPosition; // Last recorded position of the creature
    private float timeSinceLastCheck = 0f;

    // Start is called before the first frame update
    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        isFacingRight = transform.localScale.x == 1 ? true : false;
        state = GetComponent<CreatureState>();
        pathfinding = GetComponent<CreaturePathfinding>();
        maxMoveSpeed *= rb.mass;
        minMoveSpeed *= rb.mass;
    }

    // Update is called once per frame
    void Update()
    {
        facingDirection = isFacingRight ? 1 : -1;
        if (state.isPathfinding && pathfinding.path != null)
        {
            targetIsRight = (pathfinding.path.lookPoints[pathfinding.pathIndex].x - transform.position.x) > 0 ? true : false;
            targetIsUp = (pathfinding.path.lookPoints[pathfinding.pathIndex].y - transform.position.y) > 0 ? true : false;
            dist = Vector2.Distance(pathfinding.path.lookPoints[pathfinding.path.lookPoints.Length - 1], transform.position);
        }
        else if (!state.isPathfinding)
        {
            targetIsRight = (target.position.x - transform.position.x) > 0 ? true : false;
            targetIsUp = (target.position.y - transform.position.y) > 0 ? true : false;
            dist = Vector2.Distance(target.position, transform.position);
        }

        if (dist >= slowDownThreshold)
            moveSpeed = maxMoveSpeed;
        else if (dist < 0.1f)
            rb.velocity = Vector2.zero;
        else if (dist < slowDownThreshold)
            moveSpeed = Mathf.Lerp(maxMoveSpeed, minMoveSpeed, (slowDownThreshold - dist / slowDownThreshold));
        
        if (dist > slowDownThreshold)
        {
            if (state.isPathfinding && pathfinding.path != null)
            {
                Vector2 direction = (pathfinding.path.lookPoints[pathfinding.pathIndex] - (Vector2)transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
            }
            else
            {
                Vector2 direction = (target.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
            }
        }
        else
        {
            float angle = isFacingUp ? 0 : 180;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
        }
        /*
        if (Mathf.Abs(transform.rotation.eulerAngles.z) >= 95 && isFacingUp)
            Turn();
        else if (Mathf.Abs(transform.rotation.eulerAngles.z) < 85 && !isFacingUp)
            Turn();
        */
        if (targetIsRight != isFacingRight)
        {
            Turn();
        }


        if (state.isPathfinding)
        {
            // Calculate movement since the last frame
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            // Update time since last check
            timeSinceLastCheck += Time.deltaTime;
            if (timeSinceLastCheck > checkInterval)
            {
                timeSinceLastCheck = 0;
                if (distanceMoved < movementThreshold)
                {
                    // The creature is stuck, stop its movement
                    target.position = transform.position;
                    pathfinding.reachEndOfPath = true;
                    pathfinding.StopPathFinding();
                }
                lastPosition = transform.position;
            }
        }
    }

    private void FixedUpdate()
    {
        if (CanMove())
        {
            if (dist > 0.5f)
            {
                isStopped = false;
                /*
                if (state.isPathfinding && pathfinding.path != null)
                {
                    Vector2 direction = pathfinding.path.lookPoints[pathfinding.pathIndex] - rb.position;
                    rb.velocity = (direction.normalized * moveSpeed);
                }
                else
                {
                    Vector2 direction = (Vector2)target.position - rb.position;
                    rb.velocity = (direction.normalized * moveSpeed);
                }
                */
                
                if (targetIsRight)
                    GoRight(1);
                else
                    GoLeft(1);

                if (targetIsUp)
                    GoUp(1);
                else
                    GoDown(1);
                
            }
            else
                isStopped = true;
        }
    }

    /*
    void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.y *= -1;
        transform.localScale = scale;

        isFacingUp = !isFacingUp;

        isFacingRight = !isFacingRight;
    }
    */

    void Turn()
    {
        if (isFacingRight)
        {
            body.eulerAngles = new Vector3(0, 0, 180);
            Vector2 texScale = body.gameObject.GetComponent<LineRenderer>().textureScale;
            texScale.y *= -1;
            body.gameObject.GetComponent<LineRenderer>().textureScale = texScale;
        }
        else
        {
            body.eulerAngles = new Vector3(0, 0, 0);
            Vector2 texScale = body.gameObject.GetComponent<LineRenderer>().textureScale;
            texScale.y *= -1;
            body.gameObject.GetComponent<LineRenderer>().textureScale = texScale;
        }

        Vector3 scale = head.transform.localScale;
        scale.y *= -1;
        head.transform.localScale = scale;


        foreach (Transform leg in fins)
        {
            scale = leg.transform.localScale;
            scale.y *= -1;
            leg.transform.localScale = scale;
        }


        isFacingRight = !isFacingRight;
    }

    void GoRight(float speedFactor)
    {
        Vector2 force = new Vector2(moveSpeed * speedFactor, 0);

        if (rb.velocity.x < 0)
            force.x -= rb.velocity.x;
        else if (rb.velocity.x < minMoveSpeed)
            force.x += minMoveSpeed;

        rb.AddForce(force);
    }

    void GoLeft(float speedFactor)
    {
        Vector2 force = new Vector2(-moveSpeed * speedFactor, 0);

        if (rb.velocity.x > 0)
            force.x -= rb.velocity.x;
        else if (rb.velocity.x > -minMoveSpeed)
            force.x -= minMoveSpeed;

        rb.AddForce(force);
    }

    void GoUp(float speedFactor)
    {
        Vector2 force = new Vector2(0, moveSpeed * speedFactor);

        if (rb.velocity.y < 0)
            force.y -= rb.velocity.y;
        else if (rb.velocity.y < minMoveSpeed)
            force.y += minMoveSpeed;

        rb.AddForce(force);
    }

    void GoDown(float speedFactor)
    {
        Vector2 force = new Vector2(0, -moveSpeed * speedFactor);

        if (rb.velocity.y > 0)
            force.y -= rb.velocity.y;
        else if (rb.velocity.y > -minMoveSpeed)
            force.y -= minMoveSpeed;

        rb.AddForce(force);
    }

    bool CanMove()
    {
        return !state.isEating && !state.isStunned;
    }
}
