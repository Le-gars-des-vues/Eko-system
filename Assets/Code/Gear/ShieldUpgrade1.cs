using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUpgrade1 : MonoBehaviour
{
    bool isActive = false;
    InventoryItem item;
    PlayerPermanent player;
    [SerializeField] float maxShield = 10;

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
            player.hasShield = true;
            player.maxShield = maxShield;
            GameObject.Find("shieldBar").SetActive(true);
            player.currentShield = player.maxShield;
            player.SetMaxBar(player.shieldSlider, player.maxShield);
        }
        else
        {
            player.hasShield = false;
            player.maxShield = 0;
            player.currentShield = player.maxShield;
            player.SetMaxBar(player.shieldSlider, player.maxShield);
            GameObject.Find("shieldBar").SetActive(false);
        }
    }
}
