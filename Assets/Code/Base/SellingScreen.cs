using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellingScreen : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;
    [SerializeField] GameObject arrow;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.marketIsOpen)
        {
            if (Input.GetKey(KeyCode.E) && arrow.GetComponent<ArrowFill>().readyToActivate)
            {
                if (isInRange)
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
        else
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
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isInRange = true;
        arrow.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInRange = false;
        arrow.SetActive(false);
        if (player.marketIsOpen)
        {
            player.ShowOrHideMarket();
        }
    }
}
