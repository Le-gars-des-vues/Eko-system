using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    public GameObject[] hotbars;
    public GameObject[] hotbarHighlights;

    private int currentlySelected;


    private void Start()
    {
        currentlySelected = 0;

        for (int i = 0; i < hotbars.Length; i++)
        {
            if (hotbars[i] == hotbars[currentlySelected])
            {
                hotbarHighlights[i].SetActive(true);
            }
            else
            {
                hotbarHighlights[i].SetActive(false);
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentlySelected--;
            if(currentlySelected < 0)
            {

                currentlySelected = hotbars.Length - 1;
            }

            for (int i = 0; i < hotbars.Length; i++)
            {
                if (hotbars[i] == hotbars[currentlySelected])
                {
                    hotbarHighlights[i].SetActive(true);
                }
                else
                {
                    hotbarHighlights[i].SetActive(false);
                }
            }
        }else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentlySelected++;
            if (currentlySelected == hotbars.Length)
            {

                currentlySelected = 0;
            }

            for (int i = 0; i < hotbars.Length; i++)
            {
                if (hotbars[i] == hotbars[currentlySelected])
                {
                    hotbarHighlights[i].SetActive(true);
                }
                else
                {
                    hotbarHighlights[i].SetActive(false);
                }
            }
        }


    }
}
