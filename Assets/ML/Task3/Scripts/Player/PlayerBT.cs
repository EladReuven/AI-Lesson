using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
public class PlayerBT : BTree
{
    public EntityStats stats;

    Transform target;
    protected override Node SetUpTree()
    {
        //mek tree
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node> {
                new Condition(() => EvaluateGetHealthPack()),
                new BTAction(GoToHealthPack),
            }),
            

        });

        return root;
    }

    void MoveToTarget()
    {
        Vector3.MoveTowards(transform.position, target.position, stats.speed * Time.deltaTime);
    }

    void GoToHealthPack()
    {
        //get nearest health pack location
        //transform move to it
    }

    bool EvaluateGetHealthPack()
    {
        return true;
    }
}
