using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
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
}
