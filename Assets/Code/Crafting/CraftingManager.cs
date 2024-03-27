using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public int currentRecipe;
    private int mat1Quant;
    private int mat2Quant;
    private int mat3Quant;

    public TextMeshProUGUI mat1Nom;
    public TextMeshProUGUI nombreMat1;
    public TextMeshProUGUI mat2Nom;
    public TextMeshProUGUI nombreMat2;
    public TextMeshProUGUI mat3Nom;
    public TextMeshProUGUI nombreMat3;

    public GameObject theCraftingSystem;
    public TMP_Dropdown dropdown;

    [SerializeField] TextMeshProUGUI craftingName;
    [SerializeField] TextMeshProUGUI craftingDesc;
    [SerializeField] Image craftingImage;

    [SerializeField] ItemGrid gridInventaire;
    [SerializeField] ItemGrid gridStorage;

    public List<KeyValuePair<int, Recipes>> knownRecipes = new List<KeyValuePair<int, Recipes>>();

    private void Start()
    {
        mat1Quant = 0;
        mat2Quant = 0;
        mat3Quant = 0;
    }
    void Awake()
    {
        dropdown = this.gameObject.GetComponent<TMP_Dropdown>();
        /*
        for (int i = 0; i < Recipes.listOfRecipes.Count; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData {text = Recipes.listOfRecipes[i].recipeResult });
            currentRecipe = 0;
        }
        */
    }
    public void SetMat1(int leMat)
    {
        mat1Quant = leMat;
    }
    public int GetMat1()
    {
        return mat1Quant;
    }
    public void SetMat2(int leMat)
    {
        mat2Quant = leMat;
    }
    public int? GetMat2()
    {
        return mat2Quant;
    }
    public void SetMat3(int leMat)
    {
        mat3Quant = leMat;
    }
    public int? GetMat3()
    {
        return mat3Quant;
    }
    private void FixedUpdate()
    {
        changementInfo();
        CheckIngredients();
    }
    public void changementInfo()
    {
        if (dropdown.options.Count > 0)
        {
            currentRecipe = dropdown.value;
            if (currentRecipe == 0)
            {
                craftingName.text = dropdown.options[currentRecipe].text;
                craftingImage.sprite = Camera.main.GetComponent<InventoryController>().multitool;
                craftingDesc.text = "Repair your multitool!";
            }
            else
            {
                craftingName.text = dropdown.options[currentRecipe].text;
                craftingImage.sprite = Camera.main.GetComponent<InventoryController>().craftables[knownRecipes[currentRecipe].Key].itemIcon;
                craftingDesc.text = Camera.main.GetComponent<InventoryController>().craftables[knownRecipes[currentRecipe].Key].description;
            }

            theCraftingSystem.GetComponent<CraftingSystem>().CraftCheck();

            mat1Nom.text = Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].firstMaterial;
            nombreMat1.text = mat1Quant.ToString() + " / " + Recipes.listOfRecipes[currentRecipe].firstMatQuantity.ToString();

            mat2Nom.text = Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].secondMaterial;

            if (Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].secondMatQuantity == null)
            {
                nombreMat2.text = null;
            }
            else
            {
                nombreMat2.text = mat2Quant.ToString() + " / " + Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].secondMatQuantity.ToString();
            }

            mat3Nom.text = Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].thirdMaterial;
            if (Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].thirdMatQuantity == null)
            {
                nombreMat3.text = null;
            }
            else
            {
                nombreMat3.text = mat3Quant.ToString() + " / " + Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].thirdMatQuantity.ToString();
            }
        }
    }

    public void CheckIngredients()
    {
        foreach (KeyValuePair<int, Recipes> recipe in Recipes.listOfRecipes)
        {
            if (!knownRecipes.Contains(recipe))
            {
                bool firstMatPresent = false;
                bool secondMatPresent = false;
                bool thirdMatPresent = false;

                ItemGrid itemGrid = gridInventaire;
                float tempHeight = gridInventaire.GetGridSizeHeight();
                float tempWidth = gridInventaire.GetGridSizeWidth();

                for (int x = 0; x < tempWidth; x++)
                {

                    for (int y = 0; y < tempHeight; y++)
                    {

                        InventoryItem item = itemGrid.CheckIfItemPresent(x, y);
                        if (item != null)
                        {
                            if (item.itemData.itemName == recipe.Value.firstMaterial && !firstMatPresent)
                                firstMatPresent = true;

                            if (recipe.Value.secondMaterial != null && !secondMatPresent)
                            {
                                if (item.itemData.itemName == recipe.Value.secondMaterial)
                                    secondMatPresent = true;
                            }
                            else
                                secondMatPresent = true;

                            if (recipe.Value.thirdMaterial != null && !thirdMatPresent)
                            {
                                if (item.itemData.itemName == recipe.Value.thirdMaterial)
                                    thirdMatPresent = true;
                            }
                            else
                                thirdMatPresent = true;
                        }
                    }
                }

                itemGrid = gridStorage;
                tempHeight = gridStorage.GetGridSizeHeight();
                tempWidth = gridStorage.GetGridSizeWidth();

                for (int x = 0; x < tempWidth; x++)
                {

                    for (int y = 0; y < tempHeight; y++)
                    {

                        InventoryItem item = itemGrid.CheckIfItemPresent(x, y);
                        if (item != null)
                        {
                            if (item.itemData.itemName == recipe.Value.firstMaterial && !firstMatPresent)
                                firstMatPresent = true;

                            if (recipe.Value.secondMaterial != null && !secondMatPresent)
                            {
                                if (item.itemData.itemName == recipe.Value.secondMaterial)
                                    secondMatPresent = true;
                            }
                            else
                                secondMatPresent = true;

                            if (recipe.Value.thirdMaterial != null && !thirdMatPresent)
                            {
                                if (item.itemData.itemName == recipe.Value.thirdMaterial)
                                    thirdMatPresent = true;
                            }
                            else
                                thirdMatPresent = true;
                        }
                    }
                }

                if (firstMatPresent && secondMatPresent && thirdMatPresent)
                {
                    knownRecipes.Add(recipe);
                    knownRecipes.Sort(new KeyValuePairComparer());
                    GetComponent<TMP_Dropdown>().options.Clear();
                    for (int i = 0; i < knownRecipes.Count; i++)
                    {
                        GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData { text = knownRecipes[i].Value.recipeResult });
                    }
                }
            }
        }
    }
}
public class KeyValuePairComparer : IComparer<KeyValuePair<int, Recipes>>
{
    public int Compare(KeyValuePair<int, Recipes> x, KeyValuePair<int, Recipes> y)
    {
        return x.Key.CompareTo(y.Key);
    }
}
