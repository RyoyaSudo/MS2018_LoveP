using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class ReadyPlayableBehaviour : PlayableBehaviour
{
    private GameObject playerObj;       //プレイヤーオブジェ
    public string playerPath;           //プレイヤーパス

    private VirtualCameraManager virtualCameraManager;      //バーチャルカメラマネージャ
    public string virtualCameraManagerPath;                 //バーチャルカメラマネージャパス

    private TimelineManager timelineManager;    //タイムラインマネージャー
    public string timelineManagerPath;          //タイムラインマネージャーパス

    private GameObject vc1Obj;          //バーチャルカメラオブジェクト1
    private GameObject vc2Obj;          //バーチャルカメラオブジェクト2

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //各オブジェクト取得    
        playerObj = GameObject.Find(playerPath);        //プレイヤーオブジェクト
        virtualCameraManager = GameObject.Find(virtualCameraManagerPath).GetComponent<VirtualCameraManager>();//バーチャルカメラマネージャ
        timelineManager = GameObject.Find(timelineManagerPath).GetComponent<TimelineManager>();               //タイムラインマネージャー

        //バーチャルカメラのSetActive ON
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM1, true);
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM2, true);

        //バーチャルカメラオブジェクト取得
        vc1Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM1);
        vc2Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM2);

        //バーチャルカメラ取得
        Cinemachine.CinemachineVirtualCamera vc1 = vc1Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        Cinemachine.CinemachineVirtualCamera vc2 = vc2Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();

        //LookAt設定
        vc1.LookAt = playerObj.transform;
        vc2.LookAt = playerObj.transform;

        //位置設定
        Vector3 pos;
        pos = playerObj.transform.position;
        pos.y += 30.0f;
        vc1Obj.transform.position = pos + playerObj.transform.forward * 150.0f;
        pos = playerObj.transform.position;
        pos.y += 3.0f;
        vc2Obj.transform.position = pos + playerObj.transform.forward * 4.0f;

    }

    // タイムライン停止実行
    public override void OnGraphStop(Playable playable)
    {
        //バーチャルカメラのSetActive OFF
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM1, false);
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM2, false);
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
