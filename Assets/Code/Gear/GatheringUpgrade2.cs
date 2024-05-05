using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringUpgrade2 : MonoBehaviour
{
    bool isActive = false;
    InventoryItem item;

    private void OnEnable()
    {
        item = GetComponent<InventoryItem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (item.isPlaced && !isActive)
        {
            isActive = true;
            ActivateUpgrade(true);
        }
        else if (!item.isPlaced && isActive)
        {
            isActive = false;
            ActivateUpgrade(false);
        }
    }

    void ActivateUpgrade(bool activated)
    {
        GameManager.instance.player.hasMagneticRay = activated;
    }
}
