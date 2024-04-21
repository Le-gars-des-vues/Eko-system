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
    [SerializeField] float chargeFactor = 2;
    [SerializeField] List<TardidogLegAnimation> legs = new List<TardidogLegAnimation>();
    float dragForce;

    [Header("Checks")]
    public bool isStopped;
    bool isJumping;
    bool isGrounded;
    public bool isFacingRight = true;
    public int facingDirection = 1;
    public bool targetIsRight;
    public bool targetIsInFront;
    float dist = 0;
    public bool nextPointIsRight;

    [Header("Sight Variables")]
    public Transform head;
    public float startAngle;
    public float angleStep;
    public float sightAngle;
    public float rayCount;
    public float rayDistance;

    [Header("Sensor Variables")]
    [SerializeField] int numberOfRays;
    RaycastHit2D[] ledgeSensor;
    public bool ledgeFound = false;
    [SerializeField] Vector2 ledgePos;
    [SerializeField] float ledgeOffset;
    Vector2 ledgeTempPos;
    [SerializeField] [Range(0, 1)] float ledgeRaysStep;
    [SerializeField] float sensorRayLength;
    [SerializeField] float sensorStartOffset;
    bool sensorUp = false;

    [Header("Jump Variables")]
    [SerializeField] float jumpYOffset;
    [SerializeField] float minJumpHeight;
    [SerializeField] Vector2 jumpForce;
    [SerializeField] float jumpRayForwardLength;
    [SerializeField] float jumpRayUpLength;
    [SerializeField] float jumpRayDownLength;
    float speedFactor = 1;

    [SerializeField] CreatureState state;
    [SerializeField] CreaturePathfinding pathfinding;
    //[SerializeField] TardidogAttack atk;

    [Header("Safeguard Variables")]
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
        ledgeSensor = new RaycastHit2D[numberOfRays];
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
            dist = Vector2.Distance(pathfinding.path.lookPoints[pathfinding.path.lookPoints.Length - 1], transform.position);
            if (pathfinding.pathIndex < pathfinding.path.lookPoints.Length - 4)
                nextPointIsRight = (pathfinding.path.lookPoints[pathfinding.pathIndex + 3].x - transform.position.x) > 0 ? true : false;
            else
                nextPointIsRight = (target.position.x - transform.position.x) > 0 ? true : false;
        }
        else if (!state.isPathfinding)
        {
            targetIsRight = (target.position.x - transform.position.x) > 0 ? true : false;
            dist = Mathf.Abs(target.position.x - rb.position.x);
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
                    pathfinding.ReachedEndOfPath();
                }
                lastPosition = transform.position;
            }
        }

        if (isFacingRight == targetIsRight)
            targetIsInFront = true;
        else
            targetIsInFront = false;

        if (state.isPathfinding && pathfinding.path != null)
        {
            if (Mathf.Abs(pathfinding.path.lookPoints[pathfinding.pathIndex].x - rb.position.x) < 0.1f)
            {
                //isSearching = true;
                //Debug.Log("Looking for ledge");
                if (pathfinding.path.lookPoints[pathfinding.pathIndex].y - rb.position.y > 0 && !ledgeFound && isGrounded)
                {
                    for (int i = 0; i < ledgeSensor.Length; i++)
                    {
                        //Debug.Log("Ray created");
                        ledgeSensor[i] = Physics2D.Raycast(new Vector2((transform.position.x - ((ledgeSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + sensorStartOffset), Vector2.up, sensorRayLength, LayerMask.GetMask("Ground"));
                    }
                    for (int i = 0; i < ledgeSensor.Length; i++)
                    {
                        if ((i != 0 && i != ledgeSensor.Length - 1) && ledgeSensor[i])
                        {
                            if (nextPointIsRight)
                            {
                                Debug.DrawLine(new Vector2((transform.position.x - ((ledgeSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + sensorStartOffset), ledgeSensor[i].point, Color.yellow, 1);
                                if (!ledgeSensor[i - 1])
                                {
                                    PrepareLedgeJump(true, ledgeSensor[i].point, true);
                                    break;
                                }
                            }
                            else
                            {
                                Debug.DrawLine(new Vector2((transform.position.x - ((ledgeSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + sensorStartOffset), ledgeSensor[i].point, Color.yellow, 1);
                                if (!ledgeSensor[i + 1])
                                {
                                    PrepareLedgeJump(true, ledgeSensor[i].point, false);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (pathfinding.path.lookPoints[pathfinding.pathIndex].y - rb.position.y < 0 && !ledgeFound && isGrounded)
                {
                    for (int i = 0; i < ledgeSensor.Length; i++)
                    {
                        ledgeSensor[i] = Physics2D.Raycast(new Vector2((transform.position.x - ((ledgeSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + sensorStartOffset), Vector2.down, sensorRayLength, LayerMask.GetMask("Ground"));
                    }
                    
                    for (int i = 0; i < ledgeSensor.Length; i++)
                    {
                        if ((i != 0 && i != ledgeSensor.Length - 1) && ledgeSensor[i])
                        {
                            if (nextPointIsRight)
                            {
                                Debug.DrawLine(new Vector2((transform.position.x - ((ledgeSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + sensorStartOffset), ledgeSensor[i].point, Color.yellow, 1);
                                if (!ledgeSensor[i - 1])
                                {
                                    PrepareLedgeJump(false, ledgeSensor[i].point, true);
                                    break;
                                }
                            }
                            else
                            {
                                Debug.DrawLine(new Vector2((transform.position.x - ((ledgeSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + sensorStartOffset), ledgeSensor[i].point, Color.yellow, 1);
                                if (!ledgeSensor[i + 1])
                                {
                                    PrepareLedgeJump(false, ledgeSensor[i].point, false);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (ledgeFound)
        {
            target.position = new Vector2(ledgePos.x + ledgeOffset, transform.position.y);
            if (Mathf.Abs(target.position.x - transform.position.x) < 0.5f && isGrounded)
            {
                if (sensorUp)
                {
                    for (int i = 0; i < rayCount; i++)
                    {
                        float ledgeDirection = nextPointIsRight ? -1 : 1;
                        Debug.Log(nextPointIsRight);
                        // Cast a ray in the calculated direction
                        RaycastHit2D hit = Physics2D.Raycast(new Vector2(ledgePos.x + 1, ledgePos.y + ((float)i / 3)), Vector2.left * ledgeDirection, rayDistance, LayerMask.GetMask("Ground"));

                        // Check if the ray hits a platform collider
                        if (hit.collider != null)
                        {
                            if (i != rayCount - 1)
                            {
                                Debug.DrawRay(new Vector2(ledgePos.x + 1, ledgePos.y + ((float)i / 3)), Vector2.left * ledgeDirection * hit.distance, Color.yellow, 5); // Visualize the ray
                                jumpTarget.position = hit.point;
                            }
                        }
                    }
                    //Turn();
                    target.position = ledgeTempPos;
                    pathfinding.NewTarget(target.gameObject);
                    StartCoroutine(Jump(2));
                    ledgeFound = false;
                }
                else
                {
                    target.position = ledgeTempPos;
                    pathfinding.NewTarget(target.gameObject);
                    ledgeFound = false;
                }
            }
        }
    }

    void PrepareLedgeJump(bool targetIsUp, Vector2 _ledgePos, bool ToTheleft)
    {
        ledgeTempPos = target.position;
        ledgePos = _ledgePos;
        ledgeFound = true;
        sensorUp = targetIsUp;
        pathfinding.StopPathFinding();
        float _ledgeOffset = ledgeOffset;
        ledgeOffset = ToTheleft ? _ledgeOffset * -1 : Mathf.Abs(ledgeOffset);
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
                if (!(jumpCheck || jumpCheckUp || jumpCheckDown) && targetIsInFront)
                {
                    if (state.isPathfinding && pathfinding.path != null)
                    {
                        if (!isJumping && isGrounded)
                        {
                            jumpTarget.position = Vision(target.position, true);
                            if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero && jumpTarget.position.y < pathfinding.path.lookPoints[pathfinding.pathIndex].y)
                            {
                                StartCoroutine(Jump(2));
                            }
                        }
                    }
                    else if (Vector2.Distance(target.position, rb.position) > 3f && !isJumping && isGrounded)
                    {
                        jumpTarget.position = Vision(target.position, true);
                        if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                        {
                            StartCoroutine(Jump(2));
                        }
                    }
                }
            }
        }

        //Movement
        if (CanMove())
        {
            if (isGrounded)
            {
                if (!state.isAttacking)
                {
                    speedFactor = 1;
                    if (dist >= slowDownThreshold)
                        moveSpeed = maxMoveSpeed;
                    else if (dist < 0.1f)
                        rb.velocity = Vector2.zero;
                    else if (dist < slowDownThreshold)
                        moveSpeed = Mathf.Lerp(maxMoveSpeed, minMoveSpeed, (slowDownThreshold - dist / slowDownThreshold));
                }
                else
                {
                    moveSpeed = maxMoveSpeed;
                    speedFactor = chargeFactor;
                }
            }
            else
                speedFactor = 0f;

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
            if (!jumpCheckUp && targetIsInFront)
            {
                //Debug.Log(Vector2.Distance(target.position, rb.position));
                if (state.isPathfinding && pathfinding.path != null)
                {
                    if ((Mathf.Abs(jumpCheck.normal.x) == 1f) && Vector2.Distance(pathfinding.path.lookPoints[pathfinding.pathIndex], transform.position) > 3f)
                    {
                        if (!isJumping && isGrounded)
                        {
                            jumpTarget.position = Vision(target.position, false);
                            if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                            {
                                StartCoroutine(Jump(0));
                            }
                        }
                    }
                }
                else
                {
                    if ((Mathf.Abs(jumpCheck.normal.x) == 1f))
                    {
                        if (dist > 2f && !isJumping && isGrounded)
                        {
                            jumpTarget.position = Vision(target.position, false);
                            if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                            {
                                StartCoroutine(Jump(0));
                            }
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
            Turn();
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
            Turn();
    }

    IEnumerator Jump(float offset)
    {
        //Debug.Log("isJumping");
        rb.drag = 0;
        isJumping = true;
        Vector2 jumpDirection = new Vector2(jumpTarget.position.x, jumpTarget.position.y) - rb.position;
        jumpDirection.x -= ((jumpDirection.x - offset * facingDirection) / 2);
        jumpDirection.y += jumpYOffset;
        Vector2 jumpPoint = rb.position + jumpDirection;

        jumpTarget.position = jumpPoint;
        //Debug.Log(Vector2.Distance(new Vector2(jumpTarget.position.x, jumpTarget.position.y), rb.position));

        float timer = 0;
        float duration = 0.2f;

        float speedX = (jumpPoint.x - rb.position.x) * rb.mass;
        float speedY = Mathf.Sqrt(2 * Physics.gravity.magnitude * Mathf.Max(jumpPoint.y - rb.position.y, minJumpHeight)) * rb.mass;

        jumpForce = new Vector2(speedX, speedY);

        rb.velocity = Vector2.zero;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float targetAngle = Mathf.Lerp(0, 80, (jumpTarget.position.y - rb.position.y) / 5);
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle * facingDirection, timer / duration);
            transform.eulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }
        GetComponent<CreatureSound>().jumpSound.Post(gameObject);
        rb.AddForce(jumpForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    void Turn()
    {
        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;

        foreach (TardidogLegAnimation leg in legs)
        {
            leg.Turn();
        }

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
        
        /*
        for (int i = 0; i < 11; i++)
        {
            Gizmos.DrawRay(new Vector2((transform.position.x - ((11 / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + sensorStartOffset), Vector2.up * sensorRayLength);
        }
        */
    }
}

