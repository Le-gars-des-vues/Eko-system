using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CycleInfo : MonoBehaviour
{
    public static CycleInfo instance;

    public TextMeshProUGUI quotaText;
    public TextMeshProUGUI timerText;

    public bool hasTeleport;
    public TextMeshProUGUI teleporterButtonText;
    public Button teleporterButton;
    [SerializeField] GameObject portal;

    Animator anim;
    bool isShowm;

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
    }

    public void OnButtonClick()
    {
        if (!isShowm)
        {
            isShowm = true;
            anim.SetBool("isShown", true);
        }
        else
        {
            isShowm = false;
            anim.SetBool("isShown", false);
        }
    }

    public void UnlockTeleporter(bool isTrue)
    {
        hasTeleport = isTrue;
        teleporterButton.interactable = isTrue;
        if (hasTeleport)
            teleporterButtonText.text = "Teleport back to base";
        else
            teleporterButtonText.text = "No availables teleporters";
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
}
