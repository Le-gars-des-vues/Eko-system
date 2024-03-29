using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Robot : MonoBehaviour
{
    [SerializeField] DialogueSpeaker speaker;
    Animator anim;
    GameObject player;

    [SerializeField] float walkSpeed;
    [SerializeField] float movementDistanceThreshold;
    [SerializeField] Transform target;
    bool isWalking;
    Rigidbody2D rb;
    public bool isMoving;
    bool isFacingRight = true;

    [Header("Base Variables")]
    [SerializeField] GameObject respawnPoint;
    [SerializeField] GameObject sellingScreen;
    [SerializeField] GameObject craftingBench;

    bool hasShowedRespawn;
    bool hasShowedMarket;
    bool hasShowedCrafting;
    bool hasShowedBuilding;

    [SerializeField] GameObject ressourceToGive;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player.GetComponent<PlayerPermanent>().spawnAtBase)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence, speaker, true);
            DialogueManager.onDialogueEnd = GoToRespawn;
        }
        Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), player.GetComponent<CapsuleCollider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if (Vector2.Distance(target.position, transform.position) > movementDistanceThreshold)
            {
                anim.SetBool("isWalking", true);
                Vector2 direction = target.position - transform.position;
                direction.y = 0;
                if (direction.x > 0)
                    GoRight();
                else
                    GoLeft();
            }
            else
            {
                isMoving = false;
                anim.SetBool("isWalking", false);
                if (!isFacingRight)
                    Turn();
                RobotTutorial();
            }
        }
    }

    void RobotTutorial()
    {
        if (!hasShowedRespawn)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence2, speaker, true);
            DialogueManager.onDialogueEnd = GoToSellingScreen;
            hasShowedRespawn = true;
        }
        else if (!hasShowedMarket)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence3, speaker, true);
            DialogueManager.onDialogueEnd = GoToCrafting;
            hasShowedMarket = true;
        }
        else if (!hasShowedCrafting)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence4, speaker, true);
            DialogueManager.onDialogueEnd += GoToSellingScreen;
            DialogueManager.onDialogueEnd += GiveItem;
            hasShowedCrafting = true;
        }
        else if (!hasShowedBuilding)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence5, speaker, true);
            hasShowedBuilding = true;
        }
    }

    void GoRight()
    {
        if (!isFacingRight)
            Turn();
        rb.AddForce(walkSpeed * Vector2.right);
    }

    void GoLeft()
    {
        if (isFacingRight)
            Turn();
        rb.AddForce(walkSpeed * Vector2.left);
    }

    void Turn()
    {
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight;
    }

    void GoToRespawn()
    {
        isMoving = true;
        target.position = respawnPoint.transform.position + new Vector3(3, 0, 0);
    }

    void GoToSellingScreen()
    {
        isMoving = true;
        target.position = sellingScreen.transform.position + new Vector3(3, 0, 0);
    }

    void GoToCrafting()
    {
        isMoving = true;
        target.position = craftingBench.transform.position + new Vector3(3, 0, 0);
    }

    void GiveItem()
    {
        Instantiate(ressourceToGive, transform.position, transform.rotation);
    }
}
