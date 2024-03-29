using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingBench : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;
    [SerializeField] GameObject arrow;
    CraftingManager crafting;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        crafting = GameObject.Find("Crafting").transform.Find("DropdownListOfCrafts").GetComponent<CraftingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.craftingIsOpen)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (isInRange && arrow.GetComponent<Arrow>().readyToActivate)
                {
                    crafting.OnValueChanged();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            arrow.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            arrow.SetActive(false);
            if (player.craftingIsOpen)
            {
                player.ShowOrHideCrafting();
            }
        }
    }
}
