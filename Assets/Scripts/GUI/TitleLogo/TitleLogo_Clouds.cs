using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogo_Clouds : TweenAnimation
{
    [SerializeField] float moveTime;
    [SerializeField] State initState;

    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        MoveRight,
        MoveLeft,
    }

    // iTween用のハッシュテーブル各種
    Hashtable moveRightHash;
    Hashtable moveLeftHash;
    Hashtable punchHash;
    Hashtable rotateRightHash;
    Hashtable rotateLeftHash;

    // 初期値の保持
    Transform origin;

    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        moveRightHash = new Hashtable()
        {
            { "x" , -100.0f } ,
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveLeft },
        };

        moveLeftHash = new Hashtable()
        {
            { "x" , 100.0f },
            { "time" , moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveRight },
        };

        punchHash = new Hashtable()
        {
            { "y" , 50.0f },
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
        };

        rotateRightHash = new Hashtable()
        {
            { "z" , 0.11f },
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
        };

        rotateLeftHash = new Hashtable()
        {
            { "z" , -0.11f },
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
        };
    }

    /// <summary>
    /// 再生処理
    /// </summary>
    public override void Play()
    {
        Init();
        SetState(initState);
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

            case State.MoveRight:
                iTween.MoveBy(gameObject, moveRightHash);
                iTween.RotateBy(gameObject, rotateRightHash);
                break;

            case State.MoveLeft:
                iTween.MoveBy(gameObject, moveLeftHash);
                iTween.RotateBy(gameObject, rotateLeftHash);
                break;
        }
    }
}
