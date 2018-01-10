using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    public static string scoreSaveKey = "curScoreKey";
    public static string totalScoreKey = "totalScoreKey";

    public enum State
    {
        SCORE_STATE_STOP = 0,
        SCORE_STATE_RUN,
        SCORE_STATE_SAVE,
    }
    public State state { get; set; }

    [SerializeField]
    private ScoreCtrl scoreObj;

    static bool isFirst = false;

    private void Start()
    {
        PlayerPrefs.SetInt( scoreSaveKey , 0 );

        if( isFirst == false )
        {
            PlayerPrefs.SetInt( totalScoreKey , 0 );
            isFirst = true;
        }

        PlayerPrefs.Save();
    }

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

    public void SaveScore()
    {
        scoreObj.SaveScore();
    }

}
