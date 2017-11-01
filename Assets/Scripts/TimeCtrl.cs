using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCtrl : MonoBehaviour
{

    const int TIME_MAX = 4;    //スコアの桁数
    const float TIME_DEFAULT_POS = -1.5f; //スコアの1の位の基本になる位置(いい感じの値) 
    const int TOTAL_TIME = 500;

    private int[] TimeStack;     //スコアを格納する配列

    private int TimeMinute;     //分のカウント
    private int TimeSeconds;    //秒のカウント
    private int TotalTime;      //制限時間

    private GameObject[] TimeArray;    //スコアの桁数
    private float Timeleft;

    private GameObject colonObj;

    public GameObject TimePrefab;
    public Sprite[] SpriteArray;    //スコアのテクスチャ

    public GameObject colonPrefab;  //数字間の　「:」これ

    [SerializeField]
    private float padding;  //1文字の間隔

    [SerializeField]
    private float heightPadding;    //縦の間隔

    [SerializeField]
    private float colonPadding;     //コロンの間隔

    [SerializeField]
    Sprite numberSp;

    [SerializeField]
    Sprite colonSp;

    //private int TimeConma;      //秒以下の数字
    //public GameObject[] ConmaArray;    //コンマ専用配列
    //private int[] ConmaStack;           //コンマ専用スタック

    void Start()
    {
        //分,秒の初期化
        TimeMinute = 5;
        TimeSeconds = 60;
        TotalTime = TOTAL_TIME;

        //スコアの実際に表示される0~9の値を格納する変数
        TimeStack = new int[ TIME_MAX ];
        TimeArray = new GameObject[ TIME_MAX ];

        float colonSize = (float)colonSp.texture.width / 2.0f;
        //桁数分タイムを生成する
        float unitSize = ( float )numberSp.texture.width / 10.0f;   //1文字分の幅を取る

        for( int nCnt = 0 ; nCnt < TIME_MAX ; nCnt++ )
        {
            TimeArray[nCnt] = Instantiate(TimePrefab);
            Vector3 pos;
            pos.x = ((float)Screen.width / 2.0f) / 1.5f + unitSize * nCnt + (padding * nCnt); // 1920/2 /2
            pos.y = ((float)Screen.height / 2.0f) - 20.0f - heightPadding;
            pos.z = 0.0f;

            TimeArray[nCnt].transform.position = pos; //Time生成
            TimeArray[nCnt].transform.parent = gameObject.transform;   //生成されたTimeArrayに元のTimeに親子関係を紐づけする
        }
        for (int nCnt = 2; nCnt < 4; nCnt ++)
        {
            Vector3 PaddingPos;
            PaddingPos = new Vector3(colonSize, 0.0f, 0.0f);
            TimeArray[nCnt].transform.position += PaddingPos;
            TimeArray[nCnt].transform.parent = gameObject.transform;
        }
        colonObj = Instantiate(colonPrefab);
        Vector3 colonPos;
        colonPos.x = TimeArray[1].transform.position.x + colonSize * 1.5f;
        colonPos.y = TimeArray[1].transform.position.y;
        colonPos.z = 0.0f;

        colonObj.transform.position = colonPos;
        colonObj.transform.parent = gameObject.transform;
    }

    void Update()
    {
        Timeleft += Time.deltaTime;
        if( Timeleft >= 1.0f )
        {
            Timeleft = 0.0f;
            TimeSet( 1 );
        }
    }

    /************************************************************
    * 関数名：TimeSet
    * 引数:int Time
    * 戻り値:なし
    * 説明：スコアにどの値が入ったか調べる
    ***********************************************************/
    private void TimeSet( int Time )
    {
        //トータルタイムが一秒減る
        TotalTime -= Time;

        //秒が減る
        TimeSeconds--;
        //分の計算
        if( TimeSeconds < 0 )
        {
            TimeMinute -= 1;
            TimeSeconds = 59;
        }

        //入ってきたスコアをチェック
        TimeStack[ 0 ] = TimeMinute / 10 % 10;     //10の位
        TimeStack[ 1 ] = TimeMinute % 10;      //1の位

        TimeStack[ 2 ] = TimeSeconds / 10 % 10;       //10の位
        TimeStack[ 3 ] = TimeSeconds % 10;            //1の位

        //各TimeArrayにTimeStackに合ったスプライトに差し替える
        for( int nCnt = 0 ; nCnt < TIME_MAX ; nCnt++ )
        {
            TimeArray[ nCnt ].GetComponent<SpriteRenderer>().sprite = SpriteArray[ TimeStack[ nCnt ] ];
        }
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
