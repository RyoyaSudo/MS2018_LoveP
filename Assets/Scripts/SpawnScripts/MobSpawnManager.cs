using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnManager : MonoBehaviour {

    [SerializeField] string spawnPointListPath;
    private GameObject spawnPointListObj;

    /// <summary>
    /// スポーンポイント数。
    /// 初期化時にスポーンポイントの場所を管理する配列から数を取得。
    /// </summary>
    public int SpawnNum { get; private set; }

    //スポーンポイントのゲームオブジェクト保存
    List< SpawnPoint > spawnPointObject;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        SpawnNum = 0;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start () {
        // リストに登録されたスポーンポイントオブジェクトをすべて取得して蓄えておく
        spawnPointListObj = GameObject.Find( spawnPointListPath );
        spawnPointObject = new List<SpawnPoint>();

        foreach( Transform child in spawnPointListObj.transform )
        {
            SpawnPoint point = child.gameObject.GetComponent<SpawnPoint>();
            spawnPointObject.Add( point );
            SpawnNum++;
        }

        if( SpawnNum == 0 )
        {
            Debug.LogError( "スポーンポイント取得失敗" );
        }

        // HACK: モブ生成処理
        //       現状は開始したらとりあえず生成してみる。
        //       あとから何とかしたい
        for( int i = 0 ; i < SpawnNum ; i++ )
        {
            spawnPointObject[ i ].MobSpawn( 0 );
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
