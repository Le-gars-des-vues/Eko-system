using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    [SerializeField] float maxHp;
    public float currentHp;
    [SerializeField] float lowHpThreshold;
    [SerializeField] float healthRegenRate = 0.5f;
    [SerializeField] bool isACreature = true;

    [Header("Flash White Variables")]
    public bool isInvincible;
    [SerializeField] float flashWhiteDuration = 0.3f;
    [SerializeField] Material flashMaterial;
    [SerializeField] private List<SpriteRenderer> creatureGFX = new List<SpriteRenderer>();
    [SerializeField] bool usesLineRenderer = false;
    [SerializeField] private List<LineRenderer> lines = new List<LineRenderer>();
    private List<Material> ogMaterials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (isACreature)
        {
            if (currentHp <= 0)
            {
                float gravityScale;

                if (!GetComponent<CreatureUnderwater>().isUnderwater)
                    gravityScale = 1;
                else
                    gravityScale = GetComponent<Rigidbody2D>().gravityScale;

                GetComponent<CreatureDeath>().Death(gravityScale);
                GetComponent<CreatureDeath>().isDead = true;
            }

            if (currentHp < lowHpThreshold && !GetComponent<CreatureState>().hasFled)
                GetComponent<CreatureState>().isFleeing = true;

            if (GetComponent<CreatureState>().isFull && currentHp < maxHp)
                currentHp += Time.deltaTime * healthRegenRate;
        }
    }

    public void LoseHealth(float value, GameObject damageFrom)
    {
        if (!isInvincible)
        {
            if (isACreature)
            {
                GetComponent<CreatureState>().lastSourceOfDamage = damageFrom;
                GetComponent<CreatureState>().hasFled = false;
                GetComponent<CreatureSound>().hurtSound.Post(gameObject);
            }
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
            for (int i = 0; i < lines.Count; i++)
            {
                ogMaterials.Add(lines[i].material);
                lines[i].material = flashMaterial;
            }
        }

        yield return new WaitForSecondsRealtime(duration);

        for (int i = 0; i < spriteList.Count; i++)
        {
            spriteList[i].material = ogMaterials[i];
        }
        if (usesLineRenderer)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].material = ogMaterials[spriteList.Count];
            }
        }
        isInvincible = false;
    }
}
