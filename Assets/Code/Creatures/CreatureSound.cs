using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSound : MonoBehaviour
{
    public AK.Wwise.Bank soundBank;
    public AK.Wwise.Event idleSound;

    private void Start()
    {
        idleSound.Post(gameObject);
    }
}
