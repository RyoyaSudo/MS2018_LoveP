using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutShipManager : MonoBehaviour {

    [SerializeField]
    private Transform[] wayPoints;

    private int currentRoot;

    private UnityEngine.AI.NavMeshAgent agent;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = 10;//このようにスクリプトからNavMeshのプロパティをいじれる。
        agent.autoBraking = false;  //目標地点に近づいても減速しないようにOffにする
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = wayPoints[currentRoot].position;

        if (Vector3.Distance(transform.position, pos) < 20.0f)
        {
            currentRoot = (currentRoot < wayPoints.Length - 1) ? currentRoot + 1 : 0;
        }
        agent.SetDestination(pos);
    }
}
