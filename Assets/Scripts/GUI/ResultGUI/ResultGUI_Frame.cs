using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultGUI_Frame : TweenAnimation
{
    [SerializeField] float moveTime;
    // 子オブジェクト
    [SerializeField] TweenAnimation dangleBoyObj;
    [SerializeField] TweenAnimation dangleGirlObj;
    [SerializeField] TweenAnimation resultGirlObj;
    [SerializeField] TweenAnimation resultBoyObj;

    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        MoveDown,
        MoveEnd
    }

    // iTween用のハッシュテーブル各種
    Hashtable moveDownHash;

    // 初期値の保持
    Transform origin;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        moveDownHash = new Hashtable()
        {
            //{ "x" , -480.0f } ,
            { "y" , -2160.0f },
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveEnd },
        };
    }

    /// <summary>
    /// 再生処理
    /// </summary>
    public override void Play()
    {
        Init();
        SetState(State.MoveDown);
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

            case State.MoveDown:
                iTween.MoveBy(gameObject, moveDownHash);
                break;
            case State.MoveEnd:
                dangleBoyObj.Play();
                dangleGirlObj.Play();
                resultGirlObj.Play();
                resultBoyObj.Play();
                break;
        }
    }
}
