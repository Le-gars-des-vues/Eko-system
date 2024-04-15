using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TransferInfos : BehaviorNode
{
    GameObject creature;
    Transform target;

    public TransferInfos(GameObject _creature, Transform _target)
    {
        creature = _creature;
        target = _target;
    }

    public override NodeState Evaluate()
    {
        //Etat du debug
        parent.SetData("debug", creature.GetComponent<CreatureState>().debug);

        //Check if the creature is flying or not
        parent.SetData("isFlying", creature.GetComponent<CreatureState>().isFlying);
        parent.SetData("isUnderwater", creature.GetComponent<CreatureState>().isAWaterCreature);

        if (creature.GetComponent<CreatureState>().isFleeing)
        {
            parent.SetData("isFleeing", true);
        }
        else
        {
            parent.SetData("isFleeing", false);
        }

        //Check if creature has eaten recently or not
        parent.SetData("isFull", creature.GetComponent<CreatureState>().isFull);

        //Check if creature is attacking
        if (GetData("isAttacking") != null && creature != null)
            creature.GetComponent<CreatureState>().isAttacking = (bool)GetData("isAttacking");

        //Check if creature has reached end of path
        if (creature.GetComponent<CreaturePathfinding>().reachEndOfPath)
        {
            creature.GetComponent<CreaturePathfinding>().reachEndOfPath = false;
            //If the path target was food, the creature eats
            if (GetData("pathTarget") != null)
            {
                if ((string)GetData("pathTarget") == "food")
                {
                    creature.GetComponent<CreatureState>().Eat();
                    parent.SetData("isFull", creature.GetComponent<CreatureState>().isFull);
                }
                else if ((string)GetData("pathTarget") == "bait")
                {
                    creature.GetComponent<CreatureState>().EatBait();
                    int index = 0;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(creature.transform.position, 2, LayerMask.GetMask("Pixelate"));
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.gameObject.tag == "Bait" && index < 1)
                            creature.GetComponent<CreatureState>().EatObject(collider.gameObject);
                    }
                }
                parent.ClearData("pathTarget");
            }
            //Pathfinding is set back to none
            parent.ClearData("target");
            parent.SetData("pathState", 0);
        }

        if (creature.GetComponent<CreatureState>().isTamed && (Transform)GetData("target") != target)
        {
            parent.SetData("target", target);
            parent.SetData("pathTarget", "ally");
            parent.SetData("pathState", 1);
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
