using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //画面遷移を可能にする

public class Tutorial : MonoBehaviour
{
    public GameObject transition;
    public GameObject nowObj;
    private AsyncOperation async;
    ////サウンド用/////////////////////////////
    //public SoundController.Sounds titleSoundType;
    //private AudioSource titleAudioS;
    //private SoundController titleSoundCtrl;


    void Start()
    {
        ////サウンド用
        //titleSoundCtrl = GameObject.Find("SoundManager").GetComponent<SoundController>();
        ////オブジェクトについているAudioSourceを取得する
        //titleAudioS = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //_____フェード関連_____________
        if (Input.GetKeyDown(KeyCode.O))
        {
            ///<summary>
            ///トランジションをStartCoroutine
            ///GameSceneの非同期読み込み開始
            ///GameSceneのAwakeが呼ばれる
            ///
            ///</summary>

            // Spaceキーで次のシーン
            // fadePanel.GetComponent<Fade>().SetFadeIn(fadeNum);  //遷移先を設定する
            // transition.SetActive(true);
            StartCoroutine(Nowloading());
            transition.GetComponent<Transition>().StartTransition(null);
            //SceneManager.LoadScene("Game");
            
            //StartCoroutine(trans());
            
            //遷移するときのSE
            //titleAudioS.PlayOneShot(titleSoundCtrl.AudioClipCreate(titleSoundType));
        }
        //______________
    }

    IEnumerator trans()
    {
        //async = SceneManager.LoadSceneAsync("Game");
        yield return null;
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
}
