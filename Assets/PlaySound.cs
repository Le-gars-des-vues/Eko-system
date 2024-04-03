using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public string eventName = "YourWwiseEventName";

    private void Start()
    {
        Play();
    }

    // Called when you want to play the sound
    public void Play()
    {
        // Trigger the Wwise Event
        AkSoundEngine.PostEvent(eventName, gameObject);
    }
}
