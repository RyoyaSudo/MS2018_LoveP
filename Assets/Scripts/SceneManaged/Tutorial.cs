using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {
    public GameObject transition;

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
        if (Input.GetKey(KeyCode.O))
        {
            // Spaceキーで次のシーン
            // fadePanel.GetComponent<Fade>().SetFadeIn(fadeNum);  //遷移先を設定する
            transition.SetActive(true);
            transition.GetComponent<Transition>().StartTransition("Game");
            //遷移するときのSE
            //titleAudioS.PlayOneShot(titleSoundCtrl.AudioClipCreate(titleSoundType));
        }
        //______________
    }
}
