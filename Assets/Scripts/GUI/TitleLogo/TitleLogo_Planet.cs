using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogo_Planet : TweenAnimation
{

    [SerializeField] float moveTime;
    [SerializeField] float delayTime;

    [SerializeField] TweenAnimation airplaneObj;
    [SerializeField] TweenAnimation rocketObj;
    [SerializeField] TweenAnimation earthObj;
    [SerializeField] TweenAnimation cloudRightTopObj;
    [SerializeField] TweenAnimation cloudLeftTopObj;
    [SerializeField] TweenAnimation cloudLeftBottomObj;

    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        Start,
        PlanetPlay
    }

    // iTween用のハッシュテーブル各種
    Hashtable startHash;

    // 初期値の保持
    Transform origin;

    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        startHash = new Hashtable()
        {
            { "x" , 0.85f } ,
            { "y" , 0.85f } ,
            { "time", moveTime },
            { "delay", delayTime },
            { "easetype" , iTween.EaseType.easeInOutBack },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.PlanetPlay },
        };
    }

    /// <summary>
    /// 再生処理
    /// </summary>
    public override void Play()
    {
        Init();
        SetState(State.Start);
    }

    /// <summary>
    /// 一時停止処理
    /// </summary>
    public override void Pause()
    {
        SetState(State.Pause);
    }

    /// <summary>
    /// 停止処理
    /// </summary>
    public override void Stop()
    {
        SetState(State.Stop);
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    protected override void Init()
    {
        transform.position = origin.position;
        transform.rotation = origin.rotation;
        transform.localScale = origin.localScale;
    }

    /// <summary>
    /// 状態設定処理
    /// </summary>
    /// <param name="param">設定したい状態</param>
    void SetState(State param)
    {
        switch (param)
        {
            case State.Stop:
                iTween.Stop(gameObject);
                break;

            case State.Pause:
                iTween.Pause(gameObject);
                break;

            case State.Start:
                iTween.ScaleTo(gameObject, startHash);
                //iTween.PunchScale(gameObject, startHash);
                break;
            case State.PlanetPlay:
                {
                    airplaneObj.Play();
                    rocketObj.Play();
                    cloudRightTopObj.Play();
                    cloudLeftTopObj.Play();
                    cloudLeftBottomObj.Play();
                    earthObj.Play();
                    break;
                }
        }
    }
}
