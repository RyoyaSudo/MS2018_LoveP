using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class ChangePlayableAsset : PlayableAsset
{
    public string playerPath;               //プレイヤーパス
    public string virtualCameraManagerPath; //バーチャルカメラマネージャパス
    public GameObject changeDolly;          //チェンジドリーオブジェ
    public float dollyIntervalTime;         //ドリーの刻む時間
    public float dollyIntervalPos;          //ドリーの刻む位置
    public string timelineManagerPath;      //タイムラインマネージャーパス

    //Timeline再生開始時に１度呼ばれる
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        // もともとある、CreatePlayableメソッド内で、PlayableBehaviourを以下のようにインスタンス化
        var behaviour = new ChangePlayableBehaviour();

        // Resolveメソッドを利用して、charaObjectの参照を実行時に解決します。
        behaviour.virtualCameraManagerPath = virtualCameraManagerPath;
        behaviour.playerPath = playerPath;
        behaviour.changeDolly = changeDolly;
        behaviour.dollyIntervalTime = dollyIntervalTime;
        behaviour.dollyIntervalPos = dollyIntervalPos;
        behaviour.timelineManagerPath = timelineManagerPath;

        return ScriptPlayable<ChangePlayableBehaviour>.Create(graph, behaviour);
    }
}
