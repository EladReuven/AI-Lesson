using BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : Node
{
    Func<bool> condition;
    public Condition(Func<bool> conditionToCheck)
    {
        condition = conditionToCheck;
    }
    public override NodeState Evaluate()
    {
        if (condition.Invoke())
            _state = NodeState.SUCCESS;
        else
            _state = NodeState.FALIURE;
        return _state;
    }
}
