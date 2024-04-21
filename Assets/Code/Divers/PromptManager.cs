using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public static PromptManager instance;

    public delegate void OnButtonClick();
    public static OnButtonClick onButtonClick;

    public delegate void OnButtonNull();
    public static OnButtonNull onButtonNull;

    [SerializeField] GameObject prompt;
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] TextMeshProUGUI button1Text;
    [SerializeField] TextMeshProUGUI button2Text;
    [SerializeField] TextMeshProUGUI button3Text;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        prompt = GameObject.Find("Prompt");
        promptText = prompt.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        button1Text = prompt.transform.Find("Button01").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        button2Text = prompt.transform.Find("Button02").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        button3Text = prompt.transform.Find("Button03").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        prompt.SetActive(false);
    }

    public void CreateNewPrompt(Prompt thePrompt)
    {
        promptText.text = thePrompt.textToWrite;
        if (thePrompt.useOnlyOneButton)
        {
            button1Text.gameObject.transform.parent.gameObject.SetActive(false);
            button2Text.gameObject.transform.parent.gameObject.SetActive(false);
            button3Text.text = thePrompt.textButton3;
        }
        else
        {
            button3Text.gameObject.transform.parent.gameObject.SetActive(false);
            button1Text.text = thePrompt.textButton1;
            button2Text.text = thePrompt.textButton2;
        }
        prompt.SetActive(true);
        Time.timeScale = 0;
    }

    void ClosePrompt()
    {
        Time.timeScale = 1;
        prompt.SetActive(false);
    }

    public void ButtonClick(bool clickedYes)
    {
        if (clickedYes)
        {
            onButtonClick += ClosePrompt;
            onButtonClick?.Invoke();
            onButtonClick = null;
            onButtonNull = null;
        }
        else
        {
            onButtonNull += ClosePrompt;
            onButtonNull?.Invoke();
            onButtonClick = null;
            onButtonNull = null;
        }
    }
}

public class Prompt
{
    public string textToWrite;
    public string textButton1;
    public string textButton2;
    public string textButton3;
    public bool useOnlyOneButton;
    public Prompt(string _textToWrite, bool _useOnlyOneButton, string _textButton1 = null, string _textButton2 = null, string _textButton3 = null)
    {
        textToWrite = _textToWrite;
        useOnlyOneButton = _useOnlyOneButton;
        if (useOnlyOneButton)
        {
            textButton3 = _textButton3;
        }
        else
        {
            textButton1 = _textButton1;
            textButton2 = _textButton2;
        }
    }
}
