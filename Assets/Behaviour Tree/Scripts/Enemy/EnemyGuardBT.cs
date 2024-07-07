using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class EnemyGuardBT : BTree
{
    public static string CURRENT_TARGET = "currentTarget";
    public static string LAST_TARGET_POS = "lastTargetPos";
    public static string ALERT_BOOL = "alert";

    [Header("Navigating")]
    [SerializeField] protected NavMeshAgent _agent;
    [SerializeField] Transform[] _waypoints;

    [Header("Target Parameters")]
    [SerializeField] protected Player _player;
    [Space]

    [Header("Enemy parameters")]
    [Header("Sensors")]
    [SerializeField] SightSensor _sight;
    [SerializeField] HearingSensor _hearing;


    Vector3 targetLastPos = Vector3.negativeInfinity;
    bool alert = false;
    bool searching = false;


    //debugging/gizmos
    [Header("Debugging")]
    [SerializeField] Material material;

    private void Awake()
    {
        if (material == null)
            material = GetComponent<Material>();

        material = new Material(material);
    }

    protected override Node SetUpTree()
    {
        if (_player == null)
            _player = FindAnyObjectByType<Player>();

        //set up sight
        _sight.SetTarget(_player.transform);
        _sight.OnLastPositionUpdated.AddListener((pos) => targetLastPos = pos);

        //set up hearing
        _hearing.SetTarget(_player.transform);
        _hearing.OnLastPositionUpdated.AddListener(pos => targetLastPos = pos);

        Node root = new Selector(new List<Node> //root
        {
            new Sequence(new List<Node> //check if can See player, if yes, become alert and chase
            {
                new Condition(() => _sight.CheckForTarget()),
                new BTAction(() => alert = true),
                new BTAction(Chase),
            }),
            new Sequence(new List<Node> //checks if can Hear player, if yes become alert
            {
                new Condition(() => _hearing.CheckForTarget()),
                new BTAction(() => alert = true),
                new BTAction(Search),
            }),
            new Sequence(new List<Node> //if alert, Search
            {
                new Condition(() => alert),
                new BTAction(Search),
            }),
            new TaskPatrol(_agent, _waypoints), //default, return to patrol
        });

        return root;
    }

    void Chase()
    {
        print("chasing");
        _agent.SetDestination(_player.transform.position);
    }

    void Search()
    {
        print("searching");
        if (searching)
        {
            if (_agent.remainingDistance < 0.5)
            {
                alert = false;
                searching = false;
            }
        }
        else
        {
            searching = true;
            _agent.SetDestination(targetLastPos);
        }

    }


    private void OnDrawGizmos()
    {
        if (alert)
            material.color = Color.red;
        else
            material.color = Color.blue;
    }
}
