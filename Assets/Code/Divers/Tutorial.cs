using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    Dialogue dialogue;
    public List<Dialogue> tutorialTexts = new List<Dialogue>();
    [SerializeField] Sprite sprite;
    [SerializeField] string robotName;

    public bool readyToGoOut = false;
    public bool firstTimeOutside = false;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public void RobotTextMessage(string textToWrite)
    {
        TextMessage message = new TextMessage(sprite, robotName, textToWrite);
        QuickMenu.instance.textMessages.Enqueue(message);
    }
}
