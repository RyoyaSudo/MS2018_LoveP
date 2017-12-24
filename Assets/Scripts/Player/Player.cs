using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region 変数宣言

    public int RideCount { get { return rideCount; } }
    int rideCount; //現在乗車人数
    int rideGroupNum; //グループ乗車人数

    /// <summary>
    /// 乗車中の
    /// </summary>
    public Human[] RidePassengerObj { get { return ridePassengerObj; } }
    private Human[] ridePassengerObj;

    private GameObject scoreObj;

    /// <summary>
    /// スポーンマネージャーオブジェクト。
    /// 乗客生成にまつわる処理をさせるため必要。
    /// </summary>
    private CitySpawnManager citySpawnManagerObj;
    public string citySpawnManagerPath;

    private StarSpawnManager starSpawnManagerObj;
    public string starSpawnManagerPath;

    PassengerController.GROUPTYPE passengerType;//乗客タイプ

    /// <summary>
    /// 最後に乗車した乗客オブジェクト。
    /// 主にデバッグ処理に用いる
    /// </summary>
    public Human lastRideHuman;

    /// <summary>
    /// 待ち状態の人
    /// </summary>
    public GameObject awaitHumanObj;

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
    private SoundController.SoundsSeType playerType;  //プレイヤーの車両用
    private AudioSource driveSoundSource;
    public float soundVolume;
    public float min_rate = 0.0f;
    public float max_rate = 3.0f;
    //[SerializeField] AudioClip runSound;

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

    /// <summary>
    /// 街フェイズ時の挙動管理オブジェクト
    /// </summary>
    CityPhaseMove cityPhaseMoveObj;
    [SerializeField] string cityPhaseMoveObjPath;

    /// <summary>
    /// 星フェイズ時の挙動管理オブジェクト
    /// </summary>
    StarPhaseMove starPhaseMoveObj;
    [SerializeField] string starPhaseMoveObjPath;

    bool changeFade;

    /// <summary>
    /// 乗り物管理オブジェクト
    /// </summary>
    PlayerVehicle vehicleControllerObj;
    [SerializeField] string vehicleControllerObjPath;

    /// <summary>
    /// タイムラインマネージャー
    /// </summary>
    private TimelineManager timelineManagerObj;
    [SerializeField] private string timelineManagerPath;

    #endregion 変数宣言

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        // 初期化系
        Velocity = 0.0f;
        VelocityVec = Vector3.zero;
        velocityVecOld = Vector3.zero;

        cityPhaseMoveObj = null;
        starPhaseMoveObj = null;

        changeFade = true;

        IsStopped = false;

        StateTimer = 0.0f;

        lastRideHuman = null;

        // シーン内から必要なオブジェクトを取得
        gameObj = GameObject.Find( gamectrlObjPath ).GetComponent<Game>();

        // HACK: 乗客保存配列に関して
        //       最大乗客数分確保しておく
        ridePassengerObj = new Human[ 5 ];

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
        passengerTogetherUIObj = GameObject.Find( passengerTogetherUIObjPath );
        vehicleControllerObj = GameObject.Find( vehicleControllerObjPath ).GetComponent<PlayerVehicle>();
        timelineManagerObj = GameObject.Find(timelineManagerPath).GetComponent<TimelineManager>();

        //サウンド用//////////////////////////////////////
        playerSoundCtrl = GameObject.Find( "SoundManager" ).GetComponent<SoundController>();
        //オブジェクトについているAudioSourceを取得する
        AudioSource[] audioSources = GetComponents<AudioSource>();
        playerAudioS = audioSources[0];
        driveSoundSource = audioSources[1];

        driveSoundSource.clip = playerSoundCtrl.GetSeList( SoundController.SoundsSeType.CITY_DRIVE_SOUND ).clip;
        driveSoundSource.loop = true;
        driveSoundSource.volume = playerSoundCtrl.GetSeList(SoundController.SoundsSeType.CITY_DRIVE_SOUND).volume;
        driveSoundSource.Play();
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

        // デバッグ時処理
        if( Game.IsDebug )
        {
            DebugFunc();
        }

        //ドライブ音のピッチ設定
        min_rate = Mathf.Clamp(min_rate, 0.0f, max_rate);
        max_rate = Mathf.Clamp(max_rate, min_rate, 5.0f);
        float rate;
        rate = cityPhaseMoveObj.Velocity / cityPhaseMoveObj.VelocityMax;      
        driveSoundSource.pitch = Mathf.Lerp(min_rate, max_rate, rate);
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

        Debug.Log( "街フェイズ開始" );
    }

    /// <summary>
    /// 星フェイズ開始時のプレイヤー初期化処理
    /// </summary>
    public void StarPhaseInit()
    {
        if( vehicleControllerObj == null )
        {
            vehicleControllerObj = GameObject.Find( vehicleControllerObjPath ).GetComponent<PlayerVehicle>();
        }

        vehicleControllerObj.VehicleScore = vehicleControllerObj.VehicleScoreLimit[ ( int )PlayerVehicle.Type.AIRPLANE ];
        vehicleControllerObj.VehicleType = PlayerVehicle.Type.AIRPLANE;
        starSpawnManagerObj = GameObject.Find( starSpawnManagerPath ).GetComponent<StarSpawnManager>();

        if( cityPhaseMoveObj == null )
        {
            cityPhaseMoveObj = GameObject.Find( cityPhaseMoveObjPath ).GetComponent<CityPhaseMove>();
        }

        cityPhaseMoveObj.IsEnable = false;

        // TODO: 星フェイズ開始時のプレイヤー初期位置設定
        //       Earthオブジェクトにプレイヤー初期位置のスポーンポイントを仕込んでおいて、そこから設定する形に変更したほうがよさそう
        starPhaseMoveObj = GameObject.Find( starPhaseMoveObjPath ).GetComponent<StarPhaseMove>();
        starPhaseMoveObj.IsEnable = true;

        starPhaseMoveObj.Initialize();
        StateParam = State.PLAYER_STATE_FREE;

        Debug.Log( "星フェイズ開始" );
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
        switch( vehicleControllerObj.VehicleType )
        {
            case PlayerVehicle.Type.BIKE:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.SpawnHumanGroup( human.PassengerControllerObj.spawnPlace , human.PassengerControllerObj.groupType , human.CurrentModelType );
                    break;
                }
            case PlayerVehicle.Type.CAR:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.SpawnHumanGroup( human.PassengerControllerObj.spawnPlace , human.PassengerControllerObj.groupType , human.CurrentModelType );
                    break;
                }
            case PlayerVehicle.Type.BUS:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.SpawnHumanGroup( human.PassengerControllerObj.spawnPlace , human.PassengerControllerObj.groupType , human.CurrentModelType );
                    break;
                }
            case PlayerVehicle.Type.AIRPLANE:
                {
                    //乗物によって生成する人を設定
                    starSpawnManagerObj.SpawnHumanGroup( human.PassengerControllerObj.spawnPlace , human.PassengerControllerObj.groupType );
                    //citySpawnManagerObj.SpawnHumanGroup(human.spawnPlace, human.groupType);
                    break;
                }
        }
    }

    /// <summary>
    /// 乗り物に応じでスポーンマネージャを分ける(相方用）
    /// </summary>
    public void HumanCreateGroup( int ignoreSpanwPlace )
    {
        PlayerVehicle.Type type = vehicleControllerObj.VehicleType;

        switch( type )
        {
            case PlayerVehicle.Type.BIKE:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.HumanCreateByVehicleType( type , ignoreSpanwPlace , 2 , 2 , 2 );
                    break;
                }
            case PlayerVehicle.Type.CAR:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.HumanCreateByVehicleType( type , ignoreSpanwPlace , 2 , 2 , 2 );
                    break;
                }
            case PlayerVehicle.Type.BUS:
                {
                    //乗物によって生成する人を設定
                    citySpawnManagerObj.HumanCreateByVehicleType( type , ignoreSpanwPlace , 2 , 2 , 2 );
                    break;
                }
            case PlayerVehicle.Type.AIRPLANE:
                {
                    //乗物によって生成する人を設定
                    starSpawnManagerObj.HumanCreateByVehicleType( type , ignoreSpanwPlace , 2 , 2 , 2 );
                    //citySpawnManagerObj.HumanCreateByVehicleType(vehicleType, human.spawnPlace, 2, 2, 2);
                    break;
                }
        }
    }

    /// <summary>
    /// 乗客乗車処理
    /// </summary>
    /// <param name="human">対象者</param>
    public void PassengerRide( Human human )
    {
        // 可否を確認し、乗車処理を実行
        if( RideEnableCheck() )
        {
            //乗車待機状態じゃないならbreak;
            if( human.CurrentStateType != Human.STATETYPE.READY ) return;

            // 乗車オブジェクト更新
            lastRideHuman = human;

            //最初の乗客の時に他の乗客生成を行う
            if( rideCount == 0 )
            {
                // HACK: 最初の乗客を乗せた際、他の乗客を街人に変える処理
                //       10/24現在では他の乗客はFindして消してしまうやり方をする。
                human.IsProtect = true;

                PassengerDeleteAll();

                passengerType = human.PassengerControllerObj.groupType;

                // 乗客数の確認
                switch( human.PassengerControllerObj.groupType )
                {
                    case PassengerController.GROUPTYPE.Lovers:
                        rideGroupNum = 2;
                        //Debug.Log( "PEAR" );
                        break;

                    case PassengerController.GROUPTYPE.Family:
                        rideGroupNum = 3;
                        //Debug.Log( "SMALL" );
                        break;

                    case PassengerController.GROUPTYPE.Friends:
                        rideGroupNum = 5;
                        //Debug.Log( "BIG" );
                        break;

                    default:
                        Debug.LogError( "エラー:設定謎の乗客タイプが設定されています" );
                        break;
                }

                HumanCreate( human );

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
            //Debug.Log( "Ride" );
            ridePassengerObj[ rideCount ] = human;
            rideCount++;

            //フェイスUIをONにする
            passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().FaiceUION( rideCount );

            // 乗客の当たり判定を消す
            human.GetHumanModelCollider().isTrigger = true;

            //最後の人なら降ろす
            if( rideCount >= rideGroupNum )
            {
                PassengerGetOff( human );

                // 降車するのでnullに
                lastRideHuman = null;
            }
            else
            {
                // 乗客はまだ1人以上残っているため、乗車待機状態に
                StateParam = State.PLAYER_STATE_TAKE_READY;

                // TODO : 田口　2017/11/30
                //乗客の状態を「乗車」に
                human.CurrentStateType = Human.STATETYPE.RIDE;

                //乗車SE
                playerSoundCtrl.PlayOneShot(SoundController.SoundsSeType.PASSENGER_RIDE, playerAudioS);
            }
        }
    }

    /// <summary>
    /// 乗り物に応じた動作をする
    /// </summary>
    void VehicleMove()
    {
        switch( vehicleControllerObj.VehicleType )
        {
            case PlayerVehicle.Type.BIKE:
                {
                    Velocity = cityPhaseMoveObj.Velocity;
                    //playerType = SoundController.SoundsSeType.BIKE_RUN;   //プレイヤーの車両によってSEも変更する
                    break;
                }
            case PlayerVehicle.Type.CAR:
                {
                    Velocity = cityPhaseMoveObj.Velocity;
                   // playerType = SoundController.SoundsSeType.CAR_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
            case PlayerVehicle.Type.BUS:
                {
                    Velocity = cityPhaseMoveObj.Velocity;
                    //playerType = SoundController.SoundsSeType.BUS_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
            case PlayerVehicle.Type.AIRPLANE:
                {
                    //playerType = SoundController.SoundsSeType.AIRPLANE_RUN;//プレイヤーの車両によってSEも変更する
                    break;
                }
        }
    }

    /// <summary>
    /// 乗り物変化開始前準備処理
    /// </summary>
    void VehicleChangeStart()
    {
        // 変化するかチェック
        if( vehicleControllerObj.ChangeCheck() )
        {
            // 内部変数の変化後に、それに応じた処理を実行
            switch ( vehicleControllerObj.VehicleType )
            {
                case PlayerVehicle.Type.BIKE:
                    changeEffectObj.Play();
                    MoveEnable( false );
                    StateParam = State.PLAYER_STATE_IN_CHANGE;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;

                    playerSoundCtrl.PlayOneShot( SoundController.SoundsSeType.VEHICLE_CHANGE_BIKE , playerAudioS );
                    break;

                case PlayerVehicle.Type.CAR:
                    changeEffectObj.Play();
                    MoveEnable( false );
                    StateParam = State.PLAYER_STATE_IN_CHANGE;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;

                    playerSoundCtrl.PlayOneShot( SoundController.SoundsSeType.VEHICLE_CHANGE_CAR , playerAudioS );
                    break;
                    
                case PlayerVehicle.Type.BUS:
                    changeEffectObj.Play();
                    MoveEnable( false );
                    StateParam = State.PLAYER_STATE_IN_CHANGE;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;

                    playerSoundCtrl.PlayOneShot( SoundController.SoundsSeType.VEHICLE_CHANGE_BUS , playerAudioS );
                    break;

                case PlayerVehicle.Type.AIRPLANE:
                    //星フェーズへの移行開始
                    playerSoundCtrl.PlayOneShot( SoundController.SoundsSeType.VEHICLE_CHANGE_AIRPLANE , playerAudioS );
                    //ChangeStarPhase();
                    break;

                default:
                    Debug.LogError( "未確定の乗り物タイプが指定されました。" );
                    break;
            }
        }
    }

    /// <summary>
    /// 車両変化状態時の処理
    /// </summary>
    void VehicleChange()
    {
        // TODO : 田口 2017/12/17
        //指定時間がたつとStateが変わるように変更しました
        if (timelineManagerObj.stateType == TimelineManager.STATETYPE.TIMELINE_NONE)
        {
            StateParam = State.PLAYER_STATE_FREE;
            MoveEnable(true);
            changeFade = true;
        }

        //// HACK: 車両変化時の演出に関する部分
        ////       煙エフェクトを出しつつスケール値を段々小さくしたあとに段々大きくなって現れる感じにしたい。
        //if( changeFade )
        //{
        //    // HACK: 乗り物変化演出に関して
        //    //       2017/12/04にPlayerVehicle.csに処理を分けた際、問題が発生する恐れあり。
        //    //       Morphing関数の呼び出し方を工夫する必要がありそうか？
        //    changeFade = false;
        //}
        //else
        //{
        //    StateParam = State.PLAYER_STATE_FREE;
        //    MoveEnable( true );
        //    changeFade = true;
        //}
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
        vehicleControllerObj.VehicleType = PlayerVehicle.Type.AIRPLANE;
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
    /// 状態設定処理
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
                MoveEnable( false );
                break;

            case State.PLAYER_STATE_TAKE:
                break;

            case State.PLAYER_STATE_GET_OFF:
                MoveEnable( false );
                break;

            case State.PLAYER_STATE_IN_CHANGE:
                // TODO : 田口　2017/12/17
                //チェンジタイムライン開始
                timelineManagerObj.Get("ChangeTimeline").Play();
                timelineManagerObj.SetStateType(TimelineManager.STATETYPE.TIMELINE_START);
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
        if (timelineManagerObj.stateType == TimelineManager.STATETYPE.TIMELINE_NONE)
        {
            StateParam = State.PLAYER_STATE_TAKE;
            MoveEnable(true);
        }
    }

    /// <summary>
    /// 下車動作中に行う動作処理
    /// </summary>
    private void GetOff()
    {
        if (timelineManagerObj.stateType == TimelineManager.STATETYPE.TIMELINE_NONE)
        {
            StateParam = State.PLAYER_STATE_FREE;
            MoveEnable(true);

            // 乗客オブジェクト解放
            for( int i = 0 ; i < rideCount ; i++ )
            {
                ridePassengerObj[ i ].transform.parent = null;
                ridePassengerObj[ i ].GetHumanModelCollider().isTrigger = false;
            }

            rideCount = 0;

            // 乗り物変化開始
            VehicleChangeStart();
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
    public void MoveEnable( bool flags )
    {
        // 現在状態からどれに設定にするか判断
        switch( gameObj.PhaseParam )
        {
            case Game.Phase.GAME_PAHSE_READY:
                if( cityPhaseMoveObj == null )
                {
                    cityPhaseMoveObj = GameObject.Find( citySpawnManagerPath ).GetComponent<CityPhaseMove>();
                }

                cityPhaseMoveObj.IsEnable = flags;
                break;

            case Game.Phase.GAME_PAHSE_CITY:
                if( cityPhaseMoveObj == null )
                {
                    cityPhaseMoveObj = GameObject.Find( citySpawnManagerPath ).GetComponent<CityPhaseMove>();
                }

                cityPhaseMoveObj.IsEnable = flags;
                break;

            case Game.Phase.GAME_PAHSE_STAR_SHIFT:
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

            case Game.Phase.GAME_PAHSE_END:
                if( starPhaseMoveObj == null )
                {
                    starPhaseMoveObj = GameObject.Find( starPhaseMoveObjPath ).GetComponent<StarPhaseMove>();
                }

                starPhaseMoveObj.IsEnable = flags;
                break;
        }
    }

    /// <summary>
    /// 乗客降車処理
    /// </summary>
    private void PassengerGetOff( Human human )
    {
        for( int i = 0 ; i < rideCount ; i++ )
        {
            // TODO : 田口　2017/11/30
            //最後の人だけ状態を「待ち受け」に
            if( i == rideCount - 1 )
            {
                ridePassengerObj[ i ].GetComponent<Human>().CurrentStateType = Human.STATETYPE.AWAIT;
            }
            else //それ以外の状態は「下車」に
            {
                // TODO : 田口　2017/12/20
                // 状態が「待ち受け」の人のオブジェクトを取得
                ridePassengerObj[i].GetComponent<PassengerController>().SetGetOffAwaitObj(ridePassengerObj[rideCount - 1].GetComponent<Human>().gameObject);

                // TODO : 田口　2017/12/24
                // 後で上記も書き直す
                awaitHumanObj = ridePassengerObj[rideCount - 1].GetComponent<Human>().gameObject;

                // TODO : 田口　2017/12/24
                // 乗車順番を保存
                ridePassengerObj[i].GetComponent<PassengerController>().SetRideNumber(i); 

                ridePassengerObj[ i ].GetComponent<Human>().CurrentStateType = Human.STATETYPE.GETOFF;
            }
        }

        // HACK: スコア加算処理の場所
        //       現状プレイヤークラス内だが、後に変更の可能性有り。
        scoreObj.gameObject.GetComponent<ScoreCtrl>().AddScore( ( int )passengerType );

        // HACK: 乗客のタイプに応じて乗り物変更用のスコアを加算する
        //       決め打ちの数値のため、定数で定めるなどの工夫が必要かと
        switch( passengerType )
        {
            case PassengerController.GROUPTYPE.Lovers:
                //ペア作成時のSE再生///////////////////////////////////////////////
                playerSoundCtrl.PlayOneShot(SoundController.SoundsSeType.PASSENGER_COMPLETE, playerAudioS);
                vehicleControllerObj.VehicleScore += 1;
                break;

            case PassengerController.GROUPTYPE.Family:
                //ペア作成時のSE再生///////////////////////////////////////////////
                playerSoundCtrl.PlayOneShot(SoundController.SoundsSeType.PASSENGER_COMPLETE, playerAudioS);
                vehicleControllerObj.VehicleScore += 2;
                break;

            case PassengerController.GROUPTYPE.Friends:
                //ペア作成時のSE再生///////////////////////////////////////////////
                playerSoundCtrl.PlayOneShot(SoundController.SoundsSeType.PASSENGER_COMPLETE, playerAudioS);
                vehicleControllerObj.VehicleScore += 4;
                break;

            default:
                Debug.Log( "エラー:設定謎の乗客タイプが設定されています" );
                break;
        }

        // HACK: 下車状態へ移行
        //       下車状態時実行処理内で乗り物変化判定を行わなければならない。
        StateParam = State.PLAYER_STATE_GET_OFF;

        // HACK: 次の乗客を生成。
        //       後にゲーム管理側で行うように変更をかける可能性。現状はここで。
        //乗物によって生成する人を設定
        HumanCreateGroup( human.PassengerControllerObj.spawnPlace );

        //何人乗せるかUIの表示を終了
        passengerTogetherUIObj.GetComponent<PassengerTogetherUI>().PassengerTogetherUIEnd();
    }

    /// <summary>
    /// 乗客消去処理
    /// </summary>
    void PassengerDeleteAll()
    {
        GameObject[] humanAll = GameObject.FindGameObjectsWithTag( "Human" );

        foreach( GameObject deleteHuman in humanAll )
        {
            if( deleteHuman.GetComponent<Human>().IsProtect == false )
            {
                Destroy( deleteHuman.gameObject );
            }
        }
    }

    /// <summary>
    /// デバッグ時処理
    /// </summary>
    void DebugFunc()
    {
        // 乗り物変更( バイク )
        if( Input.GetKeyDown( KeyCode.T ) )
        {
            // 変化
            vehicleControllerObj.VehicleScore = vehicleControllerObj.VehicleScoreLimit[ ( int )PlayerVehicle.Type.BIKE ];
            vehicleControllerObj.ChangeCheck();

            // 再配置
            int ignorePlace = 0;

            if( lastRideHuman != null )
            {
                ignorePlace = lastRideHuman.PassengerControllerObj.spawnPlace;
            }

            PassengerDeleteAll();
            HumanCreateGroup( ignorePlace );

            // 状態設定
            StateParam = State.PLAYER_STATE_IN_CHANGE;
        }

        // 乗り物変更( 車 )
        if( Input.GetKeyDown( KeyCode.Y ) )
        {
            // 変化
            vehicleControllerObj.VehicleScore = vehicleControllerObj.VehicleScoreLimit[ ( int )PlayerVehicle.Type.CAR ];
            vehicleControllerObj.ChangeCheck();

            // 再配置
            int ignorePlace = 0;

            if( lastRideHuman != null )
            {
                ignorePlace = lastRideHuman.PassengerControllerObj.spawnPlace;
            }

            PassengerDeleteAll();
            HumanCreateGroup( ignorePlace );

            // 状態設定
            StateParam = State.PLAYER_STATE_IN_CHANGE;
        }

        // 乗り物変更( 大型車 )
        if( Input.GetKeyDown( KeyCode.U ) )
        {
            // 変化
            vehicleControllerObj.VehicleScore = vehicleControllerObj.VehicleScoreLimit[ ( int )PlayerVehicle.Type.BUS ];
            vehicleControllerObj.ChangeCheck();

            // 再配置
            int ignorePlace = 0;

            if( lastRideHuman != null )
            {
                ignorePlace = lastRideHuman.PassengerControllerObj.spawnPlace;
            }

            PassengerDeleteAll();
            HumanCreateGroup( ignorePlace );

            // 状態設定
            StateParam = State.PLAYER_STATE_IN_CHANGE;
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.tag);
    //    switch(collision.gameObject.tag)
    //    {
    //        case "Obstacle":
    //            {
    //                Debug.Log("hit");
    //                playerAudioS.PlayOneShot(playerSoundCtrl.AudioClipCreate(SoundController.SoundsSeType.BUMP_MIDDLE));
    //                break;
    //            }
    //        default:
    //            {
    //                break;
    //            }
    //    }
    //}

    public void PlaySoundEffect(SoundController.SoundsSeType sound)
    {
        playerSoundCtrl.PlayOneShot(sound, playerAudioS);
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


