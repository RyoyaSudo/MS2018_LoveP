using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffect : MonoBehaviour {

    private EffectController test;

    public EffectController.Effects type;
    // Use this for initialization
    void Start()
    {
        //外部のオブジェクトのスクリプトから関数を使用したい時にFindする。
        test = GameObject.Find("EffectManager").GetComponent<EffectController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            test.EffectCreate(type, gameObject.transform);  //エフェクトを生成
        }
    }
}
