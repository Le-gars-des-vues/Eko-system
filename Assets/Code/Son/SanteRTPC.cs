using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanteRTPC : MonoBehaviour
{
    public AK.Wwise.RTPC sante;
    [Range(0, 100)] public float rtpcValue;
    private void Update()
    {
        sante.SetValue(gameObject, rtpcValue);
    }
}
