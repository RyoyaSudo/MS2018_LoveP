using System.Collections;
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

    int  pushCharge;//pushチャージ量
    public float speedMax;//最高速
    public float turboRatio;//ターボレシオ

    public GameObject[] vehicleModel;

    public enum State
    {
        PLAYER_STATE_STOP = 0,
        PLAYER_STATE_MOVE,
        PLAYER_STATE_TAKE,
        PLAYER_STATE_TAKE_READY
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
    }

    // Update is called once per frame
    void Update()
    {
        CityMove();
    }

    private void OnTriggerStay( Collider other )
    {
        switch( other.gameObject.tag )
        {
            // 乗車エリアに関する処理
            case "RideArea":
                {
                    Human human = other.transform.parent.GetComponent<Human>();

                    if( rb.velocity.magnitude < 1.0f )//ほぼ停止してるなら
                    {
                        //乗車待機状態じゃないならbreak;
                        if( human.stateType != Human.STATETYPE.READY ) break;
                        //state = State.PLAYER_STATE_TAKE_READY;
                        //state = State.PLAYER_STATE_TAKE;

                        //最初の乗客の時に他の乗客生成を行う
                        if( rideCount == 0 )
                        {
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

                        }

                        //乗客を子にする
                        human.gameObject.transform.position = transform.position;
                        human.transform.parent = transform;
                        human.gameObject.GetComponent<Human>().SetStateType( Human.STATETYPE.TRANSPORT );
                        Debug.Log( "Ride" );
                        passengerObj[ rideCount ] = human;
                        rideCount++;

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

                            // HACK: 次の乗客を生成。
                            //       後にゲーム管理側で行うように変更をかける可能性。現状はここで。
                            List< int > posList = new List< int >();
                            posList.Add( human.spawnPlace );

                            for( int i = 0 ; i < 3 ; i++ )
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
        if( Input.GetKey( KeyCode.Space ) || Input.GetButton( "Fire1" ) )
        {
            moveH *= turnPowerPush;
        }
        else
        {
            moveH *= turnPower;//旋回力をかける
        }


        Vector3 direction = new Vector3(moveH, 0.0f, moveV);

        //Debug.Log( "Horizontal:" + moveH );

        if( Mathf.Abs( moveH ) > 0.2f )
        {
            moveRadY += moveH * 180.0f * Time.deltaTime;
            transform.rotation = Quaternion.Euler( 0 , moveRadY , 0 );
        }

        Vector3 force = transform.forward * speed;

        //rb.AddForce(force);

        // プッシュ動作
        if( Input.GetKey( KeyCode.Space ) || Input.GetButton( "Fire1" ) )
        {
            force = new Vector3( 0.0f , 0.0f , 0.0f );
            rb.velocity *= 0.975f;//減速
            //速度が一定以下なら停止する
            if( rb.velocity.magnitude < 1.0f )
            {
                rb.velocity *= 0.0f;
                state = State.PLAYER_STATE_STOP;
            }
            pushCharge++;
        }

        // プッシュ解放した後のダッシュ
        if( Input.GetKeyUp( KeyCode.Space ) || Input.GetButtonUp( "Fire1" ) )
        {
            //rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);

            if( pushCharge >= 60 )
            {
                rb.AddForce( force * turboRatio , ForceMode.VelocityChange );
            }
            //force *= ( 30.0f * rb.mass );
            pushCharge = 0;
        }

        // 今回の速度加算
        rb.AddForce( force , ForceMode.Acceleration );

        //最高速を設定
        if( rb.velocity.magnitude >= speedMax )
        {
            // Debug.Log("最高速");
            rb.velocity = rb.velocity.normalized * speedMax;
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
}


