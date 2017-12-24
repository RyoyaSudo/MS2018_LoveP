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
    private PlayerVehicle playerVehicle;             //プレイヤー車両オブジェクト

    /// <summary>
    /// 乗車
    /// </summary>
    public float rideRunTime;                        //走る時間
    public float rideWaitTime;                       //待つ時間
    public float rideJumpTime;                       //ジャンプ時間
    public float rideSitTime;                        //座る時間
    private float rideCnt;                           //カウント
    private Vector3 rideStartPos;                    //最初の位置
    private Vector3 rideEndPos;                      //終了の位置
    private Vector3 rideMiddlePos;                   //中間の位置
    private Quaternion sitStartRotation;             //座った時の回転の最初位置
    private Quaternion sitEndRotation;               //座った時の回転の終了位置
    private float rideMoveRate;                      //移動割合
    private float rideJumpRate;                      //ジャンプ割合
    private float sitMoveRate;                       //座るときの割合
    private int rideNumber;                          //乗車順番
    public void SetRideNumber(int num)
    {
        rideNumber = num;
    }
    private enum RideType                            //状態
    {
        RUN,
        WAIT,
        JUMP,
        SIT
    };
    private RideType rideType;

    /// <summary>
    /// 下車
    /// </summary>
    public float getOffRunTime;                      //走るの時間
    public float getOffJumpTime;                     //ジャンプ時間
    private float getOffStartWaitTime;               //最初の待ち時間
    public float getOffPoseTime;                     //ポーズ時間
    private float getOffCnt;                         //カウント
    private Vector3 getOffStartPos;                  //最初の位置
    private Vector3 getOffEndPos;                    //最後の位置
    private GameObject getOffAwaitObj;               //待ち受け状態の人オブジェ
    public void SetGetOffAwaitObj(GameObject obj)
    {
        getOffAwaitObj = obj;
    }
    private Vector3 getOffMiddlePos;                 //中間の位置
    private float getOffMoveRate;                    //移動割合
    private float getOffJumpRate;                    //ジャンプ割合
    static bool bFriendsPoseStart;                   //フレンズポーズ開始
    static bool bFamilyPoseStart;                    //ファミリーポーズ開始
    private enum GetOffType                          //状態
    {
        STARTWAIT ,
        JUMP,
        RUN,
        WAIT,
        POSE,
    }
    private GetOffType getOffType;

    [SerializeField] private float awaitTime;        //待ち状態の時間

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
    /// 有効化フラグ
    /// </summary>
    public bool IsEnable{ get; set; }
    
    /// <summary>
    /// グループ種類。
    /// </summary>
    public enum GROUPTYPE
    {
        Lovers = 0,    // ペア
        Family,        // 小グループ
        Friends,       // 大グループ
        TypeMax        // グループ総数
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
        IsEnable = false;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        //状態を「生成」に
        humanObj.CurrentStateType = Human.STATETYPE.CREATE;

        //乗客がどのグループかUI生成
        PassengerGroupUICreate();

        //プレイヤーオブジェクト取得
        playerObj = GameObject.Find(playerPath);

        //プレイヤー車両オブジェクト取得
        playerVehicle = playerObj.GetComponent<PlayerVehicle>();                                                   

        //タイムラインマネージャー取得
        timelineManager = GameObject.Find(timelineMangerPath).GetComponent<TimelineManager>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        if( IsEnable == false )
        {
            return;
        }

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
        switch( type )
        {
            //乗車
            case Human.STATETYPE.RIDE:
                {
                    float jumpDistance=0;                                       //ジャンプする位置
                    Vector3 endPos = Vector3.zero;                              //終了位置
                    Quaternion endRotation = Quaternion.identity;               //座った時に向かせる方向
                    int rideCount=playerObj.GetComponent<Player>().RideCount-1; //現在の乗車人数

                    switch (playerVehicle.VehicleType)
                    {
                        case PlayerVehicle.Type.BIKE:
                            {
                                jumpDistance += 2.0f;
                                endPos = playerVehicle.BikeRidePoint[rideCount].transform.position;
                                endRotation = playerVehicle.BikeRidePoint[rideCount].transform.rotation;
                                break;
                            }
                        case PlayerVehicle.Type.CAR:
                            {
                                jumpDistance += 4.0f;
                                endPos = playerVehicle.CarRidePoint[rideCount].transform.position;
                                endRotation = playerVehicle.CarRidePoint[rideCount].transform.rotation;
                                break;
                            }
                        case PlayerVehicle.Type.BUS:
                            {
                                jumpDistance += 6.0f;
                                endPos = playerVehicle.BusRidePoint[rideCount].transform.position;
                                endRotation = playerVehicle.BusRidePoint[rideCount].transform.rotation;
                                break;
                            }
                    }
                    rideType = RideType.RUN;
                    rideCnt = 0;

                    rideStartPos = transform.position;                                          //スタート位置
                    rideEndPos = endPos;                                                        //終了位置
                    Vector3 direction = rideStartPos - rideEndPos;
                    direction = direction.normalized;
                    rideMiddlePos = playerObj.transform.position + direction * jumpDistance;    //中間位置
                    rideMiddlePos.y = transform.position.y;                                     // 高さを自分の現時点の高さに代入
                    sitEndRotation = endRotation;                                               //座った時の最終方向

                    rideMoveRate = 1.0f / rideRunTime;                                          //移動割合
                    rideJumpRate = Mathf.PI / rideJumpTime;                                     //ジャンプ割合
                    sitMoveRate = 1.0f / rideSitTime;                                           //座った時の割合
                    transform.LookAt(playerObj.transform);                                      //プレイヤーの位置を向かせる

                    Destroy(passengerGroupUIEnptyObj);                                          //乗客がどのグループなのかUI削除

                    //乗車タイムライン開始
                    timelineManager.Get("RideTimeline").Play();
                    timelineManager.SetStateType(TimelineManager.STATETYPE.TIMELINE_START);
                    break;
                }

            //下車
            case Human.STATETYPE.GETOFF:
                {
                    getOffType = GetOffType.STARTWAIT;
                    getOffCnt = 0;
                    getOffStartWaitTime = rideNumber * 1.0f;                            //最初の待ち時間

                    getOffStartPos = transform.position;                                //スタート位置
                    getOffEndPos = getOffAwaitObj.transform.position;                   //終了位置

                    Vector3 direction = getOffStartPos - getOffEndPos;
                    direction = direction.normalized;
                    getOffMiddlePos = getOffEndPos + direction * 2.0f;                  //中間位置
                    getOffMoveRate = 1.0f / getOffRunTime;                              //移動割合
                    getOffJumpRate = Mathf.PI / getOffJumpTime;                         //ジャンプ割合
                    transform.LookAt(getOffAwaitObj.transform);                         //待ち受け状態の人の方を向かせる

                    switch (groupType)
                    {
                        case GROUPTYPE.Lovers:
                            //下車タイムライン開始
                            timelineManager.Get("GetOffTimeline").Play();
                            break;

                        case GROUPTYPE.Family:
                            bFamilyPoseStart = false;
                            //下車タイムライン開始
                            timelineManager.Get("GetOffSmallTimeline").Play();
                            break;

                        case GROUPTYPE.Friends:
                            bFriendsPoseStart = false;
                            //下車タイムライン開始
                            timelineManager.Get("GetOffBigTimeline").Play();
                            break;
                    }
                    timelineManager.SetStateType(TimelineManager.STATETYPE.TIMELINE_START);
                    break;
                }

            //運搬
            case Human.STATETYPE.TRANSPORT:
                //親をプレイヤーにする
                gameObject.transform.parent = playerObj.transform;
                break;

            //待ち受け
            case Human.STATETYPE.AWAIT:
                Destroy(passengerGroupUIEnptyObj);                              //乗客がどのグループなのかUI削除
                transform.LookAt(playerObj.transform);                          //プレイヤーの位置を向かせる
                switch (groupType)
                {
                    case GROUPTYPE.Lovers:
                        break;

                    case GROUPTYPE.Family:
                        getOffType = GetOffType.WAIT;
                        humanObj.ModelObj.GetComponent<HumanAnim>().AwaitON();  //待ち受けアニメーションON
                        break;

                    case GROUPTYPE.Friends:
                        getOffType = GetOffType.WAIT;
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// 生成
    /// </summary>
    private void Create ()
    {
        //状態を「待機」に
        humanObj.CurrentStateType = Human.STATETYPE.READY;
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
                    transform.LookAt(rideEndPos);        //終了位置を向かせる

                    rideType = RideType.JUMP;
                    rideCnt = 0;
                    rideMoveRate = 1.0f / rideJumpTime;  //移動割合
                }
                else
                {
                    rideCnt += Time.deltaTime;
                }
                break;

            case RideType.JUMP:
                if (rideCnt >= rideJumpTime)
                {
                    rideType = RideType.SIT;
                    rideCnt = 0;

                    //終了位置に移動
                    transform.position = rideEndPos;

                    sitStartRotation = transform.rotation;
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

            case RideType.SIT:
                if (rideCnt >= rideSitTime)
                {
                    //「運搬」状態に
                    humanObj.CurrentStateType = Human.STATETYPE.TRANSPORT;
                }
                else
                {
                    rideCnt += Time.deltaTime;
                    transform.rotation = Quaternion.Lerp(sitStartRotation, sitEndRotation, sitMoveRate * rideCnt);
                }
                break;
        }
    }

    /// <summary>
    /// 下車
    /// </summary>
    private void GetOff()
    {
        switch(groupType)
        {
            case GROUPTYPE.Lovers:
                LoversGetOff();
                break;

            case GROUPTYPE.Family:
                FamilyGetOff();
                break;

            case GROUPTYPE.Friends:
                FriendsGetOff();
                break;
        }
    }
     
    /// <summary>
    /// Lovers下車
    /// </summary>
    void LoversGetOff ()
    {
        switch (getOffType)
        {
            case GetOffType.STARTWAIT:
                if (getOffCnt >= getOffStartWaitTime)
                {
                    getOffType = GetOffType.JUMP;
                    getOffCnt = 0;
                }
                else
                {
                    getOffCnt += Time.deltaTime;
                }
                break;

            case GetOffType.JUMP:
                if (getOffCnt >= getOffJumpTime)
                {
                    //中間位置と終了位置の再設定
                    getOffMiddlePos = transform.position;
                    getOffEndPos = getOffAwaitObj.transform.position + getOffAwaitObj.transform.right * 1.0f;
                    // HACK: 地面の高さを追加
                    //       靴が埋まるため
                    getOffEndPos.y = 0.2f;
                    transform.LookAt(getOffEndPos);        //終了位置を向かせる
                    getOffType = GetOffType.RUN;
                    getOffCnt = 0;
                }
                else
                {
                    //下車アニメーションをさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().GetoffAnimON();

                    //ジャンプ位置まで移動
                    getOffCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(getOffStartPos, getOffMiddlePos, getOffMoveRate * getOffCnt);

                    Vector3 pos;
                    pos = transform.position;
                    pos.y += Mathf.Sin(getOffJumpRate * getOffCnt) * 2.0f;
                    transform.position = pos;
                }

                break;

            case GetOffType.RUN:
                if (getOffCnt >= getOffRunTime)
                {
                    transform.LookAt(playerObj.transform);        //プレイヤーの位置を向かせる
                    getOffType = GetOffType.WAIT;
                    getOffCnt = 0;

                    //下車待ちアニメーションさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().GetoffWaitAnimON();
                }
                else
                {
                    //下車走りアニメーションさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().GetoffRunAnimON();

                    //指定位置まで移動
                    getOffCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(getOffMiddlePos, getOffEndPos, getOffMoveRate * getOffCnt);
                }
                break;

            case GetOffType.WAIT:
                //「解散」状態に
                humanObj.CurrentStateType = Human.STATETYPE.RELEASE;

                break;
        }
    }

    /// <summary>
    /// Family下車
    /// </summary>
    void FamilyGetOff()
    {
        switch (getOffType)
        {
            case GetOffType.STARTWAIT:
                if (getOffCnt >= getOffStartWaitTime)
                {
                    getOffType = GetOffType.JUMP;
                    getOffCnt = 0;
                }
                else
                {
                    getOffCnt += Time.deltaTime;
                }
                break;

            case GetOffType.JUMP:
                if (getOffCnt >= getOffJumpTime)
                {
                    //中間位置再設定
                    getOffMiddlePos = transform.position;

                    //終了位置再設定
                    switch (rideNumber)
                    {
                        case 0:
                            getOffEndPos = getOffAwaitObj.transform.position + getOffAwaitObj.transform.forward * 0.7f ;
                            break;

                        case 1:
                            getOffEndPos = getOffAwaitObj.transform.position + getOffAwaitObj.transform.right * 0.8f;
                            break;
                    }
                    // HACK: 地面の高さを追加
                    //       靴が埋まるため
                    getOffEndPos.y = 0.2f;
                    transform.LookAt(getOffEndPos);        //終了位置を向かせる
                    getOffType = GetOffType.RUN;
                    getOffCnt = 0;
                }
                else
                {
                    //下車アニメーションをさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().GetoffAnimON();

                    //ジャンプ位置まで移動
                    getOffCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(getOffStartPos, getOffMiddlePos, getOffMoveRate * getOffCnt);

                    Vector3 pos;
                    pos = transform.position;
                    pos.y += Mathf.Sin(getOffJumpRate * getOffCnt) * 2.0f;
                    transform.position = pos;
                }

                break;

            case GetOffType.RUN:
                if (getOffCnt >= getOffRunTime)
                {
                    transform.LookAt(playerObj.transform);        //プレイヤーの位置を向かせる
                    getOffType = GetOffType.WAIT;
                    getOffCnt = 0;

                    //最後の人が最終位置に到着したら
                    if (rideNumber == 1)
                    {
                        bFamilyPoseStart = true;
                    }

                    //下車待ちアニメーションさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().GetoffWaitAnimON();
                }
                else
                {
                    switch (rideNumber)
                    {
                        case 0:
                            //下車子供走りアニメーションさせる
                            humanObj.ModelObj.GetComponent<HumanAnim>().GetoffChildRunON();
                            break;

                        case 1:
                            //下車走りアニメーションさせる
                            humanObj.ModelObj.GetComponent<HumanAnim>().GetoffRunAnimON();
                            break;
                    }

                    //指定位置まで移動
                    getOffCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(getOffMiddlePos, getOffEndPos, getOffMoveRate * getOffCnt);
                }
                break;

            case GetOffType.WAIT:
                if (bFamilyPoseStart)
                {
                    getOffType = GetOffType.POSE;

                    if (gameObject.name == "mob_family_child")
                    {
                        humanObj.ModelObj.GetComponent<HumanAnim>().ChildPoseON();
                    }
                    else if (gameObject.name == "mob_family_girl")
                    {
                        humanObj.ModelObj.GetComponent<HumanAnim>().MotherPoseON();
                    }
                    else if (gameObject.name == "mob_family_man")
                    {
                        humanObj.ModelObj.GetComponent<HumanAnim>().FatherPoseON();
                    }
                }
                break;

            case GetOffType.POSE:
                if (getOffCnt >= getOffPoseTime)
                {
                    //「解散」状態に
                    humanObj.CurrentStateType = Human.STATETYPE.RELEASE;
                }
                else
                {
                    getOffCnt += Time.deltaTime;
                }
                break;
        }

    }

    /// <summary>
    /// Friends下車
    /// </summary>
    void FriendsGetOff()
    {
        switch (getOffType)
        {
            case GetOffType.STARTWAIT:
                if (getOffCnt >= getOffStartWaitTime)
                {
                    getOffType = GetOffType.JUMP;
                    getOffCnt = 0;
                }
                else
                {
                    getOffCnt += Time.deltaTime;
                }
                break;

            case GetOffType.JUMP:
                if (getOffCnt >= getOffJumpTime)
                {
                    //中間位置再設定
                    getOffMiddlePos = transform.position;

                    //終了位置再設定
                    switch (rideNumber)
                    {
                        case 0:
                            getOffEndPos = getOffAwaitObj.transform.position + getOffAwaitObj.transform.right * 0.6f - getOffAwaitObj.transform.forward * 0.5f;
                            break;

                        case 1:
                            getOffEndPos = getOffAwaitObj.transform.position - getOffAwaitObj.transform.right * 0.6f - getOffAwaitObj.transform.forward * 0.5f;
                            break;

                        case 2:
                            getOffEndPos = getOffAwaitObj.transform.position + getOffAwaitObj.transform.right * 1.1f;
                            break;

                        case 3:
                            getOffEndPos = getOffAwaitObj.transform.position - getOffAwaitObj.transform.right * 1.1f;
                            break;
                    }

                    // HACK: 地面の高さを追加
                    //       靴が埋まるため
                    getOffEndPos.y = 0.2f;

                    transform.LookAt(getOffEndPos);        //終了位置を向かせる
                    getOffType = GetOffType.RUN;
                    getOffCnt = 0;
                }
                else
                {
                    //下車アニメーションをさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().GetoffAnimON();

                    //ジャンプ位置まで移動
                    getOffCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(getOffStartPos, getOffMiddlePos, getOffMoveRate * getOffCnt);

                    Vector3 pos;
                    pos = transform.position;
                    pos.y += Mathf.Sin(getOffJumpRate * getOffCnt) * 2.0f;
                    transform.position = pos;
                }

                break;

            case GetOffType.RUN:
                if (getOffCnt >= getOffRunTime)
                {
                    transform.LookAt(playerObj.transform);        //プレイヤーの位置を向かせる
                    getOffType = GetOffType.WAIT;
                    getOffCnt = 0;

                    //最後の人が最終位置に到着したら
                    if (rideNumber==3)
                    {
                        bFriendsPoseStart = true;
                    }

                    //下車待ちアニメーションさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().GetoffWaitAnimON();
                }
                else
                {
                    //下車走りアニメーションさせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().GetoffRunAnimON();

                    //指定位置まで移動
                    getOffCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(getOffMiddlePos, getOffEndPos, getOffMoveRate * getOffCnt);
                }
                break;

            case GetOffType.WAIT:
                if (bFriendsPoseStart)
                {
                    getOffType = GetOffType.POSE;

                    //ポーズをとらせる
                    switch (rideNumber)
                    {
                        case 0:
                            humanObj.ModelObj.GetComponent<HumanAnim>().FriendsPose1ON();
                            break;

                        case 1:
                            humanObj.ModelObj.GetComponent<HumanAnim>().FriendsPose2ON();
                            break;

                        case 2:
                            humanObj.ModelObj.GetComponent<HumanAnim>().FriendsPose3ON();
                            break;

                        case 3:
                            humanObj.ModelObj.GetComponent<HumanAnim>().FriendsPose4ON();
                            break;
                    }

                }
                break;

            case GetOffType.POSE:
                if ( getOffCnt >= getOffPoseTime)
                {
                    //「解散」状態に
                    humanObj.CurrentStateType = Human.STATETYPE.RELEASE;

                }
                else
                {
                    getOffCnt += Time.deltaTime;
                }
                break;
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
            humanObj.CurrentStateType = Human.STATETYPE.DESTROY;
        }
    }

    /// <summary>
    /// 待ち受け
    /// </summary>
    private void Await()
    {
        switch(groupType)
        {
            case GROUPTYPE.Lovers:
                LoversAwait();
                break;

            case GROUPTYPE.Family:
                FamilyAwait();
                break;

            case GROUPTYPE.Friends:
                FriendsAwait();
                break;
        }
    }

    /// <summary>
    /// Lovers待ち受け
    /// </summary>
    void LoversAwait ()
    {
        awaitTime -= Time.deltaTime;

        if (awaitTime < 0.0f)
        {
            //「解散」状態に
            humanObj.CurrentStateType = Human.STATETYPE.RELEASE;
        }
    }

    /// <summary>
    /// Family待ち受け
    /// </summary>
    void FamilyAwait()
    {
        switch (getOffType)
        {
            case GetOffType.WAIT:
                if (bFamilyPoseStart)
                {
                    if (gameObject.name == "mob_family_girl")
                    {
                        humanObj.ModelObj.GetComponent<HumanAnim>().MotherPoseON();
                    }
                    else if (gameObject.name == "mob_family_man")
                    {
                        humanObj.ModelObj.GetComponent<HumanAnim>().FatherPoseON();
                    }
                    getOffCnt = 0;
                    getOffType = GetOffType.POSE;
                }
                break;

            case GetOffType.POSE:
                if (getOffCnt >= getOffPoseTime)
                {
                    //「解散」状態に
                    humanObj.CurrentStateType = Human.STATETYPE.RELEASE;

                }
                else
                {
                    getOffCnt += Time.deltaTime;
                }
                break;
        }

    }

    /// <summary>
    /// Friends待ち受け
    /// </summary>
    void FriendsAwait()
    {
        switch(getOffType)
        {
            case GetOffType.WAIT:
                if (bFriendsPoseStart)
                {
                    //ポーズをとらせる
                    humanObj.ModelObj.GetComponent<HumanAnim>().FriendsPose0ON();
                    getOffCnt = 0;
                    getOffType = GetOffType.POSE;
                }
                break;

            case GetOffType.POSE:
                if (getOffCnt >= getOffPoseTime)
                {
                    //「解散」状態に
                    humanObj.CurrentStateType = Human.STATETYPE.RELEASE;

                }
                else
                {
                    getOffCnt += Time.deltaTime;
                }
                break;
        }
    }

    /// <summary>
    /// 消去
    /// </summary>
    private void Destroy()
    {
        Destroy( gameObject );
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
                    case GROUPTYPE.Lovers:
                        passengerGroupUIPlane.GetComponent<Renderer>().material = passengerGroupUIPearMat;
                        break;
                    //小グループ
                    case GROUPTYPE.Family:
                        passengerGroupUIPlane.GetComponent<Renderer>().material = passengerGroupUISmallMat;
                        break;
                    //大グループ
                    case GROUPTYPE.Friends:
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

