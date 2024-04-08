using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    Dialogue dialogue;
    public List<Dialogue> tutorialTexts = new List<Dialogue>();
    [SerializeField] Sprite sprite;
    [SerializeField] string robotName;

    public bool firstTimeOutside = false;

    public void RobotTextMessage(string textToWrite)
    {
        TextMessage message = new TextMessage(sprite, robotName, textToWrite);
        QuickMenu.instance.textMessages.Enqueue(message);
    }
}
