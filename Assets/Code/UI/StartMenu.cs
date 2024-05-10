using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.SceneManagement;

public class StartMenu : MonoBehaviour
{
    uint musicID;
    public uint introID;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject sceneLoader;
    public Animator introAnim;

    // Start is called before the first frame update
    void Start()
    {
        credits.SetActive(false);
        musicID = AudioManager.instance.PlaySound(AudioManager.instance.mainMenuMusic, gameObject);
        sceneLoader = GameObject.Find("SceneLoader");
    }

    public void PlayGame()
    {
        //SceneManager.LoadScene("MainScene");
        //loadingScreen.GetComponent<SceneLoader>().LoadGame();
        AudioManager.instance.PlaySound(AudioManager.instance.mainMenuPlay, gameObject);
        AkSoundEngine.StopPlayingID(musicID);
        introID = AudioManager.instance.PlaySound(AudioManager.instance.intro, sceneLoader);
        introAnim.SetBool("isIntro", true);
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
