using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RepairStation : MonoBehaviour
{
    [SerializeField] ItemGrid repairSlot;
    [SerializeField] ItemGrid gridInventaire;
    [SerializeField] ItemGrid gridStorage;
    [SerializeField] CraftingManager crafting;

    private int mat1Quant;
    private int mat2Quant;
    private int mat3Quant;

    [SerializeField] TextMeshProUGUI mat1Nom;
    [SerializeField] TextMeshProUGUI nombreMat1;
    [SerializeField] Image imageMat1;
    [SerializeField] TextMeshProUGUI mat2Nom;
    [SerializeField] TextMeshProUGUI nombreMat2;
    [SerializeField] Image imageMat2;
    [SerializeField] TextMeshProUGUI mat3Nom;
    [SerializeField] TextMeshProUGUI nombreMat3;
    [SerializeField] Image imageMat3;

    private float compteurItem1;
    private float compteurItem2;
    private float compteurItem3;

    private bool mat1Complete;
    private bool mat2Complete;
    private bool mat3Complete;

    bool weaponPlaced = false;

    [SerializeField] int requiredRepairQuant = 1;

    // Update is called once per frame
    void Update()
    {
        if (repairSlot.GetItem(0, 0) != null && !weaponPlaced)
        {
            weaponPlaced = true;
            RepairCheck();
            int index = GetRecipeIndexByName(repairSlot.GetItem(0, 0).itemData.itemName);
            mat1Nom.text = Recipes.listOfRecipes[index].firstMaterial;
            nombreMat1.text = mat1Quant.ToString() + " / " + requiredRepairQuant;
            imageMat1.color = new Color(1, 1, 1, 1);
            imageMat1.sprite = GetItemDataByName(Recipes.listOfRecipes[index].firstMaterial).itemIcon;

            mat2Nom.text = Recipes.listOfRecipes[index].secondMaterial;
            if (Recipes.listOfRecipes[index].secondMatQuantity == null)
            {
                nombreMat2.text = null;
                imageMat2.color = new Color(1, 1, 1, 0);
            }
            else
            {
                nombreMat2.text = mat2Quant.ToString() + " / " + requiredRepairQuant;
                imageMat2.sprite = GetItemDataByName(Recipes.listOfRecipes[index].secondMaterial).itemIcon;
                imageMat2.color = new Color(1, 1, 1, 1);
            }

            mat3Nom.text = Recipes.listOfRecipes[index].thirdMaterial;
            if (Recipes.listOfRecipes[index].thirdMatQuantity == null)
            {
                nombreMat3.text = null;
                imageMat3.color = new Color(1, 1, 1, 0);
            }
            else
            {
                nombreMat3.text = mat3Quant.ToString() + " / " + requiredRepairQuant;
                imageMat3.sprite = GetItemDataByName(Recipes.listOfRecipes[index].thirdMaterial).itemIcon;
                imageMat3.color = new Color(1, 1, 1, 1);
            }
        }
        else if (repairSlot.GetItem(0, 0) == null && weaponPlaced)
        {
            weaponPlaced = false;
            mat1Nom = null;
            mat2Nom = null;
            mat3Nom = null;
            nombreMat1.text = null;
            imageMat1.color = new Color(1, 1, 1, 0);
            nombreMat2.text = null;
            imageMat2.color = new Color(1, 1, 1, 0);
            nombreMat3.text = null;
            imageMat3.color = new Color(1, 1, 1, 0);
        }
    }

    public void Repair()
    {
        if (repairSlot.GetItem(0, 0).currentDurability == repairSlot.GetItem(0, 0).maxDurability) return;

        if (mat1Complete && mat2Complete && mat3Complete)
        {
            repairSlot.GetItem(0, 0).currentDurability = repairSlot.GetItem(0, 0).maxDurability;

            int i1 = 0;
            int i2 = 0;
            int i3 = 0;

            float tempHeight = gridInventaire.GetGridSizeHeight();
            float tempWidth = gridInventaire.GetGridSizeWidth();
            InventoryItem anItem;
            int index = GetRecipeIndexByName(repairSlot.GetItem(0, 0).itemData.itemName);

            for (int x = 0; x < tempWidth; x++)
            {
                for (int y = 0; y < tempHeight; y++)
                {
                    anItem = gridInventaire.CheckIfItemPresent(x, y);
                    if (anItem != null)
                    {
                        if (anItem.itemData.itemName == Recipes.listOfRecipes[index].firstMaterial && i1 < requiredRepairQuant && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            anItem.Delete();
                            compteurItem1--;
                            i1++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                        else if (anItem.itemData.itemName == Recipes.listOfRecipes[index].secondMaterial && i2 < requiredRepairQuant && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            anItem.Delete();
                            compteurItem2--;
                            i2++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                        else if (anItem.itemData.itemName == Recipes.listOfRecipes[index].thirdMaterial && i3 < requiredRepairQuant && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            anItem.Delete();
                            compteurItem3--;
                            i3++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                    }
                }
            }
            if (GameObject.Find("Player").GetComponent<PlayerPermanent>().CanOpenStorage())
            {
                float tempHeightStorage = gridStorage.GetGridSizeHeight();
                float tempWidthStorage = gridStorage.GetGridSizeWidth();

                for (int x = 0; x < tempWidthStorage; x++)
                {
                    for (int y = 0; y < tempHeightStorage; y++)
                    {
                        anItem = gridStorage.CheckIfItemPresent(x, y);
                        if (anItem != null)
                        {
                            if (anItem.itemData.itemName == Recipes.listOfRecipes[index].firstMaterial && i1 < requiredRepairQuant && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                            {
                                anItem.Delete();
                                compteurItem1--;
                                i1++;
                                anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                            }
                            else if (anItem.itemData.itemName == Recipes.listOfRecipes[index].secondMaterial && i2 < requiredRepairQuant && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                            {
                                anItem.Delete();
                                compteurItem2--;
                                i2++;
                                anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                            }
                            else if (anItem.itemData.itemName == Recipes.listOfRecipes[index].thirdMaterial && i3 < requiredRepairQuant && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                            {
                                anItem.Delete();
                                compteurItem3--;
                                i3++;
                                anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                            }
                        }
                    }
                }
            }
            AudioManager.instance.PlaySound(AudioManager.instance.craftingWeapon, gameObject);
        }
        RepairCheck();
    }

    public void RepairCheck()
    {

        compteurItem1 = 0;
        compteurItem2 = 0;
        compteurItem3 = 0;

        mat1Complete = false;
        mat2Complete = false;
        mat3Complete = false;


        float tempHeight = gridInventaire.GetGridSizeHeight();
        float tempWidth = gridInventaire.GetGridSizeWidth();

        InventoryItem anItem;
        int index = GetRecipeIndexByName(repairSlot.GetItem(0, 0).itemData.itemName);

        for (int x = 0; x < tempWidth; x++)
        {
            for (int y = 0; y < tempHeight; y++)
            {
                anItem = gridInventaire.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                    if (anItem.itemData.itemName == Recipes.listOfRecipes[index].firstMaterial)
                        compteurItem1 += 1f / (anItem.itemData.width * anItem.itemData.height);
                    else if (anItem.itemData.itemName == Recipes.listOfRecipes[index].secondMaterial)
                        compteurItem2 += 1f / (anItem.itemData.width * anItem.itemData.height);
                    else if (anItem.itemData.itemName == Recipes.listOfRecipes[index].thirdMaterial)
                        compteurItem3 += 1f / (anItem.itemData.width * anItem.itemData.height);
                }

            }
        }
        if (GameObject.Find("Player").GetComponent<PlayerPermanent>().CanOpenStorage())
        {
            float tempHeightStorage = gridStorage.GetGridSizeHeight();
            float tempWidthStorage = gridStorage.GetGridSizeWidth();

            for (int x = 0; x < tempWidthStorage; x++)
            {
                for (int y = 0; y < tempHeightStorage; y++)
                {
                    anItem = gridStorage.CheckIfItemPresent(x, y);
                    if (anItem != null)
                    {
                        if (anItem.itemData.itemName == Recipes.listOfRecipes[index].firstMaterial)
                            compteurItem1 += 1f / (anItem.itemData.width * anItem.itemData.height);
                        else if (anItem.itemData.itemName == Recipes.listOfRecipes[index].secondMaterial)
                            compteurItem2 += 1f / (anItem.itemData.width * anItem.itemData.height);
                        else if (anItem.itemData.itemName == Recipes.listOfRecipes[index].thirdMaterial)
                            compteurItem3 += 1f / (anItem.itemData.width * anItem.itemData.height);
                    }
                }
            }
        }
        mat1Quant = Mathf.RoundToInt(compteurItem1);
        mat2Quant = Mathf.RoundToInt(compteurItem2);
        mat2Quant = Mathf.RoundToInt(compteurItem3);
        if (Mathf.CeilToInt(compteurItem1) >= Recipes.listOfRecipes[index].firstMatQuantity)
        {
            mat1Complete = true;
        }
        if (Mathf.CeilToInt(compteurItem2) >= Recipes.listOfRecipes[index].secondMatQuantity || Recipes.listOfRecipes[index].secondMatQuantity == null)
        {
            mat2Complete = true;
        }
        if (Mathf.CeilToInt(compteurItem3) >= Recipes.listOfRecipes[index].thirdMatQuantity || Recipes.listOfRecipes[index].thirdMatQuantity == null)
        {
            mat3Complete = true;
        }
    }

    ItemData GetItemDataByName(string name)
    {
        foreach (ItemData item in crafting.ressources)
        {
            if (name == item.itemName)
            {
                return item;
            }
        }
        return null;
    }

    int GetRecipeIndexByName(string name)
    {
        foreach (KeyValuePair<int, Recipes> recipe in Recipes.listOfRecipes)
        {
            if (recipe.Value.recipeResult == name)
            {
                return recipe.Key;
            }
        }
        return 0;
    }
}
