using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

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

    /*****************************************************************************
    * 関数名:HumanSpawn
    * 引数：spawnPointNum:スポーンポイントの番号
    * 引数：gourpType:人のグループタイプ
    * 戻り値:0
    * 説明:人を生成
    *****************************************************************************/
    public void HumanSpawn(int spawnPointNum ,Human.GROUPTYPE groupType )
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
    }
}
