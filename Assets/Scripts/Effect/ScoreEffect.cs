using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEffect : MonoBehaviour {

    [SerializeField] ParticleSystem particle;
    public Transform Target { get; set; }
    bool IsChase { get; set; }

    [SerializeField] float chaseDuration;
    float chaseAddValuePerFlame;
    float chaseRate;

    Vector3 getPos;

	// Use this for initialization
	void Start () {
        IsChase = false;
        Target = null;
        chaseRate = 0.0f;
        chaseAddValuePerFlame = 1.0f / chaseDuration;

        getPos = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {

        if( IsChase == true )
        {
            chaseRate += ( chaseAddValuePerFlame * ( 60.0f * Time.deltaTime ) );
            chaseRate = Mathf.Min( chaseRate , 1.0f );

            Vector3 curV = Vector3.Slerp( getPos , Target.position , chaseRate );

            if( chaseRate == 1.0f )
            {
                IsChase = false;
            }
        }
    }
}
