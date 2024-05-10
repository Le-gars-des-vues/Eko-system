using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRTPC : MonoBehaviour
{
    public AK.Wwise.RTPC baseEtage;
    [Range(0, 100)] public float rtpcValue;
    private void Update()
    {
        baseEtage.SetValue(gameObject, rtpcValue);
    }
}
