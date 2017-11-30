using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TestPlayableAsset : PlayableAsset
{
    public Cinemachine.CinemachineVirtualCamera vcam1;  //Vカメラ
    public string playerPath;                           //プレイヤーパス
    public string mainCameraPath;                       //メインカメラパス

    //Timeline再生開始時に１度呼ばれる
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        // もともとある、CreatePlayableメソッド内で、PlayableBehaviourを以下のようにインスタンス化
        var behaviour = new TestPlayableBehaviour();

        // Resolveメソッドを利用して、charaObjectの参照を実行時に解決します。
        behaviour.vcamCamera1 = vcam1;
        behaviour.playerPath = playerPath;
        behaviour.mainCameraPath = mainCameraPath;

        return ScriptPlayable<TestPlayableBehaviour>.Create(graph, behaviour);
    }
}
