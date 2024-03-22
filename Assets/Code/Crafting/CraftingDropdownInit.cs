using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingDropdownInit : MonoBehaviour
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
    TMP_Dropdown dropdown;

    [SerializeField] TextMeshProUGUI craftingName;
    [SerializeField] TextMeshProUGUI craftingDesc;
    [SerializeField] Image craftingImage;

    private void Start()
    {
        mat1Quant = 0;
        mat2Quant = 0;
        mat3Quant = 0;
    }
    void Awake()
    {
        dropdown = this.gameObject.GetComponent<TMP_Dropdown>();
        for (int i = 0; i < Recipes.listOfRecipes.Count; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData {text = Recipes.listOfRecipes[i].recipeResult });
            currentRecipe = 0;
        }
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
    }
    public void changementInfo()
    {
        currentRecipe = dropdown.value;
        if (currentRecipe == 0)
        {
            craftingName.text = dropdown.options[dropdown.value].text;
            craftingImage.sprite = Camera.main.GetComponent<InventoryController>().multitool;
            craftingDesc.text = "Repair your multitool!";
        }
        else
        {
            craftingName.text = dropdown.options[dropdown.value].text;
            craftingImage.sprite = Camera.main.GetComponent<InventoryController>().craftables[dropdown.value - 1].itemIcon;
            craftingDesc.text = Camera.main.GetComponent<InventoryController>().craftables[dropdown.value - 1].description;
        }

        theCraftingSystem.GetComponent<CraftingSystem>().CraftCheck();

        mat1Nom.text = Recipes.listOfRecipes[currentRecipe].firstMaterial;
        nombreMat1.text = mat1Quant.ToString()+" / "+Recipes.listOfRecipes[currentRecipe].firstMatQuantity.ToString();

        mat2Nom.text = Recipes.listOfRecipes[currentRecipe].secondMaterial;
        
        if (Recipes.listOfRecipes[currentRecipe].secondMatQuantity == null)
        {
            nombreMat2.text = null;
        }
        else
        {
            nombreMat2.text = mat2Quant.ToString()+" / "+Recipes.listOfRecipes[currentRecipe].secondMatQuantity.ToString();
        }

        mat3Nom.text = Recipes.listOfRecipes[currentRecipe].thirdMaterial;
        if (Recipes.listOfRecipes[currentRecipe].thirdMatQuantity == null)
        {
            nombreMat3.text = null;
        }
        else
        {
            nombreMat3.text = mat3Quant.ToString()+" / "+Recipes.listOfRecipes[currentRecipe].thirdMatQuantity.ToString();
        }
    }
}
