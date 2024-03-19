using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        Forest_Test, 
        CharacterController,
        CameraAndHUD,
        Base,
    }

    public static void LoadAdditive(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
    }

    public static void MakeActiveScene(Scene scene)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
    }
}
