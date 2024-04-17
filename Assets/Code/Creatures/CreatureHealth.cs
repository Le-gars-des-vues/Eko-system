using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    [SerializeField] float maxHp;
    public float currentHp;
    [SerializeField] float lowHpThreshold;
    [SerializeField] float healthRegenRate = 0.5f;

    [Header("Flash White Variables")]
    public bool isInvincible;
    [SerializeField] float flashWhiteDuration = 0.3f;
    [SerializeField] Material flashMaterial;
    [SerializeField] private List<SpriteRenderer> creatureGFX = new List<SpriteRenderer>();
    [SerializeField] bool usesLineRenderer = false;
    [SerializeField] private LineRenderer line;
    private List<Material> ogMaterials = new List<Material>();

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
            GetComponent<CreatureDeath>().Death(GetComponent<Rigidbody2D>().gravityScale);
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
        isInvincible = true;
        ogMaterials.Clear();
        for (int i = 0; i < spriteList.Count; i++)
        {
            ogMaterials.Add(spriteList[i].material);
            spriteList[i].material = flashMaterial;
        }
        if (usesLineRenderer)
        {
            ogMaterials.Add(line.material);
            line.material = flashMaterial;
        }

        yield return new WaitForSecondsRealtime(duration);

        for (int i = 0; i < spriteList.Count; i++)
        {
            spriteList[i].material = ogMaterials[i];
        }
        if (usesLineRenderer)
            line.material = ogMaterials[spriteList.Count];
        isInvincible = false;
    }
}
