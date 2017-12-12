﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 人クラス
/// </summary>
/// <remarks>
/// 『人』のふるまいの管理等を行いたい
/// </remarks>
public class Human : MonoBehaviour {

    #region 変数宣言
    /// <summary>
    /// 役割の種類列挙値
    /// </summary>
    enum RoleType
    {
        Mob = 0,
        Passenger,
    }

    /// <summary>
    /// 人モデル配列。
    /// Inspector上で設定を忘れないこと！！
    /// </summary>
    [SerializeField] GameObject[] modelArray;

    /// <summary>
    /// 表示モデルタイプ
    /// </summary>
    public enum ModelType
    {
        Unknown = -1,
        Girl = 0,
        Boy,
        TypeMax
    }

    /// <summary>
    /// 現在のモデル種類
    /// 他のクラスで乗客オブジェクトの子になっているモデルを参照する際に利用する。
    /// </summary>
    /// <remarks>
    /// 参照する際利用するのは、humanModelArray[ ( int )CurrentModelType ].nameから得られる文字列。
    /// Unknownの場合検索しないようにしたい
    /// </remarks>
    public ModelType CurrentModelType { get; private set; }

    /// <summary>
    /// 乗客操作用オブジェクト
    /// </summary>
    public PassengerController PassengerControllerObj { get; private set; }

    /// <summary>
    /// モブ人操作用オブジェクト
    /// </summary>
    public MobController MobControllerObj { get; private set; }

    /// <summary>
    /// 表示モデルオブジェクト
    /// </summary>
    public GameObject ModelObj { get; private set; }

    /// <summary>
    /// 状態列挙値
    /// </summary>
    public enum STATETYPE
    {
        CREATE,    // 生成
        READY,     // 待機
        EVADE,     // 回避
        RIDE,      // 乗車
        GETOFF,    // 下車
        TRANSPORT, // 運搬
        RELEASE,   // 解散
        AWAIT,     // 待ち受け
        DESTROY    // 消去
    };

    /// <summary>
    /// 現在状態値
    /// </summary>
    public STATETYPE CurrentStateType { get { return currentStateType; } set { SetState( value ); } }

    /// <summary>
    /// 現在状態値のバッキングフィールド
    /// </summary>
    private STATETYPE currentStateType;

    /// <summary>
    /// 保護用フラグ変数。
    /// trueの時には無闇にDestoroyしないようにすること。
    /// </summary>
    public bool IsProtect { get; set; }

    #endregion

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        // 初期値設定
        CurrentModelType = ModelType.Unknown;
        CurrentStateType = STATETYPE.READY;
        IsProtect = false;

        // 必要なゲームオブジェクト取得
        PassengerControllerObj = gameObject.GetComponent<PassengerController>();
        MobControllerObj = gameObject.GetComponent<MobController>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// モデル生成
    /// </summary>
    /// <param name="type">
    /// 生成する種類
    /// </param>
    public void ModelCreate( ModelType type )
    {
        if( type >= ModelType.TypeMax || type < 0 )
        {
            Debug.LogError( "人生成時に不定なタイプが指定されました。" );
        }

        CurrentModelType = type;

        // 生成処理
        GameObject createModel = modelArray[ ( int )CurrentModelType ];

        ModelObj = Instantiate( createModel , transform.position , Quaternion.identity ) as GameObject;
        ModelObj.transform.parent = transform;
        ModelObj.name = createModel.name;
    }

    /// <summary>
    /// 自身の子となっている表示用モデルの当たり判定取得処理。
    /// </summary>
    /// <remarks>
    /// 判定の消去などに利用すると良い。
    /// </remarks>
    public Collider GetHumanModelCollider()
    {
        string childName = modelArray[ ( int )CurrentModelType ].name;
        GameObject obj = transform.Find( childName ).gameObject;
        Collider collider = obj.GetComponent<Collider>();
        return collider;
    }

    /// <summary>
    /// 状態設定処理
    /// </summary>
    /// <remarks>
    /// 基本的には、CurrentStateTypeのsetterで呼び出すようにする
    /// </remarks>
    /// <param name="type">設定値</param>
    private void SetState( STATETYPE type )
    {
        currentStateType = type;

        switch( type )
        {
            case STATETYPE.CREATE:
                break;

            case STATETYPE.READY:
                break;

            case STATETYPE.EVADE:
                break;

            case STATETYPE.RIDE:
                break;

            case STATETYPE.GETOFF:
                break;

            case STATETYPE.TRANSPORT:
                break;

            case STATETYPE.RELEASE:
                break;

            case STATETYPE.AWAIT:
                break;

            case STATETYPE.DESTROY:
                break;

        }

        if( Game.IsDebug )
        {
            Debug.Log( "人の新規状態:" + type );
        }
    }
}