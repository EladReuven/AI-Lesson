using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskChase : Node
{
    NavMeshAgent _agent;
    Transform _targetTransform;

    public TaskChase(NavMeshAgent agent, Transform targetTransform)
    {
        _agent = agent;
        _targetTransform = targetTransform;
    }

    public override NodeState Evaluate()
    {
        Transform target = GetRoot().GetData(EnemyGuardBT.LAST_TARGET_POS) as Transform;
        if (target == null)
        {
            _state = NodeState.FALIURE;
            return _state;
        }
        _agent.SetDestination(_targetTransform.position);

        //if reached destination then set target destination null maybe??????
        if(_agent.remainingDistance < 1)
        {
            GetRoot().SetData(EnemyGuardBT.LAST_TARGET_POS, null);
        }
        _state = NodeState.RUNNING;
        return _state;
    }
}
