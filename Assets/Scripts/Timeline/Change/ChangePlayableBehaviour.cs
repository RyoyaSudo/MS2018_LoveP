using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class ChangePlayableBehaviour : PlayableBehaviour
{
    private Player playerObj;               //プレイヤーオブジェ
    public string playerPath;               //プレイヤーパス
    private PlayerVehicle playerVehicle;    //プレイヤー車両オブジェクト

    private GameObject vc1Obj;              //バーチャルカメラオブジェクト1

    private VirtualCameraManager virtualCameraManager;      //バーチャルカメラマネージャ
    public string virtualCameraManagerPath;                 //バーチャルカメラマネージャパス

    private TimelineManager timelineManager;    //タイムラインマネージャー
    public string timelineManagerPath;          //タイムラインマネージャーパス

    public GameObject changeDolly;                            //チェンジドリーオブジェ
    private Cinemachine.CinemachineTrackedDolly trackDolly1;  //トラックドリー1
    public float dollyIntervalTime;    //ドリーの刻む時間
    public float dollyIntervalPos;     //ドリーの刻む位置
    private float pathPosCnt;          //パスの位置カウント

    private GameObject awaitObj;        //待ち受け状態の人オブジェ

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //各オブジェクト取得    
        playerObj = GameObject.Find(playerPath).GetComponent<Player>();                                       //プレイヤーオブジェクト    
        playerVehicle = playerObj.GetComponent<PlayerVehicle>();                                              //プレイヤー車両オブジェクト     
        virtualCameraManager = GameObject.Find(virtualCameraManagerPath).GetComponent<VirtualCameraManager>();//バーチャルカメラマネージャ
        timelineManager = GameObject.Find(timelineManagerPath).GetComponent<TimelineManager>();               //タイムラインマネージャー

        // HACK : 違う取得の方法を試します
        //今いる人オブジェクト取得
        GameObject[] humanObj = GameObject.FindGameObjectsWithTag("Human");

        //「待ち受け」状態のオブジェクトを探す
        for (int nCnt = 0; nCnt < humanObj.Length; nCnt++)
        {
            if (humanObj[nCnt].GetComponent<Human>().CurrentStateType == Human.STATETYPE.AWAIT)
            {
                awaitObj = humanObj[nCnt];
            }
        }

        //バーチャルカメラのSetActive ON
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.CHANGE_VCAM1, true);

        //バーチャルカメラオブジェクト取得
        vc1Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.CHANGE_VCAM1);

        //バーチャルカメラ取得
        Cinemachine.CinemachineVirtualCamera vc1 = vc1Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();

        //LookAt設定
        vc1.LookAt = playerObj.transform;

        //Follwの設定
        vc1.Follow = playerObj.transform;

        //位置設定
        Vector3 pos;
        pos = playerObj.transform.position;
        pos.y += 3.0f;
        changeDolly.transform.position = pos;
        Quaternion rotation;
        rotation = playerObj.transform.rotation;
        changeDolly.transform.rotation = rotation;

        //BodyのCinemachineTrackedDolly取得
        trackDolly1 = vc1.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();

        //パス位置の設定
        trackDolly1.m_PathPosition = 0;
        pathPosCnt = 0;
    }

    // タイムライン停止実行
    public override void OnGraphStop(Playable playable)
    {
        //バーチャルカメラのSetActive OFF
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.CHANGE_VCAM1, false);

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
                trackDolly1.m_PathPosition += dollyIntervalPos;

                trackDolly1.m_PathPosition = Mathf.Min(trackDolly1.m_PathPosition, 6.0f);
            }

            pathPosCnt = f;
        }

        pathPosCnt += Time.deltaTime;
    }
}
