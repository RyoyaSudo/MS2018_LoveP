using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    public enum State
    {
        SCORE_STATE_STOP = 0,
        SCORE_STATE_RUN
    }
    State state;

    [SerializeField]
    private ScoreCtrl scoreObj;

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.SCORE_STATE_STOP:
                {
                    break;
                }
            case State.SCORE_STATE_RUN:
                {
                }
                break;
        }
    }

    public void SetState(State setState)
    {
        state = setState;

        scoreObj.state = setState;
    }

}
