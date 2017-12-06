using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcVehicleManager : MonoBehaviour
{
    private int points; //wayPointsNumの受け皿
    private int pointsPos;  //モブ車の湧き位置

    private NpcVehicle[] npcObj;

    [SerializeField]
    private GameObject[] npcPrefab;

    [SerializeField]
    private string wayPointsPath;

    //
    public List<WaypointData> wayPointsData;

    //Points0~の総数
    private int pointsDataNum;

    //Pointsの0~入っている
    private Transform wayPointsDataList;

    // Use this for initialization
    void Start()
    {
        pointsDataNum = 0;

        //Listを初期化
        wayPointsData = new List<WaypointData>();
        int count = 0;

        wayPointsDataList = GameObject.Find(wayPointsPath).GetComponent<Transform>();
        foreach (Transform child in wayPointsDataList)
        {
            WaypointData obj = child.GetComponent<WaypointData>();
            wayPointsData.Add(obj);
            count++;
        }
        pointsDataNum = count;

        npcObj = new NpcVehicle[pointsDataNum];

        for (int nCnt = 0; nCnt < pointsDataNum; nCnt++)
        {
            NpcVehicleSet(npcPrefab[0], nCnt);
        }

        //UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //agent.speed = 10;//このようにスクリプトからNavMeshのプロパティをいじれる。
        //agent.autoBraking = true;  //目標地点に近づいても減速しないようにOffにする
    }

    //Npcvehicleを生成
    void NpcVehicleSet(GameObject prefab, int pointNum )
    {
        npcObj[pointNum] = Instantiate(prefab).GetComponent<NpcVehicle>();
        //Debug.Log(wayPointsData[pointNum].transform.GetChild(0).transform.name);
        //Debug.Log(wayPointsData[pointNum].transform.GetChild(0).transform.position);
        //Debug.Log(npcObj[pointNum].transform.position);

        npcObj[pointNum].Initialize(wayPointsData[pointNum].wayPointsTransform, wayPointsData[pointNum].pointsTransformNum, 0);
        npcObj[pointNum].transform.parent = transform;   //親子関係を紐づけする

    }
}