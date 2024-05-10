using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepnessRTPC : MonoBehaviour
{
    public AK.Wwise.RTPC deepness;
    [Range(0, 30)] public float rtpcValue;
    private void Update()
    {
        deepness.SetValue(gameObject, rtpcValue);
    }
}
