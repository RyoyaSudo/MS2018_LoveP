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
    public Human PassengerSpawn( int spawnPointNum , PassengerController.GROUPTYPE groupType , PASSENGER_ORDER passengerOrder, int modelType = -1 )
    {
        // 生成
        Human human = Instantiate( humanPrefab , transform.position , Quaternion.identity ).GetComponent<Human>();

        // HACK: 人モデル生成
        //       ここでいい具合にランダムなどやりたい
        Human.ModelType createType = ( Human.ModelType )modelType;

        if( modelType == -1 )
        {
            switch( groupType )
            {
                case PassengerController.GROUPTYPE.Lovers:
                    createType = ( Human.ModelType )Random.Range( ( int )Human.ModelType.LoversSt , ( int )Human.ModelType.LoversEnd );
                    break;

                case PassengerController.GROUPTYPE.Family:
                    createType = ( Human.ModelType )Random.Range( ( int )Human.ModelType.FamilySt , ( int )Human.ModelType.FamilyEnd );
                    break;

                case PassengerController.GROUPTYPE.Friends:
                    createType = ( Human.ModelType )Random.Range( ( int )Human.ModelType.FriendsSt , ( int ) Human.ModelType.FriendsEnd );
                    break;

                default:
                    break;
            }
        }

        human.ModelCreate( createType );

        // HACK: モブパラメータ設定
        //       モブパラメータはHuman.csで自己変更する仕様に変更
        human.Role = Human.RoleType.Passenger;

        // 乗客の外部設定
        human.PassengerControllerObj.groupType = groupType;            //グループタイプを設定
        human.PassengerControllerObj.spawnPlace = spawnPointNum;       //スポーンの場所を設定
        human.PassengerControllerObj.pasengerOrder = passengerOrder;   //乗客の乗車順番

        human.tag = "Passenger";

        return human;
    }

    /// <summary>
    /// モブ生成処理
    /// </summary>
    /// <param name="spawnPointNum">生成スポーンポイント番号</param>
    /// <param name="modelType">生成する見た目の種類。指定する場合はHuman.ModelType列挙型の数値を利用。</param>
    public Human MobSpawn( int spawnPointNum = -1 , int modelType = -1 )
    {
        // 生成
        Human human = Instantiate( humanPrefab , transform.position , Quaternion.identity ).GetComponent<Human>();

        // HACK: 人モデル生成
        //       ここでいい具合にランダムなどやりたい
        Human.ModelType createType = Human.ModelType.Unknown;

        if( modelType == -1 )
        {
            createType = ( Human.ModelType )Random.Range( ( int )Human.ModelType.PlayerEnd , ( int )( Human.ModelType.TypeMax - 1 ) );
        }
        else
        {
            createType = ( Human.ModelType )modelType;
        }

        human.ModelCreate( createType );

        // HACK: モブパラメータ設定
        //       モブパラメータはHuman.csで自己変更する仕様に変更
        human.Role = Human.RoleType.Mob;

        human.tag = "Mob";

        return human;
    }
}
