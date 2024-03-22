using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SceneLoader.LoadAdditive(SceneLoader.Scene.World);
        SceneLoader.LoadAdditive(SceneLoader.Scene.CameraAndHUD);
        SceneLoader.LoadAdditive(SceneLoader.Scene.CharacterController);
        SceneLoader.LoadAdditive(SceneLoader.Scene.Base);
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " has loaded sucessfully");
    }
}
