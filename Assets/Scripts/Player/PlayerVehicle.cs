using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの乗り物関連の処理をまとめたクラス
/// </summary>
public class PlayerVehicle : MonoBehaviour {

    #region 変数宣言

    /// <summary>
    /// 乗り物数。
    /// この数をInspectorで宣言し、他の乗り物関連の変数のチェックなどに利用する。
    /// </summary>
    [SerializeField] int vehicleNum;

    /// <summary>
    /// 乗り物種類列挙値
    /// </summary>
    public enum Type
    {
        BIKE = 0,
        CAR,
        BUS,
        AIRPLANE,
        MAX,
    }

    /// <summary>
    /// 現時点の乗り物タイプ
    /// </summary>
    public Type VehicleType { get { return vehicleType; } set { SetVehicle( value ); } }

    /// <summary>
    /// 現時点の乗り物タイプのバッキングフィールド
    /// </summary>
    Type vehicleType;

    /// <summary>
    /// 昔の乗り物のタイプ
    /// </summary>
    Type oldVehicleType;

    /// <summary>
    /// 乗り物モデル
    /// </summary>
    [SerializeField] GameObject[] vehicleModel;

    /// <summary>
    /// 乗り物スコア上限値。
    /// Type列挙値を引数として利用し、データを取得する。
    /// </summary>
    public int[] VehicleScoreLimit { get { return vehicleScoreLimit; } }

    /// <summary>
    /// 乗り物スコア上限値のバッキングフィールド
    /// </summary>
    [SerializeField] int[] vehicleScoreLimit;

    /// <summary>
    /// 乗り物変化に利用
    /// 上記定義値を上回った際に乗り物を別の物に変化させる
    /// </summary>
    public int VehicleScore { get; set; }

    /// <summary>
    /// 乗り物変化フラグ変数
    /// </summary>
    bool isChange;

    Vector3[] originScaleArray;

    /// <summary>
    /// 乗車位置配列
    /// </summary>
    public Transform[] BikeRidePoint { get { return bikeRidePoint; } }
    public Transform[] CarRidePoint { get { return carRidePoint; } }
    public Transform[] BusRidePoint { get { return busRidePoint; } }

    /// <summary>
    /// 乗車位置配列
    /// </summary>
    [SerializeField] Transform[] bikeRidePoint = new Transform[ 1 ];
    [SerializeField] Transform[] carRidePoint = new Transform[ 2 ];
    [SerializeField] Transform[] busRidePoint = new Transform[ 4 ];

    #endregion

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        VehicleType = Type.BIKE;
        oldVehicleType = Type.BIKE;
        VehicleScore = 0;

        vehicleModel[ ( int )vehicleType ].SetActive( true );

        originScaleArray = new Vector3[ vehicleModel.Length ];

        for( int i = 0 ; i < vehicleModel.Length ; i++ )
        {
            originScaleArray[ i ] = vehicleModel[ i ].transform.localScale;
        }

        isChange = false;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        Morphing();
    }

    /// <summary>
    /// 乗り物設定関数
    /// </summary>
    private void SetVehicle( Type setType )
    { 
        // 現在の表示物を消し、新しいものを表示
        vehicleModel[ ( int )vehicleType ].SetActive( false );
        vehicleType = setType;
        vehicleModel[ ( int )vehicleType ].SetActive( true );

        // 各乗り物に応じた処理
        switch( setType )
        {
            case Type.BIKE:
                break;

            case Type.CAR:
                break;

            case Type.BUS:
                break;

            case Type.AIRPLANE:
                break;

            default:
                Debug.LogError( "未確定の乗り物タイプが指定されました。" );
                break;
        }

        // その他共通処理
        isChange = true;
    }

    /// <summary>
    /// 乗り物変化時の演出処理
    /// </summary>
    private void Morphing()
    {
        if( isChange )
        {
            Vector3 scale;

            scale = new Vector3( vehicleModel[ ( int )oldVehicleType ].transform.localScale.x - Time.deltaTime , vehicleModel[ ( int )oldVehicleType ].transform.localScale.y - Time.deltaTime , vehicleModel[ ( int )oldVehicleType ].transform.localScale.z - Time.deltaTime );
            vehicleModel[ ( int )oldVehicleType ].transform.localScale = scale;

            if( vehicleModel[ ( int )oldVehicleType ].transform.localScale.x < 0.0f )
            {
                scale = new Vector3( 0.0f , 0.0f , 0.0f );
                vehicleModel[ ( int )oldVehicleType ].transform.localScale = scale;

                isChange = false;

                scale = new Vector3( 0.0f , 0.0f , 0.0f );
                vehicleModel[ ( int )vehicleType ].transform.localScale = scale;
            }
        }
        else
        {
            Vector3 scale;
            scale = new Vector3( vehicleModel[ ( int )vehicleType ].transform.localScale.x + Time.deltaTime , vehicleModel[ ( int )vehicleType ].transform.localScale.y + Time.deltaTime , vehicleModel[ ( int )vehicleType ].transform.localScale.z + Time.deltaTime );
            vehicleModel[ ( int )vehicleType ].transform.localScale = scale;

            if( vehicleModel[ ( int )vehicleType ].transform.localScale.x > originScaleArray[ ( int )vehicleType ].x )
            {
                vehicleModel[ ( int )vehicleType ].transform.localScale = originScaleArray[ ( int )vehicleType ];
            }
        }
    }

    /// <summary>
    /// 乗り物変化確認処理
    /// </summary>
    /// <returns>変化判定</returns>
    public bool ChangeCheck()
    {
        bool flags = false;

        // 〇変身条件
        //    ～0 : バイク
        //   1～3 : 車
        //   4～7 : 大型車( バス )
        //   8～  : 飛行機
        //if( VehicleScore >= vehicleScoreLimit[ ( int )Type.AIRPLANE ] && vehicleType != Type.AIRPLANE )
        //{
        //    // 飛行機
        //    flags = true;
        //
        //    oldVehicleType = vehicleType;
        //    VehicleType = Type.AIRPLANE;
        //}
        //else 
        if( VehicleScore >= vehicleScoreLimit[ ( int )Type.BUS ] && VehicleScore < vehicleScoreLimit[ ( int )Type.AIRPLANE ] && vehicleType != Type.BUS )
        {
            // 大型車
            flags = true;

            oldVehicleType = vehicleType;
            VehicleType = Type.BUS;
        }
        else if( VehicleScore >= vehicleScoreLimit[ ( int )Type.CAR ] && VehicleScore < vehicleScoreLimit[ ( int )Type.BUS ] && vehicleType != Type.CAR )
        {
            // 車
            flags = true;

            oldVehicleType = vehicleType;
            VehicleType = Type.CAR;
        }
        else if( VehicleScore >= vehicleScoreLimit[ ( int )Type.BIKE ] && VehicleScore < vehicleScoreLimit[ ( int )Type.CAR ] && vehicleType != Type.BIKE )
        {
            // バイク
            flags = true;

            oldVehicleType = vehicleType;
            VehicleType = Type.BIKE;
        }
        else if( VehicleScore < vehicleScoreLimit[ ( int )Type.BIKE ] && vehicleType != Type.BIKE )
        {
            // バイク
            flags = true;

            oldVehicleType = vehicleType;
            VehicleType = Type.BIKE;
        }

        Debug.Log( "乗り物スコア:" + VehicleScore );

        return flags;
    }

    /// <summary>
    /// OnGUI処理
    /// 主にデバッグ情報を出す
    /// </summary>
    private void OnGUI()
    {
        if( Game.IsOnGUIEnable == false )
        {
            return;
        }

        GUIStyleState styleState;
        styleState = new GUIStyleState();
        styleState.textColor = Color.white;

        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 48;
        guiStyle.normal = styleState;

        string str = "";
        //str = "現在乗り物状態:" + VehicleType + "\n乗り物スコア:" + VehicleScore;

        GUI.Label( new Rect( 0 , 200 , 800 , 600 ) , str , guiStyle );
    }
}
