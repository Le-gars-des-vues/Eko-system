using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButtons : MonoBehaviour
{
    PlayerPermanent player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    public void ShowOrHideInventory()
    {
        player.ShowOrHideInventory();
    }

    public void ShowOrHideMap()
    {
        player.ShowOrHideMap();
    }

    public void ShowOrHideUpgrade()
    {
        player.ShowOrHideUpgrades();
    }
}
