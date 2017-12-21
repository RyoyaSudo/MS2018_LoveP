using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogo_PushStart : TweenAnimation
{
    [SerializeField] float startTime;
    [SerializeField] float moveTime;


    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        Start,
        Bound,
        MoveUp,
        MoveDown
    }

    // iTween用のハッシュテーブル各種
    Hashtable startHash;
    Hashtable boundHash;
    Hashtable moveUpHash;
    Hashtable moveDownHash;


    // 初期値の保持
    Transform origin;

    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        startHash = new Hashtable()
        {
            { "x" , 2.0f } ,
            //{ "y" , 1.0f } ,
            { "time", startTime },
            { "easetype" , iTween.EaseType.easeInOutBack },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveUp },
        };

        // 各ハッシュの初期化
        boundHash = new Hashtable()
        {
            // { "x" , 1.0f } ,
            { "delay", 1.0f },
            { "y" , 50.0f } ,
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.loop },
        };

        moveUpHash = new Hashtable()
        {
            // { "x" , 1.0f } ,
            //{ "delay", 1.0f },
            { "y" , 15.0f } ,
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveDown },
        };

        moveDownHash = new Hashtable()
        {
            // { "x" , 1.0f } ,
            //{ "delay", 1.0f },
            { "y" , -15.0f } ,
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveUp },
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
                iTween.PunchScale(gameObject, startHash);
                break;
            case State.Bound:
                iTween.PunchPosition(gameObject, boundHash);
                break;
            case State.MoveUp:
                iTween.MoveBy(gameObject, moveUpHash);
                break;
            case State.MoveDown:
                iTween.MoveBy(gameObject, moveDownHash);
                break;
        }
    }
}
