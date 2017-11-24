﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPhaseMove : MonoBehaviour {

    float moveRadY;

    Rigidbody rb;

    [SerializeField] float speed;

    /// <summary>
    /// 星フェイズのフィールドオブジェクト。
    /// 引力計算などに情報が必要なため。
    /// </summary>
    private GameObject earthObj;
    public string earthObjPath;

    /// <summary>
    /// 最高速
    /// </summary>
    public float speedMax;

    /// <summary>
    /// ターボレシオ
    /// </summary>
    public float turboRatio;

    /// <summary>
    /// 移動処理有効化フラグ。Player.csで制御してもらう。
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start () {
        speedMax = 60.0f;
        speed = 1800f;
        moveRadY = 0.0f;

        // シーン内から必要なオブジェクトを取得
        earthObj = GameObject.Find( earthObjPath );
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update () {
        if( IsEnable )
        {
            StarMove();
        }
    }

    /// <summary>
    /// 星移動処理
    /// </summary>
    void StarMove()
    {
        float moveV = Input.GetAxis("Vertical");
        float moveH = Input.GetAxis("Horizontal");
        Vector3 gravityVec = earthObj.transform.position - transform.position;
        gravityVec.Normalize();
        rb.AddForce( 9.8f * gravityVec * ( 60.0f * Time.deltaTime ) , ForceMode.Acceleration );
        transform.up = -gravityVec.normalized;

        Vector3 direction = new Vector3(moveH, 0.0f, moveV);

        //Debug.Log( "Horizontal:" + moveH );
        Vector3 axis = transform.up;// 回転軸
                                    //float angle = 90f * Time.deltaTime; // 回転の角度

        //this.transform.rotation = q * this.transform.rotation; // クォータニオンで回転させる
        moveRadY += moveH * 180.0f * Time.deltaTime;
        //transform.rotation = Quaternion.Euler(0, moveRadY, 0);
        Quaternion q = Quaternion.AngleAxis(moveRadY, axis); // 軸axisの周りにangle回転させるクォータニオン
        this.transform.rotation = q * this.transform.rotation; // クォータニオンで回転させる
        if( Mathf.Abs( moveH ) > 0.2f )
        {

        }

        Vector3 force = transform.forward * speed;

        // プッシュ動作
        if( Input.GetKey( KeyCode.Space ) )
        {
            force = rb.velocity * 0.0f;
            //rb.velocity = rb.velocity * 0.99f;

            if( rb.velocity.magnitude < 2 )
            {
                rb.velocity *= 0;
            }

        }

        // プッシュ解放した後のダッシュ
        if( Input.GetKeyUp( KeyCode.Space ) )
        {
            rb.velocity = new Vector3( 0.0f , 0.0f , 0.0f );
            //force *= ( 30.0f * rb.mass );
            rb.AddForce( force * turboRatio * Time.deltaTime , ForceMode.VelocityChange );
        }

        // 今回の速度加算
        rb.AddForce( force * Time.deltaTime , ForceMode.Acceleration );

        if( rb.velocity.magnitude > speedMax )
        {
            rb.velocity = rb.velocity.normalized * speedMax;
        }
    }

    /// <summary>
    /// OnGUI処理
    /// 主にデバッグ情報を出す
    /// </summary>
    private void OnGUI()
    {
        if( Game.IsOnGUIEnable )
        {
            GUIStyleState styleState;
            styleState = new GUIStyleState();
            styleState.textColor = Color.white;

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 48;
            guiStyle.normal = styleState;

            string str = "";
            //str = "速度ベクトル:" + velocityVec + "\n速度量:" + velocityVec.magnitude + "\nフレーム間速度:" + velocity;

            GUI.Label( new Rect( 0 , 200 , 800 , 600 ) , str , guiStyle );
        }
    }
}