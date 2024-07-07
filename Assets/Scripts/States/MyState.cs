using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyState
{
    protected StateType type;
    public StateType Type { get { return type; } }

    private List<MyStateTransition> transitions;
    public List<MyStateTransition> Transitions => transitions;

    public void AddTransition(MyStateTransition newTransition)
    {
        if (newTransition == null)
            return;

        //if transition doesnt exist then add transition
        foreach (MyStateTransition transition in transitions)
            if (transition.Type == newTransition.Type)
                return;

        transitions.Add(newTransition);
    }

    public void RemoveTransition(MyStateTransition transition)
    {
        if (transition == null)
            return;

        //if transition exists then remove transition
        foreach (MyStateTransition t in transitions)
        {
            if (t.Type == transition.Type)
            {
                transitions.Remove(t);
                return;
            }
        }
    }
}
