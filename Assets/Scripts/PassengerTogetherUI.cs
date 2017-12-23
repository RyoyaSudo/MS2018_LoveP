using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerTogetherUI : MonoBehaviour {

    [SerializeField]
    private float faceOnPearUiMoveX;            //フェイスOnUIのサイズX
    [SerializeField]
    private float faceOnGroupSmallUiMoveX;            //フェイスOnUIのサイズX
    [SerializeField]
    private float faceOnGroupLargeUiMoveX;            //フェイスOnUIのサイズX
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
    // 初期化
    void Start ()
    {
        faceGroupNum = 0;
        facePearUiObj.SetActive(false);
        faceGroupSmallUiObj.SetActive(false);
        faceGroupLargeUiObj.SetActive(false);
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
                            foreach (FaceUI child in facePearUiArray)
                            {
                                child.IsColor = false;
                            }
                            facePearUiObj.SetActive(false);
                            bEnd = false;
                        }
                    break;
                //グループ小
                case 3:
                        //UI表示が画面外にいくと
                    if(faceGroupSmallUiObj.transform.localPosition.x <= -faceOnUilimit)
                        {
                            foreach (FaceUI child in faceGroupSmallUiArray)
                            {
                                child.IsColor = false;
                            }
                            faceGroupSmallUiObj.SetActive(false);
                            bEnd = false;
                        }
                  break;
                //グループ大
                case 5:
                    //UI表示が画面外にいくと
                    if (faceGroupLargeUiObj.transform.localPosition.x <= faceOnUilimit)
                    {
                        foreach (FaceUI child in faceGroupLargeUiArray)
                        {
                            child.IsColor = false;
                        }
                        faceGroupLargeUiObj.SetActive(false);
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
    public void PassengerTogetherUIStart ( int groupNum )
    {
        //グループの人数によってmargineを設定
        switch(groupNum)
        {
            //ペア
            case 2:
                    facePearUiArray[0].IsColor = true;
                    facePearUiObj.gameObject.SetActive(true);
                    faceGroupNum = groupNum;
                    //表示をスライドさせるよ
                iTween.MoveBy(facePearUiObj, iTween.Hash("x", faceOnPearUiMoveX, "easetype", "easeOutBounce", "time", 1.2f));
                break;
           //グループ小
            case 3:
                    faceGroupSmallUiArray[0].IsColor = true;
                    faceGroupSmallUiObj.gameObject.SetActive(true);
                    faceGroupNum = groupNum;
                iTween.MoveBy(faceGroupSmallUiObj, iTween.Hash("x", faceOnGroupSmallUiMoveX, "easetype", "easeOutBounce", "time", 1.2f));
                break;
            //グループ大
            case 5:
                    faceGroupLargeUiArray[0].IsColor = true;
                    faceGroupLargeUiObj.gameObject.SetActive(true);
                    faceGroupNum = groupNum;
                iTween.MoveBy(faceGroupLargeUiObj, iTween.Hash("x", faceOnGroupLargeUiMoveX, "easetype", "easeOutBounce", "time", 1.2f));
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
                iTween.MoveBy(facePearUiObj, iTween.Hash("x", -faceOnPearUiMoveX, "easetype", "easeInOutQuart", "time", 1.2f));              
                break;
            //グループ小
            case 3:
                iTween.MoveBy(faceGroupSmallUiObj, iTween.Hash("x", -faceOnGroupSmallUiMoveX, "easetype", "easeInOutQuart", "time", 1.2f));
                break;
            //グループ大
            case 5:
                iTween.MoveBy(faceGroupLargeUiObj, iTween.Hash("x", -faceOnGroupLargeUiMoveX, "easetype", "easeInOutQuart", "time", 1.2f));                
                break;
        }
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
        //faiceUIObj[rideCount].GetComponent<SpriteRenderer>().sprite = faiceOnUISprite;
        switch (faceGroupNum)
        {
            //ペア
            case 2:
                foreach (FaceUI child in facePearUiArray)
                {
                    facePearUiArray[rideCount].IsColor = true;
                }
                break;
            //グループ小
            case 3:
                foreach (FaceUI child in faceGroupSmallUiArray)
                {
                    faceGroupSmallUiArray[rideCount].IsColor = true;
                }
                break;
            //グループ大
            case 5:
                foreach (FaceUI child in faceGroupLargeUiArray)
                {
                    faceGroupLargeUiArray[rideCount].IsColor = true;
                }
                break;
        }
    }
}
