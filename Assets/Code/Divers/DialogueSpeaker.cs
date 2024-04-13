using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSpeaker : MonoBehaviour
{
    public TextMeshPro speechBubbleText;
    public TextMeshPro speechBubbleTextB;

    public List<Dialogue> dialogueSequence;
    public List<Dialogue> dialogueSequence2;
    public List<Dialogue> dialogueSequence3;
    public List<Dialogue> dialogueSequence4;
    public List<Dialogue> dialogueSequence5;

    public bool isSpeaking;

    public IEnumerator Speech(string textToWrite = null)
    {
        isSpeaking = true;
        int index = 0;
        if (textToWrite != null)
        {
            speechBubbleText.text = textToWrite;
            speechBubbleTextB.text = textToWrite.Replace("<color=red>", "<color=black>")
                                                .Replace("<color=green>", "<color=black>")
                                                .Replace("<color=yellow>", "<color=black>")
                                                .Replace("<color=white>", "<color=black>")
                                                .Replace("<color=lime>", "<color=black>")
                                                .Replace("<color=lightblue>", "<color=black>")
                                                .Replace("<color=blue>", "<color=black>")
                                                .Replace("<color=orange>", "<color=black>");
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
                    yield return new WaitForSeconds(0.025f);
                }
                else
                {
                    yield return null;
                    continue;
                }
            }
        }
        isSpeaking = false;
        yield return null;
    }

    public void StopDialogue()
    {
        speechBubbleText.text = "";
        speechBubbleTextB.text = "";
    }
}
