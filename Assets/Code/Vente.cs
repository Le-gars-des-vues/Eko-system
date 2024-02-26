using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Vente : MonoBehaviour
{
    [SerializeField] private RectTransform inventaireVente;
    [SerializeField] private RectTransform inventaireStorage;

    private int tempHeight;
    private int tempWidth;
    public float profit = 0;
    public float currentValue;
    ItemGrid theItemGrid;
    InventoryItem anItem;

    public GameObject quotaManager;
    [SerializeField] private TextMeshProUGUI quotaText;

    public void VenteItem()
    {
        tempHeight=inventaireVente.GetComponent<ItemGrid>().GetGridSizeHeight();
        tempWidth=inventaireVente.GetComponent <ItemGrid>().GetGridSizeWidth();

        theItemGrid=inventaireVente.GetComponent<ItemGrid>();


         for (int x = 0; x < tempWidth; x++){

             for (int y = 0; y < tempHeight; y++)
             {
              
              anItem=theItemGrid.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                    profit += (float)anItem.itemData.value / (anItem.itemData.width * anItem.itemData.height);
                    quotaText.text = profit.ToString() + " / " + quotaManager.GetComponent<Quota>().quota.ToString() + "$";
                    anItem.Delete();
                }
             }
         } 
    }

    public float calculStorage()
    {
        currentValue = 0;
        tempHeight = inventaireStorage.GetComponent<ItemGrid>().GetGridSizeHeight();
        tempWidth = inventaireStorage.GetComponent<ItemGrid>().GetGridSizeWidth();

        theItemGrid = inventaireStorage.GetComponent<ItemGrid>();


        for (int x = 0; x < tempWidth; x++)
        {

            for (int y = 0; y < tempHeight; y++)
            {
                
                anItem = theItemGrid.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                    currentValue += (float)anItem.itemData.value / (anItem.itemData.width * anItem.itemData.height);
                }
            }
        }
        return currentValue;
    }
}
