using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureUnderwater : MonoBehaviour
{
    [SerializeField] float underWaterGravityScale = 0.1f;
    [SerializeField] float underWaterDrag = 2f;
    [SerializeField] float underWaterAngularDrag = 4f;
    [SerializeField] float initialGravityScale;
    [SerializeField] float initialDrag;
    [SerializeField] float initialAngularDrag;

    Rigidbody2D rb;
    bool isUnderwater;

    public bool isAWaterCreature;
    [SerializeField] float maxOxygen = 40;
    [SerializeField] float currentOxygen;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialGravityScale = rb.gravityScale;
        initialDrag = rb.drag;
        initialAngularDrag = rb.angularDrag;
        currentOxygen = maxOxygen;
    }

    private void Update()
    {

    }

    void GoUnderwater(bool isTrue)
    {
        isUnderwater = isTrue;
        if (isTrue)
        {
            rb.gravityScale = underWaterGravityScale;
            rb.drag = underWaterDrag;
            rb.angularDrag = underWaterAngularDrag;
        }
        else
        {
            rb.gravityScale = initialGravityScale;
            rb.drag = initialDrag;
            rb.angularDrag = initialAngularDrag;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
}