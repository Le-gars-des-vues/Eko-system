using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

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

    [SerializeField] private TextMeshProUGUI profitText;
    [SerializeField] private TextMeshPro profitTV;
    [SerializeField] private Light2D tvLight;

    private void Start()
    {
        //profitTV = GameObject.Find("ProfitsTV").GetComponent<TextMeshPro>();
    }

    public void BouttonVente()
    {
        StartCoroutine(VenteItem());
    }

    IEnumerator VenteItem()
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
                    profit += anItem.itemData.value;
                    QuickMenu.instance.quotaText.text = profit.ToString() + "/" + GameManager.instance.gameObject.GetComponent<Quota>().quota.ToString() + "$";
                    profitText.text = profit.ToString() + "$";
                    GameObject.Find("ProfitsTV").GetComponent<TextMeshPro>().text = profit.ToString() + "$";
                    anItem.Delete();
                    AudioManager.instance.PlaySound(AudioManager.instance.sellingScreenSell, Camera.main.gameObject);
                    Tutorial.instance.ListenForInputs("hasSoldItem");
                    yield return new WaitForSeconds(0.5f);
                }
             }
         }
         yield return null;
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
