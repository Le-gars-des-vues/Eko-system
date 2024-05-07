using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.SceneManagement;

public class ForegroundFade : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    [SerializeField] bool isParallax;
    PlayerPermanent player;

    private float desiredAlpha;
    private float currentAlpha;
    [SerializeField] float fadeSpeed;


    private void Awake()
    {
        SceneLoader.allScenesLoaded += StartScript;
    }

    void StartScript()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        currentAlpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneLoader.instance.isLoading) return;

        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, fadeSpeed * Time.deltaTime);
        //Debug.Log(player.gameObject.transform.position.y - transform.position.y);

        if (isParallax)
        {
            if (player.gameObject.transform.position.y - transform.position.y < -3)
                desiredAlpha = 0f;
            else if (player.gameObject.transform.position.y - transform.position.y >= -3 && player.gameObject.GetComponent<GroundPlayerController>().enabled)
                desiredAlpha = 1f;
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, currentAlpha);
            }
        }
    }
}
