using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject credits;
    [SerializeField] GameObject sceneLoader;
    [SerializeField] Animator introAnim;
    bool introPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        credits.SetActive(false);
        sceneLoader = GameObject.Find("SceneLoader");
    }

    public void PlayGame()
    {
        //SceneManager.LoadScene("MainScene");
        //loadingScreen.GetComponent<SceneLoader>().LoadGame();
        AudioManager.instance.PlaySound(AudioManager.instance.mainMenuPlay, gameObject);
        introAnim.SetTrigger("isIntro");
        introPlaying = true;
        Debug.Log("LoadMainScene");
    }

    public void QuitGame()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.mainMenuDesktop, gameObject);
        Application.Quit();
        Debug.Log("Quit to Desktop");
    }

    public void ShowCredits()
    {
        credits.SetActive(true);
    }

    public void HideCredits()
    {
        credits.SetActive(false);
    }
}
