using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{

    public enum State
    {
        TIME_STATE_STOP = 0,
        TIME_STATE_RUN
    }
    State state;

    private int count;

    [SerializeField]
    private TimeCtrl timeObj;

    [SerializeField]
    private ClockMinuteHand clockObj;

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.TIME_STATE_STOP:
                {
                    break;
                }
            case State.TIME_STATE_RUN:
                {
                    break;
                }
        }
    }

    public void SetState(State setState)
    {
        state = setState;

        timeObj.state = setState;
        clockObj.state = setState;
    }
}
