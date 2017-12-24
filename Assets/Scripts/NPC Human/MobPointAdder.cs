﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobPointAdder : MonoBehaviour {

    public enum StateType
    {
        Ready = 0,
        Add,
        Wait,
        StateTypeMax,
    }

    public StateType State { get { return state; } set { SetState( value ); } }
    private StateType state;

    float stateTimer;

    [SerializeField] float pointAddDuration;
    [SerializeField] ScoreEffect scoreEffectprefab;

    public bool IsPlayerStay { get; set; }

    private GameObject playerObj;
    [SerializeField] string playerObjPath;

    // Use this for initialization
    void Start () {
        State = StateType.Ready;
        stateTimer = 0.0f;

        IsPlayerStay = false;

        playerObj = GameObject.Find( playerObjPath );
    }
	
	// Update is called once per frame
	void Update () {
        switch( State )
        {
            case StateType.Ready:
                break;

            case StateType.Add:
                for( int i = 0 ; i < 5 ; i++ )
                {
                    ScoreEffect e = Instantiate( scoreEffectprefab , transform.position , transform.rotation );
                    e.AddScore = 10;
                    e.Target = playerObj.transform;
                    e.waitTime = 0.5f;
                    e.State = ScoreEffect.StateType.Wait;
                }
                
                State = StateType.Wait;
                break;

            case StateType.Wait:
                stateTimer -= Time.deltaTime;

                if( stateTimer < 0.0f )
                {
                    State = StateType.Ready;
                }
                break;

            default:
                break;
        }
	}

    void SetState( StateType type )
    {
        state = type;

        switch( type )
        {
            case StateType.Ready:
                stateTimer = 0.0f;
                break;

            case StateType.Add:
                break;

            case StateType.Wait:
                stateTimer = pointAddDuration;
                break;

            default:
                break;
        }
    }
}
