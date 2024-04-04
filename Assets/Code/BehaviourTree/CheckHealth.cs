using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckHealth : BehaviorNode
{
    GameObject creature;

    public CheckHealth(GameObject _creature)
    {
        creature = _creature;
    }

    public override NodeState Evaluate()
    {
        if (GetData("isFleeing") != null && (bool)GetData("isFleeing"))
        {
            parent.parent.ClearData("target");
            parent.parent.ClearData("pathTarget");
            parent.parent.SetData("pathState", 2);
            creature.GetComponent<CreaturePathfinding>().ReachedEndOfPath();
            if (GetData("isAttacking") != null && (bool)GetData("isAttacking") == true)
                parent.parent.SetData("isAttacking", false);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
