using BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAction : Node
{
    public Action _action;

    public BTAction(Action action)
    {
        _action = action;
    }

    public override NodeState Evaluate()
    {
        _action.Invoke();
        _state = NodeState.SUCCESS;
        return _state;
    }
}
