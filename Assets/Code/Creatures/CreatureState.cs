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

    [Header("Hunger Variables")]
    public float currentFood;
    [SerializeField] float maxFood = 100;
    [SerializeField] float digestRate = 1;
    public bool isFull;
    [SerializeField] float isFullCooldown = 120;
    [SerializeField] float isFullTime;
    float minSenseOfSmellRadius;
    float maxSenseOfSmellRadius;
    [SerializeField] float eatingTime = 2;

    [Header("Senses Variables")]
    public float senseOfSmell = 3.5f;
    public float fovRange = 3.5f;
    public float minFollowDistance = 10f;
    public string foodName;

    private void OnEnable()
    {
        currentFood = maxFood;
        minSenseOfSmellRadius = senseOfSmell;
        maxSenseOfSmellRadius = senseOfSmell * 2;
        Eat();
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
}
