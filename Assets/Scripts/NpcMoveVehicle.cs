using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMoveVehicle : MonoBehaviour {

    public Transform[] waytPoints;

    public int currentRoot;

    [SerializeField]
    private GameObject npcPrefab;

    private GameObject npcObj;
    // Use this for initialization
    void Start()
    {
        npcObj = Instantiate( npcPrefab );
        npcObj.transform.position = gameObject.transform.position;
        npcObj.transform.parent = gameObject.transform;   //生成されたScoreArrayに元のScoreに親子関係を紐づけする

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = 10;//このようにスクリプトからNavMeshのプロパティをいじれる。
        agent.autoBraking = true;  //目標地点に近づいても減速しないようにOffにする
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = waytPoints[currentRoot].position;

        if (Vector3.Distance(transform.position, pos) < 1.0f)
        {
            currentRoot = (currentRoot < waytPoints.Length - 1) ? currentRoot + 1 : 0;
        }

        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(pos);
    }
}

