using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatIn : MonoBehaviour {

    public enum State
    {
        CATIN_STATE_IN = 0,
        CATIN_STATE_STAY,
        CATIN_STATE_OUT
    }
    State state;

    // Use this for initialization
    void Start () {
        state = State.CATIN_STATE_IN;
	}
	
	// Update is called once per frame
	void Update () {
        switch( state )
        {
            case State.CATIN_STATE_IN:
                {
                    transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                    if( transform.position.x > 100)
                    {
                        state = State.CATIN_STATE_STAY;
                    }
                    break;
                }
            case State.CATIN_STATE_STAY:
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    state = State.CATIN_STATE_STAY;
                    break;
                }
            case State.CATIN_STATE_OUT:
                {
                    transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
                    break;
                }
        }
	}
}
