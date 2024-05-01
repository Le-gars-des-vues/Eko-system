using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureUnderwater : MonoBehaviour
{
    [Header("Underwater Variables")]
    [SerializeField] float underWaterGravityScale = 0.1f;
    [SerializeField] float underWaterDrag = 2f;
    [SerializeField] float underWaterAngularDrag = 4f;
    CreatureState state;

    [Header("Underwater Detection Variables")]
    [SerializeField] Transform head;
    [SerializeField] float breathRaycastLenght;
    public bool isUnderwater;

    float initialGravityScale;
    float initialDrag;
    float initialAngularDrag;

    [Header("Oxygen Variables")]
    [SerializeField] float maxOxygen = 40;
    [SerializeField] float currentOxygen;
    [SerializeField] float oxygenDepleteRate = 1;


    private void Start()
    {
        state = GetComponent<CreatureState>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        initialGravityScale = rb.gravityScale;
        initialDrag = rb.drag;
        initialAngularDrag = rb.angularDrag;
        currentOxygen = maxOxygen;
    }

    private void Update()
    {
        RaycastHit2D headUnderWater = Physics2D.Raycast(head.position, Vector2.up, breathRaycastLenght, LayerMask.GetMask("Water"));
        if (headUnderWater)
        {
            if (!isUnderwater)
                GoUnderwater(true);
        }
        else
        {
            if (isUnderwater)
                GoUnderwater(false);
        }


        if (isUnderwater)
        {
            if (!state.isAWaterCreature)
            {
                currentOxygen -= Time.deltaTime * oxygenDepleteRate;
            }

            if (currentOxygen <= 0)
            {
                GetComponent<CreatureHealth>().currentHp -= 0.01f;
            }
        }
    }

    void GoUnderwater(bool isTrue)
    {
        isUnderwater = isTrue;
        if (state.isAWaterCreature)
            state.isFlying = isTrue;
        if (isTrue)
        {
            foreach (Rigidbody2D rb in GetComponent<CreatureDeath>().creatureRbs)
            {
                rb.gravityScale = underWaterGravityScale;
                rb.drag = underWaterDrag;
                rb.angularDrag = underWaterAngularDrag;
            }

            foreach (Rigidbody2D rb in GetComponent<CreatureDeath>().rbs)
            {
                rb.gravityScale = underWaterGravityScale;
                rb.drag = underWaterDrag;
                rb.angularDrag = underWaterAngularDrag;
            }
        }
        else
        {
            foreach (Rigidbody2D rb in GetComponent<CreatureDeath>().creatureRbs)
            {
                rb.gravityScale = initialGravityScale;
                rb.drag = initialDrag;
                rb.angularDrag = initialAngularDrag;
            }

            foreach (Rigidbody2D rb in GetComponent<CreatureDeath>().rbs)
            {
                rb.gravityScale = initialGravityScale;
                rb.drag = initialDrag;
                rb.angularDrag = initialAngularDrag;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(head.position, Vector2.up * breathRaycastLenght);
    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            if (!isUnderwater)
                GoUnderwater(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            if (!isUnderwater)
                GoUnderwater(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            if (isUnderwater)
                GoUnderwater(false);
        }
    }
    */
}
