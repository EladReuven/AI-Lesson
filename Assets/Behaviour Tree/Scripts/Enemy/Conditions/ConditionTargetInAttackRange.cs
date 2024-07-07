using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionTargetInAttackRange : Node
{
    float _attackRange;
    Transform _targetTransform;
    Transform _transform;
    
    public ConditionTargetInAttackRange(Transform transform, Transform target, float attackRange)
    {
        _transform = transform;
        _targetTransform = target;
        _attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        if (Vector3.Distance(_targetTransform.position, _transform.position) < _attackRange)
            return NodeState.SUCCESS;

        return NodeState.FALIURE;
    }
}