using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButtons : MonoBehaviour
{
    PlayerPermanent player;
    public int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    public void RightButton()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.inventaireSwap, gameObject);
        CheckUI();

        if (index == 0)
        {
            index = 1;
            player.ShowOrHideMap();
        }
        else if (index == 1)
        {
            index = 2;
            player.ShowOrHideUpgrades();
        }
        else if (index == 2)
        {
            index = 0;
            player.ShowOrHideInventory();
        }
    }

    public void LeftButton()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.inventaireSwap, gameObject);
        CheckUI();

        if (index == 0)
        {
            index = 2;
            player.ShowOrHideUpgrades();
        }
        else if (index == 2)
        {
            index = 1;
            player.ShowOrHideMap();
        }
        else if (index == 1)
        {
            index = 0;
            player.ShowOrHideInventory();
        }
    }

    void CheckUI()
    {
        if (player.inventoryOpen)
            index = 0;
        else if (player.mapIsOpen)
            index = 1;
        else if (player.upgradeIsOpen)
            index = 2;
    }
}
