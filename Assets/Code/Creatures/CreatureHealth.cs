using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    [SerializeField] float maxHp;
    [SerializeField] float currentHp;
    [SerializeField] float lowHpThreshold;
    [SerializeField] float healthRegenRate = 0.5f;

    [Header("Flash White Variables")]
    public bool isInvincible;
    [SerializeField] float flashWhiteDuration = 0.3f;
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
            GetComponent<CreatureDeath>().Death();
            GetComponent<CreatureDeath>().isDead = true;
        }

        if (currentHp < lowHpThreshold && !GetComponent<CreatureState>().hasFled)
            GetComponent<CreatureState>().isFleeing = true;

        if (GetComponent<CreatureState>().isFull && currentHp < maxHp)
            currentHp += Time.deltaTime * healthRegenRate;
    }

    public void LoseHealth(float value, GameObject damageFrom)
    {
        if (!isInvincible)
        {
            GetComponent<CreatureState>().lastSourceOfDamage = damageFrom;
            GetComponent<CreatureState>().hasFled = false;
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
