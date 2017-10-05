
/****************************************************************************
 * ファイル名:LifeCtrl.cs
 * タイトル:キャラクターコントローラー
 * 作成日：2016/10/17
 * 作成者：武内 優貴
 * 説明：スコアのテクスチャ処理
 * 更新履歴：10/17:+新規作成
 * **************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreCtrl : MonoBehaviour {
    const int SCORE_MAX = 8;    //スコアの桁数
    const float SCORE_DEFAULT_POS = 3.5f; //スコアの1の位の基本になる位置(いい感じの値) 

    private Vector3 pos;
    private int totalScore = 0;       //総スコアを格納する用
	private int[] scoreStack;     //スコアを格納する配列
    private int ScoreTest;
    private GameObject[] ScoreArray;    //スコアの桁数
    private int scoreValueCnt;          //桁数をカウントする用

	public GameObject UiCamera;		//UIのカメラ
	public GameObject ScorePrefab;
	public Sprite[] SpriteArray;    //スコアのテクスチャ
   
    [SerializeField]
    private float white_space;        //数字1つ1つの間隔を決める用
	
    void Start()
    {
        ScoreTest = 0;
        scoreValueCnt = 0;
        //スコアの実際に表示される0~9の値を格納する変数
        scoreStack = new int[SCORE_MAX];

		ScoreArray = new GameObject[SCORE_MAX];

        //桁数分スコアを生成する
        for (int nCnt = 0; nCnt < SCORE_MAX; nCnt++)
        {
            pos = new Vector3(ScorePrefab.transform.position.x + SCORE_DEFAULT_POS - (white_space * nCnt), 6.0f, 0.0f);

            ScoreArray[nCnt] = Instantiate(ScorePrefab, pos, Quaternion.identity); //Score生成

            ScoreArray[nCnt].transform.parent = gameObject.transform;   //生成されたScoreArrayに元のScoreに親子関係を紐づけする
		}
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ScoreTest += 1;
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            ScoreTest -= 1;
        }
        ScoreSet(ScoreTest);
    }

    /************************************************************
    * 関数名：ScoreSet
    * 引数:int score
    * 戻り値:なし
    * 説明：スコアにどの値が入ったか調べる
    ***********************************************************/
    public void ScoreSet(int score)
    {
        //トータルスコアに加算させる
        totalScore += score;

        //スコアの溢れチェック
        if (totalScore > 99999999)
        {
            totalScore = 99999999;
        }
        //スコアゼロチェックの代わり
        if( totalScore < 0 )
        {
            totalScore = 0;
        }

        //入ってきたスコアをチェック
        scoreStack[7] = totalScore / 10000000 % 10; //10000000の位
        scoreStack[6] = totalScore / 1000000 % 10;  //1000000の位
        scoreStack[5] = totalScore / 100000 % 10;   //100000の位
        scoreStack[4] = totalScore / 10000 % 10;    //10000の位
        scoreStack[3] = totalScore / 1000 % 10;     //1000の位
        scoreStack[2] = totalScore / 100 % 10;      //100の位
        scoreStack[1] = totalScore / 10 % 10;       //10の位
        scoreStack[0] = totalScore % 10;            //1の位 

        //各ScoreArrayにscoreStackに合ったスプライトに差し替える
        for (int nCnt = 0; nCnt < SCORE_MAX; nCnt++)
        {
            ScoreArray[nCnt].GetComponent<SpriteRenderer>().sprite = SpriteArray[scoreStack[nCnt]];
        }
        ScoreZeroCheck();   //Scoreのゼロチェック（仮）
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
                ScoreArray[nCnt].SetActive(true);
                scoreValueCnt += 1;
            }
            if( nCnt > scoreValueCnt)
            {
				ScoreArray[nCnt].SetActive(false);
            }
        }
    }
}
