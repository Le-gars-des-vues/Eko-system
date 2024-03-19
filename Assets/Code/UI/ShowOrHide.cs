using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOrHide : MonoBehaviour
{
    bool isShown = false;

    public void ShowOrHideInfo()
    {
        if (!isShown)
            GetComponent<Animator>().SetBool("isShown", true);
        else
            GetComponent<Animator>().SetBool("isShown", false);

        isShown = !isShown;
    }
}
