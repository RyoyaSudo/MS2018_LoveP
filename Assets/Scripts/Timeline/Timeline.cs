using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

public class Timeline : MonoBehaviour
{
    //PlayableDirector
    private PlayableDirector director;

    // 初期化
    void Start()
    {
    }

    // 更新
    void Update()
    {

    }

    /// <summary>
    /// Trackにバインド
    /// </summary>
    /// <param name="obj">
    /// バインドするオブジェクト
    /// </param>
    /// <param name="path">
    /// Track名
    /// </param>
    public void BindTrack(GameObject obj, string path)
    {
        //PlayableDirector取得
        director = GetComponent<PlayableDirector>();

        //Timelineからパスのトラックへの参照を取得して
        var binding = director.playableAsset.outputs.First(c => c.streamName == path);

        //そこにオブジェクトをバインドする
        director.SetGenericBinding(binding.sourceObject, obj);
    }

    /// <summary>
    /// タイムライン開始
    /// </summary>
    public void TimelineStart ()
    {
        director.Play();
    }
}
