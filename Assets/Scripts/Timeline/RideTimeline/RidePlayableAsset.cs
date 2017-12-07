using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class RidePlayableAsset : PlayableAsset
{
    public Cinemachine.CinemachineVirtualCamera rideCamera1; //バーチャルカメラ1
    public Cinemachine.CinemachineVirtualCamera rideCamera2; //バーチャルカメラ2

    public string playerPath;                           //プレイヤーパス
    public string mainCameraPath;                       //メインカメラパス

    //Timeline再生開始時に１度呼ばれる
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        // もともとある、CreatePlayableメソッド内で、PlayableBehaviourを以下のようにインスタンス化
        var behaviour = new RidePlayableBehaviour();

        // Resolveメソッドを利用して、charaObjectの参照を実行時に解決します。
        behaviour.rideCamera1 = rideCamera1;
        behaviour.rideCamera2 = rideCamera2;
        behaviour.playerPath = playerPath;
        behaviour.mainCameraPath = mainCameraPath;

        return ScriptPlayable<RidePlayableBehaviour>.Create(graph, behaviour);
    }
}
