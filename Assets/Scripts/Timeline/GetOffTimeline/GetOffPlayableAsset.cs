using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class GetOffPlayableAsset : PlayableAsset
{
    public Cinemachine.CinemachineVirtualCamera getOffvcam1;  //バーチャルカメラ1
    public GameObject getOffDolly;                            //ドリー

    public string playerPath;                           //プレイヤーパス
    public string mainCameraPath;                       //メインカメラパス

    public float dollyIntervalTime;    //ドリーの刻む時間
    public float dollyIntervalPos;     //ドリーの刻む位置

    //Timeline再生開始時に１度呼ばれる
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        // もともとある、CreatePlayableメソッド内で、PlayableBehaviourを以下のようにインスタンス化
        var behaviour = new GetOffPlayableBehaviour();

        // Resolveメソッドを利用して、charaObjectの参照を実行時に解決します。
        behaviour.getOffvcam1 = getOffvcam1;
        behaviour.getOffDolly = getOffDolly;
        behaviour.playerPath = playerPath;
        behaviour.mainCameraPath = mainCameraPath;
        behaviour.dollyIntervalTime = dollyIntervalTime;
        behaviour.dollyIntervalPos = dollyIntervalPos;

        return ScriptPlayable<GetOffPlayableBehaviour>.Create(graph, behaviour);
    }
}
