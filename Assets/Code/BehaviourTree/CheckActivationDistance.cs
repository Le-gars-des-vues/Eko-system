using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckActivationDistance : BehaviorNode
{
    CreatureState creature;
    float activationRange;

    public CheckActivationDistance(CreatureState _creature, float _activationRange)
    {
        creature = _creature;
        activationRange = _activationRange;
    }

    public override NodeState Evaluate()
    {
        if (Vector2.Distance(creature.gameObject.transform.position, creature.player.position) < activationRange)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            state = NodeState.RUNNING;
            return state;
        }
    }
}
