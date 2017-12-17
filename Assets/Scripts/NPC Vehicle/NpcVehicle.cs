using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcVehicle : MonoBehaviour {

    private List<Transform> wayPoints;

    //ima iru point 
    private int currentRoot;

    //WayPoints.Length nokawari
    private int length;

    private UnityEngine.AI.NavMeshAgent agent;

    public bool IsMoveEnable { get { return isMoveEnable; } set { SetMoveEnable( value ); } }
    private bool isMoveEnable;

    [SerializeField] float wayPointChangeDistance;
    private float wayPointChangeDistanceSq;

    private void Awake()
    {
        isMoveEnable = true;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = 10;//このようにスクリプトからNavMeshのプロパティをいじれる。
        agent.autoBraking = true;  //目標地点に近づいても減速しないようにOffにする
    }

    // Use this for initialization
    void Start()
    {
        wayPointChangeDistanceSq = wayPointChangeDistance * wayPointChangeDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if( isMoveEnable == false )
        {
            return;
        }

        // ウェイポイントまでの距離を比較し、指定距離以内であるならば次のウェイポイントに切り替え
        Vector3 wayPointPos = wayPoints[currentRoot].position;
        Vector3 posV = wayPointPos - transform.position;

        if( Vector3.SqrMagnitude( posV ) < wayPointChangeDistanceSq )
        {
            currentRoot = ( currentRoot < length - 1 ) ? currentRoot + 1 : 0;
            wayPointPos = wayPoints[currentRoot].position;
        }

        agent.SetDestination( wayPointPos );
    }

    public void Initialize(List<Transform> data, int size, int spawnIndex)
    {
        wayPoints = data;
        length = size;
        transform.position = data[ spawnIndex ].position;
        currentRoot = spawnIndex;
        agent.SetDestination( data[ spawnIndex ].position );
    }

    private void SetMoveEnable( bool flags )
    {
        isMoveEnable = flags;
        agent.enabled = flags;
    }
}
    