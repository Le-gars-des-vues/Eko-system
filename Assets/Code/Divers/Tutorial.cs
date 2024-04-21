using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    public bool isListeningForInputs = false;

    public List<Dialogue> tutorialTexts = new List<Dialogue>();
    [SerializeField] Sprite sprite;
    [SerializeField] string robotName;

    public bool readyToGoOut = false;
    public bool firstTimeOutside = false;
    public bool hasSeenFirstRessource;
    public bool hasHarvestedFirstRessource;

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

    public void ListenForInputs(string conditionName)
    {
        if (isListeningForInputs && !DialogueManager.conditions[conditionName])
        {
            bool conditionIsMet = false;
            foreach (string condition in DialogueManager.instance.currentConditions)
            {
                if (conditionName == condition)
                {
                    DialogueManager.conditions[conditionName] = true;
                    conditionIsMet = true;
                    Debug.Log("Met condition!");
                }
            }
            if (conditionIsMet)
                DialogueManager.instance.currentConditions.Remove(conditionName);
        }
        else
            return;
    }
}
