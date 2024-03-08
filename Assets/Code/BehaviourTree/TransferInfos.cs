using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TransferInfos : BehaviorNode
{
    GameObject creature;

    public TransferInfos(GameObject _creature)
    {
        creature = _creature;
    }

    public override NodeState Evaluate()
    {
        //Check if the creature is flying or not
        parent.SetData("isFlying", creature.GetComponent<CreatureState>().isFlying);

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
                parent.ClearData("pathTarget");
            }
            //Pathfinding is set back to none
            parent.ClearData("target");
            parent.SetData("pathState", 0);
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
