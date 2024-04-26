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
        {3, new Recipes("Battery", "Tugnstone", 2, "Wires",1,"Rubiol",1)},
        {4, new Recipes("Metal Sheet", "Tugnstone", 3)},
        {5, new Recipes("Wires", "Trunk", 1, "Roots", 2)},
        {6, new Recipes("Map Upgrade 1", "Microprocessor", 1, "Cured Leather", 2)},
        {7, new Recipes("Gathering Upgrade 1", "Microprocessor", 1, "Scale", 5)},
        {8, new Recipes("Combat Upgrade 1", "Microprocessor", 1, "Rubiol", 2, "Lushalite" , 2)},
        {9, new Recipes("Shield Upgrade 1", "Microprocessor", 1, "Battery", 1)},
        {10, new Recipes("Health Upgrade 1", "Microprocessor", 1, "Cured Leather", 1 , "Rubiol",2)},
        {11, new Recipes("Stamina Upgrade 1", "Microprocessor", 1, "Lushalite", 3, "Scale", 3)},
        {12, new Recipes("Fly Backpack", "Moss", 2, "Cured Leather", 2, "Fly DNA Vial", 3)},
        {13, new Recipes("Oxygen Mask", "Spit", 2, "Metal Sheet", 1, "Frog DNA Vial", 3)},
        {14, new Recipes("Heavy Punch", "Scale", 2, "Lushalite", 2, "Dog DNA Vial", 3)},
        {15, new Recipes("DNA Extractor", "Scale", 3, "Spit", 3, "Trunk", 3)},
        {16, new Recipes("Machete", "Lushalite", 5, "Metal Sheet", 1, "Bark", 2)},
        {17, new Recipes("Bow", "Bone", 3, "Roots", 3,"Spit",5)},
        {18, new Recipes("Trident", "Corallium", 5, "Fish Eyes", 1, "Petralucen", 1) },
        {19, new Recipes("Harpoon Gun", "Corallium", 3, "Scale", 3, "Trunk", 2) },
        {20, new Recipes("Arrow", "Lushalite",3, "Metal Sheet",1) },
        {21, new Recipes("Harpoon", "Corallium", 3, "Reinforced Plate", 1)},
        {22, new Recipes("Reinforced Plate", "Fin", 3)},
        {23, new Recipes("Signal Amplifier", "Aquabulbus", 3, "Battery",1,"Wires",1)},
        {24, new Recipes("Waterproof Chip", "Petralucen", 1, "Fin", 1, "Rubiol",2)},
        {25, new Recipes("Stretching Fibers", "Tentacle", 3, "Fish Eyes",1,"Trunk",2)},
        {26, new Recipes("Reinforced Plate", "Fin", 3)},
        {27, new Recipes("Lantern", "Petralucen", 3, "Metal Sheet", 1, "Fish DNA Vial",3)},
        {28, new Recipes("Map Tag", "Signal Amplifier", 1, "Rubiol", 2, "Petralucen",2)},
        {29, new Recipes("Map Upgrade 2", "Waterproof Chip", 1, "Stretching Fibers", 3)},
        {30, new Recipes("Gathering Upgrade 2", "Waterproof Chip", 1, "Signal Amplifier", 2)},
        {31, new Recipes("Combat Upgrade 2", "Waterproof Chip", 1, "Reinforced Plate", 2, "Petralucen",2)},
        {32, new Recipes("Shield Upgrade 2", "Waterproof Chip", 1, "Corallium", 3, "Tentacle",5)},
        {33, new Recipes("Health Upgrade 2", "Waterproof Chip", 1, "Corallium", 3, "Fish Eyes",2)},
        {34, new Recipes("Stamina Upgrade 2", "Waterproof Chip", 1, "Tentacle", 3, "Trunk",2)},
        /*
        {34, new Recipes("N.O.P.E. Laser Rifle", "Magnetic Core", 1, "Signal Amplifier", 3, "Battery",3)},
        {35, new Recipes("Stun Stick", "Leg", 3, "Battery", 2, "Wires",5)},
        {36, new Recipes("Power Cell", "Battery", 1, "Magnetyne", 3, "Fish Eyes",3)},
        {37, new Recipes("Magnetic Core", "Magnetyne", 5, "Lushalite", 5, "Rubiol",5)},
        {38, new Recipes("Quantum Chip", "Magnetic Core", 1, "Wires", 2, "Signal Amplifier",1)},
        {39, new Recipes("Map Upgrade 3", "Quantum Chip", 1, "Magnetyne", 3, "Leg",3)},
        {40, new Recipes("Gathering Upgrade 3", "Quantum Chip", 1, "Shroomies", 3, "Fish Eyes",2)},
        {41, new Recipes("Combat Upgrade 3", "Quantum Chip", 1, "Bone", 5, "Tentacle",3)},
        {42, new Recipes("Shield Upgrade 3", "Quantum Chip", 1, "Leg", 3,"Tugnstone",3)},
        {43, new Recipes("Health Upgrade 3", "Quantum Chip", 1, "Shroomies", 2, "Corallium",3)},
        {44, new Recipes("Stamina Upgrade 3", "Quantum Chip", 1, "Magnetyne", 3,"Bone",3)},
        */
    };
    public static Dictionary<int,Recipes> listOfBasePods = new()
    {
        /*
        {0, new Recipes("Storage Room", "Metal Sheet", 3, "Battery", 1,"Lushalite",5)},
        {1, new Recipes("Map Room", "Metal Sheet", 3, "Scale", 4, "Rubiol", 4) },
        {2, new Recipes("Enclosure Room", "Metal Sheet", 3, "Battery",1,"Cured Leather",2)},
        {3 ,new Recipes("Planter Room", "Metal Sheet", 3,"Battery", 1, "Rubiol",5)},
        {4, new Recipes("Teleporter Room", "Reinforced Plate", 3, "Signal Amplifier", 2, "Rubiol", 5)}
        */
        { 0, new Recipes("Storage Room", "Trunk", 1) },
        { 1, new Recipes("Map Room", "Trunk", 1) },
        { 2, new Recipes("Enclosure Room", "Trunk", 1) },
        { 3, new Recipes("Planter Room", "Trunk", 1) },
        { 4, new Recipes("Teleporter Room", "Trunk", 1) }
    };

    public static Dictionary<string, bool> discoveredRessources = new()
    {
        {"Aquabulbus", false},
        {"Fin", false},
        {"Bark", false},
        {"Battery", false},
        {"Bone", false},
        {"Corallium", false},
        {"Cured Leather", false},
        {"Fish Eyes", false},
        {"Leg", false},
        {"Lushalite", false},
        {"Magnetyne", false},
        {"Petralucen", false},
        {"Metal Sheet", false},
        {"Microprocessor", false},
        {"Reinforced Plate", false},
        {"Waterproof Chip", false},
        //{"Quantum Chip", false},
        {"Signal Amplifier", false},
        {"Stretching Fibers", false},
        //{"Magnetic Core", false},
        {"Moss", false},
        {"Roots", false},
        {"Rubiol", false},
        {"Scale", false},
        {"Spit", false},
        {"Trunk", false},
        {"Tugnstone", false},
        {"Wires", false},
        {"Tentacle", false},
        //{"Shroomies", false},
        {"Dog DNA Vial", false},
        {"Fly DNA Vial", false},
        {"Fish DNA Vial", false},
        {"Frog DNA Vial", false}
    };
}
