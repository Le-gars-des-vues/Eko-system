using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceRTPC : MonoBehaviour
{
    public AK.Wwise.RTPC distance;
    [Range(0, 40)] public float rtpcValue;

    private void Update()
    {
        distance.SetValue(gameObject, rtpcValue);
    }
}
