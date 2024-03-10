using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Quota : MonoBehaviour
{
    public float quota;
    public GameObject gestionnaireVente;
    public TextMeshProUGUI textQuota;

    private void Start()
    {
        textQuota = GameObject.Find("QuotaText").GetComponent<TextMeshProUGUI>();
        textQuota.text = 0 + " / " + quota + "$";
        gestionnaireVente = GameObject.Find("Vente");
    }

    public float getQuota()
    {
        return quota;
    }

    public void nouveauQuota()
    {
        gestionnaireVente.GetComponent<Vente>().profit = 0;
        quota = (quota * 1.1f) + (gestionnaireVente.GetComponent<Vente>().calculStorage()*0.1f);
        quota = Mathf.RoundToInt(quota);
        textQuota.text = 0 + "/" + quota + "$";
    }
}
