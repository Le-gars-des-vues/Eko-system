using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vente : MonoBehaviour
{
    [SerializeField] private RectTransform inventaireVente;

    private int tempHeight;
    private int tempWidth;
    public float profit = 0;
    ItemGrid theItemGrid;
    InventoryItem anItem;

    public void VenteItem()
    {
        tempHeight=inventaireVente.GetComponent<ItemGrid>().GetGridSizeHeight();
        tempWidth=inventaireVente.GetComponent <ItemGrid>().GetGridSizeWidth();

        theItemGrid=inventaireVente.GetComponent<ItemGrid>();


         for (int x = 0; x < tempWidth; x++){

             for (int y = 0; y < tempHeight; y++)
             {
                //Not working
              anItem=theItemGrid.CheckIfItemPresent(x, y);
              if (anItem != null)
                 {
                    profit += (float)anItem.itemData.value / (anItem.itemData.width * anItem.itemData.height);
                    Debug.Log(profit);
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
