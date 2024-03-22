using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject background;
    [SerializeField] GameObject text;
    bool menuIsOpen;

    private void Start()
    {
        background = transform.Find("background").gameObject;
        text = transform.Find("text").gameObject;
        background.SetActive(false);
        text.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuIsOpen)
            {
                background.SetActive(true);
                text.SetActive(true);
                menuIsOpen = true;
                Time.timeScale = 0;
            }
            else
            {
                background.SetActive(false);
                text.SetActive(false);
                menuIsOpen = false;
                Time.timeScale = 1;
            }
        }
    }
}
