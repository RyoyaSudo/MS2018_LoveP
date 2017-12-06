using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMarkCtl : MonoBehaviour {

    public float posCtl;
    float angle = 0;
    float range = 40f;
    float yspeed = 0.001f;
    float yurehaba = 5;

    // Use this for initialization
    void Start () {
        transform.Translate(new Vector3(posCtl, 0.0f,0.0f));
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f));

        transform.Translate(new Vector3(Mathf.Sin((angle)  * range) / yurehaba,
       0.0f,
       0.0f));
        angle += yspeed;
    }
}
