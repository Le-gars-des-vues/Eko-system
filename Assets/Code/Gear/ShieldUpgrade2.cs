using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUpgrade2 : MonoBehaviour
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
        player.hasShield2 = activated;
        player.shield.SetActive(activated);
        player.shieldSlider.gameObject.SetActive(activated);
        if (activated)
        {
            player.SetMaxBar(player.shieldSlider, player.maxShield);
        }
        else
        {
            player.currentShield = 0;
            player.SetMaxBar(player.shieldSlider, player.currentShield);
        }
    }
}
