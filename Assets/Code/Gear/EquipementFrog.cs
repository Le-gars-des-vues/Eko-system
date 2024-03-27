using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipementFrog : MonoBehaviour
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
            player.maxOxygen *= player.oxygenMultiplier;
            player.oxygenDepleteRate /= player.oxygenDepleteRateMultiplier;
            player.SetMaxBar(player.oxygenSlider, player.maxOxygen);
        }
        else if (!item.isPlaced && isActive)
        {
            isActive = false;
            ActivateUpgrade(false);
            player.maxOxygen /= player.oxygenMultiplier;
            player.oxygenDepleteRate *= player.oxygenDepleteRateMultiplier;
            player.SetMaxBar(player.oxygenSlider, player.maxOxygen);
        }
    }

    void ActivateUpgrade(bool activated)
    {
        player.hasOxygenMask = activated;
        player.frogMask.SetActive(activated);
    }
}
