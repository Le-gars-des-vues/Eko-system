using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    bool hasDied;
    bool hasSeenStorm;
    bool cameBackWithQuota;
    bool cameBackWithoutQuota;
    [SerializeField] GameObject ressourceToGive;
    [SerializeField] GameObject ressourceToSell;
    Vector2 initialPos;
    bool isInTour;

    [Header("Training Room Variables")]
    bool isInTraining;
    [SerializeField] Material dissolveMaterial;
    [SerializeField] Material holoMaterial;
    [SerializeField] Material ogMaterial;
    bool isTeleporting;
    [SerializeField] List<Transform> teleportPoints = new List<Transform>();
    public int teleportIndex = 0;
    int currentIndex;
    [SerializeField] Light2D holoLight;

    [SerializeField] List<SpriteRenderer> dissolveSprites = new List<SpriteRenderer>();
    List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        initialPos = robotPos.position;
        anim = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        trainingRoomDialogue.enabled = false;
        if (player.GetComponent<PlayerPermanent>().spawnAtBase)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[0]);
            baseDialogue.onDialogueEnd = GoToTraining;
        }
    }

    private void Update()
    {
        if (!isInTraining)
        {
            if (holoLight.enabled)
                holoLight.enabled = false;
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
                    if (isInTour)
                        RobotTutorial();
                }
            }
            else
            {
                if ((!isFacingRight && player.transform.position.x - robotPos.position.x > 0) || (isFacingRight && player.transform.position.x - robotPos.position.x < 0))
                    Turn();

                RobotConditionnalDialogue();
            }
        }
        else
        {
            if (!holoLight.enabled)
                holoLight.enabled = true;
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

        scale = baseDialogue.UI.transform.localScale;
        scale.x *= -1;
        baseDialogue.UI.transform.localScale = scale;

        scale = trainingRoomDialogue.UI.transform.localScale;
        scale.x *= -1;
        trainingRoomDialogue.UI.transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }



    #region Base Tutorial
    void RobotTutorial()
    {
        if (!hasShowedRespawn)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[2]);
            baseDialogue.onDialogueEnd = GoToSellingScreen;
            hasShowedRespawn = true;
        }
        else if (!hasShowedMarket)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[3]);
            baseDialogue.onDialogueEnd = GoToCrafting;
            hasShowedMarket = true;
        }
        else if (!hasShowedCrafting)
        {
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[4]);
            hasShowedCrafting = true;
            isInTour = false;
        }
    }

    void RobotConditionnalDialogue()
    {
        if (!hasDied && Tutorial.instance.hasDied)
        {
            hasDied = true;
            GoToRespawn();
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[5]);
        }
        else if (!cameBackWithoutQuota && Tutorial.instance.cameBackWithoutQuota)
        {
            cameBackWithoutQuota = true;
            GoToCrafting();
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[8]);
        }
        else if (!cameBackWithQuota && Tutorial.instance.cameBackWithQuota)
        {
            cameBackWithQuota = true;
            GoToCrafting();
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[9]);
            baseDialogue.onDialogueEnd += TourOfTheBase;
        }
        else if (!hasSeenStorm && Tutorial.instance.hasSeenStorm)
        {
            hasSeenStorm = true;
            GoToCrafting();
            baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[10]);
        }
    }

    void ReadyToGoOut()
    {
        Tutorial.instance.readyToGoOut = true;
        Tutorial.instance.hasUnlockedMessages = true;
        QuickMenu.instance.messageButton.interactable = true;
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

    void RefuseTutorial()
    {
        baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[1]);
        Tutorial.instance.hasUnlockedInfos = true;
        QuickMenu.instance.infoButton.interactable = true;
        baseDialogue.onDialogueEnd += RepairMultitool;
        baseDialogue.onDialogueEnd += ReadyToGoOut;
    }
    void RefuseTour()
    {
        hasShowedRespawn = true;
        hasShowedMarket = true;
        hasShowedCrafting = true;
        baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[7]);
    }

    void TourOfTheBase()
    {
        PromptManager.instance.CreateNewPrompt(new Prompt("Would you like a tour of the base?", false, "Yes", "No"));
        PromptManager.onButtonClick += GoToRespawn;
        PromptManager.onButtonClick += IsInTour;
        PromptManager.onButtonNull = RefuseTour;
    }

    void IsInTour()
    {
        isInTour = true;
    }

    void RepairMultitool()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasMultitool = true;
        Camera.main.GetComponent<InventoryController>().notWorking.SetActive(false);
    }

    #endregion

    #region Training Room

    void RobotTraining()
    {
        if (teleportIndex != currentIndex)
        {
            trainingRoomDialogue.EndDialogue();
            Teleport(teleportPoints[teleportIndex].position, holoMaterial, holoMaterial, LayerMask.NameToLayer("Default"));

            switch (currentIndex)
            {
                case 0:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[0]);
                    trainingRoomDialogue.onDialogueEnd += NextTeleport;
                    DialogueManager.conditions["hasMovedAround"] = false;
                    break;
                case 1:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[1]);
                    trainingRoomDialogue.onDialogueEnd += NextTeleport;
                    DialogueManager.conditions["hasJumped"] = false;
                    break;
                case 2:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[2]);
                    trainingRoomDialogue.onDialogueEnd += NextTeleport;
                    DialogueManager.conditions["hasRun"] = false;
                    break;
                case 3:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[3]);
                    trainingRoomDialogue.onDialogueEnd += NextTeleport;
                    DialogueManager.conditions["staminaIsEmpty"] = false;
                    DialogueManager.conditions["hasClimbed"] = false;
                    break;
                case 4:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[4]);
                    trainingRoomDialogue.onDialogueEnd += GiveItemToSell;
                    trainingRoomDialogue.onDialogueEnd += TalkAboutInventory;
                    break;
                case 5:
                    TeleportSellingScreen();
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[6]);
                    Tutorial.instance.hasUnlockedInfos = true;
                    QuickMenu.instance.infoButton.interactable = true;
                    trainingRoomDialogue.onDialogueEnd += TalkAboutQuickMenu;
                    break;
                case 6:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[8]);
                    trainingRoomDialogue.onDialogueEnd += TalkAboutCrafting;
                    break;
                case 7:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[11]);
                    trainingRoomDialogue.onDialogueEnd += NextTeleport;
                    break;
                case 8:
                    TeleportBackSellingScreen();
                    TeleportBackCrafting();
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[12]);
                    trainingRoomDialogue.onDialogueEnd += TalkAboutHarvest;
                    break;
                case 9:
                    sprites.Clear();
                    sprites.Add(dissolveSprites[2]);
                    StartCoroutine(Base.instance.Dissolve(sprites, 2f, false));
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[15]);
                    trainingRoomDialogue.onDialogueEnd += TalkAboutDurability;
                    break;
                case 10:
                    sprites.Clear();
                    sprites.Add(dissolveSprites[3]);
                    StartCoroutine(Base.instance.Dissolve(sprites, 2f, false));
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[17]);
                    trainingRoomDialogue.onDialogueEnd += NextTeleport;
                    break;
                case 11:
                    trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[18]);
                    break;
            }
        }
    }

    #region Miscellaneous

    void TalkAboutInventory()
    {
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[5]);
        trainingRoomDialogue.onNestedDialogueEnd += NextTeleport;
    }
    void TalkAboutQuickMenu()
    {
        Tutorial.instance.hasUnlockedInfos = true;
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[7]);
        trainingRoomDialogue.onNestedDialogueEnd += NextTeleport;
    }

    void TalkAboutCrafting()
    {
        TeleportCrafting();
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[9]);
        trainingRoomDialogue.onNestedDialogueEnd += GiveItemToCraft;
        trainingRoomDialogue.onNestedDialogueEnd += TalkAboutRecipes;
    }

    void TalkAboutHotbar()
    {
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[14]);
        trainingRoomDialogue.onNestedDialogueEnd += NextTeleport;
    }

    void TalkAboutRecipes()
    {
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[10]);
        trainingRoomDialogue.onNestedDialogueEnd += NextTeleport;
    }

    void TalkAboutDurability()
    {
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[16]);
        trainingRoomDialogue.onDialogueEnd += NextTeleport;
    }

    void TalkAboutHarvest()
    {
        sprites.Clear();
        sprites.Add(dissolveSprites[0]);
        sprites.Add(dissolveSprites[1]);
        StartCoroutine(Base.instance.Dissolve(sprites, 2f, false));
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[13]);
        trainingRoomDialogue.onNestedDialogueEnd += TalkAboutHotbar;
    }

    void GiveItemToSell()
    {
        Vector2 offset = isFacingRight ? new Vector2(2, 0) : new Vector2(-2, 0);
        Instantiate(ressourceToSell, (Vector2)transform.position + offset, transform.rotation);
    }

    void GiveItemToCraft()
    {
        Vector2 offset = isFacingRight ? new Vector2(2, 0) : new Vector2(-2, 0);
        Instantiate(ressourceToGive, (Vector2)transform.position + offset, transform.rotation);
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

    void NextTeleport()
    {
        teleportIndex++;
        Debug.Log("Moved teleport Index");
    }

    void TeleportBackCrafting()
    {
        craftingBench.GetComponent<CraftingBench>().TrainingRoom(false);
    }

    public void RevealLastRoom()
    {
        sprites.Clear();
        sprites.Add(dissolveSprites[4]);
        StartCoroutine(Base.instance.Dissolve(sprites, 2f, true));
    }

    #endregion

    void GoToTraining()
    {
        PromptManager.instance.CreateNewPrompt(new Prompt("Play the tutorial?", false, "Yes", "No"));
        PromptManager.onButtonClick = TeleportToTrainingRoom;
        PromptManager.onButtonNull = RefuseTutorial;
    }

    void TeleportToTrainingRoom()
    {
        trainingRoomDialogue.enabled = true;
        baseDialogue.enabled = false;
        StartCoroutine(player.GetComponent<PlayerPermanent>().Dissolve(2f, true, GameObject.Find("Base").GetComponent<Base>().trainingRoom.position));
        Teleport(teleportPoints[teleportIndex].position, dissolveMaterial, holoMaterial, LayerMask.NameToLayer("Default"));
        isInTraining = true;
        trainingRoomDialogue.PrepareDialogue(trainingRoomDialogue.dialogueSequences[0]);
        trainingRoomDialogue.onDialogueEnd += NextTeleport;
    }
    
    public void TeleportToBase()
    {
        trainingRoomDialogue.enabled = false;
        baseDialogue.enabled = true;
        Teleport(initialPos, holoMaterial, ogMaterial, LayerMask.NameToLayer("Pixelate"));
        isInTraining = false;
        baseDialogue.PrepareDialogue(baseDialogue.dialogueSequences[6]);
        baseDialogue.onDialogueEnd += ReadyToGoOut;
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
            AudioManager.instance.PlaySound(AudioManager.instance.teleport, gameObject);
            while (elapsedTime < dissolveTime)
            {
                elapsedTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(0.01f, 1f, elapsedTime / dissolveTime);
                sprite.material.SetFloat("_Transparency", dissolveAmount);
                yield return null;
            }

            transform.position = target;

            elapsedTime = 0;
            AudioManager.instance.PlaySound(AudioManager.instance.teleport, gameObject);
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
