using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    [SerializeField] float maxHp;
    [SerializeField] float currentHp;

    [Header("Flash White Variables")]
    public bool isInvincible;
    [SerializeField] float flashWhiteDuration;
    [SerializeField] Material flashMaterial;
    [SerializeField] private List<SpriteRenderer> creatureGFX = new List<SpriteRenderer>();
    [SerializeField] private List<Material> ogMaterials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void LoseHealth(float value)
    {
        if (!isInvincible)
        {
            currentHp -= value;
            StartCoroutine(FlashWhite(creatureGFX, flashWhiteDuration));
        }
    }

    private IEnumerator FlashWhite(List<SpriteRenderer> spriteList, float duration)
    {
        foreach (var sprite in spriteList)
        {
            ogMaterials.Add(sprite.material);
            sprite.material = flashMaterial;
        }
        isInvincible = true;
        yield return new WaitForSecondsRealtime(duration);
        int i = 0;
        foreach (var sprite in spriteList)
        {
            sprite.material = ogMaterials[i];
            i++;
        }
        isInvincible = false;
    }
}
