using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.SceneManagement;
public class DebugMenu : MonoBehaviour
{
    [SerializeField] GameObject sceneLoader;

    void Start()
    {
        sceneLoader = GameObject.Find("SceneLoader");
    }

    public void Bouton1()
    {
        sceneLoader.GetComponent<SceneLoader>().LoadForest1();
    }

    public void Bouton2()
    {
        sceneLoader.GetComponent<SceneLoader>().LoadSandbox();
    }

    public void Bouton3()
    {
        sceneLoader.GetComponent<SceneLoader>().LoadGym_1();
    }
}
