using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TestPlayableBehaviour : PlayableBehaviour
{
    private GameObject playerObject;    //プレイヤーオブジェ
    public string playerPath;           //プレイヤーパス

    private GameObject mainCameraObj;   //メインカメラオブジェ
    public string mainCameraPath;       //メインカメラパス

    public Cinemachine.CinemachineVirtualCamera vcamCamera1;

    private GameObject passengerObj;    //乗客オブジェ


    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //プレイヤーオブジェクト取得
        playerObject = GameObject.Find(playerPath);

        //カメラオブジェクト取得
        mainCameraObj = GameObject.Find(mainCameraPath);


        Vector3 pos;
        Quaternion rot;

        pos = playerObject.transform.position;
        pos += playerObject.transform.forward * 10;
        rot = playerObject.transform.rotation;
        rot.y += 180;

        vcamCamera1.transform.position = pos;
        vcamCamera1.transform.rotation = rot;

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
        Quaternion rot;

        pos = playerObject.transform.position;
        pos += playerObject.transform.forward * 10;
        rot = playerObject.transform.rotation;
        rot.y += 180;
        vcamCamera1.transform.position = pos;

        vcamCamera1.transform.rotation = rot;
    }

    /// <summary>
    /// 乗客オブジェクトセット
    /// </summary>
    /// <param name="obj">
    /// 乗客オブジェクト
    /// </param>
    public void SetPassengerObj ( GameObject obj)
    {
        passengerObj = obj;
    }
}
