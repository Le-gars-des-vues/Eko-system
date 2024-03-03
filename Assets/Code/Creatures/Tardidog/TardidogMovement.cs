using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TardidogMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Transform target;

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
    [SerializeField] float slowDownThreshold;
    [SerializeField] float minMoveSpeed;
    [SerializeField] List<TardidogLegAnimation> legs = new List<TardidogLegAnimation>();

    [Header("Checks")]
    public bool isStopped;
    bool isJumping;
    bool isGrounded;
    public bool isFacingRight = true;
    public int facingDirection;
    bool targetIsRight;

    [Header("Sight Variables")]
    public Transform head;
    public float startAngle;
    public float angleStep;
    public float sightAngle;
    public float rayCount;
    public float rayDistance;

    [SerializeField] CreatureState state;
    [SerializeField] TardidogAttack atk;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isFacingRight = transform.localScale.x == 1 ? true : false;
        state = GetComponent<CreatureState>();
    }

    // Update is called once per frame
    void Update()
    {
        facingDirection = isFacingRight ? 1 : -1;
        //Vision();
        targetIsRight = (target.position.x - transform.position.x) > 0 ? true : false;
        if (targetIsRight != isFacingRight)
        {
            Turn();
            foreach (TardidogLegAnimation leg in legs)
            {
                leg.Turn();
            }
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D grounded = Physics2D.Raycast(origin: new Vector2(transform.position.x, transform.position.y), direction: Vector2.down, groundRaycastLenght, LayerMask.GetMask("Ground"));
        isGrounded = grounded;

        RaycastHit2D frontCheck = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down, hoverRaycastLenght, LayerMask.GetMask("Ground"));
        RaycastHit2D backCheck = Physics2D.Raycast(new Vector2(transform.position.x - groundCheckOffsets.x * facingDirection, transform.position.y), Vector2.down, hoverRaycastLenght, LayerMask.GetMask("Ground"));
        if (frontCheck.collider != null)
        {
            float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity);

            float difference = frontCheck.distance - creatureHeight;
            float springForce = (difference * springStrenght) - (rayDirVel * springDamper);

            rb.AddForce(Vector2.down * springForce);

            if (backCheck.collider == null)
            {
                /*
                creatureHeight = 1.20f;

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
                */
            }
            else
            {
                //creatureHeight = 1.5f;
                isJumping = false;
            }
        }
        else if (backCheck.collider != null)
        {
            float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity);

            float difference = backCheck.distance - creatureHeight;
            float springForce = (difference * springStrenght) - (rayDirVel * springDamper);

            rb.AddForce(Vector2.down * springForce);

            if (frontCheck.collider == null)
            {
                /*
                creatureHeight = 1.20f;

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
                */
            }
            else
            {
                //creatureHeight = 1.5f;
                isJumping = false;
            }
        }

        float dist = Mathf.Abs(target.position.x - rb.position.x);
        if (!state.isAttacking)
        {
            if (dist >= slowDownThreshold)
                moveSpeed = 5;
            else if (dist < 0.1f)
                rb.velocity = Vector2.zero;
            else if (dist < slowDownThreshold)
                moveSpeed = Mathf.Lerp(moveSpeed, 1.5f, (slowDownThreshold - dist / slowDownThreshold));

            if (dist > 1f)
            {
                isStopped = false;
                if (targetIsRight)
                    GoRight(1);
                else
                    GoLeft(1);
            }
            else
                isStopped = true;
        }
        else
        {
            moveSpeed = 5;
            if (dist > 0.5f)
            {
                isStopped = false;
                if (targetIsRight)
                    GoRight(1.5f);
                else
                    GoLeft(1.5f);
            }
            else
                isStopped = true;
        }



        if (isGrounded)
        {
            if (grounded.normal != Vector2.up)
                creatureHeight = 0.45f;
            else
                creatureHeight = 0.7f;
            transform.up = Vector3.Lerp(transform.up, grounded.normal, rotateSpeed * Time.deltaTime);
        }
        else
        {
            if (rb.velocity.y > 0 && !(frontCheck || backCheck))
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.z, upAngle * facingDirection, (rotateSpeed / 3) * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, angle);
            }
            else if (rb.velocity.y < 0)
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.z, -downAngle * facingDirection, (rotateSpeed / 3) * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, angle);
            }
        }
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

    void Turn()
    {
        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    void Vision()
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
            Vector3 direction = Quaternion.Euler(0, 0, angle) * head.transform.right * facingDirection;

            // Cast a ray in the calculated direction
            RaycastHit2D hit = Physics2D.Raycast(head.transform.position, direction, rayDistance, LayerMask.GetMask("Ground"));

            Debug.DrawRay(head.transform.position, direction * rayDistance, Color.green); // Visualize the ray
            // Check if the ray hits a platform collider
            /*
            if (hit.collider != null)
            {
                
                if (Vector2.Distance(target, hit.point) < distanceFromTarget)
                {
                    distanceFromTarget = Vector2.Distance(target, hit.point);
                    targetPosition = hit.point;
                }
            }
            */
        }
        //Debug.Log(targetPosition);
        //return targetPosition;
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
        Gizmos.DrawRay(new Vector2(transform.position.x + groundCheckOffsets.x, transform.position.y), Vector2.down * hoverRaycastLenght);
        Gizmos.DrawRay(new Vector2(transform.position.x - groundCheckOffsets.x, transform.position.y), Vector2.down * hoverRaycastLenght);
        //Ground Check
        Gizmos.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector2.down * groundRaycastLenght);
    }
}

