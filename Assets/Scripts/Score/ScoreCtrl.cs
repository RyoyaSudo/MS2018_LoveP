﻿
/****************************************************************************
 * ファイル名:ScoreCtrl.cs
 * タイトル:スコアコントローラー
 * 作成日：2016/10/17
 * 作成者：武内 優貴
 * 説明：スコアのテクスチャ処理、Ptsのロゴの作成
 * 更新履歴：10/26:+新規作成
 * **************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreCtrl : MonoBehaviour
{
    private int totalScore = 0;       //総スコアを格納する用
    private int[] scoreStack;     //スコアを格納する配列

    [SerializeField]
    private GameObject[] ScoreArray;    //スコアの桁数

    private int scoreValueCnt;          //桁数をカウントする用

    public GameObject ScorePrefab;

    public Sprite[] SpriteArray;    //スコアのテクスチャ

    [SerializeField]
    Sprite numberSp;

    /// <summary>
    /// ゲームシーン管理クラスを保持する。
    /// ゲーム状態に応じたスコア加算処理を行うため
    /// </summary>
    Game gameCtrl;
    public string gameCtrlObjPath;

    /// <summary>
    /// 星フェイズ時にスコア加算された回数。
    /// この数値を利用して星フェイズのスコア算出を行う。
    /// </summary>
    int starPhaseAddScoreNum;

    /// <summary>
    /// スコアテーブル。
    /// グループの総数に応じて適時追加
    /// </summary>
    int[] scoreTable =
    {
        1000,
        1500,
        2500,
    };

    // デバッグフラグ(保存処理初期化などに利用)
    [SerializeField] bool debugFlag = false;
    //スコアの保存キー
    [SerializeField] string scorKey = "curScoreKey";

    public ScoreManager.State state;

    //スコアの加算された時のフラグ
    public bool scoreFlag;

    void Start()
    {
        starPhaseAddScoreNum = 1;

        scoreValueCnt = 0;
        //スコアの実際に表示される0~9の値を格納する変数
        scoreStack = new int[ ScoreArray.Length ];

        scoreFlag = false;
        //ScoreArray = new GameObject[ ScoreArray.Length ];

        ////桁数分スコアを生成する
        //float unitSize = ( float )numberSp.texture.width / 10.0f;

        //for( int nCnt = 0 ; nCnt < ScoreArray.Length ; nCnt++ )
        //{
        //    ScoreArray[ nCnt ] = Instantiate( ScorePrefab ); //Score生成

        //    Vector3 pos;
        //    pos.x = ((float)Screen.width / 2.0f) / -1.05f + unitSize * nCnt + (padding * nCnt);
        //    pos.y = ((float)Screen.height / 2.0f) - 20.0f - heightPadding;
        //    pos.z = 0.0f;

        //    ScoreArray[nCnt].transform.position = pos;
        //    ScoreArray[ nCnt ].transform.parent = gameObject.transform;   //生成されたScoreArrayに元のScoreに親子関係を紐づけする

        //}

        ////Ptsを生成して配置する
        //ptsLogo = Instantiate(ptsLogoPrefab);
        //Vector3 ptsPos;
        //ptsPos.x = ScoreArray[7].transform.position.x + unitSize + ptsPadding * ScoreArray.Length;  //スコアの最後の位置から離して配置
        //ptsPos.y = ScoreArray[7].transform.position.y;   //縦の位置
        //ptsPos.z = 0.0f;
        //ptsLogo.transform.position = ptsPos;
        //ptsLogo.transform.parent = gameObject.transform;

        //総スコアを保存
        PlayerPrefs.SetInt(scorKey, totalScore);

        // オブジェクトを取得
        gameCtrl = GameObject.Find( gameCtrlObjPath ).GetComponent<Game>();
    }

    void Update()
    {
        switch (state)
        {
            case ScoreManager.State.SCORE_STATE_STOP:
                {
                    break;
                }
            case ScoreManager.State.SCORE_STATE_RUN:
                {
                    //Lpロゴのアニメーションフラグ
                    scoreFlag = false;
                    //スコア加算
                    //if(Input.GetKeyDown(KeyCode.UpArrow))
                    //{
                    //    ScoreSet(100);
                    //}
                    break;
                }
        }
    }

    /************************************************************
    * 関数名：ScoreSet
    * 引数:int score
    * 戻り値:なし
    * 説明：スコアにどの値が入ったか調べる
    ***********************************************************/
    private void ScoreSet( int score )
    {
        //Lpロゴのアニメーションフラグ
        scoreFlag = true;

        //トータルスコアに加算させる
        totalScore += score;

        //スコアの溢れチェック
        if( totalScore > 99999999 )
        {
            totalScore = 99999999;
        }
        //スコアゼロチェックの代わり
        if( totalScore < 0 )
        {
            totalScore = 0;
        }

        //入ってきたスコアをチェック
        scoreStack[ 0 ] = totalScore / 10000000 % 10; //10000000の位
        scoreStack[ 1 ] = totalScore / 1000000 % 10;  //1000000の位
        scoreStack[ 2 ] = totalScore / 100000 % 10;   //100000の位
        scoreStack[ 3 ] = totalScore / 10000 % 10;    //10000の位
        scoreStack[ 4 ] = totalScore / 1000 % 10;     //1000の位
        scoreStack[ 5 ] = totalScore / 100 % 10;      //100の位
        scoreStack[ 6 ] = totalScore / 10 % 10;       //10の位
        scoreStack[ 7 ] = totalScore % 10;            //1の位 

        //各ScoreArrayにscoreStackに合ったスプライトに差し替える
        for( int nCnt = 0 ; nCnt < ScoreArray.Length ; nCnt++ )
        {
            ScoreArray[ nCnt ].GetComponent<SpriteRenderer>().sprite = SpriteArray[ scoreStack[ nCnt ] ];
        }
        ScoreZeroCheck();   //Scoreのゼロチェック（仮)

    }
    /************************************************************
    * 関数名：ScoreZeroCheck
    * 引数:なし
    * 戻り値:なし
    * 説明：スコアの使われている桁を表示させて使われていない桁を非表示にする
    ***********************************************************/
    private void ScoreZeroCheck()
    {
        for( int nCnt = 0 ; nCnt < ScoreArray.Length ; nCnt++ )
        {
            if( scoreStack[ nCnt ] > 0 )
            {
                for( ; nCnt < ScoreArray.Length ; nCnt++ )
                {
                    ScoreArray[ nCnt ].SetActive( true );
                }
                break;
            }
            else
            {
                ScoreArray[ nCnt ].SetActive( true );
            }
        }
    }

    //スコア加算関数
    public void AddScore( int groupNo )
    {
        iTween.PunchScale(this.gameObject, iTween.Hash(
        "x", 1,
        "y", 1,
        "delay", 0,
        "time", 2.0f)
        );

        int result = 0;

        // 計算部分
        switch( gameCtrl.PhaseParam )
        {
            case Game.Phase.GAME_PAHSE_CITY:
                result = CityPhaseScoreCal( groupNo );
                break;

            case Game.Phase.GAME_PAHSE_STAR:
                result = StarPhaseScoreCal( groupNo );
                break;

            default:
                Debug.LogError( "スコア加算時に不正なフェイズを読み込みました" );
                break;
        }

        ScoreSet( result );
    }

    /// <summary>
    /// スコア加算処理( 数値直代入 )
    /// </summary>
    /// <param name="value">加算値</param>
    public void AddScoreValue( int value )
    {
        ScoreSet( value );
    }

    /// <summary>
    /// 街フェイズのスコア算出処理
    /// </summary>
    int CityPhaseScoreCal( int groupNo )
    {
        int result = scoreTable[ groupNo ];

        return result;
    }

    /// <summary>
    /// 星フェイズのスコア算出処理
    /// </summary>
    int StarPhaseScoreCal( int groupNo )
    {
        int result = 0;

        // TODO: 星フェイズスコア算出処理
        // 算出式は
        // 取得グループ別係数 * Exp( 2 * Log( addNum ) ) * addNum ^ 3
        // 後ほど変える可能性もあるかも？
        result = scoreTable[ groupNo ] * ( int )( Mathf.Exp( 2 * Mathf.Log( starPhaseAddScoreNum ) ) * Mathf.Pow( starPhaseAddScoreNum , 3 ) );

        // 後処理
        starPhaseAddScoreNum++;

        return result;
    }

    public void SaveScore()
    {
        // 今回の総スコアを保存
        PlayerPrefs.SetInt( scorKey , totalScore );
    }

    /// <summary>
    /// 破棄時処理
    /// </summary>
    private void OnDestroy()
    {
        
    }

    private void OnGUI()
    {
        if( Game.IsOnGUIEnable )
        {
            GUIStyleState styleState;
            styleState = new GUIStyleState();
            styleState.textColor = Color.white;

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 48;
            guiStyle.normal = styleState;

            string str;
            str = "現在スコア:" + totalScore;

            GUI.Label( new Rect( 0 , 400 , 800 , 600 ) , str , guiStyle );
        }
    }
}
