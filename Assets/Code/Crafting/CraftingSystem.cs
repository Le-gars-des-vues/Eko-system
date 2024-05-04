using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private ItemGrid gridInventaire;
    [SerializeField] private ItemGrid gridStorage;

    private int tempHeight;
    private int tempWidth;

    private int tempHeightStorage;
    private int tempWidthStorage;

    InventoryItem anItem;

    private int laRecette;
    private float compteurItem1;
    private float compteurItem2;
    private float compteurItem3;

    private bool mat1Complete;
    private bool mat2Complete;
    private bool mat3Complete;

    public GameObject theController;
    [SerializeField] Animator craftingAnim;

    private void Start()
    {
        theController = Camera.main.gameObject;
    }

    public void CraftCheck()
    {
        
        compteurItem1 = 0; 
        compteurItem2 = 0;
        compteurItem3 = 0;

        mat1Complete = false;
        mat2Complete = false;
        mat3Complete = false;
        

        tempHeight = gridInventaire.GetGridSizeHeight();
        tempWidth = gridInventaire.GetGridSizeWidth();

        for (int x = 0; x < tempWidth; x++)
        {
            for (int y = 0; y < tempHeight; y++)
            {
               
                anItem = gridInventaire.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                    laRecette = GetComponent<CraftingManager>().knownRecipes[GetComponent<TMP_Dropdown>().value].Key;
                    if (anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].firstMaterial)
                        compteurItem1 += 1f / (anItem.itemData.width * anItem.itemData.height);
                    else if(anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].secondMaterial)
                        compteurItem2+= 1f / (anItem.itemData.width * anItem.itemData.height);
                    else if(anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].thirdMaterial)
                        compteurItem3 += 1f / (anItem.itemData.width * anItem.itemData.height);
                }
                
            }
        }
        if (GameObject.Find("Player").GetComponent<PlayerPermanent>().CanOpenStorage())
        {
            tempHeightStorage = gridStorage.GetGridSizeHeight();
            tempWidthStorage = gridStorage.GetGridSizeWidth();

            for (int x = 0; x < tempWidthStorage; x++)
            {
                for (int y = 0; y < tempHeightStorage; y++)
                {
                    anItem = gridStorage.CheckIfItemPresent(x, y);
                    if (anItem != null)
                    {

                        laRecette = GetComponent<CraftingManager>().knownRecipes[GetComponent<TMP_Dropdown>().value].Key;
                        if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].firstMaterial)
                            compteurItem1 += 1f / (anItem.itemData.width * anItem.itemData.height);
                        else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].secondMaterial)
                            compteurItem2 += 1f / (anItem.itemData.width * anItem.itemData.height);
                        else if (anItem.itemData.itemName == Recipes.listOfBasePods[laRecette].thirdMaterial)
                            compteurItem3 += 1f / (anItem.itemData.width * anItem.itemData.height);
                    }
                }
            }
        }
        GetComponent<CraftingManager>().SetMat1(Mathf.RoundToInt(compteurItem1));
        GetComponent<CraftingManager>().SetMat2(Mathf.RoundToInt(compteurItem2));
        GetComponent<CraftingManager>().SetMat3(Mathf.RoundToInt(compteurItem3));
        if (Mathf.CeilToInt(compteurItem1) >= Recipes.listOfRecipes[laRecette].firstMatQuantity)
        {
            mat1Complete = true;
        }
        if (Mathf.CeilToInt(compteurItem2) >= Recipes.listOfRecipes[laRecette].secondMatQuantity || Recipes.listOfRecipes[laRecette].secondMatQuantity==null)
        {
            mat2Complete = true;
        }
        if (Mathf.CeilToInt(compteurItem3) >= Recipes.listOfRecipes[laRecette].thirdMatQuantity || Recipes.listOfRecipes[laRecette].thirdMatQuantity == null)
        {
            mat3Complete = true;
        }
    }


    public void Craft()
    {
        if (mat1Complete && mat2Complete && mat3Complete)
        {
            int i1 = 0;
            int i2 = 0;
            int i3 = 0;
            for (int x = 0; x < tempWidth; x++)
            {
                for (int y = 0; y < tempHeight; y++)
                {
                    anItem = gridInventaire.CheckIfItemPresent(x, y);
                    if (anItem != null)
                    {
                        if (anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].firstMaterial && i1 < Recipes.listOfRecipes[laRecette].firstMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            anItem.Delete();
                            compteurItem1--;
                            i1++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                        else if (anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].secondMaterial && i2 < Recipes.listOfRecipes[laRecette].secondMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                        {
                            anItem.Delete();
                            compteurItem2--;
                            i2++;
                            anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                        }
                        else if (anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].thirdMaterial && i3 < Recipes.listOfRecipes[laRecette].thirdMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
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
                tempHeightStorage = gridStorage.GetGridSizeHeight();
                tempWidthStorage = gridStorage.GetGridSizeWidth();

                for (int x = 0; x < tempWidthStorage; x++)
                {
                    for (int y = 0; y < tempHeightStorage; y++)
                    {
                        anItem = gridStorage.CheckIfItemPresent(x, y);
                        if (anItem != null)
                        {
                            if (anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].firstMaterial && i1 < Recipes.listOfRecipes[laRecette].firstMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                            {
                                anItem.Delete();
                                compteurItem1--;
                                i1++;
                                anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                            }
                            else if (anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].secondMaterial && i2 < Recipes.listOfRecipes[laRecette].secondMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
                            {
                                anItem.Delete();
                                compteurItem2--;
                                i2++;
                                anItem.GetComponent<InventoryItem>().markedForDestroy = true;
                            }
                            else if (anItem.itemData.itemName == Recipes.listOfRecipes[laRecette].thirdMaterial && i3 < Recipes.listOfRecipes[laRecette].thirdMatQuantity && anItem.GetComponent<InventoryItem>().markedForDestroy == false)
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
            AudioManager.instance.PlaySound(AudioManager.instance.craftingSound, gameObject);
            theController.GetComponent<InventoryController>().CreateRecipeItem(laRecette, gameObject);
        }
        if (GetComponent<CraftingManager>().knownRecipes.Count > 0)
            CraftCheck();
    }

    public void CraftButton()
    {
        craftingAnim.SetTrigger("isCrafting");
    }
}
