using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckTargetInSight : Node
{
    Transform _playerTransform;
    Transform _transform;
    float _fovPatrolRange;
    float _fovChaseRange;
    float _range;


    float _checkInterval = 0.25f;
    float _checkTimer = 0f;

    public CheckTargetInSight(Transform transform, Transform playerTransform, float fovPatrolRange, float fovChaseRange)
    {
        _transform = transform;
        _playerTransform = playerTransform;
        _fovPatrolRange = fovPatrolRange;
        _fovChaseRange = fovChaseRange;
    }

    public CheckTargetInSight(Transform transform, Transform target, float range)
    {
        _transform= transform;
        _playerTransform= target;
        _range = range;
    }


    bool InIdentifyTargetRangeFromPlayer => Vector3.Distance(_playerTransform.position, _transform.position) <= _fovPatrolRange;
    bool InAquiredTargetRangeFromPlayer => Vector3.Distance(_playerTransform.position, _transform.position) <= _fovChaseRange;
    public override NodeState Evaluate()
    {
        //check if interval passed
        if(_checkTimer < _checkInterval)
        {
            _checkTimer += Time.deltaTime;
            _state = NodeState.RUNNING;
            return _state;
        }

        //reset timer
        _checkTimer = 0f;

        //get current target key
        object t = GetRoot().GetData(EnemyGuardBT.CURRENT_TARGET);

        // if target does exist, check if in fovMaxRange
        if (t == null)
        {
            if (InIdentifyTargetRangeFromPlayer)
            {
                GetRoot().SetData(EnemyGuardBT.CURRENT_TARGET, _playerTransform);
                _state = NodeState.SUCCESS;
            }
            else
                _state = NodeState.FALIURE;
        }
        //if target already exists, check if still in Aquired Target Range.
        else
        {
            if (!InAquiredTargetRangeFromPlayer)
            {
                _state = NodeState.FALIURE;
                GetRoot().ClearData(EnemyGuardBT.CURRENT_TARGET);
            }
            else
                _state = NodeState.SUCCESS;
        }
        return _state;
    }
}
