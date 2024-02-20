using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyMovement : MonoBehaviour
{
    private MonkeyPathfinding pathfinding;
    private Rigidbody2D rb;
    [SerializeField] private float monkeyHeight;
    [SerializeField] float springStrenght;
    [SerializeField] float springDamper;

    [SerializeField] private float speed;
    [SerializeField] float rotateSpeed;
    [SerializeField] Vector2 groundCheckOffsets;
    [SerializeField] float groundCheckAngle;
    [SerializeField] float wallsCheckOffset;
    [SerializeField] float climbUpForce;
    [SerializeField] float climbUpAngle;
    [SerializeField] float jumpForce;

    public bool isFacingRight = false;
    public bool targetIsRight;
    float facingDirection;
    public bool isGrounded;

    [SerializeField] float speedFactor;
    [SerializeField] float fakeGravity;
    private bool isOnDifficultTerrain;


    // Start is called before the first frame update
    void Start()
    {
        pathfinding = GetComponent<MonkeyPathfinding>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        facingDirection = isFacingRight ? 1 : -1;

        if (pathfinding.isPathfinding)
        {
            if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y))
            {
                //Debug.Log(pathfinding.target.position.x - transform.position.x);
                targetIsRight = (pathfinding.path.lookPoints[pathfinding.pathIndex].x - transform.position.x) > 0 ? true : false;
                if (targetIsRight != isFacingRight)
                {
                    Turn();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isOnDifficultTerrain)
                DifficultTerrainMode(true);
            else
                DifficultTerrainMode(false);
        }
    }
    
    private void FixedUpdate()
    {
        if (!isOnDifficultTerrain)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin: new Vector2(transform.position.x + 0.5f * facingDirection, transform.position.y), direction: Vector2.down, 3f, LayerMask.GetMask("Ground"));
            isGrounded = hit;
            transform.up = Vector2.Lerp(transform.up, hit.normal, rotateSpeed * Time.deltaTime);

            if (pathfinding.isPathfinding)
            {
                Vector2 direction = (pathfinding.path.lookPoints[pathfinding.pathIndex] - rb.position).normalized;
                Vector2 force = new Vector2(direction.x, direction.y) * pathfinding.followPathSpeed * pathfinding.speedPercent * Time.deltaTime;
                rb.AddForce(force);
            }

            RaycastHit2D climbWalls1 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 1f), Vector2.right, wallsCheckOffset, LayerMask.GetMask("Ground"));
            RaycastHit2D climbWalls2 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 1f), Vector2.left, wallsCheckOffset, LayerMask.GetMask("Ground"));
            if (climbWalls1.collider != null || climbWalls2.collider != null)
            {
                rb.AddForce(new Vector2(0.1f * facingDirection, 1) * climbUpForce);
                float angle = Mathf.LerpAngle(transform.eulerAngles.z, climbUpAngle * facingDirection, rotateSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, angle);
            }
            else
            {
                float angle = Mathf.LerpAngle(transform.eulerAngles.z, 0, rotateSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, 0, angle);

                RaycastHit2D hover1 = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsets.x, transform.position.y), Vector2.down, monkeyHeight * 2, LayerMask.GetMask("Ground"));
                RaycastHit2D hover2 = Physics2D.Raycast(new Vector2(transform.position.x - groundCheckOffsets.x, transform.position.y), Vector2.down, monkeyHeight * 2, LayerMask.GetMask("Ground"));
                if (hover1.collider != null)
                {
                    float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity);

                    float difference = hover1.distance - monkeyHeight;
                    float springForce = (difference * springStrenght) - (rayDirVel * springDamper);

                    rb.AddForce(Vector2.down * springForce);

                    if (hover2.collider == null && !isFacingRight && !isGrounded)
                    {
                        rb.AddForce(new Vector2(jumpForce * facingDirection, jumpForce), ForceMode2D.Impulse);
                    }
                }
                else if (hover2.collider != null)
                {
                    float rayDirVel = Vector2.Dot(Vector2.down, rb.velocity);

                    float difference = hover2.distance - monkeyHeight;
                    float springForce = (difference * springStrenght) - (rayDirVel * springDamper);

                    rb.AddForce(Vector2.down * springForce);

                    if (hover1.collider == null && isFacingRight && !isGrounded)
                    {
                        rb.AddForce(new Vector2(jumpForce * facingDirection, jumpForce), ForceMode2D.Impulse);
                    }
                }
            }
        }
        else
        {
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
        }
    }

    void DifficultTerrainMode(bool activated)
    {
        isOnDifficultTerrain = activated;
        rb.gravityScale = activated ? 0 : 1;
        rb.drag = activated ? 1.5f : 0.5f;
    }
    

    void Turn()
    {
        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;

        isFacingRight = !isFacingRight;
        Debug.Log("OK");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector2(transform.position.x + groundCheckOffsets.x, transform.position.y), Vector2.down * monkeyHeight * 2);
        Gizmos.DrawRay(new Vector2(transform.position.x - groundCheckOffsets.x, transform.position.y), Vector2.down * monkeyHeight * 2);
        Gizmos.DrawRay(new Vector2(transform.position.x + 0.5f * facingDirection, transform.position.y), Vector2.down * 3f);
        Gizmos.DrawRay(transform.position, Vector2.right * wallsCheckOffset);
        Gizmos.DrawRay(transform.position, Vector2.left * wallsCheckOffset);
    }
}
