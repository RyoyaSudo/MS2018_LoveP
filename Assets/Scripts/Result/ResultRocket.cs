using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultRocket : MonoBehaviour {

    bool blastFlag;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if( blastFlag )
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
	}

    public void RocketBlast()
    {
        blastFlag = true;
    }
}
