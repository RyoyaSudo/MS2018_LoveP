using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class RideTimeline : Timeline {

    /// <summary>
    /// シネマシーントラックパス
    /// 上記と同様！
    /// </summary>
    [SerializeField] private string cinemachineTrackPath;

    /// <summary>
    /// カメラオブジェクト
    /// メインカメラに対して処理を実行するため取得する
    /// </summary>
    private GameObject cameraObj;

    [SerializeField] string cameraPath;

    /// <summary>
    /// オブジェクトのバインド処理
    /// </summary>
    public override void Bind()
    {
        // バインド対象の検索
        // 必ず検索をかけてからバインドを行うこと
        cameraObj = GameObject.Find(cameraPath);

        // バインド処理
        BindTrack(cameraObj, cinemachineTrackPath);
    }

}
