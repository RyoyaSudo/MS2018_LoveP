using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerGroupUI : MonoBehaviour {

	// 初期化
	void Start ()
    {
		
	}
	
	//更新
	void Update ()
    {
        Vector3 p = Camera.main.transform.position;
        p.y = transform.position.y;
        transform.LookAt(p);
    }
}
