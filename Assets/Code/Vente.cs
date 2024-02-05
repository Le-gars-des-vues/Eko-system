using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vente : MonoBehaviour
{
    [SerializeField] private RectTransform inventaireVente;

    private int tempHeight;
    private int tempWidth;
    public int profit = 0;
    ItemGrid theItemGrid;
    InventoryItem anItem;

    public void VenteItem()
    {
        tempHeight=inventaireVente.GetComponent<ItemGrid>().GetGridSizeHeight();
        tempWidth=inventaireVente.GetComponent <ItemGrid>().GetGridSizeWidth();

        theItemGrid=inventaireVente.GetComponent<ItemGrid>();


         for (int x = 0; x < tempHeight; x++){

             for (int y = 0; y < tempWidth; y++)
             {
                //Not working
              anItem=theItemGrid.CheckIfItemPresent(x, y);
              if (anItem != null)
                 {

                    profit += anItem.itemData.value;
                    for (int ix = x; ix < x + anItem.itemData.width - 1; ix++)
                    {
                        for (int iy = x; iy < y + anItem.itemData.height - 1; iy++)
                        {
                            anItem.Delete();
                        }
                    }
                    
                 }
             }
         } 
    }
}
