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
        Debug.Log("Smoked");
    }

    public void ShowCraftedItems()
    {
        smokeEffect.gameObject.SetActive(false);
        StartCoroutine(GlowCraftedItem());

    }

    IEnumerator GlowCraftedItem()
    {
        //do stuff
        yield return new WaitForSeconds(1);
        Craft();
    }

    public void Craft()
    {
        crafting.Craft();
    }
}
