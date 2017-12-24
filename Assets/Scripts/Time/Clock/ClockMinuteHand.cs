using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockMinuteHand : MonoBehaviour {

    //時計の秒針で使う////////////////////////////////////////
    const float TIME_ROTATEMAX = 360;
    const float TIME_SPLITMAX = 60;

    [SerializeField]
    private Transform clockMinuteHandObj;
    private float timeSplit;

    //時計の背景色//////////////////////////////////////////
    [SerializeField]
    private GameObject clockBackGroundObj;

    [SerializeField]
    private Material[] clockBGMaterial;

    //タイム関係///////////////////////////////////////////
    [SerializeField]
    private float totalTime;    //全体のタイム

    private float timer;

    //タイムの割合
    [SerializeField][Range(0f,1f)]
    private float timeRateYellow;
    [SerializeField][Range(0f, 1f)]
    private float timeRateRed;

    //タイムの色が変わるタイミングの割合の値
    private float timeRatioYellow;
    private float timeRatioRed;

    private float timeComparison;   //タイムの比較用の変数

    public TimeManager.State state;

    [SerializeField]
    private TimeCtrl timeCtrlObj;

    // Use this for initialization
    void Start()
    {
        //時計の秒針////////////////////////////////////////////
        //clockMinuteHandObj = transform.Find("Time_MinuteHand");

        //state = TimeManager.State.TIME_STATE_STOP;

        timeSplit = (TIME_ROTATEMAX / TIME_SPLITMAX);  //最大回転

        //時計のデフォの色を設定
        //clockBackGroundObj = transform.FindChild("Clock").Find("Time_Circle");
        clockBackGroundObj.GetComponent<Renderer>().material = clockBGMaterial[0];

        //色の変化するタイミングのタイムの割合計算
        timeRatioYellow = totalTime * timeRateYellow;
        timeRatioRed = totalTime * timeRateRed;
    }

    void Update()
    {
        switch (state)
        {
            case TimeManager.State.TIME_STATE_STOP:
                {
                    break;
                }
            case TimeManager.State.TIME_STATE_RUN:
                {
                    ClockUpdate();
                    break;
                }
        }

    }

    // Update is called once per frame
    private void ClockUpdate()
    {
        //TimeCtrlのタイムを取得して時計と時間を同期させる
        totalTime = timeCtrlObj.GetTime();   

        timer += Time.deltaTime;

        //Timerを全体のタイムから引いて現在の
        //timeComparison = totalTime - timer;
        timeComparison = totalTime;

        //分未満だったら色を変える
        if (timeComparison <= timeRatioYellow && timeComparison >= timeRatioRed ) 
        {
            //黄色の状態
            clockBackGroundObj.GetComponent<Renderer>().material = clockBGMaterial[1];
        }else if(timeComparison <= timeRatioRed){
            //赤色の状態
            clockBackGroundObj.GetComponent<Renderer>().material = clockBGMaterial[2]; 
        }

        //秒針回転
        clockMinuteHandObj.transform.Rotate(0.0f, 0.0f, -timeSplit * Time.deltaTime);
    }
 }
