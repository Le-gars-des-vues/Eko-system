using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipes
{
    public string recipeResult;

    public string firstMaterial;
    public int firstMatQuantity;

    public string secondMaterial;
    public int? secondMatQuantity;

    public string thirdMaterial;
    public int? thirdMatQuantity;

    public Recipes(string theResult, string theFirstMaterial, int theFirstQuantity) {
    
        recipeResult = theResult;
        firstMaterial = theFirstMaterial;
        firstMatQuantity = theFirstQuantity;

        secondMaterial = null;
        secondMatQuantity = null;

        thirdMaterial = null;
        thirdMatQuantity = null;
    
    }

    public Recipes(string theResult, string theFirstMaterial, int theFirstQuantity, string theSecondMaterial, int theSecondQuantity)
    {

        recipeResult = theResult;
        firstMaterial = theFirstMaterial;
        firstMatQuantity = theFirstQuantity;

        secondMaterial = theSecondMaterial;
        secondMatQuantity = theSecondQuantity;

        thirdMaterial = null;
        thirdMatQuantity = null;

    }

    public Recipes(string theResult, string theFirstMaterial, int theFirstQuantity, string theSecondMaterial, int theSecondQuantity, string theThirdMaterial, int theThirdQuantity)
    {

        recipeResult = theResult;
        firstMaterial = theFirstMaterial;
        firstMatQuantity = theFirstQuantity;

        secondMaterial = theSecondMaterial;
        secondMatQuantity = theSecondQuantity;

        thirdMaterial = theThirdMaterial;
        thirdMatQuantity = theThirdQuantity;

    }

    public static Dictionary<int,Recipes> listOfRecipes = new()
    {
        {0, new Recipes("Reparer le MultiTool", "Shockbulb", 3)},
        {1 , new Recipes("Diamond", "Bow", 2, "Potion", 1) },
        {2 , new Recipes("North Korea", "Diamond", 1, "Plutonium", 5,"Gun",2) }
    };
    public static Dictionary<int,Recipes> listOfBasePods = new()
    {
        {0 , new Recipes("Enclos", "Bow", 1) },
        {1 , new Recipes("Planter", "Crystal", 4, "Bone", 2)}
    };
}
