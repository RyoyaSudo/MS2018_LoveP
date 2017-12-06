using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointData : MonoBehaviour
{
    public List<Transform> wayPointsTransform;

    public int pointsTransformNum;

    // Use this for initialization
    void Awake()
    {
        //Listを初期化
        wayPointsTransform = new List<Transform>();

        int Count = 0;
        pointsTransformNum = 0;
        //string log = "";

        foreach (Transform child in transform)
        {
            wayPointsTransform.Add(child);
            //log += "Transform[ + Count +  ]" + wayPointsTransform[Count].ToString() + "¥n";
            Count++;
        }
        //Debug.Log(log);
        pointsTransformNum = Count;
    }
}

