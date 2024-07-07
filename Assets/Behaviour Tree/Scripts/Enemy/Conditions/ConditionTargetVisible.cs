using BehaviourTree;
using UnityEngine;

public class ConditionTargetVisible : Node
{
    //Transform _playerTransform;
    //Transform _transform;
    //float _range;
    //float _angle;

    SightSensor _sight;

    float _checkInterval = 0.25f;
    float _checkTimer = 0f;

    public ConditionTargetVisible(SightSensor sight)
    {
        _sight = sight;
    }

    public override NodeState Evaluate()
    {
        //check if interval passed
        if (_checkTimer < _checkInterval)
        {
            _checkTimer += Time.deltaTime;
            _state = NodeState.RUNNING;
            return _state;
        }

        //reset timer
        _checkTimer = 0f;

        ////get current target key
        //object t = GetRoot().GetData(EnemyGuardBT.LAST_TARGET_POS);

        //// if target does exist, check if meets conditions
        //if (t == null)
        //{
        //}
        if (_sight.CheckForTarget())
        {

            GetRoot().SetData(EnemyGuardBT.LAST_TARGET_POS, _sight.GetLastPos());
            _state = NodeState.SUCCESS;
        }
        else
            _state = NodeState.FALIURE;
        return _state;
    }
}