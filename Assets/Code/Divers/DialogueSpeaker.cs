using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSpeaker : MonoBehaviour
{
    PlayerPermanent player;
    [SerializeField] Color colorHex;
    public TextMeshPro speechBubbleText;
    public TextMeshPro speechBubbleTextB;
    public Vector2 speakerOffset;

    public delegate void OnDialogueEnd();
    public OnDialogueEnd onDialogueEnd;
    public OnDialogueEnd onNestedDialogueEnd;

    public List<DialogueSequence> dialogueSequences = new List<DialogueSequence>();
    DialogueSequence currentSequence;

    public bool isSpeaking;
    public bool isReadyToSpeak = false;
    bool hasACondition;
    [SerializeField] float startDialogueDistance = 4;
    bool pressedKey;

    public AK.Wwise.Event speechSound;
    public AK.Wwise.Event stopSound;

    Coroutine dialogue;
    bool isZoomedIn;
    public int dialogueIndex = 0;
    List<Dialogue> currentDialogue;

    public GameObject UI;

    public bool dialogueEnded;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    private void Update()
    {
        if (currentDialogue != null)
        {
            if (player.isInDialogue)
            {
                if (dialogueIndex < currentDialogue.Count - 1 && !currentDialogue[dialogueIndex + 1].dialogueMode && !isSpeaking)
                {
                    player.isInDialogue = false;
                    if (isZoomedIn)
                    {
                        isZoomedIn = false;
                        StartCoroutine(player.MoveCamera("NormalZoom"));
                    }
                    return;
                }


                //Si on appuie sur E
                if (Input.GetKeyDown(KeyCode.E) && !pressedKey)
                {
                    //On skip l'animation du texte ou on passe a la prochaine phrase
                    pressedKey = true;
                    NextSentence();
                }
            }
            else
            {
                if (player.CanMove())
                {
                    //Si le parleur est pret a parler
                    if (isReadyToSpeak && !isSpeaking)
                    {
                        //Si la distance est suffisement courte et que la condition est remplie
                        if (Vector2.Distance((Vector2)transform.parent.transform.position + speakerOffset, player.gameObject.transform.position) < startDialogueDistance)
                        {
                            if (currentDialogue[dialogueIndex].conditionIsMet)
                            {
                                //On parle
                                StartDialogue();
                            }
                            else
                            {
                                Tutorial.instance.isListeningForInputs = true;
                                if (!DialogueManager.instance.currentConditions.Contains(currentDialogue[dialogueIndex].conditionName) && !DialogueManager.conditions[currentDialogue[dialogueIndex].conditionName])
                                    DialogueManager.instance.currentConditions.Add(currentDialogue[dialogueIndex].conditionName);

                                currentDialogue[dialogueIndex].conditionIsMet = DialogueManager.conditions[currentDialogue[dialogueIndex].conditionName];
                            }
                        }
                    }
                    else if (!isReadyToSpeak && !isSpeaking)
                    {
                        if (dialogueIndex < currentDialogue.Count - 1 && !currentDialogue[dialogueIndex + 1].dialogueMode)
                            player.isInDialogue = false;

                        if (dialogueIndex < currentDialogue.Count - 1 && !currentDialogue[dialogueIndex + 1].conditionIsMet)
                        {
                            hasACondition = true;
                        }

                        if ((Input.GetKeyDown(KeyCode.E) && !pressedKey) || hasACondition)
                        {
                            if (dialogueIndex < currentDialogue.Count - 1)
                            {
                                if (currentDialogue[dialogueIndex + 1].conditionIsMet)
                                {
                                    //On skip l'animation du texte ou on passe a la prochaine phrase
                                    pressedKey = true;
                                    NextSentence();
                                    if (dialogueIndex == currentDialogue.Count - 1)
                                    {
                                        UI.GetComponent<DialogueToolTip>().ChangeTooltip(0);
                                    }
                                }
                                else
                                {
                                    Tutorial.instance.isListeningForInputs = true;
                                    if (!DialogueManager.instance.currentConditions.Contains(currentDialogue[dialogueIndex + 1].conditionName) && !DialogueManager.conditions[currentDialogue[dialogueIndex + 1].conditionName])
                                        DialogueManager.instance.currentConditions.Add(currentDialogue[dialogueIndex + 1].conditionName);

                                    currentDialogue[dialogueIndex + 1].conditionIsMet = DialogueManager.conditions[currentDialogue[dialogueIndex + 1].conditionName];
                                }
                            }
                            else
                            {
                                pressedKey = true;
                                NextSentence();
                            }
                        }
                    }
                    else if (!isReadyToSpeak && isSpeaking)
                    {
                        if (Input.GetKeyDown(KeyCode.E) && !pressedKey)
                        {
                            pressedKey = true;
                            NextSentence();
                        }
                    }
                }
            }
        }
    }

    public void PrepareDialogue(DialogueSequence dialogueSequence)
    {
        currentSequence = dialogueSequence;
        currentDialogue = dialogueSequence.dialogueSequence;
        dialogueIndex = 0;
        isReadyToSpeak = true;
        dialogueEnded = false;
    }

    public void StartDialogue()
    {
        DialogueManager.instance.dialogueRunning = true;
        player.isInDialogue = currentDialogue[dialogueIndex].dialogueMode;
        UI.SetActive(true);
        dialogue = StartCoroutine(Speech(currentDialogue[dialogueIndex].text));
        if (speechSound != null)
            speechSound.Post(transform.parent.gameObject);
        if (player.isInDialogue && currentDialogue[dialogueIndex + 1].dialogueMode)
        {
            isZoomedIn = true;
            StartCoroutine(player.MoveCamera("ZoomIn"));
        }
        hasACondition = false;
        isReadyToSpeak = false;
    }

    public void NextSentence()
    {
        hasACondition = false;
        if (isSpeaking)
        {
            StopCoroutine(dialogue);
            if (stopSound != null)
                stopSound.Post(transform.parent.gameObject);
            speechBubbleText.text = currentDialogue[dialogueIndex].text;
            speechBubbleTextB.text = currentDialogue[dialogueIndex].text;
            speechBubbleText.maxVisibleCharacters = speechBubbleText.text.ToCharArray().Length;
            speechBubbleTextB.maxVisibleCharacters = speechBubbleTextB.text.ToCharArray().Length;
            isSpeaking = false;
            pressedKey = false;
            if (dialogueIndex == currentDialogue.Count - 1)
                UI.GetComponent<DialogueToolTip>().ChangeTooltip(2);
            else
            {
                if (!currentDialogue[dialogueIndex + 1].conditionIsMet)
                    UI.GetComponent<DialogueToolTip>().ChangeTooltip(3);
                else
                    UI.GetComponent<DialogueToolTip>().ChangeTooltip(0);
            }
        }
        else
        {
            dialogueIndex++;
            if (dialogueIndex < currentDialogue.Count)
            {
                StopCoroutine(dialogue);
                player.isInDialogue = currentDialogue[dialogueIndex].dialogueMode;
                if (player.isInDialogue && !currentDialogue[dialogueIndex - 1].dialogueMode)
                {
                    isZoomedIn = true;
                    StartCoroutine(player.MoveCamera("ZoomIn"));
                }
                dialogue = StartCoroutine(Speech(currentDialogue[dialogueIndex].text));
                if (speechSound != null)
                    speechSound.Post(transform.parent.gameObject);
                pressedKey = false;
            }
            else
            {
                dialogueEnded = true;
                EndDialogue();
                pressedKey = false;
            }
        }
    }

    public void EndDialogue()
    {
        player.isInDialogue = false;

        StopCoroutine(dialogue);
        StopDialogue();
        if (stopSound != null)
            stopSound.Post(transform.parent.gameObject);

        UI.SetActive(false);
        isSpeaking = false;
        DialogueManager.instance.dialogueRunning = false;

        currentDialogue = null;
        dialogueIndex = 0;

        if (dialogueEnded)
            onNestedDialogueEnd?.Invoke();
        onNestedDialogueEnd = null;

        if (dialogueEnded)
            onDialogueEnd?.Invoke();
        onDialogueEnd = null;

        if (isZoomedIn)
        {
            isZoomedIn = false;
            StartCoroutine(player.MoveCamera("NormalZoom"));
        }
    }

    public IEnumerator Speech(string textToWrite = null)
    {
        isSpeaking = true;
        UI.GetComponent<DialogueToolTip>().ChangeTooltip(1);
        int index = 0;
        if (textToWrite != null)
        {
            speechBubbleText.text = textToWrite;
            speechBubbleTextB.text = textToWrite;
            char[] charArray = textToWrite.ToCharArray();
            bool isLetter = true;
            for (int i = 0; i < charArray.Length; i++)
            {
                if (charArray[i] == '<')
                {
                    isLetter = false;
                    //yield return null;
                }
                else if (charArray[i] == '>')
                {
                    isLetter = true;
                    yield return null;
                    continue;
                }

                if (isLetter)
                {
                    index++;
                    speechBubbleText.maxVisibleCharacters = index;
                    speechBubbleTextB.maxVisibleCharacters = index;
                    yield return new WaitForSeconds(0.02f);
                }
                else
                {
                    yield return null;
                    continue;
                }
            }
        }
        isSpeaking = false;
        if (stopSound != null)
            stopSound.Post(transform.parent.gameObject);
        if (dialogueIndex == currentDialogue.Count - 1)
            UI.GetComponent<DialogueToolTip>().ChangeTooltip(2);
        else
        {
            if (!currentDialogue[dialogueIndex + 1].conditionIsMet)
                UI.GetComponent<DialogueToolTip>().ChangeTooltip(3);
            else
                UI.GetComponent<DialogueToolTip>().ChangeTooltip(0);
        }
            
        yield return null;
    }

    public void StopDialogue()
    {
        speechBubbleText.text = "";
        speechBubbleTextB.text = "";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere((Vector2)transform.parent.transform.position + speakerOffset, 0.1f);
    }
}
