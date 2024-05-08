using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinePlayerController : MonoBehaviour
{
    public bool isAttached;
    [SerializeField] private float pushForce;
    [SerializeField] private float slideSpeed;
    [SerializeField] private GameObject attachedVine;
    [SerializeField] private float vineKnockback;
    [SerializeField] private float vineOffset;

    private bool pushingRight;
    private bool pushingLeft;
    private bool slidingUp;
    private bool slidingDown;
    private float facingDirection;

    PlayerPermanent player;

    bool climbSoundPlaying;

    private void OnEnable()
    {
        player = gameObject.GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        facingDirection = GetComponent<PlayerPermanent>().isFacingRight ? -1 : 1;

        if (isAttached)
        {
            if (player.CanMove())
            {
                pushingLeft = Input.GetKey(KeyCode.A) ? true : false;
                pushingRight = Input.GetKey(KeyCode.D) ? true : false;

                slidingUp = Input.GetKey(KeyCode.W) ? true : false;
                slidingDown = Input.GetKey(KeyCode.S) ? true : false;

                if (Input.GetKeyDown(KeyCode.Space))
                    Detach();
            }

            //Si la position entre la vigne au dessous ou au dessous devient trop petite, c'est la vigne suivante qui devient la vigne a laquelle on s'attache
            if (attachedVine.GetComponent<RopeSegment>().connectedAbove != null)
            {
                float distanceUp = Vector2.Distance(transform.position, new Vector2(attachedVine.GetComponent<RopeSegment>().connectedAbove.transform.position.x + (vineOffset * facingDirection), attachedVine.GetComponent<RopeSegment>().connectedAbove.transform.position.y));
                if (distanceUp < 0.1f)
                {
                    if (attachedVine.GetComponent<RopeSegment>().connectedAbove.gameObject.name != "Hook")
                    {
                        attachedVine = attachedVine.GetComponent<RopeSegment>().connectedAbove;
                        transform.parent.transform.parent = attachedVine.transform;
                        transform.rotation = attachedVine.transform.rotation;
                    }
                }    
            }
            
            if (attachedVine.GetComponent<RopeSegment>().connectedBelow != null)
            {
                float distanceDown = Vector2.Distance(transform.position, new Vector2(attachedVine.GetComponent<RopeSegment>().connectedBelow.transform.position.x + (vineOffset * facingDirection), attachedVine.GetComponent<RopeSegment>().connectedBelow.transform.position.y));
                if (distanceDown < 0.1f && attachedVine.GetComponent<RopeSegment>().connectedBelow != null)
                {
                    attachedVine = attachedVine.GetComponent<RopeSegment>().connectedBelow;
                    transform.parent.transform.parent = attachedVine.transform;
                    transform.rotation = attachedVine.transform.rotation;
                }
            }
        }
    }

    //Mouvement du joueur dans fixedUpate
    private void FixedUpdate()
    {
        if (isAttached)
        {
            if (player.CanMove())
            {
                if (pushingLeft)
                    attachedVine.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector3(-1, 0, 0) * pushForce);
                if (pushingRight)
                    attachedVine.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector3(1, 0, 0) * pushForce);
                if (slidingUp)
                    Slide(1);
                if (slidingDown)
                    Slide(-1);
            }

            if (!slidingUp && !slidingDown)
            {
                if (climbSoundPlaying)
                {
                    climbSoundPlaying = false;
                    AudioManager.instance.PlaySound(AudioManager.instance.playerClimbStop, gameObject);
                }
                player.isClimbing = false;
                player.isHanging = true;
                transform.position = new Vector2(attachedVine.transform.position.x + (vineOffset * facingDirection), attachedVine.transform.position.y);
            }
        }
    }

    public void Attach(Collider2D col, Vector2 velocity)
    {
        //Attache le joueur a un morceau de la vigne pour qu'il ne s'y reattache pas
        attachedVine = col.gameObject;
        isAttached = true;
        attachedVine.GetComponent<Rigidbody2D>().AddForce(velocity.normalized * vineKnockback, ForceMode2D.Impulse);

        //Met la vigne comme parent pour que le joueur suivent sa position et rotation
        transform.parent.transform.parent = attachedVine.transform;
        transform.rotation = attachedVine.transform.rotation;
    }

    public void Detach()
    {
        //Detache le joueur pour ne plus controller la vigne
        isAttached = false;

        //Reset la position du joueur au point de la vigne de laquelle il s'est detache
        transform.parent.transform.position = new Vector2(attachedVine.transform.position.x + (vineOffset * facingDirection), attachedVine.transform.position.y);
        transform.position = new Vector2(attachedVine.transform.position.x + (vineOffset * facingDirection), attachedVine.transform.position.y);

        //Reactive le rigidbody du joueur
        GetComponent<Rigidbody2D>().simulated = true;

        //Reactive les controls au sol du joueur
        GetComponent<GroundPlayerController>().enabled = true;

        //Deparente le joueur de la vigne et reset son angle
        transform.parent.transform.parent = null;
        transform.parent.transform.eulerAngles = new Vector3(0, 0, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);

        GetComponent<GroundPlayerController>().VineJump();
        //Faut sauter le joueur et desactive le script de vigne
        GetComponent<VinePlayerController>().enabled = false;
        if (climbSoundPlaying)
        {
            climbSoundPlaying = false;
            AudioManager.instance.PlaySound(AudioManager.instance.playerClimbStop, gameObject);
        }
        player.isClimbing = false;
        player.isHanging = false;
    }

    //Slide dans une direction choisi en Lerpant vers la position de la vigne au dessus ou au dessous
    public void Slide(int direction)
    {
        player.isClimbing = true;
        if (!climbSoundPlaying)
        {
            climbSoundPlaying = true;
            AudioManager.instance.PlaySound(AudioManager.instance.playerClimb, gameObject);
        }
        if (direction > 0)
        {
            if (attachedVine.GetComponent<RopeSegment>().connectedAbove != null && attachedVine.GetComponent<RopeSegment>().connectedAbove.gameObject.name != "Hook")
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(attachedVine.GetComponent<RopeSegment>().connectedAbove.transform.position.x + (vineOffset * facingDirection), attachedVine.GetComponent<RopeSegment>().connectedAbove.transform.position.y), slideSpeed * Time.deltaTime);
        }
        else
        {
            if (attachedVine.GetComponent<RopeSegment>().connectedBelow != null)
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(attachedVine.GetComponent<RopeSegment>().connectedBelow.transform.position.x + (vineOffset * facingDirection), attachedVine.GetComponent<RopeSegment>().connectedBelow.transform.position.y), slideSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (isAttached)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(attachedVine.transform.position, transform.right * -2);
        }
    }
}
