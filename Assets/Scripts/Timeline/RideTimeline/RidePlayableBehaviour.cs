using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class RidePlayableBehaviour : PlayableBehaviour
{
    private Player playerObj;               //プレイヤーオブジェ
    public string playerPath;               //プレイヤーパス
    private PlayerVehicle playerVehicle;    //プレイヤー車両オブジェクト

    private GameObject mainCameraObj;       //メインカメラオブジェ
    public string mainCameraPath;           //メインカメラパス

    private GameObject vc1Obj;              //バーチャルカメラオブジェクト1
    private GameObject vc2Obj;              //バーチャルカメラオブジェクト2

    private VirtualCameraManager virtualCameraManager;      //バーチャルカメラマネージャ
    public string virtualCameraManagerPath;                 //バーチャルカメラマネージャパス

    private GameObject passengerObj ;   //乗客オブジェ

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //各オブジェクト取得    
        playerObj = GameObject.Find(playerPath).GetComponent<Player>();                                       //プレイヤーオブジェクト    
        playerVehicle = playerObj.GetComponent<PlayerVehicle>();                                              //プレイヤー車両オブジェクト     
        mainCameraObj = GameObject.Find(mainCameraPath);                                                      //カメラオブジェクト      
        passengerObj = playerObj.lastRideHuman.gameObject;                                                    //乗客   
        virtualCameraManager = GameObject.Find(virtualCameraManagerPath).GetComponent<VirtualCameraManager>();//バーチャルカメラマネージャ

        //バーチャルカメラのSetActive ON
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.RIDE_VCAM1, true);
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.RIDE_VCAM2, true);

        //バーチャルカメラオブジェクト取得
        vc1Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.RIDE_VCAM1);
        vc2Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.RIDE_VCAM2);

        //バーチャルカメラ取得
        Cinemachine.CinemachineVirtualCamera vc1 = vc1Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        Cinemachine.CinemachineVirtualCamera vc2 = vc2Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();

        //LookAt設定
        vc1.LookAt = passengerObj.transform;

        //位置設定
        Vector3 pos;
        pos = playerObj.transform.position;
        switch (playerVehicle.VehicleType)
        {
            case PlayerVehicle.Type.BIKE:
                pos.y += 2.0f;
                break;

            case PlayerVehicle.Type.CAR:
                pos.y += 4.0f;
                break;

            case PlayerVehicle.Type.BUS:
                pos.y += 4.0f;
                break;
        }
        vc1Obj.transform.position = pos;
        vc1Obj.transform.position += passengerObj.transform.forward * 6.0f;

        vc2Obj.transform.position = mainCameraObj.transform.position;
        vc2Obj.transform.rotation = mainCameraObj.transform.rotation;
    }

    // タイムライン停止実行
    public override void OnGraphStop(Playable playable)
    {
        //バーチャルカメラのSetActive OFF
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.RIDE_VCAM1, false);
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.RIDE_VCAM2, false);
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
