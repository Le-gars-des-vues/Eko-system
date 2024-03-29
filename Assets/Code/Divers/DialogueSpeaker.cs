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


    public IEnumerator Speech(string textToWrite = null)
    {
        if (textToWrite != null)
        {
            speechBubbleText.text = "";
            speechBubbleTextB.text = "";
            foreach (char letter in textToWrite.ToCharArray())
            {
                speechBubbleText.text += letter;
                speechBubbleTextB.text += letter;
                yield return new WaitForSeconds(0.03f);
            }
        }
        yield return null;
    }

    public void StopDialogue()
    {
        speechBubbleText.text = "";
        speechBubbleTextB.text = "";
    }
}
