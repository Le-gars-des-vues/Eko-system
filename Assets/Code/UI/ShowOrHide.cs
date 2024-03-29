using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOrHide : MonoBehaviour
{
    Animator anim;
    bool isShowm;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnButtonClick()
    {
        if (!isShowm)
        {
            isShowm = true;
            anim.SetBool("isShown", true);
        }
        else
        {
            isShowm = false;
            anim.SetBool("isShown", false);
        }
    }
}
