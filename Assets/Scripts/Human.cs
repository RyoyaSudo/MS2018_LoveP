using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour{

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
    public GROUPTYPE groupType;     // 所属するグループを示す変数
    public STATETYPE stateType;     // 自身の状態管理要変数
    public int spawnPlace;          //スポーン場所

    //モデル
    GameObject model;

    //人アタッチ
    public GameObject pearPrefab;   //ペア
    public GameObject smallPrefab;  //小グループ
    public GameObject bigPrefab;    //大グループ

    //乗車させる人数
    //int maxPassengerNum;

    //現在の乗車人数
    //int currentPassengerNum;

    // 初期化
    void Start ()
    {
        //状態を「生成」に
        stateType = STATETYPE.CREATE;

        ////乗車させる人数を設定
        //switch (groupType)
        //{
        //    //ペア
        //    case GROUPTYPE.PEAR:
        //        maxPassengerNum = pearPassengerNum;
        //        break;

        //    //小グループ
        //    case GROUPTYPE.SMAlLL:
        //        maxPassengerNum = smallPassengerNum;
        //        break;

        //    //大グループ
        //    case GROUPTYPE.BIG:
        //        maxPassengerNum = bigPassengerNum;
        //        break;
        //}
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

    /*****************************************************************************
    * 関数名:SetPassengerNum
    * 引数：passengerNum:乗客人数
    * 戻り値:0
    * 説明:乗客人数をセット
     *****************************************************************************/
    //public void SetPassengerNum ( int passengerNum )
    //{
    //    currentPassengerNum = passengerNum;

    //    //状態を「運搬」に
    //    stateType = STATETYPE.TRANSPORT;
    //}

    /*****************************************************************************
    * 関数名:SetStateType
    * 引数：type:状態
    * 戻り値:0
    * 説明:状態をセット
     *****************************************************************************/
    public void SetStateType ( STATETYPE type )
    {
        stateType = type;
    }

    /*****************************************************************************
    * 関数名:ModelCreate
    * 引数：type:状態
    * 戻り値:0
    * 説明:生成
     *****************************************************************************/
    public void ModelCreate( Human.GROUPTYPE groupType)
    {
        switch (groupType)
        {
            //ペア
            case Human.GROUPTYPE.PEAR:
                model = pearPrefab;
                break;

            //小グループ
            case Human.GROUPTYPE.SMAlLL:
                model = smallPrefab;
                break;

            //大グループ
            case Human.GROUPTYPE.BIG:
                model = bigPrefab;
                break;
        }

        //生成
        GameObject human = Instantiate(model,                                        //ゲームオブジェクト
                                               this.transform.position,             //位置
                                               Quaternion.identity) as GameObject;  //回転

        //自分の親を自分にする
        human.transform.parent = transform;
    }
}

