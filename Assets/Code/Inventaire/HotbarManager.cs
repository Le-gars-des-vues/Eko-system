using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.SceneManagement;

public class HotbarManager : MonoBehaviour
{
    public GameObject[] hotbars;
    public GameObject[] hotbarHighlights;

    public int currentlySelected;

    public PlayerPermanent player;
    public List<ItemGrid> grids;

    [SerializeField] GameObject multiToolHighlight;

    private void Awake()
    {
        SceneLoader.allScenesLoaded += StartScript;
    }

    private void StartScript()
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
        if (SceneLoader.instance.isLoading) return;

        if (!PromptManager.instance.promptOpen)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (currentlySelected != 0)
                {
                    currentlySelected = 0;
                    HotbarInput();
                }
                else
                {
                    if (multiToolHighlight != null && player.hasMultitool)
                    {
                        if (!player.isUsingMultiTool)
                        {
                            hotbarHighlights[currentlySelected].SetActive(false);
                            multiToolHighlight.SetActive(true);

                            if (player.objectInRightHand != null)
                                Destroy(player.objectInRightHand);
                            player.UnequipObject();

                            player.EquipMultiTool(true);
                            Tutorial.instance.ListenForInputs("hasEquippedMutltitool");
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
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (currentlySelected != 1)
                {
                    currentlySelected = 1;
                    HotbarInput();
                }
                else
                {
                    if (multiToolHighlight != null && player.hasMultitool)
                    {
                        if (!player.isUsingMultiTool)
                        {
                            hotbarHighlights[currentlySelected].SetActive(false);
                            multiToolHighlight.SetActive(true);

                            if (player.objectInRightHand != null)
                                Destroy(player.objectInRightHand);
                            player.UnequipObject();

                            player.EquipMultiTool(true);
                            Tutorial.instance.ListenForInputs("hasEquippedMutltitool");
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
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (currentlySelected != 2)
                {
                    currentlySelected = 2;
                    HotbarInput();
                }
                else
                {
                    if (multiToolHighlight != null && player.hasMultitool)
                    {
                        if (!player.isUsingMultiTool)
                        {
                            hotbarHighlights[currentlySelected].SetActive(false);
                            multiToolHighlight.SetActive(true);

                            if (player.objectInRightHand != null)
                                Destroy(player.objectInRightHand);
                            player.UnequipObject();

                            player.EquipMultiTool(true);
                            Tutorial.instance.ListenForInputs("hasEquippedMutltitool");
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
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (currentlySelected != 3)
                {
                    currentlySelected = 3;
                    HotbarInput();
                }
                else
                {
                    if (multiToolHighlight != null && player.hasMultitool)
                    {
                        if (!player.isUsingMultiTool)
                        {
                            hotbarHighlights[currentlySelected].SetActive(false);
                            multiToolHighlight.SetActive(true);

                            if (player.objectInRightHand != null)
                                Destroy(player.objectInRightHand);
                            player.UnequipObject();

                            player.EquipMultiTool(true);
                            Tutorial.instance.ListenForInputs("hasEquippedMutltitool");
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
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (currentlySelected != 4)
                {
                    currentlySelected = 4;
                    HotbarInput();
                }
                else
                {
                    if (multiToolHighlight != null && player.hasMultitool)
                    {
                        if (!player.isUsingMultiTool)
                        {
                            hotbarHighlights[currentlySelected].SetActive(false);
                            multiToolHighlight.SetActive(true);

                            if (player.objectInRightHand != null)
                                Destroy(player.objectInRightHand);
                            player.UnequipObject();

                            player.EquipMultiTool(true);
                            Tutorial.instance.ListenForInputs("hasEquippedMutltitool");
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
            else if (Input.GetKeyDown(KeyCode.LeftAlt) && player.hasMultitool)
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
                        Tutorial.instance.ListenForInputs("hasEquippedMutltitool");
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
            else if (Input.GetKeyDown(KeyCode.B) && player.hasMultitool)
            {
                if (multiToolHighlight != null && player.isInBase)
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

    void HotbarInput()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.hotbar, gameObject);

        if (player.isUsingMultiTool)
        {
            multiToolHighlight.SetActive(false);
            player.EquipMultiTool(false);
        }

        if (grids[currentlySelected].GetItem(0, 0) != null)
            SwitchItem();
        else
        {
            if (player.objectInRightHand != null)
                Destroy(player.objectInRightHand);
            player.UnequipObject();
        }


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
