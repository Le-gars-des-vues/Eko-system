using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellingScreen : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;

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
                if (player.inventoryOpen && player.marketIsOpen)
                {
                    player.ShowOrHideInventoryNoButtons();
                    player.ShowOrHideMarket();
                }
                else
                {
                    if (!player.inventoryOpen)
                    {
                        player.ShowOrHideInventoryNoButtons();
                    }
                    if (!player.marketIsOpen)
                    {
                        player.ShowOrHideMarket();
                    }
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
        if (player.marketIsOpen)
        {
            player.ShowOrHideMarket();
        }
    }
}
