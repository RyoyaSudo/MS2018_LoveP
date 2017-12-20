using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockMinuteHand : MonoBehaviour {

    //時計の秒針で使う////////////////////////////////////////
    const float TIME_ROTATEMAX = 360;
    const float TIME_SPLITMAX = 60;

    private Transform clockMinuteHandObj;
    private float timeSplit;

    //時計の背景色//////////////////////////////////////////
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
    // Use this for initialization
    void Start()
    {
        //時計の秒針////////////////////////////////////////////
        clockMinuteHandObj = transform.Find("Time_MinuteHand");
        timeSplit = (TIME_ROTATEMAX / TIME_SPLITMAX);  //最大回転

        //時計のデフォの色を設定
        clockBackGroundObj = transform.Find("Time_Circle").gameObject;
        clockBackGroundObj.GetComponent<Renderer>().material = clockBGMaterial[0];

        //色の変化するタイミングのタイムの割合計算
        timeRatioYellow = totalTime * timeRateYellow;
        timeRatioRed = totalTime * timeRateRed;
    }

    // Update is called once per frame
    void Update()
    {
        clockMinuteHandObj.transform.Rotate(0.0f, 0.0f, -timeSplit * Time.deltaTime);
        timer += Time.deltaTime;

        timeComparison = totalTime - timer;

        if (timeComparison <= timeRatioYellow && timeComparison >= timeRatioRed )   // 三分未満 180
        {
            //黄色の状態
            clockBackGroundObj.GetComponent<Renderer>().material = clockBGMaterial[1];
        }else if(timeComparison <= timeRatioRed){
            //赤色の状態
            clockBackGroundObj.GetComponent<Renderer>().material = clockBGMaterial[2]; 
        }
    }
 }
