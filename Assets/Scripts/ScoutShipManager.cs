using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutShipManager : MonoBehaviour {

    [SerializeField] Transform[] wayPoints;
    private int wayPointsNum;

    private int currentRoot;

    private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] float wayPointChangeDistance;
    private float wayPointChangeDistanceSq;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = 10;//このようにスクリプトからNavMeshのプロパティをいじれる。
        agent.autoBraking = false;  //目標地点に近づいても減速しないようにOffにする

        wayPointsNum = wayPoints.Length;
        currentRoot = 0;

        agent.SetDestination( wayPoints[ currentRoot ].position );

        wayPointChangeDistanceSq = wayPointChangeDistance * wayPointChangeDistance;
    }

    // Update is called once per frame
    void Update()
    {
        // ウェイポイントまでの距離を比較し、指定距離以内であるならば次のウェイポイントに切り替え
        Vector3 wayPointPos = wayPoints[currentRoot].position;
        Vector3 posV = wayPointPos - transform.position;

        if( Vector3.SqrMagnitude( posV ) < wayPointChangeDistanceSq )
        {
            currentRoot = ( currentRoot + 1 ) % wayPointsNum;
        }

        agent.SetDestination( wayPointPos );
    }
}
