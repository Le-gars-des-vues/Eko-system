using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    static DialogueManager instance;

    [SerializeField] Dialogue[] tutorialRobotText;
    [SerializeField] Robot robot;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        robot = GameObject.FindGameObjectWithTag("Robot").GetComponent<Robot>();
        RobotTalk(tutorialRobotText[0].text);
    }

    public void RobotTalk(string textToWrite)
    {
        StartCoroutine(robot.Speech(textToWrite));
    }
}

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(3, 10)]
    public string text;
}
