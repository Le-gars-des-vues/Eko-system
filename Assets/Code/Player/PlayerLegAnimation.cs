using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegAnimation : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private Transform desiredTarget;
    [SerializeField] private GameObject player;

    [Header("Animation Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float normalAnimSpeed;
    [SerializeField] private float fastAnimSpeed;
    [SerializeField] private float threshold;
    public float targetDistance;
    float footMovement;
    float stepTimer;
    public AnimationCurve yCurve;
    private float curveSpeed = 1f;
    private bool isRunning = false;
    private bool thereIsAWall;
    [SerializeField] private float legOffset;
    [SerializeField] private Vector2 jumpUpOffsets;
    [SerializeField] private Vector2 jumpMidOffsets;
    [SerializeField] private Vector2 jumpDownOffsets;
    [SerializeField] private float legAirMotion;
    private float facingDirection;
    private bool isFacingRight;
    private bool isMovingRight;
    private bool isWalkingBack = false;
    private bool isLanding = false;

    [Header("UnderWater Variables")]
    [SerializeField] Transform underwaterTarget;
    [SerializeField] Transform underwaterPivot;
    bool goingForward;
    bool reachedPos;
    bool isSwimming;
    [SerializeField] float legMovementRadius;

    //Pour regarder la position de l'autre pied
    [SerializeField] private PlayerLegAnimation otherFoot;

    // Start is called before the first frame update
    void Start()
    {
        //On fixe la cible initiale au sol
        ResetPosition();

        //Commence en regardant vers la droite
        isFacingRight = true;
        speed = normalAnimSpeed;
    }

    private void OnEnable()
    {
        facingDirection = player.GetComponent<PlayerPermanent>().isFacingRight ? 1 : -1;
    }

    // Update is called once per frame
    void Update()
    {
        //Adapte le code en fonction de si on regarde a gauche ou a droite
        facingDirection = isFacingRight ? 1 : -1;
        isMovingRight = Input.GetAxis("Horizontal") >= 0 ? true : false;

        if (isMovingRight != isFacingRight && !isWalkingBack && Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
            ChangeDirection();
        else if (isMovingRight == isFacingRight && isWalkingBack)
            ChangeDirection();


        //Detecte le changement de direction pour reinitialiser la position des pieds;
        CheckDirectionToFace(player.GetComponent<PlayerPermanent>().isFacingRight);

        if (player.GetComponent<GroundPlayerController>().enabled)
        {
            if (player.GetComponent<GroundPlayerController>().isGrounded != isLanding)
            {
                ResetPosition();
            }
            isLanding = player.GetComponent<GroundPlayerController>().isGrounded;

            if (player.GetComponent<GroundPlayerController>().isGrounded)
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    isRunning = true;
                else if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) && isRunning)
                {
                    Debug.Log("Stopped Running");
                    isRunning = false;
                    currentTarget.position = transform.position;
                }

                //Empeche l'animation des jambes s'il y a un mur devant
                RaycastHit2D wallCheck = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y), Vector2.right * facingDirection, 0.5f, LayerMask.GetMask("Ground"));
                if (wallCheck.collider != null)
                    thereIsAWall = true;
                else
                    thereIsAWall = false;

                if (isRunning)
                {
                    //Calcule la distance entre la cible du pied et sa position actuelle
                    targetDistance = Vector2.Distance(currentTarget.position, desiredTarget.position);

                    //Si la distance depasse le seuile et qu'il n'y a pas de mur devant, la position du pied devient celle de la cible
                    if (targetDistance > threshold && otherFoot.targetDistance > 1.2f && !thereIsAWall)
                    {
                        currentTarget.position = desiredTarget.position;
                    }

                    //Distance entre la cible actuelle et la position du pied
                    footMovement = Vector2.Distance(transform.position, currentTarget.position);

                    //Si le pied est en train de bouger
                    if (footMovement > 0.1f)
                    {
                        //On augmente le timer pour la courbe d'animation
                        stepTimer += Time.deltaTime;

                        //On bouge le pied en ajoutant de la hauteur selon la courbe d'animation
                        transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, currentTarget.position.y + yCurve.Evaluate(stepTimer)), speed * Time.deltaTime);
                    }
                    else
                    {
                        //Reset le timer
                        stepTimer = 0;
                        transform.position = currentTarget.position;
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x + legOffset * facingDirection, player.transform.position.y), -Vector2.up, 2f, LayerMask.GetMask("Ground"));
                    if (hit.collider != null)
                    {
                        currentTarget.position = Vector2.Lerp(currentTarget.position, new Vector2(hit.point.x, hit.point.y), speed * Time.deltaTime);
                        transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, currentTarget.position.y), speed * Time.deltaTime);
                    }
                }

                if (Input.GetKey(KeyCode.LeftShift) && !player.GetComponent<PlayerPermanent>().staminaDepleted)
                    speed = fastAnimSpeed;
                else
                    speed = normalAnimSpeed;

            }
            else
            {
                if (player.GetComponent<Rigidbody2D>().velocity.y > -4f)
                {
                    currentTarget.position = new Vector2(player.transform.position.x + (jumpMidOffsets.x * facingDirection), player.transform.position.y + jumpMidOffsets.y);
                    transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, 5 * Time.deltaTime);
                }
                else
                {
                    currentTarget.position = new Vector2(player.transform.position.x + (jumpDownOffsets.x + player.GetComponent<Rigidbody2D>().velocity.x * legAirMotion), player.transform.position.y + jumpDownOffsets.y);
                    transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, 3 * Time.deltaTime);
                }
            }
        }
        else if (player.GetComponent<WaterPlayerController>().enabled)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                isSwimming = true;
            }
            else
            {
                isSwimming = false;
                currentTarget.position = transform.position;
            }

            /*
                 if (Input.GetKey(KeyCode.LeftShift) && !playerScript.staminaDepleted)
                 {
                     speed = fastAnimSpeed;
                     curveSpeed = 2f;
                 }
                 else
                 {
                     speed = normalAnimSpeed;
                     curveSpeed = 1f;
                 }
                 */

            //Un des deux bras va vers l'arriere en premier
            if (gameObject.name == "RightLegSolver_Target")
            {
                if (goingForward == otherFoot.goingForward)
                {
                    goingForward = !goingForward;
                }
            }


            Vector2 legOrigin = new Vector2(player.transform.position.x, player.transform.position.y - 0.7f);
            Vector2 movement = new Vector2(legOrigin.x - Input.GetAxis("Horizontal"), legOrigin.y - Input.GetAxis("Vertical")); ;//new Vector2(player.transform.position.x + Input.GetAxis("Horizontal"), player.transform.position.y + Input.GetAxis("Vertical")) - legOrigin;
            underwaterPivot.position = isSwimming ? movement : movement + Vector2.down;
            underwaterPivot.transform.up = (movement - legOrigin).normalized;

            speed = 2;

            //Mesure la distance entre la cible et notre position
            targetDistance = Mathf.Abs(Vector2.Distance(transform.position, underwaterTarget.position));
            //stepTimer += Time.deltaTime * curveSpeed;
            stepTimer += Time.deltaTime;

            //Dependement de si le bras doit aller vers l'avant ou vers l'arriere, la position de la cible est assigne a l'un des deux emplacements
            underwaterTarget.position = goingForward ? underwaterPivot.Find("Point2").position : underwaterPivot.Find("Point1").position;

            //La position du bras lerp vers celle de la cible
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(underwaterTarget.position.x, underwaterTarget.position.y) /* + yCurve.Evaluate(stepTimer)*/, speed * Time.deltaTime);

            //Si la distance est assez petite. Le timer previent que le bras revienne vers la meme cible
            if (targetDistance <= 0.01f && stepTimer > 0.2f)
                reachedPos = true;

            //Si la position de la cible est atteinte pour les deux bras ou que l'autre main ne peut pas bouger
            if (reachedPos == true && otherFoot.targetDistance < 0.01f)
            {
                //La cible va a l'autre position (avant ou arriere) car le bras change de direction
                reachedPos = false;
                goingForward = !goingForward;
                stepTimer = 0;
            }
        }
    }

    
    public void CheckDirectionToFace(bool isLookingRight)
    {
        if (isLookingRight != isFacingRight)
        {
            Turn();
        }
    }

    void Turn()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x + legOffset * facingDirection, player.transform.position.y), -Vector2.up, 12f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            currentTarget.position = hit.point;
            transform.position = hit.point;
        }
        isFacingRight = !isFacingRight;
    }

    void ChangeDirection()
    {
        isWalkingBack = !isWalkingBack;
        Vector2 position = desiredTarget.localPosition;
        position.x *= -1;
        desiredTarget.localPosition = position;
    }

    public void ResetPosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 2f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            currentTarget.position = hit.point;
            transform.position = hit.point;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + jumpUpOffsets.x, player.transform.position.y + jumpUpOffsets.y), 0.05f);
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + jumpDownOffsets.x, player.transform.position.y + jumpDownOffsets.y), 0.05f);
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + jumpMidOffsets.x, player.transform.position.y + jumpMidOffsets.y), 0.05f);
    }
    
}
