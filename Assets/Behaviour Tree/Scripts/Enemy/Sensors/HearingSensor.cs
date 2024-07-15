using System;
using UnityEngine;
using UnityEngine.Events;

public class HearingSensor : MonoBehaviour, ISensor
{
    [SerializeField] float _range;
    [SerializeField] Transform _target;
    Vector3 _lastHeardPos;
    Vector3 _hearingCheckPosition;
    bool _targetEnteredHearingRange = false;

    public UnityEvent<Vector3> OnLastPositionUpdated = new();

    bool InRange => Vector3.Distance(_target.position, transform.position) <= _range;


    public bool CheckForTarget()
    {
        if (!InRange)
        {
            _targetEnteredHearingRange = false;
            return false;
        }

        if (!_targetEnteredHearingRange) //if target just entered position save target position
        {
            _targetEnteredHearingRange = true;
            _hearingCheckPosition = _target.position;
        }

        if (_hearingCheckPosition != _target.position) // if target moved since entered hearing range, target is heard
        {
            _lastHeardPos = _hearingCheckPosition;
            OnLastPositionUpdated.Invoke(_lastHeardPos);
            print("target heard");
            return true;
        }
        return false;
    }

    public Vector3 GetLastPos()
    {
        return _lastHeardPos;
    }

    public SensorType GetSensorType()
    {
        return SensorType.Sound;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void OnDrawGizmos()
    {
        if (_targetEnteredHearingRange)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _range);
    }
}
