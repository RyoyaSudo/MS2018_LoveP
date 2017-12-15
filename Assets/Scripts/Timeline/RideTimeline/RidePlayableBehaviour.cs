using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class RidePlayableBehaviour : PlayableBehaviour
{
    private Player playerObj;           //プレイヤーオブジェ
    public string playerPath;               //プレイヤーパス
    private PlayerVehicle playerVehicle;    //プレイヤー車両オブジェクト

    private GameObject mainCameraObj;   //メインカメラオブジェ
    public string mainCameraPath;       //メインカメラパス

    public Cinemachine.CinemachineVirtualCamera rideCamera1; //バーチャルカメラ1
    public Cinemachine.CinemachineVirtualCamera rideCamera2; //バーチャルカメラ2

    private GameObject passengerObj ;   //乗客オブジェ

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //プレイヤーオブジェクト取得
        playerObj = GameObject.Find(playerPath).GetComponent<Player>();

        //プレイヤー車両オブジェクト取得
        playerVehicle = playerObj.GetComponent<PlayerVehicle>();

        //カメラオブジェクト取得
        mainCameraObj = GameObject.Find(mainCameraPath);

        // 乗客取得( テスト )
        passengerObj = playerObj.lastRideHuman.gameObject;

        // HACK : 違う取得の方法を試します
        //今いる人オブジェクト取得
        //GameObject[] humanObj = GameObject.FindGameObjectsWithTag("Human");

        ////乗車の人オブジェクトを探す
        //for ( int nCnt = 0; nCnt < humanObj.Length; nCnt++ )
        //{
            //if (humanObj[nCnt].GetComponent<Human>().CurrentStateType == Human.STATETYPE.RIDE)
            //{
                //passengerObj = humanObj[nCnt];
            //}
        //}

        rideCamera2.transform.position = mainCameraObj.transform.position;
        rideCamera2.transform.rotation = mainCameraObj.transform.rotation;
    }

    // タイムライン停止実行
    public override void OnGraphStop(Playable playable)
    {
        //LookAt設定
        rideCamera1.LookAt = passengerObj.transform;

        Vector3 pos;
        pos = playerObj.transform.position;

        switch( playerVehicle.VehicleType )
        {
            case PlayerVehicle.Type.BIKE:
                pos.y += 2;
                break;

            case PlayerVehicle.Type.CAR:
                pos.y += 4;
                break;

            case PlayerVehicle.Type.BUS:
                pos.y += 4;
                break;

        }

        rideCamera1.transform.position = pos;
        rideCamera1.transform.position += passengerObj.transform.forward * 6.0f;
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
