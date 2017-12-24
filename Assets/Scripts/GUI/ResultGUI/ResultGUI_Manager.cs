using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultGUI_Manager : MonoBehaviour {

    // 子オブジェクト
    [SerializeField] TweenAnimation frameObj;


    // Use this for initialization
    void Start ()
    {
        frameObj.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
