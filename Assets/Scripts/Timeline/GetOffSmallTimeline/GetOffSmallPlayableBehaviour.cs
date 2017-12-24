using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class GetOffSmallPlayableBehaviour : PlayableBehaviour
{
    private GameObject playerObj;       //プレイヤーオブジェ
    public string playerPath;           //プレイヤーパス

    private VirtualCameraManager virtualCameraManager;      //バーチャルカメラマネージャ
    public string virtualCameraManagerPath;                 //バーチャルカメラマネージャパス

    private GameObject vc1Obj;          //バーチャルカメラオブジェクト1
    private GameObject vc2Obj;          //バーチャルカメラオブジェクト2

    private TimelineManager timelineManager;    //タイムラインマネージャー
    public string timelineManagerPath;          //タイムラインマネージャーパス

    private GameObject awaitObj;        //待ち受け状態の人オブジェ
    private Human[] getOffObj;          //下車の人オブジェ

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //各オブジェクト取得    
        playerObj = GameObject.Find(playerPath);        //プレイヤーオブジェクト
        virtualCameraManager = GameObject.Find(virtualCameraManagerPath).GetComponent<VirtualCameraManager>();//バーチャルカメラマネージャ
        timelineManager = GameObject.Find(timelineManagerPath).GetComponent<TimelineManager>();               //タイムラインマネージャー

        //待ち受けいる人オブジェクト取得
        awaitObj = playerObj.GetComponent<Player>().awaitHumanObj;

        //「下車」状態のオブジェクトを取得
        getOffObj = playerObj.GetComponent<Player>().RidePassengerObj;


        //バーチャルカメラのSetActive ON
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFFSMALL_VCAM1, true);
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFFSMALL_VCAM2, true);

        //バーチャルカメラオブジェクト取得
        vc1Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFFSMALL_VCAM1);
        vc2Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFFSMALL_VCAM2);

        //バーチャルカメラ取得
        Cinemachine.CinemachineVirtualCamera vc1 = vc1Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        Cinemachine.CinemachineVirtualCamera vc2 = vc2Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();

        //LookAt設定
        vc1.LookAt = getOffObj[0].transform;
        vc2.LookAt = awaitObj.transform;

        //位置設定
        Vector3 pos, direction;

        direction = playerObj.transform.position - awaitObj.transform.position;
        direction = direction.normalized;
        pos = awaitObj.transform.position + Quaternion.Euler(0.0f, -45.0f, 0.0f) * (direction * 3.0f);
        pos.y += 2.0f;
        vc1Obj.transform.position = pos;

        pos = awaitObj.transform.position + Quaternion.Euler(0.0f, -20.0f, 0.0f) * (direction * 2.5f);
        pos.y += 1.0f;
        vc2Obj.transform.position = pos;
    }

    // タイムライン停止実行
    public override void OnGraphStop(Playable playable)
    {
        //バーチャルカメラのSetActive OFF
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFFSMALL_VCAM1, false);
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFFSMALL_VCAM2, false);

        //タイムラインの状態を「終わり」に
        timelineManager.SetStateType(TimelineManager.STATETYPE.TIMELINE_END);
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
