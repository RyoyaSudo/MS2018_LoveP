using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerTogetherUI : MonoBehaviour {

    public GameObject faiceBgUIPrefab;       //フェイスBgUIプレファブ
    public GameObject faiceUIPrefab;         //フェイスUIプレファブ
    public Sprite faiceOnUISprite;           //フェイスOnUIのスプライト
    public Sprite faiceBgUISprite;           //フェイスBgUIのスプライト
    public float marginePear;                //フェイスUIの左右の幅(ペア)
    public float margineSmall;               //フェイスUIの左右の幅(小)
    public float margineBig;                 //フェイスUIの左右の幅(大)

    private GameObject faiceBgUIObj=null;    //フェイスBgUIオブジェクト
    private GameObject []faiceUIObj=null;    //フェイスUIオブジェクト
    private float faiceBgUISizeX;            //フェイスBgUIのサイズX
    private float faiceBgUISizeY;            //フェイスBgUIのサイズY
    private float faiceOnUISizeX;            //フェイスOnUIのサイズX
    private bool bEnd = false;             　//UI表示を終了
    private float margine;                   //フェイスUIの左右の幅

    // 初期化
    void Start ()
    {
        //フェイスBgUIの幅と高さ
        faiceBgUISizeX = faiceBgUISprite.texture.width;
        faiceBgUISizeY = faiceBgUISprite.texture.height;
        //フェイスOnUIの幅
        faiceOnUISizeX = faiceOnUISprite.texture.width;
    }

    // 更新
    void Update ()
    {
        //UI表示が終了するとき
        if (bEnd)
        {
            //UI表示が画面外にいくと
            if (faiceBgUIObj.transform.localPosition.x < -((float)Screen.width / 2.0f - faiceBgUISizeX / 2.0f) - faiceBgUISizeX)
            {
                //フェイスBgUIオブジェクト消去
                Destroy(faiceBgUIObj);

                bEnd = false;
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

        //グループの人数によってmargineを設定
        switch(groupNum)
        {
            //ペア
            case 2:
                margine = marginePear;
                break;
           //グループ小
            case 3:
                margine = margineSmall;
                break;
            //グループ大
            case 5:
                margine = margineBig;
                break;
        }


        float offset = -faiceBgUISizeX / 2;
        float padding = (faiceBgUISizeX - (margine * 2) - (faiceOnUISizeX * groupNum)) / (groupNum * 2);    //フェイスUIの間隔
        offset += margine;

        //フェイスOffUI
        for (int nCnt = 0; nCnt < groupNum; nCnt++)
        {
            //フェイスOffUI生成
            faiceUIObj[nCnt] = Instantiate(faiceUIPrefab);

            //親をBgUIに設定
            faiceUIObj[nCnt].transform.parent = faiceBgUIObj.transform;

            //位置設定
            offset += padding;
            offset += faiceOnUISizeX/2;
            pos.x = offset;
            offset += padding;
            offset += faiceOnUISizeX / 2;
            pos.y = 0.0f;
            pos.z = -0.1f;
            faiceUIObj[nCnt].transform.localPosition = pos;
            faiceUIObj[nCnt].transform.localRotation = Quaternion.identity;
        }

        //表示をスライドさせるよ
        iTween.MoveBy(faiceBgUIObj, iTween.Hash("x", faiceBgUISizeX, "easetype", "easeOutBounce", "time", 1.2f));
    }

    /// <summary>
    /// UI表示終了
    /// </summary>
    public void PassengerTogetherUIEnd ()
    {
        //表示をスライドさせるよ
        iTween.MoveBy(faiceBgUIObj, iTween.Hash("x", -faiceBgUISizeX, "easetype", "easeInOutQuart", "time", 1.2f));

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
