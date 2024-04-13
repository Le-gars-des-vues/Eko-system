using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    int dialigueIndex;
    bool isInDialogue;
    bool isReadyToTalk = false;
    [SerializeField] float startDialogueDistance;

    [SerializeField] List<Dialogue> currentDialogue;
    [SerializeField] DialogueSpeaker currentSpeaker;
    Coroutine dialogue;

    public delegate void OnDialogueEnd();
    public static OnDialogueEnd onDialogueEnd;

    [SerializeField] GameObject dialogueScreen;
    PlayerPermanent player;

    bool pressedKey = false;
    bool dialogueScreenIsOpen = false;

    public static Dictionary<string, bool> conditions = new Dictionary<string, bool>
    {
        {"craftedFirstItem", false }
    };

    bool hasACondition;
    public bool conditionIsMet = true;
    string conditionName;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        dialigueIndex = 0;
    }

    private void Start()
    {
        dialogueScreen = GameObject.Find("DialogueScreen");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    private void Update()
    {
        if (hasACondition)
        {
            if (conditions[conditionName])
            {
                conditionIsMet = true;
                hasACondition = false;
                Debug.Log("Condition is met!");
            }
        }

        if (player.isInDialogue)
        {
            if (!dialogueScreenIsOpen)
            {
                dialogueScreenIsOpen = true;
                dialogueScreen.GetComponent<Animator>().SetBool("isInDialogue", true);
            }

            if (Input.GetKeyDown(KeyCode.E) && !pressedKey)
            {
                pressedKey = true;
                NextSentence();
            }
        }
        else
        {
            if (isReadyToTalk)
            {
                if (Vector2.Distance(currentSpeaker.gameObject.transform.position, player.gameObject.transform.position) < startDialogueDistance && conditionIsMet)
                    StartTalking();
                else
                    return;
            }
        }
    }

    public void StartDialogue(List<Dialogue> dialogueSequence, DialogueSpeaker speaker, bool _isInDialogue = true, bool _hasACondition = false, string _conditionName = "")
    {
        dialigueIndex = 0;
        currentSpeaker = speaker;
        currentDialogue = dialogueSequence;
        isInDialogue = _isInDialogue;
        isReadyToTalk = true;
        if (_hasACondition)
        {
            hasACondition = _hasACondition;
            conditionName = _conditionName;
            conditionIsMet = conditions[_conditionName];
        }
        else
            conditionIsMet = true;
    }

    void StartTalking()
    {
        dialogue = StartCoroutine(currentSpeaker.Speech(currentDialogue[dialigueIndex].text));
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().isInDialogue = isInDialogue;
        isReadyToTalk = false;
    }

    public void NextSentence()
    {
        if (currentSpeaker.isSpeaking)
        {
            StopCoroutine(dialogue);
            currentSpeaker.speechBubbleText.text = currentDialogue[dialigueIndex].text;
            currentSpeaker.speechBubbleTextB.text = currentDialogue[dialigueIndex].text.Replace("<color=red>", "<color=black>")
                                                .Replace("<color=green>", "<color=black>")
                                                .Replace("<color=yellow>", "<color=black>")
                                                .Replace("<color=white>", "<color=black>")
                                                .Replace("<color=lime>", "<color=black>")
                                                .Replace("<color=lightblue>", "<color=black>")
                                                .Replace("<color=blue>", "<color=black>")
                                                .Replace("<color=orange>", "<color=black>");
            currentSpeaker.speechBubbleText.maxVisibleCharacters = currentSpeaker.speechBubbleText.text.ToCharArray().Length;
            currentSpeaker.speechBubbleTextB.maxVisibleCharacters = currentSpeaker.speechBubbleTextB.text.ToCharArray().Length;
            currentSpeaker.isSpeaking = false;
            pressedKey = false;
        }
        else
        {
            dialigueIndex++;
            if (dialigueIndex < currentDialogue.Count)
            {
                StopCoroutine(dialogue);
                dialogue = StartCoroutine(currentSpeaker.Speech(currentDialogue[dialigueIndex].text));
                pressedKey = false;
            }
            else
            {
                EndDialogue();
                pressedKey = false;
            }
        }
    }

    public void EndDialogue()
    {
        isInDialogue = false;
        player.isInDialogue = false;
        if (dialogueScreenIsOpen)
        {
            dialogueScreenIsOpen = false;
            dialogueScreen.GetComponent<Animator>().SetBool("isInDialogue", false);
        }
        StopCoroutine(dialogue);
        currentSpeaker.StopDialogue();
        onDialogueEnd?.Invoke();
        onDialogueEnd = null;
    }
}

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(3, 10)]
    public string text;
}
