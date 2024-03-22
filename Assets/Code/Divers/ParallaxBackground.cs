using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> sprites = new List<SpriteRenderer>();
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

        if (player.isInBase)
            desiredAlpha = 0f;
        else
            desiredAlpha = 1f;
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, currentAlpha);
        }
    }
}
