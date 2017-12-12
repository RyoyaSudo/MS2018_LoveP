﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScore : MonoBehaviour {

    const int SCORE_MAX = 8;    //スコアの桁数

    private int totalScore = 0;       //総スコアを格納する用
    private int[] scoreStack;     //スコアを格納する配列
    private GameObject[] ScoreArray;    //スコアの桁数
    private GameObject ptsLogo;

    public GameObject ScorePrefab;
    public GameObject ptsLogoPrefab;
    public Sprite[] SpriteArray;    //スコアのテクスチャ

    [SerializeField]
    private float padding;  //1文字の間隔

    [SerializeField]
    private float heightPadding;    //縦の間隔

    [SerializeField]
    private float ptsPadding;   //ロゴの間隔

    [SerializeField]
    Sprite numberSp;


    // 初期化
    void Start()
    {
        //スコアの実際に表示される0~9の値を格納する変数
        scoreStack = new int[SCORE_MAX];

        ScoreArray = new GameObject[SCORE_MAX];

        //桁数分スコアを生成する
        float unitSize = (float)numberSp.texture.width / 10.0f;

        for (int nCnt = 0; nCnt < SCORE_MAX; nCnt++)
        {
            ScoreArray[nCnt] = Instantiate(ScorePrefab); //Score生成

            Vector3 pos;
            pos.x = ((float)Screen.width / 2.0f) / -1.05f + unitSize * nCnt + (padding * nCnt);
            pos.y = ((float)Screen.height / 2.0f) - 20.0f - heightPadding;
            pos.z = 0.0f;

            ScoreArray[nCnt].transform.position = pos;
            ScoreArray[nCnt].transform.parent = gameObject.transform;   //生成されたScoreArrayに元のScoreに親子関係を紐づけする

        }

        //Ptsを生成して配置する
        ptsLogo = Instantiate(ptsLogoPrefab);
        Vector3 ptsPos;
        ptsPos.x = ScoreArray[7].transform.position.x + unitSize + ptsPadding * SCORE_MAX;  //スコアの最後の位置から離して配置
        ptsPos.y = ScoreArray[7].transform.position.y;   //縦の位置
        ptsPos.z = 0.0f;
        ptsLogo.transform.position = ptsPos;
        ptsLogo.transform.parent = gameObject.transform;

    }

    // 更新
    void Update()
    {
        ScoreSet(0);
    }

    /************************************************************
    * 関数名：ScoreSet
    * 引数:int score
    * 戻り値:なし
    * 説明：スコアにどの値が入ったか調べる
    ***********************************************************/
    public void ScoreSet(int score)
    {
        //ゲームシーンで保存したスコアを入れてる
        totalScore = PlayerPrefs.GetInt("scorKey");

        //トータルスコアに加算させる
        totalScore += score;

        //スコアの溢れチェック
        if (totalScore > 99999999)
        {
            totalScore = 99999999;
        }
        //スコアゼロチェックの代わり
        if (totalScore < 0)
        {
            totalScore = 0;
        }

        //入ってきたスコアをチェック
        scoreStack[0] = totalScore / 10000000 % 10; //10000000の位
        scoreStack[1] = totalScore / 1000000 % 10;  //1000000の位
        scoreStack[2] = totalScore / 100000 % 10;   //100000の位
        scoreStack[3] = totalScore / 10000 % 10;    //10000の位
        scoreStack[4] = totalScore / 1000 % 10;     //1000の位
        scoreStack[5] = totalScore / 100 % 10;      //100の位
        scoreStack[6] = totalScore / 10 % 10;       //10の位
        scoreStack[7] = totalScore % 10;            //1の位 

        //各ScoreArrayにscoreStackに合ったスプライトに差し替える
        for (int nCnt = 0; nCnt < SCORE_MAX; nCnt++)
        {
            ScoreArray[nCnt].GetComponent<SpriteRenderer>().sprite = SpriteArray[scoreStack[nCnt]];
        }

        //ScoreZeroCheck();   //Scoreのゼロチェック（仮）
    }

    /************************************************************
    * 関数名：ScoreZeroCheck
    * 引数:なし
    * 戻り値:なし
    * 説明：スコアの使われている桁を表示させて使われていない桁を非表示にする
    ***********************************************************/
    private void ScoreZeroCheck()
    {
        for (int nCnt = 0; nCnt < SCORE_MAX; nCnt++)
        {
            if (scoreStack[nCnt] > 0)
            {
                for (; nCnt < SCORE_MAX; nCnt++)
                {
                    ScoreArray[nCnt].SetActive(true);
                }
                break;
            }
            else
            {
                ScoreArray[nCnt].SetActive(true);
            }
        }
    }
}