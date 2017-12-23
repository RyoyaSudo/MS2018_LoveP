using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPassengerContoller : MonoBehaviour {

    private GameObject playerObj;                    //プレイヤーオブジェ
    [SerializeField] private string playerPath;      //プレイヤーパス

    public string rocketPath;
    public GameObject rocketObj;

    /// <summary>
    /// 乗車
    /// </summary>
    public float rideRunTime;                        //走る時間
    public float rideWaitTime;                       //待つ時間
    public float rideJumpTime;                       //ジャンプ時間
    public float rideSitTime;                        //座る時間
    private float rideCnt;                           //カウント
    public Vector3 rideStartPos;                    //最初の位置
    private Vector3 rideEndPos;                      //終了の位置
    private Vector3 rideMiddlePos;                   //中間の位置
    private Quaternion sitStartRotation;             //座った時の回転の最初位置
    private Quaternion sitEndRotation;               //座った時の回転の終了位置
    private float rideMoveRate;                      //移動割合
    private float rideJumpRate;                      //ジャンプ割合
    private float sitMoveRate;                       //座るときの割合
    private enum RideType                            //状態
    {
        RUN,
        WAIT,
        JUMP,
        SIT
    };
    private RideType rideType;

    [SerializeField] private float awaitTime;                        //待ち状態の時間

    ////タイムラインマネージャー
    //[SerializeField] private string timelineMangerPath;    //パス
    //private TimelineManager timelineManager;                //オブジェクト

    /// <summary>
    /// 有効化フラグ
    /// </summary>
    public bool IsEnable { get; set; }

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
    //public GROUPTYPE groupType;     // 所属するグループを示す変数
    public int spawnPlace;          //スポーン場所

    public SpawnPoint.PASSENGER_ORDER pasengerOrder;    //乗客の乗車順番

    /// <summary>
    /// 待機時間用カウンタ。
    /// 10/24現在、この時間を元にオブジェクト消去判定を行うこともある。
    /// </summary>
    public float destroyTime;


    public HumanAnim humanAnim;

    //アニメーター
    Animator anim;

    /// <summary>
    /// Awake時処理
    /// </summary>
    private void Awake()
    {
        IsEnable = false;
        playerObj = this.gameObject;
        //rideStartPos = transform.position;                    //最初の位置
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        anim = this.GetComponent<Animator>();
        //状態を「生成」に

        //プレイヤーオブジェクト取得
         playerObj = GameObject.Find(playerPath);
       // playerObj = this.gameObject;
        //プレイヤー車両オブジェクト取得
        // playerVehicle = playerObj.GetComponent<PlayerVehicle>();

        //タイムラインマネージャー取得
        //timelineManager = GameObject.Find(timelineMangerPath).GetComponent<TimelineManager>();

        rocketObj = GameObject.Find(rocketPath);

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        //if (IsEnable == false)
        //{
        //    return;
        //}

        //Debug.Log("run");
        //anim.SetBool("Run", true);
        Ride();
    }

    /// <summary>
    /// 状態をセット
    /// </summary>
    /// <param name="type">
    /// 状態
    /// </param>
    public void SetStateType(Human.STATETYPE type)
    {
        switch (type)
        {
            //乗車
            case Human.STATETYPE.RIDE:
                {
                    float jumpDistance = 0;                                       //ジャンプする位置
                    Vector3 endPos = Vector3.zero;                              //終了位置
                    Quaternion endRotation = Quaternion.identity;               //座った時に向かせる方向
                    

                    jumpDistance += 5.0f;
                    endPos = rocketObj.transform.position;
                    endRotation = rocketObj.transform.rotation;
                    rideType = RideType.RUN;
                    rideCnt = 0;

                    rideStartPos = transform.position;                                          //スタート位置
                    rideEndPos = endPos;                                                        //終了位置
                    Vector3 direction = rideStartPos - rideEndPos;
                    direction = direction.normalized;
                    rideMiddlePos = playerObj.transform.position + direction * jumpDistance;    //中間位置
                    sitEndRotation = endRotation;                                               //座った時の最終方向

                    rideMoveRate = 1.0f / rideRunTime;                                          //移動割合
                    rideJumpRate = Mathf.PI / rideJumpTime;                                     //ジャンプ割合
                    sitMoveRate = 1.0f / rideSitTime;                                           //座った時の割合
                    transform.LookAt(playerObj.transform);                                      //プレイヤーの位置を向かせる
                    anim.SetBool("Run", true);
                    //乗車タイムライン開始
                    //timelineManager.Get("RideTimeline").Play();
                    //timelineManager.SetStateType(TimelineManager.STATETYPE.TIMELINE_START);
                    break;
                }
            //運搬
            case Human.STATETYPE.TRANSPORT:
                //親をプレイヤーにする
                gameObject.transform.parent = playerObj.transform;
                break;

            //待ち受け
            case Human.STATETYPE.AWAIT:
                transform.LookAt(playerObj.transform);                          //プレイヤーの位置を向かせる
                break;
        }
    }

    /// <summary>
    /// 乗車
    /// </summary>
    private void Ride()
    {
        switch (rideType)
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
                    //humanObj.ModelObj.GetComponent<HumanAnim>().RideAnimON();


                    //プレイヤー位置まで移動
                    rideCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(rideStartPos, rideMiddlePos, rideMoveRate * rideCnt);
                }
                break;

            case RideType.WAIT:
                if (rideCnt >= rideWaitTime)
                {
                    //乗車ジャンプアニメーションをさせる
                    //humanObj.ModelObj.GetComponent<HumanAnim>().RideJumpAnimOn();
                    transform.LookAt(rideEndPos);        //終了位置を向かせる
                    anim.SetBool("Jump", true);
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
                    //Destroy(gameObject);
                }
                else
                {
                    //プレイヤー位置まで移動
                    rideCnt += Time.deltaTime;
                    transform.position = Vector3.Lerp(rideMiddlePos, rideEndPos, rideMoveRate * rideCnt);

                    Vector3 pos;
                    pos = transform.position;
                    pos.y += Mathf.Sin(rideJumpRate * rideCnt) * 2.0f;
                    transform.position = pos;
                }
                break;

            case RideType.SIT:
                if (rideCnt >= rideSitTime)
                {
                    //「運搬」状態に
                    //humanObj.CurrentStateType = Human.STATETYPE.TRANSPORT;
                    //Destroy(gameObject);
                }
                else
                {
                    rideCnt += Time.deltaTime;
                    transform.rotation = Quaternion.Lerp(sitStartRotation, sitEndRotation, sitMoveRate * rideCnt);
                }
                break;
        }
    }

    public void RunStart()
    {
        anim.SetBool("Run", true);
    }

    public void JumpStart()
    {
        anim.SetBool("Jump", true);
    }


    /// <summary>
    /// 待ち受け
    /// </summary>
    private void Await()
    {
        awaitTime -= Time.deltaTime;

        if (awaitTime < 0.0f)
        {

            //humanObj.CurrentStateType = Human.STATETYPE.RELEASE;
        }
    }

    /// <summary>
    /// 消去
    /// </summary>
    private void Destroy()
    {
        Destroy(gameObject);
    }

}
