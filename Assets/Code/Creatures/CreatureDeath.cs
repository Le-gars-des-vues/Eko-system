using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

using BehaviorTree;

public class CreatureDeath : MonoBehaviour
{
    [Header("Ragdoll Variables")]
    [SerializeField] private List<Animator> creatureAnimators;

    [SerializeField] private List<Collider2D> colliders;
    [SerializeField] private List<Rigidbody2D> rbs;
    [SerializeField] private List<HingeJoint2D> joints;
    [SerializeField] private List<LimbSolver2D> limbs;

    [SerializeField] private List<Rigidbody2D> creatureRbs;
    public List<Collider2D> creatureColliders;

    [SerializeField] bool isLineRenderer;
    [SerializeField] private LineRenderer line;

    [Header("Death Variables")]
    [SerializeField] MonoBehaviour[] scripts;
    [SerializeField] BTree behaviorScript;
    [SerializeField] GameObject ressourceToHarvest;
    [SerializeField] float timeToHarvest = 1;
    [SerializeField] float rangeToHarvest = 1;
    public bool isInRangeToHarvest;
    bool isHarvested;
    public bool isDead;
    float timer;
    [SerializeField] GameObject arrow;

    public List<GameObject> species;
    

    // Start is called before the first frame update
    void OnEnable()
    {
        ToggleRagdoll(false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanHarvest())
        {
            if (arrow != null)
            {
                if (!arrow.activeSelf)
                    arrow.SetActive(true);
                else
                    arrow.transform.localRotation = Quaternion.Inverse(transform.rotation);
            }
            //Debug.Log("can harvest");
            if (Input.GetKey(KeyCode.E))
            {
                //Debug.Log("Is harvesting");
                timer += Time.deltaTime;
                if (timer > timeToHarvest && arrow.GetComponent<Arrow>().readyToActivate)
                {
                    var ressourceSpawned = Instantiate(ressourceToHarvest, transform.position, transform.rotation);
                    ressourceSpawned.GetComponent<PickableObject>().PickUp(false, false);
                    isHarvested = true;
                    gameObject.SetActive(false);
                }
            }
            else if (Input.GetKeyUp(KeyCode.E))
                timer = 0;
        }
        if (isDead)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float dist = Vector2.Distance(player.transform.position, transform.position);
            if (dist < rangeToHarvest)
                isInRangeToHarvest = true;
            else
                isInRangeToHarvest = false;
        }
        else
            if (arrow != null && arrow.activeSelf)
                arrow.SetActive(false);
    }

    public void Death()
    {
        behaviorScript.enabled = false;
        foreach (MonoBehaviour script in scripts)
        {
            if (script == this)
                continue;
            script.enabled = false;
        }
        if (species.Contains(gameObject))
            species.Remove(gameObject);
        ToggleRagdoll(true, isLineRenderer);
    }

    public void ToggleRagdoll(bool ragdollOn, bool lineRenderer)
    {
        foreach (var anim in creatureAnimators)
        {
            anim.enabled = !ragdollOn;
        }

        if (!lineRenderer)
            foreach (Rigidbody2D rb in creatureRbs)
                rb.simulated = !ragdollOn;
        else
        {
            line.gameObject.GetComponent<Tentacles>().wiggleMagnitude = 0;
            line.gameObject.GetComponent<Tentacles>().wiggleSpeed = 0;
        }

        if (lineRenderer && ragdollOn)
            foreach (Rigidbody2D rb in creatureRbs)
                rb.gravityScale = 1;

        foreach (var col in creatureColliders)
        {
            col.enabled = !ragdollOn;
        }
        foreach (var col in colliders)
        {
            col.enabled = ragdollOn;
        }
        foreach (var rb in rbs)
        {
            rb.simulated = ragdollOn;
        }
        foreach (var limb in limbs)
        {
            limb.weight = ragdollOn ? 0 : 1;
        }
        foreach (var joint in joints)
        {
            joint.enabled = ragdollOn;
        }
    }

    bool CanHarvest()
    {
        return isDead && isInRangeToHarvest && !isHarvested;
    }
}
