using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCtrl : MonoBehaviour {

	const int TIME_MAX = 6;    //スコアの桁数
	const float TIME_DEFAULT_POS = -1.5f; //スコアの1の位の基本になる位置(いい感じの値) 
    const int TOTAL_TIME = 500;

	private Vector3 pos;
	private int[] TimeStack;     //スコアを格納する配列

    private int TimeMinute;     //分のカウント
    private int TimeSeconds;    //秒のカウント
    private int TimeConma;      //秒以下の数字
    private int TotalTime;      //制限時間

    private GameObject[] TimeArray;    //スコアの桁数
	private float Timeleft;

	public GameObject TimePrefab;
	public Sprite[] SpriteArray;    //スコアのテクスチャ

	public float white_space;        //数字1つ1つの間隔を決める用

    public GameObject[] ConmaArray;    //コンマ専用配列
	private int[] ConmaStack;           //コンマ専用スタック

	void Start()
	{
        //分,秒の初期化
        TimeMinute = 5;
        TimeSeconds = 60;
        TimeConma = 0;
        TotalTime = TOTAL_TIME;

        //スコアの実際に表示される0~9の値を格納する変数
        TimeStack = new int[TIME_MAX];
		TimeArray = new GameObject[TIME_MAX];

		//桁数分スコアを生成する
		for (int nCnt = 0; nCnt < TIME_MAX; nCnt++)
		{
			pos = new Vector3(TimePrefab.transform.position.x + TIME_DEFAULT_POS - (white_space * nCnt), 6.0f, 0.0f);

			TimeArray[nCnt] = Instantiate(TimePrefab, pos, Quaternion.identity); //Time生成

			TimeArray[nCnt].transform.parent = gameObject.transform;   //生成されたTimeArrayに元のTimeに親子関係を紐づけする
		}
	}

	void Update()
	{
        Timeleft += Time.deltaTime;
        if (Timeleft >= 1.0f)
        {
            Timeleft = 0.0f;
            TimeSet(1);
        }
        Debug.Log(Timeleft);

        TimeConma++;
        ConmaSet(TimeConma);
	}

	/************************************************************
    * 関数名：TimeSet
    * 引数:int Time
    * 戻り値:なし
    * 説明：スコアにどの値が入ったか調べる
    ***********************************************************/
    private void TimeSet(int Time)
    {
		//トータルタイムが一秒減る
        TotalTime -= Time;

        //秒が減る
        TimeSeconds--;
        //分の計算
        if(TimeSeconds < 0)
        {
            TimeMinute -= 1;
            TimeSeconds = 59;
        }

		//入ってきたスコアをチェック
		//TimeStack[7] = totalTime / 10000000 % 10; //10000000の位
		//TimeStack[6] = totalTime / 1000000 % 10;  //1000000の位

		TimeStack[5] = TimeMinute / 10 % 10;   //10の位
		TimeStack[4] = TimeMinute % 10;    //1の0

		TimeStack[3] = TimeSeconds / 10 % 10;     //10の位
		TimeStack[2] = TimeSeconds % 10;      //1の位

        //TimeStack[1] = TimeConma % 10;       //10の位
		//TimeStack[0] = TimeConma % 10;            //1の位

		//各TimeArrayにTimeStackに合ったスプライトに差し替える
		for (int nCnt = 0; nCnt < TIME_MAX; nCnt++)
		{
			TimeArray[nCnt].GetComponent<SpriteRenderer>().sprite = SpriteArray[TimeStack[nCnt]];
		}
		//TimeZeroCheck();   //Timeのゼロチェック（仮）
	}

    private void ConmaSet( int Conma)
    {
        int TotalConma = Conma;

        TimeStack[0] = TotalConma % 10;
        TimeStack[1] = TotalConma % 10;

		TimeArray[0].GetComponent<SpriteRenderer>().sprite = SpriteArray[TimeStack[0]];
		TimeArray[1].GetComponent<SpriteRenderer>().sprite = SpriteArray[TimeStack[1]];
    }

	/************************************************************
    * 関数名：TimeZeroCheck
    * 引数:なし
    * 戻り値:なし
    * 説明：スコアの使われている桁を表示させて使われていない桁を非表示にする
    ***********************************************************/
	private void TimeZeroCheck()
	{
        int TimeValueCnt = TIME_MAX;          //桁数をカウントする用

		//スコアの0が入っている桁をスコアの最大桁数まで調べる
		while (TimeStack[TimeValueCnt] == 0 && TimeValueCnt > TIME_MAX)
		{
			TimeArray[TimeValueCnt].SetActive(false);
			if (TimeValueCnt >= TIME_MAX - 1)
			{
				TimeArray[TimeValueCnt].SetActive(true);
				break;
			}
			TimeValueCnt--;
		}
		//上で調べた桁以外の桁をすべて調べる
		for (int nCnt = TimeValueCnt; nCnt < TIME_MAX; nCnt++)
		{
			TimeArray[nCnt].SetActive(true);
		}
	}
}
