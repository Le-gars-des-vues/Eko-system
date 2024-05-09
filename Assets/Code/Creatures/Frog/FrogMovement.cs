using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Transform target;
    [SerializeField] Transform jumpTarget;

    public HingeJoint2D[] tailJoints;

    [Header("Ground Variables")]
    [SerializeField] Vector2 groundCheckOffsets;
    [SerializeField] float inertiaFactor;
    [SerializeField] float groundRaycastLenght;
    [SerializeField] float frontRaycastLenght;

    [Header("Rotate Variables")]
    [SerializeField] float rotateSpeed;
    [SerializeField] float upAngle;
    [SerializeField] float downAngle;

    [Header("Movement Variables")]
    [SerializeField] float moveSpeed;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float minMoveSpeed;
    [SerializeField] float slowDownThreshold;
    [SerializeField] List<FrogLegAnimation> legs = new List<FrogLegAnimation>();
    float dragForce;

    [Header("Checks")]
    public bool isStopped;
    public bool isJumping;
    public bool isGrounded;
    public bool isFacingRight = true;
    public int facingDirection = 1;
    public bool targetIsRight;
    public bool targetIsUp;
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
    [SerializeField] Vector2 jumpForce;
    [SerializeField] float jumpRayForwardLength;
    [SerializeField] float jumpRayUpLength;
    [SerializeField] float jumpRayDownLength;
    [SerializeField] float minJumpDistance;
    [SerializeField] float minJumpHeight;

    [SerializeField] CreatureState state;
    [SerializeField] CreaturePathfinding pathfinding;

    [Header("Safeguard Variables")]
    [SerializeField] float movementThreshold = 1f; // Minimum movement distance to consider the creature stuck
    [SerializeField] float checkInterval = 15f; // Time interval to check for stuck condition
    private Vector3 lastPosition; // Last recorded position of the creature
    private float timeSinceLastCheck = 0f;

    [Header("Poison Variables")]
    [SerializeField] private float damage;
    [SerializeField] StatusEffect poison;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isFacingRight = transform.localScale.x == 1 ? true : false;
        state = GetComponent<CreatureState>();
        pathfinding = GetComponent<CreaturePathfinding>();

        ledgeSensor = new RaycastHit2D[numberOfRays];

        dragForce = rb.drag;
        lastPosition = transform.position;
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
            if (pathfinding.pathIndex < pathfinding.path.lookPoints.Length - 4)
                nextPointIsRight = (pathfinding.path.lookPoints[pathfinding.pathIndex + 3].x - transform.position.x) > 0 ? true : false;
            else
                nextPointIsRight = (target.position.x - transform.position.x) > 0 ? true : false;
        }
        else if (!state.isPathfinding)
        {
            targetIsRight = (target.position.x - transform.position.x) > 0 ? true : false;
            targetIsUp = (target.position.y - transform.position.y) > 0 ? true : false;
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

        if (GetComponent<CreatureUnderwater>().isUnderwater)
        {
            if (targetIsRight != isFacingRight)
            {
                Turn();
            }

            if (dist >= slowDownThreshold)
                moveSpeed = maxMoveSpeed;
            else if (dist < 0.1f)
                rb.velocity = Vector2.zero;
            else if (dist < slowDownThreshold)
                moveSpeed = Mathf.Lerp(maxMoveSpeed, minMoveSpeed, (slowDownThreshold - dist / slowDownThreshold));

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
                        StartCoroutine(Jump(2, true));
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
        if (!GetComponent<CreatureUnderwater>().isUnderwater)
        {
            //Ground check
            RaycastHit2D grounded = Physics2D.Raycast(origin: new Vector2(transform.position.x, transform.position.y), direction: Vector2.down, groundRaycastLenght, LayerMask.GetMask("Ground"));
            isGrounded = grounded;

            //JumpCheck
            RaycastHit2D jumpCheck = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.right * facingDirection, jumpRayForwardLength * 1.5f, LayerMask.GetMask("Ground"));
            RaycastHit2D frontCheck = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down, frontRaycastLenght, LayerMask.GetMask("Ground"));
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
                                StartCoroutine(Jump(2, true));
                            }
                        }
                    }
                    else if (Vector2.Distance(target.position, rb.position) > 3f && !isJumping && isGrounded)
                    {
                        jumpTarget.position = Vision(target.position, true);
                        if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                        {
                            StartCoroutine(Jump(2, true));
                        }
                    }
                }
            }

            //Movement
            if (CanMove())
            {
                if (!GetComponent<CreatureUnderwater>().isUnderwater)
                {
                    if (dist > 2f && isGrounded)
                    {
                        isStopped = false;
                        if (targetIsRight)
                            JumpRight();
                        else
                            JumpLeft();
                    }
                    else if (dist > 1f)
                    {
                        isStopped = false;
                        if (targetIsRight)
                            GoRight();
                        else
                            GoLeft();
                    }
                    else
                        isStopped = true;
                }
                else
                {
                }
            }

            //Angle Direction
            if (isGrounded)
            {
                //transform.up = Vector3.Lerp(transform.up, grounded.normal, rotateSpeed * 2 * Time.deltaTime);
                if (!isJumping)
                    rb.drag = dragForce;
            }
            /*
            else
            {
                if (rb.velocity.y > 0 && !grounded)
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
            */

            if (jumpCheck)
            {
                RaycastHit2D jumpCheckUp = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, jumpRayUpLength, LayerMask.GetMask("Ground"));
                if (!jumpCheckUp && targetIsInFront)
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
                                    StartCoroutine(Jump(0, true));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Mathf.Approximately(Mathf.Abs(jumpCheck.normal.x), 1f))
                        {
                            if (dist > 2f && !isJumping && isGrounded)
                            {
                                jumpTarget.position = Vision(target.position, false);
                                if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                                {
                                    StartCoroutine(Jump(0, true));
                                }
                            }
                        }
                    }
                }
            }
        }
        else
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
                        GoRight();
                    else
                        GoLeft();

                    if (targetIsUp)
                        GoUp(1);
                    else
                        GoDown(1);

                }
                else
                    isStopped = true;
            }
        }
    }

    void JumpRight()
    {
        if (!isFacingRight && rb.velocity.x > 0)
            Turn();

        if (!isJumping && isGrounded)
        {
            jumpTarget.position = new Vector2(transform.position.x + minJumpDistance, transform.position.y);
            StartCoroutine(Jump(1, false));
        }
    }

    void JumpLeft()
    {
        if (isFacingRight && rb.velocity.x < 0)
            Turn();

        if (!isJumping && isGrounded)
        {
            jumpTarget.position = new Vector2(transform.position.x - minJumpDistance, transform.position.y);
            StartCoroutine(Jump(1, false));
        }
    }

    void GoRight()
    {
        Vector2 force = new Vector2(moveSpeed, 0);

        if (rb.velocity.x < 0)
            force.x -= rb.velocity.x;
        else if (rb.velocity.x < minMoveSpeed)
            force.x += minMoveSpeed;

        rb.AddForce(force);

        if (!GetComponent<CreatureUnderwater>().isUnderwater)
        {
            if (!isFacingRight && rb.velocity.x > 0)
                Turn();
        }
    }

    void GoLeft()
    {
        Vector2 force = new Vector2(-moveSpeed, 0);

        if (rb.velocity.x > 0)
            force.x -= rb.velocity.x;
        else if (rb.velocity.x > -minMoveSpeed)
            force.x -= minMoveSpeed;

        rb.AddForce(force);

        if (!GetComponent<CreatureUnderwater>().isUnderwater)
        {
            if (isFacingRight && rb.velocity.x < 0)
                Turn();
        }
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

    IEnumerator Jump(float offset, bool trueJump)
    {
        //Debug.Log("isJumping");
        isJumping = true;
        rb.drag = 0;
        Vector2 jumpDirection = new Vector2(jumpTarget.position.x, jumpTarget.position.y) - rb.position;
        jumpDirection.x -= ((jumpDirection.x - offset * facingDirection) / 2);
        if (trueJump)
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

        /*
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float targetAngle = Mathf.Lerp(0, 80, (jumpTarget.position.y - rb.position.y) / 5);
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle * facingDirection, timer / duration);
            transform.eulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }
        */

        rb.AddForce(jumpForce, ForceMode2D.Impulse);
        isJumping = false;
        yield return null;
    }

    void Turn()
    {

        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;

        isFacingRight = !isFacingRight;

        float turnDirection = isFacingRight ? 1 : -1;
        JointAngleLimits2D limit = new JointAngleLimits2D();
        limit.min = tailJoints[0].limits.min + (220 * turnDirection);
        limit.max = tailJoints[0].limits.max + (200 * turnDirection);
        tailJoints[0].limits = limit;

        /*
        for (int i = 0; i < tailJoints.Length; i ++)
        {
            
            if (i == 0)
            {
                scale = tailJoints[i].gameObject.transform.localScale;
                scale.x *= -1;
                tailJoints[i].gameObject.transform.localScale = scale;
            }
            
            
            JointAngleLimits2D limit = new JointAngleLimits2D();
            limit.min = tailJoints[i].limits.min + (180 * facingDirection);
            limit.max = tailJoints[i].limits.max + (180 * facingDirection);
            tailJoints[i].limits = limit;
            
        }
        */

        foreach (FrogLegAnimation leg in legs)
        {
            leg.Turn();
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PlayerPermanent>().isInvincible)
            {
                if (collision.gameObject.GetComponent<PlayerPermanent>().poison != null)
                    collision.gameObject.GetComponent<PlayerPermanent>().StopPoison();
                collision.gameObject.GetComponent<PlayerPermanent>().poison = StartCoroutine(collision.gameObject.GetComponent<PlayerPermanent>().Poison(poison.effectDuration, poison.effectMagnitude, poison.effectFrequency));
            }
            collision.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, true, gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Ground Check
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.down * groundRaycastLenght);

        Gizmos.DrawRay(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down * jumpRayDownLength);
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.up * jumpRayUpLength);
        Gizmos.DrawRay(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.right * facingDirection * jumpRayForwardLength);

        for (int i = 0; i < 11; i++)
        {
            Gizmos.DrawRay(new Vector2((transform.position.x - ((11 / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + sensorStartOffset), Vector2.up * sensorRayLength);
        }

        /*
        for (int i = 0; i < rayCount; i++)
        {
            Gizmos.DrawRay(new Vector2(head.transform.position.x, head.transform.position.y + ((float)i / 3)), Vector2.right * facingDirection); // Visualize the ray
        }
        */
    }
}
