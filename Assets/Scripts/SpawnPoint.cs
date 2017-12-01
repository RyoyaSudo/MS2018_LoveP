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
    /// 生成した乗客を子にさせるオブジェクト
    /// </summary>
    private Transform passengersObj;

    [SerializeField] string passengersObjPath;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        passengersObj = GameObject.Find( passengersObjPath ).GetComponent<Transform>();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// 人を生成
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
    public void HumanSpawn( int spawnPointNum , Human.GROUPTYPE groupType , PASSENGER_ORDER passengerOrder )
    {
        //生成
        Human human = Instantiate( humanPrefab , transform.position , Quaternion.identity ).GetComponent<Human>();

        human.ModelCreate( groupType );         //モデル生成
        human.groupType = groupType;            //グループタイプを設定
        human.spawnPlace = spawnPointNum;       //スポーンの場所を設定
        human.pasengerOrder = passengerOrder;   //乗客の乗車順番

        // 子にする
        human.gameObject.transform.parent = passengersObj;
    }
}
