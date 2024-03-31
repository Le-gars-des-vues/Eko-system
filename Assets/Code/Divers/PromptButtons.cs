using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptButtons : MonoBehaviour
{
    public void ButtonActivate()
    {
        PromptManager.instance.ButtonClick(true);
    }

    public void ButtonNull()
    {
        PromptManager.instance.ButtonClick(false);
    }
}
