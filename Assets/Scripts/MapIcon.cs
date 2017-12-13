﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIcon : MonoBehaviour {

    public enum TYPE
    {
        TYPE_PLAYER = 0,
        TYPE_GUEST,
        TYPE_FAR_GUEST
    }
    public TYPE type;

    GameObject human;   // 親オブジェクトの乗客
    GameObject mapCamera;   // マップカメラ

	// Use this for initialization
	void Start () {
        // 親オブジェクトを取得
        // 2017/12/01 数藤
        //   オブジェクト取得方法をrootからparentに変更
        human = transform.parent.gameObject;

        // マップカメラ
        mapCamera = GameObject.Find("MapCamera");
    }
	
	// Update is called once per frame
	void Update () {
        int state = ( int )human.GetComponent<Human>().CurrentStateType; // 親オブジェクトの状態を取得
        
        // HACK: マップアイコン消す処理
        //       ガバガバなので後に修正
        if(state != 2 && state != 1 && state != 0)
        {// もし表示する必要のない状態なら消す。
            Destroy(gameObject);
        }
	}
}
