using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class GameEndPlayableBehaviour : PlayableBehaviour
{
    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
    }

    // タイムライン停止実行
    public override void OnGraphStop(Playable playable)
    {

    }

    // PlayableTrack再生実行
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
    }

    // PlayableTrack停止時実行
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
    }

    // PlayableTrack再生時毎フレーム実行
    public override void PrepareFrame(Playable playable, FrameData info)
    {
    }
}
