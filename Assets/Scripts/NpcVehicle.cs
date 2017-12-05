using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcVehicle : MonoBehaviour {

    private List<Transform> wayPoints;

    //ima iru point 
    private int currentRoot;

    //WayPoints.Length nokawari
    private int length;

    // Use this for initialization
    void Start()
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = 30;//このようにスクリプトからNavMeshのプロパティをいじれる。
        agent.autoBraking = true;  //目標地点に近づいても減速しないようにOffにする
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = wayPoints[currentRoot].position;

        if (Vector3.Distance(wayPoints[currentRoot].position, pos) < 1.0f)
        {
            currentRoot = (currentRoot < length - 1) ? currentRoot + 1 : 0;
        }

        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(pos);
    }

    public void Initialize(List<Transform> data, int size, int spawnIndex)
    {
        wayPoints = data;
        length = size;
        transform.position = wayPoints[ spawnIndex ].position;
    }
}
