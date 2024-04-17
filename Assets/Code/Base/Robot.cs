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
    [SerializeField] Transform robotPos;
    bool isWalking;
    Rigidbody2D rb;
    public bool isMoving;
    bool isFacingRight = true;

    [Header("Base Variables")]
    [SerializeField] GameObject respawnPoint;
    [SerializeField] GameObject sellingScreen;
    [SerializeField] GameObject craftingBench;
    [SerializeField] float offset;

    bool hasShowedRespawn;
    bool hasShowedMarket;
    bool hasShowedCrafting;
    bool hasShowedBuilding;

    [SerializeField] GameObject ressourceToGive;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player.GetComponent<PlayerPermanent>().spawnAtBase)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence, speaker, true);
            DialogueManager.onDialogueEnd = GoToCrafting;
            DialogueManager.onDialogueEnd += GiveItem;
            //DialogueManager.onDialogueEnd = GoToRespawn;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            if (Mathf.Abs(target.position.x - robotPos.position.x) > movementDistanceThreshold)
            {
                anim.SetBool("isWalking", true);
                Vector2 direction = target.position - robotPos.position;
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
                RobotTutorial();
            }
        }
        else
        {
            if ((!isFacingRight && player.transform.position.x - robotPos.position.x > 0) || (isFacingRight && player.transform.position.x - robotPos.position.x < 0))
                Turn();
        }
    }

    void RobotTutorial()
    {
        if (!hasShowedCrafting)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence4, speaker);
            DialogueManager.onDialogueEnd = GoToSellingScreen;
            hasShowedCrafting = true;
        }
        else if (!Tutorial.instance.firstTimeOutside && !Tutorial.instance.readyToGoOut)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence5, speaker, true, true, "craftedFirstItem");
            DialogueManager.onDialogueEnd += ReadyToGoOut;
        }
        else if (!hasShowedRespawn)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence2, speaker);
            DialogueManager.onDialogueEnd = GoToSellingScreen;
            hasShowedRespawn = true;
        }
        else if (!hasShowedMarket)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence3, speaker);
            DialogueManager.onDialogueEnd = GoToCrafting;
            hasShowedMarket = true;
        }
        else if (!hasShowedBuilding)
        {
            DialogueManager.instance.StartDialogue(speaker.dialogueSequence5, speaker);
            hasShowedBuilding = true;
        }
    }

    void ReadyToGoOut()
    {
        Tutorial.instance.readyToGoOut = true;
    }

    void GoRight()
    {
        if (!isFacingRight)
            Turn();
        transform.Translate(walkSpeed * Vector2.right * Time.deltaTime);
    }

    void GoLeft()
    {
        if (isFacingRight)
            Turn();
        transform.Translate(walkSpeed * Vector2.left * Time.deltaTime);
    }

    void Turn()
    {
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        scale = speaker.GetComponent<DialogueSpeaker>().speechBubbleText.gameObject.transform.localScale;
        scale.x *= -1;
        speaker.GetComponent<DialogueSpeaker>().speechBubbleText.gameObject.transform.localScale = scale;

        scale = speaker.GetComponent<DialogueSpeaker>().speechBubbleTextB.gameObject.transform.localScale;
        scale.x *= -1;
        speaker.GetComponent<DialogueSpeaker>().speechBubbleTextB.gameObject.transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    void GoToRespawn()
    {
        isMoving = true;
        target.position = new Vector2(respawnPoint.transform.position.x + offset, robotPos.position.y);
    }

    void TeleportToTrainingRoom()
    {
        StartCoroutine(player.GetComponent<PlayerPermanent>().Dissolve(2f, true, GameObject.Find("Base").GetComponent<Base>().trainingRoom.position));
    }

    void GoToSellingScreen()
    {
        isMoving = true;
        target.position = new Vector2(sellingScreen.transform.position.x + offset + 2.3f, robotPos.position.y);
    }

    void GoToCrafting()
    {
        isMoving = true;
        target.position = new Vector2(craftingBench.transform.position.x + offset + 2f, robotPos.position.y);
        PromptManager.instance.CreateNewPrompt(new Prompt("Play the movement tutorial?", false, "Yes", "No"));
        PromptManager.onButtonClick = TeleportToTrainingRoom;
    }

    void GiveItem()
    {
        Instantiate(ressourceToGive, transform.position, transform.rotation);
    }
}
