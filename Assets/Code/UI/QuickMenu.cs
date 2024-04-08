using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickMenu : MonoBehaviour
{
    public static QuickMenu instance;
    public Image frame;
    delegate void OnAnimationEnd();
    OnAnimationEnd onAnimationEnd;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI quotaText;
    public TextMeshProUGUI timerText;
    bool infoMenuActive = true;

    [SerializeField] GameObject messageMenu;
    bool messageMenuActive = true;
    [SerializeField] Image newMail;
    bool newMessage = false;

    public bool hasTeleport;
    [SerializeField] GameObject teleportMenu;
    bool teleportMenuActive = true;
    public TextMeshProUGUI teleporterText;
    public Button teleporterButton;
    [SerializeField] GameObject portal;

    [SerializeField] Button infoButton;
    [SerializeField] Button messageButton;
    [SerializeField] Button teleportButton;

    Animator anim;
    public bool isAnimating;
    bool isShown;

    [SerializeField] GameObject content;
    [SerializeField] GameObject messagePrefab;
    public Queue<TextMessage> textMessages = new Queue<TextMessage>();
    [SerializeField] float textCooldown;
    float textTime;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        ShowInfos();
        ShowMessages();
        ShowTeleport();
        textTime = Time.time;
    }

    private void Update()
    {
        if (!isAnimating)
        {
            if (Input.GetKeyDown(KeyCode.Z))
                ShowInfos();

            if (Input.GetKeyDown(KeyCode.X))
                ShowMessages();

            if (Input.GetKeyDown(KeyCode.C))
                ShowTeleport();
        }

        if (Time.time - textTime > textCooldown)
        {
            if (textMessages.Count > 0)
            {
                SendMessage(textMessages.Dequeue());
                if (!messageMenuActive)
                {
                    newMessage = true;
                    newMail.color = new Color(newMail.color.r, newMail.color.g, newMail.color.b, 255);
                }
            }
        }
    }

    public void SendMessage(TextMessage textMessage)
    {
        var message = Instantiate(messagePrefab);
        message.transform.Find("portrait").GetComponent<Image>().sprite = textMessage.portrait;
        message.transform.Find("messageTextInfo").GetComponent<TextMeshProUGUI>().text = textMessage.name;
        message.transform.Find("messageText").GetComponent<TextMeshProUGUI>().text = textMessage.textToWrite;
        message.transform.SetParent(content.transform);
    }

    public void OnButtonClick(bool isTrue)
    {
        isAnimating = true;

        isShown = isTrue;
        anim.SetBool("isShown", isTrue);
    }

    public void AnimationEnd()
    {
        onAnimationEnd?.Invoke();
        onAnimationEnd = null;

        isAnimating = false;
    }

    public void ShowMessages()
    {
        if (!messageMenuActive)
        {
            if (!isShown && QuickMenuNotOpen())
            {
                OnButtonClick(true);
                onAnimationEnd = ActivateMessage;
            }
            else
                ActivateMessage();

            if (newMessage)
            {
                newMessage = false;
                newMail.color = new Color(newMail.color.r, newMail.color.g, newMail.color.b, 0);
            }

            if (infoMenuActive)
                ShowInfos();
            if (teleportMenuActive)
                ShowTeleport();
        }
        else
        {
            messageMenu.SetActive(false);
            messageMenuActive = false;

            messageButton.interactable = true;

            if (isShown && QuickMenuNotOpen())
                OnButtonClick(false);
        }
    }

    public void ActivateMessage()
    {
        messageMenu.SetActive(true);
        messageMenuActive = true;
        messageButton.interactable = false;
    }

    public void ShowInfos()
    {
        if (!infoMenuActive)
        {
            if (!isShown && QuickMenuNotOpen())
            {
                OnButtonClick(true);
                onAnimationEnd = ActivateInfos;
            }
            else
                ActivateInfos();

            if (teleportMenuActive)
                ShowTeleport();
            if (messageMenuActive)
                ShowMessages();
        }
        else
        {
            infoMenuActive = false;

            float alpha = infoMenuActive ? 255 : 0;
            Debug.Log(alpha);
            dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
            quotaText.color = new Color(quotaText.color.r, quotaText.color.g, quotaText.color.b, alpha);
            timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, alpha);
            infoText.color = new Color(infoText.color.r, infoText.color.g, infoText.color.b, alpha);

            infoButton.interactable = true;

            if (isShown && QuickMenuNotOpen())
                OnButtonClick(false);
        }
    }

    public void ActivateInfos()
    {
        infoMenuActive = true;
        infoButton.interactable = false;

        float alpha = infoMenuActive ? 255 : 0;
        Debug.Log(alpha);
        dayText.color = new Color(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
        quotaText.color = new Color(quotaText.color.r, quotaText.color.g, quotaText.color.b, alpha);
        timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, alpha);
        infoText.color = new Color(infoText.color.r, infoText.color.g, infoText.color.b, alpha);
    }

    public void ShowTeleport()
    {
        if (!teleportMenuActive)
        {
            if (!isShown && QuickMenuNotOpen())
            {
                OnButtonClick(true);
                onAnimationEnd = ActivateTeleport;
            }
            else
                ActivateTeleport();

            if (infoMenuActive)
                ShowInfos();
            if (messageMenuActive)
                ShowMessages();

            CheckForOpenTeleporter();
        }
        else
        {
            teleportMenu.SetActive(false);
            teleportMenuActive = false;

            teleportButton.interactable = true;

            if (isShown && QuickMenuNotOpen())
                OnButtonClick(false);
        }
    }

    public void ActivateTeleport()
    {
        teleportMenu.SetActive(true);
        teleportMenuActive = true;
        teleportButton.interactable = false;
    }

    public void UnlockTeleporter(bool isTrue)
    {
        hasTeleport = isTrue;
        if (teleportMenuActive)
        {
            teleporterButton.interactable = isTrue;
            teleporterText.text = GameManager.instance.teleporter.Count + "available teleporters";
        }
    }

    public void ButtonTeleport()
    {
        PlayerPermanent player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        int facingDirection = player.isFacingRight ? 1 : -1;
        var thePortal = Instantiate(portal, (Vector2)player.gameObject.transform.position + Vector2.right * facingDirection, Quaternion.identity);
        thePortal.GetComponent<Portal>().isBaseTeleporter = false;
    }

    public void CheckForOpenTeleporter()
    {
        foreach (Teleporter teleporter in GameManager.instance.teleporter)
        {
            if (teleporter.isPoweredUp)
            {
                UnlockTeleporter(true);
                return;
            }
        }
        UnlockTeleporter(false);
    }

    bool QuickMenuNotOpen()
    {
        return !messageMenuActive && !teleportMenuActive && !infoMenuActive;
    }
}

public struct TextMessage
{
    public Sprite portrait;
    public string name;
    public string textToWrite;

    public TextMessage(Sprite _portrait, string _name, string _textToWrite)
    {
        portrait = _portrait;
        name = "DAY " + GameManager.instance.cycleCount.ToString("000") + " - " + GameManager.instance.TimeLeft + "\n" + _name;
        textToWrite = _textToWrite;
    }
}
