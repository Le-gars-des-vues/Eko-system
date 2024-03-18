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
        {0, new Recipes("Reparer le MultiTool", "Roots", 1)},
        {1, new Recipes("Microprocessor", "Magnetyne", 2, "Roots", 2) },
        {2, new Recipes("Cured Leather", "Bark", 3)},
        {3, new Recipes("Battery", "Rubiol", 3)},
        {4, new Recipes("Metal Sheet", "Tugnstone", 3)},
        {5, new Recipes("Wires", "Trunk", 1, "Roots", 2)},
        {6, new Recipes("Utility Upgrade 1", "Microprocessor", 2, "Wires", 2)},
        {7, new Recipes("Gathering Upgrade 1", "Microprocessor", 2, "Wires", 2)},
        {8, new Recipes("Combat Upgrade 1", "Microprocessor", 2, "Wires", 2)},
        {9, new Recipes("Shield Upgrade 1", "Microprocessor", 2, "Wires", 2)},
        {10, new Recipes("Health Upgrade 1", "Microprocessor", 2, "Wires", 2)},
        {11, new Recipes("Stamina Upgrade 1", "Microprocessor", 2, "Wires", 2)},
    };
    public static Dictionary<int,Recipes> listOfBasePods = new()
    {
        {0 , new Recipes("Storage Room", "Trunk", 1, "Roots", 2)},
        {1 , new Recipes("Enclosure Room", "Trunk", 1)},
        {2 , new Recipes("Planter Room", "Trunk", 1)},
        {3 , new Recipes("Radar Room", "Trunk", 1)},
    };
}
