using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingBench : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;
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
                if (isInRange && ArrowManager.instance.readyToActivate)
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
            ArrowManager.instance.PlaceArrow(transform.position, "CRAFTING BENCH", new Vector2(0, 1), gameObject, 1);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
            if (player.craftingIsOpen)
            {
                player.ShowOrHideCrafting();
            }
        }
    }
}
