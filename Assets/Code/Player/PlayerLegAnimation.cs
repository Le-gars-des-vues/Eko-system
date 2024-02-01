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
    public float speed;
    public float threshold;
    float targetDistance;
    float footMovement;
    float stepTimer;
    public AnimationCurve yCurve;
    private bool isRunning = false;
    private bool thereIsAWall;
    [SerializeField] private float legOffset;
    [SerializeField]private Vector2 jumpUpOffsets;
    [SerializeField] private Vector2 jumpDownOffsets;
    private float facingDirection;
    private bool isFacingRight;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<GroundPlayerController>().enabled)
        {
            if (player.GetComponent<GroundPlayerController>().isGrounded)
            {
                //Adapte le code en fonction de si on regarde a gauche ou a droite
                facingDirection = isFacingRight ? 1 : -1;

                //Detecte le changement de direction pour reinitialiser la position des pieds;
                if (Input.GetAxis("Horizontal") != 0)
                    CheckDirectionToFace(Input.GetAxis("Horizontal") > 0);

                //Empeche l'animation des jambes s'il y a un mur devant
                RaycastHit2D wallCheck = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y), Vector2.right * facingDirection, 0.5f, LayerMask.GetMask("Ground"));
                if (wallCheck.collider != null)
                    thereIsAWall = true;
                else
                    thereIsAWall = false;

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
            }
            else
            {
                if (player.GetComponent<Rigidbody2D>().velocity.y > 0)
                {
                    currentTarget.position = Vector2.Lerp(transform.position, new Vector2(player.transform.position.x + jumpUpOffsets.x, player.transform.position.y + jumpUpOffsets.y), speed * Time.deltaTime);
                    transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
                }
                else
                {
                    currentTarget.position = Vector2.Lerp(transform.position, new Vector2(player.transform.position.x + jumpDownOffsets.x, player.transform.position.y + jumpDownOffsets.y), speed * Time.deltaTime);
                    transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
                }
            }
        }
    }

    
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
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

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + jumpUpOffsets.x, player.transform.position.y + jumpUpOffsets.y), 0.05f);
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + jumpDownOffsets.x, player.transform.position.y + jumpDownOffsets.y), 0.05f);
    }
    
}
