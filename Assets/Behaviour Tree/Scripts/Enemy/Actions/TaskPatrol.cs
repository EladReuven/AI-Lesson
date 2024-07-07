using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;
public class TaskPatrol : Node
{
    NavMeshAgent _agent;
    Transform[] _waypoints;

    int _currentWaypointIndex = 0;
    float _waitTime = 1f;
    float _waitCounter = 0f;
    bool _waiting = false;

    bool ReachedDestination => _agent.remainingDistance <= _agent.stoppingDistance;

    public TaskPatrol(NavMeshAgent agent, Transform[] waypoints)
    {
        _agent = agent;
        _waypoints = waypoints;
        _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
    }

    public override NodeState Evaluate()
    {
        CheckPatrolNavigation();

        _state = NodeState.RUNNING;
        return _state;
    }

    void CheckPatrolNavigation()
    {
        if (!ReachedDestination)
            return;

        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                _agent.isStopped = false;
                _waitCounter = 0f;
                _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
            }
            return;
        }

        _waiting = true;
        _agent.isStopped = true;
        _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
    }
}
