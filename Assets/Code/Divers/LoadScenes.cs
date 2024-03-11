using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SceneLoader.LoadAdditive(SceneLoader.Scene.Forest_Test);
        SceneLoader.LoadAdditive(SceneLoader.Scene.Base);
        SceneLoader.LoadAdditive(SceneLoader.Scene.CameraAndHUD);
        SceneLoader.LoadAdditive(SceneLoader.Scene.CharacterController);
    }
}
