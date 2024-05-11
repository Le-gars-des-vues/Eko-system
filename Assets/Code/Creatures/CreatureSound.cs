using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSound : MonoBehaviour
{
    public AK.Wwise.Bank soundBank;
    public AK.Wwise.Event idleSound;
    public AK.Wwise.Event stopIdle;
    public AK.Wwise.Event hurtSound;
    public AK.Wwise.Event jumpSound;
    public AK.Wwise.Event atkSound;
    public AK.Wwise.Event deathSound;
    public uint idleSoundID;
    public bool isWaterCreature;

    private void OnEnable()
    {
        if (!isWaterCreature)
            idleSoundID = idleSound.Post(gameObject);
    }

    private void OnDisable()
    {
        AkSoundEngine.StopPlayingID(idleSoundID);
    }
}
