using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem smokeEffect;
    [SerializeField] CraftingSystem crafting;
    public void Smoke()
    {
        smokeEffect.gameObject.SetActive(true);
        StartCoroutine(GlowCraftedItem());
    }

    public void ShowCraftedItems()
    {
        smokeEffect.gameObject.SetActive(false);
    }

    IEnumerator GlowCraftedItem()
    {
        //do stuff
        Craft();
        yield return new WaitForSeconds(1);
    }

    public void Craft()
    {
        crafting.Craft();
    }
}
