using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

// A behaviour that is attached to a playable
public class GetOffPlayableBehaviour : PlayableBehaviour
{
    private GameObject playerObj;       //プレイヤーオブジェ
    public string playerPath;           //プレイヤーパス

    private GameObject mainCameraObj;   //メインカメラオブジェ
    public string mainCameraPath;       //メインカメラパス

    private VirtualCameraManager virtualCameraManager;      //バーチャルカメラマネージャ
    public string virtualCameraManagerPath;                 //バーチャルカメラマネージャパス

    private GameObject vc1Obj;          //バーチャルカメラオブジェクト1
    private GameObject vc2Obj;          //バーチャルカメラオブジェクト2

    private GameObject awaitObj;        //待ち受け状態の人オブジェ
    private GameObject getOffObj;       //下車状態の人オブジェ

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //各オブジェクト取得    
        playerObj = GameObject.Find(playerPath);        //プレイヤーオブジェクト
        mainCameraObj = GameObject.Find(mainCameraPath);//メインカメラオブジェクト
        virtualCameraManager = GameObject.Find(virtualCameraManagerPath).GetComponent<VirtualCameraManager>();//バーチャルカメラマネージャ

        // HACK : 違う取得の方法を試します
        //今いる人オブジェクト取得
        GameObject[] humanObj = GameObject.FindGameObjectsWithTag("Human");

        //「待ち受け」状態のオブジェクトを探す
        for (int nCnt = 0; nCnt < humanObj.Length; nCnt++)
        {
            if (humanObj[nCnt].GetComponent<Human>().CurrentStateType == Human.STATETYPE.AWAIT )
            {
                awaitObj = humanObj[nCnt];
            }
        }

        //「下車」状態のオブジェクトを探す
        for (int nCnt = 0; nCnt < humanObj.Length; nCnt++)
        {
            if (humanObj[nCnt].GetComponent<Human>().CurrentStateType == Human.STATETYPE.GETOFF)
            {
                getOffObj = humanObj[nCnt];
            }
        }


        //バーチャルカメラのSetActive ON
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFF_VCAM1, true);
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFF_VCAM2, true);

        //バーチャルカメラオブジェクト取得
        vc1Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFF_VCAM1);
        vc2Obj = virtualCameraManager.GetVirtualCamera(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFF_VCAM2);

        //バーチャルカメラ取得
        Cinemachine.CinemachineVirtualCamera vc1 = vc1Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        Cinemachine.CinemachineVirtualCamera vc2 = vc2Obj.GetComponent<Cinemachine.CinemachineVirtualCamera>();

        //LookAt設定
        vc1.LookAt = getOffObj.transform;
        vc2.LookAt = awaitObj.transform;

        //位置設定
        Vector3 pos, direction;

        direction = playerObj.transform.position - awaitObj.transform.position;
        direction = direction.normalized;
        pos = awaitObj.transform.position + (-direction * 5.0f);
        pos.y += 3.0f;
        vc1Obj.transform.position = pos;

        pos = awaitObj.transform.position + (direction * 3.0f);
        pos.y += 1.0f;
        vc2Obj.transform.position = pos;
    }

    // タイムライン停止実行
    public override void OnGraphStop(Playable playable)
    {
        //バーチャルカメラのSetActive OFF
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFF_VCAM1, false);
        virtualCameraManager.SetActive(VirtualCamera.VIRTUALCAMERA_TYPE.GETOFF_VCAM2, false);
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
