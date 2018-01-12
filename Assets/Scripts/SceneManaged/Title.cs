using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Title : MonoBehaviour {

    // プレハブ系
    // Hierarchy上から設定する
    [SerializeField] GameObject mainCameraPrefab;
    [SerializeField] GameObject guiCameraPrefab;
    [SerializeField] GameObject soundManagerPrefab;
    [SerializeField] GameObject inputPrefab;

    // オブジェクト系
    // シーン中シーン管理上操作したい場合に保持しておく
    GameObject mainCameraObj;
    GameObject guiCameraObj;
    GameObject soundManagerObj;

    LoveP_Input inputObj;

    GameObject fadePanel;   // フェードパネル
    public int fadeNum;     // 遷移先の番号

    // トランジションオブジェクト
    GameObject transition;

    //サウンド用/////////////////////////////
    public SoundController.SoundsSeType titleSoundType;
    private AudioSource titleAudioS;
    private SoundController titleSoundCtrl;
    private AudioSource titleBGM;

    public GameObject nowloadingObj;
    private bool transFlag;

    [SerializeField] bool isDemo = false;

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
    void Start()
    {
        //サウンド用
        titleSoundCtrl = GameObject.Find("SoundManager").GetComponent<SoundController>();
        //オブジェクトについているAudioSourceを取得する
        titleAudioS = gameObject.GetComponent<AudioSource>();

        transition = GameObject.Find( "GUI Camera/transition" );
        transFlag = false;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        //_____フェード関連_____________
        if( inputObj.GetButton( "Fire1" ) || Input.GetKeyDown( KeyCode.O ) || Input.GetButton( "Fire1" ) )
        {
            if( transFlag == false)
            {
                transFlag = true;
                // fadePanel.GetComponent<Fade>().SetFadeIn(fadeNum);  //遷移先を設定する
                transition.SetActive(true);
                transition.GetComponent<Transition>().StartHalfTransition(null);
                nowloadingObj.SetActive(true);

                if( isDemo )
                {
                    nowloadingObj.GetComponent<Nowloading>().LoadingStart( "Result" );
                }
                else
                {
                    nowloadingObj.GetComponent<Nowloading>().LoadingStart( "Game" );
                }

                //遷移するときのSE
                titleSoundCtrl.PlayOneShot( titleSoundType , titleAudioS );
            }
 
        }

        if( Input.GetKeyDown( KeyCode.R ) )
        {
            transFlag = true;
            // fadePanel.GetComponent<Fade>().SetFadeIn(fadeNum);  //遷移先を設定する
            transition.SetActive( true );
            transition.GetComponent<Transition>().StartHalfTransition( null );
            nowloadingObj.SetActive( true );
            nowloadingObj.GetComponent<Nowloading>().LoadingStart( "Result" );
        }
        //______________
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
        // ラムダ式で生成処理を実装
        // 効率が良いのかは分からん
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
        mainCameraObj       = Create( mainCameraPrefab );
        guiCameraObj        = Create( guiCameraPrefab );
        soundManagerObj     = Create( soundManagerPrefab );
        inputObj            = Create( inputPrefab ).GetComponent<LoveP_Input>();
    }
}
