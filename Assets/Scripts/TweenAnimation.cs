using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tweenアニメーション基底クラス
/// </summary>
public abstract class TweenAnimation : MonoBehaviour {

    /// <summary>
    /// 再生処理
    /// 各々で再生するものが違うため、派生先に処理を記入
    /// </summary>
    public abstract void Play();

    /// <summary>
    /// 一時停止処理
    /// 上記と同様
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// 停止処理
    /// 上記と同様
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// 初期化処理
    /// 上記と同様
    /// </summary>
    protected abstract void Init();
}
