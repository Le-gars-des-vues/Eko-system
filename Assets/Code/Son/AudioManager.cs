using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Ambient Noises")]
    public AK.Wwise.RTPC soundtrackRTPC;
    [SerializeField] [Range(0, 30)] float rtpcValue;
    public AK.Wwise.Event forestSountrack;
    public bool forestIsPlaying;
    uint playingID;

    [Header("UI Sounds")]
    public AK.Wwise.Event inventaireOuvrir;
    public AK.Wwise.Event inventaireSwap;
    public AK.Wwise.Event inventairePickUp;
    public AK.Wwise.Event inventairePlace;

    public AK.Wwise.Event craftingOuvrir;
    public AK.Wwise.Event craftingFermer;

    public AK.Wwise.Event carteOuvrir;
    public AK.Wwise.Event upgradeOuvrir;

    public AK.Wwise.Event uiFermer;

    public AK.Wwise.Event uiTexte;

    [Header("Exploration Sound")]
    public AK.Wwise.Event pickUp;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Update()
    {
        soundtrackRTPC.SetValue(gameObject, rtpcValue);
    }

    public void StopSoundtrack()
    {
        AkSoundEngine.StopPlayingID(playingID);
    }

    public void PlaySoundtrack(AK.Wwise.Event myEvent)
    {
        AkSoundEngine.StopPlayingID(playingID);
        playingID = myEvent.Post(gameObject);
    }

    public void PlaySound(AK.Wwise.Event myEvent, GameObject obj)
    {
        myEvent.Post(obj);
    }
}
