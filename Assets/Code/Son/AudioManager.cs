using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public string forestSoundtrack = "AMB_Foret";
    public bool forestIsPlaying;

    uint playingID;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    void StopSoundtrack()
    {
        AkSoundEngine.StopPlayingID(playingID);
    }

    public void PlaySoundtrack(string eventName)
    {
        AkSoundEngine.StopPlayingID(playingID);
        playingID = AkSoundEngine.PostEvent(eventName, gameObject);
    }


}
