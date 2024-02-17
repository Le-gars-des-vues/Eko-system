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
        HUD,
    }

    public static void LoadAdditive(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
    }
}
