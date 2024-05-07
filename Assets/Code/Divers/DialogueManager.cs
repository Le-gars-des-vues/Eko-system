using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] GameObject dialogueScreen;
    PlayerPermanent player;

    bool dialogueScreenIsOpen = false;
    public bool dialogueRunning;

    public static Dictionary<string, bool> conditions = new Dictionary<string, bool>
    {
        {"hasMovedAround", false },
        {"hasJumped", false },
        {"hasRun", false },
        {"hasHanged", false },
        {"hasClimbed", false },
        {"hasPickedUpRessource", false },
        {"hasOpenInventory", false },
        {"hasSoldItem", false },
        {"hasOpenInfo", false },
        {"hasCraftedFirstItem", false },
        {"hasWallJumped", false },
        {"hasEquippedMutltitool", false },
        {"hasHovered", false },
        {"hasHarvested", false },
        {"hasDestroyedPlant", false },
        {"hasPickedUpSpear", false },
        {"hasHitDummy", false },
        {"hasHitTarget", false },
    };

    public List<string> currentConditions = new List<string>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        SceneLoader.allScenesLoaded += StartScript;
    }

    private void StartScript()
    {
        dialogueScreen = GameObject.Find("DialogueScreen");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    private void Update()
    {
        if (SceneLoader.instance.isLoading) return;

        //Si le joueur est en dialogue
        if (player.isInDialogue)
        {
            //Si l'ecran de dialogue n'est pas ouvert
            if (!dialogueScreenIsOpen)
            {
                //On ouvre l'ecran de dialogue
                dialogueScreenIsOpen = true;
                dialogueScreen.GetComponent<Animator>().SetBool("isInDialogue", true);
            }
        }
        else
        {
            if (dialogueScreenIsOpen)
            {
                dialogueScreenIsOpen = false;
                dialogueScreen.GetComponent<Animator>().SetBool("isInDialogue", false);
            }
        }
    }
}

[System.Serializable]
public class Dialogue
{
    public string name;
    public bool conditionIsMet = true;
    public string conditionName;

    public bool dialogueMode = true;

    [TextArea(3, 10)]
    public string text;
}

[System.Serializable]
public class DialogueSequence
{
    public List<Dialogue> dialogueSequence;
}
