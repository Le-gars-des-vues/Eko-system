using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTree : MonoBehaviour
    {
        private BehaviorNode root = null;

        protected void Start()
        {
            root = SetupTree();
        }

        private void Update()
        {
            if (root != null)
                root.Evaluate();
        }

        protected abstract BehaviorNode SetupTree();
    }
}

