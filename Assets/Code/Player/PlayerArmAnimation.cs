using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmAnimation : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerArmAnimation otherArm;

    //Position initiale des bras
    [SerializeField] private float armXOffset;
    [SerializeField] private float armYOffset;

    //Vitesse de mouvement des bras
    [SerializeField] private float speed;
    [SerializeField] private float jumpAnimSpeed;

    //Cible pour les bras
    [SerializeField] private Transform armTarget;

    //Courbe d'animation pour la position en Y des bras
    [SerializeField] private AnimationCurve yCurve;

    //Offset pour les points ou la cible va s'arreter
    [HideInInspector] public float fXOffset;
    [HideInInspector] public float fYOffset;
    [HideInInspector] public float bXOffset;
    [HideInInspector] public float bYOffset;

    //Vector de position
    private Vector3 forwardArmPos;
    private Vector3 backwardArmPos;

    public bool goingForward;
    private bool isRunning;
    private bool reachedPos = false;
    private float targetDistance;
    private float stepTimer;

    public bool somethingClose = false;
    [SerializeField] private Vector2 jumpUpOffsets;
    [SerializeField] private Vector2 jumpDownOffsets;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(player.transform.position.x + armXOffset, player.transform.position.y + armYOffset);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<GroundPlayerController>().enabled && !somethingClose)
        {
            float facingDirection = player.GetComponent<PlayerPermanent>().isFacingRight ? 1 : -1;

            //Si le joueur appui sur A et qu'il est grounded
            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && player.GetComponent<GroundPlayerController>().isGrounded))
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
                reachedPos = false;
                stepTimer = 0;
            }
            //Si le joueur est en train de courrir
            if (isRunning)
            {
                //Mesure la distance entre la cible et notre position
                targetDistance = Mathf.Abs(Vector2.Distance(transform.position, armTarget.position));
                stepTimer += Time.deltaTime;

                //Cree la position a l'avant et l'arriere du joueur ou va venir se placer la cible
                forwardArmPos = new Vector2(player.transform.position.x + fXOffset * facingDirection, player.transform.position.y + fYOffset);
                backwardArmPos = new Vector2(player.transform.position.x + bXOffset * facingDirection, player.transform.position.y + bYOffset);

                //Dependement de si le bras doit aller vers l'avant ou vers l'arriere, la position de la cible est assigne a l'un des deux emplacements
                armTarget.position = goingForward ? forwardArmPos : backwardArmPos;

                //Si la distance est assez petite. Le timer previent que le bras revienne vers la meme cible
                if (targetDistance <= 0.01f && stepTimer > 0.2f)
                    reachedPos = true;

                //La position du bras lerp vers celle de la cible
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(armTarget.position.x, armTarget.position.y + yCurve.Evaluate(stepTimer)), speed * Time.deltaTime);

                //Si la position de la cible est atteinte pour les deux bras
                if (reachedPos == true && otherArm.targetDistance < 0.01f)
                {
                    //La cible va a l'autre position (avant ou arriere) car le bras change de direction
                    reachedPos = false;
                    goingForward = !goingForward;
                    stepTimer = 0;
                }
            }
            else
            {
                if (player.GetComponent<GroundPlayerController>().isGrounded)
                {
                    //Reset la position initiale
                    transform.position = Vector2.Lerp(transform.position, new Vector2(player.transform.position.x + armXOffset * facingDirection, player.transform.position.y + armYOffset), speed * Time.deltaTime);

                    //Un des deux bras va vers l'arriere en premier
                    goingForward = true;
                    if (otherArm.goingForward == true)
                        goingForward = false;
                }
                else
                {
                    if (player.GetComponent<Rigidbody2D>().velocity.y > 0.25f)
                    {
                        armTarget.position = Vector2.Lerp(transform.position, new Vector2(player.transform.position.x + (jumpUpOffsets.x * facingDirection), player.transform.position.y + jumpUpOffsets.y), jumpAnimSpeed * Time.deltaTime);
                        transform.position = Vector2.MoveTowards(transform.position, armTarget.position, speed * Time.deltaTime);
                    }
                    else
                    {
                        armTarget.position = Vector2.Lerp(transform.position, new Vector2(player.transform.position.x + (jumpDownOffsets.x * facingDirection), player.transform.position.y + jumpDownOffsets.y), jumpAnimSpeed * Time.deltaTime);
                        transform.position = Vector2.MoveTowards(transform.position, armTarget.position, speed * Time.deltaTime);
                    }
                }
            }
        }
    }

    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Vine")
        {
            somethingClose = true;
            armTarget.position = collision.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, new Vector3(armTarget.position.x, armTarget.position.y, transform.position.z), speed * Time.deltaTime);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Vine")
        {
            somethingClose = false;
        }
    }
    */

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawSphere(new Vector3(player.transform.position.x + armXOffset, player.transform.position.y + armYOffset, transform.position.z), 0.05f);
        //Gizmos.DrawSphere(forwardArmPos, 0.05f);
        //Gizmos.DrawSphere(backwardArmPos, 0.05f);
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + jumpUpOffsets.x, player.transform.position.y + jumpUpOffsets.y), 0.05f);
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + jumpDownOffsets.x, player.transform.position.y + jumpDownOffsets.y), 0.05f);
    }
}
