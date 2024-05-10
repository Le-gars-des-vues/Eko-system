using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urchin : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] List<Sprite> variants = new List<Sprite>();

    [SerializeField] StatusEffect poison;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = variants[Random.Range(0, variants.Count - 1)];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PlayerPermanent>().isInvincible)
            {
                if (collision.gameObject.GetComponent<PlayerPermanent>().poison != null)
                    collision.gameObject.GetComponent<PlayerPermanent>().StopPoison();
                collision.gameObject.GetComponent<PlayerPermanent>().poison = StartCoroutine(collision.gameObject.GetComponent<PlayerPermanent>().Poison(poison.effectDuration, poison.effectMagnitude, poison.effectFrequency));
            }
            collision.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, true, gameObject);
        }
    }
}
