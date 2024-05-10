using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.SceneManagement;

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
                AudioManager.instance.PlaySound(AudioManager.instance.pauseMenuOpen, gameObject);
            }
            else
            {
                PauseScreen.SetActive(false);
                menuIsOpen = false;
                Time.timeScale = 1;
                AudioManager.instance.PlaySound(AudioManager.instance.pauseMenuClose, gameObject);
            }
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1;
        AkSoundEngine.StopAll();
        AudioManager.instance.PlaySound(AudioManager.instance.backToMainMenu, gameObject);
        //SceneManager.LoadScene("MainMenu");
        SceneLoader.instance.LoadMainMenu();
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
