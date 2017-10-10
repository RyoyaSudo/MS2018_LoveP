using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    //スポーンポイントアタッチ
    public GameObject spawnPointPrefab;

    //スポーンポイントの場所
    public Vector3[] spawnPoint;

    //スポーンポイントの数
    public int spawnNum;

    //スポーンポイントのゲームオブジェクト保存
    SpawnPoint[] spawnPointObject;

	// 初期化
	void Start () 
    {
        //スポーンポイント生成
        spawnPointObject = new SpawnPoint[spawnNum];
        
        for (int i = 0; i < spawnNum; i++)
        {
            SpawnPointCreate(i, spawnPoint[i]);
        }
	}
	
	// 更新
	void Update () 
    {
        if (Input.GetKeyDown("0"))
        {
            HumanCreate(0);
        }
        if (Input.GetKeyDown("1"))
        {
            HumanCreate(1);
        }
        if (Input.GetKeyDown("2"))
        {
            HumanCreate(2);
        }
	}

    /*****************************************************************************
    * 関数名:SpawnPointCreate
    * 引数：num:番号
    * 引数:position:位置
    * 戻り値:0
    * 説明:スポーンポイントを生成
    *****************************************************************************/
    void SpawnPointCreate(int num , Vector3 position)
    {
        //生成
        GameObject SpawnPoint = Instantiate(spawnPointPrefab,                       //ゲームオブジェクト
                                               position,                            //位置
                                               Quaternion.identity) as GameObject;  //回転

        //自分の親を自分にする
        SpawnPoint.transform.parent = transform;

        //表示
        SpawnPoint.name = "Spown"+ num ;

        //生成したブロックを配列に保存
        spawnPointObject[num] = SpawnPoint.GetComponent<SpawnPoint>();
    }

    /*****************************************************************************
    * 関数名:HumanCreate
    * 引数：spawnPointNum:スポーンポイントの番号
    * 戻り値:0
    * 説明:スポーンポイントから人を生成
    *****************************************************************************/
    void HumanCreate(int spawnPointNum)
    {
        spawnPointObject[spawnPointNum].HumanSpawn();
    }
}
