using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade1 : MonoBehaviour
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
        if (item.isPlaced && !isActive)
        {
            isActive = true;
            ActivateUpgrade(true);
        }
        else if (!item.isPlaced && isActive)
        {
            isActive = false;
            ActivateUpgrade(false);
        }
    }

    void ActivateUpgrade(bool activated)
    {
        if (activated)
        {
            player.maxHp *= player.hpMultiplier;
            player.SetMaxBar(player.hpSlider, player.maxHp);
        }
        else
        {
            player.maxHp /= player.hpMultiplier;
            player.SetMaxBar(player.hpSlider, player.maxHp);
        }
    }
}
