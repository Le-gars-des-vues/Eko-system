using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFarming : MonoBehaviour
{
    bool isActive = false;
    InventoryItem item;

    private void OnEnable()
    {
        item = GetComponent<InventoryItem>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        if (item.isPlaced && !isActive)
        {
            isActive = true;
            ActivateVisual(true);
        }
        else if (!item.isPlaced && isActive)
        {
            isActive = false;
            ActivateVisual(false);
        }
    }

    void ActivateVisual(bool activated)
    {
        if (activated)
        {
            switch
        }
        else
        {

        }
    }
    */
}
