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
        {1, new Recipes("Microprocessor", "Tugnstone", 1, "Roots", 1, "Rubiol", 2) },
        {2, new Recipes("Cured Leather", "Bark", 1, "Moss", 2, "Spit", 1)},
        {3, new Recipes("Battery", "Tugnstone", 2, "Wires",1,"Rubiol",3)},
        {4, new Recipes("Metal Sheet", "Tugnstone", 3)},
        {5, new Recipes("Wires", "Trunk", 1, "Roots", 2)},
        {6, new Recipes("Utility Upgrade 1", "Microprocessor", 1, "Cured Leather", 2)},
        {7, new Recipes("Gathering Upgrade 1", "Microprocessor", 1, "Scale", 5)},
        {8, new Recipes("Combat Upgrade 1", "Microprocessor", 1, "Rubiol", 2, "Lushalite" , 2)},
        {9, new Recipes("Shield Upgrade 1", "Microprocessor", 1, "Battery", 1)},
        {10, new Recipes("Health Upgrade 1", "Microprocessor", 1, "Cured Leather", 1 , "Rubiol",1)},
        {11, new Recipes("Stamina Upgrade 1", "Microprocessor", 1, "Lushalite", 3, "Scale", 3)},
        {12, new Recipes("Fly Backpack", "Trunk", 1)},
        {13, new Recipes("Oxygen Mask", "Trunk", 1)},
        {14, new Recipes("Heavy Punch", "Trunk", 1)},
    };
    public static Dictionary<int,Recipes> listOfBasePods = new()
    {
        {0 , new Recipes("Storage Room", "Metal Sheet", 3, "Battery", 1,"Lushalite",5)},
        {1 , new Recipes("Map Room", "Metal Sheet", 3, "Scale",4,"Crystal",4) },
        {2 , new Recipes("Enclosure Room", "Metal Sheet", 3, "Battery",1,"Cured Leather",2)},
        {3, new Recipes("Planter Room", "Trunk", 1)},
        //{3 , new Recipes("Planter Room", "Metal Sheet", 3,"Battery",1,"Crystal",5)},
    };

    public static Dictionary<string, bool> discoveredRessources = new()
    {
        {"Bark", false},
        {"Battery", false},
        {"Cured Leather", false},
        {"Lushalite", false},
        {"Metal Sheet", false},
        {"Microprocessor", false},
        {"Moss", false},
        {"Roots", false},
        {"Rubiol", false},
        {"Scale", false},
        {"Spit", false},
        {"Trunk", false},
        {"Tugnstone", false},
        {"Wires", false}
    };
}
