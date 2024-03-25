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
    [SerializeField] float stunDuration = 10;
    float stunnedTime;
    bool stunTrigger;

    [Header("Flee Variables")]
    public GameObject lastSourceOfDamage;
    public bool hasFled;

    [Header("Territory Variables")]
    public BoxCollider2D territory;
    bool hasATerritory = false;

    [Header("Tamed Variables")]
    public int levelOfAffection = 0;
    [SerializeField] int tamedAffectionThreshold = 1;
    public bool isTamed;

    [SerializeField] Transform target;
    public bool debug = false;

    private void OnEnable()
    {
        currentFood = maxFood;
        minSenseOfSmellRadius = senseOfSmell;
        maxSenseOfSmellRadius = senseOfSmell * 2;
        isFull = true;

        territory = (BoxCollider2D)Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("SceneView", "Water"));
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

        if (levelOfAffection >= tamedAffectionThreshold && !isTamed)
        {
            isTamed = true;
        }

        if (isTamed)
        {
            target.position = GameObject.FindGameObjectWithTag("Player").transform.position;
            isFull = true;
        }
    }

    public void Eat()
    {
        isFull = true;
        isFullTime = Time.time;
        currentFood = maxFood;
        StartCoroutine(Eating());
    }

    public void EatBait()
    {
        levelOfAffection++;
        StartCoroutine(Eating());
    }

    public void EatObject(GameObject objectToEat)
    {
        Destroy(objectToEat);
    }

    IEnumerator Eating()
    {
        isEating = true;
        //Debug.Log("Is eating");
        yield return new WaitForSeconds(eatingTime);
        //Debug.Log("finished eating");
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
