using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStateMachine : MonoBehaviour
{
    List<MyState> states;

    MyState currentstate;
    public MyState CurrentState => currentstate;

    public MyStateMachine()
    {
        states = new List<MyState>();
    }

    public void AddState(MyState state)
    {
        //check if null
        if (states == null)
            Debug.LogError("AddState ERROR: Null reference is not allowed");

        //check if first state added
        if (states.Count == 0)
        {
            states.Add(state);
            currentstate = state;
            return;
        }

        //check if doesnt contain state already
        foreach (var existsingState in states)
            if (existsingState.Type == state.Type)
                return;

        AddState(state);
    }
    public void RemoveState(MyState state)
    {
        //check if null
        if (states == null)
            Debug.LogError("AddState ERROR: Null reference is not allowed");

        if (states.Count == 0)
            return;

        foreach (var existsingState in states)
            if (existsingState.Type == state.Type)
                states.Remove(existsingState);
    }
    public void CheckAvailableTransitions()
    {
        foreach (var transition in currentstate.Transitions)
        {
            if (transition.CheckTransition())
                return;
        }
    }
}

public enum StateType
{
    None,
    Patrol,
    Chase,
}