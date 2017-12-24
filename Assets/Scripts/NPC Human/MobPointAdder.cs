using System.Collections;
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

    ScoreCtrl scoreObj;
    [SerializeField] string scorePath;

    public bool IsPlayerStay { get; set; }

    // Use this for initialization
    void Start () {
        State = StateType.Ready;
        stateTimer = 0.0f;

        scoreObj = GameObject.Find( scorePath ).GetComponent<ScoreCtrl>();

        IsPlayerStay = false;
    }
	
	// Update is called once per frame
	void Update () {
        switch( State )
        {
            case StateType.Ready:
                break;

            case StateType.Add:
                scoreObj.AddScoreValue( 150 );
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
