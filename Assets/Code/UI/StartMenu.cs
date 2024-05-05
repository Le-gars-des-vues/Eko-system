using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject cresdits;
    // Start is called before the first frame update
    void Start(){
        cresdits = transform.Find("cresdits").gameObject;
        cresdits.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
        Debug.Log("LoadMainScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit to Desktop");
    }

    public void ShowCredits()
    {
        cresdits.SetActive(true);
    }

    public void HideCredits()
    {
        cresdits.SetActive(false);
    }
}
