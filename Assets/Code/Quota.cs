using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Quota : MonoBehaviour
{
    public int quota=0;
    public int profitActuels=0;
    public TextMeshProUGUI textQuota;


    private void Start()
    {
        textQuota.text = profitActuels + " / " + quota + "$";
    }
}
