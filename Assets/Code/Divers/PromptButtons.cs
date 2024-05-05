using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptButtons : MonoBehaviour
{
    public void ButtonActivate()
    {
        PromptManager.instance.ButtonClick(true);
        AudioManager.instance.PlaySound(AudioManager.instance.yesButton, Camera.main.gameObject);
    }

    public void ButtonNull()
    {
        PromptManager.instance.ButtonClick(false);
        AudioManager.instance.PlaySound(AudioManager.instance.noButton, Camera.main.gameObject);
    }
}
