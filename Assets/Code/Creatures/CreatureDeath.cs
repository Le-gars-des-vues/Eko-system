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
    bool isHarvested = false;
    public bool isDead = false;
    float timer;

    bool isExtracted = false;
    [SerializeField] GameObject dnaVial;
 
    public List<GameObject> species;
    

    // Start is called before the first frame update
    void OnEnable()
    {
        ToggleRagdoll(false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanExtract())
        {
            if (ArrowManager.instance.targetObject != gameObject)
                ArrowManager.instance.PlaceArrow(transform.position, "EXTRACT DNA", new Vector2(0, 0), gameObject, 1);
            //Debug.Log("can harvest");
            if (Input.GetKey(KeyCode.E))
            {
                //Debug.Log("Is harvesting");
                timer += Time.deltaTime;
                if (timer > timeToHarvest && ArrowManager.instance.readyToActivate)
                {
                    var ressourceSpawned = Instantiate(dnaVial, transform.position, transform.rotation);
                    ressourceSpawned.GetComponent<PickableObject>().PickUp(false, false);
                    isExtracted = true;
                    //timer = 0;
                    if (ArrowManager.instance.targetObject == gameObject)
                        ArrowManager.instance.RemoveArrow();
                    //gameObject.SetActive(false);
                }
            }
            else if (Input.GetKeyUp(KeyCode.E))
                timer = 0;
        }
        else if (CanHarvest())
        {
            if (ArrowManager.instance.targetObject != gameObject)
                ArrowManager.instance.PlaceArrow(transform.position, "HARVEST", new Vector2(0, 0), gameObject, 1);
            //Debug.Log("can harvest");
            if (Input.GetKey(KeyCode.E))
            {
                //Debug.Log("Is harvesting");
                timer += Time.deltaTime;
                if (timer > timeToHarvest && ArrowManager.instance.readyToActivate)
                {
                    var ressourceSpawned = Instantiate(ressourceToHarvest, transform.position, transform.rotation);
                    ressourceSpawned.GetComponent<PickableObject>().PickUp(false, false);
                    isHarvested = true;
                    //timer = 0;
                    if (ArrowManager.instance.targetObject == gameObject)
                        ArrowManager.instance.RemoveArrow();
                    //gameObject.SetActive(false);
                }
            }
            else if (Input.GetKeyUp(KeyCode.E))
                timer = 0;
        }
        else
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();

        if (isDead)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float dist = Vector2.Distance(player.transform.position, transform.position);
            if (dist < rangeToHarvest)
                isInRangeToHarvest = true;
            else
                isInRangeToHarvest = false;
        }
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

    bool CanExtract()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().objectInRightHand != null)
            return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().objectInRightHand.GetComponent<InventoryItem>().itemData.itemName == "ADN Gun" && !isExtracted && isDead && isInRangeToHarvest;
        else
            return false;
    }
}
