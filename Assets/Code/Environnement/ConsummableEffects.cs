using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsummableEffects
{
    public static void Heal(PlayerPermanent player, float healAmount)
    {
        player.ChangeHp(healAmount, false);
    }
}
