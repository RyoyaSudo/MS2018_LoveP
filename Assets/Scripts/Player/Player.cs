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
    private Game gameObj;
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

    /// <summary>
    /// 停車しているか判別するフラグ。
    /// </summary>
    public bool IsStopped { get; set; }

    //サウンド用/////////////////////////////
    private AudioSource playerAudioS;
    private SoundController playerSoundCtrl;
    private SoundController.Sounds playerType;  //プレイヤーの車両用

    /// <summary>
    /// 状態パラメータ列挙型
    /// </summary>
    public enum State
    {
        PLAYER_STATE_FREE,          // 自由移動
        PLAYER_STATE_TAKE_READY,    // 乗車待機
        PLAYER_STATE_TAKE,          // 運搬中
        PLAYER_STATE_GET_OFF,       // 乗客下車動作中
        PLAYER_STATE_IN_CHANGE      // 車両変化中
    }

    /// <summary>
    /// 状態管理変数
    /// </summary>
    public State StateParam { get { return stateParam; } set { SetState( value ); } }

    /// <summary>
    /// 状態管理変数のバッキングストア。値を変更するときはプロパティ側を基本利用。
    /// </summary>
    private State stateParam;

    /// <summary>
    /// 前回フレームの状態
    /// </summary>
    public State StateParamOld { get; private set; }

    /// <summary>
    /// 状態管理関連の汎用タイマー
    /// </summary>
    public float StateTimer { get; private set; }

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

        IsStopped = false;

        StateTimer = 0.0f;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        rideCount = 0;
        StateParam = StateParamOld = State.PLAYER_STATE_FREE;

        //エフェクト関係
        effect = GameObject.Find( "EffectManager" ).GetComponent<EffectController>();
        ChargeEffectCreate();
        ChargeMaxEffectCreate();
        ChangeEffectCreate();

        // シーン内から必要なオブジェクトを取得
        scoreObj = GameObject.Find( "Score" );

        gameObj = GameObject.Find( gamectrlObjPath ).GetComponent<Game>();
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
        // 状態に応じた行動
        switch( StateParam )
        {
            case State.PLAYER_STATE_FREE:
                VehicleMove();
                break;

            case State.PLAYER_STATE_TAKE_READY:
                TakeRady();
                break;

            case State.PLAYER_STATE_TAKE:
                VehicleMove();
                break;

            case State.PLAYER_STATE_GET_OFF:
                GetOff();
                break;

            case State.PLAYER_STATE_IN_CHANGE:
                VehicleChange();
                break;

            default:
                Debug.LogError( "プレイヤー状態で不定のタイプが設定されました。適切な挙動を割り当てて下さい。" );
                break;
        }
    }

    /// <summary>
    /// 遅延更新処理
    /// </summary>
    private void LateUpdate()
    {
        // 今回の状態の保存
        StateParamOld = StateParam;
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

        StateParam = State.PLAYER_STATE_FREE;
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

        // TODO: 星フェイズ開始時のプレイヤー初期位置設定
        //       Earthオブジェクトにプレイヤー初期位置のスポーンポイントを仕込んでおいて、そこから設定する形に変更したほうがよさそう
        transform.position = new Vector3( 250.0f , 290.0f , -350.0f );
        transform.position = new Vector3( 0.0f , 520.0f , 0.0f );

        starPhaseMoveObj = GameObject.Find( starPhaseMoveObjPath ).GetComponent<StarPhaseMove>();
        starPhaseMoveObj.IsEnable = true;
        //starPhaseMoveObj.StarPhaseStart();

        starPhaseMoveObj.Initialize();
        StateParam = State.PLAYER_STATE_FREE;

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
                    Human human = other.transform.parent.GetComponent<Human>();

                    // 可否を確認し、乗車処理を実行
                    if( RideEnableCheck() )
                    {
                        //乗車待機状態じゃないならbreak;
                        if( human.stateType != Human.STATETYPE.READY ) break;

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
                                        Debug.LogError( "エラー:設定謎の乗客タイプが設定されています" );
                                        break;
                                    }
                            }

                            HumanCreate( human );

                            //グループの大きさ分確保する
                            passengerObj = new Human[ rideGroupNum ];

                            //何人乗せるかUIを表示させる
                            passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().PassengerTogetherUIStart( rideGroupNum );
                        }

                        //　TODO : 田口　2017/11/30
                        //乗客を子にするのはHuman.csでやります
                        //乗客を子にする
                        //human.gameObject.transform.position = transform.position;
                        //human.transform.parent = transform;

                        // TODO : 田口　2017/11/30
                        //Human.csで運搬状態にしました
                        //乗客の状態を「運搬」に
                        //human.gameObject.GetComponent<Human>().SetStateType( Human.STATETYPE.TRANSPORT );
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
                            for ( int i = 0 ; i < rideCount ; i++ )
                            {
                                passengerObj[ i ].transform.parent = null;
                                passengerObj[ i ].GetHumanModelCollider().isTrigger = false;

                                // TODO : 田口　2017/11/30
                                //最後の人だけ状態を「待ち受け」に
                                if (i == rideCount-1)
                                {
                                    passengerObj[i].GetComponent<Human>().SetStateType(Human.STATETYPE.AWAIT);
                                }
                                else //それ以外の状態は「下車」に
                                {
                                    passengerObj[i].GetComponent<Human>().SetStateType(Human.STATETYPE.GETOFF);
                                }
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

                            // HACK: 下車状態へ移行
                            //       下車状態時実行処理内で乗り物変化判定を行わなければならない。
                            StateParam = State.PLAYER_STATE_GET_OFF;

                            // HACK: 次の乗客を生成。
                            //       後にゲーム管理側で行うように変更をかける可能性。現状はここで。
                            //乗物によって生成する人を設定
                            HumanCreateGroup( human );

                            //何人乗せるかUIの表示を終了
                            passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().PassengerTogetherUIEnd();
                        }
                        else
                        {
                            // 乗客はまだ1人以上残っているため、乗車待機状態に
                            StateParam = State.PLAYER_STATE_TAKE_READY;

                            // TODO : 田口　2017/11/30
                            //乗客の状態を「乗車」に
                            human.gameObject.GetComponent<Human>().SetStateType(Human.STATETYPE.RIDE);
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
                    Velocity = cityPhaseMoveObj.Velocity;
                    playerType = SoundController.Sounds.BIKE_RUN;   //プレイヤーの車両によってSEも変更する
                    break;
                }
            case VehicleType.VEHICLE_TYPE_CAR:
                {
                    Velocity = cityPhaseMoveObj.Velocity;
                    playerType = SoundController.Sounds.CAR_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
            case VehicleType.VEHICLE_TYPE_BUS:
                {
                    Velocity = cityPhaseMoveObj.Velocity;
                    playerType = SoundController.Sounds.BUS_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
            case VehicleType.VEHICLE_TYPE_AIRPLANE:
                {
                    playerType = SoundController.Sounds.AIRPLANE_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
        }
    }

    void VehicleChangeStart()
    {
        // チェンジエフェクト
        changeEffectObj.Play();
        MoveEnable( false );
        StateParam = State.PLAYER_STATE_IN_CHANGE;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    /// <summary>
    /// 車両変化処理
    /// </summary>
    void VehicleChange()
    {
        // HACK: 車両変化時の演出に関する部分
        //       煙エフェクトを出しつつスケール値を段々小さくしたあとに段々大きくなって現れる感じにしたい。
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
                StateParam = State.PLAYER_STATE_FREE;
                MoveEnable( true );
                changeFade = true;
            }
        }
    }

    //スコアが貯まる
    //プレイヤー操作の停止
    //トランジションで演出用の場面に移る
    //飛行場から飛行機が飛び立つ演出
    //トランジションイン
    //星フェーズ切り替え
    //トランジションアウト
    //
    void ChangeStarPhase()
    {
        ////state = State.PLAYER_STATE_IN_CHANGE;
        //GetComponent<Rigidbody>().velocity = Vector3.zero;

        SetVehicle(VehicleType.VEHICLE_TYPE_AIRPLANE);
        gameObj.PhaseParam = Game.Phase.GAME_PAHSE_STAR;
        starSpawnManagerObj = GameObject.Find(starSpawnManagerPath).GetComponent<StarSpawnManager>();
        var emission = ChargeMaxEffectObj.emission;
        emission.enabled = false;
        emission = ChargeEffectObj.emission;
        emission.enabled = false;

        IsStopped = false;

        cityPhaseMoveObj.IsEnable = false;
        starPhaseMoveObj.IsEnable = true;
    }

    /// <summary>
    /// 状態設定処理。自己遷移できない状態の時のみ利用。
    /// </summary>
    /// <param name="state">設定する状態。</param>
    private void SetState( State state )
    {
        stateParam = state;

        // 各状態ごとに個別にしたい処理
        switch( state )
        {
            case State.PLAYER_STATE_FREE:
                break;

            case State.PLAYER_STATE_TAKE_READY:
                // HACK: 乗車待機時間を与える
                //       タイムライン側からデータ抽出をするべきか否かで悩む。のちに判断。
                //       2017/11/30現在はマジックナンバーで。
                StateTimer = 5.0f;
                MoveEnable( false );
                break;

            case State.PLAYER_STATE_TAKE:
                break;

            case State.PLAYER_STATE_GET_OFF:
                // HACK: 下車時間を与える
                //       タイムライン側からデータ抽出をするべきか否かで悩む。のちに判断。
                //       2017/11/30現在はマジックナンバーで。
                StateTimer = 5.0f;
                MoveEnable( false );
                break;

            case State.PLAYER_STATE_IN_CHANGE:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 乗車待機時の処理
    /// </summary>
    private void TakeRady()
    {
        StateTimer -= Time.deltaTime;

        if( StateTimer < 0.0f )
        {
            StateTimer = 0.0f;
            StateParam = State.PLAYER_STATE_TAKE;

            MoveEnable( true );
        }
    }

    /// <summary>
    /// 下車動作中に行う動作処理
    /// </summary>
    private void GetOff()
    {
        StateTimer -= Time.deltaTime;

        if( StateTimer < 0.0f )
        {
            StateTimer = 0.0f;
            StateParam = State.PLAYER_STATE_FREE;

            MoveEnable( true );

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
            else if( vehicleScore >= 5 && vehicleScore < 13 && vehicleType != VehicleType.VEHICLE_TYPE_BUS )
            {
                VehicleChangeStart();
            }
            else if( vehicleScore >= 13 && vehicleType != VehicleType.VEHICLE_TYPE_AIRPLANE )
            {
                //星フェーズへの移行開始
                ChangeStarPhase();
            }
        }
    }

    /// <summary>
    /// 乗客乗車チェック処理
    /// </summary>
    /// <returns>判定結果</returns>
    private bool RideEnableCheck()
    {
        // 停車判定
        if( IsStopped == false ) return false;

        // 自己状態に応じて乗せるか判断
        bool flags = false;

        switch( StateParam )
        {
            case State.PLAYER_STATE_FREE:       flags = true;  break;
            case State.PLAYER_STATE_TAKE_READY: flags = false; break;
            case State.PLAYER_STATE_TAKE:       flags = true;  break;
            case State.PLAYER_STATE_GET_OFF:    flags = false; break;
            case State.PLAYER_STATE_IN_CHANGE:  flags = false; break;

            default: break;
        }

        return flags;
    }

    /// <summary>
    /// 移動処理有効化処理
    /// </summary>
    /// <param name="flags">フラグ</param>
    private void MoveEnable( bool flags )
    {
        // 現在状態からどれに設定にするか判断
        switch( gameObj.PhaseParam )
        {
            case Game.Phase.GAME_PAHSE_READY:
                break;

            case Game.Phase.GAME_PAHSE_CITY:
                if( cityPhaseMoveObj == null )
                {
                    cityPhaseMoveObj = GameObject.Find( citySpawnManagerPath ).GetComponent<CityPhaseMove>();
                }

                cityPhaseMoveObj.IsEnable = flags;
                break;

            case Game.Phase.GAME_PAHSE_STAR:
                if( starPhaseMoveObj == null )
                {
                    starPhaseMoveObj = GameObject.Find( starPhaseMoveObjPath ).GetComponent<StarPhaseMove>();
                }

                starPhaseMoveObj.IsEnable = flags;
                break;
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

            string str = "現在状態:" + StateParam;
            //str = "速度ベクトル:" + VelocityVec + "\n速度量:" + VelocityVec.magnitude + "\nフレーム間速度:" + velocity;

            GUI.Label( new Rect( 0 , 200 , 800 , 600 ) , str , guiStyle );
        }
    }
}


