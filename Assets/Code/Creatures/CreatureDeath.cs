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
    public List<Rigidbody2D> rbs;
    [SerializeField] private List<HingeJoint2D> joints;
    [SerializeField] private List<LimbSolver2D> limbs;
    [SerializeField] private List<FabrikSolver2D> longLimbs;

    public List<Rigidbody2D> creatureRbs;
    public List<Collider2D> creatureColliders;

    [SerializeField] bool isLineRenderer;
    [SerializeField] private List<LineRenderer> lines = new List<LineRenderer>();

    [Header("Death Variables")]
    [SerializeField] MonoBehaviour[] scripts;
    [SerializeField] BTree behaviorScript;
    public float ressourceSpawnedCount = 1;
    public GameObject ressourceToHarvest;
    [SerializeField] float timeToHarvest = 1;
    [SerializeField] float rangeToHarvest = 1;
    public bool isInRangeToHarvest;
    public bool isHarvested = false;
    public bool isDead = false;
    [SerializeField] GameObject corpse;
    bool isExtracted = false;
    public GameObject dnaVial;
    public bool isInPod;
 
    public List<GameObject> species;

    float drag;
    float angularDrag;
    

    // Start is called before the first frame update
    void OnEnable()
    {
        ToggleRagdoll(false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInPod)
        {
            if (CanExtract())
            {
                if (ArrowManager.instance.targetObject != corpse)
                    ArrowManager.instance.PlaceArrow(transform.position, "EXTRACT DNA", new Vector2(0, 0), corpse);
                //Debug.Log("can harvest");
                if (Input.GetKeyDown(KeyCode.E) && ArrowManager.instance.targetObject == corpse)
                {
                    var ressourceSpawned = Instantiate(dnaVial, transform.position, transform.rotation);
                    //ressourceSpawned.GetComponent<PickableObject>().PickUp(false, false);
                    isExtracted = true;
                    if (ArrowManager.instance.targetObject == corpse)
                        ArrowManager.instance.RemoveArrow();
                    //gameObject.SetActive(false);
                }
            }
            else if (CanHarvest())
            {
                if (ArrowManager.instance.targetObject != corpse)
                    ArrowManager.instance.PlaceArrow(transform.position, "HARVEST", new Vector2(0, 0), corpse);
                //Debug.Log("can harvest");
                if (Input.GetKeyDown(KeyCode.E) && ArrowManager.instance.targetObject == corpse)
                {
                    for (int i = 0; i < ressourceSpawnedCount; i++)
                    {
                        var ressourceSpawned = Instantiate(ressourceToHarvest, transform.position, transform.rotation);
                        //ressourceSpawned.GetComponent<PickableObject>().PickUp(false, false);
                    }
                    isHarvested = true;
                    if (ArrowManager.instance.targetObject == corpse)
                        ArrowManager.instance.RemoveArrow();
                }
            }
            else
                if (ArrowManager.instance.targetObject == corpse)
                    ArrowManager.instance.RemoveArrow();

            if (isDead)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                float dist = Vector2.Distance(player.transform.position, corpse.transform.position);
                if (dist < rangeToHarvest)
                {
                    isInRangeToHarvest = true;
                    if (!Tutorial.instance.firstHarvest)
                    {
                        Tutorial.instance.RobotTextMessage(Tutorial.instance.tutorialTexts[4].text);
                        Tutorial.instance.firstHarvest = true;
                    }
                }
                else
                    isInRangeToHarvest = false;
            }
        }
    }

    public void Death(float gravityScale)
    {
        if (GetComponent<CreatureSound>() != null)
        {
            AkSoundEngine.StopPlayingID(GetComponent<CreatureSound>().idleSoundID);
            if (GetComponent<CreatureSound>().deathSound != null)
                AudioManager.instance.PlaySound(GetComponent<CreatureSound>().deathSound, gameObject);
        }
        drag = GetComponent<Rigidbody2D>().drag;
        angularDrag = GetComponent<Rigidbody2D>().angularDrag;

        behaviorScript.enabled = false;
        foreach (MonoBehaviour script in scripts)
        {
            if (script == this)
                continue;
            script.enabled = false;
        }
        if (species.Contains(gameObject))
            species.Remove(gameObject);
        ToggleRagdoll(true, isLineRenderer, gravityScale);
        if (gameObject.name == "Frog")
        {
            gameObject.transform.Find("bone_10").GetComponent<HingeJoint2D>().connectedBody = gameObject.transform.Find("frogModel").Find("bone_1").GetComponent<Rigidbody2D>();
        }
    }

    public void ToggleRagdoll(bool ragdollOn, bool lineRenderer, float gravityScale = 1)
    {
        foreach (var anim in creatureAnimators)
        {
            anim.enabled = !ragdollOn;
        }

        if (!lineRenderer)
        {
            foreach (Rigidbody2D rb in creatureRbs)
                rb.simulated = !ragdollOn;

            foreach (var col in creatureColliders)
            {
                col.enabled = !ragdollOn;
            }
        }
        else
        {
            foreach (LineRenderer line in lines)
            {
                line.gameObject.GetComponent<Tentacles>().wiggleMagnitude = 0;
                line.gameObject.GetComponent<Tentacles>().wiggleSpeed = 0;
            }
        }

        foreach (var col in colliders)
        {
            col.enabled = ragdollOn;
        }

        if (lineRenderer && ragdollOn)
        {
            foreach (Rigidbody2D rb in creatureRbs)
            {
                rb.gravityScale = gravityScale;
                rb.drag = drag;
                rb.angularDrag = angularDrag;
            }
        }

        foreach (var rb in rbs)
        {
            rb.simulated = ragdollOn;
            if (ragdollOn)
            {
                rb.gravityScale = gravityScale;
                rb.drag = drag;
                rb.angularDrag = angularDrag;
            }
        }
        foreach (var limb in limbs)
        {
            limb.weight = ragdollOn ? 0 : 1;
        }
        foreach (var limb in longLimbs)
        {
            limb.weight = ragdollOn ? 0 : 1;
        }
        foreach (var joint in joints)
        {
            joint.enabled = ragdollOn;
        }

        if (gameObject.name == "Flumpf")
        {
            Vector2 scale = lines[0].gameObject.transform.localScale;
            scale.x = -1;
            lines[0].gameObject.transform.localScale = scale;
        }
    }

    bool CanHarvest()
    {
        return isDead && isInRangeToHarvest && !isHarvested;
    }

    bool CanExtract()
    {
        if (GameManager.instance.player.objectInRightHand != null && !GameManager.instance.player.isUsingMultiTool)
            return GameManager.instance.player.objectInRightHand.GetComponent<InventoryItem>().itemData.itemName == "ADN Gun" && !GameManager.instance.player.isUsingMultiTool&& !isExtracted && isDead && isInRangeToHarvest;
        else
            return false;
    }
}
