using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class ResultPlayableBehaviour : PlayableBehaviour
{
    //private Player playerObj;               //プレイヤーオブジェ
    //public string playerPath;               //プレイヤーパス

    //private GameObject vc3Obj;              //バーチャルカメラオブジェクト3

    //private VirtualCameraManager virtualCameraManager;      //バーチャルカメラマネージャ
    //public string virtualCameraManagerPath;                 //バーチャルカメラマネージャパス

    //private TimelineManager timelineManager;    //タイムラインマネージャー
    //public string timelineManagerPath;          //タイムラインマネージャーパス

    //public GameObject readyDolly;                             //レディードリーオブジェ
    //private Cinemachine.CinemachineTrackedDolly trackDolly;   //トラックドリー
    //public float dollyIntervalTime;    //ドリーの刻む時間
    //public float dollyIntervalPos;     //ドリーの刻む位置
    //private float pathPosCnt;          //パスの位置カウント

    private GameObject rocketObj;
    public string rocketPath;

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
