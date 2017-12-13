using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    /// <summary>
    /// 乗客の乗車順番
    /// </summary>
    public enum PASSENGER_ORDER
    {
        FIRST = 0,       //最初
        DEFOULT          //それ以外
    }

    /// <summary>
    /// 乗客プレハブ
    /// </summary>
    [SerializeField] GameObject humanPrefab;

    /// <summary>
    /// 生成した乗客の親オブジェクト
    /// </summary>
    private Transform passengerParentObj;
    [SerializeField] string passengerParentObjPath;

    /// <summary>
    /// 生成したモブキャラの親オブジェクト
    /// </summary>
    private Transform mobParentObj;
    [SerializeField] string mobParentObjPath;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        passengerParentObj = GameObject.Find( passengerParentObjPath ).GetComponent<Transform>();
        mobParentObj = GameObject.Find( mobParentObjPath ).GetComponent<Transform>();
    }

    /// <summary>
    /// 人を乗客として生成する処理
    /// </summary>
    /// <param name="spawnPointNum">
    /// スポーンポイントの番号
    /// </param>
    /// <param name="groupType">
    /// 人のグループタイプ
    /// </param>
    /// <param name="passengerOrder">
    /// 乗客の乗車順番
    /// </param>
    public void PassengerSpawn( int spawnPointNum , PassengerController.GROUPTYPE groupType , PASSENGER_ORDER passengerOrder )
    {
        // 生成
        Human human = Instantiate( humanPrefab , transform.position , Quaternion.identity ).GetComponent<Human>();

        // HACK: 人モデル生成
        //       ここでいい具合にランダムなどやりたい
        Human.ModelType createType = Human.ModelType.Unknown;

        switch( groupType )
        {
            case PassengerController.GROUPTYPE.PEAR:
                createType = Human.ModelType.Girl;
                break;

            case PassengerController.GROUPTYPE.SMAlLL:
                createType = Human.ModelType.Boy;
                break;

            case PassengerController.GROUPTYPE.BIG:
                createType = ( Human.ModelType )( Random.Range( 0 , 1 ) );
                break;

            default:
                break;
        }

        human.ModelCreate( createType );

        // 乗客設定
        human.PassengerControllerObj.groupType = groupType;            //グループタイプを設定
        human.PassengerControllerObj.spawnPlace = spawnPointNum;       //スポーンの場所を設定
        human.PassengerControllerObj.pasengerOrder = passengerOrder;   //乗客の乗車順番
        human.PassengerControllerObj.IsEnable = true;

        // 子にする
        human.gameObject.transform.parent = passengerParentObj;
    }

    /// <summary>
    /// モブ生成処理
    /// </summary>
    /// <param name="spawnPointNum">生成スポーンポイント番号</param>
    /// <param name="modelType">生成する見た目の種類。指定する場合はHuman.ModelType列挙型の数値を利用。</param>
    public void MobSpawn( int spawnPointNum , int modelType = -1 )
    {
        // 生成
        Human human = Instantiate( humanPrefab , transform.position , Quaternion.identity ).GetComponent<Human>();

        // HACK: 人モデル生成
        //       ここでいい具合にランダムなどやりたい
        Human.ModelType createType = Human.ModelType.Unknown;

        if( modelType == -1 )
        {
            createType = ( Human.ModelType )Random.Range( 0 , ( int )( Human.ModelType.TypeMax - 1 ) );
        }
        else
        {
            createType = ( Human.ModelType )modelType;
        }

        human.ModelCreate( createType );

        // HACK: モブパラメータ設定
        //       モブパラメータ追加したらここで設定してください
        human.MobControllerObj.IsEnable = true;

        // 子にする
        human.gameObject.transform.parent = mobParentObj;
    }
}
