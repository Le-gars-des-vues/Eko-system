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

    public List<DialogueSequence> dialogueSequences = new List<DialogueSequence>();

    public bool isSpeaking;
    public bool isReadyToSpeak = false;
    bool hasACondition;
    [SerializeField] float startDialogueDistance = 4;
    bool pressedKey;
    float speakingTime;
    [SerializeField] float speakCooldown = 2;

    public AK.Wwise.Event speechSound;
    public AK.Wwise.Event stopSound;

    Coroutine dialogue;
    public int dialogueIndex = 0;
    List<Dialogue> currentDialogue;

    public GameObject UI;

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
                        if (dialogueIndex < currentDialogue.Count - 1 && !currentDialogue[dialogueIndex + 1].conditionIsMet)
                            hasACondition = true;


                        if ((Input.GetKeyDown(KeyCode.E) && !pressedKey) || (hasACondition && Time.time - speakingTime > speakCooldown))
                        {
                            if (dialogueIndex < currentDialogue.Count - 1)
                            {
                                if (currentDialogue[dialogueIndex + 1].conditionIsMet)
                                {
                                    //On skip l'animation du texte ou on passe a la prochaine phrase
                                    pressedKey = true;
                                    NextSentence();
                                    if (dialogueIndex == currentDialogue.Count - 1)
                                        UI.SetActive(false);
                                }
                                else
                                {
                                    Tutorial.instance.isListeningForInputs = true;
                                    UI.SetActive(false);
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
                }
            }
        }
    }

    public void PrepareDialogue(List<Dialogue> dialogueSequence, bool _isInDialogue = true)
    {
        currentDialogue = dialogueSequence;
        dialogueIndex = 0;
        DialogueManager.instance.isInDialogue = _isInDialogue;
        isReadyToSpeak = true;
    }

    public void StartDialogue()
    {
        speakingTime = Time.time;
        dialogue = StartCoroutine(Speech(currentDialogue[dialogueIndex].text));
        if (speechSound != null)
            speechSound.Post(transform.parent.gameObject);
        player.isInDialogue = DialogueManager.instance.isInDialogue;
        if (player.isInDialogue)
            StartCoroutine(player.MoveCamera("ZoomIn"));
        UI.SetActive(true);
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
        }
        else
        {
            dialogueIndex++;
            if (dialogueIndex < currentDialogue.Count)
            {
                StopCoroutine(dialogue);
                dialogue = StartCoroutine(Speech(currentDialogue[dialogueIndex].text));
                if (speechSound != null)
                    speechSound.Post(transform.parent.gameObject);
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
        DialogueManager.instance.isInDialogue = false;
        player.isInDialogue = false;

        StopCoroutine(dialogue);
        StopDialogue();
        if (stopSound != null)
            stopSound.Post(transform.parent.gameObject);

        currentDialogue = null;
        dialogueIndex = 0;

        onDialogueEnd?.Invoke();
        onDialogueEnd = null;
        UI.SetActive(false);
        isSpeaking = false;
        StartCoroutine(player.MoveCamera("NormalZoom"));
    }

    public IEnumerator Speech(string textToWrite = null)
    {
        isSpeaking = true;
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
        speakingTime = Time.time;
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
