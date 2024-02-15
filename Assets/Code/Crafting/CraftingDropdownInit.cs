using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingDropdownInit : MonoBehaviour
{
    
    

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < Recipes.listOfRecipes.Count; i++)
        {
            this.gameObject.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData {text = Recipes.listOfRecipes[i].recipeResult });
        }
    }

    
}
