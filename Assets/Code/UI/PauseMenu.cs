using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject controlTips;
    [SerializeField] GameObject PauseScreen;
    bool menuIsOpen;

    private void Start()
    {
        controlTips = transform.Find("controlTips").gameObject;
        PauseScreen = transform.Find("PauseScreen").gameObject;
        controlTips.SetActive(false);
        PauseScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuIsOpen)
            {
                PauseScreen.SetActive(true);
                menuIsOpen = true;
                Time.timeScale = 0;
            }
            else
            {
                PauseScreen.SetActive(false);
                menuIsOpen = false;
                Time.timeScale = 1;
            }
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("LoadMainMenu");
    }

    public void ShowControlTips()
    {
        controlTips.SetActive(true);
    }

    public void HideControlTips()
    {
        controlTips.SetActive(false);
    }
}
