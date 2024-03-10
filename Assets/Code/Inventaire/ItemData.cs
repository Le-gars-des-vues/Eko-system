using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Custom/ItemData")]
public class ItemData : ScriptableObject
{
    public int width=1;
    public int height = 1;

    public string itemName;

    public int initialWidth=1;
    public int initialHeight=1;

    [HideInInspector]
    public int hotbarWidth = 1;
    [HideInInspector]
    public int hotbarHeight = 1;

    public GameObject objectToSpawn;

    public Sprite itemIcon;

    public int value=1;

    public bool isUpgrade;
    public string scriptToAttach;

    //public bool markedForDestroy;

    private void OnEnable()
    {
        width = initialWidth;
        height = initialHeight;
       //markedForDestroy = false;
    }
}
