using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;

public abstract class Timeline : MonoBehaviour
{
    //PlayableDirector
    protected PlayableDirector director;

    private GameObject virtualCameraParent;
    [SerializeField] string virtualCameraParentPath;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        //PlayableDirector取得
        director = GetComponent<PlayableDirector>();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        virtualCameraParent = GameObject.Find( virtualCameraParentPath );

        CinemachineSetting();
        Bind();
    }

    /// <summary>
    /// Trackにバインド
    /// </summary>
    /// <param name="obj">
    /// バインドするオブジェクト
    /// </param>
    /// <param name="path">
    /// Track名
    /// </param>
    public void BindTrack( GameObject obj , string path )
    {
        //Timelineからパスのトラックへの参照を取得して
        var binding = director.playableAsset.outputs.First(c => c.streamName == path);

        //そこにオブジェクトをバインドする
        director.SetGenericBinding( binding.sourceObject , obj );
    }

    /// <summary>
    /// 各トラックにオブジェクトをバインドする処理。
    /// 必ず派生先初期化時に呼び出すこと！
    /// </summary>
    public abstract void Bind();

    /// <summary>
    /// プレハブへの設定処理
    /// </summary>
    public void CinemachineSetting()
    {
        foreach( var output in director.playableAsset.outputs )
        {
            if( output.streamName == "Cinemachine Track" )
            {
                var cinemachineTrack = output.sourceObject as Cinemachine.Timeline.CinemachineTrack;

                foreach( var clip in cinemachineTrack.GetClips() )
                {
                    if( virtualCameraParent == null )
                    {
                        Debug.LogWarning( "VirtualCameraParent Not To Find" );
                        continue;
                    }

                    var vcObj = virtualCameraParent.transform.Find( clip.displayName ).gameObject;

                    if( vcObj == null )
                    {
                        Debug.LogWarning( "VirtualCameraObject Not To Find" );
                        continue;
                    }

                    var vc = vcObj.GetComponent<Cinemachine.CinemachineVirtualCameraBase>();

                    if( vc == null )
                    {
                        Debug.LogWarning( "VirtualCameraComponent Not To Find" );
                        continue;
                    }

                    var cinemachineShot = clip.asset as Cinemachine.Timeline.CinemachineShot;
                    director.SetReferenceValue( cinemachineShot.VirtualCamera.exposedName , vc );
                }
            }
        }
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    public void Play()
    {
        director.Play();
    }

    /// <summary>
    /// 停止処理
    /// </summary>
    public void Stop()
    {
        director.Stop();
    }

    /// <summary>
    /// 一時停止処理
    /// </summary>
    public void Pause()
    {
        director.Pause();
    }

    /// <summary>
    /// 再生期間
    /// </summary>
    public float Duration()
    {
        // HACK : floatにキャスト
        return (float)director.duration;
    }

    /// <summary>
    /// 状態
    /// </summary>
    public PlayState State()
    {
        return director.state;
    }

}
