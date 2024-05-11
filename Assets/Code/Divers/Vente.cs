using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.SceneManagement;

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
    [SerializeField] GameObject coinsEffect;
    [SerializeField] GameObject casinoCoins;
    [SerializeField] Animator casinoScreen;

    public TextMeshProUGUI profitText;
    public TextMeshPro profitTV;

    private void Awake()
    {
        SceneLoader.allScenesLoaded += StartScript;
    }

    private void StartScript()
    {
        profitTV = GameObject.Find("ProfitsTV").GetComponent<TextMeshPro>();
    }

    public void BouttonVente()
    {
        StartCoroutine(VenteItem());
        AudioManager.instance.PlaySound(AudioManager.instance.yesButton, gameObject);
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
                    profitTV.text = profit.ToString() + "$";
                    var coin = Instantiate(coinsEffect, anItem.transform.position, Quaternion.identity);
                    coin.GetComponent<RectTransform>().SetParent(inventaireVente);
                    anItem.Delete();
                    AudioManager.instance.PlaySound(AudioManager.instance.sellingScreenSell, Camera.main.gameObject);
                    Tutorial.instance.ListenForInputs("hasSoldItem");
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
        AudioManager.instance.PlaySound(AudioManager.instance.sellingScreenCasino, Camera.main.gameObject);
        casinoScreen.SetTrigger("isCasino");
        casinoCoins.SetActive(true);
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
