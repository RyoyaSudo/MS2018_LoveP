using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //画面遷移を可能にする
using System;

public class Tutorial : MonoBehaviour
{
    public GameObject transition;
    public GameObject nowObj;

    // プレハブ系
    // Hierarchy上から設定する
    [SerializeField] GameObject inputPrefab;

    // オブジェクト系
    // シーン中シーン管理上操作したい場合に保持しておく
    LoveP_Input inputObj;

    private AsyncOperation async;
    private bool loadingFlag;
    ////サウンド用/////////////////////////////
    //public SoundController.Sounds titleSoundType;
    //private AudioSource titleAudioS;
    //private SoundController titleSoundCtrl;


    void Start()
    {
        loadingFlag = false;
        ////サウンド用
        //titleSoundCtrl = GameObject.Find("SoundManager").GetComponent<SoundController>();
        ////オブジェクトについているAudioSourceを取得する
        //titleAudioS = gameObject.GetComponent<AudioSource>();
    }

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        InitCreateObjects();
    }

    // Update is called once per frame
    void Update()
    {
        //_____フェード関連_____________
        if( inputObj.GetButton( "Fire1" ) || Input.GetKey( KeyCode.O ) )
        {
            if (loadingFlag == true) return;
            loadingFlag = true;
            StartCoroutine(Nowloading());
            transition.GetComponent<Transition>().StartTransition(null);
            //SceneManager.LoadScene("Game");
            
            //StartCoroutine(trans());
            
            //遷移するときのSE
            //titleAudioS.PlayOneShot(titleSoundCtrl.AudioClipCreate(titleSoundType));
        }
        //______________
    }

    IEnumerator Nowloading()
    {
        var async = SceneManager.LoadSceneAsync("Game");
        nowObj.SetActive(true);

        while (async.progress < 0.9f)
        {
            Debug.Log(async.progress);
            //nowObj.transform.localScale = new Vector3(async.progress, 1.0f, 1.0f);
            nowObj.transform.Rotate(new Vector3(0.0f, 0.0f, 6.0f));
            yield return null;
        }
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
        inputObj = Create( inputPrefab ).GetComponent<LoveP_Input>();
    }
}
