using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

/// <summary>
/// タイムライン管理クラス
/// </summary>
public class TimelineManager : MonoBehaviour {

    private Timeline[] timelineObjArray;

    [SerializeField] Timeline[] timelinePrefabArray;

    // 初期化
    void Start ()
    {

    }
    private void Awake()
    {
        // 生成
        TimelineCreate();

    }

    /// <summary>
    /// タイムライン生成処理d
    /// </summary>
    private void TimelineCreate()
    {
        // 格納先生成
        int timelineNum = timelinePrefabArray.Length;

        timelineObjArray = new Timeline[ timelineNum ];

        // 各々の生成
        for( int i = 0 ; i < timelineNum ; i++ )
        {
            Timeline timeline = Instantiate( timelinePrefabArray[ i ] );

            timeline.transform.parent = transform;           // 検索を容易にするため、自身の子として追加
            timeline.name = timelinePrefabArray[ i ].name;   // 名前を元の物にする
            timelineObjArray[ i ] = timeline;                // 生成物を保存
        }
    }

    /// <summary>
    /// タイムラインオブジェクト取得処理
    /// </summary>
    /// <param name="name">オブジェクト名。対象物のプレハブ名と同様。</param>
    /// <returns>対象のオブジェクト</returns>
    public Timeline Get( string name )
    {
        int num = timelineObjArray.Length;

        for( int i = 0 ; i < num ; i++ )
        {
            Timeline obj = timelineObjArray[ i ];

            if( obj.name == name )
            {
                return obj;
            }
        }

        Debug.LogError( "タイムラインオブジェクト取得に失敗しました。\n名前:" + name );

        return null;
    }
}
