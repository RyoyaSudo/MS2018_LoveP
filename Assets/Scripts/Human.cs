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
    private GameObject playerObj;                    //プレイヤーオブジェ
    [SerializeField] private string playerPath;      //プレイヤーパス

    public float rideTime;                           //乗車するまでの時間
    private float rideCnt;                           //乗車カウント
    private Vector3 rideStartPos;                    //乗車の最初の位置
    private Vector3 rideEndPos;                      //乗車の最後の位置
    private float rideMoveRate;                      //乗車するときの移動割合

    //乗客がどのグループなのかUI
    public GameObject passengerGroupUIEnptyPrefab;   //空プレファブ
    public GameObject passengerGroupUIPlanePrefab;   //プレーンプレファブ
    public Vector3 passengerGroupUIPos;              //位置
    public Material passengerGroupUIPearMat;         //ペアマテリアル
    public Material passengerGroupUISmallMat;        //小グループマテリアル
    public Material passengerGroupUIBigMat;          //大グループマテリアル
    public Material passengerGroupUIHeartMat;        //ハートマテリアル
    private GameObject passengerGroupUIEnptyObj;     //空オブジェ

    /// <summary>
    /// グループ種類。
    /// </summary>
    public enum GROUPTYPE
    {
        PEAR = 0,      // ペア
        SMAlLL,        // 小グループ
        BIG,           // 大グループ
        TYPE_MAX       // グループ総数
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
        AWAIT ,    // 待ち受け
        DESTROY    // 消去
    };

    //宣言
    public GROUPTYPE groupType;     // 所属するグループを示す変数
    public STATETYPE stateType;     // 自身の状態管理要変数
    public int spawnPlace;          //スポーン場所

    private GameObject modelObj;    //モデルオブジェクト

    public SpawnPoint.PASSENGER_ORDER pasengerOrder;    //乗客の乗車順番

    /// <summary>
    /// 待機時間用カウンタ。
    /// 10/24現在、この時間を元にオブジェクト消去判定を行うこともある。
    /// </summary>
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
        isProtect = false;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        //状態を「生成」に
        SetStateType(STATETYPE.CREATE);

        //乗客がどのグループかUI生成
        PassengerGroupUICreate();

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

        //プレイヤーオブジェクト取得
        playerObj = GameObject.Find(playerPath);
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
                Create();
                break;

            //待機
            case STATETYPE.READY:
                Ready();
                break;

            //回避
            case STATETYPE.EVADE:
                Evade();
                break;

            //乗車
            case STATETYPE.RIDE:
                Ride();
                break;

            //下車
            case STATETYPE.GETOFF:
                GetOff();
                break;

            //運搬
            case STATETYPE.TRANSPORT: 
                Transport();
                break;

            //解散
            case STATETYPE.RELEASE:
                Release();
                break;

            //待ち受け
            case STATETYPE.AWAIT:
                Await();
                break;

            //消去
            case STATETYPE.DESTROY:
                Destroy();
                break;
        }
    }

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

    /// <summary>
    /// 状態をセット
    /// </summary>
    /// <param name="type">
    /// 状態
    /// </param>
    public void SetStateType( STATETYPE type )
    {
        stateType = type;

        //状態
        switch (stateType)
        {
            //乗車
            case STATETYPE.RIDE:
                rideStartPos = transform.position;            //スタート位置
                rideEndPos = playerObj.transform.position;    //終了位置
                rideMoveRate = 1.0f / rideTime;               //移動割合
                transform.LookAt( playerObj.transform );      //プレイヤーの位置を向かせる

                Destroy( passengerGroupUIEnptyObj );         //乗客がどのグループなのかUI削除
                break;

            //運搬
            case STATETYPE.TRANSPORT:
                //親をプレイヤーにする
                gameObject.transform.parent = playerObj.transform;
                break;

            //待ち受け
            case STATETYPE.AWAIT:
                Destroy(passengerGroupUIEnptyObj);            //乗客がどのグループなのかUI削除
                break;
        }
    }

    /// <summary>
    /// 生成
    /// </summary>
    private void Create ()
    {
        //状態を「待機」に
        SetStateType(STATETYPE.READY);
    }

    /// <summary>
    /// 待機
    /// </summary>
    private void Ready()
    {
    }

    /// <summary>
    /// 回避
    /// </summary>
    private void Evade()
    {
    }

    /// <summary>
    /// 乗車
    /// </summary>
    private void Ride()
    {
        //乗車アニメーションをさせる
        modelObj.GetComponent<test>().RideAnimON();

        //指定時間がたつと
        if (rideCnt >= rideTime)
        {
            // 乗車位置に移動
            transform.position = rideEndPos;

            //「運搬」状態に
            SetStateType(STATETYPE.TRANSPORT);
        }
        else
        {
            //プレイヤー位置まで移動
            rideCnt += Time.deltaTime;
            transform.position = Vector3.Lerp( rideStartPos , rideEndPos , rideMoveRate * rideCnt );
        }
    }

    /// <summary>
    /// 下車
    /// </summary>
    private void GetOff()
    {
        //「解散」状態に
        SetStateType(STATETYPE.RELEASE);  // TODO: 10/24現在すぐに解散状態に遷移。後にマネージャー系クラスで制御予定。

        //下車アニメーションをさせる
        modelObj.GetComponent<test>().GetoffAnimON();
        //解散アニメーションをさせる
        modelObj.GetComponent<test>().ReleaseAnimON(); // TODO: 11/7現在。乗客の状態に解散状態が設定されることがないためここで解散アニメーションをしている
    }

    /// <summary>
    /// 運搬
    /// </summary>
    private void Transport()
    {
        //乗車アニメーションをさせる
        modelObj.GetComponent<test>().RideAnimON(); // TODO: 11/8現在。乗客の状態に乗車状態が設定されることがないためここで乗車アニメーションをしている

        //運搬アニメーションをさせる
        modelObj.GetComponent<test>().TransportAnimON();
    }

    /// <summary>
    /// 解散
    /// </summary>
    private void Release ()
    {
        destroyTime -= Time.deltaTime;

        if (destroyTime < 0.0f)
        {
            destroyTime = 0.0f;
            SetStateType(STATETYPE.DESTROY);
        }
    }

    /// <summary>
    /// 待ち受け
    /// </summary>
    private void Await()
    {
        destroyTime -= Time.deltaTime;

        if (destroyTime < 0.0f)
        {
            destroyTime = 0.0f;
            SetStateType(STATETYPE.DESTROY);
        }
    }

    /// <summary>
    /// 消去
    /// </summary>
    private void Destroy()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// モデル生成
    /// </summary>
    /// <param name="groupType">
    /// グループの種類
    /// </param>
    public void ModelCreate( Human.GROUPTYPE groupType )
    {
        // HACK: 乗客モデル生成処理
        //       見た目をどれにするか決める部分。法則を上手くつけて何とかしたい。
        switch( groupType )
        {
            case Human.GROUPTYPE.PEAR:
                modelID = 0;                          // 暫定 0 = ペアのため。後に乱数にかけるなどして変更。
                break;

            case Human.GROUPTYPE.SMAlLL:
                modelID = 1;                          // 暫定 1 = グループ小のため。後に乱数にかけるなどして変更。
                break;

            case Human.GROUPTYPE.BIG:
                modelID = 2;                          // 暫定 2 = グループ大のため。後に乱数にかけるなどして変更。
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
    /// 乗客がどのグループなのかUI生成
    /// </summary>
    public void PassengerGroupUICreate ()
    {
        //空のオブジェクト
        //生成
        passengerGroupUIEnptyObj = Instantiate(passengerGroupUIEnptyPrefab);

        //自分の親を自分にする
        passengerGroupUIEnptyObj.transform.parent = transform;

        //位置設定
        passengerGroupUIEnptyObj.transform.localPosition = passengerGroupUIPos;


        //プレーンオブジェクト
        //生成
        GameObject passengerGroupUIPlane = Instantiate(passengerGroupUIPlanePrefab);

        //自分の親を空のオブジェクトにする
        passengerGroupUIPlane.transform.parent = passengerGroupUIEnptyObj.transform;

        //位置設定
        passengerGroupUIPlane.transform.localPosition = new Vector3 (0.0f , 0.0f , 0.0f);


        //順番とグループによってマテリアル設定
        switch(pasengerOrder)
        {
            //最初
            case SpawnPoint.PASSENGER_ORDER.FIRST:
                switch (groupType)
                {
                    //ペア
                    case GROUPTYPE.PEAR:
                        passengerGroupUIPlane.GetComponent<Renderer>().material = passengerGroupUIPearMat;
                        break;
                    //小グループ
                    case GROUPTYPE.SMAlLL:
                        passengerGroupUIPlane.GetComponent<Renderer>().material = passengerGroupUISmallMat;
                        break;
                    //大グループ
                    case GROUPTYPE.BIG:
                        passengerGroupUIPlane.GetComponent<Renderer>().material = passengerGroupUIBigMat;
                        break;
                    default:
                        Debug.Log("どのグループにも所属してないよ");
                        break;
                }
                break;

            //それ以降
            case SpawnPoint.PASSENGER_ORDER.DEFOULT:
                //ハート
                passengerGroupUIPlane.GetComponent<Renderer>().material = passengerGroupUIHeartMat;
                break;
        }
    }

}

