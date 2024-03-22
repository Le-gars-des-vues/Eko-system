using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundFade : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    [SerializeField] bool isParallax;
    PlayerPermanent player;

    private float desiredAlpha;
    private float currentAlpha;
    [SerializeField] float fadeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        currentAlpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
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