using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespirationRTPC : MonoBehaviour
{
    public AK.Wwise.RTPC respiration;
    [Range(0, 100)] public float rtpcValue;
    private void Update()
    {
        respiration.SetValue(gameObject, rtpcValue);
    }
}
