using System.Collections;
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
    public enum RoleType
    {
        Unknown = -1,
        Mob = 0,
        Passenger,
    }

    public RoleType Role { get { return role; } set { SetRoleType( value ); } }
    private RoleType role;

    /// <summary>
    /// 人モデル配列。
    /// Inspector上で設定を忘れないこと！！
    /// </summary>
    [SerializeField] GameObject[] modelArray = new GameObject[(int)ModelType.TypeMax];

    /// <summary>
    /// 表示モデルタイプ
    /// </summary>
    public enum ModelType
    {
        Unknown = -1,

        PlayerSt = 0,
        Player_Girl = PlayerSt,
        Player_Boy,
        PlayerEnd = Player_Boy,

        LoversSt,
        Lover_Girl = LoversSt,
        Lover_Boy,
        LoversEnd = Lover_Boy,

        FamilySt,
        Family_Child = FamilySt,
        Family_Mother,
        Family_Father,
        FamilyEnd = Family_Father,

        FriendsSt,
        Friends_GirlA = FriendsSt,
        Friends_GirlB,
        Friends_ManA,
        Friends_ManB,
        Friends_ManC,
        FriendsEnd = Friends_ManC,

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
    /// マップアイコン用オブジェクト
    /// </summary>
    public GameObject MapIconObj { get { return mapIconObj; } }
    [SerializeField] GameObject mapIconObj;

    /// <summary>
    /// 乗車エリアオブジェクト
    /// </summary>
    public GameObject RideAreaObj { get { return rideAreaObj; } }
    [SerializeField] GameObject rideAreaObj;

    [SerializeField] GameObject cursorObj;

    /// <summary>
    /// 親オブジェクトたち
    /// </summary>
    private GameObject mobsParent;
    public string mobsParentPath;
    private GameObject passengerParent;
    public string passengerParentPath;

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
        CurrentStateType = STATETYPE.CREATE;
        IsProtect = false;
        role = RoleType.Unknown;

        // 必要なゲームオブジェクト取得
        PassengerControllerObj = gameObject.GetComponent<PassengerController>();
        MobControllerObj = gameObject.GetComponent<MobController>();

        mobsParent = GameObject.Find( mobsParentPath );
        passengerParent = GameObject.Find( passengerParentPath );
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
            Debug.LogError( "人生成時に不定なタイプが指定されました。\nタイプID:" + ( int )type + "\n名前:" + type );
        }

        CurrentModelType = type;

        // 生成処理
        GameObject createModel = modelArray[ ( int )CurrentModelType ];

        ModelObj = Instantiate( createModel , transform.position , Quaternion.identity ) as GameObject;
        ModelObj.transform.parent = transform;
        ModelObj.name = createModel.name;

        // モデルの名前を管理オブジェクトとなる親につける
        ModelObj.transform.parent.name = createModel.name;
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

        // HACK: 乗客側にも状態をセット
        //       もっと良い処理順があったかも
        //       2017/12/13現在はここで処理
        if( PassengerControllerObj != null )
        {
            if( PassengerControllerObj.IsEnable )
            {
                PassengerControllerObj.SetStateType( type );
            }
        }

        if( Game.IsDebug )
        {
            //Debug.Log( "人の新規状態:" + type );
        }
    }

    /// <summary>
    /// 役割の設定処理
    /// </summary>
    /// <param name="type"></param>
    private void SetRoleType( RoleType type )
    {
        role = type;

        // 設定する役割に応じてコンポーネント切り替え
        switch( type )
        {
            case RoleType.Unknown:
                Debug.LogWarning( "不定なタイプが人オブジェクトに指定されました。" );
                break;

            case RoleType.Mob:
                MobControllerObj.IsEnable = true;
                MobControllerObj.enabled = true;

                RideAreaObj.SetActive( false );
                MapIconObj.SetActive( false );
                cursorObj.SetActive( false );

                PassengerControllerObj.IsEnable = false;
                PassengerControllerObj.enabled = false;

                gameObject.transform.parent = mobsParent.transform;

                gameObject.tag = "Mob";
                break;

            case RoleType.Passenger:
                PassengerControllerObj.IsEnable = true;
                PassengerControllerObj.enabled = true;

                RideAreaObj.SetActive( true );
                MapIconObj.SetActive( true );
                cursorObj.SetActive( true );

                MobControllerObj.IsEnable = false;
                MobControllerObj.enabled = false;

                gameObject.transform.parent = passengerParent.transform;

                gameObject.tag = "Passenger";
                break;

            default:
                break;
        }
    }
}
