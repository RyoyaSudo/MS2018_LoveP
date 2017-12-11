using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class StarShiftPlayableBehaviour : PlayableBehaviour
{
    public Cinemachine.CinemachineVirtualCamera starShiftvcam1;  //バーチャルカメラ1

    [SerializeField] private GameObject playerObj;               //プレイヤーオブジェ
    public string playerPath;                                    //プレイヤーパス

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //プレイヤーオブジェクト取得
        playerObj = GameObject.Find(playerPath);

        starShiftvcam1.LookAt = playerObj.transform;

        Vector3 pos;
        pos = playerObj.transform.position;
        pos.y += 3.0f;
        starShiftvcam1.transform.position =pos;

        //Quaternion rotation;
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
        Vector3 pos;
        pos = starShiftvcam1.transform.position;
        pos.y += Time.deltaTime*3.0f;
        starShiftvcam1.transform.position = pos;
    }
}
