﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPhaseMove : MonoBehaviour {

    float moveRadY;

    Rigidbody rb;

    [SerializeField] float speed;

    Vector3 upV;

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
    /// 独自デバイス入力処理オブジェクト
    /// </summary>
    private LoveP_Input inputObj;
    [SerializeField] string inputObjPath;

    /// <summary>
    /// 星フェイズ開始位置。
    /// 星オブジェクトに付属しているスポーンポイントを取得する
    /// </summary>
    Vector3 playerSpawnPoint;
    [SerializeField] string playerSpawnPointObjPath;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        inputObj = null;
        IsEnable = false;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start () {
        speedMax = 60.0f;
        speed = 1800f;
        moveRadY = 0.0f;

        upV = Vector3.zero;

        // シーン内から必要なオブジェクトを取得
        earthObj = GameObject.Find( earthObjPath );
        inputObj = GameObject.Find( inputObjPath ).GetComponent<LoveP_Input>();
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
        // 面の法線方向を取得
        int layerMask = LayerMask.GetMask( new string[] { "Earth" } );
        RaycastHit hitInfo;

        if( Physics.Raycast( transform.position + ( transform.up * 0.1f ) , -transform.up , out hitInfo , 100.0f , layerMask ) )
        {
            // 接した場合
            upV = hitInfo.normal;
        }
        else
        {
            upV = Vector3.up;
        }

        // 進行方向を取得
        Vector3 afterForwardV = Vector3.ProjectOnPlane( transform.forward , upV );

        Quaternion forwardQ = Quaternion.LookRotation( afterForwardV , upV );

        float moveV = inputObj.GetAxis("Vertical");
        float moveH = inputObj.GetAxis("Horizontal");
        Vector3 gravityVec = -upV;
        gravityVec.Normalize();
        rb.AddForce( 9.8f * gravityVec * ( 60.0f * Time.deltaTime ) , ForceMode.Acceleration );
        transform.up = -gravityVec.normalized;

        Vector3 direction = new Vector3(moveH, 0.0f, moveV);

        //Debug.Log( "Horizontal:" + moveH );
        Vector3 axis = transform.up;// 回転軸
                                    //float angle = 90f * Time.deltaTime; // 回転の角度

        //this.transform.rotation = q * this.transform.rotation; // クォータニオンで回転させる
        moveRadY = moveH * 180.0f * Time.deltaTime;
        //transform.rotation = Quaternion.Euler(0, moveRadY, 0);
        Quaternion q = Quaternion.AngleAxis(moveRadY, axis); // 軸axisの周りにangle回転させるクォータニオン
        this.transform.rotation = q * forwardQ; // クォータニオンで回転させる
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

    public void StarPhaseStart()
    {
        // シーン内から必要なオブジェクトを取得
        earthObj = GameObject.Find(earthObjPath);
    }

    /// <summary>
    /// フェイズ突入時の初期化処理。星フェイズに入る場合必ず呼び出すこと！
    /// </summary>
    public void Initialize()
    {
        // シーン内から必要なオブジェクトを取得
        earthObj = GameObject.Find( earthObjPath );
        playerSpawnPoint = GameObject.Find( playerSpawnPointObjPath ).GetComponent<Transform>().position;

        // HACK: 初期姿勢を決める
        // 星オブジェクトの中心に向かうベクトルを生成し、プレイヤーから直近の面の法線方向を取得
        int layerMask = LayerMask.GetMask( new string[] { "Earth" } );
        RaycastHit hitInfo;
        Vector3 castPos = playerSpawnPoint;
        Vector3 castDir = Vector3.Normalize( earthObj.transform.position - playerSpawnPoint );

        if( Physics.Raycast( castPos , castDir , out hitInfo , 1000.0f , layerMask ) )
        {
            // 接した場合
            upV = hitInfo.normal;
        }
        else
        {
            // 接しない場合。基本は無いはず。あればバグなため修正
            upV = Vector3.up;
            Debug.LogError( "星移動の初期化ミス。適切な姿勢が設定されていない可能性有り。" );
        }

        // 進行方向を取得
        Vector3 afterForwardV = Vector3.ProjectOnPlane( transform.forward , upV );

        Quaternion forwardQ = Quaternion.LookRotation( afterForwardV , upV );

        // 姿勢決定
        transform.rotation = forwardQ;
        transform.position = playerSpawnPoint;
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
            //str = "上方向:" + upV + "\n位置:" + transform.position + "\n姿勢:" + transform.rotation.eulerAngles;

            GUI.Label( new Rect( 0 , 200 , 800 , 600 ) , str , guiStyle );
        }
    }
}
