using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class StarShiftPlayableAsset : PlayableAsset
{
    public Cinemachine.CinemachineVirtualCamera starShiftvcam1;  //バーチャルカメラ1

    public string playerPath;                                    //プレイヤーパス

    //Timeline再生開始時に１度呼ばれる
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        // もともとある、CreatePlayableメソッド内で、PlayableBehaviourを以下のようにインスタンス化
        var behaviour = new StarShiftPlayableBehaviour();

        // Resolveメソッドを利用して、charaObjectの参照を実行時に解決します。
        behaviour.starShiftvcam1 = starShiftvcam1;
        behaviour.playerPath = playerPath;

        return ScriptPlayable<StarShiftPlayableBehaviour>.Create(graph, behaviour);
    }
}
