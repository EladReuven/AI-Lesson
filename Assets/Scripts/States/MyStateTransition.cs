using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyStateTransition
{
    MyState parentState;
    public TransitionType Type { get; }
    /// <summary>
    /// returns true if transition applied
    /// </summary>
    /// <returns></returns>
    public abstract bool CheckTransition();
}

public enum TransitionType
{
    PatrolToChase,
    ChaseToPatrol
}