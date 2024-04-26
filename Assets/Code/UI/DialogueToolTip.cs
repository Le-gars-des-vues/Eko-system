using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogueToolTip : MonoBehaviour
{
    [SerializeField] List<GameObject> tooltips = new List<GameObject>();
    GameObject currentTooltip;

    private void OnEnable()
    {
        tooltips[1].SetActive(true);
        currentTooltip = tooltips[1];
    }

    public void ChangeTooltip(int index)
    {
        currentTooltip.SetActive(false);
        tooltips[index].SetActive(true);
        currentTooltip = tooltips[index];
    }

    private void OnDisable()
    {
        foreach (GameObject tooltip in tooltips)
        {
            tooltip.SetActive(false);
        }
    }
}
