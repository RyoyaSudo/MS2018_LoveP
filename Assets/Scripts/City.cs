using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class City : MonoBehaviour {

    // プレハブ系
    // Hierarchy上から設定する
    [SerializeField] GameObject billdingPrefab;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] GameObject vehicleNavMeshPrefab;
    [SerializeField] GameObject shipNavMeshPrefab;
    [SerializeField] GameObject playerSpawnPrefab;

    // オブジェクト系
    // シーン中シーン管理上操作したい場合に保持しておく
    GameObject billdingObj;
    GameObject obstacleObj;
    GameObject vehicleNavMeshObj;
    GameObject shipNavMeshObj;
    GameObject playerSpawnObj;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        // オブジェクト生成処理など
        InitCreateObjects();

        // 親子付け
        billdingObj.transform.parent = transform;
        obstacleObj.transform.parent = transform;
        vehicleNavMeshObj.transform.parent = transform;
        shipNavMeshObj.transform.parent = transform;
        playerSpawnObj.transform.parent = transform;
    }

    /// <summary>
    /// 初期化時オブジェクト生成処理
    /// </summary>
    /// <remarks>
    /// Hierarchy上にプレハブと同名のオブジェクトが無いか検索し、無ければ生成。
    /// ある場合はHierarchy上の物を使用する。
    /// Debug時などはHierarchy上のもののほうが使い勝手が良いため。
    /// </remarks>
    private void InitCreateObjects()
    {
        // HACK: ラムダ式で生成処理を実装
        //       効率が良いのかは分からん
        Func< GameObject , GameObject > Create = ( GameObject prefabs ) =>
        {
            GameObject obj = GameObject.Find( prefabs.name );

            if( obj == null )
            {
                obj = Instantiate( prefabs );
                obj.name = prefabs.name;
            }

            return obj;
        };

        // 各オブジェクトの生成
        billdingObj       = Create( billdingPrefab );
        obstacleObj       = Create( obstaclePrefab );
        vehicleNavMeshObj = Create( vehicleNavMeshPrefab );
        shipNavMeshObj    = Create( shipNavMeshPrefab );
        playerSpawnObj    = Create( playerSpawnPrefab );
    }
}
