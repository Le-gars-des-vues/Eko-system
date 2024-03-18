using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade1 : MonoBehaviour
{
    float hpMultiplier;
    bool isActive = false;
    InventoryItem item;
    PlayerPermanent player;

    private void OnEnable()
    {
        item = GetComponent<InventoryItem>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        hpMultiplier = player.hpMultiplier;
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
            player.maxHp *= hpMultiplier;
            player.SetMaxBar(player.hpSlider, player.maxHp);
            player.ChangeHp(player.currentHp / 2, false);
        }
        else
        {
            player.maxHp /= hpMultiplier;
            player.SetMaxBar(player.hpSlider, player.maxHp);
            player.ChangeHp(-(player.currentHp / 2), false);
        }
    }
}
