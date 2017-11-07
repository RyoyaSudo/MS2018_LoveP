using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 人クラス
/// </summary>
/// <remarks>
/// 乗客と街人もこのクラスで生成。内部で振る舞いを決めさせる。
/// </remarks>
public class Human : MonoBehaviour
{
    /// <summary>
    /// グループ種類。
    /// </summary>
    public enum GROUPTYPE
    {
        PEAR = 0,       // ペア
        SMAlLL,        // 小グループ
        BIG,            // 大グループ
        TYPE_MAX        // グループ総数
    };

    /// <summary>
    /// 人モデル配列。
    /// Inspector上で設定を忘れないこと！！
    /// </summary>
    public GameObject[] humanModelArray;

    /// <summary>
    /// 使用しているモデルのID。
    /// 他のクラスで乗客オブジェクトの子になっているモデルを参照する際に利用する。
    /// </summary>
    /// <remarks>
    /// 参照する際利用するのは、humanModelArray[ ModelID ].nameから得られる文字列。
    /// </remarks>
    public int ModelID
    {
        get { return modelID; }
    }

    int modelID;

    //状態
    public enum STATETYPE
    {
        CREATE,    // 生成
        READY,     // 待機
        EVADE,     // 回避
        RIDE,      // 乗車
        GETOFF,    // 下車
        TRANSPORT, // 運搬
        RELEASE,   // 解散
        DESTROY     // 消去
    };

    //宣言
    public GROUPTYPE groupType;     // 所属するグループを示す変数
    public STATETYPE stateType;     // 自身の状態管理要変数
    public int spawnPlace;          //スポーン場所

    private GameObject modelObj;    //モデルオブジェクト

    /// <summary>
    /// 待機時間用カウンタ。
    /// 10/24現在、この時間を元にオブジェクト消去判定を行うこともある。
    /// </summary>
    float destroyTimeCounter;
    public float destroyTime;

    /// <summary>
    /// 保護用フラグ変数。
    /// trueの時には無闇にDestoroyしないようにすること。
    /// </summary>
    public bool IsProtect
    {
        get { return isProtect; }
        set { isProtect = value; }
    } 

    bool isProtect;

    /// <summary>
    /// Awake時処理
    /// </summary>
    private void Awake()
    {
        modelID = -1;   // 不定のタイプとして初期値を負の値に設定
        destroyTimeCounter = 0.0f;
        isProtect = false;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
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

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        //状態
        switch( stateType )
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
                //
                break;

            //乗車
            case STATETYPE.RIDE:
                //乗車アニメーションをさせる
                modelObj.GetComponent<test>().RideAnimON();
                //運搬アニメーションをさせる
                modelObj.GetComponent<test>().TransportAnimON();    // TODO: 11/7現在。乗客の状態に運搬状態が設定されることがないためここで運搬アニメーションをしている
                break;

            //下車
            case STATETYPE.GETOFF:
                stateType = STATETYPE.RELEASE;      // TODO: 10/24現在すぐに解散状態に遷移。後にマネージャー系クラスで制御予定。

                //下車アニメーションをさせる
                modelObj.GetComponent<test>().GetoffAnimON();
                //解散アニメーションをさせる
                modelObj.GetComponent<test>().ReleaseAnimON(); // TODO: 11/7現在。乗客の状態に解散状態が設定されることがないためここで解散アニメーションをしている
                break;

            //運搬
            case STATETYPE.TRANSPORT:
                break;

            //解散
            case STATETYPE.RELEASE:
                // 消去判定を行う
                destroyTimeCounter += Time.deltaTime;

                if( DestoroyCheck() )
                {
                    stateType = STATETYPE.DESTROY;
                }
                break;

            //消去
            case STATETYPE.DESTROY:
                Destroy( this.gameObject );
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
    * 関数名:GetStateType
    * 引数：なし
    * 戻り値:状態を表す変数
    * 説明:状態を返す
    ******************************************************************************
    * 2017年11月1日
    * 佐藤峻一　がこの関数を付け加えました
    * ****************************************************************************/
    
    public int GetStateType()
    {
        switch (stateType)
        {
            //生成
            case STATETYPE.CREATE:
                return 0;
            case STATETYPE.READY:
                return 1;
            case STATETYPE.EVADE:
                return 2;
            case STATETYPE.RIDE:
                return 3;
            case STATETYPE.GETOFF:
                return 4;
            case STATETYPE.TRANSPORT:
                return 5;
            case STATETYPE.RELEASE:
                return 6;
            case STATETYPE.DESTROY:
                return 7;
        }
        return 0;
    }

    /*****************************************************************************
    * 関数名:SetStateType
    * 引数：type:状態
    * 戻り値:0
    * 説明:状態をセット
     *****************************************************************************/
    public void SetStateType( STATETYPE type )
    {
        stateType = type;
    }

    /*****************************************************************************
    * 関数名:ModelCreate
    * 引数：type:状態
    * 戻り値:0
    * 説明:生成
     *****************************************************************************/
    public void ModelCreate( Human.GROUPTYPE groupType )
    {
        switch( groupType )
        {
            case Human.GROUPTYPE.PEAR:
                modelID = 0;                          // TODO: 暫定 0 = ペアのため。後に乱数にかけるなどして変更。
                break;

            case Human.GROUPTYPE.SMAlLL:
                modelID = 1;                          // TODO: 暫定 1 = グループ小のため。後に乱数にかけるなどして変更。
                break;

            case Human.GROUPTYPE.BIG:
                modelID = 2;                          // TODO: 暫定 2 = グループ大のため。後に乱数にかけるなどして変更。
                break;

            default:
                modelID = -1;
                Debug.LogError( "人生成時に不定なタイプが指定されました。" );
                break;
        }

        // 生成
        modelObj = Instantiate(humanModelArray[ modelID ],                              //ゲームオブジェクト
                                               this.transform.position,                 //位置
                                               Quaternion.identity) as GameObject;      //回転

        // 自分の親を自分にする
        modelObj.transform.parent = transform;

        // 名前変更
        modelObj.name = humanModelArray[ modelID ].name;
    }

    /// <summary>
    /// 自身の子となっている表示用モデルの当たり判定取得処理。
    /// </summary>
    /// <remarks>
    /// 判定の消去などに利用すると良い。
    /// </remarks>
    public Collider GetHumanModelCollider()
    {
        string childName = humanModelArray[ modelID ].name;
        GameObject obj = transform.Find( childName ).gameObject;
        Collider collider = obj.GetComponent<Collider>();
        return collider;
    }

    /// <summary>
    /// 消去判定処理。
    /// </summary>
    /// <returns>
    /// 判定結果
    /// </returns>
    public bool DestoroyCheck()
    {
        bool flags = false;

        // TODO: 10/24現在、乗客消去は乗せ終わってからの時間に依存
        if( destroyTimeCounter > destroyTime )
        {
            flags = true;
        }

        return flags;
    }
}

