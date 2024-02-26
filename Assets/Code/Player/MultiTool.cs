using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTool : MonoBehaviour
{
    PlayerPermanent player;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerPermanent>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseMultiTool(bool isUsing)
    {
        if (isUsing)
        {
            player.isUsingMultiTool = true;
            sprite.enabled = true;
        }
        else
        {
            player.isUsingMultiTool = false;
            sprite.enabled = false;
        }
    }
}
