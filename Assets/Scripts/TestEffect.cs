using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffect : MonoBehaviour {

    //テスト用スクリプト/////////////////

    //エフェクト用/////////////////////////////
    private EffectController test;
    public EffectController.Effects type;

    //サウンド用/////////////////////////////
    private AudioSource testAudioSource;
    private SoundController testMusic;
    public SoundController.Sounds soundType;

    private ParticleSystem testEffect;
    // Use this for initialization
    void Start()
    {
        //外部のオブジェクトのスクリプトから関数を使用したい時にFindする。
        //"スクリプトがついているクラス名" + <スクリプトのクラス名> 
        test = GameObject.Find("EffectManager").GetComponent<EffectController>();
        testMusic = GameObject.Find("SoundManager").GetComponent<SoundController>();

        //オブジェクトについているAudioSourceを取得する
        testAudioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            testEffect = test.EffectCreate(type, gameObject.transform);  //エフェクトを生成
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            //一回だけ再生 引数にそのまま入れちゃう
            testAudioSource.PlayOneShot(testMusic.AudioClipCreate(soundType));
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            testAudioSource.clip = testMusic.AudioClipCreate(SoundController.Sounds.TEST_SOUND1); 
            //ずっと流したい場合はAudioSourceのClipの中に入れる必要がある
            testAudioSource.Play();
        }
    }
}
