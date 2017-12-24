using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Result : MonoBehaviour
{
    public GameObject transition;
    public GameObject ScoreObj;
    public GameObject cameraObj;
    public GameObject GroundObj;
    public GameObject timeline;
    public GameObject timeCamera;
    public GameObject scoreCamera;
    public GameObject resultGUI;
    public GameObject resultScore;
    public GameObject totalScore;
    public GameObject skyBoxObj;
    public GameObject crossRocketObj;

    public enum State
    {
        STATE_RIDE = 0,
        STATE_ROCKET_CROSS,
        STATE_SCORE
    }

    State state;

    // プレハブ系
    // Hierarchy上から設定する
    [SerializeField] GameObject inputPrefab;

    // オブジェクト系
    // シーン中シーン管理上操作したい場合に保持しておく
    LoveP_Input inputObj;

    private bool one;
    private bool fadeFlag;

    void Start()
    {
        one = false;
        ////サウンド用
        //titleSoundCtrl = GameObject.Find("SoundManager").GetComponent<SoundController>();
        ////オブジェクトについているAudioSourceを取得する
        //titleAudioS = gameObject.GetComponent<AudioSource>();
        InitCreateObjects();
        fadeFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        //_____フェード関連_____________
        if (inputObj.GetButton("Fire1") || Input.GetKeyDown(KeyCode.O))
        {
            if(fadeFlag == false)
            {
                fadeFlag = true;
                // Spaceキーで次のシーン
                // fadePanel.GetComponent<Fade>().SetFadeIn(fadeNum);  //遷移先を設定する
                transition.SetActive(true);
                transition.GetComponent<Transition>().transitionFlag = false;
                transition.GetComponent<Transition>().StartTransition("Title");
                //遷移するときのSE
                //titleAudioS.PlayOneShot(titleSoundCtrl.AudioClipCreate(titleSoundType));
            }
            
        }

        switch( state)
        {
            case State.STATE_SCORE:
                {
                    if (one == true) break;
                    if (transition.GetComponent<Transition>().state == Transition.State.STATE_FADE_OUT)
                    {
                        one = true;
                        timeCamera.SetActive(false);
                        scoreCamera.SetActive(true);
                        resultGUI.SetActive(true);
                        crossRocketObj.SetActive(true);
                        crossRocketObj.GetComponent<CrossRocket>().Play();
                        //resultScore.transform.position += new Vector3(-400, -400, 1);
                        skyBoxObj.GetComponent<SkyboxManager>().SetStarSkyBox();
                    }                 
                    break;
                }
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
        Func<GameObject, GameObject> Create = (GameObject prefabs) =>
     {
         GameObject obj = GameObject.Find(prefabs.name);

         if (obj == null)
         {
             obj = Instantiate(prefabs);
             obj.name = prefabs.name;
         }

         return obj;
     };

        // 各オブジェクトの生成
        inputObj = Create(inputPrefab).GetComponent<LoveP_Input>();
    }

    public void SetStateType(State type)
    {
        state = type;
        switch (state)
        {
            case State.STATE_SCORE:
                {
                    GroundObj.SetActive(false);
                    ScoreObj.SetActive(true);
                    cameraObj.transform.position = new Vector3(50, 0, 0);
                    cameraObj.transform.rotation = Quaternion.identity;
                    
                    //timeline.SetActive(false);
                    break;
                }
        }
    }
}
