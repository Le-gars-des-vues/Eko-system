using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public List<GameObject> itemsInInventory;

    public bool hasMultitool;

    //Default data when starting a new game
    public GameData()
    {
        this.hasMultitool = false;
        itemsInInventory = new List<GameObject>();
    }
}
