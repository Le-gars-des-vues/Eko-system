using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TardidogMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Transform target;
    [SerializeField] Transform jumpTarget;

    [Header("Hover Variables")]
    [SerializeField] private float creatureHeight;
    [SerializeField] float springStrenght;
    [SerializeField] float springDamper;
    [SerializeField] float uprightSpringStrenght;
    [SerializeField] float uprightSpringDamper;
    [SerializeField] float hoverRaycastLenght;

    [Header("Ground Variables")]
    [SerializeField] Vector2 groundCheckOffsets;
    [SerializeField] float inertiaFactor;
    [SerializeField] float groundRaycastLenght;

    [Header("Rotate Variables")]
    [SerializeField] float rotateSpeed;
    [SerializeField] float upAngle;
    [SerializeField] float downAngle;

    [Header("Movement Variables")]
    [SerializeField] float moveSpeed;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float minMoveSpeed;
    [SerializeField] float slowDownThreshold;
    [SerializeField] List<TardidogLegAnimation> legs = new List<TardidogLegAnimation>();
    float dragForce;

    [Header("Checks")]
    public bool isStopped;
    bool isJumping;
    bool isGrounded;
    public bool isFacingRight = true;
    public int facingDirection = 1;
    bool targetIsRight;
    float dist = 0;
    float distanceFromNextPoint = 0;

    [Header("Sight Variables")]
    public Transform head;
    public float startAngle;
    public float angleStep;
    public float sightAngle;
    public float rayCount;
    public float rayDistance;

    [Header("Jump Variables")]
    [SerializeField] float jumpYOffset;
    [SerializeField] Vector2 jumpForce;
    [SerializeField] float jumpRayForwardLength;
    [SerializeField] float jumpRayUpLength;
    [SerializeField] float jumpRayDownLength;

    [SerializeField] CreatureState state;
    [SerializeField] CreaturePathfinding pathfinding;
    //[SerializeField] TardidogAttack atk;

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
        dragForce = rb.drag;
        maxMoveSpeed *= rb.mass;
        minMoveSpeed *= rb.mass;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        facingDirection = isFacingRight ? 1 : -1;
        if (state.isPathfinding && pathfinding.path != null)
        {
            targetIsRight = (pathfinding.path.lookPoints[pathfinding.pathIndex].x - transform.position.x) > 0 ? true : false;
            dist = Mathf.Abs(pathfinding.path.lookPoints[pathfinding.path.lookPoints.Length - 1].x - transform.position.x);
            distanceFromNextPoint = Vector2.Distance(pathfinding.path.lookPoints[pathfinding.pathIndex], transform.position);
        }
        else if (!state.isPathfinding)
        {
            targetIsRight = (target.position.x - transform.position.x) > 0 ? true : false;
            dist = Mathf.Abs(target.position.x - rb.position.x);
        }

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
                pathfinding.reachEndOfPath = true;
                pathfinding.StopPathFinding();
            }
            lastPosition = transform.position;
        }
    }

    private void FixedUpdate()
    {
        //Ground check
        RaycastHit2D grounded = Physics2D.Raycast(origin: new Vector2(transform.position.x, transform.position.y), direction: Vector2.down, groundRaycastLenght, LayerMask.GetMask("Ground"));
        isGrounded = grounded;

        //JumpCheck
        RaycastHit2D jumpCheck = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.right * facingDirection, jumpRayForwardLength, LayerMask.GetMask("Ground"));


        //Hovering
        RaycastHit2D frontCheck = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down, hoverRaycastLenght, LayerMask.GetMask("Ground"));
        RaycastHit2D backCheck = Physics2D.Raycast(new Vector2(transform.position.x - groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down, hoverRaycastLenght, LayerMask.GetMask("Ground"));
        if (frontCheck.collider != null && isGrounded)
        {
            float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity);

            float difference = frontCheck.distance - creatureHeight;
            float springForce = (difference * (springStrenght * rb.mass)) - (rayDirVel * springDamper);

            rb.AddForce(Vector2.down * springForce);
        }
        else if (backCheck.collider != null && isGrounded)
        {
            float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity);

            float difference = backCheck.distance - creatureHeight;
            float springForce = (difference * (springStrenght * rb.mass)) - (rayDirVel * springDamper);

            rb.AddForce(Vector2.down * springForce);

            if (frontCheck.collider == null)
            {
                RaycastHit2D jumpCheckUp = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, jumpRayUpLength, LayerMask.GetMask("Ground"));
                RaycastHit2D jumpCheckDown = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down, jumpRayDownLength, LayerMask.GetMask("Ground"));
                if (!(jumpCheck || jumpCheckUp || jumpCheckDown))
                {
                    if (state.isPathfinding && pathfinding.path != null)
                    {
                        if (!isJumping && isGrounded && (Mathf.Abs(pathfinding.path.lookPoints[pathfinding.pathIndex].x - transform.position.x) > 3f || pathfinding.path.lookPoints[pathfinding.pathIndex].y - transform.position.y > 0))
                        {
                            jumpTarget.position = Vision(target.position, true);
                            if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                            {
                                StartCoroutine(Jump(1));
                            }
                        }
                    }
                    else if (Vector2.Distance(target.position, rb.position) > 3f && !isJumping && isGrounded)
                    {
                        jumpTarget.position = Vision(target.position, true);
                        if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                        {
                            StartCoroutine(Jump(1));
                        }
                    }
                }
            }
        }

        //Movement
        if (CanMove())
        {
            float speedFactor = 1;
            if (isGrounded)
            {
                if (!state.isAttacking)
                {
                    if (dist >= slowDownThreshold)
                        moveSpeed = maxMoveSpeed;
                    else if (dist < 0.1f)
                        rb.velocity = Vector2.zero;
                    else if (dist < slowDownThreshold)
                        moveSpeed = Mathf.Lerp(maxMoveSpeed, minMoveSpeed, (slowDownThreshold - dist / slowDownThreshold));

                    speedFactor = 1;
                }
                else
                    moveSpeed = maxMoveSpeed;
                speedFactor = 1.5f;
            }
            else if (!isGrounded)
                speedFactor = 0.5f;

            if (dist > 0.5f)
            {
                isStopped = false;
                if (targetIsRight)
                    GoRight(speedFactor);
                else
                    GoLeft(speedFactor);
            }
            else
                isStopped = true;
        }

        //Angle Direction
        if (isGrounded)
        {
            transform.up = Vector3.Lerp(transform.up, grounded.normal, rotateSpeed * 2 * Time.deltaTime);
            if (!isJumping)
                rb.drag = dragForce;
        }
        else
        {
            if (rb.velocity.y > 0 && !(frontCheck || backCheck))
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.z, upAngle * facingDirection, (rotateSpeed) * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, angle);
            }
            else if (rb.velocity.y < 0)
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.z, -downAngle * facingDirection, (rotateSpeed) * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, angle);
            }
        }

        if (jumpCheck)
        {
            RaycastHit2D jumpCheckUp = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, jumpRayUpLength, LayerMask.GetMask("Ground"));
            RaycastHit2D jumpCheckDown = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down, jumpRayDownLength, LayerMask.GetMask("Ground"));
            if (!jumpCheckUp)
            {
                //Debug.Log(Vector2.Distance(target.position, rb.position));
                if (state.isPathfinding && pathfinding.path != null)
                {
                    if (Mathf.Approximately(Mathf.Abs(jumpCheck.normal.x), 1f) && Vector2.Distance(pathfinding.path.lookPoints[pathfinding.pathIndex], transform.position) > 3f)
                    {
                        if (!isJumping && isGrounded)
                        {
                            jumpTarget.position = Vision(target.position, false);
                            if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                            {
                                StartCoroutine(Jump(1));
                            }
                        }
                    }
                }
                else
                {
                    if (dist > 2f && !isJumping && isGrounded)
                    {
                        jumpTarget.position = Vision(target.position, false);
                        if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                        {
                            StartCoroutine(Jump(1));
                        }
                    }
                }
            }
        }
    }

    void GoRight(float speedFactor)
    {
        Vector2 force = new Vector2(moveSpeed * speedFactor, 0);

        if (!state.isAttacking)
        {
            if (rb.velocity.x < 0)
                force.x -= rb.velocity.x;
            else if (rb.velocity.x < minMoveSpeed)
                force.x += minMoveSpeed;
        }

        rb.AddForce(force);

        if (!isFacingRight && rb.velocity.x > 0)
        {
            Turn();
            foreach (TardidogLegAnimation leg in legs)
            {
                leg.Turn();
            }
        }
    }

    void GoLeft(float speedFactor)
    {
        Vector2 force = new Vector2(-moveSpeed * speedFactor, 0);

        if (!state.isAttacking)
        {
            if (rb.velocity.x > 0)
                force.x -= rb.velocity.x;
            else if (rb.velocity.x > -minMoveSpeed)
                force.x -= minMoveSpeed;
        }

        rb.AddForce(force);

        if (isFacingRight && rb.velocity.x < 0)
        {
            Turn();
            foreach (TardidogLegAnimation leg in legs)
            {
                leg.Turn();
            }
        }
    }

    IEnumerator Jump(int direction)
    {
        Debug.Log("isJumping");
        rb.drag = 0;
        isJumping = true;
        Vector2 jumpDirection = new Vector2(jumpTarget.position.x, jumpTarget.position.y) - rb.position;
        jumpDirection.x -= (jumpDirection.x / 2) * facingDirection * direction;
        jumpDirection.y += jumpYOffset;
        Vector2 jumpPoint = rb.position + jumpDirection;

        jumpTarget.position = jumpPoint;
        //Debug.Log(Vector2.Distance(new Vector2(jumpTarget.position.x, jumpTarget.position.y), rb.position));

        float timer = 0;
        float duration = 0.2f;

        float speedX = (jumpPoint.x - rb.position.x) * rb.mass;
        float speedY = Mathf.Sqrt(2 * Physics.gravity.magnitude * Mathf.Max(jumpPoint.y - rb.position.y, 3f)) * rb.mass;
        /*
        float massFactor = Mathf.Sqrt(rb.mass);
        float dragFactor = Mathf.Sqrt(rb.drag);

        jumpForce.y /= massFactor * dragFactor;
        jumpForce.x /= massFactor * dragFactor;
        */

        jumpForce = new Vector2(speedX, speedY);

        //Debug.Log(jumpForce);
        rb.velocity = Vector2.zero;
        //jumpForce.x -= rb.velocity.x;
        //jumpForce.y -= rb.velocity.y;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float targetAngle = Mathf.Lerp(0, 80, (jumpTarget.position.y - rb.position.y) / 5);
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle * facingDirection, timer / duration);
            transform.eulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }
        rb.AddForce(jumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    void Turn()
    {
        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    Vector2 Vision(Vector2 target, bool sight)
    {
        angleStep = sightAngle / (rayCount - 1);

        // Start angle from left limit of sight angle
        startAngle = -sightAngle / 2;

        float distanceFromTarget = 1000f;

        Vector2 targetPosition = Vector2.zero;

        if (sight)
        {
            for (int i = 0; i < rayCount; i++)
            {
                // Calculate current angle
                float angle = startAngle + angleStep * i;

                // Calculate direction of the ray
                Vector3 direction = Quaternion.Euler(0, 0, angle) * head.transform.right * facingDirection;

                // Cast a ray in the calculated direction
                RaycastHit2D hit = Physics2D.Raycast(head.transform.position, direction, rayDistance, LayerMask.GetMask("Ground"));

                // Check if the ray hits a platform collider
                if (hit.collider != null)
                {
                    if (Vector2.Distance(target, hit.point) < distanceFromTarget)
                    {
                        Debug.DrawLine(head.transform.position, hit.point, Color.green, 5); // Visualize the ray
                        distanceFromTarget = Vector2.Distance(target, hit.point);
                        targetPosition = hit.point;
                    }

                }
            }
        }
        else
        {
            for (int i = 0; i < rayCount; i++)
            {
                // Cast a ray in the calculated direction
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(head.transform.position.x, head.transform.position.y + ((float)i / 3)), Vector2.right * facingDirection, rayDistance, LayerMask.GetMask("Ground"));

                // Check if the ray hits a platform collider
                if (hit.collider != null)
                {
                    if (i != rayCount - 1)
                    {
                        Debug.DrawRay(new Vector2(head.transform.position.x, head.transform.position.y + ((float)i / 3)), Vector2.right * facingDirection, Color.green, 5); // Visualize the ray
                        targetPosition = hit.point;
                    }
                }
            }
        }
        //Debug.Log(targetPosition);
        return targetPosition;
    }

    bool CanMove()
    {
        return !state.isEating && !state.isStunned;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector2 inertia = -rb.velocity * inertiaFactor;
            rb.AddForce(inertia);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //HoverCheck
        Gizmos.DrawRay(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down * hoverRaycastLenght);
        Gizmos.DrawRay(new Vector2(transform.position.x - groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down * hoverRaycastLenght);
        //Ground Check
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.down * groundRaycastLenght);

        Gizmos.DrawRay(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down * jumpRayDownLength);
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.up * jumpRayUpLength);
        Gizmos.DrawRay(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.right * facingDirection * jumpRayForwardLength);

        /*
        for (int i = 0; i < rayCount; i++)
        {
            Gizmos.DrawRay(new Vector2(head.transform.position.x, head.transform.position.y + ((float)i / 3)), Vector2.right * facingDirection); // Visualize the ray
        }
        */
    }
}

