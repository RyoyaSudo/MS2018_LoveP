using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLogo : MonoBehaviour {

    [SerializeField] private GameObject startBack1OBj;   //背景1プレファブ
    [SerializeField] private GameObject startBack2Obj;   //背景2プレファブ
    [SerializeField] private GameObject startIconobj;    //アイコンプレファブ
    [SerializeField] private Sprite startBack1Sprite;    //背景1スプライト
    [SerializeField] private float srideInTime;          //スライドイン時間
    [SerializeField] private float srideOutTime;         //スライドアウト時間
    [SerializeField] private float scalingTime;          //スケーリング時間

    private float startBack1SizeX;       //背景1幅
    private float startBack1SizeY;       //背景1高さ
    private float count;                 //カウント


    public enum STATETYPE
    {
        NONE ,
        SRIDE_IN ,
        SCALING ,
        SRIDE_OUT ,
    }
    public STATETYPE stateType;

    // 初期化
    void Start()
    {
        //背景の幅高さ
        startBack1SizeX = startBack1Sprite.texture.width;
        startBack1SizeY = startBack1Sprite.texture.height;

        SetStateType(STATETYPE.NONE);
    }

    // 更新
    void Update()
    {
        switch (stateType)
        {
            case STATETYPE.NONE:
                break;

            case STATETYPE.SRIDE_IN:
                if ( count >= srideInTime)
                {
                    SetStateType(STATETYPE.SCALING);
                }
                else
                {
                    count += Time.deltaTime;
                }
                break;

            case STATETYPE.SCALING:
                if (count >= scalingTime)
                {
                    SetStateType(STATETYPE.SRIDE_OUT);
                }
                else
                {
                    count += Time.deltaTime;
                }
                break;

            case STATETYPE.SRIDE_OUT:
                if (count >= srideOutTime)
                {
                    SetStateType(STATETYPE.NONE);
                }
                else
                {
                    count += Time.deltaTime;
                }
                break;
        }
    }

    /// <summary>
    /// StartLogoを表示スタート
    /// </summary>
    /// <param name="type">
    /// 状態
    /// </param>
    public void SetStateType (STATETYPE type)
    {
        stateType = type;

        switch (stateType)
        {
            case STATETYPE.NONE:
                gameObject.SetActive(false);
                break;

            case STATETYPE.SRIDE_IN:
                {
                    count = 0;

                    //位置設定
                    Vector3 pos;
                    pos = Vector3.zero;
                    pos.x -= startBack1SizeX;
                    transform.localPosition = pos;

                    pos = startBack1OBj.transform.localPosition;
                    pos.z += 0.2f;
                    startBack1OBj.transform.localPosition = pos;

                    pos = startBack2Obj.transform.localPosition;
                    pos.z += 0.1f;
                    startBack2Obj.transform.localPosition = pos;

                    //表示をスライドさせるよ
                    iTween.MoveBy(gameObject, iTween.Hash("x", startBack1SizeX, "easetype", "easeOutExpo", "time", srideInTime));
                    break;
                }
            case STATETYPE.SCALING:
                {
                    count = 0;
                    //拡縮させるよ
                    iTween.PunchScale(startIconobj, iTween.Hash("x", 3, "y", 3, "time", scalingTime));
                    break;
                }

            case STATETYPE.SRIDE_OUT:
                {
                    count = 0;
                    //表示をスライドさせるよ
                    iTween.MoveBy(gameObject, iTween.Hash("x", startBack1SizeX, "easetype", "linearTween", "time", srideOutTime));
                }
                break;
        }
    }
}
