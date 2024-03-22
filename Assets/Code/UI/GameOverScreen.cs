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
        player.GetComponent<PlayerPermanent>().ToggleRagdoll(false);
        player.transform.position = GameObject.Find("Base").transform.Find("Interior").transform.Find("FirstFloor").transform.Find("EntryPoint").transform.position;
        GameObject.Find("Base").GetComponent<Base>().isInside = true;
        player.GetComponent<GroundPlayerController>().enabled = true;
        player.GetComponent<PlayerPermanent>().enabled = true;
        player.GetComponent<IKManager2D>().enabled = true;
        player.GetComponent<PlayerPermanent>().Reset();
        player.GetComponent<PlayerPermanent>().ResetFeetPosition();
        player.GetComponent<PlayerPermanent>().isInBase = true;
        //gameOverScreen.SetActive(false);
    }
}
