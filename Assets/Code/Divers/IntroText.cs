using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.SceneManagement;

public class IntroText : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] Color colorHex;
    [SerializeField] TextMeshProUGUI speechBubbleText;

    public DialogueSequence introText;

    Coroutine dialogue;
    public int dialogueIndex = 0;

    public void IntroStartDialogue()
    {
        dialogue = StartCoroutine(Speech(introText.dialogueSequence[dialogueIndex].text));
    }

    public void IntroNextSentence()
    {
        dialogueIndex++;
        if (dialogueIndex < introText.dialogueSequence.Count)
        {
            StopCoroutine(dialogue);
            dialogue = StartCoroutine(Speech(introText.dialogueSequence[dialogueIndex].text));
        }
        else
            IntroEndDialogue();
    }

    public void IntroEndDialogue()
    {
        StopCoroutine(dialogue);
        StopDialogue();
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
        yield return null;
    }

    public void StartGame()
    {
        GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadGame();
        AkSoundEngine.StopPlayingID(mainMenu.GetComponent<StartMenu>().introID);
    }

    public void StopDialogue()
    {
        speechBubbleText.text = "";
    }
}
