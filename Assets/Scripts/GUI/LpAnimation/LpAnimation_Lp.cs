using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LpAnimation_Lp : TweenAnimation 
{

    [SerializeField] float moveTime;

    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        Start,
        Default
    }

    State CurrentState;

    // iTween用のハッシュテーブル各種
    Hashtable startHash;

    // 初期値の保持
    Transform origin;

    public bool lpAnimFrag;

    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        startHash = new Hashtable()
        {
            { "x" , 0.3f } ,
            { "y" , 0.3f } ,
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.Default },
        };
    }

    /// <summary>
    /// 再生処理
    /// </summary>
    public override void Play()
    {
        Init();
        SetState(State.Start);
        if (CurrentState == State.Start)
        {
            CurrentState = State.Default;
        }

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
        CurrentState = param;
        switch (param)
        {
            case State.Stop:
                iTween.Stop(gameObject);
                break;

            case State.Pause:
                iTween.Pause(gameObject);
                break;

            case State.Start:
                iTween.PunchScale(gameObject, startHash);
                break;

            case State.Default:
                break;
        }
    }
}
