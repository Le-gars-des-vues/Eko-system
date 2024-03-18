using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUpgrade1 : MonoBehaviour
{
    bool isActive = false;
    InventoryItem item;
    PlayerPermanent player;

    private void OnEnable()
    {
        item = GetComponent<InventoryItem>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (item.isUpgrading && !isActive)
        {
            isActive = true;
            ActivateUpgrade(true);
        }
        else if (!item.isUpgrading && isActive)
        {
            isActive = false;
            ActivateUpgrade(false);
        }
    }

    void ActivateUpgrade(bool activated)
    {
        if (activated)
        {
            player.hasOptics = true;
        }
        else
        {
            player.hasOptics = false;
        }
    }
}
