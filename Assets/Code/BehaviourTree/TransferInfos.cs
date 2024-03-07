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
        if (GetData("pathState") != null && (int)GetData("pathState") == 0)
        {
            parent.SetData("pathState", -1);
            creature.GetComponent<CreaturePathfinding>().StopPathFinding();
        }

        parent.SetData("isFull", creature.GetComponent<CreatureState>().isFull);

        if (GetData("isAttacking") != null && creature != null)
            creature.GetComponent<CreatureState>().isAttacking = (bool)GetData("isAttacking");

        if (creature.GetComponent<CreaturePathfinding>().reachEndOfPath)
        {
            creature.GetComponent<CreaturePathfinding>().reachEndOfPath = false;
            if (GetData("pathTarget") != null)
            {
                if ((string)GetData("pathTarget") == "food")
                {
                    creature.GetComponent<CreatureState>().Eat();
                    parent.SetData("isFull", creature.GetComponent<CreatureState>().isFull);
                }
                parent.ClearData("pathTarget");
            }
            parent.ClearData("target");
            parent.SetData("pathState", 0);
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
