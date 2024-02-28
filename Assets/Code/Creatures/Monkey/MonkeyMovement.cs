using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyMovement : MonoBehaviour
{
    [SerializeField] MonkeyPathfinding pathfinding;
    private Rigidbody2D rb;

    [Header("Hover Variables")]
    [SerializeField] private float monkeyHeight;
    [SerializeField] float springStrenght;
    [SerializeField] float springDamper;
    [SerializeField] float uprightSpringStrenght;
    [SerializeField] float uprightSpringDamper;

    [SerializeField] private float speed;
    [SerializeField] float rotateSpeed;

    [Header("Ground Variables")]
    [SerializeField] Vector2 groundCheckOffsets;
    [SerializeField] float wallsCheckOffset;

    [Header("Climb Variables")]
    [SerializeField] float climbUpForce;
    [SerializeField] float climbUpAngle;
    [SerializeField] float climbUpOffset;
    bool isClimbing;

    public bool isFacingRight = false;
    private bool targetIsRight;

    float facingDirection;
    public bool isGrounded;

    [Header("Sensor Variables")]
    [SerializeField] int numberOfRays;
    RaycastHit2D[] upwardSensor;
    public bool ledgeFound = false;
    [SerializeField] Vector2 ledgePos;
    [SerializeField] float ledgeOffset;
    [SerializeField] [Range(0, 1)] float ledgeRaysStep;
    bool sensorUp = false;

    [Header("Sight Variables")]
    [SerializeField] Transform monkeyHead;
    [SerializeField] float sightAngle = 90f;
    [SerializeField] int rayCount = 10;
    [SerializeField] float rayDistance = 5f;
    [SerializeField] LayerMask layerMask;

    [Header("Jump Variables")]
    [SerializeField] float jumpYOffset;
    [SerializeField] float jumpDownAngle;
    [SerializeField] Vector2 jumpForce;

    bool isJumping;

    float angleStep;
    float startAngle;

    [SerializeField] Transform target;
    bool isSearchingForTarget;
    [SerializeField] Transform jumpTarget;


    // Start is called before the first frame update
    void Start()
    {
        pathfinding = GetComponent<MonkeyPathfinding>();
        rb = GetComponent<Rigidbody2D>();
        upwardSensor = new RaycastHit2D[numberOfRays];
        layerMask = LayerMask.GetMask("Ground");
        Debug.DrawLine(transform.position, target.position, Color.red, 25);
    }

    // Update is called once per frame
    void Update()
    {
        //Vision();

        facingDirection = isFacingRight ? 1 : -1;

        if (pathfinding.isPathfinding)
        {
            if (isSearchingForTarget)
                isSearchingForTarget = false;

            //Debug.Log(Vector2.Distance(pathfinding.path.lookPoints[pathfinding.pathIndex], transform.position));
            if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y))
            {
                if (pathfinding.path != null)
                    targetIsRight = (pathfinding.path.lookPoints[pathfinding.pathIndex].x - transform.position.x) > 0 ? true : false;
                if (targetIsRight != isFacingRight)
                {
                    Turn();
                }
            }
        }
        else
        {
            if (!isSearchingForTarget)
                isSearchingForTarget = true;

            if (isSearchingForTarget)
            {
                //Debug.Log(pathfinding.target.position.x - transform.position.x);
                targetIsRight = (target.position.x - transform.position.x) > 0 ? true : false;
                if (targetIsRight != isFacingRight)
                {
                    Turn();
                }
            }
        }

        if (ledgeFound)
        {
            target.position = new Vector2(ledgePos.x + ledgeOffset, transform.position.y);
            if (Vector2.Distance(target.position, transform.position) < 0.7f)
            {
                if (sensorUp)
                {
                    pathfinding.NewTarget(GameObject.FindGameObjectWithTag("Player"));
                    target.position = ledgePos;
                    StartCoroutine(Jump(1));
                    ledgeFound = false;
                }
                else
                {
                    pathfinding.NewTarget(GameObject.FindGameObjectWithTag("Player"));
                    ledgeFound = false;
                }
            }
        }
    }
    
    private void FixedUpdate()
    {
        //Navigation
        RaycastHit2D grounded = Physics2D.Raycast(origin: new Vector2(transform.position.x + 0.75f * facingDirection, transform.position.y), direction: Vector2.down, monkeyHeight * 1.75f, LayerMask.GetMask("Ground"));
        isGrounded = grounded;
        /*
        if (grounded.collider != null)
        {
            transform.up = Vector2.Lerp(transform.up, grounded.normal, rotateSpeed * Time.deltaTime);
            Debug.Log(transform.up);
        }
        */

        if (pathfinding.isPathfinding && pathfinding.path != null)
        {
            //Debug.Log(Mathf.Abs(pathfinding.path.lookPoints[pathfinding.pathIndex].x - rb.position.x));
            if (Mathf.Abs(pathfinding.path.lookPoints[pathfinding.pathIndex].x - rb.position.x) > 0.1f)
            {
                if (!isClimbing)
                {
                    climbUpOffset = 0f;
                    wallsCheckOffset = 2.07f;
                }
                if (isGrounded || rb.velocity.y >= 0)
                {
                    Vector2 direction = (pathfinding.path.lookPoints[pathfinding.pathIndex] - rb.position).normalized;
                    Vector2 force = new Vector2(direction.x, direction.y) * pathfinding.followPathSpeed * pathfinding.speedPercent * Time.deltaTime;
                    rb.AddForce(force);
                }
            }
            else
            {
                //isSearching = true;
                if (pathfinding.path.lookPoints[pathfinding.pathIndex].y - rb.position.y > 0 && !ledgeFound && isGrounded)
                {
                    for (int i = 0; i < upwardSensor.Length; i++)
                    {
                        upwardSensor[i] = (Physics2D.Raycast(new Vector2((transform.position.x - ((upwardSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + 2), Vector2.up, 5f, LayerMask.GetMask("Ground")));
                    }
                    for (int i = upwardSensor.Length / 2; i < upwardSensor.Length; i++)
                    {
                        if (upwardSensor[i] && i != upwardSensor.Length - 1)
                        {
                            Debug.DrawLine(new Vector2((transform.position.x - ((upwardSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + 2), upwardSensor[i].point, Color.green);
                            if (!upwardSensor[i + 1])
                            {
                                ledgePos = upwardSensor[i].point;
                                ledgeFound = true;
                                sensorUp = true;
                                pathfinding.isPathfinding = false;
                                ledgeOffset = Mathf.Abs(ledgeOffset);
                                Debug.Log("Right");
                                break;
                            }
                        }
                    }
                    if (!ledgeFound)
                    {
                        for (int i = 0; i < upwardSensor.Length / 2; i++)
                        {
                            if (upwardSensor[i] && i != 0)
                            {
                                if (!upwardSensor[i - 1])
                                {
                                    ledgePos = upwardSensor[i].point;
                                    ledgeFound = true;
                                    sensorUp = true;
                                    pathfinding.isPathfinding = false;
                                    ledgeOffset *= -1;
                                    Debug.Log("Left");
                                    break;
                                }
                            }
                        }
                    }
                    if (!ledgeFound)
                    {
                        climbUpOffset = 2.5f;
                        wallsCheckOffset = 3f;
                    }
                }
                else if (pathfinding.path.lookPoints[pathfinding.pathIndex].y - rb.position.y < 0 && !ledgeFound && isGrounded)
                {
                    for (int i = 0; i < upwardSensor.Length; i++)
                    {
                        upwardSensor[i] = (Physics2D.Raycast(new Vector2((transform.position.x - ((upwardSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + 2), Vector2.down, 5f, LayerMask.GetMask("Ground")));
                    }
                    for (int i = upwardSensor.Length / 2; i < upwardSensor.Length; i++)
                    {
                        if (upwardSensor[i] && i != upwardSensor.Length - 1)
                        {
                            Debug.DrawLine(new Vector2((transform.position.x - ((upwardSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + 2), upwardSensor[i].point, Color.green);
                            if (!upwardSensor[i + 1])
                            {
                                ledgePos = upwardSensor[i].point;
                                ledgeFound = true;
                                sensorUp = false;
                                pathfinding.isPathfinding = false;
                                ledgeOffset = Mathf.Abs(ledgeOffset);
                                Debug.Log("Right");
                                break;
                            }
                        }
                    }
                    if (!ledgeFound)
                    {
                        for (int i = 0; i < upwardSensor.Length / 2; i++)
                        {
                            if (upwardSensor[i] && i != 0)
                            {
                                if (!upwardSensor[i - 1])
                                {
                                    ledgePos = upwardSensor[i].point;
                                    ledgeFound = true;
                                    sensorUp = false;
                                    pathfinding.isPathfinding = false;
                                    ledgeOffset *= -1;
                                    Debug.Log("Left");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (isSearchingForTarget)
        {
            if (Mathf.Abs(target.position.x - rb.position.x) > 0.1f)
            {
                Vector2 direction = (new Vector2(target.position.x, target.position.y) - rb.position).normalized;
                Vector2 force = new Vector2(direction.x, direction.y) * pathfinding.followPathSpeed * Time.deltaTime;
                rb.AddForce(force);
            }
        }


        //Mouvement
        RaycastHit2D wallCheckRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, wallsCheckOffset, LayerMask.GetMask("Ground"));
        RaycastHit2D wallCheckLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, wallsCheckOffset, LayerMask.GetMask("Ground"));
        RaycastHit2D climbWalls1 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.right, wallsCheckOffset, LayerMask.GetMask("Ground"));
        RaycastHit2D climbWalls2 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.left, wallsCheckOffset, LayerMask.GetMask("Ground"));
        RaycastHit2D climbWalls3 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + climbUpOffset), Vector2.left, wallsCheckOffset, LayerMask.GetMask("Ground"));
        RaycastHit2D climbWalls4 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + climbUpOffset), Vector2.right, wallsCheckOffset, LayerMask.GetMask("Ground"));
        if (climbWalls1.collider != null || climbWalls2.collider != null || climbWalls3.collider != null || climbWalls4.collider != null)
        {
            isClimbing = true;
            rb.AddForce(new Vector2(0.1f * facingDirection, 1) * climbUpForce);
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, climbUpAngle * facingDirection, rotateSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        else
        {
            if (isClimbing)
                rb.AddForce(Vector2.right * facingDirection, ForceMode2D.Impulse);
            isClimbing = false;
            RaycastHit2D hover1 = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x, transform.position.y), Vector2.down, monkeyHeight * 1.5f, LayerMask.GetMask("Ground"));
            RaycastHit2D hover2 = Physics2D.Raycast(new Vector2(transform.position.x - groundCheckOffsets.x, transform.position.y), Vector2.down, monkeyHeight * 1.5f, LayerMask.GetMask("Ground"));
            if (hover1.collider != null)
            {
                float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity);

                float difference = hover1.distance - monkeyHeight;
                float springForce = (difference * springStrenght) - (rayDirVel * springDamper);

                rb.AddForce(Vector2.down * springForce);

                if (hover2.collider == null)
                {
                    monkeyHeight = 1.20f;
                    RaycastHit2D jumpCheck = Physics2D.Raycast(transform.position, Vector2.left, 7f, LayerMask.GetMask("Ground"));
                    if (!jumpCheck)
                    {
                        if (pathfinding.isPathfinding)
                        {
                            if (!isFacingRight && !isGrounded && Vector2.Distance(pathfinding.path.lookPoints[pathfinding.pathIndex], rb.position) > 3f && !isJumping)
                            {
                                jumpTarget.position = Vision(pathfinding.path.lookPoints[pathfinding.pathIndex]);
                                if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                                {
                                    StartCoroutine(Jump(-1));
                                }
                            }
                        }
                        else if (isSearchingForTarget)
                        {
                            if (!isFacingRight && !isGrounded && Vector2.Distance(target.position, rb.position) > 3f && !isJumping)
                            {
                                jumpTarget.position = Vision(target.position);
                                if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                                {
                                    StartCoroutine(Jump(-1));
                                }
                            }
                        }
                        //rb.AddForce(new Vector2(jumpForce * facingDirection, jumpForce), ForceMode2D.Impulse);
                    }
                }
                else
                {
                    monkeyHeight = 1.5f;
                    isJumping = false;
                }
            }
            else if (hover2.collider != null)
            {
                float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity);

                float difference = hover2.distance - monkeyHeight;
                float springForce = (difference * springStrenght) - (rayDirVel * springDamper);

                rb.AddForce(Vector2.down * springForce);

                if (hover1.collider == null)
                {
                    monkeyHeight = 1.20f;
                    RaycastHit2D jumpCheck = Physics2D.Raycast(transform.position, Vector2.right, 7f, LayerMask.GetMask("Ground"));
                    if (!jumpCheck)
                    {
                        if (pathfinding.isPathfinding)
                        {
                            if (isFacingRight && !isGrounded && Vector2.Distance(pathfinding.path.lookPoints[pathfinding.pathIndex], rb.position) > 3f && !isJumping)
                            {
                                jumpTarget.position = Vision(pathfinding.path.lookPoints[pathfinding.pathIndex]);
                                if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                                {
                                    StartCoroutine(Jump(-1));
                                }
                            }
                        }
                        else if (isSearchingForTarget)
                        {
                            if (isFacingRight && !isGrounded && Vector2.Distance(target.position, rb.position) > 3f && !isJumping)
                            {
                                jumpTarget.position = Vision(target.position);
                                if (new Vector2(jumpTarget.position.x, jumpTarget.position.y) != Vector2.zero)
                                {
                                    StartCoroutine(Jump(-1));
                                }
                            }
                        }

                        //rb.AddForce(new Vector2(jumpForce * facingDirection, jumpForce), ForceMode2D.Impulse);
                    }
                }
                else
                {
                    monkeyHeight = 1.5f;
                    isJumping = false;
                }
            }

            if (grounded.collider != null)
            {
                Quaternion targetRot = Quaternion.Euler(grounded.normal);

                Quaternion currentRot = transform.rotation;
                Quaternion goalRot = Utilities.ShortestRotation(targetRot, currentRot);

                Vector3 rotAxis;
                float rotDegrees;

                goalRot.ToAngleAxis(out rotDegrees, out rotAxis);
                rotAxis.Normalize();

                float rotRadians = rotDegrees * Mathf.Deg2Rad;

                rb.AddTorque((rotAxis.z * (rotRadians * uprightSpringStrenght)) - (rb.angularVelocity * uprightSpringDamper));
            }
            else
            {
                if (rb.velocity.y > 0 && !(hover1 || hover2))
                {
                    float angle = Mathf.LerpAngle(transform.eulerAngles.z, climbUpAngle * facingDirection, (rotateSpeed / 3) * Time.deltaTime);
                    transform.eulerAngles = new Vector3(0, 0, angle);
                }
                else if (rb.velocity.y < 0)
                {
                    float angle = Mathf.LerpAngle(transform.eulerAngles.z, -jumpDownAngle * facingDirection, (rotateSpeed / 3) * Time.deltaTime);
                    transform.eulerAngles = new Vector3(0, 0, angle);
                }
            }
        }
    }

    /*
if (pathfinding.isPathfinding)
{
    Vector2 direction = (pathfinding.path.lookPoints[pathfinding.pathIndex] - rb.position).normalized;
    Vector2 force = new Vector2(direction.x, direction.y) * pathfinding.followPathSpeed * speedFactor * pathfinding.speedPercent * Time.deltaTime;
    rb.AddForce(force);

    Vector2 lookDir = (pathfinding.path.lookPoints[pathfinding.pathIndex] - rb.position).normalized;
    transform.right = Vector2.Lerp(transform.right, lookDir * facingDirection, rotateSpeed * Time.deltaTime);

    RaycastHit2D closeToGround = Physics2D.Raycast(transform.position, -transform.up, monkeyHeight, LayerMask.GetMask("Ground"));
    if (closeToGround.collider == null)
    {
        rb.AddForce(-transform.up * fakeGravity);
    }
}
*/
    
    IEnumerator Jump(int direction)
    {
        rb.drag = 0;
        isJumping = true;
        Vector2 jumpDirection = new Vector2(jumpTarget.position.x, jumpTarget.position.y) - rb.position;
        jumpDirection.x -= (jumpDirection.x / 2) * facingDirection * direction;
        jumpDirection.y += jumpYOffset;
        Vector2 jumpPoint = rb.position + jumpDirection;

        jumpTarget.position = jumpPoint;
        Debug.Log(Vector2.Distance(new Vector2(jumpTarget.position.x, jumpTarget.position.y), rb.position));

        float timer = 0;
        float duration = 0.5f;
        
        float speedX = jumpPoint.x - rb.position.x;
        float speedY = Mathf.Sqrt(2 * Physics.gravity.magnitude * Mathf.Max(jumpPoint.y - rb.position.y, 3f));
        jumpForce = new Vector2(speedX, speedY);

        //Debug.Log(jumpForce);
        rb.velocity = Vector2.zero;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float targetAngle = Mathf.Lerp(0, 80, (jumpTarget.position.y - rb.position.y) / 5);
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle * facingDirection, timer / duration);
            transform.eulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
    }

    void Turn()
    {
        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    Vector2 Vision(Vector2 target)
    {
        angleStep = sightAngle / (rayCount - 1);

        // Start angle from left limit of sight angle
        startAngle = -sightAngle / 2;

        float distanceFromTarget = 1000f;

        Vector2 targetPosition = Vector2.zero;

        for (int i = 0; i < rayCount; i++)
        {
            // Calculate current angle
            float angle = startAngle + angleStep * i;

            // Calculate direction of the ray
            Vector3 direction = Quaternion.Euler(0, 0, angle) * monkeyHead.transform.right * facingDirection;

            // Cast a ray in the calculated direction
            RaycastHit2D hit = Physics2D.Raycast(monkeyHead.transform.position, direction, rayDistance, layerMask);

            // Check if the ray hits a platform collider
            if (hit.collider != null)
            {
                Debug.DrawLine(monkeyHead.transform.position, hit.point, Color.green, 10); // Visualize the ray
                if (Vector2.Distance(target, hit.point) < distanceFromTarget)
                {
                    distanceFromTarget = Vector2.Distance(target, hit.point);
                    targetPosition = hit.point;
                }
            }
        }
        //Debug.Log(targetPosition);
        return targetPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //HoverCheck
        Gizmos.DrawRay(new Vector2(transform.position.x + groundCheckOffsets.x, transform.position.y), Vector2.down * monkeyHeight * 1.5f);
        Gizmos.DrawRay(new Vector2(transform.position.x - groundCheckOffsets.x, transform.position.y), Vector2.down * monkeyHeight * 1.5f);
        //Ground Check
        Gizmos.DrawRay(new Vector2(transform.position.x + 0.75f * facingDirection, transform.position.y), Vector2.down * monkeyHeight * 1.75f);

        //Wall Check
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.left * wallsCheckOffset);
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y + climbUpOffset), Vector2.left * wallsCheckOffset);
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.right * wallsCheckOffset);
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y + climbUpOffset), Vector2.right * wallsCheckOffset);

        Gizmos.DrawRay(transform.position, Vector2.right * 7f);
        Gizmos.DrawRay(transform.position, Vector2.left * 7f);
        /*
        if (pathfinding.isPathfinding)
        {
            for (int i = 0; i < upwardSensor.Length; i++)
            {
                Gizmos.DrawRay(new Vector2((transform.position.x - ((upwardSensor.Length / 2) * ledgeRaysStep) + i * ledgeRaysStep), transform.position.y + 2), Vector2.up * 5f);
            }
        }
        */
        if (ledgeFound)
        {
            Gizmos.DrawSphere(new Vector3(ledgePos.x, ledgePos.y, 0), 0.4f);
        }
    }
}
