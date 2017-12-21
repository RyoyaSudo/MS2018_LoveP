using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCtrl : MonoBehaviour
{
    private int[] TimeStack;     //スコアを格納する配列

    [SerializeField]
    private float TotalTime;      //制限時間

    [SerializeField] 
    private GameObject[] TimeArray;    //スコアの桁数

    private float timer;

    public GameObject TimePrefab;
    public Sprite[] SpriteArray;    //スコアのテクスチャ

    [SerializeField]
    Sprite numberSp;

    public TimeManager.State state;

    void Start()
    {
        timer = 0.0f;

        //スコアの実際に表示される0~9の値を格納する変数
        TimeStack = new int[ TimeArray.Length ];
//        TimeArray = new GameObject[ TIME_MAX ];

        //桁数分タイムを生成する
//        float unitSize = ( float )numberSp.texture.width / 10.0f;   //1文字分の幅を取る

//        for( int nCnt = 0 ; nCnt < TIME_MAX ; nCnt++ )
//        {
//            TimeArray[nCnt] = Instantiate(TimePrefab);
//            Vector3 pos;
        //    pos.x = ((float)Screen.width / 2.0f) / 1.5f + unitSize * nCnt + (padding * nCnt); // 1920/2 /2
        //    pos.y = ((float)Screen.height / 2.0f) - 20.0f - heightPadding;
        //    pos.z = 0.0f;

        //    TimeArray[nCnt].transform.position = pos; //Time生成
        //    TimeArray[nCnt].transform.parent = gameObject.transform;   //生成されたTimeArrayに元のTimeに親子関係を紐づけする
        //}
        TimeSet(0);
    }

    void Update()
    {
        switch(state)
        {
            case TimeManager.State.TIME_STATE_STOP:
                {
                    break;
                }
            case TimeManager.State.TIME_STATE_RUN:
                {
                    TimeUpdate();
                    break;
                }
        }
    }

    private void TimeUpdate()
    {
        timer += Time.deltaTime;
        if(timer >= 1.0f)
        {
            TotalTime -= timer;

            if( TotalTime < 0.0f ) TotalTime = 0.0f;

            TimeSet(TotalTime);
            timer = 0.0f;
        }
    }

    /************************************************************
    * 関数名：TimeSet
    * 引数:int Time
    * 戻り値:なし
    * 説明：スコアにどの値が入ったか調べる
    ***********************************************************/
    private void TimeSet( float Time )
    {
        Time = Mathf.Floor(TotalTime);

        int timeSeconds = (int)Time;

        //スコアゼロチェックの代わり
        if (TotalTime < 0)
        {
            TotalTime = 0;
        }

        //入ってきた時間をチェック
        TimeStack[ 0 ] = timeSeconds / 100 % 10;     //100の位
        TimeStack[ 1 ] = timeSeconds / 10 % 10;      //10の位
        TimeStack[ 2 ] = timeSeconds % 10;          //1の位

        //各TimeArrayにTimeStackに合ったスプライトに差し替える
        for( int nCnt = 0 ; nCnt < TimeArray.Length ; nCnt++ )
        {
            TimeArray[ nCnt ].GetComponent<SpriteRenderer>().sprite = SpriteArray[ TimeStack[ nCnt ] ];
        }
    }

    public float GetTime()
    {
        return TotalTime;
    }

    //------------------------もう使わなくなったやつ------------------------///
    //秒以下のコンマの表示と時間のゼロチェック

    //秒以下の設定
    /*private void ConmaSet( int Conma )
    {
        int TotalConma = Conma;

        TimeStack[ 0 ] = TotalConma % 10;
        TimeStack[ 1 ] = TotalConma % 10;

        TimeArray[ 0 ].GetComponent<SpriteRenderer>().sprite = SpriteArray[ TimeStack[ 0 ] ];
        TimeArray[ 1 ].GetComponent<SpriteRenderer>().sprite = SpriteArray[ TimeStack[ 1 ] ];
    }*/

    /************************************************************
    * 関数名：TimeZeroCheck
    * 引数:なし
    * 戻り値:なし
    * 説明：スコアの使われている桁を表示させて使われていない桁を非表示にする
    ***********************************************************/
    /*
    private void TimeZeroCheck()
    {
        int TimeValueCnt = TIME_MAX;          //桁数をカウントする用

        //スコアの0が入っている桁をスコアの最大桁数まで調べる
        while( TimeStack[ TimeValueCnt ] == 0 && TimeValueCnt > TIME_MAX )
        {
            TimeArray[ TimeValueCnt ].SetActive( false );
            if( TimeValueCnt >= TIME_MAX - 1 )
            {
                TimeArray[ TimeValueCnt ].SetActive( true );
                break;
            }
            TimeValueCnt--;
        }
        //上で調べた桁以外の桁をすべて調べる
        for( int nCnt = TimeValueCnt ; nCnt < TIME_MAX ; nCnt++ )
        {
            TimeArray[ nCnt ].SetActive( true );
        }
    }*/
}
