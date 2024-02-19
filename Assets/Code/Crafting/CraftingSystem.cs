using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private RectTransform inventaireCrafting;

    private int tempHeight;
    private int tempWidth;
    public float profit = 0;
    ItemGrid theItemGrid;
    InventoryItem anItem;

    [SerializeField] private int quota;
    [SerializeField] private TextMeshProUGUI quotaText;
    // Start is called before the first frame update
    public void VenteItem()
    {
        tempHeight = inventaireCrafting.GetComponent<ItemGrid>().GetGridSizeHeight();
        tempWidth = inventaireCrafting.GetComponent<ItemGrid>().GetGridSizeWidth();

        theItemGrid = inventaireCrafting.GetComponent<ItemGrid>();


        for (int x = 0; x < tempWidth; x++)
        {

            for (int y = 0; y < tempHeight; y++)
            {
                //Not working
                anItem = theItemGrid.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                    profit += (float)anItem.itemData.value / (anItem.itemData.width * anItem.itemData.height);
                    quotaText.text = profit.ToString() + " / " + quota.ToString();
                    anItem.Delete();
                    /*
                    for (int ix = x; ix < x + anItem.itemData.width - 1; ix++)
                    {
                        for (int iy = x; iy < y + anItem.itemData.height - 1; iy++)
                        {
                            
                        }
                    }
                    */
                }
            }
        }
    }
}
