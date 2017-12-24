using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  //画面遷移を可能にする
using UnityEngine;
using UnityEngine.Playables;
using System;

/// <summary>
/// ゲームシーン管理クラス
/// </summary>
/// <remarks>
/// シーンの状態管理などを主に行う。
/// </remarks>
public class Game : MonoBehaviour {

    // プレハブ系
    // Hierarchy上から設定する
    [SerializeField] GameObject CityPrefab;
    [SerializeField] GameObject StarPrefab;
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] GameObject mainCameraPrefab;
    [SerializeField] GameObject guiCameraPrefab;
    [SerializeField] GameObject starSpawnPrefab;
    [SerializeField] GameObject MiniMapPrefab;
    [SerializeField] GameObject effectManagerPrefab;
    [SerializeField] GameObject soundManagerPrefab;
    [SerializeField] GameObject skyboxManagerPrefab;
    [SerializeField] GameObject transitionPrefab;
    [SerializeField] GameObject timelinePrefab;
    [SerializeField] GameObject inputPrefab;
    [SerializeField] GameObject npcVehiclesPrefab;
    [SerializeField] GameObject pointsListPrefab;
    [SerializeField] GameObject scoutShipPrefab;
    [SerializeField] GameObject shipPointsPrefab;
    [SerializeField] GameObject citySpawnListPrefab;
    [SerializeField] GameObject citySpawnManagerPrefab;
    [SerializeField] GameObject vcManagerPrefab;
    [SerializeField] GameObject mobSpawnListPrefab;
    [SerializeField] GameObject mobSpawnManagerPrefab;

    // オブジェクト系
    // シーン中シーン管理上操作したい場合に保持しておく
    GameObject CityObj;
    GameObject StarObj;
    GameObject mainCameraObj;
    GameObject guiCameraObj;
    GameObject starSpawnManagerObj;
    GameObject MiniMapObj;
    GameObject effectManagerObj;
    GameObject soundManagerObj;
    GameObject skyboxManagerObj;
    GameObject transitionObj;
    GameObject npcVehiclesObj;
    GameObject pointsListObj;
    GameObject scoutShipObj;
    GameObject shipPointsObj;
    GameObject citySpawnListObj;
    GameObject citySpawnManagerObj;
    GameObject vcManagerObj;
    GameObject mobSpawnListObj;
    GameObject mobSpawnManagerObj;

    Player playerObj;
    TimeCtrl timeObj;
    TimeManager timeManagerObj;
    ScoreManager scoreManagerObj;
    TimelineManager timelineObj;
    LoveP_Input inputObj;
    StartLogo startLogoObj;


    /// <summary>
    /// ゲームシーン状態の列挙値
    /// </summary>
    public enum Phase
    {
        GAME_PHASE_INIT = 0,
        GAME_PAHSE_READY,
        GAME_PAHSE_STARTLOGO ,
        GAME_PAHSE_CITY,
        GAME_PAHSE_STAR_SHIFT,
        GAME_PAHSE_STAR,
        GAME_PAHSE_END,

        GAME_PAHSE_NUM,
    }

    /// <summary>
    /// 状態に応じた汎用タイマー変数
    /// </summary>
    float phaseTimer;

    /// <summary>
    /// ゲームシーンフェーズ変数
    /// </summary>
    public Phase PhaseParam { get { return phaseParam; } set { SetPhase( value ); } }

    /// <summary>
    /// ゲームシーンフェーズ変数のバッキングストア。値を変更するときはプロパティ側を基本利用。
    /// </summary>
    [SerializeField] Phase phaseParam;

    /// <summary>
    /// デバッグ用フラグ変数
    /// デバッグ時にしたくない処理を除外する時などに使うこと
    /// </summary>
    public static bool IsDebug;

    [SerializeField] bool isDebug;

    /// <summary>
    /// OnGUI有効化フラグ
    /// 他のスクリプトで参照してOnGUIを出す出さないを選択する
    /// </summary>
    public static bool IsOnGUIEnable;

    [SerializeField] bool isOnGUIEnable;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        // 変数初期化
        phaseTimer = 0.0f;

        // オブジェクト生成処理など
        InitCreateObjects();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start () {
        // フェイズステータス設定
        // Hierarchy上でフェイズを設定できるように依存した処理はできるだけかかない
        PhaseParam = phaseParam;
    }
	
	/// <summary>
    /// 更新処理
    /// </summary>
	void Update () {
		switch( phaseParam )
        {
            case Phase.GAME_PHASE_INIT:
                PhaseParam = Phase.GAME_PAHSE_READY;
                break;

            case Phase.GAME_PAHSE_READY:
                {
                    if ( timelineObj.stateType == TimelineManager.STATETYPE.TIMELINE_NONE )
                    {
                        PhaseParam = Phase.GAME_PAHSE_STARTLOGO;
                    }
                    break;
                }

            case Phase.GAME_PAHSE_STARTLOGO:
                {
                    if (startLogoObj.stateType == StartLogo.STATETYPE.NONE)
                    {
                        PhaseParam = Phase.GAME_PAHSE_CITY;
                    }
                }
                break;

            case Phase.GAME_PAHSE_CITY:
                {
                    //デバッグ用
                    if( isDebug && Input.GetKeyUp( KeyCode.P ) )
                    {
                        //PhaseParam = Phase.GAME_PAHSE_STAR;
                    }
                    if (isDebug && Input.GetKeyUp(KeyCode.I))
                    {
                        //PhaseParam = Phase.GAME_PAHSE_STAR_SHIFT;
                    }

                    if ( timeObj.GetComponent<TimeCtrl>().GetTime() <= 0 && !isDebug )
                    {
                        SceneManager.LoadScene("Result");
                    }
                    break;
                }

            case Phase.GAME_PAHSE_STAR_SHIFT:
                {
                    // HACK : 田口　2017/12/7
                    //タイムラインが終了するとフェーズ移行
                    //ただこの場合タイムラインを途中で止めてもフェーズ移行する
                    if (PlayState.Paused == timelineObj.Get("StarShiftTimeline").State())
                    {
                        //PhaseParam = Phase.GAME_PAHSE_STAR;
                    }
                    break;
                }

            case Phase.GAME_PAHSE_STAR:
                {
                    if (timeObj.GetComponent<TimeCtrl>().GetTime() <= 0 && !isDebug )
                    {
                        SceneManager.LoadScene("Result");
                    }
                    break;
                }

            case Phase.GAME_PAHSE_END:
                {
                    if (inputObj.GetButton("Fire1"))
                    {
                        SceneManager.LoadScene("Result");
                    }
                }
                break;
        }

        //デバッグ用
        if( isDebug && Input.GetKeyUp( KeyCode.O ) )
        {
            transitionObj.GetComponent<Transition>().StartTransition("Result");
        }

        if( Input.GetKeyDown( KeyCode.L ) )
        {
            //PhaseParam = ( Phase )Mathf.Min( ( ( int )PhaseParam + 1 ) , ( ( int )Phase.GAME_PAHSE_NUM - 1 ) );
        }


        
        // HACK: OnGUIデバッグ時On・Off処理
        //       もっといい方法がありそうだけど現状これで
        IsOnGUIEnable = isOnGUIEnable;
        IsDebug = isDebug;
    }

    /// <summary>
    /// フェーズ設定諸処理
    /// </summary>
    /// <param name="phase">設定値</param>
    private void SetPhase( Phase phase )
    {
        phaseParam = phase;

        switch( phaseParam )
        {
            case Phase.GAME_PHASE_INIT:
                PhaseInitStart();
                break;

            case Phase.GAME_PAHSE_READY:
                PhaseReadyStart();
                break;

            case Phase.GAME_PAHSE_STARTLOGO:
                PhaseStartLogoStart();
                break;

            case Phase.GAME_PAHSE_CITY:
                PhaseCityStart();
                break;

            case Phase.GAME_PAHSE_STAR_SHIFT:
                PhaseStarShiftStart();
                break;

            case Phase.GAME_PAHSE_STAR:
                PhaseStarStart();
                break;

            case Phase.GAME_PAHSE_END:
                PhaseEndStart();
                break;
        }
    }

    // 同じコンポーネントを使ってるところは後できれいにする
    // TODO: 各フェイズ初期化処理。あとで確実に問題が生じるので、何か不都合が生じたら優先して見る！
    void PhaseInitStart()
    {
        CityObj.SetActive( true );
        StarObj.SetActive( false );
        citySpawnManagerObj.SetActive( true );
        starSpawnManagerObj.SetActive( false );
        timeManagerObj.SetState( TimeManager.State.TIME_STATE_STOP );
        scoreManagerObj.SetState( ScoreManager.State.SCORE_STATE_STOP );
        playerObj.CityPhaseInit();
        playerObj.MoveEnable( false );
        //mainCameraObj.GetComponent<LovePCameraController>().enabled = true;
        //mainCameraObj.GetComponent<StarCameraController>().enabled = false;
        skyboxManagerObj.GetComponent<SkyboxManager>().SetCitySkyBox();
        phaseTimer = 0;
    }

    void PhaseReadyStart()
    {
        CityObj.SetActive( true );
        StarObj.SetActive( false );
        citySpawnManagerObj.SetActive( true );
        starSpawnManagerObj.SetActive( false );
        timeManagerObj.SetState(TimeManager.State.TIME_STATE_STOP);
        scoreManagerObj.SetState(ScoreManager.State.SCORE_STATE_STOP);
        timelineObj.Get("ReadyTimeline").Play();                                //タイムライン開始
        timelineObj.SetStateType(TimelineManager.STATETYPE.TIMELINE_START);
        playerObj.CityPhaseInit();
        playerObj.MoveEnable( false );
        //mainCameraObj.GetComponent<LovePCameraController>().enabled = true;
        //mainCameraObj.GetComponent<StarCameraController>().enabled = false;
        skyboxManagerObj.GetComponent<SkyboxManager>().SetCitySkyBox();
        phaseTimer = 0;
    }

    void PhaseStartLogoStart()
    {
        startLogoObj.gameObject.SetActive(true);
        startLogoObj.SetStateType(StartLogo.STATETYPE.SRIDE_IN);
    }

    void PhaseCityStart()
    {
        CityObj.SetActive( true );
        StarObj.SetActive( false );
        citySpawnManagerObj.SetActive( true );
        starSpawnManagerObj.SetActive( false );
        timeManagerObj.SetState( TimeManager.State.TIME_STATE_RUN );
        scoreManagerObj.SetState(ScoreManager.State.SCORE_STATE_RUN);
        playerObj.CityPhaseInit();
        playerObj.MoveEnable( true );
        //mainCameraObj.GetComponent<LovePCameraController>().enabled = true;
        //mainCameraObj.GetComponent<StarCameraController>().enabled = false;
        skyboxManagerObj.GetComponent<SkyboxManager>().SetCitySkyBox();
    }

    void PhaseStarShiftStart()
    {
        //タイムライン開始
        timelineObj.Get("StarShiftTimeline").Play();
        playerObj.MoveEnable( false );
    }

    void PhaseStarStart()
    {
        CityObj.SetActive(false);
        StarObj.SetActive(true);
        citySpawnManagerObj.SetActive(false);
        starSpawnManagerObj.SetActive(true);
        starSpawnManagerObj.GetComponent<StarSpawnManager>().Init();
        playerObj.StarPhaseInit();
        playerObj.MoveEnable( true );

        timeManagerObj.SetState( TimeManager.State.TIME_STATE_RUN );
        scoreManagerObj.SetState(ScoreManager.State.SCORE_STATE_RUN);

        mainCameraObj.GetComponent<LovePCameraController>().enabled = false;
        mainCameraObj.GetComponent<StarCameraController>().enabled = true;

        MiniMapObj.GetComponent<MiniMap>().enabled = false;
        MiniMapObj.GetComponent<StarMiniMap>().enabled = true;

        npcVehiclesObj.SetActive( false );
        pointsListObj.SetActive( false );
        scoutShipObj.SetActive( false );
        shipPointsObj.SetActive( false );

        skyboxManagerObj.GetComponent<SkyboxManager>().SetStarSkyBox();
    }

    void PhaseEndStart()
    {
        playerObj.MoveEnable( false );
    }

    /// <summary>
    /// 初期化時オブジェクト生成処理
    /// </summary>
    /// <remarks>
    /// Hierarchy上にプレハブと同名のオブジェクトが無いか検索し、無ければ生成。
    /// ある場合はHierarchy上の物を使用する。
    /// Debug時などはHierarchy上のもののほうが使い勝手が良いため。
    /// </remarks>
    private void InitCreateObjects()
    {
        // HACK: ラムダ式で生成処理を実装
        //       効率が良いのかは分からん
        Func< GameObject , GameObject > Create = ( GameObject prefabs ) =>
        {
            GameObject obj = GameObject.Find( prefabs.name );

            if( obj == null )
            {
                obj = Instantiate( prefabs );
                obj.name = prefabs.name;
            }
           
            return obj;
        };

        // 各オブジェクトの生成
        CityObj             = Create( CityPrefab );
        StarObj             = Create( StarPrefab );
        mainCameraObj       = Create( mainCameraPrefab );
        guiCameraObj        = Create( guiCameraPrefab );
        starSpawnManagerObj = Create( starSpawnPrefab );
        MiniMapObj          = Create( MiniMapPrefab );
        effectManagerObj    = Create( effectManagerPrefab );
        soundManagerObj     = Create( soundManagerPrefab );
        playerObj           = Create( PlayerPrefab ).GetComponent<Player>();
        skyboxManagerObj    = Create( skyboxManagerPrefab );
        transitionObj       = Create( transitionPrefab );
        timelineObj         = Create( timelinePrefab ).GetComponent<TimelineManager>();
        inputObj            = Create( inputPrefab ).GetComponent<LoveP_Input>();
        npcVehiclesObj      = Create( npcVehiclesPrefab );
        pointsListObj       = Create( pointsListPrefab );
        scoutShipObj        = Create( scoutShipPrefab );
        shipPointsObj       = Create( shipPointsPrefab );
        citySpawnManagerObj = Create( citySpawnManagerPrefab );
        citySpawnListObj    = Create( citySpawnListPrefab );
        vcManagerObj        = Create( vcManagerPrefab );
        mobSpawnManagerObj  = Create( mobSpawnManagerPrefab );
        mobSpawnListObj     = Create( mobSpawnListPrefab );

        // HACK: 直接生成したもの以外で保持したいオブジェクトを取得
        //       直接パスを記述。後に変更したほうがいいか？
        timeObj = GameObject.Find("Time").GetComponent<TimeCtrl>();
        timeManagerObj = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        scoreManagerObj = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        startLogoObj = GameObject.Find("StartLogo").GetComponent<StartLogo>();
    }

}
