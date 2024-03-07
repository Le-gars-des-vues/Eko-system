using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConsummableEffects
{
    public float effectRange;
    public float effectMagnitude;
    public float effectCountdown;

    public void Heal(PlayerPermanent player, float healAmount)
    {
        player.ChangeHp(healAmount, false);
    }

    public void Flash(Transform transform, float flashRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, flashRadius, LayerMask.GetMask("Creature"));
        foreach (Collider2D creature in colliders)
        {
            if (creature.gameObject.GetComponent<CreatureState>() != null)
            {
                creature.gameObject.GetComponent<CreatureState>().isStunned = true;
            }
        }
    }
}
