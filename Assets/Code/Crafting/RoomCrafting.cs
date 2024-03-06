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
    private int tempWidth;
    ItemGrid theItemGrid;
    InventoryItem anItem;

    private int farms=0;
    private int enclos=0;


    private int laRecette;
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
        

        theItemGrid = inventaireCrafting.GetComponent<ItemGrid>();
        tempHeight = inventaireCrafting.GetComponent<ItemGrid>().GetGridSizeHeight();
        tempWidth = inventaireCrafting.GetComponent<ItemGrid>().GetGridSizeWidth();

        for (int x = 0; x < tempWidth; x++)
        {

            for (int y = 0; y < tempHeight; y++)
            {
               
                anItem = theItemGrid.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                   
                    laRecette = theCraftingDropdownInit.GetComponent<TMP_Dropdown>().value;
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


    public void RoomCraft()
    {

        if (mat1Complete && mat2Complete && mat3Complete)
        {
            if (laRecette == 0)
            {
                enclos++;
            }
            else if(laRecette == 1)
            {
                farms++;
            }
            int i1 = 0;
            int i2 = 0;
            int i3 = 0;
            for (int x = 0; x < tempWidth; x++)
            {

                for (int y = 0; y < tempHeight; y++)
                {
                    
                    anItem = theItemGrid.CheckIfItemPresent(x, y);
                    if (anItem != null)
                    {
                        if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].firstMaterial && i1 < Recipes.listOfBasePods[laRecette].firstMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            anItem.Delete();
                            compteurItem1--;
                            i1++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                        else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].secondMaterial && i2 < Recipes.listOfBasePods[laRecette].secondMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            anItem.Delete();
                            compteurItem2--;
                            i2++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                        else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].thirdMaterial && i3 < Recipes.listOfBasePods[laRecette].thirdMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
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

        RoomCraftCheck();
    }
}
