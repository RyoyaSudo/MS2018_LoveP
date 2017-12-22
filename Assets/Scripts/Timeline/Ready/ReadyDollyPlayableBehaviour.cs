using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class ReadyDollyPlayableBehaviour : PlayableBehaviour
{
    private Player playerObj;               //プレイヤーオブジェ
    public string playerPath;               //プレイヤーパス

    private GameObject vc3Obj;              //バーチャルカメラオブジェクト3

    private VirtualCameraManager virtualCameraManager;      //バーチャルカメラマネージャ
    public string virtualCameraManagerPath;                 //バーチャルカメラマネージャパス

    private TimelineManager timelineManager;    //タイムラインマネージャー
    public string timelineManagerPath;          //タイムラインマネージャーパス

    public GameObject readyDolly;                             //レディードリーオブジェ
    private Cinemachine.CinemachineTrackedDolly trackDolly;   //トラックドリー
    public float dollyIntervalTime;    //ドリーの刻む時間
    public float dollyIntervalPos;     //ドリーの刻む位置
    private float pathPosCnt;          //パスの位置カウント

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //各オブジェクト取得    
        playerObj = GameObject.Find(playerPath).GetComponent<Player>();                                       //プレイヤーオブジェクト    
        virtualCameraManager = GameObject.Find(virtualCameraManagerPath).GetComponent<VirtualCameraManager>();//バーチャルカメラマネージャ
        timelineManager = GameObject.Find(timelineManagerPath).GetComponent<TimelineManager>();               //タイムラインマネージャー

        //バーチャルカメラのSetActive ON
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM3, true);

        //バーチャルカメラオブジェクト取得
        vc3Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM3);

        //バーチャルカメラ取得
        Cinemachine.CinemachineVirtualCamera vc3 = vc3Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();

        //LookAt設定
        vc3.LookAt = playerObj.transform;

        //Follwの設定
        vc3.Follow = playerObj.transform;

        //位置設定
        Vector3 pos;
        pos = playerObj.transform.position;
        pos.y += 3.0f;
        readyDolly.transform.position = pos;
        Quaternion rotation;
        rotation = playerObj.transform.rotation;
        readyDolly.transform.rotation = rotation;

        pos = playerObj.transform.position;
        pos.y += 3.0f;
        vc3Obj.transform.position = pos + playerObj.transform.forward * 4.0f;


        //BodyのCinemachineTrackedDolly取得
        trackDolly = vc3.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();

        //パス位置の設定
        trackDolly.m_PathPosition = 0;
        pathPosCnt = 0;
    }

    // タイムライン停止実行
    public override void OnGraphStop(Playable playable)
    {
        //バーチャルカメラのSetActive OFF
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.READY_VCAM3, false);

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
        if (pathPosCnt >= dollyIntervalTime)
        {
            float f;

            for (f = pathPosCnt; f >= dollyIntervalTime; f -= dollyIntervalTime)
            {
                trackDolly.m_PathPosition += dollyIntervalPos;

                trackDolly.m_PathPosition = Mathf.Min(trackDolly.m_PathPosition, 2.0f);
            }

            pathPosCnt = f;
        }

        pathPosCnt += Time.deltaTime;
    }
}
