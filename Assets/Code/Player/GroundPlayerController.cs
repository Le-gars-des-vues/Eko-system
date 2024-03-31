using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Mouvement Variables")]
    //Vitesse de marche
    public float walkSpeed;
    //Vitesse de course
    public float runSpeed;
    //Vitesse maximum
    public float maxSpeed = 2f;
    //Actual vitesse du personnage in-game
    private float targetSpeed;
    //Permet de changer de direction tout en gardant sa vitesse
    private float speedDif;
    //Force du mouvement (physic-based!)
    private float mouvement;
    //Acceleration
    public float mouveAcceleration;
    //Temps pour atteindre la vitesse maximale
    public float accelTime;
    //Force d'acceleration
    private float accelForce;
    //Temps pour ralentir
    public float deccelTime;
    //Force de decceleration
    private float deccelForce;
    //Check de direction
    private bool isFacingRight;
    //Temps de "grace" ou le joueur peut encore sauter meme s'il n'est plus sur le sol
    public float coyoteTime;
    //Acceleration et decceleration dans les airs
    [Range(0.01f, 1)] public float accelAir;
    [Range(0.01f, 1)] public float deccelAir;

    [Header("Jump Variables")]
    //Force du jump
    private float jumpForce;
    //Checks de saut
    private bool isJumping;
    //Check si le joueur a arreter son saut
    private bool isJumpCut;
    //Check si le joueur est en train de tomber
    private bool isJumpFalling;
    //Temps durant lequel le joueur est considerer a l'apex de son saut
    public float jumpHangTimeThreshold;
    //Multiplicateur de vitesse quand le joueur est a l'apex de son saut
    public float jumpHangAccelMult;
    //Multiplicateur maximum de vitesse quand le joueur est a l'apex de son saut
    public float jumpHangMaxSpeedMult;
    //Timer qui empeche au joueur de sauter plusieurs en une touche au clavier
    public float pressedJumpTime;
    //Buffer durant lequel le joueur peut appuyer sur le bouton saut pour sauter meme s'il n'a pas encore toucher le sol
    public float jumpBuffer = 0.1f;
    //Hauteur du saut
    public float jumpHeight;
    //Temps que le joueur prend pour atteindre l'apex de son saut
    public float jumpTimeToApex;
    //Check pour le double jump
    public bool jumpedOnce;
    public bool jumpedTwiced;

    [Header("Fly Variables")]
    bool isFlying;
    [SerializeField] float flyForce;

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
    LayerMask groundLayer;
    //Taille du raycast de collision avec le sol
    public float groundRaycastLength;
    //Check si le joueur est au sol
    public bool isGrounded;

    [Header("Corner Collision Variables")]
    //Layer pour corriger les collisions avec les coins
    public LayerMask cornerCorrectLayer;
    //Taille des raycasts de collisions avec les coins
    public float topRaycastLength;
    [SerializeField]private Vector3 edgeRaycastOffset;
    [SerializeField] private Vector3 innerRaycastOffset;
    [SerializeField] private bool canCornerCorrect;

    [Header("Wall Collision Variables")]
    //Taille des boite invisibles qui check les collisions avec les murs pour les walljump
    public Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    //Timers
    [SerializeField] private float lastOnWallTime;
    public float lastOnWallRightTime;
    public float lastOnWallLeftTime;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D friction;
    bool hasWallJumped;

    [Header("Wall Jump Variables")]
    //Force du saut
    [SerializeField]private Vector2 wallJumpForce;
    //A quel point le joueur controlle sont personnage lors d'un wall jump
    [Range(0f, 1f)] public float wallJumpRunLerp;
    //Pendant combien de temps est-ce que le joueur perd controlle de son personnage lors du walljump
    [Range(0f, 1.5f)] public float wallJumpTime;
    //Check & Timers
    private bool isWallJumping;
    private float wallJumpStartTime;
    private int lastWallJumpDir;

    [Header("Slide Variables")]
    [SerializeField] private float slideSpeed;
    //Acceleration quand le joueur slide
    [SerializeField] private float slideAccel;
    //Check si le joueur est en train de slide
    private bool isSliding;
    //Vitesse de slide

    [Header("Vine Variables")]
    private List<Collider2D> vineParts = new List<Collider2D>();
    private Collider2D nearestVine = null;
    private float nearestVineDistance = 100f;
    public bool isVineJumping;
    private float vineTimer;
    [SerializeField] [Range(0f, 1f)] private float vineLerp;
    [SerializeField] [Range(0f, 1f)] private float vineLerpTime;
    //[SerializeField] private float launchForce;
    [SerializeField] private Vector2 launchForce;
    private float vineJumpStartTime;

    private PlayerPermanent player;
    [SerializeField] private float runStaminaCost;
    [SerializeField] private float jumpStaminaCost;
    [SerializeField] private float climbStaminaCost;

    private void OnEnable()
    {
        vineTimer = 0.35f;
        isFacingRight = GetComponent<PlayerPermanent>().isFacingRight;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerPermanent>();

        //On set up quelques variables pour le deplacement
        rb = gameObject.GetComponent<Rigidbody2D>();
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrength / Physics2D.gravity.y;
        accelForce = (50 * accelTime) / maxSpeed;
        deccelForce = (50 * deccelTime) / maxSpeed;
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
        //On set up la gravite de notre personnage
        SetGravityScale(gravityScale);
        groundLayer = LayerMask.GetMask("Ground", "Creature");
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

        if (player.CanMove())
        {
            //Courir
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (isGrounded)
                {
                    if (!player.staminaDepleted)
                    {
                        maxSpeed = runSpeed;
                        player.ChangeStamina(-runStaminaCost);
                    }
                    else
                        maxSpeed = walkSpeed;
                }
            }
            else
            {
                maxSpeed = walkSpeed;
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnJumpInput();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                OnJumpUpInput();
            }
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
        if (isVineJumping && Time.time - vineJumpStartTime > vineLerpTime)
            isVineJumping = false;

        if (isGrounded && !isJumping && !isWallJumping)
        {
            isJumpCut = false;
            isJumpFalling = false;
            jumpedOnce = false;
            jumpedTwiced = false;
            hasWallJumped = false;
            /*
            if (GetComponent<PlayerPermanent>().hasDoubleJump)
                jumpedOnce = false;
            else
                jumpedOnce = true;
            */
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

        if (!isFlying)
        {
            //Gravity
            if (isSliding)
            {
                SetGravityScale(0f);
            }
            else if (rb.velocity.y < 0 && GetInput().y < 0)
            {
                SetGravityScale(gravityScale * fastFallGravMult);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFastFallSpeed));
            }
            else if (isJumpCut)
            {
                SetGravityScale(gravityScale * jumpCutGravMult);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
            }
            else if ((isJumping || isWallJumping || isJumpFalling || isVineJumping) && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
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

        //Empeche de se reaccrocher a une vigne quand on se detache
        if (vineTimer > 0)
            vineTimer -= Time.deltaTime;

        //Si il y a au moins une vigne a portee
        if (vineParts.Count >= 1)
        {
            //Si on appui sur la touche W
            if ((Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W)) && vineTimer <= 0)
            {
                //Pour chaque vigne a portee, on check celle qui est la plus proche du corps
                foreach (var col in vineParts)
                {
                    float distance = Vector2.Distance(transform.position, col.gameObject.transform.position);
                    if (distance < nearestVineDistance)
                    {
                        nearestVineDistance = distance;
                        nearestVine = col;
                    }
                }
                //On active le script de vigne et on s'attache a la vigne
                GetComponent<VinePlayerController>().enabled = true;
                GetComponent<VinePlayerController>().Attach(nearestVine, rb.velocity);

                //Desactive le rigidbody
                rb.simulated = false;

                //Reinitialise la vigne la plus proche
                nearestVine = null;
                nearestVineDistance = 100;

                //Assure que la gravite est normale quand on sort de la vigne
                isJumpCut = false;
                isJumpFalling = false;
                isWallJumping = false;
                isVineJumping = false;

                //Desactive le controller au sol
                GetComponent<GroundPlayerController>().enabled = false;
            }
        }

        if (isFlying)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.W))
            {
                rb.AddForce(flyForce * Vector2.up);
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.A))
            {
                rb.AddForce(flyForce * Vector2.left);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S))
            {
                rb.AddForce(flyForce * Vector2.down);
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D))
            {
                rb.AddForce(flyForce * Vector2.right);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                isFlying = false;
                SetGravityScale(gravityScale);
            }
        }
    }

    private void FixedUpdate()
    {
        CheckCollision();

        if (player.CanMove())
        {
            if (isWallJumping)
                MoveCharacter(wallJumpRunLerp);

            else if (isVineJumping)
                MoveCharacter(vineLerp);

            else
                MoveCharacter(1);
        }

        if (canCornerCorrect)
            CornerCorrect();

        if (isSliding)
            Slide();
    }

    //Inputs Functions
    public void OnJumpInput()
    {
        pressedJumpTime = jumpBuffer;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut() || CanVineJumpCut())
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

    public void Jump()
    {
        if (jumpedOnce)
        {
            jumpedTwiced = true;
            if (player.hasFlyBackpack)
                Fly();
        }
        else
        {
            if (!player.staminaDepleted)
            {
                player.ChangeStamina(-jumpStaminaCost);
                jumpedOnce = true;
                pressedJumpTime = 0;
                float force = jumpForce;
                if (rb.velocity.y < 0)
                {
                    force -= rb.velocity.y;
                }
                rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            }
        }
    }

    void Fly()
    {
        SetGravityScale(0);
        isFlying = true;
    }

    private void WallJump(int dir)
    {
        if (!player.staminaDepleted)
        {
            player.ChangeStamina(-jumpStaminaCost);
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
            jumpedOnce = false;
            //hasWallJumped = true;
        }
    }

    private void Slide()
    {
        //We remove the remaining upwards Impulse to prevent upwards sliding
        /*
        if (rb.velocity.y > 0)
        {
            rb.AddForce(-rb.velocity.y * Vector2.up, ForceMode2D.Impulse);
        }
        */
        float speedDif = slideSpeed - rb.velocity.y;
        float movement = speedDif * slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        if (!player.staminaDepleted)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(Mathf.Abs(slideSpeed) * Vector2.up);
                if (rb.velocity.magnitude > slideSpeed)
                    rb.velocity = Vector3.ClampMagnitude(rb.velocity, Mathf.Abs(slideSpeed));
                player.ChangeStamina(-climbStaminaCost);
            }
            else
                rb.AddForce(movement * Vector2.up);
        }
        else
            rb.AddForce(movement * Vector2.up);
    }

    public void VineJump()
    {
        isVineJumping = true;
        vineJumpStartTime = Time.time;

        float direction = isFacingRight ? 1 : -1;
        Vector2 force = new Vector2(launchForce.x, launchForce.y);
        force.x *= direction; //Applique la force dans la direction que la personnage regarde

        //Reset la velocite du personnage avant de le faire sauter
        //rb.velocity = new Vector2(0, 0);

        //Annule la velocite en X et Y pour que le saut soit toujours de la meme longueur et hauteur
        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0) 
            force.y -= rb.velocity.y;

        rb.AddForce(force, ForceMode2D.Impulse);
        if (hasWallJumped)
            hasWallJumped = false;
    }

    //Checks Functions
    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), Vector2.down, groundRaycastLength, groundLayer);
        Vector2 frontWallCheckPoint = new Vector2(player.gameObject.transform.position.x + 0.306f, player.gameObject.transform.position.y - 0.4f);
        Vector2 backWallCheckPoint = new Vector2(player.gameObject.transform.position.x - 0.42f, player.gameObject.transform.position.y - 0.36f);

        //Right Wall Check
        if (Physics2D.OverlapBox(frontWallCheckPoint, wallCheckSize, 0, groundLayer))// && isFacingRight) || (Physics2D.OverlapBox(backWallCheckPoint, wallCheckSize, 0, groundLayer) && !isFacingRight)) && !isWallJumping)
            lastOnWallRightTime = coyoteTime;

        //Left Wall Check
        if (Physics2D.OverlapBox(backWallCheckPoint, wallCheckSize, 0, groundLayer))// || (Physics2D.OverlapBox(backWallCheckPoint, wallCheckSize, 0, groundLayer) && isFacingRight)) && !isWallJumping)
            lastOnWallLeftTime = coyoteTime;

        if (lastOnWallTime > 0)
        {
            if (GetComponent<CapsuleCollider2D>().sharedMaterial == friction)
            {
                GetComponent<CapsuleCollider2D>().sharedMaterial = noFriction;
            }
        }
        else
            GetComponent<CapsuleCollider2D>().sharedMaterial = friction;

        //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
        lastOnWallTime = Mathf.Max(lastOnWallLeftTime, lastOnWallRightTime);

        canCornerCorrect = Physics2D.Raycast(transform.position + edgeRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) && 
                            !Physics2D.Raycast(transform.position + innerRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) ||
                           Physics2D.Raycast(transform.position - edgeRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) && 
                           !Physics2D.Raycast(transform.position - innerRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer);
    }

    private void Turn()
    {
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
        return !jumpedTwiced && !jumpedTwiced;
    }

    private bool CanWallJump()
    {
        return pressedJumpTime > 0 && !hasWallJumped && lastOnWallTime > 0 && !isGrounded && (!isWallJumping || (lastOnWallRightTime > 0 && lastWallJumpDir == 1) || (lastOnWallLeftTime > 0 && lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return isJumping && rb.velocity.y > 0;
    }

    private bool CanVineJumpCut()
    {
        return isVineJumping && rb.velocity.y > 0;
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
        if (collision.gameObject.tag == "Water" && !player.waterPlayerController.enabled)
        {
            Debug.Log("Entered water");
            player.waterPlayerController.enabled = true;
            player.groundPlayerController.enabled = false;

            //SetGravityScale(0.1f);
            //rb.drag = 1f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!vineParts.Contains(collision) && collision.gameObject.tag == "Vine")
        {
            vineParts.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (vineParts.Contains(collision) && collision.gameObject.tag == "Vine")
            vineParts.Remove(collision);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z) + Vector3.down * groundRaycastLength);

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
