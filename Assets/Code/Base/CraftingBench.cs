using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingBench : MonoBehaviour
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
        if (!player.craftingIsOpen)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (isInRange && arrow.GetComponent<ArrowFill>().readyToActivate)
                {
                    if (!player.inventoryOpen)
                    {
                        player.ShowOrHideInventoryNoButtons();
                    }
                    if (!player.craftingIsOpen)
                    {
                        player.ShowOrHideCrafting();
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (player.inventoryOpen && player.craftingIsOpen)
                {
                    player.ShowOrHideInventoryNoButtons();
                    player.ShowOrHideCrafting();
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
        if (player.craftingIsOpen)
        {
            player.ShowOrHideCrafting();
        }
    }
}
