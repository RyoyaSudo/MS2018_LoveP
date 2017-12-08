using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class GetOffPlayableBehaviour : PlayableBehaviour
{
    private GameObject playerObj;       //プレイヤーオブジェ
    public string playerPath;           //プレイヤーパス

    private GameObject mainCameraObj;   //メインカメラオブジェ
    public string mainCameraPath;       //メインカメラパス

    public GameObject getOffDolly;      //ドリー

    public Cinemachine.CinemachineVirtualCamera getOffvcam1;  //バーチャルカメラ1
    private Cinemachine.CinemachineTrackedDolly trackDolly1;  //トラックドリー1

    public float dollyIntervalTime;    //ドリーの刻む時間
    public float dollyIntervalPos;     //ドリーの刻む位置
    private float pathPosCnt;          //パスの位置カウント

    // タイムライ開始実行
    public override void OnGraphStart(Playable playable)
    {
        //プレイヤーオブジェクト取得
        playerObj = GameObject.Find(playerPath);

        //カメラオブジェクト取得
        mainCameraObj = GameObject.Find(mainCameraPath);

        //BodyのCinemachineTrackedDolly取得
        trackDolly1 = getOffvcam1.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();


        //LookAt設定
        getOffvcam1.LookAt = playerObj.transform;

        //Follow設定
        getOffvcam1.Follow = playerObj.transform;

        //ドリーの位置と回転を設定
        Vector3 pos;
        pos = playerObj.transform.position;
        pos += playerObj.transform.forward * 0.5f;
        //pos.y += 2.0f;
        getOffDolly.transform.position = pos;

        Quaternion rotation;
        rotation = playerObj.transform.rotation;
        //rotation.y += 70.0f;
        getOffDolly.transform.rotation = rotation;

        //パス位置の設定
        trackDolly1.m_PathPosition = 0;
        pathPosCnt = 0;
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

        if( pathPosCnt >= dollyIntervalTime)
        {
            float f;

            for( f = pathPosCnt; f >= dollyIntervalTime; f -= dollyIntervalTime)
            {
                trackDolly1.m_PathPosition += dollyIntervalPos;

                trackDolly1.m_PathPosition = Mathf.Min(trackDolly1.m_PathPosition , 3.0f );
            }

            pathPosCnt = f;
        }

        pathPosCnt += Time.deltaTime;


        //trackDolly1.m_PathPosition = pathPosCnt;
        //
        //if (pathPosCnt >= 3.0f )
        //{
        //    pathPosCnt = 3.0f;
        //}
        //else
        //{
        //    pathPosCnt += Time.deltaTime;
        //}
    }
}
