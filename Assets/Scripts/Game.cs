﻿using System.Collections;
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
    [SerializeField] GameObject SpawnManagerPrefab;
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

    // オブジェクト系
    // シーン中シーン管理上操作したい場合に保持しておく
    GameObject CityObj;
    GameObject StarObj;
    GameObject PlayerObj;
    GameObject mainCameraObj;
    GameObject guiCameraObj;
    GameObject SpawnManagerObj;
    GameObject starSpawnManagerObj;
    GameObject TimeObj;
    GameObject MiniMapObj;
    GameObject effectManagerObj;
    GameObject soundManagerObj;
    GameObject skyboxManagerObj;
    GameObject transitionObj;
    GameObject npcVehiclesObj;
    GameObject pointsListObj;
    GameObject scoutShipObj;
    GameObject shipPointsObj;
    
    TimelineManager timelineObj;
    LoveP_Input inputObj;

    int readyCount;

    public enum Phase
    {
        GAME_PAHSE_READY = 0,
        GAME_PAHSE_CITY,
        GAME_PAHSE_STAR_SHIFT,
        GAME_PAHSE_STAR,
        GAME_PAHSE_END
    }

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
            case Phase.GAME_PAHSE_READY:
                {
                    readyCount++;
                    if( readyCount > 180 )
                    {
                        PhaseParam = Phase.GAME_PAHSE_CITY;
                    }
                    break;
                }

            case Phase.GAME_PAHSE_CITY:
                {
                    //デバッグ用
                    if( isDebug && Input.GetKeyUp( KeyCode.P ) )
                    {
                        PhaseParam = Phase.GAME_PAHSE_STAR;
                    }
                    if (isDebug && Input.GetKeyUp(KeyCode.I))
                    {
                        PhaseParam = Phase.GAME_PAHSE_STAR_SHIFT;
                    }

                    if ( TimeObj.GetComponent<TimeCtrl>().GetTime() <= 0 && !isDebug )
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
                        PhaseParam = Phase.GAME_PAHSE_STAR;
                    }
                    break;
                }

            case Phase.GAME_PAHSE_STAR:
                {
                    if (TimeObj.GetComponent<TimeCtrl>().GetTime() <= 0 && !isDebug )
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
            case Phase.GAME_PAHSE_READY:
                PhaseReadyStart();
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
    void PhaseReadyStart()
    {
        //SetPhase(Phase.GAME_PAHSE_CITY);
        TimeObj.GetComponent<TimeCtrl>().SetState(TimeCtrl.State.TIME_STATE_STOP);
        readyCount = 0;
    }

    void PhaseCityStart()
    {
        CityObj.SetActive(true);
        StarObj.SetActive(false);
        SpawnManagerObj.SetActive(true);
        starSpawnManagerObj.SetActive(false);
        mainCameraObj.GetComponent<LovePCameraController>().enabled = true;
        PlayerObj.GetComponent<Player>().CityPhaseInit();
        TimeObj.GetComponent<TimeCtrl>().SetState(TimeCtrl.State.TIME_STATE_RUN);
        //SpawnManagerObj.GetComponent<CitySpawnManager>().HumanCreate(1, Human.GROUPTYPE.PEAR);
        skyboxManagerObj.GetComponent<SkyboxManager>().SetCitySkyBox();
    }

    void PhaseStarShiftStart()
    {
        //タイムライン開始
        timelineObj.Get("StarShiftTimeline").Play();
    }

    void PhaseStarStart()
    {
        CityObj.SetActive(false);
        StarObj.SetActive(true);
        SpawnManagerObj.SetActive(false);
        starSpawnManagerObj.SetActive(true);
        starSpawnManagerObj.GetComponent<StarSpawnManager>().Init();
        PlayerObj.GetComponent<Player>().StarPhaseInit();
        TimeObj.GetComponent<TimeCtrl>().SetState( TimeCtrl.State.TIME_STATE_RUN );
        mainCameraObj.GetComponent<LovePCameraController>().enabled = false;
        mainCameraObj.GetComponent<StarCameraController>().enabled = true;
        MiniMapObj.GetComponent<MiniMap>().enabled = false;
        MiniMapObj.GetComponent<StarMiniMap>().enabled = true;
        skyboxManagerObj.GetComponent<SkyboxManager>().SetStarSkyBox();
    }

    void PhaseEndStart()
    {

    }

    //タイトルシーンから移行
    //初期設定を行う（Playerの位置、GameStateの設定、初期乗客スポーン）
    //ゲーム開始準備（Ready,Playerの操作を受け付けない）
    //ゲーム開始（Playerを動けるようにする、タイムの動作開始）
    //ゲーム中
    //時間切れでリザルトに遷移

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
        CityObj = Create( CityPrefab );
        StarObj = Create( StarPrefab );

        mainCameraObj       = Create( mainCameraPrefab );
        guiCameraObj        = Create( guiCameraPrefab );
        SpawnManagerObj     = Create( SpawnManagerPrefab );
        starSpawnManagerObj = Create( starSpawnPrefab );
        MiniMapObj          = Create( MiniMapPrefab );
        effectManagerObj    = Create( effectManagerPrefab );
        soundManagerObj     = Create( soundManagerPrefab );
        PlayerObj           = Create( PlayerPrefab );
        skyboxManagerObj    = Create( skyboxManagerPrefab );
        transitionObj       = Create( transitionPrefab );
        timelineObj         = Create( timelinePrefab ).GetComponent<TimelineManager>();
        inputObj            = Create( inputPrefab ).GetComponent<LoveP_Input>();
        npcVehiclesObj      = Create( npcVehiclesPrefab );
        pointsListObj       = Create( pointsListPrefab );
        scoutShipObj        = Create( scoutShipPrefab );
        shipPointsObj       = Create( shipPointsPrefab );

        // HACK: 直接生成したもの以外で保持したいオブジェクトを取得
        //       直接パスを記述。後に変更したほうがいいか？
        TimeObj = GameObject.Find( "Time" );
    }

}
