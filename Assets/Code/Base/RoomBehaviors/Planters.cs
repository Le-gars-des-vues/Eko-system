using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planters : MonoBehaviour
{
    bool isInRange = false;
    bool hasAPlant = false;
    [SerializeField] GameObject growLight;
    [SerializeField] Transform spawnPoint;

    PlayerPermanent player;

    [SerializeField] List<GameObject> plants = new List<GameObject>();
    [SerializeField] GameObject plantToGrow;
    [SerializeField] GameObject thePlant;
    [SerializeField] int timeToGrow = 1;
    public int growthIndex = 0;

    [SerializeField] int maxRessource = 3;

    [SerializeField] GameObject room;
    [SerializeField] Farming farming;


    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        farming = GameObject.Find("Farming").GetComponent<Farming>();
        GameManager.instance.planters.Add(this);
        room = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.farmingIsOpen)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (ArrowManager.instance.targetObject == gameObject)
                {
                    if (isInRange && ArrowManager.instance.readyToActivate)
                    {
                        if (!player.inventoryOpen)
                        {
                            player.ShowOrHideInventoryNoButtons();
                        }
                        if (!player.farmingIsOpen)
                        {
                            player.ShowOrHideFarming(true);
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (player.inventoryOpen && player.farmingIsOpen)
                {
                    player.ShowOrHideInventoryNoButtons();
                    player.ShowOrHideFarming(true);
                }
            }
        }

        if (hasAPlant && growthIndex >= timeToGrow && thePlant.GetComponent<HarvestableRessourceNode>().isHarvested)
        {
            hasAPlant = false;
            Destroy(thePlant);
            plantToGrow = null;
            growLight.SetActive(false);
        }

        if (room.GetComponent<RoomInfo>().isRefunded)
        {
            GameManager.instance.planters.Remove(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isInRange = true;
        if (!hasAPlant)
            ArrowManager.instance.PlaceArrow(transform.position, "PLANT", new Vector2(-0.35f, -8), gameObject, 1);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInRange = false;
        if (ArrowManager.instance.targetObject == gameObject)
            ArrowManager.instance.RemoveArrow();
    }

    public void Plant()
    {
        growthIndex = 0;
        switch (farming.farmingSlot.GetItem(0, 0).itemData.itemName)
        {
            case "Infpisum Pine":
                plantToGrow = plants[0];
                break;
            case "Macrebosia Nut":
                plantToGrow = plants[1];
                break;
            case "Caeruletam Leaf":
                plantToGrow = plants[2];
                break;
            default:
                return;
        }
        if (ArrowManager.instance.targetObject == gameObject)
            ArrowManager.instance.RemoveArrow();

        hasAPlant = true;
        if (farming.farmingSlot.GetItem(0, 0) != null)
            farming.farmingSlot.GetItem(0, 0).Delete();
    }

    public void Grow()
    {
        if (hasAPlant)
        {
            if (growthIndex < maxRessource + (timeToGrow - 1))
            {
                growthIndex++;
            }
            if (growthIndex == timeToGrow)
            {
                thePlant = Instantiate(plantToGrow, spawnPoint.position, spawnPoint.rotation);
                thePlant.GetComponent<HarvestableRessourceNode>().ressourceAmount = 1;
                growLight.SetActive(true);
            }
            else if (growthIndex > timeToGrow && thePlant != null && thePlant.GetComponent<HarvestableRessourceNode>().ressourceAmount < maxRessource)
                thePlant.GetComponent<HarvestableRessourceNode>().ressourceAmount++;
        }
    }
}
