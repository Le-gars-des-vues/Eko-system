using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Robot : MonoBehaviour
{
    Animator anim;
    GameObject player;
    [SerializeField] SpriteRenderer sprite;

    [Header("Dialogue Variables")]
    [SerializeField] DialogueSpeaker baseDialogue;
    [SerializeField] DialogueSpeaker trainingRoomDialogue;

    [Header("Movement Variables")]
    [SerializeField] float walkSpeed;
    [SerializeField] float movementDistanceThreshold;
    [SerializeField] Transform target;
    [SerializeField] Transform robotPos;
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
    [SerializeField] GameObject ressourceToSell;

    [Header("Training Room Variables")]
    bool isInTraining;
    [SerializeField] Material dissolveMaterial;
    [SerializeField] Material holoMaterial;
    [SerializeField] Material ogMaterial;
    bool isTeleporting;
    [SerializeField] List<Transform> teleportPoints = new List<Transform>();
    public int teleportIndex = 0;
    int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        trainingRoomDialogue.enabled = false;
        if (player.GetComponent<PlayerPermanent>().spawnAtBase)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[0].dialogueSequence);
            baseDialogue.onDialogueEnd = GoToTraining;
            //DialogueManager.onDialogueEnd = GoToRespawn;
        }
    }

    private void Update()
    {
        if (!isInTraining)
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
        else
        {
            if ((!isFacingRight && player.transform.position.x - robotPos.position.x > 0.5f) || (isFacingRight && player.transform.position.x - robotPos.position.x < -0.5f))
                Turn();

            //Si le robot n'est pas en train de se teleporter, d'attendre pour parler ou de parler
            if (!isTeleporting)
            {
                RobotTraining();
            }
        }
    }

    void Turn()
    {
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        scale = baseDialogue.speechBubbleText.gameObject.transform.localScale;
        scale.x *= -1;
        baseDialogue.speechBubbleText.gameObject.transform.localScale = scale;

        scale = baseDialogue.speechBubbleTextB.gameObject.transform.localScale;
        scale.x *= -1;
        baseDialogue.speechBubbleTextB.gameObject.transform.localScale = scale;

        scale = trainingRoomDialogue.speechBubbleText.gameObject.transform.localScale;
        scale.x *= -1;
        trainingRoomDialogue.speechBubbleText.gameObject.transform.localScale = scale;

        scale = trainingRoomDialogue.speechBubbleTextB.gameObject.transform.localScale;
        scale.x *= -1;
        trainingRoomDialogue.speechBubbleTextB.gameObject.transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }



    #region Base Tutorial
    void RobotTutorial()
    {
        if (!hasShowedCrafting)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[3].dialogueSequence);
            baseDialogue.onDialogueEnd = GoToSellingScreen;
            baseDialogue.onDialogueEnd += GiveItemToCraft;
            hasShowedCrafting = true;
        }
        else if (!Tutorial.instance.firstTimeOutside && !Tutorial.instance.readyToGoOut)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[4].dialogueSequence);
            baseDialogue.onDialogueEnd += ReadyToGoOut;
        }
        else if (!hasShowedRespawn)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[1].dialogueSequence);
            baseDialogue.onDialogueEnd = GoToSellingScreen;
            hasShowedRespawn = true;
        }
        else if (!hasShowedMarket)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[2].dialogueSequence);
            baseDialogue.onDialogueEnd = GoToCrafting;
            hasShowedMarket = true;
        }
        else if (!hasShowedBuilding)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[4].dialogueSequence);
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

    void GoToRespawn()
    {
        isMoving = true;
        target.position = new Vector2(respawnPoint.transform.position.x + offset, robotPos.position.y);
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
    }

    void GiveItemToCraft()
    {
        Vector2 offset = isFacingRight ? new Vector2(2, 0) : new Vector2(-2, 0);
        Instantiate(ressourceToGive, (Vector2)transform.position + offset, transform.rotation);
    }

    #endregion

    #region Training Room

    void RobotTraining()
    {
        if (teleportIndex != currentIndex)
        {
            trainingRoomDialogue.EndDialogue();
            Teleport(teleportPoints[teleportIndex].position, holoMaterial, holoMaterial, LayerMask.GetMask("Default"));

            switch (currentIndex)
            {
                case 0:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[0].dialogueSequence, false);
                    DialogueManager.conditions["hasMovedAround"] = false;
                    break;
                case 1:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[1].dialogueSequence, false);
                    DialogueManager.conditions["hasJumped"] = false;
                    break;
                case 2:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[2].dialogueSequence, false);
                    DialogueManager.conditions["hasRun"] = false;
                    break;
                case 3:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[3].dialogueSequence, false);
                    DialogueManager.conditions["staminaIsEmpty"] = false;
                    DialogueManager.conditions["hasClimbed"] = false;
                    break;
                case 4:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[4].dialogueSequence);
                    trainingRoomDialogue.onDialogueEnd += GiveItemToSell;
                    trainingRoomDialogue.onDialogueEnd += TalkAboutInventory;
                    break;
                case 5:
                    TeleportSellingScreen();
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[6].dialogueSequence);
                    trainingRoomDialogue.onDialogueEnd += TalkAboutQuickMenu;
                    break;
                case 6:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[8].dialogueSequence, false);
                    trainingRoomDialogue.onDialogueEnd += TeleportBackSellingScreen;
                    trainingRoomDialogue.onDialogueEnd += TalkAboutCrafting;
                    break;
                case 7:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[10].dialogueSequence, false);
                    trainingRoomDialogue.onDialogueEnd += TeleportBackCrafting;
                    break;
                case 8:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[11].dialogueSequence, false);
                    break;
                case 9:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[12].dialogueSequence, false);
                    trainingRoomDialogue.onDialogueEnd += TalkAboutHotbar;
                    break;
                case 10:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[12].dialogueSequence, false);
                    trainingRoomDialogue.onDialogueEnd += TalkAboutHotbar;
                    break;
            }
        }
    }

    void TalkAboutInventory()
    {
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[5].dialogueSequence, false);
    }
    void TalkAboutQuickMenu()
    {
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[7].dialogueSequence, false);
    }

    void TalkAboutCrafting()
    {
        TeleportCrafting();
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[9].dialogueSequence);
    }

    void TalkAboutHotbar()
    {
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[13].dialogueSequence);
    }

    void GiveItemToSell()
    {
        Vector2 offset = isFacingRight ? new Vector2(2, 0) : new Vector2(-2, 0);
        Instantiate(ressourceToSell, (Vector2)transform.position + offset, transform.rotation);
    }

    void TeleportSellingScreen()
    {
        sellingScreen.GetComponent<SellingScreen>().TrainingRoom(true);
    }

    void TeleportCrafting()
    {
        craftingBench.GetComponent<CraftingBench>().TrainingRoom(true);
    }

    void TeleportBackSellingScreen()
    {
        sellingScreen.GetComponent<SellingScreen>().TrainingRoom(false);
    }

    void TeleportBackCrafting()
    {
        craftingBench.GetComponent<CraftingBench>().TrainingRoom(false);
    }

    void GoToTraining()
    {
        PromptManager.instance.CreateNewPrompt(new Prompt("Play the movement tutorial?", false, "Yes", "No"));
        PromptManager.onButtonClick = TeleportToTrainingRoom;
        PromptManager.onButtonNull = GoToCrafting;
    }

    void TeleportToTrainingRoom()
    {
        trainingRoomDialogue.enabled = true;
        baseDialogue.enabled = false;
        StartCoroutine(player.GetComponent<PlayerPermanent>().Dissolve(2f, true, GameObject.Find("Base").GetComponent<Base>().trainingRoom.position));
        Teleport(teleportPoints[teleportIndex].position, dissolveMaterial, holoMaterial, LayerMask.GetMask("Default"));
        isInTraining = true;
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[0].dialogueSequence, false);
    }

    void Teleport(Vector2 target, Material startMaterial, Material endMaterial, LayerMask endLayer)
    {
        currentIndex = teleportIndex;
        StartCoroutine(Dissolve(1f, true, target, startMaterial, endMaterial, endLayer));
    }

    public IEnumerator Dissolve(float dissolveTime, bool teleport, Vector2 target, Material startMaterial, Material endMaterial, LayerMask endLayer, bool isTrue = true)
    {
        isTeleporting = true;
        trainingRoomDialogue.isSpeaking = true;
        float elapsedTime = 0;
        if (teleport)
        {
            sprite.material = startMaterial;

            while (elapsedTime < dissolveTime)
            {
                elapsedTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(0.01f, 1f, elapsedTime / dissolveTime);
                sprite.material.SetFloat("_Transparency", dissolveAmount);
                yield return null;
            }

            transform.position = target;

            elapsedTime = 0;
            while (elapsedTime < dissolveTime)
            {
                elapsedTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(1, 0.01f, elapsedTime / dissolveTime);
                sprite.material.SetFloat("_Transparency", dissolveAmount);
                yield return null;
            }
            gameObject.layer = endLayer;
            sprite.material = endMaterial;
        }
        else
        {
            if (isTrue)
            {
                sprite.material = startMaterial;

                while (elapsedTime < dissolveTime)
                {
                    elapsedTime += Time.deltaTime;
                    float dissolveAmount = Mathf.Lerp(0.01f, 1f, elapsedTime / dissolveTime);
                    sprite.material.SetFloat("_Transparency", dissolveAmount);
                    yield return null;
                }
            }
            else
            {
                while (elapsedTime < dissolveTime)
                {
                    elapsedTime += Time.deltaTime;
                    float dissolveAmount = Mathf.Lerp(1f, 0.01f, elapsedTime / dissolveTime);
                    sprite.material.SetFloat("_Transparency", dissolveAmount);
                    yield return null;
                }
                gameObject.layer = endLayer;
                sprite.material = endMaterial;
            }
        }
        isTeleporting = false;
        trainingRoomDialogue.isSpeaking = false;
    }

    #endregion
}
