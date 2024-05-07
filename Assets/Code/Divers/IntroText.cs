using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.SceneManagement;

public class IntroText : MonoBehaviour
{
    [SerializeField] Color colorHex;
    [SerializeField] TextMeshProUGUI speechBubbleText;

    public DialogueSequence introText;

    public AK.Wwise.Event speechSound;
    public AK.Wwise.Event stopSound;

    Coroutine dialogue;
    public int dialogueIndex = 0;

    public void IntroStartDialogue()
    {
        dialogue = StartCoroutine(Speech(introText.dialogueSequence[dialogueIndex].text));
        if (speechSound != null)
            speechSound.Post(gameObject);
    }

    public void IntroNextSentence()
    {
        dialogueIndex++;
        if (dialogueIndex < introText.dialogueSequence.Count)
        {
            StopCoroutine(dialogue);
            dialogue = StartCoroutine(Speech(introText.dialogueSequence[dialogueIndex].text));
            if (speechSound != null)
                speechSound.Post(gameObject);
        }
        else
            IntroEndDialogue();
    }

    public void IntroEndDialogue()
    {
        StopCoroutine(dialogue);
        StopDialogue();
        if (stopSound != null)
            stopSound.Post(gameObject);
    }

    public IEnumerator Speech(string textToWrite = null)
    {
        int index = 0;
        if (textToWrite != null)
        {
            speechBubbleText.text = textToWrite;
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
                    yield return new WaitForSeconds(0.02f);
                }
                else
                {
                    yield return null;
                    continue;
                }
            }
        }
        if (stopSound != null)
            stopSound.Post(gameObject);
        yield return null;
    }

    public void StartGame()
    {
        GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadGame();
    }

    public void StopDialogue()
    {
        speechBubbleText.text = "";
    }
}
