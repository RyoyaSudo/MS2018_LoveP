using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 乗客クラス
/// </summary>
/// <remarks>
/// 乗客と街人もこのクラスで生成。内部で振る舞いを決めさせる。
/// </remarks>
public class PassengerController : MonoBehaviour
{
    private GameObject playerObj;                    //プレイヤーオブジェ
    [SerializeField] private string playerPath;      //プレイヤーパス

    /// <summary>
    /// 乗車
    /// </summary>
    public float rideRunTime;                        //走る時間
    public float rideWaitTime;                       //待つ時間
    public float rideJumpTime;                       //ジャンプ時間
    private float rideCnt;                           //カウント
    private Vector3 rideStartPos;                    //最初の位置
    private Vector3 rideEndPos;                      //最後の位置
    private Vector3 rideMiddlePos;                   //中間の位置
    private float rideMoveRate;                      //移動割合
    private float rideJumpRate;                      //ジャンプ割合
    private enum RideType                            //状態
    {
        RUN ,
        WAIT ,
        JUMP
    };
    private RideType rideType;

    public float getOffTime;                         //下車するまでの時間
    private float getOffCnt;                         //下車カウント


    //乗客がどのグループなのかUI
    public GameObject passengerGroupUIEnptyPrefab;   //空プレファブ
    public GameObject passengerGroupUIPlanePrefab;   //プレーンプレファブ
    public Vector3 passengerGroupUIPos;              //位置
    public Material passengerGroupUIPearMat;         //ペアマテリアル
    public Material passengerGroupUISmallMat;        //小グループマテリアル
    public Material passengerGroupUIBigMat;          //大グループマテリアル
    public Material passengerGroupUIHeartMat;        //ハートマテリアル
    private GameObject passengerGroupUIEnptyObj;     //空オブジェ

    //タイムラインマネージャー
    [SerializeField] private string timelineMangerPath ;    //パス
    private TimelineManager timelineManager;                //オブジェクト
    
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

    //宣言
    public GROUPTYPE groupType;     // 所属するグループを示す変数
    public int spawnPlace;          //スポーン場所

    public SpawnPoint.PASSENGER_ORDER pasengerOrder;    //乗客の乗車順番

    /// <summary>
    /// 待機時間用カウンタ。
    /// 10/24現在、この時間を元にオブジェクト消去判定を行うこともある。
    /// </summary>
    public float destroyTime;

    /// <summary>
    /// 人オブジェクト
    /// 状態参照用に利用
    /// </summary>
    private Human humanObj;

    /// <summary>
    /// Awake時処理
    /// </summary>
    private void Awake()
    {
        humanObj = gameObject.GetComponent<Human>();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        //状態を「生成」に
        SetStateType( Human.STATETYPE.CREATE );

        //乗客がどのグループかUI生成
        PassengerGroupUICreate();

        //プレイヤーオブジェクト取得
        playerObj = GameObject.Find(playerPath);

        //タイムラインマネージャー取得
        timelineManager = GameObject.Find(timelineMangerPath).GetComponent<TimelineManager>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        //状態
        switch( humanObj.CurrentStateType )
        {
            //生成
            case Human.STATETYPE.CREATE:
                Create();
                break;

            //待機
            case Human.STATETYPE.READY:
                Ready();
                break;

            //回避
            case Human.STATETYPE.EVADE:
                Evade();
                break;

            //乗車
            case Human.STATETYPE.RIDE:
                Ride();
                break;

            //下車
            case Human.STATETYPE.GETOFF:
                GetOff();
                break;

            //運搬
            case Human.STATETYPE.TRANSPORT: 
                Transport();
                break;

            //解散
            case Human.STATETYPE.RELEASE:
                Release();
                break;

            //待ち受け
            case Human.STATETYPE.AWAIT:
                Await();
                break;

            //消去
            case Human.STATETYPE.DESTROY:
                Destroy();
                break;
        }
    }

    /// <summary>
    /// 状態をセット
    /// </summary>
    /// <param name="type">
    /// 状態
    /// </param>
    public void SetStateType( Human.STATETYPE type )
    {
        // HACK: 乗客の状態設定に関して
        //       状態設定はHuman.csで、参照をここで行うほうがいいような感じがする。
        //       現状はここからHuman.csに対して状態をセット。
        humanObj.CurrentStateType = type;

        switch( type )
        {
            //乗車
            case Human.STATETYPE.RIDE:
                rideType = RideType.RUN;
                rideCnt = 0;

                rideStartPos = transform.position;                                   //スタート位置
                rideEndPos = playerObj.transform.position;                           //終了位置

                Vector3 direction = rideStartPos - rideEndPos;
                direction = direction.normalized;
                rideMiddlePos = playerObj.transform.position + direction * 2.0f;    //中間位置

                rideMoveRate = 1.0f / rideRunTime;                                  //移動割合
                rideJumpRate = Mathf.PI / rideJumpTime;                             //ジャンプ割合
                transform.LookAt( playerObj.transform );                            //プレイヤーの位置を向かせる

                Destroy( passengerGroupUIEnptyObj );                                //乗客がどのグループなのかUI削除

                //乗車タイムライン開始
                timelineManager.Get("RideTimeline").Play();
                break;

            //下車
            case Human.STATETYPE.GETOFF:
                getOffCnt = 0;

                //下車タイムライン開始
                timelineManager.Get("GetOffTimeline").Play();
                break;

            //運搬
            case Human.STATETYPE.TRANSPORT:
                //親をプレイヤーにする
                gameObject.transform.parent = playerObj.transform;
                break;

            //待ち受け
            case Human.STATETYPE.AWAIT:
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
        SetStateType( Human.STATETYPE.READY );
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
        switch( rideType )
        {
            case RideType.RUN:
                //指定時間がたつと
                if (rideCnt >= rideRunTime)
                {
                    //中間位置に移動
                    transform.position = rideMiddlePos;
                    rideType = RideType.WAIT;
                    rideCnt = 0;
                    Debug.Log("Run");
                }
                else
                {
                    //乗車アニメーションをさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().RideAnimON();

                    //プレイヤー位置まで移動
                    rideCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(rideStartPos, rideMiddlePos, rideMoveRate * rideCnt);
                }
                break;

            case RideType.WAIT:
                if (rideCnt >= rideWaitTime)
                {
                    //乗車ジャンプアニメーションをさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().RideJumpAnimOn();

                    rideType = RideType.JUMP;
                    rideCnt = 0;
                    rideMoveRate = 1.0f / rideJumpTime;  //移動割合
                    Debug.Log("Wait");

                }
                else
                {
                    rideCnt += Time.deltaTime;
                }
                break;

            case RideType.JUMP:
                if (rideCnt >= rideJumpTime)
                {
                    //中間位置に移動
                    transform.position = rideEndPos;

                    //「運搬」状態に
                    SetStateType( Human.STATETYPE.TRANSPORT );
                    Debug.Log("Jump");

                }
                else
                {
                    //プレイヤー位置まで移動
                    rideCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(rideMiddlePos, rideEndPos, rideMoveRate * rideCnt);

                    Vector3 pos;
                    pos = transform.position;
                    pos.y += Mathf.Sin(rideJumpRate * rideCnt)*2.0f;
                    transform.position = pos;
                }
                break;
        }
    }

    /// <summary>
    /// 下車
    /// </summary>
    private void GetOff()
    {
        //下車アニメーションをさせる
        humanObj.ModelObj.GetComponent<HumanAnim>().GetoffAnimON();

        //指定時間がたつと
        if (getOffCnt >= getOffTime)
        {
            //「解散」状態に
            SetStateType( Human.STATETYPE.RELEASE );
        }
        else
        {
            getOffCnt += Time.deltaTime;
        }

    }

    /// <summary>
    /// 運搬
    /// </summary>
    private void Transport()
    {
        //運搬アニメーションをさせる
        humanObj.ModelObj.GetComponent<HumanAnim>().TransportAnimON();
    }

    /// <summary>
    /// 解散
    /// </summary>
    private void Release ()
    {
        //解散アニメーションをさせる
        humanObj.ModelObj.GetComponent<HumanAnim>().ReleaseAnimON(); // TODO: 11/7現在。乗客の状態に解散状態が設定されることがないためここで解散アニメーションをしている

        destroyTime -= Time.deltaTime;

        if (destroyTime < 0.0f)
        {
            destroyTime = 0.0f;
            SetStateType( Human.STATETYPE.DESTROY );
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
            SetStateType( Human.STATETYPE.DESTROY );
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

