using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TransferInfos : BehaviorNode
{
    GameObject creature;
    bool isAttacking;

    public TransferInfos(GameObject _creature)
    {
        creature = _creature;
    }

    public override NodeState Evaluate()
    {
        if (GetData("isAttacking") != null && creature != null)
        {
            creature.GetComponent<CreatureState>().isAttacking = (bool)GetData("isAttacking");
            Debug.Log((bool)GetData("isAttacking"));
            Debug.Log(creature.GetComponent<CreatureState>().isAttacking);
        }
        state = NodeState.SUCCESS;
        return state;
    }
}
