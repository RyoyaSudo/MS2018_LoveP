using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerTogetherUI : MonoBehaviour {

    public GameObject faiceBgUIPrefab;       //フェイスBgUIプレファブ
    public GameObject faiceUIPrefab;         //フェイスUIプレファブ
    public Sprite faiceOnUISprite;           //フェイスOnUIのスプライト
    public Sprite faiceBgUISprite;           //フェイスBgUIのスプライト
    public int rideNum;                      //乗車人数
    public float padding;                    //フェイスUIの間隔
    public float srideSpeed;                 //スライドさせるスピード

    private GameObject faiceBgUIObj=null;    //フェイスBgUIオブジェクト
    private GameObject []faiceUIObj=null;    //フェイスUIオブジェクト
    private float faiceBgUISizeX;            //フェイスBgUIのサイズX
    private float faiceBgUISizeY;            //フェイスBgUIのサイズY
    private bool bStart = false;             //UI表示をスタート
    private bool bEnd = false;             　//UI表示を終了
    private float srideCnt=0 ;               //UIをスライドさせるときに使うカウント


    // 初期化
    void Start ()
    {
        //フェイスBgUIの幅と高さ
        faiceBgUISizeX = faiceBgUISprite.texture.width;
        faiceBgUISizeY = faiceBgUISprite.texture.height;
    }

    // 更新
    void Update ()
    {
        if (Input.GetKeyDown("9"))
        {
            PassengerTogetherUIStart(rideNum);
        }
        if (Input.GetKeyDown("8"))
        {
            PassengerTogetherUIEnd();
        }

        if (Input.GetKeyDown("1"))
        {
            FaiceUION(1);
        }
        if (Input.GetKeyDown("2"))
        {
            FaiceUION(2);
        }
        if (Input.GetKeyDown("3"))
        {
            FaiceUION(3);
        }
        if (Input.GetKeyDown("4"))
        {
            FaiceUION(4);
        }
        if (Input.GetKeyDown("5"))
        {
            FaiceUION(5);
        }

        Vector3 pos;

        //UI表示がスタートしたら
        if (bStart)
        {
            srideCnt += srideSpeed;

            if (srideCnt > faiceBgUISizeX)
            {
                pos = faiceBgUIObj.transform.localPosition;
                pos.x = -((float)Screen.width / 2.0f - faiceBgUISizeX / 2.0f);
                faiceBgUIObj.transform.localPosition = pos;
                bStart = false;
                srideCnt = 0;
            }
            else
            {
                //UIをスライドさせる
                pos = faiceBgUIObj.transform.localPosition;
                pos.x += srideSpeed;
                faiceBgUIObj.transform.localPosition = pos;
            }
        }

        //UI表示が終了するとき
        if (bEnd)
        {
            if (srideCnt >= faiceBgUISizeX)
            {
                bEnd = false;
                srideCnt = 0;

                //フェイスBgUIオブジェクト消去
                Destroy(faiceBgUIObj);
            }
            else
            {
                //UIをスライドさせる
                pos = faiceBgUIObj.transform.localPosition;
                pos.x -= srideSpeed;
                faiceBgUIObj.transform.localPosition = pos;

                srideCnt += srideSpeed;
            }
        }
    }

    /// <summary>
    /// UI表示をスタート
    /// </summary>
    /// <param name="groupNum">
    /// 乗車させるグループの人数
    /// </param>
    public void PassengerTogetherUIStart ( int groupNum )
    {
        //フェイスBgUI生成
        faiceBgUIObj = Instantiate(faiceBgUIPrefab);

        //親を自分に設定
        faiceBgUIObj.transform.parent = gameObject.transform;

        //位置設定
        Vector3 pos;
        pos.x = -((float)Screen.width / 2.0f - faiceBgUISizeX / 2.0f) - faiceBgUISizeX;
        pos.y = -((float)Screen.height / 2.0f - faiceBgUISizeY / 2.0f);
        pos.z = 12.0f;
        faiceBgUIObj.transform.localPosition = pos;
        faiceBgUIObj.transform.localRotation = Quaternion.identity;

        //乗車人数分の配列を作る
        faiceUIObj = new GameObject[groupNum];

        //フェイスOffUI
        for (int nCnt = 0; nCnt < groupNum; nCnt++)
        {
            //フェイスOffUI生成
            faiceUIObj[nCnt] = Instantiate(faiceUIPrefab);

            //親をBgUIに設定
            faiceUIObj[nCnt].transform.parent = faiceBgUIObj.transform;

            //位置設定
            pos.x = -(faiceOnUISprite.texture.width * 2) + (nCnt * padding);
            pos.y = 0.0f;
            pos.z = -0.1f;
            faiceUIObj[nCnt].transform.localPosition = pos;
            faiceUIObj[nCnt].transform.localRotation = Quaternion.identity;
        }

        //スタートさせたよフラグをtrueに
        bStart = true;
    }

    /// <summary>
    /// UI表示終了
    /// </summary>
    public void PassengerTogetherUIEnd ()
    {
        bEnd = true;
    }

    /// <summary>
    ///  フェイスUIをONにするよ
    /// </summary>
    /// <param name="rideCount">
    /// 今乗車させた人が何人目か
    /// </param>
    public void FaiceUION ( int rideCount)
    {
        rideCount--;
        faiceUIObj[rideCount].GetComponent<SpriteRenderer>().sprite = faiceOnUISprite;
    }
}
