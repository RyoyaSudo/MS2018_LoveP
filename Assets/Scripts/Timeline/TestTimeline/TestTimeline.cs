using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// テスト用タイムラインクラス
/// タイムライン作成の際に参考にして頂ければ。
/// </summary>
public class TestTimeline : Timeline {

    /// <summary>
    /// アニメーショントラックパス
    /// 複数になる場合、固有の名前を変数名、トラック名共につけること。
    /// 分かりやすく！
    /// </summary>
    [SerializeField] private string animationTrackPath;

    /// <summary>
    /// シネマシーントラックパス
    /// 上記と同様！
    /// </summary>
    [SerializeField] private string cinemachineTrackPath;

    /// <summary>
    /// プレイヤーオブジェクト
    /// 今回のタイムラインではプレイヤーが必要なため取得する
    /// </summary>
    private GameObject playerObj;
    [SerializeField] string playerPath;

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
        playerObj = GameObject.Find( playerPath );
        cameraObj = GameObject.Find( cameraPath );

        // バインド処理
        BindTrack( playerObj , animationTrackPath );
        BindTrack( cameraObj , cinemachineTrackPath );
    }
}
