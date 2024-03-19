using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class GameOverScreen : MonoBehaviour
{
    public GameObject player;
    public GameObject gameOverScreen;

    public void BackToBase()
    {
        player.transform.position = GameObject.Find("Base").transform.Find("Interior").transform.Find("EntryPoint").transform.Find("FirstFloor").transform.position;
        player.GetComponent<PlayerPermanent>().Reset();
        player.GetComponent<GroundPlayerController>().enabled = true;
        player.GetComponent<PlayerPermanent>().enabled = true;
        player.GetComponent<IKManager2D>().enabled = true;
        player.GetComponent<PlayerPermanent>().ToggleRagdoll(false);
        //gameOverScreen.SetActive(false);
    }
}
