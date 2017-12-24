﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerTogetherUI : MonoBehaviour {

    [SerializeField]
    private float faceOnMoveX;            //フェイスOnUIのサイズX
    [SerializeField]
    private float faceOnUilimit;

    private bool bEnd = false;             　//UI表示を終了

    //顔アイコンのスプライトの配列
    [SerializeField]
    private Sprite[] faceUiList;
    //顔アイコンのスプライトの影バージョン
    [SerializeField]
    private Sprite[] faceShadeUiList;

    //faceUiの表示するやつら
    [SerializeField]
    private FaceUI[] facePearUiArray;
    [SerializeField]
    private FaceUI[] faceGroupSmallUiArray;
    [SerializeField]
    private FaceUI[] faceGroupLargeUiArray;
    [SerializeField]
    private GameObject facePearUiObj;
    [SerializeField]
    private GameObject faceGroupSmallUiObj;
    [SerializeField]
    private GameObject faceGroupLargeUiObj;

    private int faceGroupNum;

    private Vector3 facePearUiDefPos;
    private Vector3 faceGroupSmallUiDefPos;
    private Vector3 faceGroupLargeUiDefPos;

    // 初期化
    void Start ()
    {
        faceGroupNum = 0;
        facePearUiDefPos = facePearUiObj.transform.position;
        faceGroupSmallUiDefPos = faceGroupSmallUiObj.transform.position;
        //Debug.Log("グループのポスy" + faceGroupSmallUiDefPos.x);
        faceGroupLargeUiDefPos = faceGroupLargeUiObj.transform.position;
        bEnd = false;
        //facePearUiObj.SetActive(false);
        //faceGroupSmallUiObj.SetActive(false);
        //faceGroupLargeUiObj.SetActive(false);
    }

    // 更新
    void Update ()
    {
        //UI表示が終了するとき
        if (bEnd)
        {
            switch (faceGroupNum)
            {
                //ペア
                case 2:
                        //UI表示が画面外にいくと
                    if (facePearUiObj.transform.localPosition.x <= -faceOnUilimit )
                     {
                         facePearUiArray[0].IsColor = false;
                         facePearUiArray[1].IsColor = false;
                         facePearUiObj.transform.localPosition = new Vector3( -1537.0f , -435.0f , 0.0f );
                        bEnd = false;
                        }
                    break;
                //グループ小
                case 3:
                        //UI表示が画面外にいくと
                    if(faceGroupSmallUiObj.transform.localPosition.x <= -faceOnUilimit)
                        {
                            faceGroupSmallUiArray[0].IsColor = false;
                            faceGroupSmallUiArray[1].IsColor = false;
                            faceGroupSmallUiArray[2].IsColor = false;
                        faceGroupSmallUiObj.transform.localPosition = new Vector3(-1537.0f, -435.0f, 0.0f);
                            bEnd = false;
                        }
                  break;
                //グループ大
                case 5:
                    //UI表示が画面外にいくと
                    if (faceGroupLargeUiObj.transform.localPosition.x <= -faceOnUilimit)
                    {
                        faceGroupLargeUiArray[0].IsColor = false;
                        faceGroupLargeUiArray[1].IsColor = false;
                        faceGroupLargeUiArray[2].IsColor = false;
                        faceGroupLargeUiArray[3].IsColor = false;
                        faceGroupLargeUiArray[4].IsColor = false;
                        faceGroupLargeUiObj.transform.localPosition = new Vector3(-1537.0f, -435.0f, 0.0f);
                        bEnd = false;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// UI表示をスタート
    /// </summary>
    /// <param name="groupNum">
    /// 乗車させるグループの人数
    /// </param>
    public void PassengerTogetherUIStart ( int groupNum , Human rideHuman )
    {
        //Debug.Log(groupNum);
        //グループの人数によってmargineを設定
        int activeIdx = 0;

        switch(groupNum)
        {
            //ペア
            case 2:
                activeIdx = rideHuman.CurrentModelType - Human.ModelType.LoversSt;

                facePearUiObj.gameObject.SetActive(true);
                facePearUiArray[ activeIdx ].IsColor = true;
                faceGroupNum = groupNum;
                //表示をスライドさせるよ
                iTween.MoveBy(facePearUiObj, iTween.Hash("x", faceOnMoveX, "easetype", "easeOutBounce", "time", 1.2f));
                break;
           //グループ小
            case 3:
                activeIdx = rideHuman.CurrentModelType - Human.ModelType.FamilySt;

                Debug.Log("入ったよ");
                faceGroupSmallUiObj.gameObject.SetActive(true);               
                faceGroupSmallUiArray[ activeIdx ].IsColor = true;
                faceGroupNum = groupNum;
                iTween.MoveBy(faceGroupSmallUiObj, iTween.Hash("x", faceOnMoveX, "easetype", "easeOutBounce", "time", 1.2f));
                break;
            //グループ大
            case 5:
                activeIdx = rideHuman.CurrentModelType - Human.ModelType.FriendsSt;

                faceGroupLargeUiObj.gameObject.SetActive(true);
                faceGroupLargeUiArray[ activeIdx ].IsColor = true;
                    faceGroupNum = groupNum;
                iTween.MoveBy(faceGroupLargeUiObj, iTween.Hash("x", faceOnMoveX, "easetype", "easeOutBounce", "time", 1.2f));
                break;
        } 
    }

    /// <summary>
    /// UI表示終了
    /// </summary>
    public void PassengerTogetherUIEnd()
    {
        //グループの人数によってmargineを設定
        switch (faceGroupNum)
        {
            //ペア
            case 2:

                //表示をスライドさせるよ
                iTween.MoveBy(facePearUiObj, iTween.Hash("x", -faceOnMoveX, "easetype", "easeInOutQuart", "time", 1.2f, "delay", 6.0f));

                bEnd = true;
                break;
            //グループ小
            case 3:
                iTween.MoveBy(faceGroupSmallUiObj, iTween.Hash("x", -faceOnMoveX, "easetype", "easeInOutQuart", "time", 1.2f, "delay", 6.0f));

                bEnd = true;
                break;
            //グループ大
            case 5:
                iTween.MoveBy(faceGroupLargeUiObj, iTween.Hash("x", -faceOnMoveX, "easetype", "easeInOutQuart", "time", 1.2f, "delay", 6.0f));

                bEnd = true;
                break;
        }
    }

    /// <summary>
    ///  フェイスUIをONにするよ
    /// </summary>
    /// <param name="rideCount">
    /// 今乗車させた人が何人目か
    /// </param>
    public void FaiceUION ( int rideCount , Human rideHuman )
    {
        rideCount--;
        int activeIdx = 0;

        switch( faceGroupNum)
        {
            //ペア
            case 2:
                activeIdx = rideHuman.CurrentModelType - Human.ModelType.LoversSt;
                facePearUiArray[ activeIdx ].IsColor = true;
                break;
            //グループ小
            case 3:
                activeIdx = rideHuman.CurrentModelType - Human.ModelType.FamilySt;
                faceGroupSmallUiArray[ activeIdx ].IsColor = true;
                break;
            //グループ大
            case 5:
                activeIdx = rideHuman.CurrentModelType - Human.ModelType.FriendsSt;
                faceGroupLargeUiArray[ activeIdx ].IsColor = true;
                break;
        }
    }
}
