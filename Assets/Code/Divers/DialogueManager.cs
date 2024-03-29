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

    [SerializeField] GameObject tutorialScreen;
    PlayerPermanent player;

    bool pressedKey = false;
    bool dialogueScreenIsOpen = false;

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
        tutorialScreen = GameObject.Find("TutorialScreen");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    private void Update()
    {
        if (player.isInDialogue)
        {
            if (!dialogueScreenIsOpen)
            {
                dialogueScreenIsOpen = true;
                tutorialScreen.GetComponent<Animator>().SetBool("isInDialogue", true);
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
                if (Vector2.Distance(currentSpeaker.gameObject.transform.position, player.gameObject.transform.position) < startDialogueDistance)
                    StartTalking();
                else
                    return;
            }
        }
    }

    public void StartDialogue(List<Dialogue> dialogueSequence, DialogueSpeaker speaker, bool _isInDialogue)
    {
        dialigueIndex = 0;
        currentSpeaker = speaker;
        currentDialogue = dialogueSequence;
        isInDialogue = _isInDialogue;
        isReadyToTalk = true;
    }

    void StartTalking()
    {
        dialogue = StartCoroutine(currentSpeaker.Speech(currentDialogue[dialigueIndex].text));
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().isInDialogue = isInDialogue;
        isReadyToTalk = false;
    }

    public void NextSentence()
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

    public void EndDialogue()
    {
        isInDialogue = false;
        player.isInDialogue = false;
        if (dialogueScreenIsOpen)
        {
            dialogueScreenIsOpen = false;
            tutorialScreen.GetComponent<Animator>().SetBool("isInDialogue", false);
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
