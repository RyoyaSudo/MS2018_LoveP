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

    Human human;   // 親オブジェクトの乗客
    GameObject mapCamera;   // マップカメラ
    [SerializeField] Material mat;

    [SerializeField] Texture[] groupTexArray;
    [SerializeField] Texture[] charTexArray;

    Player playerObj;
    [SerializeField] string playerObjPath;

    // Use this for initialization
    void Start () {
        // 親オブジェクトを取得
        // 2017/12/01 数藤
        //   オブジェクト取得方法をrootからparentに変更
        human = transform.parent.gameObject.GetComponent<Human>();
        playerObj = GameObject.Find( playerObjPath ).GetComponent<Player>();

        // マップカメラ
        mapCamera = GameObject.Find("MapCamera");

        //mat.SetTexture( "_MainTex" , tex );
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

        // テクスチャの切り替え
        if( playerObj.StateParam == Player.State.PLAYER_STATE_TAKE || playerObj.StateParam == Player.State.PLAYER_STATE_TAKE_READY )
        {
            // 顔を出す
            int idx = human.CurrentModelType - Human.ModelType.LoversSt;
            mat.SetTexture( "_MainTex" , charTexArray[ idx ] );
        }
        else
        {
            // グループのアイコン出す
            int idx = (int)human.PassengerControllerObj.groupType;
            mat.SetTexture( "_MainTex" , groupTexArray[ idx ] );
        }

        // 角度合わせる
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = mapCamera.transform.rotation.eulerAngles.y + 180.0f;
        transform.rotation = Quaternion.Euler( euler );

        Debug.Log( "Map:" + mapCamera.transform.rotation.eulerAngles );
    }
}
