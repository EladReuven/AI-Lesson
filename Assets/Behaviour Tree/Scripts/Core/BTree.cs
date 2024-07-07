using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class BTree : MonoBehaviour
    {
        private Node _root = null;

        protected void Start()
        {
            _root = SetUpTree();
        }

        private void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract Node SetUpTree();
    }
}
