using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoomDropdownInit : MonoBehaviour
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

    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomDesc;

    private void Start()
    {
        mat1Quant = 0;
        mat2Quant = 0;
        mat3Quant = 0;
    }
    void Awake()
    {
        for (int i = 0; i < Recipes.listOfBasePods.Count; i++)
        {
            this.gameObject.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData {text = Recipes.listOfBasePods[i].recipeResult });
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
        currentRecipe = this.gameObject.GetComponent<TMP_Dropdown>().value;

        theCraftingSystem.GetComponent<RoomCrafting>().RoomCraftCheck();

        roomName.text = Camera.main.GetComponent<InventoryController>().buildablesSide[currentRecipe].GetComponent<RoomInfo>().roomType;
        roomDesc.text = Camera.main.GetComponent<InventoryController>().buildablesSide[currentRecipe].GetComponent<RoomInfo>().roomDesc;

        mat1Nom.text = Recipes.listOfBasePods[currentRecipe].firstMaterial;
        nombreMat1.text = mat1Quant.ToString()+" / "+Recipes.listOfBasePods[currentRecipe].firstMatQuantity.ToString();

        mat2Nom.text = Recipes.listOfBasePods[currentRecipe].secondMaterial;
        
        if (Recipes.listOfBasePods[currentRecipe].secondMatQuantity == null)
        {
            nombreMat2.text = null;
        }
        else
        {
            nombreMat2.text = mat2Quant.ToString()+" / "+Recipes.listOfBasePods[currentRecipe].secondMatQuantity.ToString();
        }

        mat3Nom.text = Recipes.listOfBasePods[currentRecipe].thirdMaterial;
        if (Recipes.listOfBasePods[currentRecipe].thirdMatQuantity == null)
        {
            nombreMat3.text = null;
        }
        else
        {
            nombreMat3.text = mat3Quant.ToString()+" / "+Recipes.listOfBasePods[currentRecipe].thirdMatQuantity.ToString();
        }
    }
    
}
