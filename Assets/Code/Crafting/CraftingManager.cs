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
    [SerializeField] Image imageMat1;
    public TextMeshProUGUI mat2Nom;
    public TextMeshProUGUI nombreMat2;
    [SerializeField] Image imageMat2;
    public TextMeshProUGUI mat3Nom;
    public TextMeshProUGUI nombreMat3;
    [SerializeField] Image imageMat3;

    public GameObject theCraftingSystem;
    public TMP_Dropdown dropdown;

    [SerializeField] TextMeshProUGUI craftingName;
    [SerializeField] TextMeshProUGUI craftingDesc;
    [SerializeField] Image craftingImage;
    [SerializeField] GameObject noRecipe;

    [SerializeField] ItemGrid gridInventaire;
    [SerializeField] ItemGrid gridStorage;

    [SerializeField] GameObject placeholder;

    public List<KeyValuePair<int, Recipes>> knownRecipes = new List<KeyValuePair<int, Recipes>>();
    public List<KeyValuePair<int, Recipes>> learnedRecipes = new List<KeyValuePair<int, Recipes>>();
    //float craftCheckTime;
    [SerializeField] float craftCheckInterval;
    public List<ItemData> ressources = new List<ItemData>();

    private void Start()
    {
        mat1Quant = 0;
        mat2Quant = 0;
        mat3Quant = 0;
    }
    void Awake()
    {
        dropdown = this.gameObject.GetComponent<TMP_Dropdown>();
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
    /*
    private void FixedUpdate()
    {
        if (Time.time - craftCheckTime > craftCheckInterval)
        {
            CheckIngredients();
        }
    }
    */

    public void OnValueChanged()
    {
        CheckIngredients();
        ChangementInfo();
    }

    public void ChangementInfo()
    {
        if (knownRecipes.Count > 0)
        {
            if (noRecipe.activeSelf)
            {
                noRecipe.SetActive(false);
                craftingImage.gameObject.SetActive(true);
            }
            if (dropdown.value >= knownRecipes.Count)
                dropdown.value--;
            currentRecipe = dropdown.value;
            if (currentRecipe >= 0)
            {
                if (knownRecipes[currentRecipe].Key == 0)
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
                nombreMat1.text = mat1Quant.ToString() + " / " + Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].firstMatQuantity.ToString();
                imageMat1.color = new Color(1, 1, 1, 1);
                imageMat1.sprite = GetItemDataByName(Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].firstMaterial).itemIcon;

                mat2Nom.text = Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].secondMaterial;
                if (Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].secondMatQuantity == null)
                {
                    nombreMat2.text = null;
                    imageMat2.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    nombreMat2.text = mat2Quant.ToString() + " / " + Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].secondMatQuantity.ToString();
                    imageMat2.sprite = GetItemDataByName(Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].secondMaterial).itemIcon;
                    imageMat2.color = new Color(1, 1, 1, 1);
                }

                mat3Nom.text = Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].thirdMaterial;
                if (Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].thirdMatQuantity == null)
                {
                    nombreMat3.text = null;
                    imageMat3.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    nombreMat3.text = mat3Quant.ToString() + " / " + Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].thirdMatQuantity.ToString();
                    imageMat3.sprite = GetItemDataByName(Recipes.listOfRecipes[knownRecipes[currentRecipe].Key].thirdMaterial).itemIcon;
                    imageMat3.color = new Color(1, 1, 1, 1);
                }
            }
            //craftCheckTime = Time.time;
            dropdown.options.Clear();
            for (int i = 0; i < knownRecipes.Count; i++)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData { text = knownRecipes[i].Value.recipeResult });
            }
            dropdown.RefreshShownValue();
        }
        else
        {
            noRecipe.SetActive(true);
            craftingImage.gameObject.SetActive(false);
            mat1Nom.text = "";
            mat2Nom.text = "";
            mat3Nom.text = "";
            nombreMat1.text = "";
            nombreMat2.text = "";
            nombreMat3.text = "";
            imageMat1.color = new Color(1, 1, 1, 0);
            imageMat2.color = new Color(1, 1, 1, 0);
            imageMat3.color = new Color(1, 1, 1, 0);
            craftingName.text = "";
            craftingDesc.text = "";
            dropdown.value = -1;
        }
    }

    public void CheckIngredients()
    {
        foreach (KeyValuePair<int, Recipes> recipe in Recipes.listOfRecipes)
        {
            if (recipe.Key == 0 && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasMultitool == true)
                continue;

            if (!learnedRecipes.Contains(recipe))
            {
                bool firstMatPresent = false;
                bool secondMatPresent = false;
                bool thirdMatPresent = false;

                if (recipe.Value.firstMaterial != null && !firstMatPresent)
                {
                    if (Recipes.discoveredRessources[recipe.Value.firstMaterial])
                        firstMatPresent = true;
                }
                else
                    firstMatPresent = true;

                if (recipe.Value.secondMaterial != null && !secondMatPresent)
                {
                    if (Recipes.discoveredRessources[recipe.Value.secondMaterial])
                        secondMatPresent = true;
                }
                else
                    secondMatPresent = true;

                if (recipe.Value.thirdMaterial != null && !thirdMatPresent)
                {
                    if (Recipes.discoveredRessources[recipe.Value.thirdMaterial])
                        thirdMatPresent = true;
                }
                else
                    thirdMatPresent = true;

                if (firstMatPresent && secondMatPresent && thirdMatPresent)
                {
                    learnedRecipes.Add(recipe);
                    knownRecipes.Add(recipe);
                    knownRecipes.Sort(new KeyValuePairComparer());
                    dropdown.options.Add(new TMP_Dropdown.OptionData { text = recipe.Value.recipeResult });
                }
            }
        }
    }

    ItemData GetItemDataByName(string name)
    {
        foreach (ItemData item in ressources)
        {
            if (name == item.itemName)
            {
                return item;
            }
        }
        return null;
    }
}

public class KeyValuePairComparer : IComparer<KeyValuePair<int, Recipes>>
{
    public int Compare(KeyValuePair<int, Recipes> x, KeyValuePair<int, Recipes> y)
    {
        return x.Key.CompareTo(y.Key);
    }
}
