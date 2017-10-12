using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour {

    //グループタイプ
   public enum GROUPTYPE
    {
        PEAR ,      //ペア
        SMAlLL ,    //小グループ
        BIG         //大グループ
    };

    //状態
    public enum STATETYPE
    {
        CREATE ,    //生成
        READY ,     //待機
        EVADE ,     //回避
        RIDE ,      //乗車
        GETOFF ,    //下車
        TRANSPORT , //運搬
        RELEASE ,   //解散
        DESTROY     //消去
    };

    //宣言
    public GROUPTYPE groupType;
    public STATETYPE stateType;

    //乗車人数
    public int pearRideNum;
    public int smallRideNum;
    public int bigRideNum;

    //乗車させる人数
    int maxRideNum;

    //現在の乗車人数
    //int currentRideNum=0;

	// 初期化
	void Start ()
    {
        //状態を「生成」に
        stateType = STATETYPE.CREATE;

        //乗車させる人数を設定
        switch (groupType)
        {
            //ペア
            case GROUPTYPE.PEAR:
                maxRideNum = pearRideNum;
                break;

            //小グループ
            case GROUPTYPE.SMAlLL:
                maxRideNum = smallRideNum;
                break;

            //大グループ
            case GROUPTYPE.BIG:
                maxRideNum = bigRideNum;
                break;
        }
    }
	
	// 更新
	void Update ()
    {
        //状態
        switch (stateType)
        {
            //生成
            case STATETYPE.CREATE:
                //状態を「待機」に
                stateType = STATETYPE.READY;
                break;

            //待機
            case STATETYPE.READY:
                break;

            //回避
            case STATETYPE.EVADE:
                break;

            //乗車
            case STATETYPE.RIDE:
   
                break;

            //下車
            case STATETYPE.GETOFF:

                break;

            //運搬
            case STATETYPE.TRANSPORT:
                break;

            //解散
            case STATETYPE.RELEASE:
                break;

            //消去
            case STATETYPE.DESTROY:
                Destroy(this.gameObject);
                break;
        }
    }
}