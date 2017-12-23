﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPassengerManager : MonoBehaviour {

    public GameObject[] passengerPrefab;
    public GameObject[] passengers;
    public int passengerNum;
    private int count;
    private int passengerCount;
    public GameObject Rocket;

	// Use this for initialization
	void Start () {
        count = 0;
        passengerCount = 0;
        passengers = new GameObject[passengerNum];
        Vector3 pos = transform.position;
        for(int i = 0; i < passengerNum; i++)
        {
            passengers[i] = Instantiate(passengerPrefab[0], pos, Quaternion.identity);
            passengers[i].transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            passengers[i].GetComponent<ResultPassengerContoller>().rideStartPos = pos;
            pos.z += 20;
        }
	}
	
	// Update is called once per frame
	void Update () {
        count++;
        if( passengerCount < passengerNum )
        {
            
            if(count % 20 == 0)
            {
                passengers[passengerCount].GetComponent<ResultPassengerContoller>().SetStateType(Human.STATETYPE.RIDE);
                //passengers[passengerCount].GetComponent<ResultPassengerContoller>().rid += passengerCount;
                //passengers[passengerCount].GetComponent<ResultPassengerContoller>().RunStart();
                Debug.Log(passengerCount);
                passengerCount++;
            }
        }
        if (count > passengerNum * 20 + 150)
        {
            Rocket.GetComponent<ResultRocket>().RocketBlast();
        }
    }
}
