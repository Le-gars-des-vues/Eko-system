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
    float targetDistance;
    float footMovement;
    float stepTimer;
    public AnimationCurve yCurve;
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

    //Pour regarder la position de l'autre pied
    [SerializeField] private PlayerLegAnimation otherfoot;
    

    // Start is called before the first frame update
    void Start()
    {
        //On fixe la cible initiale au sol
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -Vector2.up, 12f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            currentTarget.position = new Vector2(hit.point.x, hit.point.y);
        }

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
        if (player.GetComponent<GroundPlayerController>().enabled)
        {
            if (player.GetComponent<GroundPlayerController>().isGrounded != isLanding)
            {
                ResetPosition();
            }
            isLanding = player.GetComponent<GroundPlayerController>().isGrounded;

            if (player.GetComponent<GroundPlayerController>().isGrounded)
            {
                //Adapte le code en fonction de si on regarde a gauche ou a droite
                facingDirection = isFacingRight ? 1 : -1;
                isMovingRight = Input.GetAxis("Horizontal") >= 0 ? true : false;

                //Detecte le changement de direction pour reinitialiser la position des pieds;
                CheckDirectionToFace(player.GetComponent<PlayerPermanent>().isFacingRight);

                //Empeche l'animation des jambes s'il y a un mur devant
                RaycastHit2D wallCheck = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y), Vector2.right * facingDirection, 0.5f, LayerMask.GetMask("Ground"));
                if (wallCheck.collider != null)
                    thereIsAWall = true;
                else
                    thereIsAWall = false;

                if (isMovingRight != isFacingRight && !isWalkingBack && Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
                    ChangeDirection();
                else if (isMovingRight == isFacingRight && isWalkingBack)
                    ChangeDirection();

                //Calcule la distance entre la cible du pied et sa position actuelle
                targetDistance = Vector2.Distance(currentTarget.position, desiredTarget.position);

                //Si la distance depasse le seuile et qu'il n'y a pas de mur devant, la position du pied devient celle de la cible
                if (targetDistance > threshold && otherfoot.targetDistance > 1.2f && !thereIsAWall)
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

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    isRunning = true;
                }
                else
                {
                    isRunning = false;
                    currentTarget.position = transform.position;
                }

                if (!isRunning)
                {
                    RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x + legOffset * facingDirection, player.transform.position.y), -Vector2.up, 2f, LayerMask.GetMask("Ground"));
                    if (hit.collider != null)
                        currentTarget.position = Vector2.Lerp(currentTarget.position, new Vector2(hit.point.x, hit.point.y), speed * Time.deltaTime);
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

    void ResetPosition()
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
