using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    public enum State
    {
        SCORE_STATE_STOP = 0,
        SCORE_STATE_RUN,
        SCORE_STATE_SAVE,
    }
    public State state { get; set; }

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

            case State.SCORE_STATE_SAVE:
                {
                    scoreObj.SaveScore();
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
