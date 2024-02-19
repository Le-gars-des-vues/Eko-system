using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private RectTransform inventaireCrafting;

    private int tempHeight;
    private int tempWidth;
    ItemGrid theItemGrid;
    InventoryItem anItem;


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
    public void CraftCheck()
    {
        tempHeight = inventaireCrafting.GetComponent<ItemGrid>().GetGridSizeHeight();
        tempWidth = inventaireCrafting.GetComponent<ItemGrid>().GetGridSizeWidth();

        compteurItem1 = 0; 
        compteurItem2 = 0;
        compteurItem3 = 0;

        mat1Complete = false;
        mat2Complete = false;
        mat3Complete = false;
        

        theItemGrid = inventaireCrafting.GetComponent<ItemGrid>();


        for (int x = 0; x < tempWidth; x++)
        {

            for (int y = 0; y < tempHeight; y++)
            {
               
                anItem = theItemGrid.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                   
                    laRecette = theCraftingDropdownInit.GetComponent<TMP_Dropdown>().value;
                    if (anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].firstMaterial)
                    {
                        compteurItem1 += 1f / (anItem.itemData.width * anItem.itemData.height);
                        Debug.Log(compteurItem1);
                        theCraftingDropdownInit.GetComponent<CraftingDropdownInit>().SetMat1(Mathf.RoundToInt(compteurItem1));
                        anItem.itemData.markedForDestroy = true;
                    }
                    else if(anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].secondMaterial)
                    {
                        compteurItem2+= 1f / (anItem.itemData.width * anItem.itemData.height);
                        theCraftingDropdownInit.GetComponent<CraftingDropdownInit>().SetMat2(Mathf.RoundToInt(compteurItem2));
                        anItem.itemData.markedForDestroy = true;
                    }
                    else if(anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].thirdMaterial)
                    {
                        compteurItem3 += 1f / (anItem.itemData.width * anItem.itemData.height);
                        theCraftingDropdownInit.GetComponent<CraftingDropdownInit>().SetMat3(Mathf.RoundToInt(compteurItem3));
                        anItem.itemData.markedForDestroy = true;
                    }
                    
                    
                }
                
            }
        }
        if (theCraftingDropdownInit.GetComponent<CraftingDropdownInit>().GetMat1() >= Recipes.listOfRecipes[laRecette].firstMatQuantity)
        {
            mat1Complete = true;
        }
        if (theCraftingDropdownInit.GetComponent<CraftingDropdownInit>().GetMat2() >= Recipes.listOfRecipes[laRecette].secondMatQuantity || Recipes.listOfRecipes[laRecette].secondMatQuantity==null)
        {
            mat2Complete = true;
        }
        if (theCraftingDropdownInit.GetComponent<CraftingDropdownInit>().GetMat3() >= Recipes.listOfRecipes[laRecette].thirdMatQuantity || Recipes.listOfRecipes[laRecette].thirdMatQuantity == null)
        {
            mat3Complete = true;
        }
    }


    public void Craft()
    {
        if(mat1Complete && mat2Complete && mat3Complete)
        {
            theController.GetComponent<InventoryController>().CreateRecipeItem(laRecette);
        }
    }
}
