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

    public bool brokeSpearInTutorial = false;

    public bool readyToGoOut = false;
    public bool firstTimeOutside = false;
    public bool firstTimeWater = false;
    public bool firstVineCollision = false;
    public bool firstWeaponBreak = false;
    public bool firstCreatureDeath = false;
    public bool firstHarvest = false;

    public bool hasUnlockedInfos;
    public bool hasUnlockedMessages;
    public bool hasUnlockedTeleporter;

    public bool hasDied;
    public bool hasSeenStorm;
    public bool cameBackWithQuota;
    public bool cameBackWithoutQuota;

    public bool day2;
    public bool day3;

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
        AudioManager.instance.PlaySound(AudioManager.instance.textMessage, Camera.main.gameObject);
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
                }
            }
            if (conditionIsMet)
                DialogueManager.instance.currentConditions.Remove(conditionName);
        }
        else
            return;
    }
}
