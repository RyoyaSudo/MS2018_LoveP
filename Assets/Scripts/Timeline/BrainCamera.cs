using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainCamera : MonoBehaviour {

    private TimelineManager timelineManagerObj;              //タイムラインマネージャーオブジェ
    [SerializeField] private string timelineManagerPath ;    //タイムラインマネージャーパス

    private GameObject gameMaineCameraObj;                   //ゲームメインカメラオブジェ
    [SerializeField] private string gameMainCameraPath;      //ゲームメインカメラパス

    private Vector3 blendStartPos;
    private Vector3 blendEndPos;
    private Quaternion blendStartRotation;
    private Quaternion blendEndRotation;
    private float blendMoveRate;
    private float blendCnt;
    public float blendTime;
    public float waitTime;

	// 初期化
	void Start ()
    {
        timelineManagerObj = GameObject.Find(timelineManagerPath).GetComponent<TimelineManager>();
        gameMaineCameraObj = GameObject.Find(gameMainCameraPath);
        SetActive(false);
        blendCnt = 0;
    }
	
	// 更新
	void Update ()
    {
        //タイムラインの状態
        switch (timelineManagerObj.stateType)
        {
            case TimelineManager.STATETYPE.TIMELINE_NONE:
                break;

            case TimelineManager.STATETYPE.TIMELINE_START:
                break;

            case TimelineManager.STATETYPE.TIMELINE_END:
                {
                    timelineManagerObj.SetStateType(TimelineManager.STATETYPE.TIMELINE_BLENDING);
                }
                break;

            case TimelineManager.STATETYPE.TIMELINE_BLENDING:
                {
                    //指定時間がたつと
                    if (blendCnt >= blendTime+waitTime)
                    {
                        timelineManagerObj.SetStateType(TimelineManager.STATETYPE.TIMELINE_NONE);
                        SetActive(false);
                        blendCnt = 0;
                    }
                    else
                    {
                        //ブレンド
                        blendCnt += Time.deltaTime;
                        transform.position = Vector3.Lerp(blendStartPos, blendEndPos, blendMoveRate * blendCnt);
                        transform.rotation = Quaternion.Lerp(blendStartRotation, blendEndRotation, blendMoveRate * blendCnt);
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// CameraのON,OFF
    /// </summary>
    /// <param name="active">
    /// ONかOFFか
    /// </param>
    public void SetActive(bool active)
    {
        GetComponent<Camera>().enabled = active;
    }

    /// <summary>
    /// Blendの設定
    /// </summary>
    public void SetBlending ()
    {
        blendStartPos = transform.position;                     //開始位置
        blendStartRotation = transform.rotation;
        blendEndPos = gameMaineCameraObj.transform.position;    //終了位置
        blendEndRotation = gameMaineCameraObj.transform.rotation;
        blendMoveRate = 1.0f / blendTime;                       //移動割合
    }
}
