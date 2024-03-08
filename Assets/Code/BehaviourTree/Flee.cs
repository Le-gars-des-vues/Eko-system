using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Flee : BehaviorNode
{
    Transform target;
    GameObject creature;
    float fleeDistance;
    float stopFleeingDistance;

    public Flee(Transform _target, GameObject _creature, float _fleeDistance)
    {
        target = _target;
        creature = _creature;
        fleeDistance = _fleeDistance;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("isFleeing");
        Vector2 direction = creature.transform.position - creature.GetComponent<CreatureState>().lastSourceOfDamage.transform.position;
        target.position = (Vector2)creature.transform.position + direction.normalized * fleeDistance;

        if (Vector2.Distance(creature.transform.position, creature.GetComponent<CreatureState>().lastSourceOfDamage.transform.position) > fleeDistance)
        {
            creature.GetComponent<CreatureState>().hasFled = true;
            creature.GetComponent<CreatureState>().isFleeing = false;
        }


        state = NodeState.RUNNING;
        return state;
    }
}
