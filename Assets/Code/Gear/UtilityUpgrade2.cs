using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityUpgrade2 : MonoBehaviour
{
    float staminaMultiplier = 1.5f;
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
            player.maxStamina *= staminaMultiplier;
            player.SetMaxBar(player.staminaSlider, player.maxStamina);
            player.ChangeHp(player.currentStamina / 2, false);
        }
        else
        {
            player.maxStamina /= staminaMultiplier;
            player.SetMaxBar(player.staminaSlider, player.maxStamina);
            player.ChangeHp(-(player.currentStamina / 2), false);
        }
    }
}
