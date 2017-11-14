using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    GameObject fadePanel;   // フェードパネル
    public int fadeNum;     // 遷移先の番号

    //サウンド用/////////////////////////////
    public SoundController.Sounds titleSoundType;
    private AudioSource titleAudioS;
    private SoundController titleSoundCtrl;


    void Start()
    {
        fadePanel = GameObject.Find("Fade");   //パネルオブジェクトを取得

        //サウンド用
        titleSoundCtrl = GameObject.Find("SoundManager").GetComponent<SoundController>();
        //オブジェクトについているAudioSourceを取得する
        titleAudioS = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //_____フェード関連_____________
        if (Input.GetKey(KeyCode.Space))
        {// Spaceキーで次のシーン
            fadePanel.GetComponent<Fade>().SetFadeIn(fadeNum);  //遷移先を設定する
            //遷移するときのSE
            titleAudioS.PlayOneShot(titleSoundCtrl.AudioClipCreate(titleSoundType));
        }
        //______________
    }
}
