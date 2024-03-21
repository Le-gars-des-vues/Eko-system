using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RoomCrafting : MonoBehaviour
{
    [SerializeField] private RectTransform inventaireCrafting;

    private int tempHeight;
    private int tempHeightStorage;
    private int tempWidth;
    private int tempWidthStorage;
    ItemGrid inventoryItemGrid;
    ItemGrid storageItemGrid;
    InventoryItem anItem;

    private int farms=0;
    private int enclos=0;


    public int laRecette;
    private float compteurItem1;
    private float compteurItem2;
    private float compteurItem3;

    private bool mat1Complete;
    private bool mat2Complete;
    private bool mat3Complete;

    public GameObject theCraftingDropdownInit;
    public GameObject theController;

    // Start is called before the first frame update

    public void SetFarm(int theFarms)
    {
        farms = theFarms;
    }

    public void SetEnclos(int theEnclos)
    {
        enclos = theEnclos;
    }
    public int GetFarms()
    {
        return farms;
    }
    public int GetEnclos()
    {
        return enclos;
    }
    public void RoomCraftCheck()
    {
        
        compteurItem1 = 0; 
        compteurItem2 = 0;
        compteurItem3 = 0;

        mat1Complete = false;
        mat2Complete = false;
        mat3Complete = false;
        

        inventoryItemGrid = inventaireCrafting.GetComponent<ItemGrid>();
        tempHeight = inventaireCrafting.GetComponent<ItemGrid>().GetGridSizeHeight();
        tempWidth = inventaireCrafting.GetComponent<ItemGrid>().GetGridSizeWidth();

        laRecette = theCraftingDropdownInit.GetComponent<TMP_Dropdown>().value;
        for (int x = 0; x < tempWidth; x++)
        {

            for (int y = 0; y < tempHeight; y++)
            {
               
                anItem = inventoryItemGrid.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                    if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].firstMaterial)
                    {
                        compteurItem1 += 1f / (anItem.itemData.width * anItem.itemData.height);
                       
                    }
                    else if(anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].secondMaterial)
                    {
                        compteurItem2+= 1f / (anItem.itemData.width * anItem.itemData.height);

                    }
                    else if(anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].thirdMaterial)
                    {
                        compteurItem3 += 1f / (anItem.itemData.width * anItem.itemData.height);
                        
                    }
                    
                    
                }
                
            }
        }
        if (GameObject.Find("Player").GetComponent<PlayerPermanent>().CanOpenStorage())
        {
            storageItemGrid = GameObject.Find("GridStorage").GetComponent<ItemGrid>();
            tempHeightStorage = storageItemGrid.GetGridSizeHeight();
            tempWidthStorage = storageItemGrid.GetGridSizeWidth();
            for (int x = 0; x < tempWidthStorage; x++)
            {

                for (int y = 0; y < tempHeightStorage; y++)
                {

                    anItem = storageItemGrid.CheckIfItemPresent(x, y);
                    if (anItem != null)
                    {

                        laRecette = theCraftingDropdownInit.GetComponent<TMP_Dropdown>().value;
                        if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].firstMaterial)
                        {
                            compteurItem1 += 1f / (anItem.itemData.width * anItem.itemData.height);

                        }
                        else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].secondMaterial)
                        {
                            compteurItem2 += 1f / (anItem.itemData.width * anItem.itemData.height);

                        }
                        else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].thirdMaterial)
                        {
                            compteurItem3 += 1f / (anItem.itemData.width * anItem.itemData.height);

                        }


                    }

                }
            }
        }
        theCraftingDropdownInit.GetComponent<RoomDropdownInit>().SetMat1(Mathf.RoundToInt(compteurItem1));
        theCraftingDropdownInit.GetComponent<RoomDropdownInit>().SetMat2(Mathf.RoundToInt(compteurItem2));
        theCraftingDropdownInit.GetComponent<RoomDropdownInit>().SetMat3(Mathf.RoundToInt(compteurItem3));
        if (Mathf.CeilToInt(compteurItem1) >= Recipes.listOfBasePods[laRecette].firstMatQuantity)
        {
            mat1Complete = true;
        }
        if (Mathf.CeilToInt(compteurItem2) >= Recipes.listOfBasePods[laRecette].secondMatQuantity || Recipes.listOfBasePods[laRecette].secondMatQuantity==null)
        {
            mat2Complete = true;
        }
        if (Mathf.CeilToInt(compteurItem3) >= Recipes.listOfBasePods[laRecette].thirdMatQuantity || Recipes.listOfBasePods[laRecette].thirdMatQuantity == null)
        {
            mat3Complete = true;
        }
    }

    public Ingredients RoomCraft()
    {
        if (mat1Complete && mat2Complete && mat3Complete)
        {
            Ingredients ingredientList = new Ingredients(laRecette);

            int i1 = 0;
            int i2 = 0;
            int i3 = 0;
            for (int x = 0; x < tempWidth; x++)
            {

                for (int y = 0; y < tempHeight; y++)
                {

                    anItem = inventoryItemGrid.CheckIfItemPresent(x, y);
                    if (anItem != null)
                    {
                        if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].firstMaterial && i1 < Recipes.listOfBasePods[laRecette].firstMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            if (ingredientList.firstMat == null)
                            {
                                ingredientList.firstMat = Instantiate(anItem.itemData);
                                ingredientList.firstMatQuantity = Recipes.listOfBasePods[laRecette].firstMatQuantity;
                            }
                            anItem.Delete();
                            compteurItem1--;
                            i1++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                        else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].secondMaterial && i2 < Recipes.listOfBasePods[laRecette].secondMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            if (ingredientList.secondMat == null)
                            {
                                ingredientList.secondMat = Instantiate(anItem.itemData);
                                ingredientList.secondMatQuantity = Recipes.listOfBasePods[laRecette].secondMatQuantity;
                            }
                            anItem.Delete();
                            compteurItem2--;
                            i2++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                        else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].thirdMaterial && i3 < Recipes.listOfBasePods[laRecette].thirdMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            if (ingredientList.thirdMat == null)
                            {
                                ingredientList.thirdMat = Instantiate(anItem.itemData);
                                ingredientList.thirdMatQuantity = Recipes.listOfBasePods[laRecette].thirdMatQuantity;
                            }
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
                for (int x = 0; x < tempWidthStorage; x++)
                {

                    for (int y = 0; y < tempHeightStorage; y++)
                    {

                        anItem = storageItemGrid.CheckIfItemPresent(x, y);
                        if (anItem != null)
                        {
                            if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].firstMaterial && i1 < Recipes.listOfBasePods[laRecette].firstMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                            {
                                if (ingredientList.firstMat == null)
                                {
                                    ingredientList.firstMat = Instantiate(anItem.itemData);
                                    ingredientList.firstMatQuantity = Recipes.listOfBasePods[laRecette].firstMatQuantity;
                                }
                                anItem.Delete();
                                compteurItem1--;
                                i1++;
                                anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                            }
                            else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].secondMaterial && i2 < Recipes.listOfBasePods[laRecette].secondMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                            {
                                if (ingredientList.secondMat == null)
                                {
                                    ingredientList.secondMat = Instantiate(anItem.itemData);
                                    ingredientList.secondMatQuantity = Recipes.listOfBasePods[laRecette].secondMatQuantity;
                                }
                                anItem.Delete();
                                compteurItem2--;
                                i2++;
                                anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                            }
                            else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].thirdMaterial && i3 < Recipes.listOfBasePods[laRecette].thirdMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                            {
                                if (ingredientList.thirdMat == null)
                                {
                                    ingredientList.thirdMat = Instantiate(anItem.itemData);
                                    ingredientList.thirdMatQuantity = Recipes.listOfBasePods[laRecette].thirdMatQuantity;
                                }
                                anItem.Delete();
                                compteurItem3--;
                                i3++;
                                anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                            }
                        }
                    }
                }
            }
            return ingredientList;
        }
        else
        {
            Debug.Log("Not enough materials!");
            return new Ingredients(-1);
        }
        //RoomCraftCheck();
    }
}

public struct Ingredients
{
    public int index;
    public ItemData firstMat;
    public ItemData secondMat;
    public ItemData thirdMat;

    public int firstMatQuantity;
    public int? secondMatQuantity;
    public int? thirdMatQuantity;

    public Ingredients(int _index, ItemData _firstMat = null, ItemData _secondMat = null, ItemData _thirdMat = null, int _firstMatQuantity = 0, int? _secondMatQuantity = 0, int? _thirdMatQuantity = 0)
    {
        index = _index;
        firstMat = _firstMat;
        secondMat = _secondMat;
        thirdMat = _thirdMat;

        firstMatQuantity = _firstMatQuantity;
        secondMatQuantity = _secondMatQuantity;
        thirdMatQuantity = _thirdMatQuantity;
    }
}
