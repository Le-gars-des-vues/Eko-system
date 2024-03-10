using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureState : MonoBehaviour
{
    [Header("State Variables")]
    public bool isAttacking = false;
    public bool isFleeing = false;
    public bool isEating = false;
    public bool isPathfinding;
    public bool isStunned;
    public bool isFlying;

    [Header("Hunger Variables")]
    public float currentFood;
    [SerializeField] float maxFood = 100;
    [SerializeField] float digestRate = 1;
    public bool isFull;
    [SerializeField] float isFullCooldown = 120;
    [SerializeField] float isFullTime = 0;
    float minSenseOfSmellRadius;
    float maxSenseOfSmellRadius;
    [SerializeField] float eatingTime = 2;

    [Header("Senses Variables")]
    public float senseOfSmell = 3.5f;
    public float fovRange = 3.5f;
    public float minFollowDistance = 10f;
    public string foodName;

    [Header("Stun Variables")]
    [SerializeField] float stunDuration;
    float stunnedTime;
    bool stunTrigger;

    [Header("Flee Variables")]
    public GameObject lastSourceOfDamage;
    public bool hasFled;

    public BoxCollider2D territory;
    bool hasATerritory = false;

    private void OnEnable()
    {
        currentFood = maxFood;
        minSenseOfSmellRadius = senseOfSmell;
        maxSenseOfSmellRadius = senseOfSmell * 2;
        isFull = true;

        territory = (BoxCollider2D)Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("SceneView"));
    }

    // Update is called once per frame
    void Update()
    {
        if (isFull)
        {
            if (Time.time - isFullTime > isFullCooldown)
                isFull = false;
        }

        if (!isFull && currentFood > 0)
            currentFood -= digestRate * Time.deltaTime;

        senseOfSmell = Mathf.Lerp(minSenseOfSmellRadius, maxSenseOfSmellRadius, (maxFood - currentFood) / maxFood);

        if (isStunned)
        {
            if (!stunTrigger)
            {
                stunTrigger = true;
                stunnedTime = Time.time;
            }
            if (Time.time - stunnedTime > stunDuration)
            {
                isStunned = false;
                stunTrigger = false;
            }
        }
    }

    public void Eat()
    {
        isFull = true;
        isFullTime = Time.time;
        currentFood = maxFood;
        StartCoroutine(Eating());
    }

    IEnumerator Eating()
    {
        isEating = true;
        Debug.Log("Is eating");
        yield return new WaitForSeconds(eatingTime);
        Debug.Log("finished eating");
        isEating = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Territory")
        {
            if (!hasATerritory)
                territory = (BoxCollider2D)collision;
        }
    }
}