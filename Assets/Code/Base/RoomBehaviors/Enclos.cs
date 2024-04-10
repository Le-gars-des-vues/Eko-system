using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enclos : MonoBehaviour
{
    bool isInRange = false;
    bool hasACreature = false;
    [SerializeField] Transform spawnPoint;

    PlayerPermanent player;

    [SerializeField] List<GameObject> creatures = new List<GameObject>();
    [SerializeField] GameObject creatureToClone;
    [SerializeField] GameObject theCreature;
    [SerializeField] int timeToGrow = 1;
    public int growthIndex = 0;

    [SerializeField] int maxRessource = 3;

    [SerializeField] GameObject room;
    [SerializeField] Farming farming;


    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        farming = GameObject.Find("Farming").GetComponent<Farming>();
        GameManager.instance.enclosure.Add(this);
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
                        if (!hasACreature)
                        {
                            if (!player.inventoryOpen)
                            {
                                player.ShowOrHideInventoryNoButtons();
                            }
                            if (!player.farmingIsOpen)
                            {
                                player.ShowOrHideFarming(false);
                            }
                        }
                        else
                        {
                            Harvest();
                            if (ArrowManager.instance.targetObject == gameObject)
                                ArrowManager.instance.RemoveArrow();
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
                    player.ShowOrHideFarming(false);
                }
            }
        }
        /*
        if (hasACreature && growthIndex >= timeToGrow && theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().isHarvested)
        {
            hasACreature = false;
            Destroy(theCreature);
            creatureToClone = null;
        }
        */

        if (room.GetComponent<RoomInfo>().isRefunded)
        {
            GameManager.instance.enclosure.Remove(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isInRange = true;
        if (!hasACreature)
            ArrowManager.instance.PlaceArrow(transform.position, "CLONE", new Vector2(0, -2), gameObject, 1);
        else
            ArrowManager.instance.PlaceArrow(transform.position, "HARVEST", new Vector2(0, -2), gameObject, 1);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInRange = false;
        if (ArrowManager.instance.targetObject == gameObject)
            ArrowManager.instance.RemoveArrow();
    }

    void Harvest()
    {
        for (int i = 0; i < theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().ressourceSpawnedCount; i++)
        {
            Instantiate(theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().ressourceToHarvest, transform.position, transform.rotation);
        }
        Instantiate(theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().dnaVial, transform.position, transform.rotation);
        hasACreature = false;
        Destroy(theCreature);
        creatureToClone = null;
    }

    public void Clone()
    {
        switch (farming.farmingSlot.GetItem(0, 0).itemData.itemName)
        {
            case "Dog DNA Vial":
                creatureToClone = creatures[0];
                break;
            case "Fly DNA Vial":
                creatureToClone = creatures[1];
                break;
            case "Frog DNA Vial":
                creatureToClone = creatures[2];
                break;
            default:
                return;
        }
        if (ArrowManager.instance.targetObject == gameObject)
            ArrowManager.instance.RemoveArrow();

        hasACreature = true;
        if (farming.farmingSlot.GetItem(0, 0) != null)
            farming.farmingSlot.GetItem(0, 0).Delete();
    }

    public void Grow()
    {
        if (hasACreature)
        {
            if (growthIndex < maxRessource)
            {
                growthIndex++;
            }
            if (growthIndex == timeToGrow)
            {
                theCreature = Instantiate(creatureToClone, spawnPoint.position, spawnPoint.rotation);
                theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().Death(0);
                theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().isInPod = true;
                theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().ressourceSpawnedCount = 1;
            }
            else if (growthIndex > timeToGrow && theCreature != null && theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().ressourceSpawnedCount < maxRessource)
                theCreature.transform.GetChild(0).GetComponent<CreatureDeath>().ressourceSpawnedCount++;
        }
    }
}
