using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultTotalScore : MonoBehaviour {

    const int SCORE_MAX = 8;    //スコアの桁数

    private int curScore = 0;       //総スコアを格納する用
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

    [SerializeField] string curScoreKey;
    [SerializeField] string totalScoreKey;
    [SerializeField] bool debugFlag;

    [SerializeField] float posX;
    [SerializeField] float posY;

    [SerializeField] ResultScore curScoreObj;

    private void Awake()
    {
        if (debugFlag)
        {
            int t = PlayerPrefs.GetInt(totalScoreKey, 0);
            PlayerPrefs.SetInt(totalScoreKey, t + 10);
        }

        //curScore = PlayerPrefs.GetInt(curScoreKey, 0);
        totalScore = PlayerPrefs.GetInt(totalScoreKey, 0);
    }

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
            pos.x = transform.parent.transform.position.x + posX + unitSize * nCnt + (padding * nCnt);
            pos.y = transform.parent.transform.position.y + posY - heightPadding;
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
        this.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.0f);

        // HACK:スコア加算
        totalScore += curScoreObj.curScore;
    }

    // 更新
    void Update()
    {
        ScoreSet(totalScore);
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(totalScoreKey, totalScore);
        PlayerPrefs.Save();
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

    private void OnGUI()
    {
        //GUIStyleState styleState;
        //styleState = new GUIStyleState();
        //styleState.textColor = Color.white;
        //
        //GUIStyle guiStyle = new GUIStyle();
        //guiStyle.fontSize = 48;
        //guiStyle.normal = styleState;
        //
        //string str;
        //str = "現在スコア:" + curScore + "\n総スコア:" + totalScore;
        //
        //GUI.Label(new Rect(0, 400, 800, 600), str, guiStyle);
    }
}
