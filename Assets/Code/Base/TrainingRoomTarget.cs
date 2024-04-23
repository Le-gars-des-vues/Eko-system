using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TrainingRoomTarget : MonoBehaviour
{
    bool isRevealed;
    [SerializeField] Robot robot;
    [SerializeField] SpriteRenderer sprite;
    Light2D light1;
    Light2D light2;

    [SerializeField] List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    private void Start()
    {
        sprite.material.SetFloat("_Transparency", 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spear" && !isRevealed)
        {
            isRevealed = true;
            StartCoroutine(Base.instance.Dissolve(sprites, 2f, false));
            sprite.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            light1.enabled = true;
            light2.enabled = true;
            robot.RevealLastRoom();
            Tutorial.instance.ListenForInputs("hasHitTarget");
        }
    }
}
