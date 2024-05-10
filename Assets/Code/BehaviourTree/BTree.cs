using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTree : MonoBehaviour
    {
        private BehaviorNode root = null;
        float updateTime;

        protected void Start()
        {
            root = SetupTree();
        }

        private void Update()
        {
            if (root != null)
            {
                if (Vector2.Distance(GetComponent<CreatureState>().player.position, gameObject.transform.position) < GetComponent<CreatureState>().activationRange)
                {
                    GetComponent<CreatureState>().playerInRange = true;
                    if (updateTime <= 0)
                    {
                        root.Evaluate();
                        updateTime = 0.25f;
                    }
                    else
                        updateTime -= Time.deltaTime;
                }
                else
                    GetComponent<CreatureState>().playerInRange = false;
            }
        }

        protected abstract BehaviorNode SetupTree();
    }
}

