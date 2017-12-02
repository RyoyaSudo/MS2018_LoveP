using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

/// <summary>
/// タイムライン管理クラス
/// </summary>
public class TimelineManager : MonoBehaviour {

    public GameObject[] timelinePrefab;                     //タイムラインプレファブ
    private Timeline [] timelineObj;                        //タイムラインオブジェ

    //タイムラインのプレファブと同じ名前を入力
    [SerializeField] private string riedPath;               //乗車プレファブのパス
    [SerializeField] private string getoffPath;             //下車プレファブのパス

    private GameObject playerObj;                           //プレイヤーオブジェ
    [SerializeField] private string playerPath;             //プレイヤーパス

    private GameObject cameraObj;                           //カメラオブジェ
    [SerializeField] private string cameraPath;             //カメラパス

    [SerializeField] private string animationTrackPath;     //アニメーショントラックパス
    [SerializeField] private string cinemachineTrackPath;   //シネマシーントラックパス

    // 初期化
    void Start ()
    {
        timelineObj = new Timeline[timelinePrefab.Length];

        //プレイヤーオブジェクト取得
        playerObj = GameObject.Find(playerPath);

        //カメラオブジェクト取得
        cameraObj = GameObject.Find(cameraPath);

        //生成
        for ( int nCnt = 0; nCnt < timelinePrefab.Length; nCnt++ )
        {
            TimelineCreate(nCnt);
        }
    }

    //更新
    void Update()
    {
        //タイムラインテスト用
        if (Input.GetKeyDown("9"))
        {
            for (int nCnt = 0; nCnt < timelinePrefab.Length; nCnt++)
            {
                //乗車
                if (timelineObj[nCnt].name == riedPath)
                {
                    //再生
                    timelineObj[nCnt].Play();
                }
            }
        }
        if (Input.GetKeyDown("8"))
        {
            for (int nCnt = 0; nCnt < timelinePrefab.Length; nCnt++)
            {
                //下車
                if (timelineObj[nCnt].name == getoffPath)
                {
                    //再生
                    timelineObj[nCnt].Play();
                }
            }
        }
    }

    /// <summary>
    /// タイムライン生成
    /// </summary>
    /// <param name="num">
    /// 生成順番
    /// </param>
    void TimelineCreate ( int num )
    {
        //生成
        GameObject timeline = Instantiate(timelinePrefab[num]);
        
        //自分の親を自分にする
        timeline.transform.parent = transform;

        //表示名
        timeline.name =timelinePrefab[num].name;

        //バインド
        //乗車
        if ( timeline.name == riedPath )
        {
            timeline.GetComponent<Timeline>().BindTrack(playerObj,animationTrackPath);
            timeline.GetComponent<Timeline>().BindTrack(cameraObj, cinemachineTrackPath);
        }
        //下車
        else if ( timeline.name == getoffPath )
        {
            timeline.GetComponent<Timeline>().BindTrack(cameraObj, cinemachineTrackPath);
        }
        else
        {
            Debug.Log("パスがないです");
        }

        //生成したタイムラインを保存
        timelineObj[num] = timeline.GetComponent<Timeline>();
    }
}
