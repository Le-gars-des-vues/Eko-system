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
    [SerializeField] private float normalAnimSpeed;
    [SerializeField] private float fastAnimSpeed;

    //Cible pour les bras
    [SerializeField] private Transform armTarget;

    //Courbe d'animation pour la position en Y des bras
    [SerializeField] private AnimationCurve yCurve;
    private float curveSpeed = 1f;

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
    private float facingDirection;

    public bool somethingClose = false;
    [SerializeField] private Vector2 pickupInitialPos;
    [SerializeField] private float armMovementRadius;
    [SerializeField] private float armMovementTime;
    [SerializeField] private float armMovementCooldown;
    [SerializeField] private Vector2 armWeaponOffsets;

    [HideInInspector] [SerializeField] private Vector2 armUpOffsets;
    [HideInInspector] [SerializeField] private Vector2 jumpUpOffsets;
    [HideInInspector] [SerializeField] private Vector2 jumpDownOffsets;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(player.transform.position.x + armXOffset, player.transform.position.y + armYOffset);
        speed = normalAnimSpeed;
    }

    private void OnEnable()
    {
        facingDirection = player.GetComponent<PlayerPermanent>().isFacingRight ? 1 : -1;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxis("Mouse X") + " / " + Input.GetAxis("Mouse Y"));
        facingDirection = player.GetComponent<PlayerPermanent>().isFacingRight ? 1 : -1;

        if (player.GetComponent<GroundPlayerController>().enabled)
        {
            if (player.GetComponent<PlayerPermanent>().objectInRightHand == null || (player.GetComponent<PlayerPermanent>().objectInRightHand != null && (player.GetComponent<PlayerPermanent>().objectInRightHand.tag == "Usable" || player.GetComponent<PlayerPermanent>().objectInRightHand.tag == "Throwable")))
            {
                if (player.GetComponent<PlayerPermanent>().objectInRightHand != null && gameObject.name == "RightArmSolver_Target" && player.GetComponent<PlayerPermanent>().objectInRightHand.tag == "Throwable" && Input.GetMouseButton(0))
                {
                    if (!player.GetComponent<PlayerPermanent>().inventoryOpen)
                    {
                        Vector2 throwPos = Vector2.Lerp(new Vector2(player.transform.position.x + (jumpDownOffsets.x * facingDirection), player.transform.position.y + jumpDownOffsets.y),
                                    new Vector2(player.transform.position.x + (armUpOffsets.x * facingDirection), player.transform.position.y + armUpOffsets.y),
                                    player.GetComponent<PlayerPermanent>().objectInRightHand.GetComponent<ThrowableObject>().timer / player.GetComponent<PlayerPermanent>().objectInRightHand.GetComponent<ThrowableObject>().timeToMaxThrow);

                        armTarget.position = throwPos;
                        transform.position = Vector2.MoveTowards(transform.position, armTarget.position, speed * Time.deltaTime);
                    }
                }
                else if (somethingClose)
                {

                }
                else if (!player.GetComponent<GroundPlayerController>().isGrounded)
                {
                    if (player.GetComponent<Rigidbody2D>().velocity.y > 0f)
                    {
                        armTarget.position = new Vector2(player.transform.position.x + (jumpUpOffsets.x * facingDirection), player.transform.position.y + jumpUpOffsets.y);
                        transform.position = Vector2.MoveTowards(transform.position, armTarget.position, normalAnimSpeed * Time.deltaTime);
                    }
                    else
                    {
                        armTarget.position = new Vector2(player.transform.position.x + (jumpDownOffsets.x * facingDirection), player.transform.position.y + jumpDownOffsets.y);
                        transform.position = Vector2.MoveTowards(transform.position, armTarget.position, normalAnimSpeed * Time.deltaTime);
                    }
                }
                else
                {
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
                        if (Input.GetKey(KeyCode.LeftShift) && !player.GetComponent<PlayerPermanent>().staminaDepleted)
                        {
                            speed = fastAnimSpeed;
                            curveSpeed = 2f;
                        }
                        else
                        {
                            speed = normalAnimSpeed;
                            curveSpeed = 1f;
                        }

                        //Mesure la distance entre la cible et notre position
                        targetDistance = Mathf.Abs(Vector2.Distance(transform.position, armTarget.position));
                        stepTimer += Time.deltaTime * curveSpeed;

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
                        //Reset la position initiale
                        transform.position = Vector2.Lerp(transform.position, new Vector2(player.transform.position.x + armXOffset * facingDirection, player.transform.position.y + armYOffset), speed * Time.deltaTime);

                        //Un des deux bras va vers l'arriere en premier
                        if (gameObject.name == "RightArmSolver_Target")
                            goingForward = true;
                        else
                            goingForward = false;
                    }
                }
            }
            else if (player.GetComponent<PlayerPermanent>().objectInRightHand != null && gameObject.name == "RightArmSolver_Target")
            {
                if (player.GetComponent<PlayerPermanent>().objectInRightHand.tag == "Spear")
                {
                    if (!player.GetComponent<PlayerPermanent>().inventoryOpen)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            Vector2 throwPos = Vector2.Lerp(new Vector2(player.transform.position.x + (jumpDownOffsets.x * facingDirection), player.transform.position.y + jumpDownOffsets.y),
                                new Vector2(player.transform.position.x + (armUpOffsets.x * facingDirection), player.transform.position.y + armUpOffsets.y),
                                player.GetComponent<PlayerPermanent>().objectInRightHand.GetComponent<ThrowableObject>().timer / player.GetComponent<PlayerPermanent>().objectInRightHand.GetComponent<ThrowableObject>().timeToMaxThrow);

                            armTarget.position = throwPos;
                            transform.position = Vector2.MoveTowards(transform.position, armTarget.position, speed * Time.deltaTime);
                        }
                        else
                        {
                            //Position initiale de la main sur la lance
                            pickupInitialPos = new Vector2(player.transform.position.x + (armWeaponOffsets.x * facingDirection), player.transform.position.y + armWeaponOffsets.y);
                            //Position de la souris et direction du mouvement de la souris
                            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            Vector2 mouseDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                            //Check pour voir si la souris va dans la meme direction que celle qui fait face au joueur
                            float isPointingRight = mouseDirection.x > 0 ? 1 : -1;
                            bool lookingUp = mousePos.y - transform.position.y >= 0 ? true : false;
                            bool isPointingUp = mouseDirection.y > 0 ? true : false;

                            //Si on bouge la souris
                            if (mouseDirection.magnitude != 0)
                            {
                                armMovementTime = Time.time;

                                //Si la souris de ne va pas dans la meme direction que le joueur regarde
                                if ((isPointingRight != facingDirection && lookingUp != isPointingUp) && Vector2.Distance(player.transform.position, mousePos) < 25f)
                                {
                                    //La main recule
                                    Vector2 offset = pickupInitialPos - mousePos;
                                    armTarget.position = Vector2.ClampMagnitude(offset, armMovementRadius);
                                    transform.position = Vector2.MoveTowards(transform.position, (Vector3)pickupInitialPos + armTarget.position, speed * Time.deltaTime);
                                }
                                //Sinon la main avance
                                else
                                {
                                    Vector2 offset = mousePos - pickupInitialPos;
                                    armTarget.position = Vector2.ClampMagnitude(offset, armMovementRadius);
                                    transform.position = Vector2.MoveTowards(transform.position, (Vector3)pickupInitialPos + armTarget.position, speed * Mathf.Max(Mathf.Abs(Input.GetAxis("Mouse X")), Mathf.Abs(Input.GetAxis("Mouse Y"))) * Time.deltaTime);
                                }
                            }

                            //Si cela fait un moment que la souris n'a pas bouge, la main revient a sa position initiale
                            if (Time.time - armMovementTime > armMovementCooldown)
                            {
                                armTarget.position = pickupInitialPos;
                                transform.position = Vector2.MoveTowards(transform.position, armTarget.position, speed * Time.deltaTime);
                            }
                        }
                    }
                }
                else if (player.GetComponent<PlayerPermanent>().objectInRightHand.tag == "MultiTool")
                {
                    pickupInitialPos = new Vector2(player.transform.position.x + (-0.4f * facingDirection), player.transform.position.y);
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 offset = mousePos - pickupInitialPos;
                    armTarget.position = Vector2.ClampMagnitude(offset, armMovementRadius);
                    transform.position = Vector2.MoveTowards(transform.position, (Vector3)pickupInitialPos + armTarget.position, speed * Time.deltaTime);
                }
            }
            else if (player.GetComponent<PlayerPermanent>().objectInRightHand != null && gameObject.name == "LeftArmSolver_Target")
            {
                if (player.GetComponent<PlayerPermanent>().objectInRightHand.tag == "Spear")
                {
                    if (!Input.GetMouseButton(0))
                    {
                        armTarget.position = player.GetComponent<PlayerPermanent>().objectInRightHand.transform.Find("LeftHandPos").transform.position;
                        transform.position = Vector2.MoveTowards(transform.position, armTarget.position, speed * 4 * Time.deltaTime);
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
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + armWeaponOffsets.x, player.transform.position.y + armWeaponOffsets.y), 0.05f);
        Gizmos.DrawSphere(new Vector2(player.transform.position.x + armUpOffsets.x, player.transform.position.y + armUpOffsets.y), 0.05f);
    }
}
