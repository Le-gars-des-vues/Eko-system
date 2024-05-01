using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class GameOverScreen : MonoBehaviour
{
    public GameObject player;
    public GameObject gameOverScreen;
    public Vector2 respawnPoint;

    private void Start()
    {
        respawnPoint = GameObject.Find("Base").transform.Find("Interior").transform.Find("FirstFloor").transform.Find("NewCycle").transform.position;
    }

    public void BackToBase()
    {
        GameManager.instance.Storm(false);
        AudioManager.instance.PlaySound(AudioManager.instance.backToBase, Camera.main.gameObject);
        player.GetComponent<PlayerPermanent>().enabled = true;
        player.GetComponent<PlayerPermanent>().ToggleRagdoll(false);
        player.GetComponent<GroundPlayerController>().enabled = true;

        Base.instance.Teleport(false, true, respawnPoint);
        player.GetComponent<PlayerPermanent>().Reset();
        //gameOverScreen.SetActive(false);
    }
}
