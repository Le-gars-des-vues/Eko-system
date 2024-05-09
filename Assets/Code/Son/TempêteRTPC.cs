using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempeteRTPC : MonoBehaviour
{
    public AK.Wwise.RTPC tempete;
    [Range(0, 120)] public float rtpcValue;
    public AK.Wwise.RTPC tempeteBase;
    [Range(0, 100)] public float rtpcValue2;
    private void Update()
    {
        tempete.SetValue(gameObject, rtpcValue);
        tempeteBase.SetValue(gameObject, rtpcValue2);
    }
}
