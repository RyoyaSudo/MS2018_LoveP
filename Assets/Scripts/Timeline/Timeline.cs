using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

public abstract class Timeline : MonoBehaviour
{
    //PlayableDirector
    private PlayableDirector director;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        Bind();
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
    public void BindTrack( GameObject obj , string path )
    {
        //PlayableDirector取得
        director = GetComponent<PlayableDirector>();

        //Timelineからパスのトラックへの参照を取得して
        var binding = director.playableAsset.outputs.First(c => c.streamName == path);

        //そこにオブジェクトをバインドする
        director.SetGenericBinding( binding.sourceObject , obj );
    }

    /// <summary>
    /// 各トラックにオブジェクトをバインドする処理。
    /// 必ず派生先初期化時に呼び出すこと！
    /// </summary>
    public abstract void Bind();

    /// <summary>
    /// 開始処理
    /// </summary>
    public void Play()
    {
        director.Play();
    }

    /// <summary>
    /// 停止処理
    /// </summary>
    public void Stop()
    {
        director.Stop();
    }

    /// <summary>
    /// 一時停止処理
    /// </summary>
    public void Pause()
    {
        director.Pause();
    }
}
