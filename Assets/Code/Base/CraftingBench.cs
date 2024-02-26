using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingBench : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;
    bool isCrafting;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInRange)
            {
                if (!player.inventoryOpen && !isCrafting)
                {
                    isCrafting = true;
                    player.ShowOrHideInventory(true, player.CanOpenStorage(), true);
                }
                else
                {
                    isCrafting = false;
                    player.ShowOrHideInventory(false, player.CanOpenStorage(), true);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInRange = false;
        if (player.inventoryOpen)
        {
            player.ShowOrHideInventory(false, player.CanOpenStorage(), true);
        }
        isCrafting = false;
    }
}
