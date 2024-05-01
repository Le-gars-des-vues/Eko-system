using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaUpgrade2 : MonoBehaviour
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
            player.maxStamina *= player.staminaMultiplier;
            player.SetMaxBar(player.staminaSlider, player.maxStamina);
        }
        else
        {
            player.maxStamina /= player.staminaMultiplier;
            player.SetMaxBar(player.staminaSlider, player.maxStamina);
        }
    }
}
