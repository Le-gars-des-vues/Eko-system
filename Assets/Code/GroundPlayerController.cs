using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlayerController : MonoBehaviour
{
    [Header("GameObjects")]
    public Rigidbody2D rb;

    [Header("Mouvement Variables")]
    public float walkSpeed;
    public float runSpeed;
    public float maxSpeed = 2f;
    private float targetSpeed;
    private float speedDif;
    private float mouvement;
    public float mouveAcceleration;
    public float accelTime;
    private float accelForce;
    public float deccelTime;
    private float deccelForce;
    public bool isFacingRight;
    public float coyoteTime;
    [Range(0.01f, 1)] public float accelAir;
    [Range(0.01f, 1)] public float deccelAir;

    [Header("Jump Variables")]
    public float jumpForce;
    private bool isJumping;
    private bool isJumpCut;
    private bool isJumpFalling;
    public float jumpHangTimeThreshold;
    public float jumpHangAccelMult;
    public float jumpHangMaxSpeedMult;
    public float pressedJumpTime;
    public float jumpBuffer = 0.1f;
    public float jumpHeight;
    public float jumpTimeToApex;
    public bool jumpedOnce => isGrounded ? false : true;
    public bool jumpedTwiced;

    [Header("Gravity Variables")]
    //Force de la gravite
    public float gravityScale;
    //Augmentation de la gravite en chute libre
    public float fallGravityMult;
    //Vitesse maximale en tombant
    public float maxFallSpeed;
    //Augmentation de la gravite en fastfall
    public float fastFallGravMult;
    //Vitesse maximale en fastfall
    public float maxFastFallSpeed;
    ////Augmentation de la gravite en jumpcut
    public float jumpCutGravMult;
    //Variable pour calculer la force de la gravite
    private float gravityStrength;
    [Range(0f, 1f)] public float jumpHangGravMult;

    [Header("Ground Collision Variables")]
    //Layer de sol
    public LayerMask groundLayer;
    //Taille du raycast de collision avec le sol
    public float groundRaycastLength;
    public bool isGrounded;

    [Header("Corner Collision Variables")]
    //Layer pour corriger les collisions avec les coins
    public LayerMask cornerCorrectLayer;
    //Taille des raycasts de collisions avec les coins
    public float topRaycastLength;
    public Vector3 edgeRaycastOffset;
    public Vector3 innerRaycastOffset;
    public bool canCornerCorrect;

    [Header("Wall Collision Variables")]
    //Boites de collision avec les murs pour les walljump
    public Transform frontWallCheckPoint;
    public Transform backWallCheckPoint;
    //Taille des boite invisibles qui check les collisions avec les murs pour les walljump
    public Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    //Timers
    private float lastOnWallTime;
    private float lastOnWallRightTime;
    private float lastOnWallLeftTime;

    [Header("Wall Jump Variables")]
    //Force du saut
    public Vector2 wallJumpForce;
    //A quel point le joueur controlle sont personnage lors d'un wall jump
    [Range(0f, 1f)] public float wallJumpRunLerp;
    //Pendant combien de temps est-ce que le joueur perd controlle de son personnage lors du walljump
    [Range(0f, 1.5f)] public float wallJumpTime;
    //Check & Timers
    private bool isWallJumping;
    private float wallJumpStartTime;
    private int lastWallJumpDir;

    [Header("Miscellaneous Variables")]
    [Range(0f, 1f)] public float blastRunLerp;

    [Header("Slide Variables")]
    //Check
    private bool isSliding;
    public float slideSpeed;
    public float slideAccel;

    // Start is called before the first frame update
    void Start()
    {
        //On set up quelques variables pour le deplacement
        rb = gameObject.GetComponent<Rigidbody2D>();
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrength / Physics2D.gravity.y;
        accelForce = (50 * accelTime) / maxSpeed;
        deccelForce = (50 * deccelTime) / maxSpeed;
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
        isFacingRight = true;
        //On set up la gravite de notre personnage
        SetGravityScale(gravityScale);
    }

    // Update is called once per frame
    void Update()
    {
        //Timers
        pressedJumpTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastOnWallRightTime -= Time.deltaTime;
        lastOnWallLeftTime -= Time.deltaTime;
        //Inputs
        if (GetInput().x != 0)
        {
            CheckDirectionToFace(GetInput().x > 0);
        }

        /*
        //Courir
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (isGrounded)
            {
                maxSpeed = runSpeed;
            }     
        }
        else
        {
            maxSpeed = walkSpeed;
        }
        */
            
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUpInput();
        }
        //State
        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;
            isJumpFalling = true;
        }
        if (isWallJumping && Time.time - wallJumpStartTime > wallJumpTime)
        {
            isWallJumping = false;
        }
        if (isGrounded && !isJumping && !isWallJumping)
        {
            isJumpCut = false;
            isJumpFalling = false;
            jumpedTwiced = false;
        }

        //Checks
        if (CanWallJump() && pressedJumpTime > 0)
        {
            isWallJumping = true;
            isJumping = false;
            isJumpCut = false;
            isJumpFalling = false;

            wallJumpStartTime = Time.time;
            lastWallJumpDir = (lastOnWallRightTime > 0) ? -1 : 1;

            WallJump(lastWallJumpDir);
        }
        else if (CanJump() && pressedJumpTime > 0)
        {
            isJumping = true;
            isWallJumping = false;
            isJumpCut = false;
            isJumpFalling = false;
            Jump();
        }

        if (CanSlide() && ((lastOnWallLeftTime > 0 && GetInput().x < 0) || (lastOnWallRightTime > 0 && GetInput().x > 0)))
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        //Gravity
        if (isSliding)
        {
            SetGravityScale(0f);
        }
        else if (rb.velocity.y < 00 && GetInput().y < 0)
        {
            SetGravityScale(gravityScale * fastFallGravMult);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFastFallSpeed));
        }
        else if (isJumpCut)
        {
            SetGravityScale(gravityScale * jumpCutGravMult);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else if ((isJumping || isWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
        {
            SetGravityScale(gravityScale * jumpHangGravMult);
        }
        else if (rb.velocity.y < 0)
        {
            SetGravityScale(gravityScale * fallGravityMult);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            SetGravityScale(gravityScale);
        }
    }

    private void FixedUpdate()
    {
        CheckCollision();
        if (isWallJumping)
        {
            MoveCharacter(wallJumpRunLerp);
        }
        else
        {
            MoveCharacter(1);
        }
        if (canCornerCorrect)
        {
            CornerCorrect();
        }
        if (isSliding)
        {
            Slide();
        }
    }

    //Inputs Functions
    public void OnJumpInput()
    {
        pressedJumpTime = jumpBuffer;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
        {
            isJumpCut = true;
        }
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //Action Functions
    private void MoveCharacter(float lerpAmount)
    {
        targetSpeed = GetInput().x * maxSpeed;
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);
        if (isGrounded)
        {
            mouveAcceleration = (Mathf.Abs(targetSpeed) > 0.1f) ? accelForce : deccelForce;
        }
        else
        {
            mouveAcceleration = (Mathf.Abs(targetSpeed) > 0.1f) ? accelForce * accelAir : deccelForce * deccelAir;
        }
        if ((isJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
        {
            mouveAcceleration *= jumpHangAccelMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }
        speedDif = targetSpeed - rb.velocity.x;
        mouvement = speedDif * mouveAcceleration;
        rb.AddForce(mouvement * Vector2.right, ForceMode2D.Force);
    }

    private void Jump()
    {
        if (jumpedOnce == true)
        {
            jumpedTwiced = true;
        }
        pressedJumpTime = 0;
        float force = jumpForce;
        if (rb.velocity.y < 0)
        {
            force -= rb.velocity.y;
        }
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void WallJump(int dir)
    {
        //Ensures we can't call Wall Jump multiple times from one press
        pressedJumpTime = 0;
        lastOnWallRightTime = 0;
        lastOnWallLeftTime = 0;

        Vector2 force = new Vector2(wallJumpForce.x, wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= rb.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        rb.AddForce(force, ForceMode2D.Impulse);
        Turn();
        jumpedTwiced = false;
    }

    private void Slide()
    {
        //We remove the remaining upwards Impulse to prevent upwards sliding
        if (rb.velocity.y > 0)
        {
            rb.AddForce(-rb.velocity.y * Vector2.up, ForceMode2D.Impulse);
        }
        float speedDif = slideSpeed - rb.velocity.y;
        float movement = speedDif * slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }

    //Checks Functions
    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastLength, groundLayer);
            
        if (!isJumping)
        {
            //Right Wall Check
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && isFacingRight) || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !isFacingRight)) && !isWallJumping)
            {
                lastOnWallRightTime = coyoteTime;
            }
                
            //Right Wall Check
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !isFacingRight) || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) && isFacingRight)) && !isWallJumping)
            {
                lastOnWallLeftTime = coyoteTime;
            }

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);
        }
        

        canCornerCorrect = Physics2D.Raycast(transform.position + edgeRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) && 
                            !Physics2D.Raycast(transform.position + innerRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) ||
                           Physics2D.Raycast(transform.position - edgeRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) && 
                           !Physics2D.Raycast(transform.position - innerRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer);
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight;
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
        {
            Turn();
        }
    }

    private bool CanJump()
    {
        return !jumpedTwiced && !isJumping;
    }

    private bool CanWallJump()
    {
        return pressedJumpTime > 0 && lastOnWallTime > 0 && !isGrounded && (!isWallJumping || (lastOnWallRightTime > 0 && lastWallJumpDir == 1) || (lastOnWallLeftTime > 0 && lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return isJumping && rb.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return isWallJumping && rb.velocity.y > 0;
    }

    public bool CanSlide()
    {
        if (lastOnWallTime > 0 && !isJumping && !isWallJumping && !isGrounded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Misc. Functions
    public void SetGravityScale (float scale)
    {
        rb.gravityScale = scale;
    }

    void CornerCorrect()
    {
        //Push player to the right
        RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRaycastOffset + Vector3.up * topRaycastLength, Vector3.left, topRaycastLength, cornerCorrectLayer);
        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRaycastLength,
                transform.position - edgeRaycastOffset + Vector3.up * topRaycastLength);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            return;
        }

        //Push player to the left
        hit = Physics2D.Raycast(transform.position + innerRaycastOffset + Vector3.up * topRaycastLength, Vector3.right, topRaycastLength, cornerCorrectLayer);
        if (hit.collider != null)
        {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRaycastLength,
                transform.position + edgeRaycastOffset + Vector3.up * topRaycastLength);
            transform.position = new Vector3(transform.position.x - newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            GetComponent<WaterPlayerController>().enabled = true;
            SetGravityScale(0.1f);
            rb.drag = 1f;
            GetComponent<GroundPlayerController>().enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastLength);

        //Corner Check
        Gizmos.DrawLine(transform.position + edgeRaycastOffset, transform.position + edgeRaycastOffset + Vector3.up * topRaycastLength);
        Gizmos.DrawLine(transform.position - edgeRaycastOffset, transform.position - edgeRaycastOffset + Vector3.up * topRaycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset, transform.position + innerRaycastOffset + Vector3.up * topRaycastLength);
        Gizmos.DrawLine(transform.position - innerRaycastOffset, transform.position - innerRaycastOffset + Vector3.up * topRaycastLength);

        //Corner Distance Check
        Gizmos.DrawLine(transform.position - innerRaycastOffset + Vector3.up * topRaycastLength,
                        transform.position - innerRaycastOffset + Vector3.up * topRaycastLength + Vector3.left * topRaycastLength);
        Gizmos.DrawLine(transform.position + innerRaycastOffset + Vector3.up * topRaycastLength,
                        transform.position + innerRaycastOffset + Vector3.up * topRaycastLength + Vector3.right * topRaycastLength);
    }
}
