using BehaviourTree;
using System.Collections.Generic;

public class Sequence : Node
{
    public Sequence() : base()
    {
    }

    public Sequence(List<Node> children) : base(children)
    {
    }

    public override NodeState Evaluate()
    {
        bool isAnyChildRunning = false;
        foreach (var child in _children)
        {
            switch (child.Evaluate())
            {
                case NodeState.RUNNING:
                    isAnyChildRunning = true;
                    break;
                case NodeState.SUCCESS:
                    continue;
                case NodeState.FALIURE:
                    _state = NodeState.FALIURE;
                    return _state;
                default:
                    _state = NodeState.SUCCESS;
                    break;
            }
        }
        _state = isAnyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return _state;
    }
}
