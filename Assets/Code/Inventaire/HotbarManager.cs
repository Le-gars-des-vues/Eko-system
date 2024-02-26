using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    public GameObject[] hotbars;
    public GameObject[] hotbarHighlights;

    public int currentlySelected;

    public PlayerPermanent player;
    public List<ItemGrid> grids;

    [SerializeField] GameObject multiToolHighlight;

    private void Start()
    {
        grids = new List<ItemGrid>();
        for (int i = 0; i < hotbars.Length; i++)
        {
            grids.Add(hotbars[i].GetComponent<ItemGrid>());
        }
        currentlySelected = 0;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();

        for (int i = 0; i < hotbars.Length; i++)
        {
            if (hotbars[i] == hotbars[currentlySelected])
            {
                hotbarHighlights[i].SetActive(true);
            }
            else
            {
                hotbarHighlights[i].SetActive(false);
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentlySelected--;
            if(currentlySelected < 0)
            {

                currentlySelected = hotbars.Length - 1;
            }

            if (player.isUsingMultiTool)
            {
                multiToolHighlight.SetActive(false);
                player.EquipMultiTool(false);
            }

            if (grids[currentlySelected].GetItem(0, 0) != null)
                SwitchItem();

            for (int i = 0; i < hotbars.Length; i++)
            {
                if (hotbars[i] == hotbars[currentlySelected])
                {
                    hotbarHighlights[i].SetActive(true);
                }
                else
                {
                    hotbarHighlights[i].SetActive(false);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentlySelected++;
            if (currentlySelected == hotbars.Length)
            {

                currentlySelected = 0;
            }

            if (player.isUsingMultiTool)
            {
                multiToolHighlight.SetActive(false);
                player.EquipMultiTool(false);
            }

            if (grids[currentlySelected].GetItem(0, 0) != null)
                SwitchItem();

            for (int i = 0; i < hotbars.Length; i++)
            {
                if (hotbars[i] == hotbars[currentlySelected])
                {
                    hotbarHighlights[i].SetActive(true);
                }
                else
                {
                    hotbarHighlights[i].SetActive(false);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (multiToolHighlight != null)
            {
                if (!player.isUsingMultiTool)
                {
                    hotbarHighlights[currentlySelected].SetActive(false);
                    multiToolHighlight.SetActive(true);

                    if (player.objectInRightHand != null)
                        Destroy(player.objectInRightHand);
                    player.UnequipObject();

                    player.EquipMultiTool(true);
                }
                else
                {
                    hotbarHighlights[currentlySelected].SetActive(true);
                    multiToolHighlight.SetActive(false);
                    player.EquipMultiTool(false);

                    if (grids[currentlySelected].GetItem(0, 0) != null)
                    {
                        SpawnObject(currentlySelected, out GameObject objectSpawned);
                        player.EquipObject(objectSpawned);
                    }
                }
            }
        }
    }

    void SwitchItem()
    {
        if (player.objectInRightHand != null)
            Destroy(player.objectInRightHand);
        player.UnequipObject();
        SpawnObject(currentlySelected, out GameObject objectSpawned);
        player.EquipObject(objectSpawned);
    }

    public void SpawnObject(int index, out GameObject objectSpawned)
    {
        var objectToSpawn = Instantiate(grids[index].GetItem(0, 0).itemData.objectToSpawn);
        objectToSpawn.GetComponent<PickableObject>().itemInInventory = grids[index].GetItem(0, 0).gameObject;
        objectToSpawn.GetComponent<InventoryItem>().stackAmount = grids[index].GetItem(0, 0).stackAmount;
        objectToSpawn.GetComponent<PickableObject>().PickUp(true, true);
        objectToSpawn.GetComponent<PickableObject>().inventory = grids[index];
        objectSpawned = objectToSpawn;
    }
}
