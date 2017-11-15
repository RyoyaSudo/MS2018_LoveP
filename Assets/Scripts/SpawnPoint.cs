using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    /// <summary>
    /// 乗客の乗車順番
    /// </summary>
    public enum PASSENGER_ORDER
    {
        FIRST=0,     //最初
        DEFOULT      //それ以外
    }

    //人プレファブアタッチ
    public GameObject humanPrefab;

    //人プレファブ
   // GameObject humanPrefab;


    //初期化
    void Start()
    {

    }

    // 更新
    void Update()
    {
        //回転
        transform.Rotate(new Vector3(0, 20, 0) * Time.deltaTime);
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
    public void HumanSpawn(int spawnPointNum ,Human.GROUPTYPE groupType , PASSENGER_ORDER passengerOrder)
    {
        //生成
        GameObject human = Instantiate(humanPrefab,                                 //ゲームオブジェクト
                                               this.transform.position,             //位置
                                               Quaternion.identity) as GameObject;  //回転

        //モデル生成
        human.GetComponent<Human>().ModelCreate(groupType);

        //グループタイプを設定
        human.GetComponent<Human>().groupType = groupType;

        //スポーンの場所を設定
        human.GetComponent<Human>().spawnPlace = spawnPointNum;

        //乗客の乗車順番
        human.GetComponent<Human>().pasengerOrder = passengerOrder;
    }
}
