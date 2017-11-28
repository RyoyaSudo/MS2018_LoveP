using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    int rideCount; //現在乗車人数
    int rideGroupNum; //グループ乗車人数
    private Human[] passengerObj;

    private GameObject scoreObj;

    /// <summary>
    /// スポーンマネージャーオブジェクト。
    /// 乗客生成にまつわる処理をさせるため必要。
    /// </summary>
    private CitySpawnManager citySpawnManagerObj;
    public string citySpawnManagerPath;

    private StarSpawnManager starSpawnManagerObj;
    public string starSpawnManagerPath;

    public GameObject[] vehicleModel;//乗り物モデル
    int vehicleScore;//乗車スコア
    Human.GROUPTYPE passengerType;//乗客タイプ

    /// <summary>
    /// 乗客乗車人数を示すUIオブジェクト。
    /// プレイヤー側から操作するために取得。
    /// </summary>
    private GameObject passengerTogetherUIObj;
    public string passengerTogetherUIObjPath;

    /// <summary>
    /// ゲームシーン管理オブジェクト。
    /// シーン管理クラスで利用したい処理があるため取得。
    /// </summary>
    private GameObject gameObj;
    public string gamectrlObjPath;

    //エフェクト関係
    private EffectController effect;

    public ParticleSystem ChargeEffectObj { get; private set; }
    public ParticleSystem ChargeMaxEffectObj { get; private set; }

    private ParticleSystem scoreUpEffectObj;     //スコアアップエフェクト
    private ParticleSystem changeEffectObj;

    /// <summary>
    /// 移動量ベクトル
    /// </summary>
    public Vector3 VelocityVec { get; private set; }

    Vector3 velocityVecOld;

    /// <summary>
    /// 移動量(スカラー)
    /// </summary>
    public float Velocity { get; private set; }

    //サウンド用/////////////////////////////
    private AudioSource playerAudioS;
    private SoundController playerSoundCtrl;
    private SoundController.Sounds playerType;  //プレイヤーの車両用

    /// <summary>
    /// 状態パラメータ
    /// </summary>
    public enum State
    {
        PLAYER_STATE_STOP = 0,
        PLAYER_STATE_MOVE,
        PLAYER_STATE_TAKE,
        PLAYER_STATE_TAKE_READY,
        PLAYER_STATE_IN_CHANGE
    }

    /// <summary>
    /// 状態管理変数
    /// </summary>
    public State StateParam { get; set; }

    public enum VehicleType
    {
        VEHICLE_TYPE_BIKE = 0,
        VEHICLE_TYPE_CAR,
        VEHICLE_TYPE_BUS,
        VEHICLE_TYPE_AIRPLANE,
    }
    VehicleType vehicleType;

    /// <summary>
    /// 街フェイズ時の挙動管理オブジェクト
    /// </summary>
    CityPhaseMove cityPhaseMoveObj;

    /// <summary>
    /// 街フェイズ時の挙動管理オブジェクトのパス
    /// </summary>
    [SerializeField] string cityPhaseMoveObjPath;

    /// <summary>
    /// 星フェイズ時の挙動管理オブジェクト
    /// </summary>
    StarPhaseMove starPhaseMoveObj;

    /// <summary>
    /// 星フェイズ時の挙動管理オブジェクトのパス
    /// </summary>
    [SerializeField] string starPhaseMoveObjPath;

    bool changeFade;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        // 初期化系
        vehicleType = VehicleType.VEHICLE_TYPE_BIKE;
        vehicleModel[ ( int )vehicleType ].SetActive( true );
        vehicleScore = 0;
        Velocity = 0.0f;
        VelocityVec = Vector3.zero;
        velocityVecOld = Vector3.zero;

        cityPhaseMoveObj = null;
        starPhaseMoveObj = null;

        changeFade = true;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        rideCount = 0;
        StateParam = State.PLAYER_STATE_STOP;

        //エフェクト関係
        effect = GameObject.Find( "EffectManager" ).GetComponent<EffectController>();
        ChargeEffectCreate();
        ChargeMaxEffectCreate();
        ChangeEffectCreate();

        // シーン内から必要なオブジェクトを取得
        scoreObj = GameObject.Find( "Score" );

        gameObj = GameObject.Find( gamectrlObjPath );
        passengerTogetherUIObj = GameObject.Find( passengerTogetherUIObjPath );

        //サウンド用//////////////////////////////////////
        playerSoundCtrl = GameObject.Find( "SoundManager" ).GetComponent<SoundController>();
        //オブジェクトについているAudioSourceを取得する
        playerAudioS = gameObject.GetComponent<AudioSource>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        //if( StateParam == State.PLAYER_STATE_IN_CHANGE ) return;
        switch (StateParam)
        {
            case State.PLAYER_STATE_STOP:
                {
                    VehicleMove();
                    break;
                }
            case State.PLAYER_STATE_MOVE:
                {
                    VehicleMove();
                    break;
                }
            case State.PLAYER_STATE_IN_CHANGE:
                {
                    VehicleChange();
                    break;
                }
        }
    }

    /// <summary>
    /// 街フェイズ開始時のプレイヤー初期化処理
    /// </summary>
    public void CityPhaseInit()
    {
        citySpawnManagerObj = GameObject.Find( citySpawnManagerPath ).GetComponent<CitySpawnManager>();

        cityPhaseMoveObj = GameObject.Find( cityPhaseMoveObjPath ).GetComponent<CityPhaseMove>();
        cityPhaseMoveObj.IsEnable = true;

        if( starPhaseMoveObj != null )
        {
            starPhaseMoveObj.IsEnable = false;
        }

        StateParam = State.PLAYER_STATE_STOP;
        transform.rotation = new Quaternion( 0.0f , 0.0f , 0.0f , 0.0f );

        ScriptDebug.Log( "街フェイズ開始" );
    }

    /// <summary>
    /// 星フェイズ開始時のプレイヤー初期化処理
    /// </summary>
    public void StarPhaseInit()
    {
        vehicleScore = 13;
        SetVehicle( VehicleType.VEHICLE_TYPE_AIRPLANE );
        starSpawnManagerObj = GameObject.Find( starSpawnManagerPath ).GetComponent<StarSpawnManager>();

        if( cityPhaseMoveObj != null )
        {
            cityPhaseMoveObj.IsEnable = false;
        }

        starPhaseMoveObj = GameObject.Find( starPhaseMoveObjPath ).GetComponent<StarPhaseMove>();
        starPhaseMoveObj.IsEnable = true;

        transform.position = new Vector3( 250.0f , 290.0f , -300.0f );

        ScriptDebug.Log( "星フェイズ開始" );
    }

    /// <summary>
    /// 乗り物設定関数
    /// </summary>
    public void SetVehicle( VehicleType setVehicleType )
    {
        //SE再生/////////////////////////////////////////////////////////////
        // TODO オブジェクトが見つからなかったため一時コメントアウトしました
        //playerAudioS.PlayOneShot(playerSoundCtrl.AudioClipCreate(SoundController.Sounds.TYPE_CHANGE));

        vehicleModel[ ( int )vehicleType ].SetActive( false );
        vehicleType = setVehicleType;
        vehicleModel[ ( int )vehicleType ].SetActive( true );


        switch ( setVehicleType )
        {
            case VehicleType.VEHICLE_TYPE_BIKE:
                break;

            case VehicleType.VEHICLE_TYPE_CAR:
                break;

            case VehicleType.VEHICLE_TYPE_BUS:
                break;

            case VehicleType.VEHICLE_TYPE_AIRPLANE:
                break;

            default:
                ScriptDebug.Log( "未確定の乗り物タイプが指定されました。" );
                break;
        }
    }

    /// <summary>
    /// 状態設定関数
    /// </summary>
    public void SetState( State setState )
    {
        StateParam = setState;
    }

    /// <summary>
    /// チャージエフェクト生成
    /// </summary>
    public void ChargeEffectCreate()
    {
        //生成
        ChargeEffectObj = effect.EffectCreate( EffectController.Effects.CHARGE_EFFECT , gameObject.transform );

        //再生OFF
        var emissione = ChargeEffectObj.emission;
        emissione.enabled = false;

        //位置設定
        Vector3 pos;
        pos = ChargeEffectObj.transform.localPosition;
        pos.y = -1.0f;
        pos.z = -1.0f;
        ChargeEffectObj.transform.localPosition = pos;
    }

    /// <summary>
    /// チャージマックスエフェクト生成
    /// </summary>
    public void ChargeMaxEffectCreate()
    {
        //生成
        ChargeMaxEffectObj = effect.EffectCreate( EffectController.Effects.CHARGE_MAX_EFFECT , gameObject.transform );

        //再生OFF
        var emissione = ChargeMaxEffectObj.emission;
        emissione.enabled = false;

        //位置設定
        Vector3 pos;
        pos = ChargeMaxEffectObj.transform.localPosition;
        pos.y = 0.0f;
        pos.z = -1.0f;
        ChargeMaxEffectObj.transform.localPosition = pos;
    }

    /// <summary>
    /// 変身エフェクト生成
    /// </summary>
    public void ChangeEffectCreate()
    {
        //生成
        changeEffectObj = effect.EffectCreate(EffectController.Effects.CHANGE_EFFECT, gameObject.transform);

        //再生OFF
        var emissione = changeEffectObj.emission;
        emissione.enabled = true;

        //位置設定
        Vector3 pos;
        pos = changeEffectObj.transform.localPosition;
        pos.y = 0.0f;
        pos.z = -1.0f;
        changeEffectObj.transform.localPosition = pos;
    }

    /// <summary>
    /// 乗り物に応じでスポーンマネージャを分ける(初乗り）
    /// </summary>
    public void HumanCreate( Human human )
    {
        switch( vehicleType )
        {
            case VehicleType.VEHICLE_TYPE_BIKE:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.SpawnHumanGroup( human.spawnPlace , human.groupType );
                    break;
                }
            case VehicleType.VEHICLE_TYPE_CAR:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.SpawnHumanGroup( human.spawnPlace , human.groupType );
                    break;
                }
            case VehicleType.VEHICLE_TYPE_BUS:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.SpawnHumanGroup( human.spawnPlace , human.groupType );
                    break;
                }
            case VehicleType.VEHICLE_TYPE_AIRPLANE:
                {
                    //乗物によって生成する人を設定
                    starSpawnManagerObj.SpawnHumanGroup( human.spawnPlace , human.groupType );
                    //citySpawnManagerObj.SpawnHumanGroup(human.spawnPlace, human.groupType);
                    break;
                }
        }
    }

    /// <summary>
    /// 乗り物に応じでスポーンマネージャを分ける(相方用）
    /// </summary>
    public void HumanCreateGroup( Human human )
    {
        switch( vehicleType )
        {
            case VehicleType.VEHICLE_TYPE_BIKE:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.HumanCreateByVehicleType( vehicleType , human.spawnPlace , 2 , 2 , 2 );
                    break;
                }
            case VehicleType.VEHICLE_TYPE_CAR:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.HumanCreateByVehicleType( vehicleType , human.spawnPlace , 2 , 2 , 2 );
                    break;
                }
            case VehicleType.VEHICLE_TYPE_BUS:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.HumanCreateByVehicleType( vehicleType , human.spawnPlace , 2 , 2 , 2 );
                    break;
                }
            case VehicleType.VEHICLE_TYPE_AIRPLANE:
                {
                    //乗物によって生成する人を設定
                    starSpawnManagerObj.HumanCreateByVehicleType( vehicleType , human.spawnPlace , 2 , 2 , 2 );
                    //citySpawnManagerObj.HumanCreateByVehicleType(vehicleType, human.spawnPlace, 2, 2, 2);
                    break;
                }
        }
    }

    /// <summary>
    /// 当たり判定後に行う処理
    /// </summary>
    private void OnTriggerStay( Collider other )
    {
        switch( other.gameObject.tag )
        {
            // 乗車エリアに関する処理
            case "RideArea":
                {
                    Debug.Log( "RideAreaON" );

                    Human human = other.transform.parent.GetComponent<Human>();

                    if( Velocity < 1.0f )//ほぼ停止してるなら
                    {
                        Debug.Log( "stop" );

                        //乗車待機状態じゃないならbreak;
                        if( human.stateType != Human.STATETYPE.READY ) break;
                        //StateParam = State.PLAYER_STATE_TAKE_READY;
                        //StateParam = State.PLAYER_STATE_TAKE;

                        //最初の乗客の時に他の乗客生成を行う
                        if( rideCount == 0 )
                        {
                            Debug.Log( "rideCnt" );

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

                            HumanCreate( human );
                            //citySpawnManagerObj.SpawnHumanGroup( human.spawnPlace , human.groupType );

                            //グループの大きさ分確保する
                            passengerObj = new Human[ rideGroupNum ];
                            //spawnManagerにペアを生成してもらう
                            //citySpawnManagerObj.gameObject.GetComponent<SpawnManager>().

                            //何人乗せるかUIを表示させる
                            passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().PassengerTogetherUIStart( rideGroupNum );
                        }

                        //乗客を子にする
                        human.gameObject.transform.position = transform.position;
                        human.transform.parent = transform;
                        human.gameObject.GetComponent<Human>().SetStateType( Human.STATETYPE.TRANSPORT );
                        Debug.Log( "Ride" );
                        passengerObj[ rideCount ] = human;
                        rideCount++;

                        //フェイスUIをONにする
                        passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().FaiceUION( rideCount );

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
                            // HACK: スコア加算処理の場所
                            //       現状プレイヤークラス内だが、後に変更の可能性有り。
                            scoreObj.gameObject.GetComponent<ScoreCtrl>().AddScore( ( int )passengerType );
                            rideCount = 0;

                            //乗客のタイプに応じで乗り物変更用のスコアを加算する
                            switch( passengerType )
                            {
                                case Human.GROUPTYPE.PEAR:
                                    {
                                        //ペア作成時のSE再生///////////////////////////////////////////////
                                        playerAudioS.PlayOneShot( playerSoundCtrl.AudioClipCreate( SoundController.Sounds.CREATING_PEAR ) );
                                        vehicleScore += 1;
                                        break;
                                    }
                                case Human.GROUPTYPE.SMAlLL:
                                    {
                                        vehicleScore += 2;
                                        //ペア作成時のSE再生///////////////////////////////////////////////
                                        playerAudioS.PlayOneShot( playerSoundCtrl.AudioClipCreate( SoundController.Sounds.CREATING_PEAR ) );
                                        break;
                                    }
                                case Human.GROUPTYPE.BIG:
                                    {
                                        //ペア作成時のSE再生///////////////////////////////////////////////
                                        playerAudioS.PlayOneShot( playerSoundCtrl.AudioClipCreate( SoundController.Sounds.CREATING_PEAR ) );
                                        vehicleScore += 4;
                                        break;
                                    }
                                default:
                                    {
                                        Debug.Log( "エラー:設定謎の乗客タイプが設定されています" );
                                        break;
                                    }
                            }

                            // HACK: 乗り物変更処理
                            //       後に関数化する。
                            // 〇変身条件
                            //    初期        : バイク
                            //    ＋1ポイント : 車
                            //    ＋4ポイント : 大型車( バス )
                            //    ＋8ポイント : 飛行機
                            if( vehicleScore >= 1 && vehicleScore < 5 && vehicleType != VehicleType.VEHICLE_TYPE_CAR )
                            {
                                VehicleChangeStart();
                            }
                            else if( vehicleScore >= 5 && vehicleScore < 13 && vehicleType != VehicleType.VEHICLE_TYPE_BUS)
                            {
                                VehicleChangeStart();
                            }
                            else if( vehicleScore >= 13 && vehicleType != VehicleType.VEHICLE_TYPE_AIRPLANE)
                            {
                                SetVehicle( VehicleType.VEHICLE_TYPE_AIRPLANE );
                                gameObj.GetComponent<Game>().SetPhase( Game.Phase.GAME_PAHSE_STAR );
                                starSpawnManagerObj = GameObject.Find( starSpawnManagerPath ).GetComponent<StarSpawnManager>();
                                var emission = ChargeMaxEffectObj.emission;
                                emission.enabled = false;
                                emission = ChargeEffectObj.emission;
                                emission.enabled = false;

                                cityPhaseMoveObj.IsEnable = false;
                                starPhaseMoveObj.IsEnable = true;
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
                                    pos = Random.Range( 0 , citySpawnManagerObj.SpawnNum - 1 );

                                    if( posList.IndexOf( pos ) == -1 )
                                    {
                                        posList.Add( pos );
                                        break;
                                    }
                                }


                                // 生成処理実行
                                citySpawnManagerObj.HumanCreate( pos , ( Human.GROUPTYPE )i );
                            }
                            */

                            //乗物によって生成する人を設定
                            HumanCreateGroup( human );

                            //何人乗せるかUIの表示を終了
                            passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().PassengerTogetherUIEnd();
                        }
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// 乗り物に応じた動作をする
    /// </summary>
    void VehicleMove()
    {
        switch (vehicleType)
        {
            case VehicleType.VEHICLE_TYPE_BIKE:
                {
                    //CityMove();
                    //CityMoveCharcterController();
                    Velocity = cityPhaseMoveObj.Velocity;
                    playerType = SoundController.Sounds.BIKE_RUN;   //プレイヤーの車両によってSEも変更する
                    break;
                }
            case VehicleType.VEHICLE_TYPE_CAR:
                {
                    //CityMove();
                    //CityMoveCharcterController();
                    Velocity = cityPhaseMoveObj.Velocity;
                    playerType = SoundController.Sounds.CAR_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
            case VehicleType.VEHICLE_TYPE_BUS:
                {
                    //CityMove();
                    //CityMoveCharcterController();
                    Velocity = cityPhaseMoveObj.Velocity;
                    playerType = SoundController.Sounds.BUS_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
            case VehicleType.VEHICLE_TYPE_AIRPLANE:
                {
                    //StarMove();
                    playerType = SoundController.Sounds.AIRPLANE_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
        }
    }

    void VehicleChangeStart()
    {
        //チェンジエフェクト
        changeEffectObj.Play();
        cityPhaseMoveObj.IsEnable = false;
        StateParam = State.PLAYER_STATE_IN_CHANGE;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void VehicleChange()
    {
        if (changeFade)
        {
            Vector3 scale;

            scale = new Vector3(vehicleModel[(int)vehicleType].transform.localScale.x - Time.deltaTime, vehicleModel[(int)vehicleType].transform.localScale.y - Time.deltaTime, vehicleModel[(int)vehicleType].transform.localScale.z - Time.deltaTime);
            vehicleModel[(int)vehicleType].transform.localScale = scale;
            if (vehicleModel[(int)vehicleType].transform.localScale.x < 0.0f)
            {
                scale = new Vector3(0.0f, 0.0f, 0.0f);
                vehicleModel[(int)vehicleType].transform.localScale = scale;
                changeFade = false;
                SetVehicle(vehicleType + 1);
                scale = new Vector3(0.0f, 0.0f, 0.0f);
                vehicleModel[(int)vehicleType].transform.localScale = scale;
            }
        }
        else
        {
            Vector3 scale;
            scale = new Vector3(vehicleModel[(int)vehicleType].transform.localScale.x + Time.deltaTime, vehicleModel[(int)vehicleType].transform.localScale.y + Time.deltaTime, vehicleModel[(int)vehicleType].transform.localScale.z + Time.deltaTime);
            vehicleModel[(int)vehicleType].transform.localScale = scale;
            if (vehicleModel[(int)vehicleType].transform.localScale.x > 1.0f)
            {
                scale = new Vector3(1.0f, 1.0f, 1.0f);
                vehicleModel[(int)vehicleType].transform.localScale = scale;
                StateParam = State.PLAYER_STATE_STOP;
                cityPhaseMoveObj.IsEnable = true;
                changeFade = true;
            }
        }
    }

    /// <summary>
    /// OnGUI処理
    /// 主にデバッグ情報を出す
    /// </summary>
    private void OnGUI()
    {
        if( Game.IsOnGUIEnable )
        {
            GUIStyleState styleState;
            styleState = new GUIStyleState();
            styleState.textColor = Color.white;

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 48;
            guiStyle.normal = styleState;

            string str = "";
            //str = "速度ベクトル:" + VelocityVec + "\n速度量:" + VelocityVec.magnitude + "\nフレーム間速度:" + velocity;

            GUI.Label( new Rect( 0 , 200 , 800 , 600 ) , str , guiStyle );
        }
    }
}


