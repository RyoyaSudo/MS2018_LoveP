using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour {

    public List<WaypointData> wayPointsData;

    public int pointsDataNum;

	// Use this for initialization
	void Start () {
        pointsDataNum = 0;
        //Listを初期化
        wayPointsData = new List<WaypointData>();
        int count = 0;

        foreach( Transform child in transform)
        {
            WaypointData obj = child.GetComponent<WaypointData>();
            wayPointsData.Add(obj);
            //Debug.Log(wayPointsData[count]);
            count++;
        }
        pointsDataNum = count;
	}
}
