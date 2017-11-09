﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    Rigidbody rb;
    float moveRadY;

    float pushPower;
    float pushAddValue;
    float pushForceFriction;

    int rideCount; //現在乗車人数
    int rideGroupNum; //グループ乗車人数
    private Human[] passengerObj;

    public GameObject scoreObj;
    public SpawnManager spawnManagerObj;

    public float turnPowerPush;//プッシュ時旋回力
    public float turnPower;//旋回力

    float  pushCharge;//pushチャージ量
    public float speedMax;//最高速
    public float turboRatio;//ターボレシオ

    public GameObject[] vehicleModel;//乗り物モデル
    int vehicleScore;//乗車スコア
    Human.GROUPTYPE passengerType;//乗客タイプ

    public GameObject passengerTogetherUIObj;   //乗客何人乗せるかUI
    public GameObject gameObj;

    public GameObject earth;

    [SerializeField]
    private float CHARGE_MAX;

    //エフェクト関係
    private EffectController effect;
    private ParticleSystem chargeEffectObj;      //チャージエフェクトオブジェ
    private ParticleSystem chargeMaxEffectObj;   //チャージマックスエフェクトオブジェ
    private bool bChargeMax=false;               //チャージがマックス状態かどうか    

    /// <summary>
    /// 重力量。Playerは個別に設定する。
    /// </summary>
    public Vector3 gravity;
    Vector3 curGravity;

    /// <summary>
    /// 前回フレーム時の位置。重力計算などに参照する。
    /// </summary>
    Vector3 oldPos;

    public enum State
    {
        PLAYER_STATE_STOP = 0,
        PLAYER_STATE_MOVE,
        PLAYER_STATE_TAKE,
        PLAYER_STATE_TAKE_READY,
        PLAYER_STATE_IN_CHANGE
    }
    State state;

    public enum VehicleType
    {
        VEHICLE_TYPE_BIKE = 0,
        VEHICLE_TYPE_CAR,
        VEHICLE_TYPE_BUS,
        VEHICLE_TYPE_AIRPLANE,
    }
    VehicleType vehicleType;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pushPower = 0.0f;
        pushAddValue = 0.10f;
        pushForceFriction = 0.05f;
        rideCount = 0;
        pushCharge = 0;
        state = State.PLAYER_STATE_STOP;
        vehicleType = VehicleType.VEHICLE_TYPE_BIKE;
        vehicleModel[ ( int )vehicleType ].SetActive( true );
        vehicleScore = 0;
        oldPos = transform.position;

        //エフェクト関係
        effect = GameObject.Find("EffectManager").GetComponent<EffectController>();
        ChargeEffectCreate();
        ChargeMaxEffectCreate();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.PLAYER_STATE_IN_CHANGE)return;
        switch( vehicleType )
        {
            case VehicleType.VEHICLE_TYPE_BIKE:
                {
                    CityMove();
                    break;
                }
            case VehicleType.VEHICLE_TYPE_CAR:
                {
                    CityMove();
                    break;
                }
            case VehicleType.VEHICLE_TYPE_BUS:
                {
                    CityMove();
                    break;
                }
            case VehicleType.VEHICLE_TYPE_AIRPLANE:
                {
                    StarMove();
                    break;
                }
        }
    }

    private void OnTriggerStay( Collider other )
    {
        Debug.Log("onTriggerStay");
        switch( other.gameObject.tag )
        {
            // 乗車エリアに関する処理
            case "RideArea":
                {
                    Debug.Log("RideAreaON");

                    Human human = other.transform.parent.GetComponent<Human>();

                    if( rb.velocity.magnitude < 1.0f )//ほぼ停止してるなら
                    {
                        Debug.Log("stop");

                        //乗車待機状態じゃないならbreak;
                        if ( human.stateType != Human.STATETYPE.READY ) break;
                        //state = State.PLAYER_STATE_TAKE_READY;
                        //state = State.PLAYER_STATE_TAKE;

                        //最初の乗客の時に他の乗客生成を行う
                        if( rideCount == 0 )
                        {
                            Debug.Log("rideCnt");

                            // HACK: 最初の乗客を乗せた際、他の乗客を街人に変える処理
                            //       10/24現在では他の乗客はFindして消してしまうやり方をする。
                            human.IsProtect = true;

                            GameObject[] humanAll = GameObject.FindGameObjectsWithTag( "Human" );

                            foreach( GameObject deleteHuman in humanAll )
                            {
                                if( deleteHuman.GetComponent<Human>().IsProtect == false )
                                {
                                    Destroy( deleteHuman.gameObject );
                                }
                            }

                            passengerType = human.groupType;

                            // 乗客数の確認
                            switch( human.groupType )
                            {
                                case Human.GROUPTYPE.PEAR:
                                    {
                                        rideGroupNum = 2;
                                        Debug.Log( "PEAR" );
                                        break;
                                    }
                                case Human.GROUPTYPE.SMAlLL:
                                    {
                                        rideGroupNum = 3;
                                        Debug.Log( "SMALL" );
                                        break;
                                    }
                                case Human.GROUPTYPE.BIG:
                                    {
                                        rideGroupNum = 5;
                                        Debug.Log( "BIG" );
                                        break;
                                    }
                                default:
                                    {
                                        Debug.Log( "エラー:設定謎の乗客タイプが設定されています" );
                                        break;
                                    }
                            }

                            spawnManagerObj.SpawnHumanGroup( human.spawnPlace , human.groupType );

                            //グループの大きさ分確保する
                            passengerObj = new Human[ rideGroupNum ];
                            //spawnManagerにペアを生成してもらう
                            //spawnManagerObj.gameObject.GetComponent<SpawnManager>().

                            //何人乗せるかUIを表示させる
                            passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().PassengerTogetherUIStart(rideGroupNum);
                        }

                        //乗客を子にする
                        human.gameObject.transform.position = transform.position;
                        human.transform.parent = transform;
                        human.gameObject.GetComponent<Human>().SetStateType( Human.STATETYPE.TRANSPORT );
                        Debug.Log( "Ride" );
                        passengerObj[ rideCount ] = human;
                        rideCount++;

                        //フェイスUIをONにする
                        passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().FaiceUION(rideCount);

                        // 乗客の当たり判定を消す
                        human.GetHumanModelCollider().isTrigger = true;

                        //最後の人なら降ろす
                        if( rideCount >= rideGroupNum )
                        {
                            for( int i = 0 ; i < rideCount ; i++ )
                            {
                                passengerObj[ i ].transform.parent = null;
                                passengerObj[ i ].GetComponent<Human>().stateType = Human.STATETYPE.GETOFF;
                                passengerObj[ i ].GetHumanModelCollider().isTrigger = false;
                            }
                            scoreObj.gameObject.GetComponent<ScoreCtrl>().AddScore( rideGroupNum );
                            rideCount = 0;

                            //乗客のタイプに応じで乗り物変更用のスコアを加算する
                            switch (passengerType)
                            {
                                case Human.GROUPTYPE.PEAR:
                                    {
                                        vehicleScore += 1;
                                        break;
                                    }
                                case Human.GROUPTYPE.SMAlLL:
                                    {
                                        vehicleScore += 2;
                                        break;
                                    }
                                case Human.GROUPTYPE.BIG:
                                    {
                                        vehicleScore += 4;
                                        break;
                                    }
                                default:
                                    {
                                        Debug.Log("エラー:設定謎の乗客タイプが設定されています");
                                        break;
                                    }
                            }

                            //初期　バイク
                            //＋1ポイント　車
                            //＋4ポイント　バス
                            //＋8ポイント　飛行機

                            // TODO: 後できれいにする
                            if (vehicleScore >= 1 && vehicleScore < 5)
                            {
                                SetVehicle(VehicleType.VEHICLE_TYPE_CAR);
                            }
                            else if (vehicleScore >= 5 && vehicleScore < 13)
                            {
                                SetVehicle(VehicleType.VEHICLE_TYPE_BUS);
                            }
                            else if (vehicleScore >= 13)
                            {
                                SetVehicle(VehicleType.VEHICLE_TYPE_AIRPLANE);
                                gameObj.GetComponent<Game>().SetPhase(Game.Phase.GAME_PAHSE_STAR);
                            }

                            //if (vehicleScore >= 4)
                            //{
                            //    SetVehicle(VehicleType.VEHICLE_TYPE_AIRPLANE);
                            //    gameObj.GetComponent<Game>().SetPhase(Game.Phase.GAME_PAHSE_STAR);
                            //    Debug.Log("Star");
                            //}

                            // HACK: 次の乗客を生成。
                            //       後にゲーム管理側で行うように変更をかける可能性。現状はここで。

                            // TODO: 2017/11/07田口コメントアウトしました
                            /*
                            List< int > posList = new List< int >();
                            posList.Add( human.spawnPlace );

                            // プレイヤーの乗り物の種類に応じて
                            // 出現させるグループを制御

                            for ( int i = 0 ; i < 3 ; i++ )
                            {
                                int pos;
                                // 同じスポーンポイントで生成しないための制御処理
                                while( true )
                                {
                                    pos = Random.Range( 0 , spawnManagerObj.SpawnNum - 1 );

                                    if( posList.IndexOf( pos ) == -1 )
                                    {
                                        posList.Add( pos );
                                        break;
                                    }
                                }


                                // 生成処理実行
                                spawnManagerObj.HumanCreate( pos , ( Human.GROUPTYPE )i );
                            }
                            */

                            //乗物によって生成する人を設定
                            spawnManagerObj.HumanCreateByVehicleType(vehicleType, human.spawnPlace, 2, 2, 2);

                            //何人乗せるかUIの表示を終了
                            passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().PassengerTogetherUIEnd();
                        }
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// 街移動処理
    /// </summary>
    void CityMove()
    {
        float moveV = Input.GetAxis("Vertical");
        float moveH = Input.GetAxis("Horizontal");

        //プッシュ時と通常時で旋回力を分ける
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1"))
        {
            moveH *= turnPowerPush;
        }
        else
        {
            moveH *= turnPower;//旋回力をかける
        }

        if (Mathf.Abs(moveH) > 0.2f)
        {
            moveRadY += moveH * 180.0f * Time.deltaTime;

            transform.rotation = Quaternion.Euler(transform.rotation.x, moveRadY, transform.rotation.z);
        }

        Vector3 force = transform.forward * speed;

        //rb.AddForce(force);

        // プッシュ動作
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1"))
        {           

            force = new Vector3(0.0f, 0.0f, 0.0f);
            rb.velocity *= 0.975f;//減速
            //速度が一定以下なら停止する
            if (rb.velocity.magnitude < 1.0f)
            {
                rb.velocity *= 0.0f;
                state = State.PLAYER_STATE_STOP;
            }

            //チャージエフェクト再生
            if (pushCharge == 0)
            {
                var emission = chargeEffectObj.emission;
                emission.enabled = true;
            }

            //チャージがマックスになったら
            if (pushCharge >= CHARGE_MAX)
            {
                //チャージマックスエフェクト再生
                if (!bChargeMax)
                {
                    //チャージエフェクト停止
                    var emission = chargeEffectObj.emission;
                    emission.enabled = false;

                    //チャージマックスエフェクト再生
                    emission = chargeMaxEffectObj.emission;
                    emission.enabled = true;

                    bChargeMax = true;
                }
            }

            pushCharge+=Time.deltaTime;
        }

        // プッシュ解放した後のダッシュ
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Fire1"))
        {
            //rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);

            if (pushCharge >= CHARGE_MAX)
            {
                rb.AddForce(force * turboRatio * Time.deltaTime, ForceMode.VelocityChange);
            }
            else
            {
            }

            if (bChargeMax)
            {           
                //チャージマックスエフェクト停止
                var emission = chargeMaxEffectObj.emission;
                emission.enabled = false;
                bChargeMax = false;
            }
            else
            {
                //チャージエフェクト停止
                var emissione = chargeEffectObj.emission;
                emissione.enabled = false;

            }

            //force *= ( 30.0f * rb.mass );
            pushCharge = 0;
        }

        // 今回の速度加算
        rb.AddForce(force * Time.deltaTime, ForceMode.Acceleration);

        //最高速を設定
        Vector3 checkV = rb.velocity;
        checkV.y = 0.0f;

        if (checkV.magnitude >= speedMax)
        {
            // Debug.Log("最高速");
            // HACK:最高速制御処理
            //      XZ方向のベクトルを作りspeedMax以上行かないように設定。
            //      後にY方向の力を加算する。
            float YAxisPower = rb.velocity.y;

            checkV = checkV.normalized * speedMax;
            checkV.y = YAxisPower;
            rb.velocity = checkV;
        }

        ////停止処理
        //if( rb.velocity.magnitude < 1.0f)
        //{
        //    rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        //    if ( state == State.PLAYER_STATE_MOVE )
        //    {
        //        state = State.PLAYER_STATE_STOP;
        //    }
        //}
        //else
        //{
        //    state = State.PLAYER_STATE_MOVE;
        //}

        //rb.AddForce( direction * 10.0f , ForceMode.VelocityChange );

        // TODO: 画面外に落ちたときの処理
        //       仮で追加
        if (transform.position.y < -50.0f)
        {
            Vector3 newPos = transform.position;
            newPos.y = 30.0f;
            transform.position = newPos;
        }

        // TODO: ジャンプ処理
        //       デバッグ時に活用できそうなので実装
        if (Input.GetKey(KeyCode.J))
        {
            Vector3 jumpForce = -curGravity * 2.0f;
            rb.AddForce(jumpForce, ForceMode.Acceleration);
        }

        // TODO: 重力計算
        //       Unity内蔵のものだと重力のかかりが弱いので、自前で計算する。
        curGravity += (gravity * Time.deltaTime);

        rb.AddForce(curGravity, ForceMode.Acceleration);

        if (transform.position.y == oldPos.y)
        {
            curGravity = Vector3.zero;
            Debug.Log("Gravity Reset");
        }

        oldPos = transform.position;
    }

    /// <summary>
    /// 星移動処理
    /// </summary>
    void StarMove()
    {
        float moveV = Input.GetAxis("Vertical");
        float moveH = Input.GetAxis("Horizontal");
        Vector3 gravityVec = earth.transform.position - transform.position;
        gravityVec.Normalize();
        rb.AddForce(9.8f * gravityVec, ForceMode.Acceleration);
        transform.up = -gravityVec.normalized;

        Vector3 direction = new Vector3(moveH, 0.0f, moveV);

        //Debug.Log( "Horizontal:" + moveH );
        Vector3 axis = transform.up;// 回転軸
                                    //float angle = 90f * Time.deltaTime; // 回転の角度

        //this.transform.rotation = q * this.transform.rotation; // クォータニオンで回転させる
        moveRadY += moveH * 180.0f * Time.deltaTime;
        //transform.rotation = Quaternion.Euler(0, moveRadY, 0);
        Quaternion q = Quaternion.AngleAxis(moveRadY, axis); // 軸axisの周りにangle回転させるクォータニオン
        this.transform.rotation = q * this.transform.rotation; // クォータニオンで回転させる
        if (Mathf.Abs(moveH) > 0.2f)
        {

        }

        Vector3 force = transform.forward * speed;

        // プッシュ動作
        if (Input.GetKey(KeyCode.Space))
        {
            force = rb.velocity * 0.0f;
            //rb.velocity = rb.velocity * 0.99f;

            if (rb.velocity.magnitude < 2)
            {
                rb.velocity *= 0;
            }

        }

        // プッシュ解放した後のダッシュ
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            //force *= ( 30.0f * rb.mass );
            rb.AddForce(force * turboRatio * Time.deltaTime, ForceMode.VelocityChange);
        }

        // 今回の速度加算
        rb.AddForce(force * Time.deltaTime, ForceMode.Acceleration);


        if (rb.velocity.magnitude > speedMax)
        {
            rb.velocity = rb.velocity.normalized * speedMax;
        }
    }

    /// <summary>
    /// 乗り物設定関数
    /// </summary>
    void SetVehicle( VehicleType setVehicleType )
    {
        vehicleModel[(int)vehicleType].SetActive(false);
        vehicleType = setVehicleType;
        vehicleModel[(int)vehicleType].SetActive(true);
    }

    private void OnGUI()
    {
        GUIStyleState styleState;
        styleState = new GUIStyleState();
        styleState.textColor = Color.white;

        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 48;
        guiStyle.normal = styleState;

        string str;
        str = "速度ベクトル:" + rb.velocity + "\n速度量:" + rb.velocity.magnitude;

        GUI.Label( new Rect( 0 , 200 , 800 , 600 ) , str , guiStyle );
    }

    /// <summary>
    /// チャージエフェクト生成
    /// </summary>
    public void ChargeEffectCreate()
    {
        //生成
        chargeEffectObj = effect.EffectCreate(EffectController.Effects.CHARGE_EFFECT, gameObject.transform);

        //再生OFF
        var emissione = chargeEffectObj.emission;
        emissione.enabled = false;

        //位置設定
        Vector3 pos;
        pos = chargeEffectObj.transform.localPosition;
        pos.y = -1.0f;
        pos.z = -1.0f;
        chargeEffectObj.transform.localPosition = pos;
    }

    /// <summary>
    /// チャージマックスエフェクト生成
    /// </summary>
    public void ChargeMaxEffectCreate()
    {
        //生成
        chargeMaxEffectObj = effect.EffectCreate(EffectController.Effects.CHARGE_MAX_EFFECT, gameObject.transform);

        //再生OFF
        var emissione = chargeMaxEffectObj.emission;
        emissione.enabled = false;

        //位置設定
        Vector3 pos;
        pos = chargeMaxEffectObj.transform.localPosition;
        pos.y = 0.0f;
        pos.z = -1.0f;
        chargeMaxEffectObj.transform.localPosition = pos;
    }
}


