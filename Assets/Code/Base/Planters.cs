using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planters : MonoBehaviour
{
    bool isInRange = false;
    bool hasAPlant = false;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject growLight;
    [SerializeField] Transform spawnPoint;

    GameManager gm;
    PlayerPermanent player;

    [SerializeField] List<GameObject> plants = new List<GameObject>();
    [SerializeField] GameObject plantToGrow;
    [SerializeField] GameObject thePlant;
    [SerializeField] int timeToGrow = 1;
    public int growthIndex = 0;

    [SerializeField] int maxRessource = 3;


    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.planters.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanPlant())
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (isInRange && arrow.GetComponent<ArrowFill>().readyToActivate)
                {
                    Plant();
                    arrow.SetActive(false);
                }
            }
        }

        if (hasAPlant && thePlant == null && growthIndex >= timeToGrow)
        {
            hasAPlant = false;
            growLight.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isInRange = true;
        if (CanPlant())
            arrow.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInRange = false;
        arrow.SetActive(false);
    }

    void Plant()
    {
        switch (player.objectInRightHand.GetComponent<InventoryItem>().itemData.itemName)
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
        hasAPlant = true;
        if (player.objectInRightHand != null)
            Destroy(player.objectInRightHand);
        player.UnequipObject();
    }

    public void Grow()
    {
        if (hasAPlant)
        {
            if (growthIndex < maxRessource)
            {
                growthIndex++;
            }
            if (growthIndex == timeToGrow)
            {
                thePlant = Instantiate(plantToGrow, transform.position, transform.rotation);
                thePlant.GetComponent<HarvestableRessourceNode>().ressourceAmount = 1;
                growLight.SetActive(true);
            }
            else if (growthIndex > timeToGrow && thePlant != null && thePlant.GetComponent<HarvestableRessourceNode>().ressourceAmount < maxRessource)
                thePlant.GetComponent<HarvestableRessourceNode>().ressourceAmount++;
        }
    }

    bool CanPlant()
    {
        if (player.objectInRightHand != null && !player.isUsingMultiTool)
            return player.objectInRightHand.GetComponent<InventoryItem>().itemData.itemType == "Plant" && isInRange && !hasAPlant;
        else
            return false;
    }
}
