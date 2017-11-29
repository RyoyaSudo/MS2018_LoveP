using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMarkCtl : MonoBehaviour {

    public float posCtl;
	// Use this for initialization
	void Start () {
        transform.Translate(new Vector3(posCtl, 0.0f,0.0f));
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f));
	}
}
