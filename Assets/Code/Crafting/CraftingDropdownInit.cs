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

    private void Start()
    {
        mat1Quant = 0;
        mat2Quant = 0;
        mat3Quant = 0;
    }
    void Awake()
    {
        for (int i = 0; i < Recipes.listOfRecipes.Count; i++)
        {
            this.gameObject.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData {text = Recipes.listOfRecipes[i].recipeResult });
            changementInfo();
        }
    }


    public void changementInfo()
    {
        currentRecipe = this.gameObject.GetComponent<TMP_Dropdown>().value;

        mat1Quant = 0;
        mat2Quant = 0;
        mat3Quant = 0;


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
