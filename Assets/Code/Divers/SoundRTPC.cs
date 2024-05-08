using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundRTPC : MonoBehaviour
{
    public AK.Wwise.RTPC RTPC;
    [SerializeField] [Range(0, 30)] float rtpcValue;
    private void Update()
    {
        RTPC.SetValue(gameObject, rtpcValue);
    }
}
