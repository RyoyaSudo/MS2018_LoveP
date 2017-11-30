using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  //画面遷移を可能にする
using UnityEngine;
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
    public GameObject CityPrefab;
    public GameObject StarPrefab;
    public GameObject PlayerPrefab;
    public GameObject mainCameraPrefab;
    public GameObject guiCameraPrefab;
    public GameObject SpawnManagerPrefab;
    public GameObject starSpawnPrefab;
    public GameObject MiniMapPrefab;
    public GameObject effectManagerPrefab;
    public GameObject soundManagerPrefab;
    public GameObject skyboxManagerPrefab;
    public GameObject transitionPrefab;

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

    int readyCount;

    public enum Phase
    {
        GAME_PAHSE_READY = 0,
        GAME_PAHSE_CITY,
        GAME_PAHSE_STAR
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
    [SerializeField] bool debugFlags;

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
                    if( debugFlags && Input.GetKeyUp( KeyCode.P ) )
                    {
                        PhaseParam = Phase.GAME_PAHSE_STAR;
                    }

                    if ( TimeObj.GetComponent<TimeCtrl>().GetTime() <= 0 && !debugFlags )
                    {
                        SceneManager.LoadScene("Result");
                    }
                    break;
                }

            case Phase.GAME_PAHSE_STAR:
                {
                    if (TimeObj.GetComponent<TimeCtrl>().GetTime() <= 0 && !debugFlags )
                    {
                        SceneManager.LoadScene("Result");
                    }
                    break;
                }
        }

        //デバッグ用
        if( debugFlags && Input.GetKeyUp( KeyCode.O ) )
        {
            transitionObj.GetComponent<Transition>().StartTransition("Result");
        }
        
        // HACK: OnGUIデバッグ時On・Off処理
        //       もっといい方法がありそうだけど現状これで
        IsOnGUIEnable = isOnGUIEnable;
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

            case Phase.GAME_PAHSE_STAR:
                PhaseStarStart();
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

    void PhaseStarStart()
    {
        CityObj.SetActive(false);
        StarObj.SetActive(true);
        SpawnManagerObj.SetActive(false);
        starSpawnManagerObj.SetActive(true);
        starSpawnManagerObj.GetComponent<StarSpawnManager>().Init();
        //PlayerObj.transform.position = new Vector3(250.0f, 290.0f, -300.0f);
        //PlayerObj.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        PlayerObj.GetComponent<Player>().StarPhaseInit();
        mainCameraObj.GetComponent<LovePCameraController>().enabled = false;
        mainCameraObj.GetComponent<StarCameraController>().enabled = true;
        MiniMapObj.GetComponent<MiniMap>().enabled = false;
        MiniMapObj.GetComponent<StarMiniMap>().enabled = true;
        skyboxManagerObj.GetComponent<SkyboxManager>().SetStarSkyBox();
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
        
        mainCameraObj = Create( mainCameraPrefab );
        guiCameraObj = Create( guiCameraPrefab );
        SpawnManagerObj = Create( SpawnManagerPrefab );
        starSpawnManagerObj = Create(starSpawnPrefab);
        MiniMapObj = Create( MiniMapPrefab );
        effectManagerObj = Create( effectManagerPrefab );
        soundManagerObj = Create( soundManagerPrefab );
        PlayerObj = Create( PlayerPrefab );
        skyboxManagerObj = Create( skyboxManagerPrefab );
        transitionObj = Create(transitionPrefab);

        // HACK: 直接生成したもの以外で保持したいオブジェクトを取得
        //       直接パスを記述。後に変更したほうがいいか？
        TimeObj = GameObject.Find( "Time" );
    }

}
