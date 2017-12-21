using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class GetOffPlayableAsset : PlayableAsset
{
    public string virtualCameraManagerPath;             //バーチャルカメラマネージャパス
    public string timelineManagerPath;                  //タイムラインマネージャーパス

    public string playerPath;                           //プレイヤーパス

    //Timeline再生開始時に１度呼ばれる
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        // もともとある、CreatePlayableメソッド内で、PlayableBehaviourを以下のようにインスタンス化
        var behaviour = new GetOffPlayableBehaviour();

        // Resolveメソッドを利用して、charaObjectの参照を実行時に解決します。
        behaviour.virtualCameraManagerPath = virtualCameraManagerPath;
        behaviour.playerPath = playerPath;
        behaviour.timelineManagerPath = timelineManagerPath;

        return ScriptPlayable<GetOffPlayableBehaviour>.Create(graph, behaviour);
    }
}
